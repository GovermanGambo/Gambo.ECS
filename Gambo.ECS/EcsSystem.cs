using System;

namespace Gambo.ECS
{
    public abstract class EcsSystem
    {
        private bool m_enabled = true;
        private EcsRegistry m_registry;

        public bool Enabled
        {
            get => m_enabled;
            set
            {
                m_enabled = value;

                if (m_enabled)
                    OnEnableBase();
                else
                    OnDisableBase();
            }
        }

        public EcsRegistry Registry
        {
            get => m_registry;
            set
            {
                if (m_registry != null)
                {
                    m_registry.OnComponentAdded -= OnComponentAdded;
                    m_registry.OnComponentRemoved -= OnComponentRemoved;
                }

                m_registry = value;

                if (m_registry == null) return;

                m_registry.OnComponentAdded += OnComponentAdded;
                m_registry.OnComponentRemoved += OnComponentRemoved;
                OnRegistryAttached();
            }
        }

        ~EcsSystem()
        {
            m_registry.OnComponentAdded -= OnComponentAdded;
            m_registry.OnComponentRemoved -= OnComponentRemoved;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EcsSystem other) return other.GetType() == GetType() && other.Registry == Registry;

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