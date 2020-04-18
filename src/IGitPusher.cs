namespace Svn2GitNetX
{
    public interface IGitPusher
    {
        /// <summary>
        /// Calls 'git push --all'.
        /// </summary>
        void PushAll();
    }
}
