using System;

namespace Invio.Extensions.DependencyInjection {

    /// <summary>
    /// Implementation of factory pattern to provide services for use
    /// with a implementation of <see cref="IServiceCollection" />.
    /// </summary>
    /// <typeparam name="TService">The type of the service the factory provides.</typeparam>
    public interface IFactory<out TService> {

        /// <summary>
        /// Provides an object of the type <typeparamref name="TService"/>. It will
        /// run based upon the lifetime specified for <typeparamref name="TService"/>
        /// in the overarching <see cref="IServiceCollection" />.
        /// </summary>
        /// <returns>
        /// An instance of the <typeparamref name="TService"/> defined for the factory.
        /// </returns>
        TService Provide();

    }

}
