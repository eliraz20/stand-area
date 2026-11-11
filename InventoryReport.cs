static class InventoryReport
{
    public static void Run(string[] args)
    {
        if (args.Length != 2)
            throw new FormatException("Usage: inventory study.tsv");
        var rows = Rows(args[1]);
        var areas = new Dictionary<string, double>();
        var stems = new List<(string Plot, string Id, string Species, double Basal)>();
        var unique = new HashSet<(string, string)>();
        foreach (var r in rows)
        {
            if (r is ["plot", var id, var area])
                Require(areas.TryAdd(id, (double)Positive(area)), "Duplicate plot.");
            else if (r is ["stem", var plot, var stem, var species, var dbh])
            {
                Require(unique.Add((plot, stem)), "Duplicate stem.");
                var radius = (double)Positive(dbh) / 200;
                stems.Add((plot, stem, species, Math.PI * radius * radius));
            }
            else
                throw new FormatException("Expected plot or stem record.");
        }

        Require(areas.Count > 0 && stems.All(s => areas.ContainsKey(s.Plot)), "Every stem needs a declared plot.");
        foreach (var(plot, area)in areas)
        {
            var subset = stems.Where(s => s.Plot == plot).ToArray();
            Console.WriteLine($"{plot}: {subset.Length} stems, {subset.Sum(s => s.Basal) * 10000 / area:0.000} m²/ha");
            foreach (var g in subset.GroupBy(s => s.Species))
                Console.WriteLine($"  {g.Key}: {g.Sum(s => s.Basal) * 10000 / area:0.000} m²/ha");
        }

        foreach (var g in stems.GroupBy(s => s.Species))
            Console.WriteLine($"pooled {g.Key}: {g.Sum(s => s.Basal) * 10000 / areas.Values.Sum():0.000} m²/ha");
        Console.WriteLine($"pooled all: {stems.Sum(s => s.Basal) * 10000 / areas.Values.Sum():0.000} m²/ha");
    }

    static List<string[]> Rows(string file, int width = 0, bool allowEmpty = false)
    {
        using var reader = file == "-" ? Console.In : File.OpenText(file);
        var rows = new List<string[]>();
        int line = 0;
        while (reader.ReadLine()is string s)
        {
            line++;
            if (string.IsNullOrWhiteSpace(s) || s.TrimStart().StartsWith("#"))
                continue;
            var cells = s.Split('\t', StringSplitOptions.TrimEntries);
            if ((width > 0 && cells.Length != width) || (!allowEmpty && cells.Any(string.IsNullOrEmpty)))
                throw new FormatException($"Invalid columns on line {line}.");
            rows.Add(cells);
        }

        if (rows.Count == 0)
            throw new FormatException("Empty input.");
        return rows;
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
            throw new FormatException(message);
    }

    static decimal Amount(string text)
    {
        var value = decimal.Parse(text, System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint);
        Require(value >= 0, "Quantity must be nonnegative.");
        return value;
    }

    static decimal Positive(string text)
    {
        var value = Amount(text);
        Require(value > 0, "Quantity must be positive.");
        return value;
    }
}
