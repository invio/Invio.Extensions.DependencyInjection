using System;

namespace Invio.Extensions.DependencyInjection {

    public interface IFactory<T> {

        T Provide();

    }

}
