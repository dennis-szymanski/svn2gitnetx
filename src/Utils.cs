using System;
using System.Text.RegularExpressions;

namespace Svn2GitNetX
{
    public static class Utils
    {
        public static string EscapeQuotes( string source )
        {
            if( source == null )
            {
                throw new ArgumentNullException( "source" );
            }

            return Regex.Replace( source, "'|\"", ( Match m ) =>
             {
                 return "\\" + m.Value;
             } );
        }

        public static string RemoveFromTwoEnds( string source, char pattern )
        {
            if( source == null )
            {
                throw new ArgumentNullException( "source" );
            }

            char[] trimChars = { pattern };
            return source.TrimEnd( trimChars ).TrimStart( trimChars );
        }

        public static bool EqualsIgnoreCase( this string str, string other )
        {
            return str.Equals( other, StringComparison.InvariantCultureIgnoreCase );
        }
    }
}
