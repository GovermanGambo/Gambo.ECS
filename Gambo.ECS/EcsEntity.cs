using System;

namespace Gambo.ECS
{
    /// <summary>
    ///     Represents an entity in the Entity-Component-System
    /// </summary>
    public class EcsEntity
    {
        private EcsRegistry m_owner;

        internal EcsEntity(int id, EcsRegistry owner)
        {
            Id = id;
            m_owner = owner;
        }

        /// <summary>
        ///     The entity Id within the attached registry
        /// </summary>
        public int Id { get; }

        public static bool operator ==(EcsEntity left, EcsEntity right)
        {
            return left?.Id == right?.Id && left?.m_owner == right?.m_owner;
        }

        public static bool operator !=(EcsEntity left, EcsEntity right)
        {
            return left?.Id != right?.Id || left?.m_owner != right?.m_owner;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EcsEntity other) return Id == other.Id && m_owner == other.m_owner;

            return false;
        }

        protected bool Equals(EcsEntity other)
        {
            return Id == other.Id && Equals(m_owner, other.m_owner);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, m_owner);
        }

        internal void SetOwner(EcsRegistry registry)
        {
            m_owner = registry;
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