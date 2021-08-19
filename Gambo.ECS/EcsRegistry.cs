using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gambo.ECS
{
    /// <summary>
    ///     Represents a collection of entities and their attached components
    /// </summary>
    public class EcsRegistry
    {
        private static int m_nextRegistryId;
        private readonly Dictionary<EcsEntity, List<object>> m_components;
        private readonly HashSet<EcsEntity> m_entities;

        private readonly int m_id;

        private int m_nextEntityId;

        public EcsRegistry()
        {
            m_id = m_nextRegistryId;
            m_nextRegistryId++;
            m_entities = new HashSet<EcsEntity>();
            m_components = new Dictionary<EcsEntity, List<object>>();
        }

        /// <summary>
        ///     The amount of entities contained in the registry
        /// </summary>
        public int EntitiesCount => m_entities.Count;

        /// <summary>
        ///     A dictionary of the entity-component relationships contained in the registry
        /// </summary>
        public ReadOnlyDictionary<EcsEntity, List<object>> Components =>
            new(m_components);

        /// <summary>
        ///     Called whenever an entity is added to the registry
        /// </summary>
        public event EventHandler<EntityEventArgs>? OnEntityAdded;
        
        /// <summary>
        ///     Called whenever an entity is removed from the registry
        /// </summary>
        public event EventHandler<EntityEventArgs>? OnEntityRemoved;

        /// <summary>
        ///     Called whenever a component is attached to an entity
        /// </summary>
        public event EventHandler<ComponentEventArgs>? OnComponentAdded;
        
        /// <summary>
        ///     Called whenever a component is removed from an entity
        /// </summary>
        public event EventHandler<ComponentEventArgs>? OnComponentRemoved;

        /// <summary>
        ///     Fetches the entity with the specified Id
        /// </summary>
        /// <param name="entityID">The id of the entity</param>
        /// <returns>The requested entity if it exists in the registry, null if not</returns>
        public EcsEntity? GetEntity(int entityID)
        {
            foreach (var entity in m_entities)
                if (entity.Id == entityID)
                    return entity;

            return null;
        }

        /// <summary>
        ///     Creates a new entity and attaches it to the registry
        /// </summary>
        /// <returns>The new entity</returns>
        public EcsEntity CreateEntity()
        {
            var entity = new EcsEntity(m_nextEntityId, this);
            m_entities.Add(entity);
            m_nextEntityId++;

            OnEntityAdded?.Invoke(this, new EntityEventArgs(entity));

            return entity;
        }

        /// <summary>
        ///     Removes the specified entity from the registry
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        /// <param name="permanent">Set to true to remove all attached components</param>
        /// <returns>True if removed, false if not found</returns>
        public bool RemoveEntity(EcsEntity entity, bool permanent = false)
        {
            var removed = m_entities.Remove(entity);

            if (permanent) m_components.Remove(entity);

            if (removed) OnEntityRemoved?.Invoke(this, new EntityEventArgs(entity));

            return removed;
        }

        /// <summary>
        ///     Attaches a new component of the specified type to specified entity
        /// </summary>
        /// <param name="componentType">Component type to attach</param>
        /// <param name="entity">Entity to attach component to</param>
        /// <param name="args">Component constructor arguments</param>
        /// <returns>The created component</returns>
        /// <exception cref="ArgumentException">Thrown if the component is not a struct, or if the type has no compatible constructor</exception>
        public object AddComponent(Type componentType, EcsEntity entity, params object[] args)
        {
            AssertEntity(entity);
            
            AssertComponentType(componentType);

            object? component = Activator.CreateInstance(componentType, args);

            if (component == null)
            {
                throw new ArgumentException($"Unable to add component of type {componentType}");
            }

            if (!m_components.ContainsKey(entity)) m_components.Add(entity, new List<object>());

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

        /// <summary>
        ///     Removes the specified component from the entity
        /// </summary>
        /// <param name="entity">The entity to remove the component from</param>
        /// <typeparam name="T">The type of component to remove</typeparam>
        public void RemoveComponent<T>(EcsEntity entity) where T : struct
        {
            if (!m_components.ContainsKey(entity)) return;

            var entityComponents = m_components[entity];
            for (int i = 0; i < entityComponents.Count; i++)
            {
                object component = entityComponents.ElementAt(i);
                if (component.GetType() != typeof(T)) continue;
                
                entityComponents.Remove(component);
                OnComponentRemoved?.Invoke(this, new ComponentEventArgs(entity, component));
                break;
            }
        }

        /// <summary>
        ///     Gets all components attached to the specified entity
        /// </summary>
        /// <param name="entity">The entity to query for components</param>
        /// <returns>A list of all components attached to the entity</returns>
        public List<object> GetComponents(EcsEntity entity)
        {
            AssertEntity(entity);

            return m_components.ContainsKey(entity) ? m_components[entity] : new List<object>();
        }

        /// <summary>
        ///     Gets all components of a specified type in the registry
        /// </summary>
        /// <typeparam name="TComponent">The type of components to query</typeparam>
        /// <returns>All components of the given type present in the registry</returns>
        public IEnumerable<TComponent> GetComponentsOfType<TComponent>() where TComponent : struct
        {
            var componentsByType = m_components
                .Values
                .SelectMany(x => x)
                .Where(c => c.GetType() == typeof(TComponent))
                .Select(x => (TComponent) x);

            return componentsByType;
        }

        /// <summary>
        ///     Gets a specific type of component on an entity
        /// </summary>
        /// <param name="entity">The entity to query</param>
        /// <typeparam name="T">The type of components to query</typeparam>
        /// <returns>The first matching component found</returns>
        public T? GetComponent<T>(EcsEntity entity) where T : struct
        {
            AssertEntity(entity);

            if (!m_components.ContainsKey(entity)) return null;

            var componentsInEntity = m_components[entity];
            foreach (var component in componentsInEntity)
                if (component.GetType() == typeof(T))
                    return (T) component;

            return null;
        }

        /// <summary>
        ///     Replaces the instance of a component type with the specified instance
        /// </summary>
        /// <param name="component">The component instance to replace. Type will be inferred from the instance</param>
        /// <param name="entity">The entity to manipulate</param>
        public void ReplaceComponent(object component, EcsEntity entity)
        {
            AssertEntity(entity);
            var componentType = component.GetType();
            AssertComponentType(componentType);

            int componentIndex = m_components[entity].Select(x => x.GetType()).ToList().IndexOf(componentType);

            if (componentIndex < 0)
            {
                AddComponent(componentType, entity);
            }
            else
            {
                m_components[entity][componentIndex] = component;
            }
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

        private static void AssertComponentType(Type componentType)
        {
            bool isStruct = componentType.IsValueType && !componentType.IsEnum;

            if (!isStruct)
                throw new ArgumentException($"Type {componentType} must be a struct to be registered as a component!");
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