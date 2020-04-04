using System.Threading;
using Microsoft.Extensions.Logging;

namespace Svn2GitNet.Tests
{
    public static class TestHelper
    {
        public static ILoggerFactory CreateLoggerFactory()
        {
            ILoggerFactory factory = new LoggerFactory();
            factory.CreateLogger( nameof( Svn2GitNet ) );

            return factory;
        }

        public static ICommandRunner CreateCommandRunner()
        {
            return new CommandRunner(CreateLoggerFactory().CreateLogger<CommandRunner>(), false, new CancellationToken());
        }
    }
}