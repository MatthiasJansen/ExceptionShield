# Exceptional

Exceptional is a exception manager, with the simple goal of making exception management exceptionally simple.



## Responsibilities
## Particularities

### About

#### Policy Groups

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