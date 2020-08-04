using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Parithon.vMeeterPuppet.Service.Hubs
{
  public class VoicemeeterHub : Hub
  {
    private readonly ILogger<VoicemeeterHub> _logger;
    private readonly VoicemeeterService _service;

    public VoicemeeterHub(ILogger<VoicemeeterHub> logger, VoicemeeterService service)
    {
      _logger = logger;
      _service = service;
    }

    public async Task SetParameterFloatValue(string paramName, float value)
    {
      _logger.LogInformation("SetParameterFloatValue", paramName, value);
      await _service.SetParameterAsync(Context.ConnectionId, paramName, value);
    }

    public async Task SetParameterBooleanValue(string paramName, bool value)
    {
      _logger.LogInformation($"[SetParameterBooleanValue] {paramName}: {value}", paramName, value);
      await _service.SetParameterAsync(Context.ConnectionId, paramName, value);
    }
  }
}
