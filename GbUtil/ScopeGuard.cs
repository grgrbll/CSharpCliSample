using System;
namespace GbUtil
{
    public class ScopeGuard : IDisposable
    {
        private bool isDisposed = false;
        private Action onLeaveScope;

        public ScopeGuard(Action func)
        {
            onLeaveScope = func;
        }

        public void Dispose()
        {
            if (!isDisposed)
                onLeaveScope();
            isDisposed = true;
        }

        ~ScopeGuard()
        {
            if (!isDisposed)
                throw new ApplicationException("Scope guard must be used within a 'using' statement.");
	    }
    }
}
