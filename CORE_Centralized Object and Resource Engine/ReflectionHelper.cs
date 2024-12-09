using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CORE
{
    internal static class ReflectionHelper
    {
        internal static readonly Dictionary<Type, PropertyInfo[]> PropertiesCache = new();
        private static readonly Dictionary<PropertyInfo, Action<object, object>> SettersCache = new();
        private static readonly Dictionary<Type, ConstructorInfo> DefaultConstructorCache = new();

        internal static object CreateInstance(Type type)
        {
            // Check the cache first
            if (!DefaultConstructorCache.TryGetValue(type, out var constructor))
            {
                // Retrieve and cache the constructor
                constructor = type.GetConstructors().FirstOrDefault();
                if (constructor == null)
                {
                    throw new InvalidOperationException($"Type {type.Name} does not have a public constructor.");
                }
                DefaultConstructorCache[type] = constructor;
            }

            // Invoke the constructor
            return constructor.Invoke(null);
        }

        internal static Action<object, object> GetSetter(PropertyInfo property)
        {
            if (!SettersCache.TryGetValue(property, out var setter))
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                var convertedInstance = Expression.Convert(instance, property.DeclaringType);
                var convertedValue = Expression.Convert(value, property.PropertyType);

                var body = Expression.Assign(Expression.Property(convertedInstance, property), convertedValue);
                setter = Expression.Lambda<Action<object, object>>(body, instance, value).Compile();

                SettersCache[property] = setter;
            }
            return setter;
        }


    }
}



