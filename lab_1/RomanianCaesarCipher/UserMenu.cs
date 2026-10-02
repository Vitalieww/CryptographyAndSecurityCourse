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

        private void EncryptMessage(string caesarType)
        {
            try
            {
                string encryptedMessage = "";
                Console.Write("Enter the message: ");
                string message = Console.ReadLine() ?? string.Empty;
                if (caesarType == "Caesar-1")
                {
                    Console.Write("Enter the encryption key: ");
                    if (!int.TryParse(Console.ReadLine()?.Trim().Trim('\uFEFF'), out int key))
                    {
                        Console.WriteLine("Key must be a valid integer.");
                        return;
                    }
                    encryptedMessage = CaesarCipher.Encrypt(message, key, alphabet);
                }
                else if (caesarType == "Caesar-2")
                {
                    Console.Write("Enter key-1: ");
                    if (!int.TryParse(Console.ReadLine()?.Trim().Trim('\uFEFF'), out int key))
                    {
                        Console.WriteLine("Key must be a valid integer.");
                        return;
                    }
                    Console.Write("Enter encryption key-2: ");
                    string key2 = Console.ReadLine() ?? "";
                    encryptedMessage = CaesarCipher2Key.Encrypt(message, key, key2, alphabet);
                }
                Console.WriteLine($"Encrypted message: {encryptedMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DecryptMessage(string caesarType)
        {
            try
            {
                string decryptedMessage = "";
                if (caesarType == "Caesar-1")
                {
                    Console.Write("Enter the encrypted message: ");
                    string message = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter the decryption key: ");
                    if (!int.TryParse(Console.ReadLine()?.Trim().Trim('\uFEFF'), out int key))
                    {
                        Console.WriteLine("Key must be a valid integer.");
                        return;
                    }

                    decryptedMessage = CaesarCipher.Decrypt(message, key, alphabet);
                }
                else if (caesarType == "Caesar-2")
                {
                    Console.Write("Enter the encrypted message: ");
                    string message = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter decryption key-1: ");
                    if (!int.TryParse(Console.ReadLine()?.Trim().Trim('\uFEFF'), out int key))
                    {
                        Console.WriteLine("Key must be a valid integer.");
                        return;
                    }
                    Console.Write("Enter decryption key-2: ");
                    string key2 = Console.ReadLine() ?? "";
                    decryptedMessage = CaesarCipher2Key.Decrypt(message, key, key2, alphabet);
                }
                Console.WriteLine($"Decrypted message: {decryptedMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
