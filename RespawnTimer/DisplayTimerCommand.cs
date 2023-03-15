using Neuron.Core.Meta;
using Neuron.Modules.Commands;
using Synapse3.SynapseModule.Command;

namespace RespawnTimer
{
    [Automatic]
    [SynapseCommand(
        CommandName = "RespawnTimer",
        Description = "hide or show the respawn timer\r\n",
        Aliases = new[] { "rt" },
        Platforms = new[] { CommandPlatform.PlayerConsole }
    )]
    public class DisplayTimerCommand : SynapseCommand
    {
        public override void Execute(SynapseContext context, ref CommandResult result)
        {
            var newValue = !(bool)context.Player.Data[RespawnTimerPlugin.DataKey];
            context.Player.Data[RespawnTimerPlugin.DataKey] = newValue;
        }
    }
}
