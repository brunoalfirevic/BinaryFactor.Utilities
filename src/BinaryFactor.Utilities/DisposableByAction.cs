// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

using System;

namespace BinaryFactor.Utilities
{
    public class DisposableByAction : IDisposable
    {
        private readonly Action action;

        public DisposableByAction(Action action = null) => this.action = action;

        void IDisposable.Dispose() => this.action?.Invoke();
    }
}
