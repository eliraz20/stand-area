using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
if (args.Length > 0 && args[0] == "inventory")
{
    try { InventoryReport.Run(args); return 0; }
    catch (Exception e) when (e is FormatException or IOException or ArgumentException or OverflowException or UnauthorizedAccessException)
    { Console.Error.WriteLine(e.Message); return 1; }
}

if (args.Length != 1 || args[0] == "--help")
{
    Console.WriteLine("Usage: dotnet run -- inventory study.tsv  |  original format: <input-file|->");
    return args.Length == 1 ? 0 : 2;
}

try
{
    string[] lines = args[0] == "-" ? Console.In.ReadToEnd().ReplaceLineEndings("\n").TrimEnd('\n').Split('\n') : File.ReadAllLines(args[0]);
    Check(lines.Length >= 2 && lines.Any(line => !string.IsNullOrWhiteSpace(line)), "Input is empty or missing records.");
    var area = (double)Num(lines[0]);
    Check(area > 0, "Plot area must be positive.");
    foreach (var group in lines.Skip(1).Select(s => Fields(s, 2)).GroupBy(f => f[0]))
    {
        double basal = 0;
        foreach (var f in group)
        {
            var d = (double)Num(f[1]);
            Check(d > 0, "Diameter must be positive.");
            basal += Math.PI * Math.Pow(d / 200, 2);
        }

        Console.WriteLine($"{group.Key}: {group.Count()} stems, {basal:0.000} m² sampled; {basal * 10000 / area:0.000} m²/ha");
    }

    return 0;
}
catch (Exception e)when (e is IOException or FormatException or OverflowException or ArgumentException or UnauthorizedAccessException or DivideByZeroException)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}

static void Check(bool valid, string message)
{
    if (!valid)
        throw new FormatException(message);
}

static string[] Fields(string line, int count)
{
    var fields = line.Split('\t', StringSplitOptions.TrimEntries);
    Check(fields.Length == count && fields.All(s => s.Length > 0), $"Expected {count} tab-separated values in '{line}'.");
    return fields;
}

static decimal Num(string value) => decimal.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
