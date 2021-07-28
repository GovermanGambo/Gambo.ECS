using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    public class EcsRegistry
    {
        private static int m_nextRegistryId;
        private readonly Dictionary<EcsEntity, HashSet<object>> m_components;
        private readonly HashSet<EcsEntity> m_entities;

        private readonly int m_id;

        private int m_nextEntityId;

        public EcsRegistry()
        {
            m_id = m_nextRegistryId;
            m_nextRegistryId++;
            m_entities = new HashSet<EcsEntity>();
            m_components = new Dictionary<EcsEntity, HashSet<object>>();
        }

        public int EntitiesCount => m_entities.Count;

        public ReadOnlyDictionary<EcsEntity, HashSet<object>> Components =>
            new(m_components);

        public event EventHandler<EntityEventArgs> OnEntityAdded;
        public event EventHandler<EntityEventArgs> OnEntityRemoved;

        public event EventHandler<ComponentEventArgs> OnComponentAdded;
        public event EventHandler<ComponentEventArgs> OnComponentRemoved;

        public EcsEntity GetEntity(int entityID)
        {
            foreach (var entity in m_entities)
                if (entity.Id == entityID)
                    return entity;

            return null;
        }

        public EcsEntity CreateEntity()
        {
            var entity = new EcsEntity(m_nextEntityId, this);
            m_entities.Add(entity);
            m_nextEntityId++;

            OnEntityAdded?.Invoke(this, new EntityEventArgs(entity));

            return entity;
        }

        public bool AddEntity(EcsEntity entity)
        {
            var added = m_entities.Add(entity);

            if (added) OnEntityAdded?.Invoke(this, new EntityEventArgs(entity));

            return added;
        }

        public bool RemoveEntity(EcsEntity entity, bool permanent = false)
        {
            var removed = m_entities.Remove(entity);

            if (permanent) m_components.Remove(entity);

            if (removed) OnEntityRemoved?.Invoke(this, new EntityEventArgs(entity));

            return removed;
        }

        public object AddComponent(Type componentType, EcsEntity entity, params object[] args)
        {
            AssertEntity(entity);

            var component = Activator.CreateInstance(componentType, args);

            if (!m_components.ContainsKey(entity)) m_components.Add(entity, new HashSet<object>());

            var componentsInEntity = m_components[entity];

            var attributes = Attribute.GetCustomAttributes(componentType);
            if (attributes.Any(a => a is UniqueAttribute))
            {
                if (componentsInEntity.Any(c => c.GetType() == componentType))
                {
                    throw new ArgumentException($"An instance of this unique component already exists on entity!");
                }
            }

            componentsInEntity.Add(component);

            OnComponentAdded?.Invoke(this, new ComponentEventArgs(entity, component));

            return component;
        }

        public void RemoveComponent<T>(EcsEntity entity) where T : class
        {
            if (!m_components.ContainsKey(entity)) return;

            var entityComponents = m_components[entity];
            for (var i = 0; i < entityComponents.Count; i++)
            {
                var component = entityComponents.ElementAt(i);
                if (component.GetType() == typeof(T))
                {
                    entityComponents.Remove(component);
                    OnComponentRemoved?.Invoke(this, new ComponentEventArgs(entity, component));
                    break;
                }
            }
        }

        public HashSet<object> GetComponents(EcsEntity entity)
        {
            AssertEntity(entity);

            return m_components.ContainsKey(entity) ? m_components[entity] : new HashSet<object>();
        }

        public IEnumerable<TComponent> GetComponentsOfType<TComponent>() where TComponent : class
        {
            var componentsByType = m_components
                .Values
                .SelectMany(x => x)
                .Where(c => c.GetType() == typeof(TComponent))
                .Select(x => (TComponent) x);

            return componentsByType;
        }

        public T GetComponent<T>(EcsEntity entity) where T : class
        {
            AssertEntity(entity);

            if (!m_components.ContainsKey(entity)) return default;

            var componentsInEntity = m_components[entity];
            foreach (var component in componentsInEntity)
                if (component.GetType() == typeof(T))
                    return (T) component;

            return default;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EcsRegistry other) return Equals(other);

            return false;
        }

        private bool Equals(EcsRegistry other)
        {
            return m_id == other.m_id && Equals(m_entities, other.m_entities) && Equals(m_components, other.m_components);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_id, m_entities, m_components);
        }

        private void AssertEntity(EcsEntity entity)
        {
            if (!m_entities.Contains(entity))
                throw new ArgumentException($"Entity with id {entity.Id} was not found in the registry.");
        }
    }

    public class RegistryEventArgs : EventArgs
    {
        public RegistryEventArgs(EcsRegistry registry)
        {
            Registry = registry;
        }

        public EcsRegistry Registry { get; }
    }
}