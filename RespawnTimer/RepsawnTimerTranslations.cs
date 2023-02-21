using Neuron.Core.Meta;
using Neuron.Modules.Configs.Localization;

namespace RespawnTimer;

[Automatic]
public class RepsawnTimerTranslations : Translations<RepsawnTimerTranslations>
{
    public string RespawnMessage = @"Time for next respawn: {0}\n<align=""right""><color=blue>MTF Ticket {1}</color>\n<align=""right""><color=green>CHI Ticket {2}</color>";

}