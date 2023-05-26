using System;
using System.Collections.Generic;
using System.Dynamic;
using MEC;
using Neuron.Core.Dev;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Neuron.Core.Plugins;
using Neuron.Modules.Configs.Localization;
using PlayerRoles;
using Respawning;
using Synapse3.SynapseModule;
using Synapse3.SynapseModule.Enums;
using Synapse3.SynapseModule.Events;
using Synapse3.SynapseModule.Map;
using Synapse3.SynapseModule.Player;
using Synapse3.SynapseModule.Teams;
using UnityEngine;

namespace RespawnTimer;

[Plugin(
    Name = "Respawn Timer",
    Description = "Add a respawn timer for the spectator",
    Version = "1.0.0",
    Author = "VT"
)]
public class RespawnTimerPlugin : ReloadablePlugin<RespawnTimerConfig, RespawnTimerTranslations>
{
    public const string DataKey = "RespawnTimer";

}

[Automatic]
public class EventHandler : Listener
{
    private readonly RespawnTimerPlugin _plugin;
    private readonly RoundService _round;
    private readonly PlayerService _player;
    private readonly TeamService _team;


    public EventHandler(RespawnTimerPlugin plugin
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
        var dataBaseValue = ev.Player.GetData(RespawnTimerPlugin.DataKey);
        if (dataBaseValue == null || !bool.TryParse(dataBaseValue, out value))
        {
            value = true;
            ev.Player.SetData(RespawnTimerPlugin.DataKey, true.ToString());
        }
        ev.Player.Data[RespawnTimerPlugin.DataKey] = value;
    }

    [EventHandler]
    public void Leave(LeaveEvent ev)
    {
        var value = ev.Player.Data[RespawnTimerPlugin.DataKey];
        ev.Player.SetData(RespawnTimerPlugin.DataKey, value.ToString());
    }

    [EventHandler]
    public void Start(RoundStartEvent ev)
    {
        Timing.RunCoroutine(TimerCoroutine());
    }

    private IEnumerator<float> TimerCoroutine()
    {
        yield return Timing.WaitForSeconds(2);
        var translations = _plugin.Translation;

        while (!_round.RoundEnded)
        {
            yield return Timing.WaitForSeconds(1f);

            var players = _player.GetPlayers(p => (p.RoleType == RoleTypeId.Spectator
                || p.RoleType == RoleTypeId.Overwatch), PlayerType.Player);
            
            foreach (var player in players)
            {
                try 
	            {
                    if (!(bool)player.Data[RespawnTimerPlugin.DataKey])
                        continue;

                    player.SendHint(GetDisplayMessage(player, translations), 1.25f);
                }
	            catch (Exception e)
	            {
                    SynapseLogger<RespawnTimerPlugin>.Error(e);
                    yield break;
                }   
            }

        }
    }

    private string GetDisplayMessage(SynapsePlayer player, RespawnTimerTranslations translations)
    {
        var playerTranslation =  player.GetTranslation(translations);

        var message = playerTranslation.RespawnMessage;
        message = message.Replace("\\n", "\n");
        var time = TimeSpan.FromSeconds(RespawnManager.Singleton._timeForNextSequence - RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);
        
        var timeTeam = _team.NextTeam == uint.MaxValue
                    ? time.ToString(playerTranslation.TimeFromat)
                    : $"({_team.GetTeamName(_team.NextTeam)}) {time.ToString(playerTranslation.TimeFromat)}";

        var mtfTicket = Mathf.Round(RespawnTokensManager.Counters[1].Amount);
        var chiTicket = Mathf.Round(RespawnTokensManager.Counters[0].Amount);
        return message.Format(timeTeam, mtfTicket, chiTicket);
    }
}