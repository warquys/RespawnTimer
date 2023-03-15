using MEC;
using Neuron.Core.Dev;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Neuron.Core.Plugins;
using PlayerRoles;
using Respawning;
using Synapse3.SynapseModule;
using Synapse3.SynapseModule.Enums;
using Synapse3.SynapseModule.Events;
using Synapse3.SynapseModule.Map;
using Synapse3.SynapseModule.Player;
using Synapse3.SynapseModule.Teams;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RespawnTimer;

[Plugin(
    Name = "Respawn Timer",
    Description = "Add a respawn timer for the spectator",
    Version = "1.0.0",
    Author = "VT"
)]
public class RepsawnTimerPlugin : ReloadablePlugin<RepsawnTimerConfig, RespawnTimerTranslations>
{
    public const string DataKey = "RespawnTimer";

}

[Automatic]
public class ExampleEventHandler : Listener
{
    private readonly RepsawnTimerPlugin _plugin;
    private readonly RoundService _round;
    private readonly PlayerService _player;
    private readonly TeamService _team;


    public ExampleEventHandler(RepsawnTimerPlugin plugin
        , RoundService round, PlayerService player, TeamService team)
    {
        _plugin = plugin;
        _round = round;
        _player = player;
        _team = team;
    }


    [EventHandler]
    public void Join(JoinEvent ev)
    {
        bool value;
        var dataBaseValue = ev.Player.GetData(RepsawnTimerPlugin.DataKey);
        if (!bool.TryParse(dataBaseValue, out value))
        {
            value = true;
            ev.Player.SetData(RepsawnTimerPlugin.DataKey, true.ToString());
        }
        ev.Player.Data[RepsawnTimerPlugin.DataKey] = value;
    }

    [EventHandler]
    public void Leave(LeaveEvent ev)
    {
        var value = ev.Player.Data[RepsawnTimerPlugin.DataKey];
        ev.Player.SetData(RepsawnTimerPlugin.DataKey, value.ToString());
    }

    [EventHandler]
    public void Start(RoundStartEvent ev)
    {
        Timing.RunCoroutine(TimerCoroutine());
    }

    private IEnumerator<float> TimerCoroutine()
    {
        yield return Timing.WaitForSeconds(2);
        while (!_round.RoundEnded)
        {
            yield return Timing.WaitForSeconds(1f);

            var translations = _plugin.Translation;
            var players = _player.GetPlayers(p => (p.RoleType == RoleTypeId.Spectator
                || p.RoleType == RoleTypeId.Overwatch), PlayerType.Player);
            foreach (var player in players)
            {
                if ((bool)player.Data[RepsawnTimerPlugin.DataKey])
                    continue;

                var mesasge = player.GetTranslation(translations).RespawnMessage;
                mesasge = mesasge.Replace("\\n", "\n");
                var time = TimeSpan.FromSeconds(RespawnManager.Singleton._timeForNextSequence - RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);
                var timeTeam = _team.NextTeam == uint.MaxValue 
                    ? time.ToString("mm\\:ss") 
                    : $"({_team.GetTeamName(_team.NextTeam)}) {time.ToString("mm\\:ss")}";
                var mtfTicket = Mathf.Round(RespawnTokensManager.Counters[1].Amount);
                var chiTicket = Mathf.Round(RespawnTokensManager.Counters[0].Amount);
                mesasge = mesasge.Format(timeTeam, mtfTicket, chiTicket);
                player.SendHint(mesasge, 1.25f);
            }

        }
    }
}