using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonoalphabeticCryptanalysis.Data;
using MonoalphabeticCryptanalysis.Models;

namespace MonoalphabeticCryptanalysis.Services
{
    public class FrequencyAnalyzer
    {
        public int TotalLetters { get; private set; }
        public double IndexOfCoincidence { get; private set; }
        public IReadOnlyList<LetterFrequency> AlphabeticalFrequencies { get; private set; } = [];
        public IReadOnlyList<LetterFrequency> SortedFrequencies { get; private set; } = [];

        public void Analyze(string text, DecryptionKey? currentKey = null)
        {
            var letterCounts = new Dictionary<char, int>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                letterCounts[c] = 0;
            }

            int count = 0;
            foreach (char ch in text)
            {
                if (char.IsLetter(ch))
                {
                    char upper = char.ToUpperInvariant(ch);
                    if (upper >= 'A' && upper <= 'Z')
                    {
                        letterCounts[upper]++;
                        count++;
                    }
                }
            }

            TotalLetters = count;

            // Calculate Index of Coincidence (IoC)
            if (TotalLetters > 1)
            {
                double sum = 0;
                foreach (var kvp in letterCounts)
                {
                    sum += (double)kvp.Value * (kvp.Value - 1);
                }
                IndexOfCoincidence = sum / ((double)TotalLetters * (TotalLetters - 1));
            }
            else
            {
                IndexOfCoincidence = 0.0;
            }

            // Build LetterFrequency objects
            var list = new List<LetterFrequency>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                int cCount = letterCounts[c];
                double pct = TotalLetters > 0 ? (cCount * 100.0 / TotalLetters) : 0.0;
                double expected = SampleData.StandardEnglishFrequencies.GetValueOrDefault(c, 0.0);

                char? plain = null;
                if (currentKey != null && currentKey.TryGetPlain(c, out char p))
                {
                    plain = p;
                }

                list.Add(new LetterFrequency(c, cCount, pct, expected, plain));
            }

            AlphabeticalFrequencies = list.AsReadOnly();
            SortedFrequencies = list.OrderByDescending(f => f.Count).ToList().AsReadOnly();
        }

        public List<NgramFrequency> GetTopDigraphs(string text, int top = 15)
        {
            return GetNgrams(text, 2, top);
        }

        public List<NgramFrequency> GetTopTrigraphs(string text, int top = 15)
        {
            return GetNgrams(text, 3, top);
        }

        public List<NgramFrequency> GetTopDoubleLetters(string text, int top = 10)
        {
            var dict = new Dictionary<string, int>();
            string clean = new(text.Where(char.IsLetter).Select(char.ToUpperInvariant).ToArray());

            for (int i = 0; i < clean.Length - 1; i++)
            {
                if (clean[i] == clean[i + 1])
                {
                    string pair = clean.Substring(i, 2);
                    dict[pair] = dict.GetValueOrDefault(pair, 0) + 1;
                }
            }

            int totalPairs = dict.Values.Sum();
            return dict
                .OrderByDescending(kv => kv.Value)
                .Take(top)
                .Select(kv => new NgramFrequency(kv.Key, kv.Value, totalPairs > 0 ? (kv.Value * 100.0 / totalPairs) : 0.0))
                .ToList();
        }

        private List<NgramFrequency> GetNgrams(string text, int n, int top)
        {
            var dict = new Dictionary<string, int>();
            string clean = new(text.Where(char.IsLetter).Select(char.ToUpperInvariant).ToArray());

            for (int i = 0; i <= clean.Length - n; i++)
            {
                string gram = clean.Substring(i, n);
                dict[gram] = dict.GetValueOrDefault(gram, 0) + 1;
            }

            int totalNgrams = clean.Length >= n ? clean.Length - n + 1 : 0;
            return dict
                .OrderByDescending(kv => kv.Value)
                .Take(top)
                .Select(kv => new NgramFrequency(kv.Key, kv.Value, totalNgrams > 0 ? (kv.Value * 100.0 / totalNgrams) : 0.0))
                .ToList();
        }
    }
}
