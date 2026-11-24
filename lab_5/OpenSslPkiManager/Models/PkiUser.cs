namespace OpenSslPkiManager.Models
{
    public class PkiUser
    {
        public string Username { get; set; } = string.Empty;
        public string KeyPath { get; set; } = string.Empty;
        public string CsrPath { get; set; } = string.Empty;
        public string CertPath { get; set; } = string.Empty;
        public string PubKeyPath { get; set; } = string.Empty;
        public int KeySize { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
