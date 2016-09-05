# Invio.Extensions.DependencyInjection

[![Appveyor](https://ci.appveyor.com/api/projects/status/26pr0w6y0jw2p3rn/branch/master?svg=true)](https://ci.appveyor.com/project/carusology/invio-extensions-dependencyinjection/branch/master)
[![Travis CI](https://img.shields.io/travis/invio/Invio.Extensions.DependencyInjection.svg?maxAge=3600&label=travis)](https://travis-ci.org/invio/Invio.Extensions.DependencyInjection)
[![NuGet](https://img.shields.io/nuget/v/Invio.Extensions.DependencyInjection.svg)](https://www.nuget.org/packages/Invio.Extensions.DependencyInjection/)
[![Coverage](https://coveralls.io/repos/github/invio/Invio.Extensions.DependencyInjection/badge.svg?branch=master)](https://coveralls.io/github/invio/Invio.Extensions.DependencyInjection?branch=master)

This library implements the [factory pattern](https://en.wikipedia.org/wiki/Factory_\(object-oriented\_programming\)) as extension methods to [Microsoft's ASP.NET Core Dependency Injection library](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html). It is [.NET Standard 1.3](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) compliant for cross-platform use.

# Installation
The latest version of this package is available on NuGet. To install, run the following command:

```
PM> Install-Package Invio.Extensions.DependencyInjection
```

# Usage

In Microsoft's Dependency Injection framework, a developer adds a collection of `ServiceDescriptor` objects to an `IServiceCollection` during an ASP.NET application's initial call to `ConfigureServices` in the `Startup` class. Say, for example, we had a service with the interface of `IExampleService` which in turn had an implementation of `ExampleService`. This could be registered in a variety of ways, such as:

```csharp
// Simple implementationType example

public void ConfigureServices(IServiceCollection services) {
    services.AddSingleton<IExampleService, ExampleService>();
    services.AddSingleton<IExampleService>(new ExampleService());
    services.AddSingleton(typeof(IExampleService), typeof(ExampleService));
}
```

Sometimes, the hydration of a service may be dependent on some level of configuration. For example, maybe a string from your `appsettings.json` file is needed to determine how to provide an `IExampleService`. This could be defined with an `implementationFactory` lambda, which receives an implementation of `IServiceProvider` in and expects a valid implementation of `IExampleService` back out. This can be registered in a variety of ways, such as:

```csharp
// Simple implementationFactory example

public IConfigurationRoot Configuration { get; }

public void ConfigureServices(IServiceCollection services) {
    var myConfig = Configuration["MyConfigurationString"];

    services.AddSingleton<IExampleService>(provider => new ExampleService(myConfig));
    services.AddSingleton(typeof(IExampleService), provider => new ExampleService(myConfig));
}
```

This lambda works well for trivial dependencies, but what if the creation of `ExampleService` was complex? Perhaps it requires a few dependencies that are defined in your `IServiceProvider` as well as some decisions based upon configuration defined in `appsettings.json` that have been modularized into [Microsoft's Options framework](https://docs.asp.net/en/latest/fundamentals/configuration.html)? It could become unwieldly, like this:

```csharp
// Complex implementationFactory example

public IConfigurationRoot Configuration { get; }

public void ConfigureServices(IServiceCollection services) {
    services.Configure<DependentConfiguration>(Configuration.GetSection("Dependent"));
    services.AddSingleton<IDependencyFoo, DependencyFoo>();
    services.AddTransient<IDependencyBar, DependencyBar>();

    services.AddSingleton(
        typeof(IExampleService),
        provider => {
            var configuration = provider.GetRequiredService<IOptions<DependentConfiguration>>().Value;
            var fooService = provider.GetRequiredService<IDependencyFoo>();
            var barService = provider.GetRequiredService<IDependencyBar>();

            if (configuration.IsFooWrapped) {
                fooService = new Wrapper<IDependencyFoo>(fooService);
            }

            if (!configuration.IsImmutable) {
                barService.Mood = Mood.Depressed;
                barService.SadTimes = true;
            }

            return new ExampleServiceBehavior(
                new ExampleService(fooService, barService)
            );
        }
    );

}
```

Now we're introducing real complexity into an area that is difficult to test. This is where this library's utilization of the factory pattern can be employed. This library adds a new interface, [`IFactory<T>`](https://github.com/invio/Invio.Extensions.DependencyInjection/blob/master/src/Invio.Extensions.DependencyInjection/IFactory.cs), that has a single method, `Provide()`, that returns a service of type `T`. By defining the factory as another service within the `IServiceCollection`, we can reroute `initializationFactory` lambda's complexity into something that can be unit tested, like so:

```csharp
public class ExampleServiceFactory : IFactory<IExampleService> {

    private IOptions<DependentConfiguration> options { get; }
    private IDependencyFoo fooService { get; }
    private IDependencyBar barService { get; }

    public ExampleServiceFactory(
        IOptions<DependentConfiguration> options,
        IDependencyFoo fooService,
        IDependencyBar barService) {

        if (options == null) {
            throw new ArgumentNullException(nameof(options));
        } else if (fooService == null) {
            throw new ArgumentNullException(nameof(fooService));
        } else if (barService == null) {
            throw new ArgumentNullException(nameof(barService));
        }

        this.options = options;
        this.fooService = fooService;
        this.barService = barService;
    }

    public T Provide() {
        var configuration = options.Value;

        if (configuration.IsFooWrapped) {
            fooService = new Wrapper<IDependencyFoo>(fooService);
        }

        if (!configuration.IsImmutable) {
            barService.Mood = Mood.Depressed;
            barService.SadTimes = true;
        }

        return new ExampleServiceBehavior(
            new ExampleService(fooService, barService)
        );
    }

}
```

Now our `ConfigureServices` method in the `Startup` class can be defined to use the `IFactory<IExampleService>` implementation to provide an `IExampleService` like so.

```csharp
// Refactored implementationFactory using IFactory<IExampleService>

public IConfigurationRoot Configuration { get; }

public void ConfigureServices(IServiceCollection services) {
    services.Configure<DependentConfiguration>(Configuration.GetSection("Dependent"));
    services.AddSingleton<IDependencyFoo, DependencyFoo>();
    services.AddTransient<IDependencyBar, DependencyBar>();

    // The factory is another service now.
    services.AddTransient<IFactory<IExampleService>, ExampleServiceFactory>();

    services.AddSingleton(
        typeof(IExampleService),
        provider => provider.GetRequiredService<IFactory<IExampleService>>().Provide()
    );
}
```

That's a good step, but we can go further. Use [the `IServiceCollection` extension methods](https://github.com/invio/Invio.Extensions.DependencyInjection/blob/master/src/Invio.Extensions.DependencyInjection/ServiceCollectionFactoryExtensions.cs) provided in this library to further drop down the amount of boilerplate code, like so:

```csharp
// Refactored boilerplate implementationFactory into extension method

public IConfigurationRoot Configuration { get; }

public void ConfigureServices(IServiceCollection services) {
    services.Configure<DependentConfiguration>(Configuration.GetSection("Dependent"));
    services.AddSingleton<IDependencyFoo, DependencyFoo>();
    services.AddTransient<IDependencyBar, DependencyBar>();

    services.AddSingletonWithFactory<IExampleService, ExampleServiceFactory>();
}
```

That's it. <3
