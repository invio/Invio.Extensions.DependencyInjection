Started from frustrations that came out of this StackOverflow post, this is an implementation of the factory pattern that can work with types known at run-time. This does not solve the core problem yet.

# IFactory

- 'implementationFactory' is a factory
- This formalizes it with a `IFactory` concept
- Use a factory to add testable way to hydrate objects with conditional dependencies
