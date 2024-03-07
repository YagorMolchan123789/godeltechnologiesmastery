using System.Text;

namespace Mastery.KeeFi.Business.Extensions;

public static class StackTraceExtensions
{
    public static string GetString(this string stackTrace) 
    {
        StringBuilder stackTraceBuilder = new StringBuilder(stackTrace);

        return stackTraceBuilder
               .ToString().Remove(stackTrace.IndexOf("at System.Runtime"))
               .Substring(stackTrace.IndexOf("at Mastery.KeeFi"))
               .ToString();
    }
}
