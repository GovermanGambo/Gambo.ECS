using System;
using System.Collections.Generic;

namespace Gambo.ECS
{
    public class ComponentView
    {
        private readonly Dictionary<Type, object> components;

        internal ComponentView(Dictionary<Type, object> components)
        {
            this.components = components;
        }

        /// <summary>
        ///     Fetches the component of this type, if it exists in the view.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public TComponent Get<TComponent>() where TComponent : class
        {
            if (!components.ContainsKey(typeof(TComponent))) return null;

            var component = components[typeof(TComponent)];
            return component as TComponent;
        }
    }
}