namespace Svn2GitNetX
{
    public interface ILockBreaker
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// Should we break locks.
        /// Should be set to whatever <see cref="Options.BreakLocks"/> is set to.
        /// </summary>
        bool ShouldBreakLocks { get; }

        // ---------------- Functions ----------------

        /// <summary>
        /// Breaks any index.lock files in the .git/svn/refs/remotes/svn/* folder.
        /// These locks can appear if a process was killed.
        /// </summary>
        void BreakLocks();
    }

    public static class ILockBreakerExtensions
    {
        public static void BreakLocksIfEnabled( this ILockBreaker lockBreaker )
        {
            if( lockBreaker.ShouldBreakLocks )
            {
                lockBreaker.BreakLocks();
            }
        }
    }
}
