using Microsoft.Extensions.Logging;
// ReSharper disable ClassNeverInstantiated.Global

namespace WebJetMoviesAPI.Utils
{
    /// <summary>
    ///     Shared static logger factory, to be accessed through 
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