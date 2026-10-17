using System.Text;

namespace RomanianPlayfairCipher.Models
{
    public class PlayfairMatrix
    {
        // TODO: Define the 6x5 matrix structure and position mappings
        private const int Row = 6;
        private const int Col = 5;
        char[,] matrix = new char[Row, Col];

        private readonly Dictionary<char, (int Row, int Col)> lookupDict = [];

        string romanianAlphabet = "AĂÂBCDEFGHIÎKLMNOPQRSȘTȚUVWXYZ";

        PlayfairMatrix(string key)
        {
            key = key.ToUpper()
                .Replace('J', 'I')
                .Replace('Ş', 'Ș')
                .Replace('Ţ', 'Ț');
            StringBuilder result = new();
            HashSet<char> seen = [];

            foreach (char c in key)
            {
                if (!seen.Contains(c))
                {
                    seen.Add(c);
                    result.Append(c);
                }
            }

            foreach (char c in romanianAlphabet)
            {
                if (!seen.Contains(c))
                {
                    seen.Add(c);
                    result.Append(c);
                }
            }

            int currentChrIndx = 0;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {

                    char ch = result[currentChrIndx++];
                    matrix[i, j] = ch;
                    lookupDict[ch] = (i, j);
                }

            }
        }

    }
}
