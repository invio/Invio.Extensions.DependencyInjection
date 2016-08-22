using System;
using Microsoft.Extensions.DependencyInjection;
using Invio.Extensions.DependencyInjection.Fakes;
using Xunit;

namespace Invio.Extensions.DependencyInjection {

    public class ServiceCollectionFactoryExtensionsTests {

        public static TheoryData BasicImplementations {
            get {
                var serviceType = typeof(IFakeService);
                var factoryType = typeof(FakeServiceFactory);

                return new TheoryData<Func<IServiceCollection, IServiceCollection>> {
                    { c => c.AddTransientWithFactory<IFakeService, FakeServiceFactory>() },
                    { c => c.AddTransientWithFactory(serviceType, factoryType) },
                    { c => c.AddWithFactory(serviceType, factoryType, ServiceLifetime.Transient) },

                    { c => c.AddScopedWithFactory<IFakeService, FakeServiceFactory>() },
                    { c => c.AddScopedWithFactory(serviceType, factoryType) },
                    { c => c.AddWithFactory(serviceType, factoryType, ServiceLifetime.Scoped) },

                    { c => c.AddSingletonWithFactory<IFakeService, FakeServiceFactory>() },
                    { c => c.AddSingletonWithFactory(serviceType, factoryType) },
                    { c => c.AddWithFactory(serviceType, factoryType, ServiceLifetime.Singleton) }
                };
            }
        }

        public static TheoryData DependentImplementations {
            get {
                var serviceType = typeof(IFakeService);
                var factoryType = typeof(FakeDependentServiceFactory);

                return new TheoryData<Func<IServiceCollection, IServiceCollection>> {
                    { c => c.AddTransientWithFactory<IFakeService, FakeDependentServiceFactory>() },
                    { c => c.AddTransientWithFactory(serviceType, factoryType) },
                    { c => c.AddWithFactory(serviceType, factoryType, ServiceLifetime.Transient) },

                    { c => c.AddScopedWithFactory<IFakeService, FakeDependentServiceFactory>() },
                    { c => c.AddScopedWithFactory(serviceType, factoryType) },
                    { c => c.AddWithFactory(serviceType, factoryType, ServiceLifetime.Scoped) },

                    { c => c.AddSingletonWithFactory<IFakeService, FakeDependentServiceFactory>() },
                    { c => c.AddSingletonWithFactory(serviceType, factoryType) },
                    { c => c.AddWithFactory(serviceType, factoryType, ServiceLifetime.Singleton) }
                };
            }
        }

        [Theory]
        [MemberData(nameof(BasicImplementations))]
        [MemberData(nameof(DependentImplementations))]
        public void AddWithFactory_NullCollection(
            Func<IServiceCollection, IServiceCollection> addWithFactory) {

            // Arrange
            IServiceCollection collection = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => addWithFactory(collection)
            );
        }

        [Theory]
        [MemberData(nameof(BasicImplementations))]
        public void AddWithFactory_BasicImplementations(
            Func<IServiceCollection, IServiceCollection> addWithFactory) {

            // Arrange
            var collection = addWithFactory(new ServiceCollection());
            var provider = collection.BuildServiceProvider();

            // Act
            var service = provider.GetService<IFakeService>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<FakeService>(service);
        }

        [Theory]
        [MemberData(nameof(DependentImplementations))]
        public void AddWithFactory_DependentImplementations(
            Func<IServiceCollection, IServiceCollection> addWithFactory) {

            // Arrange
            var collection = addWithFactory(new ServiceCollection());
            collection.AddTransient<IFakeFactoryDependency, FakeFactoryDependency>();
            var provider = collection.BuildServiceProvider();

            // Act
            var service = provider.GetService<IFakeService>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<FakeService>(service);
        }

    }

}
