using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CORE
{
    public class CommandBinder
    {
        private InjectionBinder InjectionBinder { get; set; }
        public CommandBinder(InjectionBinder dependencyResolver)
        {
            InjectionBinder = dependencyResolver;
        }

        public SignalCommandContext<TSignal, TParam> Bind<TSignal, TParam>()
            where TSignal : ISignal<TParam>
        {
            var signalType = typeof(TSignal);
            TSignal signal = (TSignal)ReflectionHelper.CreateInstance(typeof(TSignal));
            InjectionBinder.EnableInjection(signal);
            SignalCommandContext<TSignal, TParam> context = new(signal, InjectionBinder);
            return context;
        }

    }

    public class SignalCommandContext<TSignal, TParam> where TSignal : ISignal<TParam>
    {
        private InjectionBinder InjectionBinder { get; set; }
        private List<IBaseCommand<TParam>> BoundCommands { get; set; }
        private TSignal Signal { get; set; }
        public SignalCommandContext(TSignal signal, InjectionBinder dependencyResolver)
        {
            BoundCommands = new();
            InjectionBinder = dependencyResolver;
            Signal = signal;
        }

        public SignalCommandContext<TSignal, TParam> To<TCommand>()
            where TCommand : IBaseCommand<TParam>
        {
            TCommand command = (TCommand)ReflectionHelper.CreateInstance(typeof(TCommand));
            InjectionBinder.InjectPropertiesInto(command);
            BoundCommands.Add(command);
            return this;
        }

        public void In(ExecutionType executionType)
        {
            Signal.AddListener(param => ExecuteCommands(Signal, param, executionType));
        }

        private async void ExecuteCommands(TSignal signal, TParam param, ExecutionType executionType)
        {
            if (executionType == ExecutionType.Sequence)
            {
                await ExecuteSequentially(BoundCommands, param);
            }
            else
            {
                await ExecuteInParallel(BoundCommands, param);
            }
        }

        private async Task ExecuteSequentially(List<IBaseCommand<TParam>> commands, TParam param)
        {
            foreach (var command in commands)
            {
                await ExecuteCommand(command, param);
            }
        }

        private async Task ExecuteInParallel(List<IBaseCommand<TParam>> commands, TParam param)
        {
            var tasks = commands.Select(command => ExecuteCommand(command, param));
            await Task.WhenAll(tasks);
        }

        private async Task ExecuteCommand(IBaseCommand<TParam> command, TParam param)
        {
            try
            {
                if (command is ICommand<TParam> syncCommand)
                {
                    syncCommand.Execute(param);
                }
                else if (command is ICommandAsync<TParam> asyncCommand)
                {
                    await asyncCommand.ExecuteAsync(param);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error executing command {command.GetType().Name} for signal {Signal.GetType().Name} with param {param}: {ex.Message}");
            }
        }


    }


    public enum ExecutionType
    {
        Parallel,
        Sequence
    }

}