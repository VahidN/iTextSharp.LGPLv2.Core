namespace System.util;

public static class RealExtensions
{
    private const double Epsilon = 2.2204460492503131E-16;

    public static bool ApproxEquals(this double d1, double d2)
    {
        if (d1 == d2)
        {
            return true;
        }

        var tolerance = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * Epsilon;
        var difference = d1 - d2;

        return -tolerance < difference && tolerance > difference;
    }

    public static bool ApproxEquals(this float d1, float d2)
    {
        if (d1 == d2)
        {
            return true;
        }

        var tolerance = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * Epsilon;
        var difference = d1 - d2;

        return -tolerance < difference && tolerance > difference;
    }

    public static bool ApproxGreaterEqual(this double a, double b) => ApproxEquals(a, b) || a > b;

    public static bool ApproxGreaterEqual(this float a, float b) => ApproxEquals(a, b) || a > b;

    public static bool ApproxGreaterThan(this double a, double b) => !ApproxEquals(a, b) && a > b;

    public static bool ApproxGreaterThan(this float a, float b) => !ApproxEquals(a, b) && a > b;

    public static bool ApproxLowerEqual(this double a, double b) => ApproxEquals(a, b) || a < b;

    public static bool ApproxLowerEqual(this float a, float b) => ApproxEquals(a, b) || a < b;

    public static bool ApproxLowerThan(this double a, double b) => !ApproxEquals(a, b) && a < b;

    public static bool ApproxLowerThan(this float a, float b) => !ApproxEquals(a, b) && a < b;

    public static bool ApproxNotEqual(this double a, double b) => !ApproxEquals(a, b);

    public static bool ApproxNotEqual(this float a, float b) => !ApproxEquals(a, b);
}