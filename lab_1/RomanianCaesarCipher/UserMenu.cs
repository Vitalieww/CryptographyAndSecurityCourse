using System;

namespace lab_1
{
    public class UserMenu(string alphabet)
    {
        public void Display()
        {
            bool toContinue = true;
            while (toContinue)
            {
                Console.WriteLine("Choose an option: ");
                Console.WriteLine("1. 1-Key Caesar Cipher");
                Console.WriteLine("2. 2-Key Caesar Cipher");
                Console.WriteLine("3. Exit");
                string? raw = Console.ReadLine();
                if (raw == null)
                {
                    break;
                }
                string input = raw.Trim().Trim('\uFEFF');
                if (!int.TryParse(input, out int userChoice))
                {
                    Console.WriteLine("Please choose a valid option");
                    continue;
                }
                if (userChoice == 1)
                {
                    DisplayCaesarOptions("Caesar-1");
                }
                else if (userChoice == 2)
                {
                    DisplayCaesarOptions("Caesar-2");
                }
                else if (userChoice == 3)
                {
                    toContinue = false;
                }
                else
                {
                    Console.WriteLine("Please choose a valid option.");
                }
            }
        }
        public void DisplayCaesarOptions(string caesarType)
        {
            bool toContinue = true;
            while (toContinue)
            {
                Console.WriteLine("Choose an option: ");
                Console.WriteLine("1. Encrypt");
                Console.WriteLine("2. Decrypt");
                Console.WriteLine("3. Exit");
                string? raw = Console.ReadLine();
                if (raw == null)
                {
                    break;
                }
                string input = raw.Trim().Trim('\uFEFF');
                if (!int.TryParse(input, out int userChoice))
                {
                    Console.WriteLine("Please choose a valid option");
                    continue;
                }
                if (userChoice == 1)
                {
                    EncryptMessage(caesarType);
                }
                else if (userChoice == 2)
                {
                    DecryptMessage(caesarType);
                }
                else if (userChoice == 3)
                {
                    toContinue = false;
                }
                else
                {
                    Console.WriteLine("Please choose a valid option");
                }
            }
        }

        private string? ReadValidMessage(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? raw = Console.ReadLine();
                if (raw == null)
                {
                    return null;
                }
                string message = raw;
                string normalized = CaesarCipher2Key.Normalize(message);
                bool valid = true;
                foreach (char c in normalized)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        continue;
                    }
                    if (!alphabet.Contains(char.ToUpper(c)))
                    {
                        Console.WriteLine($"Error: Invalid character '{c}' rejected. Allowed characters are Romanian alphabet letters (A-Z, Ă, Â, Î, Ș, Ț).");
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    return message;
                }
                Console.WriteLine("Please enter the text again.");
            }
        }

        private int? ReadValidKey(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? raw = Console.ReadLine();
                if (raw == null)
                {
                    return null;
                }
                raw = raw.Trim().Trim('\uFEFF');
                if (int.TryParse(raw, out int key) && key >= 1 && key <= alphabet.Length - 1)
                {
                    return key;
                }
                Console.WriteLine($"Error: Invalid key. Allowed range is an integer between 1 and {alphabet.Length - 1} inclusive.");
                Console.WriteLine("Please enter the key again.");
            }
        }

        private string? ReadValidKey2(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? raw = Console.ReadLine();
                if (raw == null)
                {
                    return null;
                }
                string key2 = raw;
                string normalized = CaesarCipher2Key.Normalize(key2).Replace(" ", "");
                if (normalized.Length < 7)
                {
                    Console.WriteLine($"Error: Key 2 (keyword) must be at least 7 characters long. (Entered length: {normalized.Length}).");
                    Console.WriteLine("Please enter the keyword again.");
                    continue;
                }

                bool valid = true;
                foreach (char c in normalized)
                {
                    if (!alphabet.Contains(char.ToUpper(c)))
                    {
                        Console.WriteLine($"Error: Invalid character '{c}' rejected in keyword. Only Romanian alphabet letters are allowed.");
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return key2;
                }
                Console.WriteLine("Please enter the keyword again.");
            }
        }

        private void EncryptMessage(string caesarType)
        {
            string? message = ReadValidMessage("Enter the message: ");
            if (message == null) return;
            string encryptedMessage;

            if (caesarType == "Caesar-1")
            {
                int? key = ReadValidKey($"Enter the encryption key (1 - {alphabet.Length - 1}): ");
                if (key == null) return;
                encryptedMessage = CaesarCipher.Encrypt(message, key.Value, alphabet);
            }
            else
            {
                int? key = ReadValidKey($"Enter key-1 (shift: 1 - {alphabet.Length - 1}): ");
                if (key == null) return;
                string? key2 = ReadValidKey2("Enter encryption key-2 (keyword >= 7 characters): ");
                if (key2 == null) return;
                string permutedAlphabet = CaesarCipher2Key.BuildPermutedAlphabet(key2, alphabet);
                Console.WriteLine($"Permuted Alphabet: {permutedAlphabet}");
                encryptedMessage = CaesarCipher2Key.Encrypt(message, key.Value, key2, alphabet);
            }

            Console.WriteLine($"Encrypted message: {encryptedMessage}");
        }

        private void DecryptMessage(string caesarType)
        {
            string? message = ReadValidMessage("Enter the encrypted message: ");
            if (message == null) return;
            string decryptedMessage;

            if (caesarType == "Caesar-1")
            {
                int? key = ReadValidKey($"Enter the decryption key (1 - {alphabet.Length - 1}): ");
                if (key == null) return;
                decryptedMessage = CaesarCipher.Decrypt(message, key.Value, alphabet);
            }
            else
            {
                int? key = ReadValidKey($"Enter decryption key-1 (shift: 1 - {alphabet.Length - 1}): ");
                if (key == null) return;
                string? key2 = ReadValidKey2("Enter decryption key-2 (keyword >= 7 characters): ");
                if (key2 == null) return;
                string permutedAlphabet = CaesarCipher2Key.BuildPermutedAlphabet(key2, alphabet);
                Console.WriteLine($"Permuted Alphabet: {permutedAlphabet}");
                decryptedMessage = CaesarCipher2Key.Decrypt(message, key.Value, key2, alphabet);
            }

            Console.WriteLine($"Decrypted message: {decryptedMessage}");
        }
    }
}
