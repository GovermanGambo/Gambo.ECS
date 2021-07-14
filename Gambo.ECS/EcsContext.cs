using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    public class EcsContext
    {
        private readonly HashSet<EcsSystem> systems;
        private readonly IServiceProvider services;

        public EcsContext()
        {
            Registry = new EcsRegistry();
            systems = new HashSet<EcsSystem>();
        }

        public EcsContext(IServiceProvider services)
        {
            this.services = services;
            
            Registry = new EcsRegistry();
            systems = new HashSet<EcsSystem>();
        }

        public EcsRegistry Registry { get; }
        public ReadOnlyCollection<EcsSystem> Systems => new(systems.ToList());

        public TSystem AddSystem<TSystem>(params object[] args) where TSystem : EcsSystem
        {
            var system = CreateSystem<TSystem>(args);

            AddSystem(system);

            return system;
        }

        public TSystem AddSystem<TSystem>() where TSystem : EcsSystem
        {
            if (services == null)
            {
                return AddSystem<TSystem>(Array.Empty<object>());
            }
            
            var systemType = typeof(TSystem);
            var constructor = systemType.GetConstructors()[0];

            var paramInfos = constructor.GetParameters();
            object[] parameters = new object[paramInfos.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var type = paramInfos[i].GetType();
                object service = services.GetService(type);
                parameters[i] = service;
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