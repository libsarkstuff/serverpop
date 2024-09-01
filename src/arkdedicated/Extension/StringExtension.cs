using System.Text.RegularExpressions;

namespace arkdedicated.Extension;

public static class StringExtension
{
    private const string ShortServerNotation = "[a-zA-Z][0-9]{3,6}";
    public static string ArkShortName(this string longName)
    {
        return Regex.Match(longName, ShortServerNotation).Value;
    }
}