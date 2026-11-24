using System.Text.RegularExpressions;
using OpenSslPkiManager.Models;

namespace OpenSslPkiManager.Services
{
    public class PkiService
    {
        public string BaseDirectory { get; }
        public string CaDirectory { get; }
        public string CertsDirectory { get; }
        public string CrlDirectory { get; }
        public string NewCertsDirectory { get; }
        public string PrivateDirectory { get; }
        public string UsersDirectory { get; }
        public string ConfigPath { get; }
        public string CaKeyPath { get; }
        public string CaCertPath { get; }
        public string CaCrlPath { get; }
        public string IndexTxtPath { get; }

        public PkiService(string? customBaseDir = null)
        {
            if (string.IsNullOrWhiteSpace(customBaseDir))
            {
                string cwd = Directory.GetCurrentDirectory();
                if (Directory.Exists(Path.Combine(cwd, "lab_5")) || cwd.EndsWith("CryptographyAndSecurityCourse", StringComparison.OrdinalIgnoreCase))
                {
                    BaseDirectory = Path.GetFullPath(Path.Combine(cwd, "lab_5", "pki_store"));
                }
                else
                {
                    BaseDirectory = Path.GetFullPath(Path.Combine(cwd, "pki_store"));
                }
            }
            else
            {
                BaseDirectory = Path.GetFullPath(customBaseDir);
            }

            CaDirectory = Path.Combine(BaseDirectory, "ca");
            CertsDirectory = Path.Combine(CaDirectory, "certs");
            CrlDirectory = Path.Combine(CaDirectory, "crl");
            NewCertsDirectory = Path.Combine(CaDirectory, "newcerts");
            PrivateDirectory = Path.Combine(CaDirectory, "private");
            UsersDirectory = Path.Combine(BaseDirectory, "users");
            ConfigPath = Path.Combine(CaDirectory, "openssl.cnf");
            CaKeyPath = Path.Combine(PrivateDirectory, "ca.key");
            CaCertPath = Path.Combine(CertsDirectory, "ca.crt");
            CaCrlPath = Path.Combine(CrlDirectory, "ca.crl");
            IndexTxtPath = Path.Combine(CaDirectory, "index.txt");
        }

        public void EnsurePkiDirectories()
        {
            Directory.CreateDirectory(BaseDirectory);
            Directory.CreateDirectory(CaDirectory);
            Directory.CreateDirectory(CertsDirectory);
            Directory.CreateDirectory(CrlDirectory);
            Directory.CreateDirectory(NewCertsDirectory);
            Directory.CreateDirectory(PrivateDirectory);
            Directory.CreateDirectory(UsersDirectory);

            if (!File.Exists(IndexTxtPath))
            {
                File.WriteAllText(IndexTxtPath, string.Empty);
            }

            string serialPath = Path.Combine(CaDirectory, "serial");
            if (!File.Exists(serialPath))
            {
                File.WriteAllText(serialPath, "1000\n");
            }

            string crlNumberPath = Path.Combine(CaDirectory, "crlnumber");
            if (!File.Exists(crlNumberPath))
            {
                File.WriteAllText(crlNumberPath, "1000\n");
            }

            GenerateOpenSslConfig();
        }

        public void GenerateOpenSslConfig()
        {
            // OpenSSL on Windows expects forward slashes in config paths
            string normalizedCaDir = CaDirectory.Replace('\\', '/');

            string configContent = $@"# OpenSSL Configuration File for Internal PKI
# Laboratory Work No. 5 - Public Key Infrastructure & Digital Signatures

[ ca ]
default_ca = CA_default

[ CA_default ]
dir               = {normalizedCaDir}
certs             = $dir/certs
crl_dir           = $dir/crl
new_certs_dir     = $dir/newcerts
database          = $dir/index.txt
serial            = $dir/serial
RANDFILE          = $dir/private/.rand

# The root key and root certificate
private_key       = $dir/private/ca.key
certificate       = $dir/certs/ca.crt

# Certificate Revocation List (CRL) parameters
crlnumber         = $dir/crlnumber
crl               = $dir/crl/ca.crl
crl_extensions    = crl_ext
default_crl_days  = 30

# Default hashing digest
default_md        = sha256

name_opt          = ca_default
cert_opt          = ca_default
default_days      = 365
preserve          = no
policy            = policy_loose

[ policy_loose ]
countryName             = optional
stateOrProvinceName     = optional
localityName            = optional
organizationName        = optional
organizationalUnitName  = optional
commonName              = supplied
emailAddress            = optional

[ req ]
default_bits        = 2048
distinguished_name  = req_distinguished_name
string_mask         = utf8only
default_md          = sha256

[ req_distinguished_name ]
countryName                     = Country Name (2 letter code)
countryName_default             = MD
stateOrProvinceName             = State or Province Name
stateOrProvinceName_default     = Chisinau
localityName                    = Locality Name
localityName_default            = Chisinau
0.organizationName              = Organization Name
0.organizationName_default      = Technical University of Moldova
organizationalUnitName          = Organizational Unit Name
organizationalUnitName_default  = FAF
commonName                      = Common Name
emailAddress                    = Email Address

[ v3_ca ]
# Extensions for a self-signed root CA
subjectKeyIdentifier = hash
authorityKeyIdentifier = keyid:always,issuer
basicConstraints = critical, CA:true
keyUsage = critical, digitalSignature, cRLSign, keyCertSign

[ usr_cert ]
# Extensions for user / end-entity certificates (365 days)
basicConstraints = CA:FALSE
nsCertType = client, email
nsComment = ""UTM FAF PKI Generated End-Entity Certificate""
subjectKeyIdentifier = hash
authorityKeyIdentifier = keyid,issuer
keyUsage = critical, nonRepudiation, digitalSignature, keyEncipherment
extendedKeyUsage = clientAuth, emailProtection

[ crl_ext ]
# Extensions for CRLs
authorityKeyIdentifier = keyid:always
";
            File.WriteAllText(ConfigPath, configContent);
        }

        public bool IsCaInitialized()
        {
            return File.Exists(CaKeyPath) && File.Exists(CaCertPath) && File.Exists(CaCrlPath);
        }

        public (bool success, string message) InitializeCa(
            int keySize = 4096,
            int days = 3650,
            string country = "MD",
            string org = "Technical University of Moldova",
            string orgUnit = "FAF",
            string commonName = "UTM-FAF-Root-CA")
        {
            EnsurePkiDirectories();

            // 1. Generate RSA Root CA Private Key (4096 bits)
            string genKeyArgs = $"genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:{keySize} -out \"{CaKeyPath}\"";
            var genKeyRes = OpenSslCommandRunner.Run(genKeyArgs);
            if (!genKeyRes.Success)
            {
                return (false, $"Failed to generate CA root private key: {genKeyRes.CombinedOutput}");
            }

            // 2. Generate Self-Signed Root CA Certificate (validity: 3650 days / 10 years)
            string subject = $"/C={country}/ST=Chisinau/L=Chisinau/O={org}/OU={orgUnit}/CN={commonName}";
            string reqArgs = $"req -config \"{ConfigPath}\" -key \"{CaKeyPath}\" -new -x509 -days {days} -sha256 -extensions v3_ca -out \"{CaCertPath}\" -subj \"{subject}\"";
            var reqRes = OpenSslCommandRunner.Run(reqArgs);
            if (!reqRes.Success)
            {
                return (false, $"Failed to generate self-signed CA certificate: {reqRes.CombinedOutput}");
            }

            // 3. Generate initial CRL
            string crlArgs = $"ca -config \"{ConfigPath}\" -gencrl -out \"{CaCrlPath}\"";
            var crlRes = OpenSslCommandRunner.Run(crlArgs);
            if (!crlRes.Success)
            {
                return (false, $"Failed to generate initial CRL: {crlRes.CombinedOutput}");
            }

            return (true, $"Root CA successfully initialized!\nPrivate Key: {CaKeyPath} ({keySize}-bit RSA)\nRoot Certificate: {CaCertPath} (Valid for {days} days / ~10 years)\nInitial CRL: {CaCrlPath}");
        }

        public (bool success, string message, PkiUser? user) IssueUserCertificate(
            string username,
            int keySize = 2048,
            int days = 365,
            string country = "MD",
            string org = "Technical University of Moldova",
            string orgUnit = "FAF")
        {
            if (!IsCaInitialized())
            {
                return (false, "Root CA is not initialized yet. Please initialize the CA first.", null);
            }

            if (keySize < 2048)
            {
                return (false, "User private key dimension must be at least 2048 bits per requirements.", null);
            }

            string userFolder = Path.Combine(UsersDirectory, username);
            Directory.CreateDirectory(userFolder);

            string keyPath = Path.Combine(userFolder, $"{username}.key");
            string csrPath = Path.Combine(userFolder, $"{username}.csr");
            string certPath = Path.Combine(userFolder, $"{username}.crt");
            string pubKeyPath = Path.Combine(userFolder, $"{username}.pub");

            // 1. Generate User RSA Private Key
            string genKeyArgs = $"genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:{keySize} -out \"{keyPath}\"";
            var genKeyRes = OpenSslCommandRunner.Run(genKeyArgs);
            if (!genKeyRes.Success)
            {
                return (false, $"Failed to generate user private key: {genKeyRes.CombinedOutput}", null);
            }

            // 2. Generate Certificate Signing Request (CSR)
            string subject = $"/C={country}/ST=Chisinau/O={org}/OU={orgUnit}/CN={username}";
            string csrArgs = $"req -config \"{ConfigPath}\" -new -key \"{keyPath}\" -out \"{csrPath}\" -subj \"{subject}\"";
            var csrRes = OpenSslCommandRunner.Run(csrArgs);
            if (!csrRes.Success)
            {
                return (false, $"Failed to generate CSR: {csrRes.CombinedOutput}", null);
            }

            // 3. CA Signs the User Certificate (validity: 365 days)
            string signArgs = $"ca -config \"{ConfigPath}\" -batch -extensions usr_cert -days {days} -notext -md sha256 -in \"{csrPath}\" -out \"{certPath}\"";
            var signRes = OpenSslCommandRunner.Run(signArgs);
            if (!signRes.Success)
            {
                return (false, $"CA failed to sign certificate: {signRes.CombinedOutput}", null);
            }

            // 4. Extract Public Key
            string pubKeyArgs = $"x509 -in \"{certPath}\" -pubkey -noout -out \"{pubKeyPath}\"";
            OpenSslCommandRunner.Run(pubKeyArgs);

            var user = new PkiUser
            {
                Username = username,
                KeyPath = keyPath,
                CsrPath = csrPath,
                CertPath = certPath,
                PubKeyPath = pubKeyPath,
                KeySize = keySize,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(days),
                IsRevoked = false
            };

            return (true, $"Certificate issued successfully for user '{username}'!\nKey Size: {keySize} bits RSA\nValidity: {days} days\nCertificate: {certPath}", user);
        }

        public (bool success, string message) RevokeUserCertificate(string username, string reason = "keyCompromise")
        {
            if (!IsCaInitialized())
            {
                return (false, "Root CA is not initialized.");
            }

            string certPath = Path.Combine(UsersDirectory, username, $"{username}.crt");
            if (!File.Exists(certPath))
            {
                return (false, $"Certificate file not found for user '{username}' at: {certPath}");
            }

            // 1. Revoke certificate
            string revokeArgs = $"ca -config \"{ConfigPath}\" -revoke \"{certPath}\" -crl_reason {reason}";
            var revokeRes = OpenSslCommandRunner.Run(revokeArgs);
            if (!revokeRes.Success)
            {
                // Notice: OpenSSL exits with error if certificate was already revoked
                if (revokeRes.CombinedOutput.Contains("already revoked"))
                {
                    return (false, $"Certificate for '{username}' was already revoked previously.");
                }
                return (false, $"Failed to revoke certificate: {revokeRes.CombinedOutput}");
            }

            // 2. Regenerate CRL
            string crlArgs = $"ca -config \"{ConfigPath}\" -gencrl -out \"{CaCrlPath}\"";
            var crlRes = OpenSslCommandRunner.Run(crlArgs);
            if (!crlRes.Success)
            {
                return (false, $"Certificate revoked, but failed to update CRL: {crlRes.CombinedOutput}");
            }

            return (true, $"Certificate for user '{username}' has been successfully REVOKED (Reason: {reason})!\nCertificate Revocation List (CRL) updated: {CaCrlPath}");
        }

        public (bool success, string message, string sigPath) SignDocument(string username, string documentPath)
        {
            if (!File.Exists(documentPath))
            {
                return (false, $"Document file not found: {documentPath}", string.Empty);
            }

            string keyPath = Path.Combine(UsersDirectory, username, $"{username}.key");
            if (!File.Exists(keyPath))
            {
                return (false, $"Private key not found for user '{username}' at: {keyPath}", string.Empty);
            }

            string sigPath = $"{documentPath}.sig";

            // Sign document using SHA-256 and RSA private key
            string signArgs = $"dgst -sha256 -sign \"{keyPath}\" -out \"{sigPath}\" \"{documentPath}\"";
            var res = OpenSslCommandRunner.Run(signArgs);
            if (!res.Success)
            {
                return (false, $"Failed to sign document: {res.CombinedOutput}", string.Empty);
            }

            return (true, $"Document signed successfully!\nSignature File: {sigPath}\nAlgorithm: RSA-SHA256", sigPath);
        }

        public VerificationResult VerifySignature(string documentPath, string signaturePath, string certificatePath)
        {
            var result = new VerificationResult();

            if (!File.Exists(documentPath))
            {
                result.SignatureMessage = $"Document file not found: {documentPath}";
                return result;
            }

            if (!File.Exists(signaturePath))
            {
                result.SignatureMessage = $"Signature file not found: {signaturePath}";
                return result;
            }

            if (!File.Exists(certificatePath))
            {
                result.CertificateMessage = $"Certificate file not found: {certificatePath}";
                return result;
            }

            // Extract signer subject from certificate
            var subjRes = OpenSslCommandRunner.Run($"x509 -in \"{certificatePath}\" -subject -noout");
            result.SignerSubject = subjRes.StandardOutput.Trim();

            // --- STEP 1: Cryptographic Signature Verification ---
            string tempPubKey = Path.GetTempFileName();
            try
            {
                // Extract public key from user certificate
                var pubKeyRes = OpenSslCommandRunner.Run($"x509 -in \"{certificatePath}\" -pubkey -noout");
                if (pubKeyRes.Success)
                {
                    File.WriteAllText(tempPubKey, pubKeyRes.StandardOutput);

                    // Verify signature against document
                    var verifySigRes = OpenSslCommandRunner.Run($"dgst -sha256 -verify \"{tempPubKey}\" -signature \"{signaturePath}\" \"{documentPath}\"");
                    if (verifySigRes.Success && verifySigRes.StandardOutput.Contains("Verified OK"))
                    {
                        result.IsSignatureValid = true;
                        result.SignatureMessage = "Cryptographic signature is mathematically VALID (Document integrity and authorship verified).";
                    }
                    else
                    {
                        result.IsSignatureValid = false;
                        result.SignatureMessage = "Cryptographic signature verification FAILED (Document was modified or signature does not match).";
                    }
                }
                else
                {
                    result.IsSignatureValid = false;
                    result.SignatureMessage = $"Failed to extract public key from certificate: {pubKeyRes.CombinedOutput}";
                }
            }
            finally
            {
                if (File.Exists(tempPubKey))
                {
                    File.Delete(tempPubKey);
                }
            }

            // --- STEP 2: PKI Chain & Revocation Verification ---
            if (!File.Exists(CaCertPath))
            {
                result.CertificateMessage = "Cannot verify PKI trust: Root CA certificate missing.";
                return result;
            }

            string verifyCertArgs = $"verify -CAfile \"{CaCertPath}\" -CRLfile \"{CaCrlPath}\" -crl_check \"{certificatePath}\"";
            var verifyCertRes = OpenSslCommandRunner.Run(verifyCertArgs);

            if (verifyCertRes.CombinedOutput.Contains("certificate revoked"))
            {
                result.IsCertRevoked = true;
                result.IsCertChainValid = false;
                result.CertificateMessage = "Certificate is REVOKED by the Certified Authority (Present in CRL).";
            }
            else if (verifyCertRes.CombinedOutput.Contains("has expired"))
            {
                result.IsCertExpired = true;
                result.IsCertChainValid = false;
                result.CertificateMessage = "Certificate has EXPIRED.";
            }
            else if (verifyCertRes.Success && verifyCertRes.CombinedOutput.Contains(": OK"))
            {
                result.IsCertChainValid = true;
                result.CertificateMessage = "Certificate is VALID and TRUSTED (Issued by Root CA, unexpired, and not revoked).";
            }
            else
            {
                result.IsCertChainValid = false;
                result.CertificateMessage = $"Certificate trust validation failed: {verifyCertRes.CombinedOutput.Trim()}";
            }

            return result;
        }

        public List<CertificateRecord> GetCertificates()
        {
            var list = new List<CertificateRecord>();
            if (!File.Exists(IndexTxtPath))
            {
                return list;
            }

            var lines = File.ReadAllLines(IndexTxtPath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('\t');
                if (parts.Length < 5) continue;

                var rec = new CertificateRecord
                {
                    Status = parts[0],
                    ExpirationDate = parts[1]
                };

                if (rec.Status == "R" && parts.Length >= 6)
                {
                    rec.RevocationDate = parts[2];
                    rec.SerialNumber = parts[3];
                    rec.CertificateFile = parts[4];
                    rec.DistinguishedName = parts[5];
                }
                else
                {
                    rec.RevocationDate = "N/A";
                    rec.SerialNumber = parts[2];
                    rec.CertificateFile = parts[3];
                    rec.DistinguishedName = parts[4];
                }

                list.Add(rec);
            }

            return list;
        }

        public string InspectCertificate(string certPath)
        {
            if (!File.Exists(certPath)) return "Certificate file not found.";
            var res = OpenSslCommandRunner.Run($"x509 -in \"{certPath}\" -text -noout");
            return res.CombinedOutput;
        }

        public string InspectCrl()
        {
            if (!File.Exists(CaCrlPath)) return "CRL file not found. Initialize CA first.";
            var res = OpenSslCommandRunner.Run($"crl -in \"{CaCrlPath}\" -text -noout");
            return res.CombinedOutput;
        }
    }
}
