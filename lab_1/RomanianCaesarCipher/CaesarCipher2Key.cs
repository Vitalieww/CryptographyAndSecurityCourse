using System.Text;

namespace lab_1
{
    public class CaesarCipher2Key
    {
        public static void Key1Validator(int key, string alphabet)
        {
            if (key < 1 || key > alphabet.Length - 1)
            {
                throw new ArgumentException($"Key 1 must be an integer between 1 and {alphabet.Length - 1} inclusive. Received: {key}");
            }
        }

        public static void Key2Validator(string key, string alphabet)
        {
            string normalized = Normalize(key).Replace(" ", "");
            int minLength = 7;
            if (normalized.Length < minLength)
            {
                throw new ArgumentException($"Key 2 (keyword) must be at least {minLength} characters long. Current length: {normalized.Length}");
            }
            foreach (char c in normalized)
            {
                if (!alphabet.Contains(char.ToUpper(c)))
                {
                    throw new ArgumentException($"Key 2 contains invalid character '{c}'. Only letters from the Romanian alphabet are allowed.");
                }
            }
        }

        public static string Normalize(string message)
        {
            return message
                .Replace('ş', 'ș')
                .Replace('Ş', 'Ș')
                .Replace('ţ', 'ț')
                .Replace('Ţ', 'Ț');
        }

        public static void StringValidator(string message, string alphabet)
        {
            foreach (char c in message)
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                if (!alphabet.Contains(char.ToUpper(c)))
                {
                    throw new ArgumentException($"The message contains invalid character '{c}'. Only letters from the Romanian alphabet are allowed.");
                }
            }
        }

        public static string BuildPermutedAlphabet(string key2, string alphabet)
        {
            string normalizedKey = Normalize(key2).Replace(" ", "").ToUpper();
            StringBuilder result = new();
            HashSet<char> seen = [];

            foreach (char c in normalizedKey)
            {
                if (!seen.Contains(c))
                {
                    seen.Add(c);
                    result.Append(c);
                }
            }

            foreach (char c in alphabet)
            {
                if (!seen.Contains(c))
                {
                    seen.Add(c);
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static string Encrypt(string normalMessage, int key, string key2, string alphabet)
        {
            Key1Validator(key, alphabet);
            Key2Validator(key2, alphabet);
            normalMessage = Normalize(normalMessage);
            StringValidator(normalMessage, alphabet);

            string permutedAlphabet = BuildPermutedAlphabet(key2, alphabet);

            StringBuilder encodedMessage = new();
            for (int i = 0; i < normalMessage.Length; i++)
            {
                if (!char.IsWhiteSpace(normalMessage[i]))
                {
                    int index = permutedAlphabet.IndexOf(char.ToUpper(normalMessage[i]));
                    int shiftedIndex = (index + key) % permutedAlphabet.Length;
                    char new_c = permutedAlphabet[shiftedIndex];
                    encodedMessage.Append(new_c);
                }
            }
            return encodedMessage.ToString();
        }

        public static string Decrypt(string normalMessage, int key, string key2, string alphabet)
        {
            Key1Validator(key, alphabet);
            Key2Validator(key2, alphabet);
            normalMessage = Normalize(normalMessage);
            StringValidator(normalMessage, alphabet);

            string permutedAlphabet = BuildPermutedAlphabet(key2, alphabet);

            StringBuilder decodedMessage = new();
            for (int i = 0; i < normalMessage.Length; i++)
            {
                if (!char.IsWhiteSpace(normalMessage[i]))
                {
                    int index = permutedAlphabet.IndexOf(char.ToUpper(normalMessage[i]));
                    int shiftedIndex = (index - (key % permutedAlphabet.Length) + permutedAlphabet.Length) % permutedAlphabet.Length;
                    char new_c = permutedAlphabet[shiftedIndex];
                    decodedMessage.Append(new_c);
                }
            }
            return decodedMessage.ToString();
        }
    }
}