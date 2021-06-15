# Gambo.ECS
A simple Entity-Component-System for .NET 5.0.

## Getting Started
Use structs to define data-only components.
```
public struct TagComponent
{
    public string Tag { get; }

    public TagComponent(string tag)
    {
        Tag = tag;
    }

    public TagComponent(TagComponent tagComponent)
    {
        Tag = tagComponent.Tag;
    }
}
```
Create a new `EcsContext` class, and use `registry.CreateEntity()` to create a new entity in the context's registry.
```
EcsContext context = new EcsContext();
EcsRegistry registry = context.Registry;
EcsEntity entity = registry.CreateEntity()
```
Use the newly created entity to create your component. Pass in your component's constructor parameters after the entity instance.
```
registry.AddComponent<TagComponent>(entity, "MyTag");
```
Implement the `EcsSystem` abstract class to define your own systems.
```
public class PrintTagSystem : EcsSystem
{
    public override void Dispatch()
    {
        foreach (var tagComponent in components)
        {
            Console.WriteLine(tagComponent.Tag);
        }
    }

    protected override void OnComponentAdded(object sender, ComponentEventArgs e)
    {
        if (e.Component is TagComponent tagComponent)
        {
            components.Add(tagComponent);
        }
    }

    protected override void OnComponentRemoved(object sender, ComponentEventArgs e)
    {
        if (e.Component is TagComponent tagComponent)
        {
            components.Remove(tagComponent);
        }
    }

    protected override void OnRegistryAttached()
    {
        components = Registry.GetComponentsOfType<TagComponent>().ToList();
    }

    private List<TagComponent> components = new List<TagComponent>();
}
```
Add your system to the context to attach it to the registry.
```
context.AddSystem<PrintTagSystem>();
```
