namespace TeamcityAPI.Util;

public static class LogUtil
{
    public static void Debug<T>(List<T>? list, string title = "") where T : class
    {
        if (list == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(title))
        {
            Console.WriteLine(title);
        }
        foreach (var item in list)
        {
            Console.WriteLine(item.ToString());
        }
        Console.WriteLine("");
    }
}