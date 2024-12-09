using System;

namespace CORE
{
    public class Signal<Param> : ISignal<Param>
    {
        private event Action<Param> OnEvent;

        public void AddListener(Action<Param> listener)
        {
            OnEvent += listener;
        }

        public void RemoveListener(Action<Param> listener)
        {
            OnEvent -= listener;
        }

        public void Dispatch(Param param)
        {
            OnEvent?.Invoke(param);
        }
    }


}
