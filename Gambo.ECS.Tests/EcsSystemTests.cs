﻿using NUnit.Framework;

namespace Gambo.ECS.Tests
{
    internal class DummySystem : EcsSystem
    {
        public override void Dispatch()
        {
        }

        protected override void OnComponentAdded(object sender, ComponentEventArgs e)
        {
        }

        protected override void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
        }
    }
    
    internal class TestSystem : EcsSystem
    {
        public string Name { get; }
        public TestSystem(string name)
        {
            Name = name;
        }

        public override void Dispatch()
        {
        }

        protected override void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            
        }

        protected override void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            
        }
    }
    
    public class EcsSystemTests
    {
        [SetUp]
        public void Setup()
        {
            system = new TestSystem("TestSystem");
        }

        [Test]
        public void SystemsShouldBeEqualIfSameTypeAndRegistry()
        {
            var registry = new EcsRegistry();
            system.Registry = registry;

            var systemB = new TestSystem("TestSystemB");
            systemB.Registry = registry;
            
            Assert.AreEqual(system, systemB);
        }

        [Test]
        public void SystemsShouldNotBeEqualIfDifferentRegistries()
        {
            var systemB = new TestSystem("TestSystemB");
            var registryA = new EcsRegistry();
            var registryB = new EcsRegistry();

            system.Registry = registryA;
            systemB.Registry = registryB;
            
            Assert.AreNotEqual(system, systemB);
        }
        
        [Test]
        public void SystemsShouldNotBeEqualIfDifferentTypes()
        {
            var systemB = new DummySystem();
            var registry = new EcsRegistry();

            system.Registry = registry;
            systemB.Registry = registry;
            
            Assert.AreNotEqual(system, systemB);
        }

        private TestSystem system;
    }
}