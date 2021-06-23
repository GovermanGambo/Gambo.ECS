using System;

namespace Gambo.ECS
{
    public abstract class EcsSystem
    {
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                
                if (enabled)
                {
                    OnEnableBase();
                }
                else
                {
                    OnDisableBase();
                }
            }
        }

        public EcsRegistry Registry
        {
            get => registry;
            set
            {
                if (registry != null)
                {
                    registry.OnComponentAdded -= OnComponentAdded;
                    registry.OnComponentRemoved -= OnComponentRemoved;
                }
                
                registry = value;

                if (registry == null) return;
                
                registry.OnComponentAdded += OnComponentAdded;
                registry.OnComponentRemoved += OnComponentRemoved;
                OnRegistryAttached();

            }
        }

        ~EcsSystem()
        {
            registry.OnComponentAdded -= OnComponentAdded;
            registry.OnComponentRemoved -= OnComponentRemoved;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EcsSystem other)
            {
                return other.GetType() == GetType() && other.Registry == Registry;
            }

            return false;
        }

        protected bool Equals(EcsSystem other)
        {
            return other.GetType() == GetType() && other.Registry == Registry;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), Registry);
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }

        protected virtual void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            
        }

        protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            
        }

        protected virtual void OnRegistryAttached()
        {
            
        }

        private bool enabled = true;
        private EcsRegistry registry;
        
        private void OnEnableBase()
        {
            OnEnable();
        }

        private void OnDisableBase()
        {
            OnDisable();
        }
    }
}