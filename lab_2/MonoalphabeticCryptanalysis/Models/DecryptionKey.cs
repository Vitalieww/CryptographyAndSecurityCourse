using System.Collections.Generic;
using System.Text;

namespace MonoalphabeticCryptanalysis.Models
{
    public class DecryptionKey
    {
        // cipherLetter (uppercase) -> plaintextLetter (lowercase)
        private readonly Dictionary<char, char> _cipherToPlain = new();
        // plaintextLetter (lowercase) -> cipherLetter (uppercase)
        private readonly Dictionary<char, char> _plainToCipher = new();

        public DecryptionKey()
        {
        }

        public DecryptionKey(Dictionary<char, char> initialMapping)
        {
            foreach (var kvp in initialMapping)
            {
                SetMapping(kvp.Key, kvp.Value);
            }
        }

        public bool SetMapping(char cipherLetter, char plainLetter)
        {
            char c = char.ToUpperInvariant(cipherLetter);
            char p = char.ToLowerInvariant(plainLetter);

            if (c < 'A' || c > 'Z' || p < 'a' || p > 'z')
            {
                return false;
            }

            // If plaintext is already used for another ciphertext letter, remove the old one
            if (_plainToCipher.TryGetValue(p, out char oldCipher) && oldCipher != c)
            {
                _cipherToPlain.Remove(oldCipher);
            }

            // If ciphertext was already mapped to another plaintext, remove old plain reverse
            if (_cipherToPlain.TryGetValue(c, out char oldPlain) && oldPlain != p)
            {
                _plainToCipher.Remove(oldPlain);
            }

            _cipherToPlain[c] = p;
            _plainToCipher[p] = c;
            return true;
        }

        public bool RemoveMapping(char cipherLetter)
        {
            char c = char.ToUpperInvariant(cipherLetter);
            if (_cipherToPlain.TryGetValue(c, out char p))
            {
                _cipherToPlain.Remove(c);
                _plainToCipher.Remove(p);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _cipherToPlain.Clear();
            _plainToCipher.Clear();
        }

        public bool TryGetPlain(char cipherLetter, out char plainLetter)
        {
            return _cipherToPlain.TryGetValue(char.ToUpperInvariant(cipherLetter), out plainLetter);
        }

        public bool TryGetCipher(char plainLetter, out char cipherLetter)
        {
            return _plainToCipher.TryGetValue(char.ToLowerInvariant(plainLetter), out cipherLetter);
        }

        public int MappedCount => _cipherToPlain.Count;
        public bool IsComplete => _cipherToPlain.Count == 26;

        public IReadOnlyDictionary<char, char> GetCipherToPlainMap() => _cipherToPlain;
        public IReadOnlyDictionary<char, char> GetPlainToCipherMap() => _plainToCipher;

        public DecryptionKey Clone()
        {
            var clone = new DecryptionKey();
            foreach (var kvp in _cipherToPlain)
            {
                clone.SetMapping(kvp.Key, kvp.Value);
            }
            return clone;
        }
    }
}
