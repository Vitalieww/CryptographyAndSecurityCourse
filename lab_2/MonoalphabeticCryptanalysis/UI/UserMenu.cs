using System;
using System.Collections.Generic;
using MonoalphabeticCryptanalysis.Data;
using MonoalphabeticCryptanalysis.Models;
using MonoalphabeticCryptanalysis.Services;

namespace MonoalphabeticCryptanalysis.UI
{
    public class UserMenu
    {
        private string _ciphertext;
        private readonly FrequencyAnalyzer _analyzer = new();
        private readonly SubstitutionEngine _engine = new();
        private readonly DecryptionKey _currentKey = new();

        public UserMenu()
        {
            _ciphertext = SampleData.Variant23Ciphertext;
            _analyzer.Analyze(_ciphertext, _currentKey);
        }

        public void Display()
        {
            bool running = true;
            while (running)
            {
                ConsoleVisualizer.PrintHeader("LABORATORY NO. 2 - MONOALPHABETIC CIPHER CRYPTANALYSIS");
                Console.WriteLine($"Current Ciphertext: Variant 23 ({_analyzer.TotalLetters} letters, IoC = {_analyzer.IndexOfCoincidence:F4})");
                Console.WriteLine($"Currently Mapped Letters: {_currentKey.MappedCount} / 26\n");

                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. View Letter Frequency Statistics & Comparative Bar Chart");
                Console.WriteLine("2. View N-gram Analysis (Digraphs, Trigraphs, Double Letters)");
                Console.WriteLine("3. Interactive Manual Substitution REPL (Set / Remove letter mappings)");
                Console.WriteLine("4. Display Plaintext & Reconstructed Alphabet Key");
                Console.WriteLine("5. Reset Mappings / Reload Variant 23");
                Console.WriteLine("6. Exit");
                Console.Write("\nEnter choice (1-6): ");

                string? choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        DisplayFrequencies();
                        break;
                    case "2":
                        DisplayNgrams();
                        break;
                    case "3":
                        RunInteractiveRepl();
                        break;
                    case "4":
                        DisplayFullSolution();
                        break;
                    case "5":
                        _currentKey.Clear();
                        _analyzer.Analyze(_ciphertext, _currentKey);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Mappings reset to empty state.\n");
                        Console.ResetColor();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Please choose between 1 and 6.\n");
                        Console.ResetColor();
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress Enter to return to main menu...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private void DisplayFrequencies()
        {
            _analyzer.Analyze(_ciphertext, _currentKey);
            ConsoleVisualizer.PrintFrequencyTable(_analyzer.AlphabeticalFrequencies, "Alphabetical Frequency Table", showMapping: true);
            ConsoleVisualizer.PrintFrequencyTable(_analyzer.SortedFrequencies, "Descending Frequency Table", showMapping: true);
            ConsoleVisualizer.PrintFrequencyBarChart(_analyzer.SortedFrequencies);

            ConsoleVisualizer.PrintSubHeader("Index of Coincidence (IoC)");
            Console.WriteLine($"Observed IoC: {_analyzer.IndexOfCoincidence:F4}");
            Console.WriteLine("Reference values:");
            Console.WriteLine("  * Natural English text : ~0.0667 (0.065 - 0.068)");
            Console.WriteLine("  * Polyalphabetic / Random text : ~0.0385");
            Console.WriteLine($"Conclusion: Since {_analyzer.IndexOfCoincidence:F4} ≈ 0.066, this confirms monoalphabetic substitution.");
        }

        private void DisplayNgrams()
        {
            var digraphs = _analyzer.GetTopDigraphs(_ciphertext, 15);
            var trigraphs = _analyzer.GetTopTrigraphs(_ciphertext, 15);
            var doubles = _analyzer.GetTopDoubleLetters(_ciphertext, 10);

            ConsoleVisualizer.PrintNgramsTable("Top Ciphertext Digraphs (Observed)", digraphs);
            Console.WriteLine("Expected English Digraphs: " + string.Join(", ", SampleData.CommonDigraphs.Take(15)));

            ConsoleVisualizer.PrintNgramsTable("Top Ciphertext Trigraphs (Observed)", trigraphs);
            Console.WriteLine("Expected English Trigraphs: " + string.Join(", ", SampleData.CommonTrigraphs));

            ConsoleVisualizer.PrintNgramsTable("Top Repeated / Double Letters (Observed)", doubles);
            Console.WriteLine("Expected English Double Letters: " + string.Join(", ", SampleData.CommonDoubleLetters));
        }



        private void RunInteractiveRepl()
        {
            ConsoleVisualizer.PrintHeader("INTERACTIVE MANUAL SUBSTITUTION REPL");
            Console.WriteLine("Commands:");
            Console.WriteLine("  map <CIPHER> <plain>   : e.g. 'map W t'");
            Console.WriteLine("  unmap <CIPHER>         : e.g. 'unmap W'");
            Console.WriteLine("  preview                : Show current decrypted text snippet");
            Console.WriteLine("  key                    : Show current key mapping table");
            Console.WriteLine("  clear                  : Reset all mappings");
            Console.WriteLine("  exit                   : Return to main menu\n");

            while (true)
            {
                Console.Write("repl> ");
                string? line = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0].ToLowerInvariant();

                if (cmd == "exit" || cmd == "quit")
                {
                    break;
                }
                else if (cmd == "preview")
                {
                    ConsoleVisualizer.PrintColoredPartialText(_ciphertext, _currentKey, 600);
                }
                else if (cmd == "key")
                {
                    ConsoleVisualizer.PrintAlphabetMappingTable(_currentKey);
                }
                else if (cmd == "clear")
                {
                    _currentKey.Clear();
                    Console.WriteLine("All mappings cleared.");
                }
                else if (cmd == "map" && parts.Length >= 3)
                {
                    char c = parts[1][0];
                    char p = parts[2][0];
                    if (_currentKey.SetMapping(c, p))
                    {
                        Console.WriteLine($"Mapped Cipher '{char.ToUpperInvariant(c)}' -> Plain '{char.ToLowerInvariant(p)}'. Total mapped: {_currentKey.MappedCount}/26");
                    }
                    else
                    {
                        Console.WriteLine("Invalid characters.");
                    }
                }
                else if (cmd == "unmap" && parts.Length >= 2)
                {
                    char c = parts[1][0];
                    if (_currentKey.RemoveMapping(c))
                    {
                        Console.WriteLine($"Removed mapping for Cipher '{char.ToUpperInvariant(c)}'. Total mapped: {_currentKey.MappedCount}/26");
                    }
                    else
                    {
                        Console.WriteLine($"Cipher '{char.ToUpperInvariant(c)}' was not mapped.");
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command. Type 'map <C> <p>', 'unmap <C>', 'preview', 'key', 'clear', or 'exit'.");
                }
            }
        }

        private void DisplayFullSolution()
        {
            ConsoleVisualizer.PrintHeader("PLAINTEXT & SUBSTITUTION KEY");

            if (!_currentKey.IsComplete)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Note] Key has {_currentKey.MappedCount}/26 letters mapped. Unmapped letters are shown in UPPERCASE.\n");
                Console.ResetColor();
            }

            ConsoleVisualizer.PrintAlphabetMappingTable(_currentKey);

            ConsoleVisualizer.PrintSubHeader("Decrypted Plaintext (with current key)");
            string plaintext = _engine.ProgressiveSubstitute(_ciphertext, _currentKey);
            Console.WriteLine(plaintext);
        }
    }
}
