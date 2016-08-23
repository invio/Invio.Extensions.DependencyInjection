using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Invio.Extensions.DependencyInjection {

    /// <summary>
    /// Extension methods for adding services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionFactoryExtensions {

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> that
        /// will be hydrated via a factory specified in <typeparamref name="TServiceFactory"/>
        /// to the specified <see cref="IServiceCollection" />. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TServiceFactory">The type of the factory to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        /// <seealso cref="ServiceLifetime.Transient" />
        public static IServiceCollection AddTransientWithFactory<TService, TServiceFactory>(
            this IServiceCollection collection)
                where TService : class
                where TServiceFactory : class, IFactory<TService> {

            return collection.AddTransientWithFactory(typeof(TService), typeof(TServiceFactory));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> that
        /// will be hydrated via a factory specified in <paramref name="factoryType"/>
        /// to the specified <see cref="IServiceCollection" />. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="factoryType">The type of the factory to use.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        /// <seealso cref="ServiceLifetime.Transient" />
        public static IServiceCollection AddTransientWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType) {

            return collection.AddWithFactory(serviceType, factoryType, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService"/> that
        /// will be hydrated via a factory specified in <typeparamref name="TServiceFactory"/>
        /// to the specified <see cref="IServiceCollection" />. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TServiceFactory">The type of the factory to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        public static IServiceCollection AddScopedWithFactory<TService, TServiceFactory>(
            this IServiceCollection collection)
                where TService : class
                where TServiceFactory : class, IFactory<TService> {

            return collection.AddScopedWithFactory(typeof(TService), typeof(TServiceFactory));
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType"/> that
        /// will be hydrated via a factory specified in <paramref name="factoryType"/>
        /// to the specified <see cref="IServiceCollection" />. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="factoryType">The type of the factory to use.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        public static IServiceCollection AddScopedWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType) {

            return collection.AddWithFactory(serviceType, factoryType, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService"/> that
        /// will be hydrated via a factory specified in <typeparamref name="TServiceFactory"/>
        /// to the specified <see cref="IServiceCollection" />. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TServiceFactory">The type of the factory to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        /// <seealso cref="ServiceLifetime.Singleton" />
        public static IServiceCollection AddSingletonWithFactory<TService, TServiceFactory>(
            this IServiceCollection collection)
                where TService : class
                where TServiceFactory : class, IFactory<TService> {

            return collection.AddSingletonWithFactory(typeof(TService), typeof(TServiceFactory));
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> that
        /// will be hydrated via a factory specified in <paramref name="factoryType"/>
        /// to the specified <see cref="IServiceCollection" />. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="factoryType">The type of the factory to use.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        /// <seealso cref="ServiceLifetime.Singleton" />
        public static IServiceCollection AddSingletonWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType) {

            return collection.AddWithFactory(serviceType, factoryType, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Adds a service of the type specified in <paramref name="serviceType"/> that
        /// will be hydrated via a factory specified in <paramref name="factoryType"/>
        /// to the specified <see cref="IServiceCollection" />. Its lifetime is controlled
        /// by the value specified via <paramref name="lifetime"/>. If the factory is not yet added
        /// to the <see cref="IServiceCollection" />, it will also be added as well.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="factoryType">The type of the factory to use.</param>
        /// <param name="lifetime">The lifetime of the service before the factory is reused.</param>
        /// <returns>
        /// A reference to this <see cref="IServiceCollection"/> instance after the operation has
        /// completed.
        /// </returns>
        public static IServiceCollection AddWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType,
            ServiceLifetime lifetime) {

            const string provideMethodName = nameof(IFactory<object>.Provide);

            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            } else if (serviceType == null) {
                throw new ArgumentNullException(nameof(serviceType));
            } else if (factoryType == null) {
                throw new ArgumentNullException(nameof(factoryType));
            }

            collection.TryAddTransient(factoryType);

            var descriptor = new ServiceDescriptor(
                serviceType,
                (IServiceProvider provider) => {
                    var factory = provider.GetRequiredService(factoryType);

                    var service =
                        factory
                            .GetType()
                            .GetMethod(provideMethodName, Type.EmptyTypes)
                            .Invoke(factory, null);

                    return service;
                },
                lifetime
            );

            collection.Add(descriptor);

            return collection;
        }

    }

}
