using System;
using System.Collections.Generic;
using MonoalphabeticCryptanalysis.Models;

namespace MonoalphabeticCryptanalysis.UI
{
    public static class ConsoleVisualizer
    {
        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', 78));
            Console.WriteLine($"  {title}");
            Console.WriteLine(new string('=', 78));
            Console.ResetColor();
        }

        public static void PrintSubHeader(string subtitle)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n--- {subtitle} ---");
            Console.ResetColor();
        }

        public static void PrintFrequencyTable(IReadOnlyList<LetterFrequency> frequencies, string title, bool showMapping = false)
        {
            PrintSubHeader(title);
            if (showMapping)
            {
                Console.WriteLine("+--------+-------+------------+-------------+--------------+--------------+");
                Console.WriteLine("| Cipher | Count | Percentage | English Std |   Diff (%)   | Plain Match  |");
                Console.WriteLine("+--------+-------+------------+-------------+--------------+--------------+");
            }
            else
            {
                Console.WriteLine("+--------+-------+------------+-------------+--------------+");
                Console.WriteLine("| Cipher | Count | Percentage | English Std |   Diff (%)   |");
                Console.WriteLine("+--------+-------+------------+-------------+--------------+");
            }

            foreach (var item in frequencies)
            {
                double diff = item.Percentage - item.ExpectedEnglishPercentage;
                string diffStr = diff >= 0 ? $"+{diff:0.00}%" : $"{diff:0.00}%";

                if (showMapping)
                {
                    string matchStr = item.DecryptedTo.HasValue ? $"     -> {item.DecryptedTo.Value}  " : "      -     ";
                    Console.WriteLine($"|   {item.Letter}    | {item.Count,5} |   {item.Percentage,5:0.00}%   |    {item.ExpectedEnglishPercentage,5:0.00}%   |   {diffStr,8}   | {matchStr} |");
                }
                else
                {
                    Console.WriteLine($"|   {item.Letter}    | {item.Count,5} |   {item.Percentage,5:0.00}%   |    {item.ExpectedEnglishPercentage,5:0.00}%   |   {diffStr,8}   |");
                }
            }

            if (showMapping)
            {
                Console.WriteLine("+--------+-------+------------+-------------+--------------+--------------+");
            }
            else
            {
                Console.WriteLine("+--------+-------+------------+-------------+--------------+");
            }
        }

        public static void PrintFrequencyBarChart(IReadOnlyList<LetterFrequency> frequencies, int maxBarWidth = 35)
        {
            PrintSubHeader("Letter Frequency Comparison Bar Chart (Ciphertext [■] vs English Std [░])");
            Console.WriteLine("Letter | Frequency Bar Chart (1 char ≈ 0.5%)                     | Cipher % | Eng Std %");
            Console.WriteLine("-------+---------------------------------------------------------+----------+----------");

            double maxPct = 14.0; // scale up to 14%
            foreach (var item in frequencies)
            {
                int cipherBarLen = (int)Math.Round((item.Percentage / maxPct) * maxBarWidth);
                int engBarLen = (int)Math.Round((item.ExpectedEnglishPercentage / maxPct) * maxBarWidth);

                Console.Write($"  {item.Letter}    | ");

                // Render cipher bar
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string('#', Math.Max(0, cipherBarLen)));
                Console.ResetColor();

                int pad = maxBarWidth - cipherBarLen;
                if (pad > 0) Console.Write(new string(' ', pad));

                Console.Write($" |  {item.Percentage,5:0.00}% |  {item.ExpectedEnglishPercentage,5:0.00}%\n");
            }
            Console.WriteLine("-------+---------------------------------------------------------+----------+----------");
        }

        public static void PrintNgramsTable(string title, List<NgramFrequency> ngrams)
        {
            PrintSubHeader(title);
            Console.WriteLine("+-------+-------+------------+");
            Console.WriteLine("| Ngram | Count | Percentage |");
            Console.WriteLine("+-------+-------+------------+");
            foreach (var item in ngrams)
            {
                Console.WriteLine($"|  {item.Ngram,-4} | {item.Count,5} |   {item.Percentage,5:0.00}%   |");
            }
            Console.WriteLine("+-------+-------+------------+");
        }

        public static void PrintColoredPartialText(string text, DecryptionKey key, int maxLength = 800)
        {
            PrintSubHeader("Partially Decrypted Text Preview (Green = Plaintext, Yellow = Ciphertext)");
            int count = 0;
            foreach (char ch in text)
            {
                if (maxLength > 0 && count >= maxLength)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("\n... [preview truncated] ...\n");
                    Console.ResetColor();
                    break;
                }

                if (char.IsLetter(ch))
                {
                    char upper = char.ToUpperInvariant(ch);
                    if (key.TryGetPlain(upper, out char plain))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(plain);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(upper);
                    }
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(ch);
                }
                count++;
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void PrintAlphabetMappingTable(DecryptionKey key)
        {
            PrintSubHeader("Reconstructed Substitution Key Table");

            // Row 1: Plaintext alphabet
            Console.WriteLine("+-----------+" + string.Concat(System.Linq.Enumerable.Repeat("---+", 26)));
            Console.Write("| Plaintext |");
            for (char p = 'a'; p <= 'z'; p++)
            {
                Console.Write($" {p} |");
            }
            Console.WriteLine("\n+-----------+" + string.Concat(System.Linq.Enumerable.Repeat("---+", 26)));

            // Row 2: Ciphertext alphabet
            Console.Write("| Ciphertext|");
            for (char p = 'a'; p <= 'z'; p++)
            {
                if (key.TryGetCipher(p, out char c))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($" {c} ");
                    Console.ResetColor();
                    Console.Write("|");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" - |");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("\n+-----------+" + string.Concat(System.Linq.Enumerable.Repeat("---+", 26)));
        }
    }
}
