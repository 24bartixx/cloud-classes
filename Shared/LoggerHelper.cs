using System.Runtime.CompilerServices;

namespace Shared;

public static class LoggerHelper
{
    public static void LogCall(string message = "", [CallerMemberName] string methodName = "")
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: {methodName} | {message}");
    }
}