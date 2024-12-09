namespace CORE
{

    public abstract class Bootstrap : ViewContext
    {
        protected InjectionBinder InjectionBinder { get; set; }
        protected CommandBinder CommandBinder { get; set; }

        void Awake()
        {
            InjectionBinder = new();
            CommandBinder = new(InjectionBinder);
            SetBindings();
        }

        public void AddView(View view)
        {
            SubscribeForLifeCycleUpdates(view);
            InjectionBinder.InjectPropertiesInto(view);
        }

        public void RemoveView(View view)
        {
            UnsubscribeForLifeCycleUpdates(view);
        }

        public abstract void SetBindings();
    }
}
