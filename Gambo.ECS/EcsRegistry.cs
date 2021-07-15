using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    public class EcsRegistry
    {
        private static int nextRegistryID;
        private readonly Dictionary<EcsEntity, HashSet<object>> components;
        private readonly HashSet<EcsEntity> entities;

        private readonly int id;

        private int nextEntityID;

        public EcsRegistry()
        {
            id = nextRegistryID;
            nextRegistryID++;
            entities = new HashSet<EcsEntity>();
            components = new Dictionary<EcsEntity, HashSet<object>>();
        }

        public int EntitiesCount => entities.Count;

        public ReadOnlyDictionary<EcsEntity, HashSet<object>> Components =>
            new(components);

        public event EventHandler<EntityEventArgs> OnEntityAdded;
        public event EventHandler<EntityEventArgs> OnEntityRemoved;

        public event EventHandler<ComponentEventArgs> OnComponentAdded;
        public event EventHandler<ComponentEventArgs> OnComponentRemoved;

        public EcsEntity GetEntity(int entityID)
        {
            foreach (var entity in entities)
                if (entity.ID == entityID)
                    return entity;

            return null;
        }

        public EcsEntity CreateEntity()
        {
            var entity = new EcsEntity(nextEntityID, this);
            entities.Add(entity);
            nextEntityID++;

            OnEntityAdded?.Invoke(this, new EntityEventArgs(entity));

            return entity;
        }

        public bool AddEntity(EcsEntity entity)
        {
            var added = entities.Add(entity);

            if (added) OnEntityAdded?.Invoke(this, new EntityEventArgs(entity));

            return added;
        }

        public bool RemoveEntity(EcsEntity entity, bool permanent = false)
        {
            var removed = entities.Remove(entity);

            if (permanent) components.Remove(entity);

            if (removed) OnEntityRemoved?.Invoke(this, new EntityEventArgs(entity));

            return removed;
        }

        public T AddComponent<T>(EcsEntity entity, params object[] args) where T : class
        {
            AssertEntity(entity);

            var component = (T) Activator.CreateInstance(typeof(T), args);

            if (!components.ContainsKey(entity)) components.Add(entity, new HashSet<object>());

            var componentsInEntity = components[entity];

            var attributes = Attribute.GetCustomAttributes(component.GetType());
            if (attributes.Any(a => a is UniqueAttribute))
            {
                if (componentsInEntity.Any(c => c.GetType() == typeof(T)))
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
            if (!components.ContainsKey(entity)) return;

            var entityComponents = components[entity];
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

            return components.ContainsKey(entity) ? components[entity] : new HashSet<object>();
        }

        public IEnumerable<TComponent> GetComponentsOfType<TComponent>() where TComponent : class
        {
            var componentsByType = components
                .Values
                .SelectMany(x => x)
                .Where(c => c.GetType() == typeof(TComponent))
                .Select(x => (TComponent) x);

            return componentsByType;
        }

        public T GetComponent<T>(EcsEntity entity) where T : class
        {
            AssertEntity(entity);

            if (!components.ContainsKey(entity)) return default;

            var componentsInEntity = components[entity];
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
            return id == other.id && Equals(entities, other.entities) && Equals(components, other.components);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, entities, components);
        }

        private void AssertEntity(EcsEntity entity)
        {
            if (!entities.Contains(entity))
                throw new ArgumentException($"Entity with id {entity.ID} was not found in the registry.");
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