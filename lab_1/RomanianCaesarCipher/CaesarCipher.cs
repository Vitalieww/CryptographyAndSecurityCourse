
using System.Text;

namespace lab_1
{
    public class CaesarCipher
    {
        private static void KeyValidator(int key, string alphabet)
        {
            if (key < 0 || key > alphabet.Length - 1)
            {
                throw new ArgumentException($"Key cannot be smaller than 0 or bigger than {alphabet.Length - 1}");
            }
        }

        private static string Normalize(string message)
        {
            return message
                .Replace('ş', 'ș')
                .Replace('Ş', 'Ș')
                .Replace('ţ', 'ț')
                .Replace('Ţ', 'Ț');
        }

        private static void StringValidator(string message, string alphabet)
        {
            foreach (char c in message)
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                if (!alphabet.Contains(char.ToUpper(c)))
                {
                    throw new ArgumentException($"The message contains a letter '{c}' that does not exist in the alphabet");
                }
            }
        }

        public static string Encrypt(string normalMessage, int key, string alphabet)
        {
            normalMessage = Normalize(normalMessage);
            KeyValidator(key, alphabet);
            StringValidator(normalMessage, alphabet);
            StringBuilder encodedMessage = new();
            for (int i = 0; i < normalMessage.Length; i++)
            {
                if (!char.IsWhiteSpace(normalMessage[i]))
                {
                    int index = alphabet.IndexOf(char.ToUpper(normalMessage[i]));
                    int shiftedIndex = (index + key) % alphabet.Length;
                    char new_c = alphabet[shiftedIndex];
                    encodedMessage.Append(new_c);
                }
            }
            return encodedMessage.ToString();
        }

        public static string Decrypt(string normalMessage, int key, string alphabet)
        {
            normalMessage = Normalize(normalMessage);
            KeyValidator(key, alphabet);
            StringValidator(normalMessage, alphabet);
            StringBuilder decodedMessage = new();
            for (int i = 0; i < normalMessage.Length; i++)
            {
                if (!char.IsWhiteSpace(normalMessage[i]))
                {
                    int index = alphabet.IndexOf(char.ToUpper(normalMessage[i]));
                    int shiftedIndex = (index - (key % alphabet.Length) + alphabet.Length) % alphabet.Length;
                    char new_c = alphabet[shiftedIndex];
                    decodedMessage.Append(new_c);
                }
            }
            return decodedMessage.ToString();
        }
    }
}