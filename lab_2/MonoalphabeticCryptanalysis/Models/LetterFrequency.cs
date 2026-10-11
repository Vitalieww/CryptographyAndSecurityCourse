namespace MonoalphabeticCryptanalysis.Models
{
    public class LetterFrequency
    {
        public char Letter { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
        public double ExpectedEnglishPercentage { get; set; }
        public char? DecryptedTo { get; set; }

        public LetterFrequency(char letter, int count, double percentage, double expectedPercentage = 0, char? decryptedTo = null)
        {
            Letter = letter;
            Count = count;
            Percentage = percentage;
            ExpectedEnglishPercentage = expectedPercentage;
            DecryptedTo = decryptedTo;
        }
    }

    public class NgramFrequency
    {
        public string Ngram { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }

        public NgramFrequency(string ngram, int count, double percentage)
        {
            Ngram = ngram;
            Count = count;
            Percentage = percentage;
        }
    }
}
