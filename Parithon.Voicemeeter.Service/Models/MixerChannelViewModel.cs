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
    public bool A1 { get; set; }
    public bool A2 { get; set; }
    public bool A3 { get; set; }
    public bool A4 { get; set; }
    public bool A5 { get; set; }
    public bool B1 { get; set; }
    public bool B2 { get; set; }
    public bool B3 { get; set; }
    public bool Mono { get; set; }
    public bool MC { get; set; }
    public bool EQ { get; set; }
    public bool Solo { get; set; }
    public bool Mute { get; set; }
    public float Gain { get; set; }
    public float Level { get; set; }
    public float Comp { get; set; }
  }
}
