using System.Collections.Generic;

namespace Svn2GitNetX
{
    public interface IStaleSvnBranchDeleter
    {
        /// <summary>
        /// Calls out to SVN and fetches the branches at the HEAD revision.
        /// </summary>
        IEnumerable<string> QueryHeadSvnBranches();

        IEnumerable<string> GetGitBranchesToPurge( IEnumerable<string> headSvnBranches );

        IEnumerable<string> PurgeGitBranches( IEnumerable<string> branchesToPurge );
    }
}
