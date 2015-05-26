using System;

namespace ExtensionComponentBase
{
    public interface IExtensionComponent
    {
        void StartAction(Action<Exception> exceptionCallBack);
        void WaitForActionCompletion();
    }
}
