using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Parithon.vMeeterPuppet.Proxy.Helpers;

namespace Parithon.vMeeterPuppet.Proxy
{
  public class VoicemeeterRemote
  {
    private readonly ILogger<VoicemeeterRemote> _logger;

    public VoicemeeterRemote(ILogger<VoicemeeterRemote> logger)
    {
      _logger = logger;
      string installationPath = VoicemeeterHelpers.FindVoicemeeterInstallationPath();
      _logger.LogDebug("Using the following installation path", installationPath);
      InteropHelpers.SetDllDirectory(installationPath);
    }

    public Task<int> LoginAsync()
    {
      return RuntimeInformation.OSArchitecture == Architecture.X64
        ?
        Task.Run(() => InteropHelpers.VBVMR64_Login())
        :
        Task.Run(() => InteropHelpers.VBVMR_Login());
    }

    public Task<int> LogoutAsync()
    {
      return RuntimeInformation.OSArchitecture == Architecture.X64
        ?
        Task.Run(() => InteropHelpers.VBVMR64_Logout())
        :
        Task.Run(() => InteropHelpers.VBVMR_Logout());
    }

    public Task<int> RunVoicemeeterAsync()
    {
      return RuntimeInformation.OSArchitecture == Architecture.X64
        ?
        Task.Run(() => InteropHelpers.VBVMR64_RunVoicemeeter())
        :
        Task.Run(() => InteropHelpers.VBVMR_RunVoicemeeter());
    }

    public Task<int> IsParametersDirtyAsync()
    {
      return RuntimeInformation.OSArchitecture == Architecture.X64
        ?
        Task.Run(() => InteropHelpers.VBVMR64_IsParametersDirty())
        :
        Task.Run(() => InteropHelpers.VBVMR_IsParametersDirty());
    }

    public Task<(int result, VoicemeeterType type)> GetVoicemeeterTypeAsync()
    {
      return Task.Run(() =>
      {
        VoicemeeterType pType = VoicemeeterType.Unknown;
        int pResult = RuntimeInformation.OSArchitecture == Architecture.X64
          ?
          InteropHelpers.VBVMR64_GetVoicemeeterType(out pType)
          :
          InteropHelpers.VBVMR_GetVoicemeeterType(out pType);
        return (pResult, pType);
      });
    }

    public Task<(int result, Version version)> GetVoicemeeterVersionAsync()
    {
      return Task.Run(() =>
      {
        Version version = null;
        int pVersion = 0;
        int pResult = RuntimeInformation.OSArchitecture == Architecture.X64
          ?
          InteropHelpers.VBVMR64_GetVoicemeeterVersion(out pVersion)
          :
          InteropHelpers.VBVMR_GetVoicemeeterVersion(out pVersion);
        if (pResult == 0)
        {
          int major = Convert.ToInt32((pVersion & 0xFF000000) >> 24);
          int minor = Convert.ToInt32((pVersion & 0x00FF0000) >> 16);
          int build = Convert.ToInt32((pVersion & 0x0000FF00) >> 8);
          int revision = Convert.ToInt32(pVersion & 0x000000FF);
          version = new Version(major, minor, build, revision);
        }
        return (pResult, version);
      });
    }

    public Task<(int result, float value)> GetParameterFloatAsync(string paramName)
    {
      return Task.Run(() =>
      {
        float pValue = -1f;
        int pResult = RuntimeInformation.OSArchitecture == Architecture.X64
          ?
          InteropHelpers.VBVMR64_GetParameterFloat(VoicemeeterHelpers.GetParamNameBytes(paramName), out pValue)
          :
          InteropHelpers.VBVMR_GetParameterFloat(VoicemeeterHelpers.GetParamNameBytes(paramName), out pValue);
        return (pResult, pValue);
      });
    }

    public Task<(int result, string value)> GetParameterStringAsync(string paramName)
    {
      return Task.Run(() =>
      {
        byte[] pValue = new byte[512];
        int pResult = RuntimeInformation.OSArchitecture == Architecture.X64
          ?
          InteropHelpers.VBVMR64_GetParameterStringA(VoicemeeterHelpers.GetParamNameBytes(paramName), pValue)
          :
          InteropHelpers.VBVMR_GetParameterStringA(VoicemeeterHelpers.GetParamNameBytes(paramName), pValue);
        return (pResult, Encoding.ASCII.GetString(pValue).TrimEnd(char.MinValue));
      });
    }

    public Task<int> SetParameterFloatAsync(string paramName, float value)
    {
      return RuntimeInformation.OSArchitecture == Architecture.X64
        ?
        Task.Run(() => InteropHelpers.VBVMR64_SetParameterFloat(VoicemeeterHelpers.GetParamNameBytes(paramName), value))
        :
        Task.Run(() => InteropHelpers.VBVMR_SetParameterFloat(VoicemeeterHelpers.GetParamNameBytes(paramName), value));
    }
  }
}
