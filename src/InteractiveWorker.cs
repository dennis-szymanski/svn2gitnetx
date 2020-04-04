using Microsoft.Extensions.Logging;

namespace Svn2GitNet
{
    public class InteractiveWorker : Worker
    {
         public InteractiveWorker( 
            Options options,
            ICommandRunner commandRunner,
            string gitConfigCommandArguments,
            IMessageDisplayer messageDisplayer,
            ILogger logger 
        ) :
        base( options, commandRunner, messageDisplayer, logger )
        {
            this.GitConfigCommandArguments = gitConfigCommandArguments;
        }

        public string GitConfigCommandArguments { get; set; }
    }
}