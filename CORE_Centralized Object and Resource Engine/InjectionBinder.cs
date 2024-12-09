using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CORE
{

    public class InjectionBinder
    {
        private readonly Dictionary<Type, Object> Bindings = new();


        public void Bind<TInterface, TImplementation>()
        where TImplementation : TInterface
        {
            if (typeof(TImplementation).IsAbstract)
            {
                throw new InvalidOperationException($"TImplementation {typeof(TImplementation).Name} must be a concrete class.");
            }

            Bindings[typeof(TInterface)] = ReflectionHelper.CreateInstance(typeof(TImplementation));
        }

        public void Bind<TInterface, TImplementation>(TImplementation obj)
        where TImplementation : TInterface
        {
            if (typeof(TImplementation).IsAbstract)
            {
                throw new InvalidOperationException($"TImplementation {typeof(TImplementation).Name} must be a concrete class.");
            }

            Bindings[typeof(TInterface)] = obj;
        }

        public void EnableInjection<T>(T implementation)
        {
            Bindings[typeof(T)] = implementation;
        }

        public void InjectPropertiesInto(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type instanceType = instance.GetType();

            if (!ReflectionHelper.PropertiesCache.TryGetValue(instanceType, out PropertyInfo[] properties))
            {
                properties = instanceType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => Attribute.IsDefined(p, typeof(Dependency)))
                    .ToArray();

                ReflectionHelper.PropertiesCache[instanceType] = properties;
            }

            foreach (PropertyInfo property in properties)
            {
                try
                {
                    object value = Resolve(property.PropertyType);
                    if (value == null)
                    {
                        throw new InvalidOperationException($"No binding found for {property.PropertyType}");
                    }

                    ReflectionHelper.GetSetter(property).Invoke(instance, value);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to inject property {property.Name} on {instance.GetType()}: {ex.Message}");
                }
            }
        }

        private object Resolve(Type interfaceType)
        {
            if (!Bindings.TryGetValue(interfaceType, out Object obj))
            {
                throw new InvalidOperationException($"Service of type {interfaceType.Name} is not registered.");
            }

            return obj;
        }

    }

    public class BindingConfig
    {
        public Type interfaceType;
        public Object obj;

        public BindingConfig(Type interfaceType, Object obj)
        {
            this.interfaceType = interfaceType;
            this.obj = obj;
        }

    }


    /// <summary>
    /// Used for dependency injection from CORE
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Dependency : Attribute
    {

    }

    public enum BindingType
    {
        /// <summary>
        /// non static singleton, only one exist in Container but other instances can present outside DI injection 
        /// </summary>
        Singleton,

        /// <summary>
        /// create a new object of type everytime it is injected
        /// </summary>
        Transient
    }
}
