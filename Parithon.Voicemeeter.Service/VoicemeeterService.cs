using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parithon.Voicemeeter.Proxy;
using Parithon.Voicemeeter.Service.Hubs;
using Parithon.Voicemeeter.Service.Models;

namespace Parithon.Voicemeeter.Service
{
  public class VoicemeeterService : BackgroundService
  {
    private readonly ILogger<VoicemeeterService> _logger;
    private readonly IHubContext<VoicemeeterHub> _hub;
    private readonly VoicemeeterRemote _remote;
    public readonly ICollection<ChannelParameter> Parameters = new List<ChannelParameter>();

    public VoicemeeterService(ILogger<VoicemeeterService> logger, IHubContext<VoicemeeterHub> hub, VoicemeeterRemote remote)
    {
      _logger = logger;
      _hub = hub;
      _remote = remote;
    }

    public bool IsConnected { get; private set; }

    private string Caller = string.Empty;

    public async Task SetParameterAsync(string channelId, string paramName, float value)
    {
      this.Caller = channelId;
      await _remote.SetParameterFloatAsync(paramName, value);
    }

    public async Task SetParameterAsync(string channelId, string paramName, bool value)
    {
      this.Caller = channelId;
      await _remote.SetParameterFloatAsync(paramName, value ? 1 : 0);
    }

    private async Task GetParameter(Range stripIds, int physicalChannels, int virtualChannels)
    {
      //var changes = new Queue<(string id, dynamic oldValue, dynamic newValue)>();

      for (var stripId = stripIds.Start.Value; stripId <= stripIds.End.Value; stripId++)
      {
        var id = $"Strip[{stripId}]";
        CheckForUpdate($"{id}.Mono", await TryGetFloatValueAsync($"{id}.Mono") > 0);
        CheckForUpdate($"{id}.Mute", await TryGetFloatValueAsync($"{id}.Mute") > 0);
        CheckForUpdate($"{id}.Gain", await TryGetFloatValueAsync($"{id}.Gain"));
        CheckForUpdate($"{id}.Label", await TryGetStringValueAsync($"{id}.Label"));
        CheckForUpdate($"{id}.Solo", await TryGetFloatValueAsync($"{id}.Solo") > 0);
        CheckForUpdate($"{id}.Level", await TryGetFloatValueAsync($"{id}.Level"));
        CheckForUpdate($"{id}.MC", await TryGetFloatValueAsync($"{id}.MC") > 0);

        for (var physicalChannel = 0; physicalChannel <= physicalChannels; physicalChannel++)
        {
          var pc = $"{id}.A{physicalChannel + 1}";
          CheckForUpdate(pc, await TryGetFloatValueAsync(pc) > 0);
        }

        for (var virtualChannel = 0; virtualChannel <= virtualChannels; virtualChannel++)
        {
          var vc = $"{id}.B{virtualChannel + 1}";
          CheckForUpdate(vc, await TryGetFloatValueAsync(vc) > 0);
        }
      }

      async void CheckForUpdate(string id, dynamic value)
      {
        if (Parameters.Any(param => param.Id == id))
        {
          var param = Parameters.Single(p => p.Id == id);
          if (value != param.Value)
          {
            var arg = new { id, oldValue = (object)param.Value, newValue = (object)value };
            _logger.LogInformation($"Parameter changed: {arg.id}", arg);
            await _hub.Clients.AllExcept(this.Caller).SendAsync("ReceivedParameterUpdate", arg);
            // await _hub.Clients.All.SendAsync("ReceivedParameterUpdate", arg);
            param.Value = value;
          }
        }
        else
        {
          var channelId = id.Split(".")[0];
          var param = id.Split(".")[1];
          Parameters.Add(new ChannelParameter { ChannelId = channelId, ParamName = param, Value = value });
        }
      }

      async Task<string> TryGetStringValueAsync(string paramName)
      {
        (int result, string value) = await _remote.GetParameterStringAsync(paramName);
        if (result < 0) return string.Empty;
        return value;
      }

      async Task<float> TryGetFloatValueAsync(string paramName)
      {
        (int result, float value) = await _remote.GetParameterFloatAsync(paramName);
        if (result < 0) return float.MinValue;
        return value;
      }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Logging into the Voicemeeter Remote API");

      this.IsConnected = (await _remote.LoginAsync()) switch
      {
        0 => true,
        1 => await _remote.RunVoicemeeterAsync() == 0,
        _ => false,
      };

      (int vmResult, VoicemeeterType vmType) = await _remote.GetVoicemeeterTypeAsync();
      if (vmResult < 0)
        throw new ApplicationException("An error occurred while communicating with the Voicemeeter Remote API");
      
      while (!stoppingToken.IsCancellationRequested)
      {
        int isDirtyValue = await _remote.IsParametersDirtyAsync();
        if (isDirtyValue < 0)
          throw new ApplicationException("An error occurred whil communicating with the Voicemeeter Remote API");

        if (isDirtyValue > 0)
        {
          _logger.LogInformation("Parameters are dirty, updating...");
          switch (vmType)
          {
            case VoicemeeterType.Voicemeeter:
              await GetParameter(0..2, 2, 0);
              break;
            case VoicemeeterType.VoicemeeterBanana:
              await GetParameter(0..4, 3, 2);
              break;
            case VoicemeeterType.VoicemeeterPotato:
              await GetParameter(0..7, 5, 3);
              break;
          }
          _logger.LogInformation("Retrieved all parameters");
          this.Caller = string.Empty;
        }

        await Task.Delay(20);
      }

      if (this.IsConnected)
      {
        _logger.LogInformation("Logging out of the Voicemeeter Remote API");
        await _remote.LogoutAsync();
      }
    }
  }
}
