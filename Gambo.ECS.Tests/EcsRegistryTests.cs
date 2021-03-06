using System;
using System.Linq;
using NUnit.Framework;

namespace Gambo.ECS.Tests
{
    public class EcsRegistryTests
    {
        private EcsRegistry registry;

        [SetUp]
        public void Setup()
        {
            registry = new EcsRegistry();
        }

        [Test]
        public void EntityShouldBeAdded()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.GetEntity(entityA.Id);

            Assert.AreEqual(1, registry.EntitiesCount);
            Assert.AreEqual(entityB, entityA);
        }

        [Test]
        public void EntityShouldBeRemoved()
        {
            var entityA = registry.CreateEntity();
            registry.RemoveEntity(entityA);

            var removedEntity = registry.GetEntity(entityA.Id);

            Assert.AreEqual(0, registry.EntitiesCount);
            Assert.IsNull(removedEntity);
            Assert.Throws<ArgumentException>(() => registry.GetComponent<TestComponent>(entityA));
        }

        [Test]
        public void EntityShouldNotBeAddedIfAlreadyAdded()
        {
            var entity = registry.CreateEntity();
            var added = registry.AddEntity(entity);
            Assert.False(added);
        }

        [Test]
        public void EntityShouldBeAddedIfRemoved()
        {
            var entity = registry.CreateEntity();
            registry.RemoveEntity(entity);
            var added = registry.AddEntity(entity);

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
        public void ComponentShouldBeAddedIfComponentIsAlreadyAdded()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<TestComponent>(entity);
            var component2 = registry.AddComponent<TestComponent>(entity);

            Assert.NotNull(component);
            Assert.NotNull(component2);
            Assert.AreEqual(2, registry.GetComponents(entity).Count);
            Assert.AreEqual(component, registry.GetComponent<TestComponent>(entity));
        }

        [Test]
        public void ComponentShouldbeAddedWithoutTypeParams()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent(typeof(TagComponent), entity, "Test");
            
            Assert.AreEqual(typeof(TagComponent), component.GetType());

            var tagComponent = component as TagComponent;
            
            Assert.NotNull(tagComponent);
            Assert.AreEqual("Test", tagComponent.Tag);
        }
        
        [Test]
        public void UniqueComponentShouldNotBeAddedIfComponentIsAlreadyAdded()
        {
            var entity = registry.CreateEntity();
            var component = registry.AddComponent<UniqueComponent>(entity);
            Assert.Throws<ArgumentException>(() => registry.AddComponent<UniqueComponent>(entity));
        }

        [Unique]
        public class UniqueComponent
        {
            
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
            var tag = new TagComponent("Test");
            var component = registry.AddComponent<TagComponent>(entity, tag.Tag);

            Assert.NotNull(component);
            Assert.AreEqual(tag.Tag, component.Tag);
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

        [Test]
        public void GetComponentShouldReturnNullIfDoesntExist()
        {
            var entity = registry.CreateEntity();
            var component = registry.GetComponent<TestComponent>(entity);

            Assert.Null(component);
        }

        [Test]
        public void ViewShouldReturn2ViewsWith2Components2()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.CreateEntity();
            var testComponentA = registry.AddComponent<TestComponent>(entityA);
            var tagComponentA = registry.AddComponent<TagComponent>(entityA, "TagA");
            var testComponentB = registry.AddComponent<TestComponent>(entityB);
            var tagComponentB = registry.AddComponent<TagComponent>(entityB, "TagB");

            var views = registry.View<TestComponent, TagComponent>().ToArray();
            var (testA, tagA) = views[0];
            var (testB, tagB) = views[1];


            Assert.AreEqual(testComponentA, testA);
            Assert.AreEqual(testComponentB, testB);
            Assert.AreEqual(tagComponentA, tagA);
            Assert.AreEqual(tagComponentB, tagB);
        }

        [Test]
        public void ViewShouldReturn1ViewWith2Components2()
        {
            var entityA = registry.CreateEntity();
            var entityB = registry.CreateEntity();
            var testComponentA = registry.AddComponent<TestComponent>(entityA);
            var tagComponentA = registry.AddComponent<TagComponent>(entityA, "TagA");
            var testComponentB = registry.AddComponent<TestComponent>(entityB);

            var views = registry.View<TestComponent, TagComponent>().ToArray();

            Assert.AreEqual(views.Length, 1);
            var (testA, tagA) = views[0];


            Assert.AreEqual(testComponentA, testA);
            Assert.AreEqual(tagComponentA, tagA);
        }

        [Test]
        public void FindComponentOfTypeShouldReturn1Component()
        {
            var entityA = registry.CreateEntity();
            var testComponent = registry.AddComponent<TestComponent>(entityA);

            var result = registry.FindComponentOfType<TestComponent>();
            
            Assert.NotNull(result);
            Assert.AreEqual(testComponent, result);
        }
        
        [Test]
        public void FindComponentOfTypeShouldReturnNull()
        {
            var entityA = registry.CreateEntity();

            var result = registry.FindComponentOfType<TestComponent>();
            
            Assert.Null(result);
        }

        private class TestComponent
        {
        }

        private class TagComponent
        {
            public TagComponent()
            {
            }

            public TagComponent(string tag)
            {
                Tag = tag;
            }

            public string Tag { get; }
        }
    }
}