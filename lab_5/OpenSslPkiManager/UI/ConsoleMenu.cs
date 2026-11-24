using OpenSslPkiManager.Services;

namespace OpenSslPkiManager.UI
{
    public class ConsoleMenu
    {
        private readonly PkiService _pkiService;

        public ConsoleMenu()
        {
            _pkiService = new PkiService();
        }

        public void Display()
        {
            if (!OpenSslCommandRunner.IsOpenSslAvailable(out string version))
            {
                DisplayFormatter.PrintError("OpenSSL is not found in system PATH. Please ensure OpenSSL is installed.");
                return;
            }

            bool running = true;
            while (running)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("    Internal PKI & Digital Signature Manager (Lab 5) - UTM FAF");
                Console.WriteLine($"    OpenSSL Engine: {version}");
                Console.WriteLine("================================================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" 1. Initialize Root Certified Authority (CA) [4096-bit RSA, 10 Years]");
                Console.WriteLine(" 2. Issue User Private Key & Certificate [2048+ bit RSA, 365 Days]");
                Console.WriteLine(" 3. Revoke User Certificate & Update CRL");
                Console.WriteLine(" 4. Sign Document / File (RSA-SHA256 Digital Signature)");
                Console.WriteLine(" 5. Verify Document Signature & PKI Trust Chain (with CRL Check)");
                Console.WriteLine(" 6. List All Issued Certificates & Status");
                Console.WriteLine(" 7. Inspect Certificate or CRL Details (X.509 Text)");
                Console.WriteLine(" 8. Run Automated Verification & Security Attack Suite");
                Console.WriteLine(" 9. Exit");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.ResetColor();
                Console.Write("Enter your choice (1-9): ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        HandleInitializeCa();
                        break;
                    case "2":
                        HandleIssueUserCertificate();
                        break;
                    case "3":
                        HandleRevokeUserCertificate();
                        break;
                    case "4":
                        HandleSignDocument();
                        break;
                    case "5":
                        HandleVerifySignature();
                        break;
                    case "6":
                        HandleListCertificates();
                        break;
                    case "7":
                        HandleInspectDetails();
                        break;
                    case "8":
                        HandleAutomatedDemo();
                        break;
                    case "9":
                        running = false;
                        DisplayFormatter.PrintSuccess("Exiting PKI Manager. Goodbye!");
                        break;
                    default:
                        DisplayFormatter.PrintError("Invalid choice. Please select an option from 1 to 9.");
                        break;
                }
            }
        }

        private void HandleInitializeCa()
        {
            DisplayFormatter.PrintHeader("INITIALIZE ROOT CERTIFIED AUTHORITY (CA)");

            if (_pkiService.IsCaInitialized())
            {
                DisplayFormatter.PrintWarning("A Root CA is already initialized!");
                Console.Write("Do you want to re-initialize and wipe existing CA data? (y/N): ");
                string? confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (confirm != "y" && confirm != "yes")
                {
                    DisplayFormatter.PrintInfo("CA initialization aborted.");
                    return;
                }
            }

            Console.WriteLine("Parameters for Root CA:");
            Console.WriteLine("- Algorithm: RSA");
            Console.WriteLine("- Key Dimension: 4096 bits (Mandated requirement)");
            Console.WriteLine("- Certificate Validity: 3650 days / 10 years (Mandated requirement)");
            Console.Write("Proceed with initialization? (Y/n): ");
            string? go = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (go == "n" || go == "no") return;

            var (success, message) = _pkiService.InitializeCa(keySize: 4096, days: 3650);
            if (success)
            {
                DisplayFormatter.PrintSuccess(message);
            }
            else
            {
                DisplayFormatter.PrintError(message);
            }
        }

        private void HandleIssueUserCertificate()
        {
            DisplayFormatter.PrintHeader("ISSUE USER PRIVATE KEY & CERTIFICATE");

            if (!_pkiService.IsCaInitialized())
            {
                DisplayFormatter.PrintError("Root CA is not initialized! Please run Option 1 first.");
                return;
            }

            Console.Write("Enter username / entity name (e.g. alice, bob): ");
            string? username = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                DisplayFormatter.PrintError("Username cannot be empty.");
                return;
            }

            Console.Write("Enter RSA key dimension (min 2048, e.g. 2048, 3072, 4096) [Default: 2048]: ");
            string? keyInput = Console.ReadLine()?.Trim();
            int keySize = 2048;
            if (!string.IsNullOrWhiteSpace(keyInput) && int.TryParse(keyInput, out int parsedKey))
            {
                keySize = parsedKey;
            }

            if (keySize < 2048)
            {
                DisplayFormatter.PrintWarning("Key size is below required 2048 bits. Enforcing 2048 bits.");
                keySize = 2048;
            }

            DisplayFormatter.PrintInfo($"Generating {keySize}-bit RSA key, CSR, and issuing certificate (validity: 365 days)...");
            var (success, message, user) = _pkiService.IssueUserCertificate(username, keySize: keySize, days: 365);
            if (success && user != null)
            {
                DisplayFormatter.PrintSuccess(message);
                Console.WriteLine($"  Private Key: {user.KeyPath}");
                Console.WriteLine($"  CSR:         {user.CsrPath}");
                Console.WriteLine($"  Certificate: {user.CertPath}");
                Console.WriteLine($"  Public Key:  {user.PubKeyPath}");
            }
            else
            {
                DisplayFormatter.PrintError(message);
            }
        }

        private void HandleRevokeUserCertificate()
        {
            DisplayFormatter.PrintHeader("REVOKE USER CERTIFICATE");

            if (!_pkiService.IsCaInitialized())
            {
                DisplayFormatter.PrintError("Root CA is not initialized!");
                return;
            }

            Console.Write("Enter username to revoke: ");
            string? username = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                DisplayFormatter.PrintError("Username cannot be empty.");
                return;
            }

            Console.WriteLine("Standard Revocation Reasons: keyCompromise, CACompromise, affiliationChanged, superseded, cessationOfOperation, unspecified");
            Console.Write("Enter reason [Default: keyCompromise]: ");
            string? reason = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(reason)) reason = "keyCompromise";

            var (success, message) = _pkiService.RevokeUserCertificate(username, reason);
            if (success)
            {
                DisplayFormatter.PrintSuccess(message);
            }
            else
            {
                DisplayFormatter.PrintError(message);
            }
        }

        private void HandleSignDocument()
        {
            DisplayFormatter.PrintHeader("SIGN DOCUMENT / FILE (DIGITAL SIGNATURE)");

            Console.Write("Enter signer username: ");
            string? username = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                DisplayFormatter.PrintError("Username cannot be empty.");
                return;
            }

            Console.Write("Enter path to file/document to sign: ");
            string? filePath = Console.ReadLine()?.Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                DisplayFormatter.PrintError($"File not found: {filePath}");
                return;
            }

            var (success, message, sigPath) = _pkiService.SignDocument(username, filePath);
            if (success)
            {
                DisplayFormatter.PrintSuccess(message);
            }
            else
            {
                DisplayFormatter.PrintError(message);
            }
        }

        private void HandleVerifySignature()
        {
            DisplayFormatter.PrintHeader("VERIFY DIGITAL SIGNATURE & CERTIFICATE TRUST");

            Console.Write("Enter path to original document: ");
            string? docPath = Console.ReadLine()?.Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(docPath) || !File.Exists(docPath))
            {
                DisplayFormatter.PrintError($"Document not found: {docPath}");
                return;
            }

            Console.Write($"Enter path to signature file [Default: {docPath}.sig]: ");
            string? sigPath = Console.ReadLine()?.Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(sigPath)) sigPath = $"{docPath}.sig";
            if (!File.Exists(sigPath))
            {
                DisplayFormatter.PrintError($"Signature file not found: {sigPath}");
                return;
            }

            Console.Write("Enter signer username (or direct path to signer certificate): ");
            string? certInput = Console.ReadLine()?.Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(certInput))
            {
                DisplayFormatter.PrintError("Certificate input cannot be empty.");
                return;
            }

            string certPath = File.Exists(certInput)
                ? certInput
                : Path.Combine(_pkiService.UsersDirectory, certInput, $"{certInput}.crt");

            if (!File.Exists(certPath))
            {
                DisplayFormatter.PrintError($"Signer certificate not found at: {certPath}");
                return;
            }

            DisplayFormatter.PrintInfo("Running full cryptographic verification and PKI trust check...");
            var result = _pkiService.VerifySignature(docPath, sigPath, certPath);
            DisplayFormatter.PrintVerificationResult(result);
        }

        private void HandleListCertificates()
        {
            DisplayFormatter.PrintHeader("CERTIFICATE DATABASE (CA index.txt)");
            var records = _pkiService.GetCertificates();
            DisplayFormatter.PrintCertificatesTable(records);
        }

        private void HandleInspectDetails()
        {
            DisplayFormatter.PrintHeader("INSPECT X.509 CERTIFICATE OR CRL");
            Console.WriteLine("1. Inspect Root CA Certificate");
            Console.WriteLine("2. Inspect Certificate Revocation List (CRL)");
            Console.WriteLine("3. Inspect User Certificate");
            Console.Write("Select an option (1-3): ");

            string? opt = Console.ReadLine()?.Trim();
            switch (opt)
            {
                case "1":
                    Console.WriteLine(_pkiService.InspectCertificate(_pkiService.CaCertPath));
                    break;
                case "2":
                    Console.WriteLine(_pkiService.InspectCrl());
                    break;
                case "3":
                    Console.Write("Enter username: ");
                    string? user = Console.ReadLine()?.Trim();
                    string userCert = Path.Combine(_pkiService.UsersDirectory, user ?? "", $"{user}.crt");
                    Console.WriteLine(_pkiService.InspectCertificate(userCert));
                    break;
                default:
                    DisplayFormatter.PrintError("Invalid option.");
                    break;
            }
        }

        private void HandleAutomatedDemo()
        {
            DisplayFormatter.PrintHeader("AUTOMATED END-TO-END PKI DEMONSTRATION & SECURITY AUDIT");
            DisplayFormatter.PrintInfo("This automated suite proves all laboratory requirements:");
            DisplayFormatter.PrintInfo("  1. CA initialization with 4096-bit RSA & 10 years validity");
            DisplayFormatter.PrintInfo("  2. User issuance with 2048-bit RSA & 365 days validity");
            DisplayFormatter.PrintInfo("  3. Document signing and valid verification");
            DisplayFormatter.PrintInfo("  4. Tampering attack detection (document altered)");
            DisplayFormatter.PrintInfo("  5. User certificate revocation and CRL verification");
            Console.WriteLine();

            string demoDir = Path.Combine(_pkiService.BaseDirectory, "demo_workspace");
            Directory.CreateDirectory(demoDir);

            // Step 1: CA init
            DisplayFormatter.PrintInfo("--> [Test 1] Initializing Root CA (4096-bit RSA, 3650 days)...");
            var caRes = _pkiService.InitializeCa(4096, 3650);
            if (!caRes.success) { DisplayFormatter.PrintError(caRes.message); return; }
            DisplayFormatter.PrintSuccess("Root CA initialized successfully.");

            // Step 2: User issuance
            DisplayFormatter.PrintInfo("--> [Test 2] Issuing Certificate for 'alice' (2048-bit RSA, 365 days)...");
            var userRes = _pkiService.IssueUserCertificate("alice", 2048, 365);
            if (!userRes.success) { DisplayFormatter.PrintError(userRes.message); return; }
            DisplayFormatter.PrintSuccess("Alice certificate issued.");

            // Step 3: Sign document
            string sampleFile = Path.Combine(demoDir, "financial_contract.txt");
            File.WriteAllText(sampleFile, "PAYMENT AUTHORIZATION: Transfer 5,000 EUR to Account #8492048.");
            DisplayFormatter.PrintInfo($"--> [Test 3] Alice signs '{Path.GetFileName(sampleFile)}'...");
            var signRes = _pkiService.SignDocument("alice", sampleFile);
            if (!signRes.success) { DisplayFormatter.PrintError(signRes.message); return; }
            DisplayFormatter.PrintSuccess($"Document signed: {signRes.sigPath}");

            // Step 4: Verify initial signature
            DisplayFormatter.PrintInfo("--> [Test 4] Verifying Alice's signature on authentic document...");
            string aliceCert = Path.Combine(_pkiService.UsersDirectory, "alice", "alice.crt");
            var verifyRes1 = _pkiService.VerifySignature(sampleFile, signRes.sigPath, aliceCert);
            DisplayFormatter.PrintVerificationResult(verifyRes1);

            // Step 5: Tampering test
            DisplayFormatter.PrintInfo("--> [Test 5] Simulating Man-in-the-Middle Attack (tampering document to 500,000 EUR)...");
            string tamperedFile = Path.Combine(demoDir, "tampered_financial_contract.txt");
            File.WriteAllText(tamperedFile, "PAYMENT AUTHORIZATION: Transfer 500,000 EUR to Account #8492048.");
            var verifyRes2 = _pkiService.VerifySignature(tamperedFile, signRes.sigPath, aliceCert);
            DisplayFormatter.PrintVerificationResult(verifyRes2);

            // Step 6: Revocation test
            DisplayFormatter.PrintInfo("--> [Test 6] Revoking Alice's certificate due to keyCompromise...");
            var revokeRes = _pkiService.RevokeUserCertificate("alice", "keyCompromise");
            if (!revokeRes.success) { DisplayFormatter.PrintError(revokeRes.message); return; }
            DisplayFormatter.PrintSuccess("Alice's certificate revoked and CRL updated.");

            // Step 7: Verify signature after revocation
            DisplayFormatter.PrintInfo("--> [Test 7] Verifying Alice's signature AFTER revocation (CRL enforcement)...");
            var verifyRes3 = _pkiService.VerifySignature(sampleFile, signRes.sigPath, aliceCert);
            DisplayFormatter.PrintVerificationResult(verifyRes3);

            DisplayFormatter.PrintHeader("AUTOMATED TEST SUITE COMPLETED SUCCESSFULLY!");
        }
    }
}
