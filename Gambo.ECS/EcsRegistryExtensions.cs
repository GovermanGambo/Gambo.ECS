﻿using System.Collections.Generic;
using System.Linq;

namespace Gambo.ECS
{
    public static class EcsRegistryExtensions
    {
        public static T AddComponent<T>(this EcsRegistry registry, EcsEntity entity, params object[] args) where T : class
        {
            object component = registry.AddComponent(typeof(T), entity, args);
            return component as T;
        }
        
        public static IEnumerable<(T1, T2)> View<T1, T2>(this EcsRegistry registry)
            where T1 : class
            where T2 : class
        {
            var result = new List<(T1, T2)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1 componentA = null;
                T2 componentB = null;
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

                result.Add((componentA, componentB));
            }

            return result;
        }

        public static IEnumerable<(T1, T2, T3)> View<T1, T2, T3>(this EcsRegistry registry)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            var result = new List<(T1, T2, T3)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1 componentA = null;
                T2 componentB = null;
                T3 componentC = null;
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

                result.Add((componentA, componentB, componentC));
            }

            return result;
        }

        public static IEnumerable<(T1, T2, T3, T4)> View<T1, T2, T3, T4>(this EcsRegistry registry)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            var result = new List<(T1, T2, T3, T4)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1 componentA = null;
                T2 componentB = null;
                T3 componentC = null;
                T4 componentD = null;
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

                result.Add((componentA, componentB, componentC, componentD));
            }

            return result;
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> View<T1, T2, T3, T4, T5>(this EcsRegistry registry)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            var result = new List<(T1, T2, T3, T4, T5)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                T1 componentA = null;
                T2 componentB = null;
                T3 componentC = null;
                T4 componentD = null;
                T5 componentE = null;
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

                result.Add((componentA, componentB, componentC, componentD, componentE));
            }

            return result;
        }

        /// <summary>
        /// Resolves the first discovered component of the specified type.
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T">The component's type</typeparam>
        /// <returns></returns>
        public static T FindComponentOfType<T>(this EcsRegistry registry) where T : class
        {
            object component = registry.Components
                .Values
                .SelectMany(x => x)
                .FirstOrDefault(c => c.GetType() == typeof(T));

            return component as T;
        }
    }
}