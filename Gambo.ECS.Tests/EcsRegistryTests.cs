using System;
using System.Linq;
using NUnit.Framework;

namespace Gambo.ECS.Tests
{
    public class EcsRegistryTests
    {
        [SetUp]
        public void Setup()
        {
            registry = new EcsRegistry();
        }
        
        [Test]
        public void EntityShouldBeAdded()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.GetEntity(entityA.ID);

            Assert.AreEqual(1, registry.EntitiesCount);
            Assert.AreEqual(entityB, entityA);
        }

        [Test]
        public void EntityShouldBeRemoved()
        {
            var entityA = registry.CreateEntity();
            registry.RemoveEntity(entityA);

            var removedEntity = registry.GetEntity(entityA.ID);
            
            Assert.AreEqual(0, registry.EntitiesCount);
            Assert.IsNull(removedEntity);
            Assert.Throws<ArgumentException>(() => registry.GetComponent<TestComponent>(entityA));
        }

        [Test]
        public void EntityShouldNotBeAddedIfAlreadyAdded()
        {
            var entity = registry.CreateEntity();
            bool added = registry.AddEntity(entity);
            Assert.False(added);
        }

        [Test]
        public void EntityShouldBeAddedIfRemoved()
        {
            var entity = registry.CreateEntity();
            registry.RemoveEntity(entity);
            bool added = registry.AddEntity(entity);
            
            Assert.True(added);
        }

        [Test]
        public void EntityComponentsShouldPersistIfNotPermanent()
        {
            var entity = registry.CreateEntity();
            registry.AddComponent<TagComponent>(entity, "MyTag");
            registry.RemoveEntity(entity);
            registry.AddEntity(entity);
            
            Assert.AreEqual(1, registry.GetComponents(entity).Count);
            Assert.AreEqual("MyTag", registry.GetComponent<TagComponent>(entity).Tag);
        }

        [Test]
        public void EntityComponentsShouldNotPersistIfPermanent()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<TagComponent>(entity, "MyTag");
            registry.RemoveEntity(entity, true);
            registry.AddEntity(entity);
            
            Assert.IsEmpty(registry.GetComponents(entity));
            Assert.AreNotEqual(component, registry.GetComponent<TagComponent>(entity));
        }

        private struct TestComponent
        {
            
        }

        [Test]
        public void ComponentShouldNotBeAddedIfEntityNotExists()
        {
            var entity = registry.CreateEntity();
            registry.RemoveEntity(entity);
            Assert.Throws<ArgumentException>(() => registry.AddComponent<TestComponent>(entity));
        }

        [Test]
        public void ComponentShouldBeAddedIfEntityExists()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<TestComponent>(entity);
            
            Assert.NotNull(component);
            Assert.AreEqual(1, registry.GetComponents(entity).Count);
            Assert.AreEqual(component, registry.GetComponent<TestComponent>(entity));
        }

        [Test]
        public void ComponentShouldBeRemovedIfComponentExists()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<TestComponent>(entity);
            registry.RemoveComponent<TestComponent>(entity);
            
            Assert.AreEqual(0, registry.GetComponents(entity).Count);
        }
        
        [Test]
        public void ComponentShouldBeRemovedIfComponentDontExists()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<TestComponent>(entity);
            registry.RemoveComponent<TestComponent>(entity);
            registry.RemoveComponent<TestComponent>(entity);
            
            Assert.AreEqual(0, registry.GetComponents(entity).Count);
        }

        [Test]
        public void ComponentShouldNotBeAddedIfAlreadyExists()
        {
            var entity = registry.CreateEntity();
            registry.AddComponent<TestComponent>(entity);
            Assert.Throws<ArgumentException>(() => registry.AddComponent<TestComponent>(entity));
        }

        private struct TagComponent
        {
            public string Tag { get; }

            public TagComponent(string tag)
            {
                Tag = tag;
            }

            public TagComponent(TagComponent tagComponent)
            {
                Tag = tagComponent.Tag;
            }
        }
        
        [Test]
        public void ComponentShouldBeAddedWithArguments()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<TagComponent>(entity, "MyTag");
            
            Assert.NotNull(component);
            Assert.AreEqual("MyTag", component.Tag);
        }
        
        [Test]
        public void ComponentShouldBeAddedWithArguments2()
        {
            var entity = registry.CreateEntity();
            var tag = new TagComponent();
            var component = registry.AddComponent<TagComponent>(entity, tag);
            
            Assert.NotNull(component);
            Assert.AreEqual(tag, component);
        }

        [Test]
        public void ComponentsShouldBeAttachedToDifferentEntities()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.CreateEntity();
            var componentA = registry.AddComponent<TagComponent>(entityA, "EntityA");
            var componentB = registry.AddComponent<TagComponent>(entityB, "EntityB");
            
            Assert.AreEqual(1, registry.GetComponents(entityA).Count);
            Assert.AreEqual(1, registry.GetComponents(entityB).Count);
            Assert.AreNotEqual(componentA.Tag, componentB.Tag);
        }

        [Test]
        public void RemoveComponentShouldAffectOneEntity()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.CreateEntity();
            var componentA = registry.AddComponent<TagComponent>(entityA, "EntityA");
            var componentB = registry.AddComponent<TagComponent>(entityB, "EntityB");
            registry.RemoveComponent<TagComponent>(entityA);
            
            Assert.AreEqual(0, registry.GetComponents(entityA).Count);
            Assert.AreEqual(1, registry.GetComponents(entityB).Count);
            Assert.AreEqual(componentB, registry.GetComponent<TagComponent>(entityB));
            Assert.AreNotEqual(componentA, registry.GetComponent<TagComponent>(entityA));
        }

        [Test]
        public void GetComponentsOfTypeShouldReturn2()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.CreateEntity();
            registry.AddComponent<TagComponent>(entityA, "TagA");
            registry.AddComponent<TestComponent>(entityB);
            registry.AddComponent<TagComponent>(entityB, "TagB");

            var components = registry.GetComponentsOfType<TagComponent>();
            
            Assert.AreEqual(2, components.Count());
            Assert.AreEqual("TagA", components.ElementAt(0).Tag);
            Assert.AreEqual("TagB", components.ElementAt(1).Tag);
        }

        private EcsRegistry registry;
    }
}