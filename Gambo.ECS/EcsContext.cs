using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    public class EcsContext
    {
        private readonly HashSet<EcsSystem> systems;

        public EcsContext()
        {
            Registry = new EcsRegistry();
            systems = new HashSet<EcsSystem>();
        }

        public EcsRegistry Registry { get; }
        public ReadOnlyCollection<EcsSystem> Systems => new(systems.ToList());

        public TSystem AddSystem<TSystem>(params object[] args) where TSystem : EcsSystem
        {
            var system = (TSystem) Activator.CreateInstance(typeof(TSystem), args);
            if (system == null)
                throw new ArgumentException($"No suitable constructor was found for system type {typeof(TSystem)}");

            system.Registry = Registry;
            system.Enabled = true;
            systems.Add(system);

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
    }
}