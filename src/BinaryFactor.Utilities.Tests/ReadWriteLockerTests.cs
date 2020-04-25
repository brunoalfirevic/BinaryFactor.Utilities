// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Tests
{
    using Shouldly;
    using BinaryFactor.Utilities;
    using System.Threading;
    using System;

    public class ReadWriteLockerTests
    {
        private readonly ReadWriteLocker locker = new ReadWriteLocker(TimeSpan.Zero);

        public void TestReadLock()
        {
            using (this.locker.EnterReadLock())
            {
                InAnotherThread(() => Should.NotThrow(() =>
                {
                    using (this.locker.EnterReadLock())
                    {
                    }
                }));

                InAnotherThread(() => Should.Throw<LockTimeoutException>(() =>
                {
                    using (this.locker.EnterWriteLock())
                    {
                    }
                }));

                Should.Throw<LockRecursionException>(() =>
                {
                    using (this.locker.EnterWriteLock())
                    {
                    }
                });
            }
        }

        public void TestUpgradeableReadLock()
        {
            using (this.locker.EnterUpgradeableReadLock())
            {
                InAnotherThread(() => Should.NotThrow(() =>
                {
                    using (this.locker.EnterReadLock())
                    {
                    }
                }));

                InAnotherThread(() => Should.Throw<LockTimeoutException>(() =>
                {
                    using (this.locker.EnterWriteLock())
                    {
                    }
                }));

                using (this.locker.EnterWriteLock())
                {
                    InAnotherThread(() => Should.Throw<LockTimeoutException>(() =>
                    {
                        using (this.locker.EnterReadLock())
                        {
                        }
                    }));
                }
            }
        }

        public void TestWriteLock()
        {
            using (this.locker.EnterWriteLock())
            {
                InAnotherThread(() => Should.Throw<LockTimeoutException>(() =>
                {
                    using (this.locker.EnterReadLock())
                    {
                    }
                }));
            }
        }

        private void InAnotherThread(Action action)
        {
            Exception exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch(Exception e)
                {
                    exception = e;
                }
            });

            thread.Start();
            thread.Join();

            if (exception != null)
                throw exception;
        }
    }
}
