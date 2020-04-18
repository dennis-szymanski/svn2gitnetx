namespace Svn2GitNetX
{
    public interface IGitPusher
    {
        /// <summary>
        /// Calls 'git push --all'.
        /// </summary>
        void PushAll();

        /// <summary>
        /// Calls 'git push --prune' which should delete
        /// remote branches that do not have a local equivalent.
        /// </summary>
        void PushPrune();
    }
}
