using System;

namespace Svn2GitNetX
{
    public interface IFixer
    {
        void FixBranches();
        void FixTags();
        void FixTrunk();
        void OptimizeRepos();
    }
}