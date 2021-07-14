using System;

namespace Gambo.ECS
{
    public class EcsContextBuilder
    {
        private readonly EcsContext context;

        public EcsContextBuilder()
        {
            context = new EcsContext();
        }

        /// <summary>
        /// Adds an existing registry to the context.
        /// </summary>
        /// <param name="registry">The registry to attach</param>
        /// <returns></returns>
        public EcsContextBuilder WithRegistry(EcsRegistry registry)
        {
            context.Registry = registry;

            return this;
        }

        /// <summary>
        /// Attaches a service provider to the context. Used to resolve services that may be specified in a system.
        /// </summary>
        /// <param name="serviceProvider">The IServiceProvider to use for service resolution.</param>
        /// <returns></returns>
        public EcsContextBuilder WithServiceProvider(IServiceProvider serviceProvider)
        {
            context.ServiceProvider = serviceProvider;

            return this;
        }

        /// <summary>
        /// Builds the context.
        /// </summary>
        /// <returns></returns>
        public EcsContext Build()
        {
            return context;
        }
    }
}