using System.Security.Cryptography;
using System.Text;
using RomanianPlayfairCipher.Models;

namespace RomanianPlayfairCipher.Services
{
    public class PlayfairCipher
    {
        public List<(char, char)> PrepareText(string cleanText)
        {
            cleanText = cleanText.ToUpper()
                .Replace('J', 'I')
                .Replace('Ş', 'Ș')
                .Replace('Ţ', 'Ț')
                .Replace(" ", string.Empty);
            int i = 0;
            List<(char, char)> pairs = [];
            while (i < cleanText.Length)
            {
                char first = cleanText[i];
                //if last letter is lonely
                if (i == cleanText.Length - 1)
                {
                    if (first == 'X')
                    {
                        pairs.Add((first, 'Z'));
                    }
                    else
                    {
                        pairs.Add((first, 'X'));
                    }
                    i++;
                }
                // if double letters in same pair
                else if (first == cleanText[i + 1])
                {
                    if (first == 'X')
                    {
                        pairs.Add((first, 'Z'));
                    }
                    else
                    {
                        pairs.Add((first, 'X'));
                    }
                    i++;
                }
                //If not double letters
                else
                {
                    pairs.Add((first, cleanText[i + 1]));
                    i += 2;
                }
            }
            return pairs;
        }
        public string Encrypt(string plainText, PlayfairMatrix matrix)
        {
            List<(char, char)> preparedText = PrepareText(plainText);
            StringBuilder cipheredChars = new();
            foreach ((char, char) pair in preparedText)
            {
                char c1 = pair.Item1;
                char c2 = pair.Item2;

                var (r1, col1) = matrix.GetPosition(c1);
                var (r2, col2) = matrix.GetPosition(c2);

                char newLetter1;
                char newLetter2;

                if (r1 == r2) // rule for same row
                {
                    int newCol1 = (col1 + 1) % PlayfairMatrix.Col;
                    int newCol2 = (col2 + 1) % PlayfairMatrix.Col;

                    newLetter1 = matrix.GetChar(r1, newCol1);
                    newLetter2 = matrix.GetChar(r2, newCol2);

                }
                else if (col1 == col2)
                {
                    int newRow1 = (r1 + 1) % PlayfairMatrix.Row;
                    int newRow2 = (r2 + 1) % PlayfairMatrix.Row;

                    newLetter1 = matrix.GetChar(newRow1, col1);
                    newLetter2 = matrix.GetChar(newRow2, col2);
                }
                else
                {
                    newLetter1 = matrix.GetChar(r1, col2);
                    newLetter2 = matrix.GetChar(r2, col1);
                }
                cipheredChars.Append(newLetter1).Append(newLetter2);
            }
            return cipheredChars.ToString();
        }

        public string Decrypt(string cipheredText, PlayfairMatrix matrix)
        {
            string cleanCipher = cipheredText.ToUpper()
                .Replace('J', 'I')
                .Replace('Ş', 'Ș')
                .Replace('Ţ', 'Ț')
                .Replace(" ", string.Empty);

            if (cleanCipher.Length % 2 != 0)
            {
                throw new ArgumentException("Ciphered text must have an even amount of characters.");
            }

            StringBuilder unchipheredText = new();
            for (int i = 0; i < cleanCipher.Length; i += 2)
            {
                char c1 = cleanCipher[i];
                char c2 = cleanCipher[i + 1];

                var (r1, col1) = matrix.GetPosition(c1);
                var (r2, col2) = matrix.GetPosition(c2);

                char newLetter1;
                char newLetter2;

                if (r1 == r2) //rule for same row
                {
                    int newCol1 = (col1 - 1 + PlayfairMatrix.Col) % PlayfairMatrix.Col;
                    int newCol2 = (col2 - 1 + PlayfairMatrix.Col) % PlayfairMatrix.Col;

                    newLetter1 = matrix.GetChar(r1, newCol1);
                    newLetter2 = matrix.GetChar(r2, newCol2);
                }
                else if (col1 == col2)
                {
                    int newRow1 = (r1 - 1 + PlayfairMatrix.Row) % PlayfairMatrix.Row;
                    int newRow2 = (r2 - 1 + PlayfairMatrix.Row) % PlayfairMatrix.Row;

                    newLetter1 = matrix.GetChar(newRow1, col1);
                    newLetter2 = matrix.GetChar(newRow2, col2);
                }
                else
                {
                    newLetter1 = matrix.GetChar(r1, col2);
                    newLetter2 = matrix.GetChar(r2, col1);
                }
                unchipheredText.Append(newLetter1).Append(newLetter2);
            }
            return unchipheredText.ToString();
        }
    }
}
