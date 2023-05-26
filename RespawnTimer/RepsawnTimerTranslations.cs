using Neuron.Core.Meta;
using Neuron.Modules.Configs.Localization;

namespace RespawnTimer;

[Automatic]
public class RespawnTimerTranslations : Translations<RespawnTimerTranslations>
{
    public string RespawnMessage { get; set; } = @"\n\n\n\n\n\n\n\nTime for next respawn: {0}\n\n\n\n\n\n\n<align=""right""><color=blue>MTF Ticket {1}</color>\n<align=""right""><color=green>CHI Ticket {2}</color>";

    public string TimeFromat { get; set; } = "mm\\:ss";
}