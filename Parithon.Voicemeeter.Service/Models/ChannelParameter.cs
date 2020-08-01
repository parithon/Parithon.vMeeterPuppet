using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parithon.Voicemeeter.Service.Models
{
  public class ChannelParameter
  {
    public string Id => $"{ChannelId}.{ParamName}";
    public string ChannelId { get; set; }
    public string ParamName { get; set; }
    public dynamic Value { get; set; }
  }
}
