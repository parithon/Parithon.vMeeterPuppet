using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

  public enum ChannelParameterType: int
  {
    A1=0,
    A2=1,
    A3=2,
    A4=3,
    A5=4,
    B1=5,
    B2=6,
    B3=7,
    B4=8,
    B5=9,
    Mono=10,
    Solo=11,
    MC=12,
    K=13,
    Mute=14
  }

  public class ParameterComparer : IComparer<string>
  {
    public int Compare([AllowNull] string x, [AllowNull] string y)
    {
      if (x != null && y != null)
      {
        int valx = (int)Enum.Parse<ChannelParameterType>(x, true);
        int valy = (int)Enum.Parse<ChannelParameterType>(y, true);
        return valx > valy ? 1 : 0;
      }
      return 0;
    }
  }
}

