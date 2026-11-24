# Laboratory Work No. 5: Public Key Infrastructure (PKI) and Digital Signature Algorithm (DSA)

**Course:** Cryptography and Information Security  
**Institution:** Technical University of Moldova (UTM)  
**Faculty:** Faculty of Computers, Informatics and Microelectronics  
**Department:** Software Engineering and Automatics  
**Student:** Vitalie Vasilean (FAF-241)  
**Supervisor:** Lecturer Maia Zaica  

---

## 1. Laboratory Objectives & Requirements

1. **Root Certified Authority (CA) Initialization:**
   - Algorithm: **RSA**
   - Key Dimension: **4096 bits**
   - Self-Signed Certificate Validity: **10 years (3650 days)**
   - Constraints: `CA:TRUE`, `cRLSign`, `keyCertSign`
2. **User Credential Lifecycle:**
   - Private Key: RSA minimum **2048 bits**
   - Certificate Validity: **365 days (1 year)**
   - CSR creation and CA certificate signing (`CA:FALSE`, `digitalSignature`, `nonRepudiation`, `keyEncipherment`)
3. **Revocation & CRL Management:**
   - Revocation in CA database (`index.txt`) with standard reasons (`keyCompromise`, etc.)
   - Generation and update of X.509 Certificate Revocation List (CRL)
4. **Digital Signatures & Verification:**
   - Document signing using RSA-SHA256 private key
   - Dual-stage verification:
     1. Cryptographic signature check (integrity and authorship)
     2. PKI chain check against Root CA and CRL (detecting expired or revoked certificates)

---

## 2. Project Architecture

```
lab_5/
|-- OpenSslPkiManager/                # C# .NET 10.0 Object-Oriented Console Suite
|   |-- Models/
|   |   |-- CertificateRecord.cs      # CA Database entity mapping index.txt
|   |   |-- PkiUser.cs                # User identity and file paths
|   |   |-- VerificationResult.cs     # Comprehensive verification outcome
|   |-- Services/
|   |   |-- OpenSslCommandRunner.cs   # OpenSSL process execution wrapper
|   |   |-- PkiService.cs             # PKI engine: CA, issuance, revocation, CRL, signing
|   |-- UI/
|   |   |-- ConsoleMenu.cs            # Interactive terminal menu & automated demo suite
|   |   |-- DisplayFormatter.cs       # Clean tables, audit verdict cards, colors
|   |-- Program.cs                    # Application entry point
|   |-- OpenSslPkiManager.csproj      # .NET 10.0 project definition
|-- scripts/                          # Cross-Platform Automation Scripts
|   |-- openssl.cnf.template          # OpenSSL X.509 v3 configuration template
|   |-- pki_manager.ps1               # PowerShell CLI implementation
|   |-- pki_manager.sh                # POSIX Bash CLI implementation
|-- pki_store/                        # Runtime PKI state directory
|   |-- ca/                           # CA private key, cert, CRL, serial, index.txt
|   |-- users/                        # User private keys, CSRs, certificates, public keys
|   |-- demo_workspace/               # Test files, signatures, tampering scenarios
|-- report/                           # Academic LaTeX Report & PDF
|   |-- report.tex                    # Full 17-page LaTeX documentation
|   |-- report.pdf                    # Compiled PDF laboratory report
|   |-- figures/utm_logo.png          # University logo
```

---

## 3. How to Run

### Method A: C# .NET Application (Interactive)

From the repository root:
```powershell
# Run the interactive console application:
dotnet run --project lab_5/OpenSslPkiManager/OpenSslPkiManager.csproj
```

### Method B: Automated Demonstration & Security Audit (Instant 1-Click Verification)

To execute the complete end-to-end audit verifying all 7 lifecycle requirements (CA init, user issuance, document signing, verification, tampering attack, certificate revocation, and CRL verification):
```powershell
dotnet run --project lab_5/OpenSslPkiManager/OpenSslPkiManager.csproj -- --demo
```

### Method C: PowerShell Script

```powershell
powershell -ExecutionPolicy Bypass -File .\lab_5\scripts\pki_manager.ps1
```

### Method D: Bash Script (Linux / macOS / WSL)

```bash
chmod +x lab_5/scripts/pki_manager.sh
./lab_5/scripts/pki_manager.sh
```

---

## 4. Key OpenSSL Commands Implemented

1. **CA Key (4096-bit RSA):**
   ```bash
   openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out ca.key
   ```
2. **CA Self-Signed Certificate (3650 days):**
   ```bash
   openssl req -config openssl.cnf -key ca.key -new -x509 -days 3650 -sha256 -extensions v3_ca -out ca.crt -subj "/C=MD/ST=Chisinau/L=Chisinau/O=Technical University of Moldova/OU=FAF/CN=UTM-FAF-Root-CA"
   ```
3. **User Key (2048-bit RSA) & CSR:**
   ```bash
   openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048 -out user.key
   openssl req -config openssl.cnf -new -key user.key -out user.csr -subj "/C=MD/ST=Chisinau/O=Technical University of Moldova/OU=FAF/CN=alice"
   ```
4. **CA Sign User Certificate (365 days):**
   ```bash
   openssl ca -config openssl.cnf -batch -extensions usr_cert -days 365 -notext -md sha256 -in user.csr -out user.crt
   ```
5. **Digital Signature (RSA-SHA256):**
   ```bash
   openssl dgst -sha256 -sign user.key -out document.txt.sig document.txt
   ```
6. **Signature Verification (Public Key):**
   ```bash
   openssl x509 -in user.crt -pubkey -noout -out user.pub
   openssl dgst -sha256 -verify user.pub -signature document.txt.sig document.txt
   ```
7. **Certificate Revocation & CRL Update:**
   ```bash
   openssl ca -config openssl.cnf -revoke user.crt -crl_reason keyCompromise
   openssl ca -config openssl.cnf -gencrl -out ca.crl
   ```
8. **PKI Trust Chain & Revocation Verification:**
   ```bash
   openssl verify -CAfile ca.crt -CRLfile ca.crl -crl_check user.crt
   ```

---

## 5. Laboratory Report

The comprehensive academic report is available in:
- LaTeX Source: [`lab_5/report/report.tex`](file:///c:/Users/vital/University_folder/year_3/sem_5/CS/CryptographyAndSecurityCourse/lab_5/report/report.tex)
- Compiled Document: [`lab_5/report/report.pdf`](file:///c:/Users/vital/University_folder/year_3/sem_5/CS/CryptographyAndSecurityCourse/lab_5/report/report.pdf)
