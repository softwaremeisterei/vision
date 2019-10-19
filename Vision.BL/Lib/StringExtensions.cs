using System;

public static class StringExtensions
{
    public static string Shorten(this string name, int length)
    {
        if (name == null) { return name; }

        if (name.Length > length)
        {
            name = name.Substring(0, length - (Math.Min(length, 3))) + "...";
        }

        return name;
    }
}
