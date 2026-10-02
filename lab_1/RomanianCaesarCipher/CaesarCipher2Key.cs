using System.Text;

namespace lab_1
{
    public class CaesarCipher2Key
    {
        private static void Key1Validator(int key, string alphabet)
        {
            if (key < 0 || key > alphabet.Length - 1)
            {
                throw new ArgumentException($"The first key cannot be smaller than 0 or bigger than {alphabet.Length - 1}");
            }
        }

        private static void Key2Validator(string key, string alphabet)
        {
            int minLength = 7;
            if (key.Length < minLength)
            {
                throw new ArgumentException($"The second key is shorter than {minLength}");
            }
            key = key.ToUpper();
            foreach (char c in key)
            {
                if (!alphabet.Contains(c))
                {
                    throw new ArgumentException($"The second key contains a letter '{c}' that does not exist in the alphabet");
                }
            }
        }

        private static string Key2Normalize(string key)
        {
            key = Normalize(key).Replace(" ", "").ToUpper();
            key = RemoveDuplicates(key);
            return key;
        }

        private static string RemoveDuplicates(string input)
        {
            StringBuilder result = new();
            HashSet<char> seen = [];

            foreach (char c in input)
            {
                if (!seen.Contains(c))
                {
                    seen.Add(c);
                    result.Append(c);
                }
            }

            return result.ToString();
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

        private static string ConcatKey2ToAlphabet(string key, string alphabet)
        {
            string modifiedAlphabet = alphabet;
            foreach (char c in key)
            {
                if (modifiedAlphabet.Contains(c))
                {
                    modifiedAlphabet = modifiedAlphabet.Replace($"{c}", "");
                }
            }
            modifiedAlphabet = string.Concat(key, modifiedAlphabet);
            return modifiedAlphabet;
        }

        public static string Encrypt(string normalMessage, int key, string key2, string alphabet)
        {
            normalMessage = Normalize(normalMessage);
            key2 = Key2Normalize(key2);
            Key1Validator(key, alphabet);
            Key2Validator(key2, alphabet);
            StringValidator(normalMessage, alphabet);
            string modifiedAlphabet = ConcatKey2ToAlphabet(key2, alphabet);
            Console.WriteLine($"Permuted Alphabet: {modifiedAlphabet}");

            StringBuilder encodedMessage = new();
            for (int i = 0; i < normalMessage.Length; i++)
            {
                if (!char.IsWhiteSpace(normalMessage[i]))
                {
                    int index = alphabet.IndexOf(char.ToUpper(normalMessage[i]));
                    int shiftedIndex = (index + key) % modifiedAlphabet.Length;
                    char new_c = modifiedAlphabet[shiftedIndex];
                    encodedMessage.Append(new_c);
                }
            }
            return encodedMessage.ToString();
        }

        public static string Decrypt(string normalMessage, int key, string key2, string alphabet)
        {
            normalMessage = Normalize(normalMessage);
            key2 = Key2Normalize(key2);
            Key1Validator(key, alphabet);
            Key2Validator(key2, alphabet);
            StringValidator(normalMessage, alphabet);
            string modifiedAlphabet = ConcatKey2ToAlphabet(key2, alphabet);
            Console.WriteLine($"Permuted Alphabet: {modifiedAlphabet}");

            StringBuilder decodedMessage = new();
            for (int i = 0; i < normalMessage.Length; i++)
            {
                if (!char.IsWhiteSpace(normalMessage[i]))
                {
                    int index = modifiedAlphabet.IndexOf(char.ToUpper(normalMessage[i]));
                    int shiftedIndex = (index - (key % alphabet.Length) + alphabet.Length) % alphabet.Length;
                    char new_c = alphabet[shiftedIndex];
                    decodedMessage.Append(new_c);
                }
            }
            return decodedMessage.ToString();
        }
    }
}