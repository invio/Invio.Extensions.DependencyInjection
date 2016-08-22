using System;

namespace Invio.Extensions.DependencyInjection.Fakes {

    public class FakeDependentServiceFactory : IFactory<IFakeService> {

        public FakeDependentServiceFactory(IFakeFactoryDependency dependency) {
            if (dependency == null) {
                throw new ArgumentNullException(nameof(dependency));
            }
        }

        public IFakeService Provide() {
            return new FakeService();
        }

    }

}
