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
        this.Channels = _service.Parameters.GroupBy(g => g.ChannelId, g => new { g.Id, g.ParamName, g.Value }, (key, parameters) =>
        {
          return new MixerChannelViewModel
          {

            Id = key,
            Label = parameters.SingleOrDefault(p => p.ParamName == "Label")?.Value,
            Gain = parameters.SingleOrDefault(p => p.ParamName == "Gain")?.Value ?? 0,
            InputOutputChannels = parameters.Where(p => p.Value is bool)
              .Select(p => new ChannelParameterViewModel
              {
                Id = p.Id,
                ParamName = p.ParamName,
                Value = p.Value
              })
              .OrderBy(p => p.ParamName, new ParameterComparer())
          };
        });
      }
    }
  }
}
