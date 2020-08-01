using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Parithon.Voicemeeter.Proxy;
using Parithon.Voicemeeter.Service.Models;

namespace Parithon.Voicemeeter.Service.Pages
{
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;
    private readonly VoicemeeterService _service;

    public IndexModel(ILogger<IndexModel> logger, VoicemeeterService service)
    {
      _logger = logger;
      _service = service;
    }

    [BindProperty]
    public IEnumerable<MixerChannelViewModel> Channels { get; private set; } = new List<MixerChannelViewModel>();

    public void OnGet()
    {
      if (_service.IsConnected)
      {
        this.Channels = _service.Parameters.GroupBy(g => g.ChannelId, g => new { g.Id, g.ParamName, g.Value }, (key, parameters) => {
          return new MixerChannelViewModel
          {
            A1 = parameters.SingleOrDefault(p => p.ParamName == "A1")?.Value ?? false,
            A2 = parameters.SingleOrDefault(p => p.ParamName == "A2")?.Value ?? false,
            A3 = parameters.SingleOrDefault(p => p.ParamName == "A3")?.Value ?? false,
            A4 = parameters.SingleOrDefault(p => p.ParamName == "A4")?.Value ?? false,
            A5 = parameters.SingleOrDefault(p => p.ParamName == "A5")?.Value ?? false,
            B1 = parameters.SingleOrDefault(p => p.ParamName == "B1")?.Value ?? false,
            B2 = parameters.SingleOrDefault(p => p.ParamName == "B2")?.Value ?? false,
            B3 = parameters.SingleOrDefault(p => p.ParamName == "B3")?.Value ?? false,
            Comp = parameters.SingleOrDefault(p => p.ParamName == "Comp")?.Value ?? 0,
            EQ = parameters.SingleOrDefault(p => p.ParamName == "EQ")?.Value ?? false,
            Gain = parameters.SingleOrDefault(p => p.ParamName == "Gain")?.Value ?? 0,
            Id = key,
            Label = parameters.SingleOrDefault(p => p.ParamName == "Label")?.Value,
            Level = parameters.SingleOrDefault(p => p.ParamName == "Level")?.Value ?? 0,
            MC = parameters.SingleOrDefault(p => p.ParamName == "MC")?.Value ?? false,
            Mono = parameters.SingleOrDefault(p => p.ParamName == "Mono")?.Value ?? false,
            Mute = parameters.SingleOrDefault(p => p.ParamName == "Mute")?.Value ?? false,
            Solo = parameters.SingleOrDefault(p => p.ParamName == "Solo")?.Value ?? false,
          };
        });
      }
    }
  }
}
