using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    public class EcsContext
    {
        private readonly HashSet<EcsSystem> systems;

        internal EcsContext()
        {
            Registry = new EcsRegistry();
            systems = new HashSet<EcsSystem>();
        }

        public EcsRegistry Registry { get; internal set; }
        public ReadOnlyCollection<EcsSystem> Systems => new(systems.ToList());
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Adds a system to the context, with the specified constructor parameters.
        /// </summary>
        /// <param name="args">Constructor parameters of the specified system.</param>
        /// <typeparam name="TSystem">The system sub-type</typeparam>
        /// <returns></returns>
        public TSystem AddSystem<TSystem>(bool hasParameters, params object[] args) where TSystem : EcsSystem
        {
            if (!hasParameters)
            {
                return AddSystem<TSystem>();
            }
            
            var system = CreateSystem<TSystem>(args);

            AddSystem(system);

            return system;
        }

        /// <summary>
        /// Adds a system to the context. If a service provider is attached, it will use it to resolve the
        /// system's services during construction.
        /// </summary>
        /// <typeparam name="TSystem"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TSystem AddSystem<TSystem>() where TSystem : EcsSystem
        {
            if (ServiceProvider == null)
            {
                return AddSystem<TSystem>(true, Array.Empty<object>());
            }
            
            var systemType = typeof(TSystem);
            var constructor = systemType.GetConstructors()[0];

            var paramInfos = constructor.GetParameters();
            object[] parameters = new object[paramInfos.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var type = paramInfos[i].ParameterType;
                object service = ServiceProvider.GetService(type);
                parameters[i] = service ?? throw new ArgumentException($"No service of type {type} was found in the registry!");
            }

            var system = CreateSystem<TSystem>(parameters);
            
            AddSystem(system);

            return system;
        }

        public bool RemoveSystem<TSystem>() where TSystem : EcsSystem
        {
            var system = systems.FirstOrDefault(s => s.GetType() == typeof(TSystem));

            if (system == null) return false;

            system.Enabled = false;
            return systems.Remove(system);
        }

        public TSystem GetSystem<TSystem>() where TSystem : EcsSystem
        {
            var system = systems.FirstOrDefault(s => s.GetType() == typeof(TSystem));

            return system as TSystem;
        }

        private TSystem CreateSystem<TSystem>(object[] parameters)
        {
            var system = (TSystem)Activator.CreateInstance(typeof(TSystem), parameters);
            if (system == null)
                throw new ArgumentException($"No suitable constructor was found for system type {typeof(TSystem)}");

            return system;
        }

        private void AddSystem(EcsSystem system)
        {
            system.Registry = Registry;
            system.Enabled = true;
            systems.Add(system);
        }
    }
}