public static class AlphabetConfig
{
    // The standard 31-letter Romanian alphabet
    public const string RomanianAlphabet = "AĂÂBCDEFGHIÎKLMNOPQRSȘTȚUVWXYZ";

    public static string Normalize(string text)
    {
        return text.ToUpper()
            .Replace('J', 'I')
            .Replace('Ş', 'Ș')
            .Replace('Ţ', 'Ț');
    }

    public static bool IsValidMessage(string text, out string errorMessage)
    {
        foreach (char c in text)
        {
            if (!RomanianAlphabet.Contains(c))
            {
                errorMessage = $"Char '{c}' is not allowd, only (A-Z, Ă, Â, Î, Ș, Ț).";
                return false;
            }
        }
        errorMessage = string.Empty;
        return true;
    }

    // Validates a key (no spaces, length >= 7)
    public static bool IsValidKey(string? key, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            errorMessage = "Key cannot be empty.";
            return false;
        }

        if (key.Length < 7)
        {
            errorMessage = $"Key '{key}' is too short ({key.Length} chars). Min length is 7.";
            return false;
        }

        foreach (char c in key)
        {
            if (!RomanianAlphabet.Contains(c) || char.IsWhiteSpace(c))
            {
                errorMessage = $"Char '{c}' is not allowed, only Romanian alphabet characters are allowed.";
                return false;
            }
        }

        errorMessage = string.Empty;
        return true;
    }

}
