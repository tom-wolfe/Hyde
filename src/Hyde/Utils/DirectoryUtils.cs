namespace Hyde.Utils;

public static class DirectoryUtils
{
    public static void Clean(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        Array.ForEach(Directory.GetDirectories(dir), d => Directory.Delete(d, true));
        Array.ForEach(Directory.GetFiles(dir), File.Delete);
    }
}
