using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parithon.Voicemeeter.Service.Models
{
  public class ChannelParameterViewModel
  {
    public string Id { get; set; }
    public string ParamName { get; set; }
    public bool? Value { get; set; }
  }
}
