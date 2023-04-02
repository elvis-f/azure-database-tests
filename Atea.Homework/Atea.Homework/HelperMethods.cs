using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Atea.Homework;

public class HelperMethods
{
    public static string GetParameter(HttpRequest req, string str)
    {
        return HttpUtility.UrlDecode(req.Query[str].FirstOrDefault());
    }

    public static string ParseStream(MemoryStream stream)
    {
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static DateTimeOffset? ParseDto(string str, string format = "dd-MM-yyyy")
    {
        try
        {
            return DateTimeOffset.ParseExact(str, format, null);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}