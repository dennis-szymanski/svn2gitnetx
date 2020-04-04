using System;

namespace Svn2GitNetX
{
    public interface IGrabber
    {
        void FetchBranches();
        void FetchRebaseBraches();
        void Clone();
        MetaInfo GetMetaInfo();
    }
}