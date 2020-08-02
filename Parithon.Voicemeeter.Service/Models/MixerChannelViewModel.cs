using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parithon.Voicemeeter.Service.Models
{
  public class MixerChannelViewModel
  {
    public string Id { get; set; }
    public string Label { get; set; }
    public float Gain { get; set; }
    public IEnumerable<ChannelParameterViewModel> InputOutputChannels { get; set; } = new List<ChannelParameterViewModel>();
  }
}
