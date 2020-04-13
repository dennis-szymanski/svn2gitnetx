using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Svn2GitNetX
{
    public static class OptionXmlParser
    {
        // ---------------- Fields ----------------

        internal const string RootXmlNodeName = "svn2gitnetx";

        internal const string OptionsXmlNodeName = "options";

        // -------- Option Element Names --------

        internal const string IsVerboseElement = "IsVerbose";
        internal const string IncludeMetaDataElement = "IncludeMetaData";
        internal const string NoMinimizeUrlElement = "NoMinimizeUrl";
        internal const string RootIsTrunkElement = "RootIsTrunk";
        internal const string SubpathToTrunkElement = "SubpathToTrunk";
        internal const string NoTrunkElement = "NoTrunk";
        internal const string BranchesElement = "Branches";
        internal const string BranchElement = "Branch";
        internal const string NoBranchesElement = "NoBranches";
        internal const string TagsElement = "Tags";
        internal const string TagElement = "Tag";
        internal const string NoTagsElement = "NoTags";
        internal const string ExcludesElement = "Excludes";
        internal const string ExcludeElement = "Exclude";
        internal const string RevisionElement = "Revision";
        internal const string UserNameElement = "UserName";
        internal const string UserNameMethodElement = "UserNameMethod";
        internal const string PasswordElement = "Password";
        internal const string PasswordMethodElement = "PasswordMethod";
        internal const string AuthorsFileElement = "AuthorsFile";
        internal const string BreakLocksElement = "BreakLocks";
        internal const string FetchAttemptsElement = "FetchAttempts";
        internal const string IgnoreGcErrorsElement = "IgnoreGcErrors";
        internal const string StaleSvnBranchPurgeOptionElement = "StaleSvnBranchPurgeOption";

        // ---------------- Functions ----------------

        public static Options ParseOptionFromFile( string fileName )
        {
            using( FileStream fs = new FileStream( fileName, FileMode.Open, FileAccess.Read ) )
            {
                return ParseOptionsFromStream( fs );
            }
        }

        public static Options ParseOptionFromString( string str )
        {
            using( MemoryStream stream = new MemoryStream( Encoding.UTF8.GetBytes( str ) ) )
            {
                return ParseOptionsFromStream( stream );
            }
        }

        public static Options ParseOptionsFromStream( Stream stream )
        {
            XDocument xmlDoc = XDocument.Load( stream );

            XElement rootNode = xmlDoc.Root;
            string rootName = rootNode.Name.LocalName ?? string.Empty;
            if( rootName != RootXmlNodeName )
            {
                throw new InvalidOperationException(
                    $"XML root name does not match what is expected.  Expected: {RootXmlNodeName}, Got: {rootName}"
                );
            }

            Options options = new Options();

            foreach( XElement childNode in rootNode.Descendants() )
            {
                if( childNode.Name.LocalName.EqualsIgnoreCase( OptionsXmlNodeName ) )
                {
                    ParseOptions( options, childNode );
                }
            }

            return options;
        }

        private static void ParseOptions( Options opt, XElement optionNode )
        {
            List<string> errors = new List<string>();

            List<string> branches = null;
            List<string> tags = null;
            List<string> excludes = null;

            void ConvertToBool( XElement element, Action<bool> setAction )
            {
                if( bool.TryParse( element.Value, out bool parsedValue ) )
                {
                    setAction( parsedValue );
                }
                else
                {
                    errors.Add( $"Could not convert option in {element.Name.LocalName} to a bool" );
                }
            }

            void ConvertToInt( XElement element, Action<int> setAction )
            {
                if( int.TryParse( element.Value, out int parsedValue ) )
                {
                    setAction( parsedValue );
                }
                else
                {
                    errors.Add( $"Could not convert option in {element.Name.LocalName} to an int" );
                }
            }

            void ConvertToEnum<TEnum>( XElement element, Action<TEnum> setAction ) where TEnum : struct
            {
                if( Enum.TryParse<TEnum>( element.Value, true, out TEnum parsedValue ) )
                {
                    setAction( parsedValue );
                }
                else
                {
                    errors.Add( $"Could not convert option in {element.Name.LocalName} to the correct of {typeof(TEnum).Name}" );
                }
            }

            foreach( XElement option in optionNode.Descendants() )
            {
                string optionName = option.Name.LocalName;
                if( optionName.EqualsIgnoreCase( IsVerboseElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.IsVerbose = o );
                }
                else if( optionName.EqualsIgnoreCase( IncludeMetaDataElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.IncludeMetaData = o );
                }
                else if( optionName.EqualsIgnoreCase( RootIsTrunkElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.RootIsTrunk = o );
                }
                else if( optionName.EqualsIgnoreCase( SubpathToTrunkElement ) )
                {
                    opt.SubpathToTrunk = option.Value;
                }
                else if( optionName.EqualsIgnoreCase( NoTrunkElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.NoTrunk = o );
                }
                else if( optionName.EqualsIgnoreCase( BranchesElement ) )
                {
                    if( branches == null )
                    {
                        branches = new List<string>();
                    }

                    foreach( XElement branch in option.Descendants() )
                    {
                        if( branch.Name.LocalName.EqualsIgnoreCase( BranchElement ) )
                        {
                            branches.Add( branch.Value );
                        }
                    }
                }
                else if( optionName.EqualsIgnoreCase( NoBranchesElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.NoBranches = o );
                }
                else if( optionName.EqualsIgnoreCase( TagsElement ) )
                {
                    if( tags == null )
                    {
                        tags = new List<string>();
                    }

                    foreach( XElement tag in option.Descendants() )
                    {
                        if( tag.Name.LocalName.EqualsIgnoreCase( TagElement ) )
                        {
                            tags.Add( tag.Value );
                        }
                    }
                }
                else if( optionName.EqualsIgnoreCase( NoTagsElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.NoTags = o );
                }
                else if( optionName.EqualsIgnoreCase( ExcludesElement ) )
                {
                    if( excludes == null )
                    {
                        excludes = new List<string>();
                    }

                    foreach( XElement exclude in option.Descendants() )
                    {
                        if( exclude.Name.LocalName.EqualsIgnoreCase( ExcludeElement ) )
                        {
                            excludes.Add( exclude.Value );
                        }
                    }
                }
                else if( optionName.EqualsIgnoreCase( RevisionElement ) )
                {
                    opt.Revision = option.Value;
                }
                else if( optionName.EqualsIgnoreCase( UserNameElement ) )
                {
                    opt.UserName = option.Value;
                }
                else if( optionName.EqualsIgnoreCase( UserNameMethodElement ) )
                {
                    ConvertToEnum<CredentialsMethod>( option, ( o ) => opt.UserNameMethod = o );
                }
                else if( optionName.EqualsIgnoreCase( PasswordElement ) )
                {
                    opt.Password = option.Value;
                }
                else if( optionName.EqualsIgnoreCase( PasswordMethodElement ) )
                {
                    ConvertToEnum<CredentialsMethod>( option, ( o ) => opt.PasswordMethod = o );
                }
                else if( optionName.EqualsIgnoreCase( AuthorsFileElement ) )
                {
                    opt.Authors = option.Value;
                }
                else if( optionName.EqualsIgnoreCase( BreakLocksElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.BreakLocks = o );
                }
                else if( optionName.EqualsIgnoreCase( FetchAttemptsElement ) )
                {
                    ConvertToInt( option, ( o ) => opt.FetchAttempts = o );
                }
                else if( optionName.EqualsIgnoreCase( IgnoreGcErrorsElement ) )
                {
                    ConvertToBool( option, ( o ) => opt.IgnoreGcErrors = o );
                }
                else if( optionName.EqualsIgnoreCase( StaleSvnBranchPurgeOptionElement ) )
                {
                    ConvertToEnum<StaleSvnBranchPurgeOptions>( option, ( o ) => opt.StaleSvnBranchPurgeOption = o );
                }
            } // End foreach

            if( errors.Count != 0 )
            {
                StringBuilder errorString = new StringBuilder();
                errorString.AppendLine( "Errors when validating XML config:" );
                foreach( string error in errors )
                {
                    errorString.AppendLine( "\t- " + error );
                }

                throw new InvalidOperationException( errorString.ToString() );
            }

            // Only override list elements if they were specified in the XML.
            if( branches != null )
            {
                opt.Branches = branches;
            }

            if( tags != null )
            {
                opt.Tags = tags;
            }

            if( excludes != null )
            {
                opt.Exclude = excludes;
            }
        }
    }
}
