using System;

namespace Gambo.ECS
{
    public class ComponentEventArgs : EventArgs
    {
        public ComponentEventArgs(EcsEntity entity, object component)
        {
            Entity = entity;
            Component = component;
        }

        public EcsEntity Entity { get; }
        public object Component { get; }
    }
}