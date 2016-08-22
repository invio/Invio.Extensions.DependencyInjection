namespace Invio.Extensions.DependencyInjection.Fakes {

    public class FakeServiceFactory : IFactory<IFakeService> {

        public IFakeService Provide() {
            return new FakeService();
        }

    }

}
