# Changelog Gambo.ECS
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

### Version 0.4.0 - 2021-07-14
 - EcsContext:
   - Must now be constructed using EcsContextBuilder
   - Added support for resolving system services when adding a system.
 - Removed single instance per type constraint for EcsRegistry
   - Added [Unique] attribute which can be used to specify components that should be unique per entity

### Version 0.3.2
 - Removed ComponentView and its associated methods

### Version 0.3.1
 - Improved performance of View methods

### Version 0.3.0
 - Added additional 'View' methods that returns tuples instead of ComponentView.
 - Set ComponentView and its related methods to be deprecated.
 - Added method 'FindComponentOfType<T>' to registry

### Version 0.2.1
 - Removed abstract 'Dispatch' method on 'EcsSystem'
 - Set Systems to be enabled by default

### Version 0.2.0
 - Added views for retrieving component collections per entity

### Version 0.1.0
 - System base class for creating custom systems
 - Simple Entity class, only defined by an ID
 - Registry class for registering entites and their components
 - Context class for maintaining a context of entities, components and systems