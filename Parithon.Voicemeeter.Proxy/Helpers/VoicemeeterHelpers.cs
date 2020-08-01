using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Parithon.Voicemeeter.Proxy.Helpers
{
  public static class VoicemeeterHelpers
  {
    public static string FindVoicemeeterInstallationPath()
    {
      string keyName = string.Empty;
      switch (System.Runtime.InteropServices.RuntimeInformation.OSArchitecture)
      {
        case System.Runtime.InteropServices.Architecture.X86:
          keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\VB:Voicemeeter {17359A74-1236-5467}";
          break;
        case System.Runtime.InteropServices.Architecture.X64:
          keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\VB:Voicemeeter {17359A74-1236-5467}";
          break;
      }
      var result = Registry.GetValue(keyName, "UninstallString", string.Empty) as string;
      if (string.IsNullOrEmpty(result))
        return string.Empty;

      return System.IO.Path.GetDirectoryName(result);
    }

    public static byte[] GetParamNameBytes(string paramName)
    {
      return Encoding.ASCII.GetBytes(paramName + char.MinValue);
    }
  }
}
