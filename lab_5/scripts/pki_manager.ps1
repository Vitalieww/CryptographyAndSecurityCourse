<#
.SYNOPSIS
    Internal PKI Management Script using OpenSSL
    Laboratory Work No. 5 - Public Key Infrastructure & Digital Signatures
.DESCRIPTION
    Complies with requirements:
    - Root CA private key: RSA 4096-bit, self-signed certificate: 3650 days (10 years)
    - User private key: RSA >= 2048-bit, certificate validity: 365 days (1 year)
    - Certificate issuance, revocation (CRL generation and checking), document signing and verification.
#>

param (
    [string]$BaseDir = "$PSScriptRoot\..\pki_store"
)

$ErrorActionPreference = "Stop"

$caDir = "$BaseDir\ca"
$certsDir = "$caDir\certs"
$crlDir = "$caDir\crl"
$newCertsDir = "$caDir\newcerts"
$privateDir = "$caDir\private"
$usersDir = "$BaseDir\users"
$cnfFile = "$caDir\openssl.cnf"
$templateFile = "$PSScriptRoot\openssl.cnf.template"

function Write-Header {
    param([string]$Text)
    Write-Host "`n========================================================" -ForegroundColor Cyan
    Write-Host "  $Text" -ForegroundColor Yellow
    Write-Host "========================================================" -ForegroundColor Cyan
}

function Ensure-OpenSsl {
    try {
        $version = openssl version 2>$null
        if (-not $version) {
            Write-Error "OpenSSL is not installed or not in PATH."
        }
    } catch {
        Write-Error "OpenSSL command execution failed. Ensure OpenSSL is installed."
    }
}

function Init-PkiStructure {
    New-Item -ItemType Directory -Force -Path $caDir, $certsDir, $crlDir, $newCertsDir, $privateDir, $usersDir | Out-Null
    
    if (-not (Test-Path "$caDir\index.txt")) {
        New-Item -ItemType File -Path "$caDir\index.txt" -Force | Out-Null
    }
    if (-not (Test-Path "$caDir\serial")) {
        "1000`n" | Set-Content -Path "$caDir\serial" -NoNewline
    }
    if (-not (Test-Path "$caDir\crlnumber")) {
        "1000`n" | Set-Content -Path "$caDir\crlnumber" -NoNewline
    }
    
    $normalizedCaPath = ($caDir -replace '\\', '/')
    $templateContent = Get-Content -Raw $templateFile
    $cnfContent = $templateContent.Replace("__CA_DIR__", $normalizedCaPath)
    $cnfContent | Set-Content -Path $cnfFile -Force
}

function Initialize-CA {
    Write-Header "Initialize Root Certified Authority (CA)"
    Init-PkiStructure

    $caKey = "$privateDir\ca.key"
    $caCrt = "$certsDir\ca.crt"

    if (Test-Path $caCrt) {
        Write-Host "CA already initialized!" -ForegroundColor Yellow
        $recreate = Read-Host "Do you want to re-initialize and wipe existing CA data? (y/N)"
        if ($recreate -ne "y" -and $recreate -ne "Y") {
            Write-Host "Aborted CA initialization." -ForegroundColor Gray
            return
        }
        Remove-Item -Recurse -Force "$caDir\*"
        Init-PkiStructure
    }

    Write-Host "Generating CA Root Private Key (RSA 4096-bit)..." -ForegroundColor Green
    & openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out $caKey

    Write-Host "Generating Self-Signed CA Certificate (Validity: 3650 days / 10 years)..." -ForegroundColor Green
    $subj = "/C=MD/ST=Chisinau/L=Chisinau/O=Technical University of Moldova/OU=FAF/CN=UTM-FAF-Root-CA"
    & openssl req -config $cnfFile -key $caKey -new -x509 -days 3650 -sha256 -extensions v3_ca -out $caCrt -subj $subj

    Write-Host "Generating Initial Certificate Revocation List (CRL)..." -ForegroundColor Green
    $caCrl = "$crlDir\ca.crl"
    & openssl ca -config $cnfFile -gencrl -out $caCrl

    Write-Host "`nRoot CA successfully initialized!" -ForegroundColor Green
    Write-Host "CA Key:         $caKey" -ForegroundColor White
    Write-Host "CA Certificate: $caCrt" -ForegroundColor White
    Write-Host "CA CRL:         $caCrl" -ForegroundColor White
}

function Issue-UserCertificate {
    Write-Header "Issue User Private Key & Certificate"
    $caCrt = "$certsDir\ca.crt"
    if (-not (Test-Path $caCrt)) {
        Write-Host "Root CA is not initialized! Please initialize the CA first (Option 1)." -ForegroundColor Red
        return
    }

    $username = Read-Host "Enter username/entity name (e.g., alice, bob)"
    if ([string]::IsNullOrWhiteSpace($username)) {
        Write-Host "Invalid username." -ForegroundColor Red
        return
    }
    $username = $username.Trim()

    $keyBits = Read-Host "Enter RSA key dimension (2048, 3072, 4096) [Default: 2048]"
    if ([string]::IsNullOrWhiteSpace($keyBits)) { $keyBits = "2048" }
    if ($keyBits -ne "2048" -and $keyBits -ne "3072" -and $keyBits -ne "4096") {
        Write-Host "Key size must be minimum 2048 bits. Setting to 2048." -ForegroundColor Yellow
        $keyBits = "2048"
    }

    $userPath = "$usersDir\$username"
    New-Item -ItemType Directory -Force -Path $userPath | Out-Null

    $userKey = "$userPath\$username.key"
    $userCsr = "$userPath\$username.csr"
    $userCrt = "$userPath\$username.crt"
    $userPub = "$userPath\$username.pub"

    Write-Host "`n1. Generating User RSA Private Key ($keyBits-bit)..." -ForegroundColor Green
    & openssl genpkey -algorithm RSA -pkeyopt "rsa_keygen_bits:$keyBits" -out $userKey

    Write-Host "2. Creating Certificate Signing Request (CSR)..." -ForegroundColor Green
    $subj = "/C=MD/ST=Chisinau/O=Technical University of Moldova/OU=FAF/CN=$username"
    & openssl req -config $cnfFile -new -key $userKey -out $userCsr -subj $subj

    Write-Host "3. CA Signing User Certificate (Validity: 365 days)..." -ForegroundColor Green
    & openssl ca -config $cnfFile -batch -extensions usr_cert -days 365 -notext -md sha256 -in $userCsr -out $userCrt

    Write-Host "4. Extracting Public Key for easy signature verification..." -ForegroundColor Green
    & openssl x509 -in $userCrt -pubkey -noout -out $userPub

    Write-Host "`nUser Certificate & Key successfully issued!" -ForegroundColor Green
    Write-Host "Private Key: $userKey" -ForegroundColor White
    Write-Host "Certificate: $userCrt" -ForegroundColor White
    Write-Host "Public Key:  $userPub" -ForegroundColor White
}

function Revoke-UserCertificate {
    Write-Header "Revoke User Certificate"
    $username = Read-Host "Enter username to revoke"
    $userCrt = "$usersDir\$username\$username.crt"

    if (-not (Test-Path $userCrt)) {
        Write-Host "Certificate not found at $userCrt" -ForegroundColor Red
        return
    }

    Write-Host "Revocation reasons: keyCompromise, CACompromise, affiliationChanged, superseded, cessationOfOperation, unspecified" -ForegroundColor Cyan
    $reason = Read-Host "Enter revocation reason [Default: keyCompromise]"
    if ([string]::IsNullOrWhiteSpace($reason)) { $reason = "keyCompromise" }

    Write-Host "`n1. Revoking certificate in CA database..." -ForegroundColor Green
    & openssl ca -config $cnfFile -revoke $userCrt -crl_reason $reason

    Write-Host "2. Regenerating Certificate Revocation List (CRL)..." -ForegroundColor Green
    $caCrl = "$crlDir\ca.crl"
    & openssl ca -config $cnfFile -gencrl -out $caCrl

    Write-Host "`nCertificate for '$username' has been REVOKED and CRL has been updated!" -ForegroundColor Green
}

function Sign-Document {
    Write-Header "Sign Document / File (Digital Signature)"
    $username = Read-Host "Enter signer username"
    $userKey = "$usersDir\$username\$username.key"
    $userCrt = "$usersDir\$username\$username.crt"

    if (-not (Test-Path $userKey) -or -not (Test-Path $userCrt)) {
        Write-Host "User key or certificate not found for '$username'." -ForegroundColor Red
        return
    }

    $filePath = Read-Host "Enter path to file/document to sign"
    if (-not (Test-Path $filePath)) {
        Write-Host "File not found: $filePath" -ForegroundColor Red
        return
    }

    $sigPath = "$filePath.sig"

    Write-Host "Signing file '$filePath' using SHA-256 and RSA private key of '$username'..." -ForegroundColor Green
    & openssl dgst -sha256 -sign $userKey -out $sigPath $filePath

    Write-Host "`nDocument successfully signed!" -ForegroundColor Green
    Write-Host "Original Document: $filePath" -ForegroundColor White
    Write-Host "Digital Signature: $sigPath" -ForegroundColor White
    Write-Host "Signer Certificate: $userCrt" -ForegroundColor White
}

function Verify-Signature {
    Write-Header "Verify Digital Signature & Certificate Validity"
    $filePath = Read-Host "Enter path to the original file/document"
    if (-not (Test-Path $filePath)) {
        Write-Host "File not found: $filePath" -ForegroundColor Red
        return
    }

    $sigPath = Read-Host "Enter path to signature file [Default: $filePath.sig]"
    if ([string]::IsNullOrWhiteSpace($sigPath)) { $sigPath = "$filePath.sig" }
    if (-not (Test-Path $sigPath)) {
        Write-Host "Signature file not found: $sigPath" -ForegroundColor Red
        return
    }

    $username = Read-Host "Enter signer username (or path to signer certificate)"
    if (Test-Path $username) {
        $certPath = $username
    } else {
        $certPath = "$usersDir\$username\$username.crt"
    }

    if (-not (Test-Path $certPath)) {
        Write-Host "Signer certificate not found: $certPath" -ForegroundColor Red
        return
    }

    Write-Host "`n--- Step 1: Cryptographic Signature Verification ---" -ForegroundColor Cyan
    $tempPub = [System.IO.Path]::GetTempFileName()
    try {
        & openssl x509 -in $certPath -pubkey -noout -out $tempPub
        $sigVerifyOutput = & openssl dgst -sha256 -verify $tempPub -signature $sigPath $filePath 2>&1
        $sigOk = ($LASTEXITCODE -eq 0 -and $sigVerifyOutput -match "Verified OK")

        if ($sigOk) {
            Write-Host "[+] Cryptographic Signature is VALID!" -ForegroundColor Green
            Write-Host "    The document has NOT been tampered with and was signed by the key holder." -ForegroundColor Green
        } else {
            Write-Host "[-] Cryptographic Signature is INVALID!" -ForegroundColor Red
            Write-Host "    The document or signature was modified or does not match this public key." -ForegroundColor Red
        }
    } finally {
        if (Test-Path $tempPub) { Remove-Item $tempPub -Force }
    }

    Write-Host "`n--- Step 2: PKI Certificate Trust & Revocation Check ---" -ForegroundColor Cyan
    $caCrt = "$certsDir\ca.crt"
    $caCrl = "$crlDir\ca.crl"

    if (-not (Test-Path $caCrt)) {
        Write-Host "[-] Cannot check certificate trust: CA certificate missing." -ForegroundColor Red
        return
    }

    # Verify against CA and CRL
    $certVerifyOutput = & openssl verify -CAfile $caCrt -CRLfile $caCrl -crl_check $certPath 2>&1
    $certExitCode = $LASTEXITCODE

    if ($certExitCode -eq 0 -and $certVerifyOutput -match ": OK") {
        Write-Host "[+] Signer Certificate is TRUSTED and VALID!" -ForegroundColor Green
        Write-Host "    - Issued by our Root CA" -ForegroundColor Green
        Write-Host "    - Within validity period (365 days)" -ForegroundColor Green
        Write-Host "    - NOT revoked in CRL" -ForegroundColor Green
    } else {
        Write-Host "[-] Signer Certificate Verification FAILED!" -ForegroundColor Red
        if ($certVerifyOutput -match "certificate revoked") {
            Write-Host "    [!] REASON: The certificate has been REVOKED by the CA!" -ForegroundColor Red
        } elseif ($certVerifyOutput -match "has expired") {
            Write-Host "    [!] REASON: The certificate has EXPIRED!" -ForegroundColor Red
        } else {
            Write-Host "    [!] Details: $certVerifyOutput" -ForegroundColor Red
        }
    }

    Write-Host "`n--- Overall Status ---" -ForegroundColor Cyan
    if ($sigOk -and $certExitCode -eq 0) {
        Write-Host ">>> OVERALL RESULT: FULLY TRUSTED & VALID DIGITAL SIGNATURE <<<" -ForegroundColor Green
    } elseif ($sigOk -and $certExitCode -ne 0) {
        Write-Host ">>> OVERALL RESULT: UNTRUSTED / REVOKED SIGNATURE (Key matches, but certificate is INVALID) <<<" -ForegroundColor Yellow
    } else {
        Write-Host ">>> OVERALL RESULT: VERIFICATION REJECTED <<<" -ForegroundColor Red
    }
}

function List-Certificates {
    Write-Header "List PKI Certificates & CA Database"
    $indexPath = "$caDir\index.txt"
    if (-not (Test-Path $indexPath)) {
        Write-Host "CA database index.txt does not exist." -ForegroundColor Yellow
        return
    }

    $lines = Get-Content $indexPath
    if ($lines.Count -eq 0) {
        Write-Host "No certificates have been issued yet." -ForegroundColor Yellow
        return
    }

    Write-Host "Status | Expiration (GMT)   | Revocation Date   | Serial | Subject" -ForegroundColor Cyan
    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray
    foreach ($line in $lines) {
        $parts = $line -split "`t"
        $status = switch ($parts[0]) {
            "V" { "VALID  " }
            "R" { "REVOKED" }
            "E" { "EXPIRED" }
            Default { $parts[0] }
        }
        $exp = $parts[1]
        $rev = if ($parts[0] -eq "R") { $parts[2] } else { "N/A               " }
        $serial = if ($parts[0] -eq "R") { $parts[3] } else { $parts[2] }
        $subj = if ($parts[0] -eq "R") { $parts[5] } else { $parts[4] }

        $color = if ($parts[0] -eq "V") { "Green" } elseif ($parts[0] -eq "R") { "Red" } else { "Yellow" }
        Write-Host "$status | $exp | $rev | $serial   | $subj" -ForegroundColor $color
    }
}

function View-Details {
    Write-Header "Inspect Certificate or CRL"
    Write-Host "1. View Root CA Certificate"
    Write-Host "2. View Certificate Revocation List (CRL)"
    Write-Host "3. View User Certificate"
    $opt = Read-Host "Select option (1-3)"

    switch ($opt) {
        "1" {
            $caCrt = "$certsDir\ca.crt"
            if (Test-Path $caCrt) {
                & openssl x509 -in $caCrt -text -noout
            } else {
                Write-Host "CA certificate not found." -ForegroundColor Red
            }
        }
        "2" {
            $caCrl = "$crlDir\ca.crl"
            if (Test-Path $caCrl) {
                & openssl crl -in $caCrl -text -noout
            } else {
                Write-Host "CRL not found." -ForegroundColor Red
            }
        }
        "3" {
            $username = Read-Host "Enter username"
            $userCrt = "$usersDir\$username\$username.crt"
            if (Test-Path $userCrt) {
                & openssl x509 -in $userCrt -text -noout
            } else {
                Write-Host "Certificate for $username not found." -ForegroundColor Red
            }
        }
    }
}

# --- Main Interactive Loop ---
Ensure-OpenSsl

while ($true) {
    Write-Header "Public Key Infrastructure (PKI) & Digital Signatures (DSA)"
    Write-Host "1. Initialize Root Certified Authority (CA) [4096-bit RSA, 10 Years]"
    Write-Host "2. Issue User Private Key & Certificate [2048+ bit RSA, 365 Days]"
    Write-Host "3. Revoke User Certificate & Update CRL"
    Write-Host "4. Sign Document / File"
    Write-Host "5. Verify Document Signature & PKI Trust Chain"
    Write-Host "6. List All Issued Certificates"
    Write-Host "7. Inspect Certificate or CRL Details"
    Write-Host "8. Exit"
    Write-Host "--------------------------------------------------------" -ForegroundColor Gray
    $choice = Read-Host "Select an option (1-8)"

    switch ($choice) {
        "1" { Initialize-CA }
        "2" { Issue-UserCertificate }
        "3" { Revoke-UserCertificate }
        "4" { Sign-Document }
        "5" { Verify-Signature }
        "6" { List-Certificates }
        "7" { View-Details }
        "8" { Write-Host "`nExiting PKI Manager. Goodbye!`n" -ForegroundColor Green; exit }
        Default { Write-Host "Invalid option. Please choose 1-8." -ForegroundColor Red }
    }
}
