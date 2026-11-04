using DesKeySchedule.Services;

namespace DesKeySchedule.UI
{
    public class ConsoleMenu
    {
        public static void Display()
        {
            Console.WriteLine("Welcome to step 2.3 of DES Algorithm");
            Console.WriteLine("1. Enter the key in bits.");
            Console.WriteLine("2. Enter the key in hex.");
            Console.WriteLine("3. Get a random key generated.");
            string? choice = Console.ReadLine();
            if (!int.TryParse(choice, out int intChoice))
            {
                Console.WriteLine("Invalid input. Please enter a valid whole number.");
            }
            switch (intChoice)
            {
                case 1:
                    {
                        Console.Write("Write the key in bits (56 bits):");
                        string? bitKey = Console.ReadLine();
                        if (BitUtils.ValidateBinaryKey(bitKey))
                        {
                            int roundsAmount = AskForRounds();
                            List<RoundState> rounds = KeyScheduleService.Pc2Rounds(bitKey, roundsAmount);
                            DisplayFormatter.PrintRounds(rounds);
                        }
                        else
                        {
                            Console.WriteLine("Invalid key!");
                        }
                        break;
                    }
                case 2:
                    {
                        Console.Write("Write the key in hex (14 hex): ");
                        string? hexKey = Console.ReadLine();
                        if (BitUtils.ValidateHexKey(hexKey))
                        {
                            int roundsAmount = AskForRounds();
                            List<RoundState> rounds = KeyScheduleService.Pc2Rounds(BitUtils.HexStringToBinaryString(hexKey), roundsAmount);
                            DisplayFormatter.PrintRounds(rounds);
                        }
                        else
                        {
                            Console.WriteLine("Invalid key!");
                        }
                        break;
                    }
                case 3:
                    {
                        string randomKey = RandomKeyGenerator.RandomBitsString(56);
                        Console.WriteLine($"The random key generated: {randomKey}");
                        int roundsAmount = AskForRounds();
                        List<RoundState> rounds = KeyScheduleService.Pc2Rounds(randomKey, roundsAmount);
                        DisplayFormatter.PrintRounds(rounds);
                        break;
                    }

                default:
                    {
                        Console.WriteLine("Invalid option.");
                        break;
                    }
            }
        }

        private static int AskForRounds()
        {
            int intAmount = 0;
            bool toContinue = true;
            while (toContinue)
            {
                Console.WriteLine("Enter the amount of rounds (between 1 and 16): ");
                string? roundsAmount = Console.ReadLine();
                if (!int.TryParse(roundsAmount, out intAmount))
                {
                    Console.WriteLine("Invalid input. Please enter a valid whole number (between 1 and 16).");
                }

                if (intAmount <= 16 && intAmount > 0)
                {
                    toContinue = false;
                }
                else
                {
                    Console.WriteLine("Invalid option (between 1 and 16).");
                }
            }
            return intAmount;
        }
    }
}
