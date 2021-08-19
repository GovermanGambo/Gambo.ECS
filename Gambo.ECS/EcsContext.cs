using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    public class EcsContext
    {
        private readonly HashSet<EcsSystem> m_systems = new();

        internal EcsContext()
        {
            Registry = new EcsRegistry();
        }

        /// <summary>
        ///     The registry bound to this context
        /// </summary>
        public EcsRegistry Registry { get; internal set; }

        /// <summary>
        ///     The ServiceProvider used for service injection when creating systems
        /// </summary>
        public IServiceProvider? ServiceProvider { get; set; }

        /// <summary>
        ///     The systems attached to this context
        /// </summary>
        public ReadOnlyCollection<EcsSystem> Systems => new(m_systems.ToList());

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
                object? service = ServiceProvider.GetService(type);
                parameters[i] = service ?? throw new ArgumentException($"No service of type {type} was found in the registry!");
            }

            var system = CreateSystem<TSystem>(parameters);
            
            AddSystem(system);

            return system;
        }

        /// <summary>
        ///     Removes the specified system from the context
        /// </summary>
        /// <typeparam name="TSystem">Type of the system</typeparam>
        /// <returns>True if system was removed, false if it was not found</returns>
        public bool RemoveSystem<TSystem>() where TSystem : EcsSystem
        {
            var system = m_systems.FirstOrDefault(s => s.GetType() == typeof(TSystem));

            if (system == null) return false;

            system.Enabled = false;
            return m_systems.Remove(system);
        }

        /// <summary>
        ///     Gets the system of the specified type from the system
        /// </summary>
        /// <typeparam name="TSystem">The system type</typeparam>
        /// <returns>The system if it was found, null if not</returns>
        public TSystem? GetSystem<TSystem>() where TSystem : EcsSystem
        {
            var system = m_systems.FirstOrDefault(s => s.GetType() == typeof(TSystem));

            return system as TSystem;
        }

        private static TSystem CreateSystem<TSystem>(object[] parameters)
        {
            var system = (TSystem?)Activator.CreateInstance(typeof(TSystem), parameters);
            if (system == null)
                throw new ArgumentException($"No suitable constructor was found for system type {typeof(TSystem)}");

            return system;
        }

        private void AddSystem(EcsSystem system)
        {
            system.Registry = Registry;
            system.Enabled = true;
            m_systems.Add(system);
        }
    }
}