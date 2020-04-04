using System;

namespace Svn2GitNetX
{
    public class ConsoleMessageDisplayer : IMessageDisplayer
    {
        public void Show( string message )
        {
            Console.WriteLine( message );
        }
    }
}