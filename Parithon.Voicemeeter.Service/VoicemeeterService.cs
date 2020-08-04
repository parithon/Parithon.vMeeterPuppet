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
    public VoicemeeterType MixerType { get; private set; }

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

    private async Task GetParameter((int channelCount, ChannelParameterType[] parameters) hwInputChannels, (int channelCount, ChannelParameterType[] parameters) virtualInputChannels, int hwOutputChannels, int virtualOutputChannels)
    {
      for (var hwInputChannel = 0; hwInputChannel < hwInputChannels.channelCount; hwInputChannel++)
      {
        var id = $"Strip[{hwInputChannel}]";
        CheckForUpdate($"{id}.Label", await TryGetStringValueAsync($"{id}.Label", $"{hwInputChannel + 1}"));
        CheckForUpdate($"{id}.Gain", await TryGetFloatValueAsync($"{id}.Gain"));
        foreach (var parameter in hwInputChannels.parameters)
        {
          var paramName = Enum.GetName(typeof(ChannelParameterType), parameter);
          CheckForUpdate($"{id}.{paramName}", await TryGetFloatValueAsync($"{id}.{paramName}") > 0);
        }
      }

      for (var virtInput = 0; virtInput < virtualInputChannels.channelCount; virtInput++)
      {
        var id = $"Strip[{virtInput + hwInputChannels.channelCount}]";
        var virtNames = new[]
        {
          "VAIO",
          "AUX",
          "VAIO 3"
        };
        CheckForUpdate($"{id}.Label", await TryGetStringValueAsync($"{id}.Label", virtNames[virtInput]));
        foreach (var parameter in virtualInputChannels.parameters)
        {
          var paramName = Enum.GetName(typeof(ChannelParameterType), parameter);
          if (virtNames[virtInput] == "AUX" && paramName == "MC")
          {
            paramName = "K";
          }
          CheckForUpdate($"{id}.{paramName}", await TryGetFloatValueAsync($"{id}.{paramName}") > 0);
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

      async Task<string> TryGetStringValueAsync(string paramName, string defaultValue = "")
      {
        (int result, string value) = await _remote.GetParameterStringAsync(paramName);
        if (result < 0 || value.Length == 0) return defaultValue;
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

      while (!stoppingToken.IsCancellationRequested)
      {
        (int verResult, Version version) = await _remote.GetVoicemeeterVersionAsync();
        if (verResult < 0 && this.IsConnected)
        {
          await _hub.Clients.All.SendAsync("VoicemeeterClosed");
        }
        if (verResult == 0 && !this.IsConnected)
        {
          await _hub.Clients.All.SendAsync("VoicemeeterStarted");
        }
        this.IsConnected = verResult >= 0;

        if (this.IsConnected)
        {
          (int vmResult, VoicemeeterType vmType) = await _remote.GetVoicemeeterTypeAsync();
          if (vmResult < 0)
            throw new ApplicationException("An error occurred while communicating with the Voicemeeter Remote API");

          if (vmType != this.MixerType)
          {
            this.MixerType = vmType;
            this.Parameters.Clear();
            await _hub.Clients.All.SendAsync("VoicemeeterTypeChanged");
          }

          int isDirtyValue = await _remote.IsParametersDirtyAsync();
          if (isDirtyValue < 0)
            throw new ApplicationException("An error occurred while communicating with the Voicemeeter Remote API");

          if (isDirtyValue > 0)
          {
            _logger.LogInformation("Parameters are dirty, updating...");
            switch (vmType)
            {
              case VoicemeeterType.Voicemeeter:
                await GetParameter((2, new[] {
                  ChannelParameterType.A1,
                  ChannelParameterType.B1,
                  ChannelParameterType.Mono,
                  ChannelParameterType.Solo,
                  ChannelParameterType.Mute
                }), (1, new[] {
                  ChannelParameterType.A1,
                  ChannelParameterType.B1,
                  ChannelParameterType.MC,
                  ChannelParameterType.Solo,
                  ChannelParameterType.Mute
                }), 1, 1);
                break;
              case VoicemeeterType.VoicemeeterBanana:
                await GetParameter((3, new[] {
                  ChannelParameterType.A1,
                  ChannelParameterType.A2,
                  ChannelParameterType.A3,
                  ChannelParameterType.B1,
                  ChannelParameterType.B2,
                  ChannelParameterType.Mono,
                  ChannelParameterType.Solo,
                  ChannelParameterType.Mute
                }), (2, new[] {
                  ChannelParameterType.A1,
                  ChannelParameterType.A2,
                  ChannelParameterType.A3,
                  ChannelParameterType.B1,
                  ChannelParameterType.B2,
                  ChannelParameterType.MC,
                  ChannelParameterType.Solo,
                  ChannelParameterType.Mute
                }), 3, 2);
                break;
              case VoicemeeterType.VoicemeeterPotato:
                await GetParameter((5, new[] {
                  ChannelParameterType.A1,
                  ChannelParameterType.A2,
                  ChannelParameterType.A3,
                  ChannelParameterType.A4,
                  ChannelParameterType.A5,
                  ChannelParameterType.B1,
                  ChannelParameterType.B2,
                  ChannelParameterType.B3,
                  ChannelParameterType.Mono,
                  ChannelParameterType.Solo,
                  ChannelParameterType.Mute
                }), (3, new[] {
                  ChannelParameterType.A1,
                  ChannelParameterType.A2,
                  ChannelParameterType.A3,
                  ChannelParameterType.A4,
                  ChannelParameterType.A5,
                  ChannelParameterType.B1,
                  ChannelParameterType.B2,
                  ChannelParameterType.B3,
                  ChannelParameterType.MC,
                  ChannelParameterType.Solo,
                  ChannelParameterType.Mute
                }), 5, 3);
                break;
            }
            _logger.LogInformation("Retrieved all parameters");
            this.Caller = string.Empty;
          }
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
