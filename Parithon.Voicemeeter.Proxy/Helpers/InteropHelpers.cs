using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Parithon.vMeeterPuppet.Proxy.Helpers
{
  internal static class InteropHelpers
  {
    #region kernel32.dll
    /// <summary>
    /// Set the search location for DLLs
    /// </summary>
    /// <param name="lpPathName">The location to search for DLLs</param>
    /// <returns></returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);
    #endregion

    #region VoicemeeterRemote.dll
    /// <summary>
    /// Open communication pipe with Voicemeeter
    /// </summary>
    /// <returns>
    ///  0: OK
    ///  1: OK but Voicemeeter Application is not running
    /// -1: cannot get client
    /// -2: unexpected login (logout was not called before a subsequent login)
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_Login();

    /// <summary>
    /// Close communication pipe with Voicemeeter
    /// </summary>
    /// <returns>
    ///  0: OK
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_Logout();

    /// <summary>
    /// Run Voicemeeter Application (automatically retrieves the installation location and runs voicemeeter)
    /// </summary>
    /// <returns>
    ///  0: OK
    /// -1: not installed
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_RunVoicemeeter();

    /// <summary>
    /// Get the Voicemeeter type
    /// </summary>
    /// <param name="pType">
    /// A pointer which represents the type of Voicemeeter client
    /// 1: Voicemeeter (original)
    /// 2: Voicemeeter Banana
    /// 3: Voicemeeter Potato
    /// 
    /// VOICEMEETER STRIP/BUS INDEX ASSIGNMENT
    /// | STRIP 1 | STRIP 2 | VIRTUAL INPUT | BUS A | BUS B |
    /// |+-------+|+-------+|+-------------+|+-----+|+-----+|
    /// |    0    |    1    |       2       |   0   |   1   |
    /// 
    /// VOICEMEETER BANANA STRIP/BUS INDEX ASSIGNMENT
    /// | STRIP 1 | STRIP 2 | STRIP 3 | VIRTUAL INPUT | VIRTUAL AUX | BUS A1 | BUS A2 | BUS A3 | BUS B1 | BUS B2 |
    /// |+-------+|+-------+|+-------+|+-------------+|+-----------+|+------+|+------+|+------+|+------+|+------+|
    /// |    0    |    1    |    2    |       3       |      4      |    0   |    1   |    2   |    3   |    4   |
    /// 
    /// VOICEMEETER POTATO STRIP/BUS INDEX ASSIGNMENT
    /// | STRIP 1 | STRIP 2 | STRIP 3 | STRIP 4 | STRIP 5 | VIRTUAL INPUT | VIRTUAL AUX | VAIO 3 | BUS A1 | BUS A2 | BUS A3 | BUS A4 | BUS A5 | BUS B1 | BUS B2 | BUS B3 |
    /// |+-------+|+-------+|+-------+|+-------+|+-------+|+-------------+|+-----------+|+------+|+------+|+------+|+------+|+------+|+------+|+------+|+------+|+------+|
    /// |    0    |    1    |    2    |    3    |    4    |       5       |      6      |    7   |    0   |    1   |    2   |    3   |    4   |    5   |    6   |    7   |
    /// </param>
    /// <returns>
    ///  0: OK (no errors)
    /// -1: error occurred
    /// -2: no server
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_GetVoicemeeterType([Out] out VoicemeeterType pType);

    /// <summary>
    /// Get the Voicemeeter version
    /// </summary>
    /// <param name="pVersion">
    /// A pointer which represents the version: v1.v2.v3.v4
    /// v1 = (version & 0xFF000000) >> 24;
    /// v2 = (version & 0x00FF0000) >> 16;
    /// v3 = (version & 0x0000FF00) >> 8;
    /// v4 = (version & 0x000000FF);
    /// </param>
    /// <returns>
    ///  0: OK (no errors)
    /// -1: error occurred
    /// -2: no server
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_GetVoicemeeterVersion([Out] out int pVersion);

    /// <summary>
    /// Check if any parameter has changed
    /// </summary>
    /// <returns>
    ///  0: no parameter changes
    ///  1: new parameters -> update your display
    /// -1: error occurred
    /// -2: no server
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_IsParametersDirty();

    /// <summary>
    /// Get a parameters value
    /// </summary>
    /// <param name="paramName">Name of the parameter to retrieve the value for</param>
    /// <param name="pValue">The value for the parameter</param>
    /// <returns>
    ///  0: OK (no error)
    /// -1: error occurred
    /// -2: no server
    /// -3: unknown parameter
    /// -5: structure mismatch
    /// </returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_GetParameterFloat(byte[] paramName, [Out] out float pValue);

    /// <summary>
    /// Get parameter value
    /// </summary>
    /// <param name="paraName">Name of the parameter</param>
    /// <param name="szString">
    ///  0: OK
    /// -1: error occurred
    /// -2: no server
    /// -3: unknown parameter
    /// -5: structure mismatch
    /// </param>
    /// <returns></returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_GetParameterStringA(byte[] paramName, [Out] byte[] szString);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramName"></param>
    /// <param name="pValue"></param>
    /// <returns></returns>
    [DllImport("VoicemeeterRemote.dll", CharSet = CharSet.Auto)]
    public static extern int VBVMR_SetParameterFloat(byte[] paramName, [In] float pValue);
    #endregion

    #region VoicemeeterRemote64.dll
    /// <summary>
    /// Open communication pipe with Voicemeeter
    /// </summary>
    /// <returns>
    ///  0: OK
    ///  1: OK but Voicemeeter Application is not running
    /// -1: cannot get client
    /// -2: unexpected login (logout was not called before a subsequent login)
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_Login")]
    public static extern int VBVMR64_Login();

    /// <summary>
    /// Close communication pipe with Voicemeeter
    /// </summary>
    /// <returns>
    ///  0: OK
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_Logout")]
    public static extern int VBVMR64_Logout();

    /// <summary>
    /// Run Voicemeeter Application (automatically retrieves the installation location and runs voicemeeter)
    /// </summary>
    /// <returns>
    ///  0: OK
    /// -1: not installed
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_RunVoicemeeter")]
    public static extern int VBVMR64_RunVoicemeeter();

    /// <summary>
    /// Get the Voicemeeter type
    /// </summary>
    /// <param name="pType">
    /// A pointer which represents the type of Voicemeeter client
    /// 1: Voicemeeter (original)
    /// 2: Voicemeeter Banana
    /// 3: Voicemeeter Potato
    /// 
    /// VOICEMEETER STRIP/BUS INDEX ASSIGNMENT
    /// | STRIP 1 | STRIP 2 | VIRTUAL INPUT | BUS A | BUS B |
    /// |+-------+|+-------+|+-------------+|+-----+|+-----+|
    /// |    0    |    1    |       2       |   0   |   1   |
    /// 
    /// VOICEMEETER BANANA STRIP/BUS INDEX ASSIGNMENT
    /// | STRIP 1 | STRIP 2 | STRIP 3 | VIRTUAL INPUT | VIRTUAL AUX | BUS A1 | BUS A2 | BUS A3 | BUS B1 | BUS B2 |
    /// |+-------+|+-------+|+-------+|+-------------+|+-----------+|+------+|+------+|+------+|+------+|+------+|
    /// |    0    |    1    |    2    |       3       |      4      |    0   |    1   |    2   |    3   |    4   |
    /// 
    /// VOICEMEETER POTATO STRIP/BUS INDEX ASSIGNMENT
    /// | STRIP 1 | STRIP 2 | STRIP 3 | STRIP 4 | STRIP 5 | VIRTUAL INPUT | VIRTUAL AUX | VAIO 3 | BUS A1 | BUS A2 | BUS A3 | BUS A4 | BUS A5 | BUS B1 | BUS B2 | BUS B3 |
    /// |+-------+|+-------+|+-------+|+-------+|+-------+|+-------------+|+-----------+|+------+|+------+|+------+|+------+|+------+|+------+|+------+|+------+|+------+|
    /// |    0    |    1    |    2    |    3    |    4    |       5       |      6      |    7   |    0   |    1   |    2   |    3   |    4   |    5   |    6   |    7   |
    /// </param>
    /// <returns>
    ///  0: OK (no errors)
    /// -1: error occurred
    /// -2: no server
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_GetVoicemeeterType")]
    public static extern int VBVMR64_GetVoicemeeterType([Out] out VoicemeeterType pType);

    /// <summary>
    /// Get the Voicemeeter version
    /// </summary>
    /// <param name="pVersion">
    /// A pointer which represents the version: v1.v2.v3.v4
    /// v1 = (version & 0xFF000000) >> 24;
    /// v2 = (version & 0x00FF0000) >> 16;
    /// v3 = (version & 0x0000FF00) >> 8;
    /// v4 = (version & 0x000000FF);
    /// </param>
    /// <returns>
    ///  0: OK (no errors)
    /// -1: error occurred
    /// -2: no server
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_GetVoicemeeterVersion")]
    public static extern int VBVMR64_GetVoicemeeterVersion([Out] out int pVersion);

    /// <summary>
    /// Check if any parameter has changed
    /// </summary>
    /// <returns>
    ///  0: no parameter changes
    ///  1: new parameters -> update your display
    /// -1: error occurred
    /// -2: no server
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_IsParametersDirty")]
    public static extern int VBVMR64_IsParametersDirty();

    /// <summary>
    /// Get a parameters value
    /// </summary>
    /// <param name="paramName">Name of the parameter to retrieve the value for</param>
    /// <param name="pValue">The value for the parameter</param>
    /// <returns>
    ///  0: OK (no error)
    /// -1: error occurred
    /// -2: no server
    /// -3: unknown parameter
    /// -5: structure mismatch
    /// </returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_GetParameterFloat")]
    public static extern int VBVMR64_GetParameterFloat(byte[] paramName, [Out] out float pValue);

    /// <summary>
    /// Get parameter value
    /// </summary>
    /// <param name="paraName">Name of the parameter</param>
    /// <param name="szString">
    ///  0: OK
    /// -1: error occurred
    /// -2: no server
    /// -3: unknown parameter
    /// -5: structure mismatch
    /// </param>
    /// <returns></returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_GetParameterStringA")]
    public static extern int VBVMR64_GetParameterStringA(byte[] paramName, [Out] byte[] szString);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramName"></param>
    /// <param name="pValue"></param>
    /// <returns></returns>
    [DllImport("VoicemeeterRemote64.dll", CharSet = CharSet.Auto, EntryPoint = "VBVMR_SetParameterFloat")]
    public static extern int VBVMR64_SetParameterFloat(byte[] paramName, [In] float pValue);
    #endregion
  }
}
