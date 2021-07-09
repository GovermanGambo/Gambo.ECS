using System;

namespace Gambo.ECS
{
    public class EcsEntity
    {
        private readonly EcsRegistry owner;

        internal EcsEntity(int id, EcsRegistry owner)
        {
            ID = id;
            this.owner = owner;
        }

        public int ID { get; }

        public static bool operator ==(EcsEntity left, EcsEntity right)
        {
            return left?.ID == right?.ID && left?.owner == right?.owner;
        }

        public static bool operator !=(EcsEntity left, EcsEntity right)
        {
            return left?.ID != right?.ID || left?.owner != right?.owner;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EcsEntity other) return ID == other.ID && owner == other.owner;

            return false;
        }

        protected bool Equals(EcsEntity other)
        {
            return ID == other.ID && Equals(owner, other.owner);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, owner);
        }
    }

    public class EntityEventArgs : EventArgs
    {
        public EntityEventArgs(EcsEntity entity)
        {
            EcsEntity = entity;
        }

        public EcsEntity EcsEntity { get; }
    }
}