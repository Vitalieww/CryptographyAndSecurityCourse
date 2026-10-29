using System.Text;

namespace RomanianPlayfairCipher.Models
{
    public class PlayfairMatrix
    {
        public const int Row = 6;
        public const int Col = 5;
        char[,] matrix = new char[Row, Col];

        private readonly Dictionary<char, (int Row, int Col)> lookupDict = [];

        public PlayfairMatrix(string key)
        {
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

            foreach (char c in AlphabetConfig.RomanianAlphabet)
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

        public (int Row, int Col) GetPosition(char c)
        {
            return lookupDict[c];
        }
        public char GetChar(int row, int col)
        {
            return matrix[row, col];
        }

        public void DisplayMatrix()
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                Console.Write("[ ");
                for (int j = 0; j < cols; j++)
                {
                    // Format each number with a fixed width (e.g., 6 spaces) for alignment
                    Console.Write($"{matrix[i, j],6} ");
                }
                Console.WriteLine(" ]");
            }
        }
    }
}
