using System;

namespace Gambo.ECS
{
    public class EcsEntity
    {
        public static bool operator ==(EcsEntity left, EcsEntity right)
        {
            return left?.ID == right?.ID && left?.owner == right?.owner;
        }
        
        public static bool operator !=(EcsEntity left, EcsEntity right)
        {
            return left?.ID != right?.ID || left?.owner != right?.owner;
        }

        public int ID { get; }

        public override bool Equals(object? obj)
        {
            if (obj is EcsEntity other)
            {
                return this.ID == other.ID && this.owner == other.owner;
            }

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

        internal EcsEntity(int id, EcsRegistry owner)
        {
            ID = id;
            this.owner = owner;
        }

        private readonly EcsRegistry owner;
    }
    
    public class EntityEventArgs : EventArgs
    {
        public EcsEntity EcsEntity { get; }
        
        public EntityEventArgs(EcsEntity entity)
        {
            EcsEntity = entity;
        }
    }
}