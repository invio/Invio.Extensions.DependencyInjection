using System;
using Microsoft.Extensions.DependencyInjection;
using Invio.Extensions.DependencyInjection.Fakes;
using Xunit;

namespace Invio.Extensions.DependencyInjection {

    public class ServiceCollectionFactoryExtensionsTests {

        [Fact]
        public void AddTransientWithFactory_StaticallyTyped_NullCollection() {

            // Arrange
            IServiceCollection collection = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => collection.AddTransientWithFactory<IFakeService, FakeDependentServiceFactory>()
            );
        }

        [Fact]
        public void AddTransientWithFactory_StaticallyTyped_NoInnerDependency() {

            // Arrange
            var collection = new ServiceCollection();
            collection.AddTransientWithFactory<IFakeService, FakeServiceFactory>();

            var provider = collection.BuildServiceProvider();

            // Act
            var service = provider.GetService<IFakeService>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<FakeService>(service);
        }

        [Fact]
        public void AddTransientWithFactory_StaticallyTyped_WithInnerDependency() {

            // Arrange
            var collection = new ServiceCollection();
            collection.AddTransient<IFakeFactoryDependency, FakeFactoryDependency>();
            collection.AddTransientWithFactory<IFakeService, FakeDependentServiceFactory>();

            var provider = collection.BuildServiceProvider();

            // Act
            var service = provider.GetService<IFakeService>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<FakeService>(service);
        }

    }

}
