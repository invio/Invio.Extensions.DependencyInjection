using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Invio.Extensions.DependencyInjection {

    public static class ServiceCollectionFactoryExtensions {

        public static IServiceCollection AddTransientWithFactory<TService, TServiceFactory>(
            this IServiceCollection collection)
                where TService : class
                where TServiceFactory : class, IFactory<TService> {

            return collection.AddTransientWithFactory(typeof(TService), typeof(TServiceFactory));
        }

        public static IServiceCollection AddTransientWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType) {

            return Add(collection, serviceType, factoryType, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScopedWithFactory<TService, TServiceFactory>(
            this IServiceCollection collection)
                where TService : class
                where TServiceFactory : class, IFactory<TService> {

            return collection.AddScopedWithFactory(typeof(TService), typeof(TServiceFactory));
        }

        public static IServiceCollection AddScopedWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType) {

            return Add(collection, serviceType, factoryType, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddSingletonWithFactory<TService, TServiceFactory>(
            this IServiceCollection collection)
                where TService : class
                where TServiceFactory : class, IFactory<TService> {

            return Add(
                collection,
                typeof(TService),
                typeof(TServiceFactory),
                ServiceLifetime.Singleton
            );
        }

        public static IServiceCollection AddSingletonWithFactory(
            this IServiceCollection collection,
            Type serviceType,
            Type factoryType) {

            return Add(collection, serviceType, factoryType, ServiceLifetime.Singleton);
        }

        private static IServiceCollection Add(
            IServiceCollection collection,
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
