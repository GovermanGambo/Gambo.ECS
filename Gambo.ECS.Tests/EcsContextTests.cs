using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Gambo.ECS.Tests
{
    public class EcsContextTests
    {
        private EcsContext context;

        [SetUp]
        public void SetUp()
        {
            context = new EcsContextBuilder().Build();
        }

        [Test]
        public void SystemShouldBeAddedIfNotExists()
        {
            var system = context.AddSystem<TestSystem>(true, "Name");
            var addedSystem = context.GetSystem<TestSystem>();

            Assert.AreEqual(system, addedSystem);
        }

        [Test]
        public void SystemShouldNotBeAddedIfExists()
        {
            var systemA = context.AddSystem<TestSystem>(true, "Name");
            var systemB = context.AddSystem<TestSystem>(true, "Name");

            Assert.AreEqual(1, context.Systems.Count);
        }

        [Test]
        public void SystemShouldBeRemoved()
        {
            var system = context.AddSystem<TestSystem>(true, "Name");
            var removed = context.RemoveSystem<TestSystem>();

            Assert.True(removed);
        }

        [Test]
        public void SystemShouldNotBeRemovedIfNotExists()
        {
            var removed = context.RemoveSystem<TestSystem>();
            Assert.False(removed);
        }

        [Test]
        public void SystemShouldGetRegisteredService()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, TestService>();
            var serviceProvider = services.BuildServiceProvider();

            var diContext = new EcsContextBuilder()
                .WithServiceProvider(serviceProvider)
                .Build();
            
            diContext.AddSystem<DISystem>();

            var system = diContext.GetSystem<DISystem>();
            bool result = system.DoSomething();
            
            Assert.True(result);
        }

        [Test]
        public void ContextShouldThrowExceptionIfNoService()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            
            var diContext = new EcsContextBuilder()
                .WithServiceProvider(serviceProvider)
                .Build();

            Assert.Throws<ArgumentException>(() => diContext.AddSystem<DISystem>());
        }
    }

    public class DISystem : EcsSystem
    {
        private readonly IService service;

        public DISystem(IService service)
        {
            this.service = service;
        }

        public bool DoSomething()
        {
            return service.DoSomething();
        }
    }

    public interface IService
    {
        bool DoSomething();
    }

    public class TestService : IService
    {
        public bool DoSomething()
        {
            return true;
        }
    }
}