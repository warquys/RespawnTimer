using System.ComponentModel;
using Neuron.Core.Meta;
using Syml;

namespace RespawnTimer;

[Automatic]
[DocumentSection("Respawn Timer")]
public class RespawnTimerConfig : IDocumentSection
{
    [Description("Use the data base to save the user preference")]
    public bool RegisterPlayerPreference { get; set; } = false;
}