Started from frustrations that came out of [this StackOverflow post](http://stackoverflow.com/questions/39029344/factory-pattern-with-open-generics), this is an implementation of the factory pattern that can work with types known at run-time.

[![Build status](https://ci.appveyor.com/api/projects/status/26pr0w6y0jw2p3rn/branch/master?svg=true)](https://ci.appveyor.com/project/carusology/invio-extensions-dependencyinjection/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Invio.Extensions.DependencyInjection.svg)](https://www.nuget.org/packages/Invio.Extensions.DependencyInjection/)
[![Travis](https://img.shields.io/travis/invio/Invio.Extensions.DependencyInjection.svg?maxAge=3600&label=travis)](https://travis-ci.org/invio/Invio.Extensions.DependencyInjection)

# Factory

- 'implementationFactory' is a factory
- This formalizes it with a `IFactory` concept
- Use a factory to add testable way to hydrate objects with conditional dependencies
