using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                Func<IServiceCollection, IServiceCollection> addDependentService =
                    c => c.AddTransient<IFakeFactoryDependency, FakeFactoryDependency>();

                var cases = new List<Func<IServiceCollection, IServiceCollection>> {
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

                var theoryData = new TheoryData<Func<IServiceCollection, IServiceCollection>>();
                foreach (var caseFunc in cases) {
                    theoryData.Add(c => caseFunc(addDependentService(c)));
                }

                return theoryData;
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
        [MemberData(nameof(DependentImplementations))]
        public void AddWithFactory_GetService(
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

        /// <summary>
        /// Considering the reflection that is being executed to run the factory.
        /// This test ensures that we can resolve via this method around 100K without
        /// taking more than a 200 ms penalty.
        ///
        /// When running this locally any of the transient calls  were around 52 ms for 100K, and
        /// everything else was just at 11 ms for 100K.
        ///
        /// Invoke is better for maintenance, but poor for performance. If this becomes a problem then
        /// it is likely that we'll need to switch to delegates.
        /// http://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
        /// </summary>
        [Theory]
        [MemberData(nameof(BasicImplementations))]
        [MemberData(nameof(DependentImplementations))]
        public void GetService_ExecutionTime(
            Func<IServiceCollection, IServiceCollection> addWithFactory) {
            const int executionCount = 100000;

            // Arrange
            var collection = addWithFactory(new ServiceCollection());
            var provider = collection.BuildServiceProvider();

            // Act
            var stopWatch = Stopwatch.StartNew();
            for (var i = 0; i < executionCount; i++) {
                provider.GetService<IFakeService>();
            }
            stopWatch.Stop();

            // Assert
            Assert.True(stopWatch.ElapsedMilliseconds < 200);
        }

        public static TheoryData RuntimeTypedImplementations {
            get {
                return new TheoryData<Func<IServiceCollection, Type, Type, IServiceCollection>> {
                    {
                        (collection, service, factory) =>
                            collection.AddTransientWithFactory(service, factory)
                    },
                    {
                        (collection, service, factory) =>
                            collection.AddWithFactory(service, factory, ServiceLifetime.Transient)
                    },
                    {
                        (collection, service, factory) =>
                            collection.AddScopedWithFactory(service, factory)
                    },
                    {
                        (collection, service, factory) =>
                            collection.AddWithFactory(service, factory, ServiceLifetime.Scoped)
                    },
                    {
                        (collection, service, factory) =>
                            collection.AddSingletonWithFactory(service, factory)
                    },
                    {
                        (collection, service, factory) =>
                            collection.AddWithFactory(service, factory, ServiceLifetime.Singleton)
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(RuntimeTypedImplementations))]
        public void AddWithFactory_NullServiceType(
            Func<IServiceCollection, Type, Type, IServiceCollection> addWithFactory) {

            // Arrange
            var collection = new ServiceCollection();
            var factoryType = typeof(FakeServiceFactory);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => addWithFactory(collection, null, factoryType)
            );
        }

        [Theory]
        [MemberData(nameof(RuntimeTypedImplementations))]
        public void AddWithFactory_NullFactoryType(
            Func<IServiceCollection, Type, Type, IServiceCollection> addWithFactory) {

            // Arrange
            var collection = new ServiceCollection();
            var serviceType = typeof(IFakeService);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => addWithFactory(collection, serviceType, null)
            );
        }

    }

}
