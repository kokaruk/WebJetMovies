using Microsoft.Extensions.Logging;

namespace WebJetMoviesAPI.Utils
{
    /// <summary>
    ///     Shared logger
    /// </summary>
    public class StaticLogger
    {
        internal static ILoggerFactory LoggerFactory { get; set; } // = new LoggerFactory();

        internal static ILogger CreateLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }

        internal static ILogger CreateLogger(string categoryName)
        {
            return LoggerFactory.CreateLogger(categoryName);
        }
    }
}