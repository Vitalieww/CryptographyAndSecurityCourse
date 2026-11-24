namespace OpenSslPkiManager.Models
{
    public class CertificateRecord
    {
        public string Status { get; set; } = string.Empty; // "V" (Valid), "R" (Revoked), "E" (Expired)
        public string ExpirationDate { get; set; } = string.Empty;
        public string RevocationDate { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string CertificateFile { get; set; } = string.Empty;
        public string DistinguishedName { get; set; } = string.Empty;

        public string StatusDisplay => Status switch
        {
            "V" => "VALID",
            "R" => "REVOKED",
            "E" => "EXPIRED",
            _ => Status
        };
    }

    public class VerificationResult
    {
        public bool IsSignatureValid { get; set; }
        public bool IsCertChainValid { get; set; }
        public bool IsCertRevoked { get; set; }
        public bool IsCertExpired { get; set; }
        public string SignatureMessage { get; set; } = string.Empty;
        public string CertificateMessage { get; set; } = string.Empty;
        public string SignerSubject { get; set; } = string.Empty;

        public bool IsFullyValid => IsSignatureValid && IsCertChainValid && !IsCertRevoked && !IsCertExpired;
    }
}
