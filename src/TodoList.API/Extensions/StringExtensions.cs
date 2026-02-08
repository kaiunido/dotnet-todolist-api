using System.Text.RegularExpressions;

namespace TodoList.API.Extensions;

public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var result = Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2");
        return result.ToLowerInvariant();
    }
}