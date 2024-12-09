using System;
using System.Threading.Tasks;

namespace CORE
{
    public interface IBaseCommand<T>
    {

    }
    public interface ICommand<T> : IBaseCommand<T>
    {
        void Execute(T param);
    }

    public interface ICommandAsync<T> : IBaseCommand<T>
    {
        Task ExecuteAsync(T param);
    }

    public interface ISignal<T>
    {
        void AddListener(Action<T> listener);
        void RemoveListener(Action<T> listener);
        void Dispatch(T param); // Generic object array for flexibility
    }


}

