using NUnit.Framework;

namespace Gambo.ECS.Tests
{
    public class EcsContextTests
    {
        private EcsContext context;

        [SetUp]
        public void SetUp()
        {
            context = new EcsContext();
        }

        [Test]
        public void SystemShouldBeAddedIfNotExists()
        {
            var system = context.AddSystem<TestSystem>("Name");
            var addedSystem = context.GetSystem<TestSystem>();

            Assert.AreEqual(system, addedSystem);
        }

        [Test]
        public void SystemShouldNotBeAddedIfExists()
        {
            var systemA = context.AddSystem<TestSystem>("Name");
            var systemB = context.AddSystem<TestSystem>("Name");

            Assert.AreEqual(1, context.Systems.Count);
        }

        [Test]
        public void SystemShouldBeRemoved()
        {
            var system = context.AddSystem<TestSystem>("Name");
            var removed = context.RemoveSystem<TestSystem>();

            Assert.True(removed);
        }

        [Test]
        public void SystemShouldNotBeRemovedIfNotExists()
        {
            var removed = context.RemoveSystem<TestSystem>();
            Assert.False(removed);
        }
    }
}