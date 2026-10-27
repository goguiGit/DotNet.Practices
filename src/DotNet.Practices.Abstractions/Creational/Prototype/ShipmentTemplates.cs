using DotNet.Practices.Abstractions.Creational.Prototype.Abstracts;

namespace DotNet.Practices.Abstractions.Creational.Prototype;

// Prototype registry: callers get a copy they can change without touching the template.
public sealed class ShipmentTemplates
{
    private readonly Dictionary<string, ShipmentTemplate> _templates = [];

    public void Register(string name, ShipmentTemplate template) => _templates[name] = template;

    public ShipmentTemplate Create(string name) => _templates[name].Clone();
}
