using OpenSslPkiManager.Models;

namespace OpenSslPkiManager.UI
{
    public static class DisplayFormatter
    {
        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  {title}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.ResetColor();
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[+] SUCCESS: {message}");
            Console.ResetColor();
        }

        public static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[!] WARNING: {message}");
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[-] ERROR: {message}");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[*] {message}");
            Console.ResetColor();
        }

        public static void PrintCertificatesTable(List<CertificateRecord> records)
        {
            if (records.Count == 0)
            {
                PrintWarning("No certificates found in the CA database.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("| {0,-8} | {1,-6} | {2,-14} | {3,-15} | {4,-45} |", "Status", "Serial", "Expiration", "Revoked At", "Subject"));
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.ResetColor();

            foreach (var r in records)
            {
                if (r.Status == "V") Console.ForegroundColor = ConsoleColor.Green;
                else if (r.Status == "R") Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Yellow;

                string displaySubject = r.DistinguishedName.Length > 45 ? r.DistinguishedName[..42] + "..." : r.DistinguishedName;
                Console.WriteLine(string.Format("| {0,-8} | {1,-6} | {2,-14} | {3,-15} | {4,-45} |",
                    r.StatusDisplay,
                    r.SerialNumber,
                    r.ExpirationDate,
                    r.RevocationDate,
                    displaySubject));
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.ResetColor();
        }

        public static void PrintVerificationResult(VerificationResult result)
        {
            PrintHeader("DIGITAL SIGNATURE & PKI VERIFICATION AUDIT");

            Console.WriteLine($"Signer Subject:  {result.SignerSubject}");
            Console.WriteLine();

            // 1. Signature Check
            Console.Write("1. Cryptographic Signature (RSA-SHA256): ");
            if (result.IsSignatureValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[ VALID ]");
                Console.ResetColor();
                Console.WriteLine($"   -> {result.SignatureMessage}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ FAILED ]");
                Console.ResetColor();
                Console.WriteLine($"   -> {result.SignatureMessage}");
            }

            // 2. PKI Trust & Revocation Check
            Console.Write("2. PKI Certificate Trust & Revocation: ");
            if (result.IsCertChainValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[ TRUSTED & ACTIVE ]");
                Console.ResetColor();
                Console.WriteLine($"   -> {result.CertificateMessage}");
            }
            else if (result.IsCertRevoked)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ REVOKED ]");
                Console.ResetColor();
                Console.WriteLine($"   -> {result.CertificateMessage}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ UNTRUSTED / INVALID ]");
                Console.ResetColor();
                Console.WriteLine($"   -> {result.CertificateMessage}");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.Write("OVERALL SECURITY VERDICT: ");
            if (result.IsFullyValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("AUTHENTIC, UNMODIFIED, AND FULLY TRUSTED");
            }
            else if (result.IsSignatureValid && result.IsCertRevoked)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("SIGNATURE MATCHES, BUT SIGNER CERTIFICATE HAS BEEN REVOKED BY CA!");
            }
            else if (result.IsSignatureValid && !result.IsCertChainValid)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("SIGNATURE MATCHES, BUT CERTIFICATE CHAIN IS UNTRUSTED OR EXPIRED!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("REJECTED (Document tampered or signature invalid)!");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.ResetColor();
        }
    }
}
