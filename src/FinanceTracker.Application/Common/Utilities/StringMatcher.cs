namespace FinanceTracker.Application.Common.Utilities;

public static class StringMatcher
{
    public static double CalculateSimilarity(string? source, string? target)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target)) return 0.0;

        var s = source.Trim().ToLowerInvariant();
        var t = target.Trim().ToLowerInvariant();

        if (s == t) return 1.0;

        if (s.Contains(t) || t.Contains(s)) return 0.90;

        int distance = ComputeLevenshteinDistance(s, t);
        int maxLength = Math.Max(s.Length, t.Length);

        return 1.0 - ((double)distance / maxLength);
    }

    private static int ComputeLevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}
