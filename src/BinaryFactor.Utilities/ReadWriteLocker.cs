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

        public DiposableLock EnterReadLock()
        {
            if (this.defaultTimeout == null)
            {
                this.locker.EnterReadLock();
                return new DiposableLock(this.locker, LockType.Read);
            }
            else
            {
                return EnterReadLock(this.defaultTimeout.Value);
            }
        }

        public DiposableLock EnterReadLock(int millisecondsTimeout)
        {
            return EnterReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public DiposableLock EnterReadLock(TimeSpan timeout)
        {
            return this.locker.TryEnterReadLock(timeout)
                ? new DiposableLock(this.locker, LockType.Read)
                : throw new LockTimeoutException();
        }

        public DiposableLock EnterUpgradeableReadLock()
        {
            if (this.defaultTimeout == null)
            {
                this.locker.EnterUpgradeableReadLock();
                return new DiposableLock(this.locker, LockType.UpgradeableRead);
            }
            else
            {
                return EnterUpgradeableReadLock(this.defaultTimeout.Value);
            }
        }

        public DiposableLock EnterUpgradeableReadLock(int millisecondsTimeout)
        {
            return EnterUpgradeableReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public DiposableLock EnterUpgradeableReadLock(TimeSpan timeout)
        {
            return this.locker.TryEnterUpgradeableReadLock(timeout)
                ? new DiposableLock(this.locker, LockType.UpgradeableRead)
                : throw new LockTimeoutException();
        }

        public DiposableLock EnterWriteLock()
        {
            if (this.defaultTimeout == null)
            {
                this.locker.EnterWriteLock();
                return new DiposableLock(this.locker, LockType.Write);
            }
            else
            {
                return EnterWriteLock(this.defaultTimeout.Value);
            }
        }

        public DiposableLock EnterWriteLock(int millisecondsTimeout)
        {
            return EnterWriteLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public DiposableLock EnterWriteLock(TimeSpan timeout)
        {
            return this.locker.TryEnterWriteLock(timeout)
                ? new DiposableLock(this.locker, LockType.Write)
                : throw new LockTimeoutException();
        }

        public class DiposableLock : IDisposable
        {
            private readonly ReaderWriterLockSlim readerWriterLock;

            public DiposableLock(ReaderWriterLockSlim readerWriterLock, LockType lockType)
            {
                this.readerWriterLock = readerWriterLock;
                LockType = lockType;
            }

            public LockType LockType { get; }

            void IDisposable.Dispose()
            {
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
            }
        }
    }
}
