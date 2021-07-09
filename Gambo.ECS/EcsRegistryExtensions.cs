using System.Collections.Generic;
using System.Linq;

namespace Gambo.ECS
{
    public static class EcsRegistryExtensions
    {
        public static IEnumerable<(T1, T2)> View<T1, T2>(this EcsRegistry registry)
            where T1 : class
            where T2 : class
        {
            var result = new List<(T1, T2)>();

            var components = registry.Components;

            foreach (var entity in components.Keys)
            {
                var componentA = components[entity].FirstOrDefault(t => t.GetType() == typeof(T1)) as T1;
                var componentB = components[entity].FirstOrDefault(t => t.GetType() == typeof(T2)) as T2;

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
                var componentA = components[entity].FirstOrDefault(t => t.GetType() == typeof(T1)) as T1;
                var componentB = components[entity].FirstOrDefault(t => t.GetType() == typeof(T2)) as T2;
                var componentC = components[entity].FirstOrDefault(t => t.GetType() == typeof(T3)) as T3;

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
                var componentA = components[entity].FirstOrDefault(t => t.GetType() == typeof(T1)) as T1;
                var componentB = components[entity].FirstOrDefault(t => t.GetType() == typeof(T2)) as T2;
                var componentC = components[entity].FirstOrDefault(t => t.GetType() == typeof(T3)) as T3;
                var componentD = components[entity].FirstOrDefault(t => t.GetType() == typeof(T4)) as T4;

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
                var componentA = components[entity].FirstOrDefault(t => t.GetType() == typeof(T1)) as T1;
                var componentB = components[entity].FirstOrDefault(t => t.GetType() == typeof(T2)) as T2;
                var componentC = components[entity].FirstOrDefault(t => t.GetType() == typeof(T3)) as T3;
                var componentD = components[entity].FirstOrDefault(t => t.GetType() == typeof(T4)) as T4;
                var componentE = components[entity].FirstOrDefault(t => t.GetType() == typeof(T5)) as T5;

                if (componentA == null || componentB == null || componentC == null || componentD == null
                    || componentE == null)
                    continue;

                result.Add((componentA, componentB, componentC, componentD, componentE));
            }

            return result;
        }
    }
}