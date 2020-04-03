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
