using System;
using System.Collections.Generic;
using System.Reflection;

namespace LiteBindDI
{
    public class LiteBindContainer
    {
        private class Binding
        {
            public Func<object> Factory;
            public bool IsSingleton;
            public object SingletonInstance;
        }

        private readonly Dictionary<Type, Binding> _bindings = new();
        private readonly LiteBindContainer _parent;

        public LiteBindContainer(LiteBindContainer parent = null)
        {
            _parent = parent;
        }

        /// <summary>
        /// Binds a singleton instance to both its interface and concrete type.
        /// Useful for lifecycle systems (Initialize, Tick, Dispose) and access by interface.
        /// </summary>
        public void BindSingletonInterfaceAndSelf<TInterface, TImpl>(TImpl instance) where TImpl : TInterface
        {
            if (instance == null)
                throw new LiteBindException($"[LiteBind] Cannot bind null instance for type `{typeof(TImpl).FullName}`.");

            var typeInterface = typeof(TInterface);
            var typeImpl = typeof(TImpl);

            if (_bindings.ContainsKey(typeInterface))
                throw new LiteBindException($"[LiteBind] Type `{typeInterface.FullName}` is already bound.");

            if (_bindings.ContainsKey(typeImpl))
                throw new LiteBindException($"[LiteBind] Type `{typeImpl.FullName}` is already bound.");

            var binding = new Binding
            {
                Factory = () => instance,
                IsSingleton = true,
                SingletonInstance = instance
            };

            _bindings[typeInterface] = binding;
            _bindings[typeImpl] = binding;
        }

        /// <summary>
        /// Binds a specific singleton instance of type T.
        /// </summary>
        public void BindSingleton<T>(T instance)
        {
            if (instance == null)
                throw new LiteBindException($"[LiteBind] Cannot bind null instance for type `{typeof(T).FullName}`.");

            if (_bindings.ContainsKey(typeof(T)))
                throw new LiteBindException($"[LiteBind] Type `{typeof(T).FullName}` is already bound.");

            _bindings[typeof(T)] = new Binding
            {
                Factory = () => instance,
                IsSingleton = true,
                SingletonInstance = instance
            };
        }

        /// <summary>
        /// Binds a singleton using a factory delegate. Instantiated on first resolve.
        /// </summary>
        public void BindSingleton<T>(Func<T> factory)
        {
            if (factory == null)
                throw new LiteBindException($"[LiteBind] Factory for type `{typeof(T).FullName}` is null.");

            if (_bindings.ContainsKey(typeof(T)))
                throw new LiteBindException($"[LiteBind] Type `{typeof(T).FullName}` is already bound.");

            _bindings[typeof(T)] = new Binding
            {
                Factory = () => factory(),
                IsSingleton = true,
                SingletonInstance = null
            };
        }

        /// <summary>
        /// Binds a transient dependency. A new instance is created on each resolve.
        /// </summary>
        public void BindTransient<T>(Func<T> factory)
        {
            if (factory == null)
                throw new LiteBindException($"[LiteBind] Factory for type `{typeof(T).FullName}` is null.");

            if (_bindings.ContainsKey(typeof(T)))
                throw new LiteBindException($"[LiteBind] Type `{typeof(T).FullName}` is already bound.");

            _bindings[typeof(T)] = new Binding
            {
                Factory = () => factory(),
                IsSingleton = false
            };
        }

        /// <summary>
        /// Resolves an instance of type T.
        /// </summary>
        public T Resolve<T>()
        {
            var type = typeof(T);

            if (!TryGetBindingOrParent(type, out var binding))
            {
                throw new LiteBindException(
                    $"[LiteBind] Type `{type.FullName}` was not bound.\nAvailable bindings:\n - {GetAvailableBindings()}"
                );
            }

            if (binding.IsSingleton)
            {
                if (binding.SingletonInstance == null)
                    binding.SingletonInstance = binding.Factory();

                return (T)binding.SingletonInstance;
            }

            return (T)binding.Factory();
        }

        /// <summary>
        /// Resolves an instance by runtime Type (non-generic).
        /// Used internally by lifecycle runner.
        /// </summary>
        public object Resolve(Type type)
        {
            if (!TryGetBindingOrParent(type, out var binding))
            {
                throw new LiteBindException(
                    $"[LiteBind] Type `{type.FullName}` was not bound.\nAvailable bindings:\n - {GetAvailableBindings()}"
                );
            }

            if (binding.IsSingleton)
            {
                if (binding.SingletonInstance == null)
                    binding.SingletonInstance = binding.Factory();

                return binding.SingletonInstance;
            }

            return binding.Factory();
        }

        /// <summary>
        /// Binds a parameterized factory via IFactory interface.
        /// </summary>
        public void BindFactory<TParam, TResult>(Func<TParam, TResult> factory)
        {
            var key = typeof(IFactory<TParam, TResult>);

            if (_bindings.ContainsKey(key))
                throw new LiteBindException($"[LiteBind] Factory for `{key.FullName}` is already bound.");

            _bindings[key] = new Binding
            {
                Factory = () => new FactoryWrapper<TParam, TResult>(factory ?? throw new ArgumentNullException(nameof(factory))),
                IsSingleton = true,
                SingletonInstance = null
            };
        }

        /// <summary>
        /// Returns all registered types (for lifecycle scanning).
        /// </summary>
        public IEnumerable<Type> GetAllBoundTypes()
        {
            foreach (var kvp in _bindings)
                yield return kvp.Key;
        }

        /// <summary>
        /// Attempts to find binding in current or parent container.
        /// </summary>
        private bool TryGetBindingOrParent(Type type, out Binding binding)
        {
            if (_bindings.TryGetValue(type, out binding))
                return true;

            return _parent != null && _parent.TryGetBindingOrParent(type, out binding);
        }

        /// <summary>
        /// Returns a list of all registered types (for error messages).
        /// </summary>
        private string GetAvailableBindings()
        {
            return _bindings.Count > 0
                ? string.Join("\n - ", _bindings.Keys)
                : "No bindings available.";
        }


        public void InjectInto(object target)
        {
            var type = target.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            // Поля
            foreach (var field in type.GetFields(flags))
            {
                if (field.GetCustomAttribute<LiteInjectAttribute>() != null)
                {
                    var resolved = Resolve(field.FieldType);
                    field.SetValue(target, resolved);
                }
            }

            // Свойства
            foreach (var prop in type.GetProperties(flags))
            {
                if (prop.GetCustomAttribute<LiteInjectAttribute>() != null && prop.CanWrite)
                {
                    var resolved = Resolve(prop.PropertyType);
                    prop.SetValue(target, resolved);
                }
            }

            // Методы без параметров
            foreach (var method in type.GetMethods(flags))
            {
                if (method.GetCustomAttribute<LiteInjectAttribute>() != null && method.GetParameters().Length == 0)
                {
                    method.Invoke(target, null);
                }
            }
        }
    }

    public class FactoryWrapper<TParam, TResult> : IFactory<TParam, TResult>
    {
        private readonly Func<TParam, TResult> _factory;

        public FactoryWrapper(Func<TParam, TResult> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory), "[LiteBind] Passed factory delegate is null.");
        }

        public TResult Create(TParam param)
        {
            return _factory(param);
        }
    }
}
