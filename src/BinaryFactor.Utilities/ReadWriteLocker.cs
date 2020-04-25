// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Threading;

    public enum LockType
    {
        Read,
        UpgradeableRead,
        Write
    }

    public class LockTimeoutException : Exception
    {
    }

    public class ReadWriteLocker
    {
        private readonly ReaderWriterLockSlim locker;
        private readonly TimeSpan? defaultTimeout;

        public ReadWriteLocker(int defaultMillisecondsTimeout, LockRecursionPolicy? recursionPolicy = null)
            : this(TimeSpan.FromMilliseconds(defaultMillisecondsTimeout), recursionPolicy)
        {
        }

        public ReadWriteLocker(TimeSpan defaultTimeout, LockRecursionPolicy? recursionPolicy = null)
            : this((TimeSpan?) defaultTimeout, recursionPolicy)
        {
        }

        public ReadWriteLocker(LockRecursionPolicy? recursionPolicy = null)
            : this(null, recursionPolicy)
        {
        }

        private ReadWriteLocker(TimeSpan? defaultTimeout, LockRecursionPolicy? recursionPolicy)
        {
            this.defaultTimeout = defaultTimeout;

            this.locker = recursionPolicy == null
                ? new ReaderWriterLockSlim()
                : new ReaderWriterLockSlim(recursionPolicy.Value);
        }

        public DisposableLock EnterReadLock() => DoEnterReadLock(this.defaultTimeout);

        public DisposableLock EnterReadLock(int millisecondsTimeout) => DoEnterReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));

        public DisposableLock EnterReadLock(TimeSpan timeout) => DoEnterReadLock(timeout);

        public DisposableLock EnterUpgradeableReadLock() => DoEnterUpgradeableReadLock(this.defaultTimeout);

        public DisposableLock EnterUpgradeableReadLock(int millisecondsTimeout) => DoEnterUpgradeableReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));

        public DisposableLock EnterUpgradeableReadLock(TimeSpan timeout) => DoEnterUpgradeableReadLock(timeout);

        public DisposableLock EnterWriteLock() => DoEnterWriteLock(this.defaultTimeout);

        public DisposableLock EnterWriteLock(int millisecondsTimeout) => DoEnterWriteLock(TimeSpan.FromMilliseconds(millisecondsTimeout));

        public DisposableLock EnterWriteLock(TimeSpan timeout) => DoEnterWriteLock(timeout);

        private DisposableLock DoEnterReadLock(TimeSpan? timeout)
        {
            if (this.defaultTimeout == null)
            {
                this.locker.EnterReadLock();
            }
            else
            {
                if (!this.locker.TryEnterReadLock(timeout.Value))
                    throw new LockTimeoutException();
            }

            return new DisposableLock(this.locker, LockType.Read);
        }

        private DisposableLock DoEnterUpgradeableReadLock(TimeSpan? timeout)
        {
            if (this.defaultTimeout == null)
            {
                this.locker.EnterUpgradeableReadLock();
            }
            else
            {
                if (!this.locker.TryEnterUpgradeableReadLock(timeout.Value))
                    throw new LockTimeoutException();
            }

            return new DisposableLock(this.locker, LockType.UpgradeableRead);
        }

        private DisposableLock DoEnterWriteLock(TimeSpan? timeout)
        {
            if (this.defaultTimeout == null)
            {
                this.locker.EnterWriteLock();
            }
            else
            {
                if (!this.locker.TryEnterWriteLock(timeout.Value))
                    throw new LockTimeoutException();
            }

            return new DisposableLock(this.locker, LockType.Write);
        }

        public class DisposableLock : IDisposable
        {
            private readonly ReaderWriterLockSlim readerWriterLock;
            
            private bool hasBeenDisposed;

            public DisposableLock(ReaderWriterLockSlim readerWriterLock, LockType lockType)
            {
                this.readerWriterLock = readerWriterLock;
                LockType = lockType;
            }

            public LockType LockType { get; }

            void IDisposable.Dispose()
            {
                if (this.hasBeenDisposed)
                    throw new ObjectDisposedException(nameof(DisposableLock));

                switch (LockType)
                {
                    case LockType.Read:
                        this.readerWriterLock.ExitReadLock();
                        break;

                    case LockType.UpgradeableRead:
                        this.readerWriterLock.ExitUpgradeableReadLock();
                        break;

                    case LockType.Write:
                        this.readerWriterLock.ExitWriteLock();
                        break;
                }

                this.hasBeenDisposed = true;
            }
        }
    }
}
