using System.Text;
using MonoalphabeticCryptanalysis.Models;

namespace MonoalphabeticCryptanalysis.Services
{
    public class SubstitutionEngine
    {
        /// <summary>
        /// Partially decrypts text using current key:
        /// - Mapped letters become LOWERCASE plaintext.
        /// - Unmapped letters remain in UPPERCASE ciphertext.
        /// - Other characters (punctuation, numbers, spaces) remain unchanged.
        /// (This directly mirrors the notation used in Lab 2 Section 2.3).
        /// </summary>
        public string ProgressiveSubstitute(string text, DecryptionKey key)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (char.IsLetter(ch))
                {
                    char upper = char.ToUpperInvariant(ch);
                    if (key.TryGetPlain(upper, out char plain))
                    {
                        // Mapped -> lowercase plaintext
                        sb.Append(plain);
                    }
                    else
                    {
                        // Unmapped -> UPPERCASE ciphertext
                        sb.Append(upper);
                    }
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Fully decrypts text, preserving original casing where possible.
        /// </summary>
        public string FullDecrypt(string text, DecryptionKey key)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (char.IsLetter(ch))
                {
                    char upper = char.ToUpperInvariant(ch);
                    if (key.TryGetPlain(upper, out char plain))
                    {
                        sb.Append(char.IsUpper(ch) ? char.ToUpperInvariant(plain) : plain);
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}
