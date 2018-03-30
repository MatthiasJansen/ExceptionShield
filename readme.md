# ExceptionShield

ExceptionShield is a exception manager, with the simple goal of making exception management exceptionally simple.

## Builds & CI

| Branch      | Status           | Coverage |
|:-----------:|:----------------:|:--------:|
| master      | ![Build Status](https://matthias-jansen.visualstudio.com/_apis/public/build/definitions/b510d81b-9eed-4f52-be90-b5b864a4aa98/4/badge) | [![codecov](https://codecov.io/gh/MatthiasJansen/ExceptionShield/branch/master/graph/badge.svg)](https://codecov.io/gh/MatthiasJansen/ExceptionShield) |
| release 0.8 | ![Build Status](https://matthias-jansen.visualstudio.com/_apis/public/build/definitions/b510d81b-9eed-4f52-be90-b5b864a4aa98/4/badge) | [![codecov](https://codecov.io/gh/MatthiasJansen/ExceptionShield/branch/release/0.8/graph/badge.svg)](https://codecov.io/gh/MatthiasJansen/ExceptionShield) |


## Responsibilities
## Particularities

### About

#### Policy Groups

Policy groups are used to take care of special cases. In many situations a single policy will suffice to handle a certain exception.
The concept of policy groups enables us to define handling for special cases. These special cases are differentiated by context.
Each group requires at least one policy which is the default policy. This policy takes effect whenever no more specific context could be matched.
Additional policies can be provided together with the context in which they are supposed to take effect.

#### Policies

#### Handlers

A handler is pretty much a conversion step in the pipeline.
It takes an exception and returns a different one.
This can be an exception of the same type.

Handlers are meant to be stateless, however nothing prevents you from creating a handler that has state.
Keep in mind however that such a handler needs to be thread safe.

#### Terminators

Whenever the exception resulting from a policy is not supposed to be thrown in the end, a terminator is required.

### Examples

#### Service Configuration

#### Service Usage

#### Integration
##### IOC/DI
##### ORM
##### Testing
##### Other Frameworks

Coming soon!