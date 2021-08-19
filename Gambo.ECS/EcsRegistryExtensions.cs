using System;
using System.Collections.Generic;
using System.Linq;

namespace Gambo.ECS
{
    public static class EcsRegistryExtensions
    {
        /// <summary>
        ///     Adds an instance of the specified Component type to the specified entity.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T AddComponent<T>(this EcsRegistry registry, EcsEntity entity, params object[] args) where T : struct
        {
            object component = registry.AddComponent(typeof(T), entity, args);
            return (T)component;
        }
        
        /// <summary>
        ///     Returns all components of a specified type on the entity
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> GetComponents<T>(this EcsRegistry registry, EcsEntity entity) where T : struct
        {
            if (!registry.Components.ContainsKey(entity))
            {
                throw new ArgumentException($"Entity with id {entity.Id} was not found in the registry.");
            }
            
            var components = registry.Components[entity]
                .Where(c => c.GetType() == typeof(T));

            return components as IEnumerable<T>;
        }

        /// <summary>
        ///     Checks if the specified entity has the specified type of component
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<T>(this EcsRegistry registry, EcsEntity entity) where T : struct
        {
            object component = registry.GetComponent<T>(entity);

            return component != null;
        }

        public static bool HasEntity(this EcsRegistry registry, int id)
        {
            var entity = registry.GetEntity(id);

            return entity != null;
        }

        public static bool HasEntity(this EcsRegistry registry, EcsEntity entity)
        {
            return registry.HasEntity(entity.Id);
        }

        public static void ReplaceComponent<T>(this EcsRegistry registry, T component, EcsEntity entity)
            where T : struct
        {
            registry.ReplaceComponent(component, entity);
        }
        
        public static IEnumerable<(T1, T2)> View<T1, T2>(this EcsRegistry registry)
            where T1 : struct
            where T2 : struct
        {
            var result = new List<(T1, T2)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1? componentA = null;
                T2? componentB = null;
                foreach (var component in components[entity])
                {
                    switch (component)
                    {
                        case T1 t1:
                            componentA = t1;
                            break;
                        case T2 t2:
                            componentB = t2;
                            break;
                    }
                }

                if (componentA == null || componentB == null) continue;

                result.Add((componentA.Value, componentB.Value));
            }

            return result;
        }

        public static IEnumerable<(T1, T2, T3)> View<T1, T2, T3>(this EcsRegistry registry)
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            var result = new List<(T1, T2, T3)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1? componentA = null;
                T2? componentB = null;
                T3? componentC = null;
                foreach (var component in components[entity])
                {
                    switch (component)
                    {
                        case T1 t1:
                            componentA = t1;
                            break;
                        case T2 t2:
                            componentB = t2;
                            break;
                        case T3 t3:
                            componentC = t3;
                            break;
                    }
                }

                if (componentA == null || componentB == null || componentC == null) continue;

                result.Add((componentA.Value, componentB.Value, componentC.Value));
            }

            return result;
        }

        public static IEnumerable<(T1, T2, T3, T4)> View<T1, T2, T3, T4>(this EcsRegistry registry)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
        {
            var result = new List<(T1, T2, T3, T4)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1? componentA = null;
                T2? componentB = null;
                T3? componentC = null;
                T4? componentD = null;
                foreach (var component in components[entity])
                {
                    switch (component)
                    {
                        case T1 t1:
                            componentA = t1;
                            break;
                        case T2 t2:
                            componentB = t2;
                            break;
                        case T3 t3:
                            componentC = t3;
                            break;
                        case T4 t4:
                            componentD = t4;
                            break;
                    }
                }

                if (componentA == null || componentB == null || componentC == null || componentD == null) continue;

                result.Add((componentA.Value, componentB.Value, componentC.Value, componentD.Value));
            }

            return result;
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> View<T1, T2, T3, T4, T5>(this EcsRegistry registry)
            where T1 : struct
            where T2 : struct
            where T3 : struct
            where T4 : struct
            where T5 : struct
        {
            var result = new List<(T1, T2, T3, T4, T5)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1? componentA = null;
                T2? componentB = null;
                T3? componentC = null;
                T4? componentD = null;
                T5? componentE = null;
                foreach (var component in components[entity])
                {
                    switch (component)
                    {
                        case T1 t1:
                            componentA = t1;
                            break;
                        case T2 t2:
                            componentB = t2;
                            break;
                        case T3 t3:
                            componentC = t3;
                            break;
                        case T4 t4:
                            componentD = t4;
                            break;
                        case T5 t5:
                            componentE = t5;
                            break;
                    }
                }

                if (componentA == null || componentB == null || componentC == null || componentD == null
                    || componentE == null)
                    continue;

                result.Add((componentA.Value, componentB.Value, componentC.Value, componentD.Value, componentE.Value));
            }

            return result;
        }

        /// <summary>
        /// Resolves the first discovered component of the specified type.
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T">The component's type</typeparam>
        /// <returns></returns>
        public static T? FindComponentOfType<T>(this EcsRegistry registry) where T : struct
        {
            object component = registry.Components
                .Values
                .SelectMany(x => x)
                .FirstOrDefault(c => c.GetType() == typeof(T));

            return component as T?;
        }
    }
}