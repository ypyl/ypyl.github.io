---
layout: post
title: "PowerShell How to"
date: 2011-12-09

tags: powershell
categories: administration
---
0. How to delete all bin, obj folders?

```powershell
Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
```

1. How to set folder's permission?

```powershell
 #set owner and principals for %SystemRoot%\TEMP
 #http://channel9.msdn.com/Forums/Coffeehouse/Powershell-subinacl-ownership-of-directories
 Write-Host -ForegroundColor green "Set owner and principals for %SystemRoot%\TEMP"
 $pathToSystemRoot = get-content env:systemroot
 $currentUser = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
 $Principal = new-object security.principal.ntaccount $currentUser
 $path = Join-Path $pathToSystemRoot \temp
 
 $code = @"
using System;
using System.Runtime.InteropServices;

namespace WSG.Utils
{
public class PermissionsSetter
{

  [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
  internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
  ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);


  [DllImport("kernel32.dll", ExactSpelling = true)]
  internal static extern IntPtr GetCurrentProcess();


  [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
  internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
  phtok);


  [DllImport("advapi32.dll", SetLastError = true)]
  internal static extern bool LookupPrivilegeValue(string host, string name,
  ref long pluid);


  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct TokPriv1Luid
  {
   public int Count;
   public long Luid;
   public int Attr;
  }


  internal const int SE_PRIVILEGE_DISABLED = 0x00000000;
  internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
  internal const int TOKEN_QUERY = 0x00000008;
  internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;


  public const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
  public const string SE_AUDIT_NAME = "SeAuditPrivilege";
  public const string SE_BACKUP_NAME = "SeBackupPrivilege";
  public const string SE_CHANGE_NOTIFY_NAME = "SeChangeNotifyPrivilege";
  public const string SE_CREATE_GLOBAL_NAME = "SeCreateGlobalPrivilege";
  public const string SE_CREATE_PAGEFILE_NAME = "SeCreatePagefilePrivilege";
  public const string SE_CREATE_PERMANENT_NAME = "SeCreatePermanentPrivilege";
  public const string SE_CREATE_SYMBOLIC_LINK_NAME = "SeCreateSymbolicLinkPrivilege";
  public const string SE_CREATE_TOKEN_NAME = "SeCreateTokenPrivilege";
  public const string SE_DEBUG_NAME = "SeDebugPrivilege";
  public const string SE_ENABLE_DELEGATION_NAME = "SeEnableDelegationPrivilege";
  public const string SE_IMPERSONATE_NAME = "SeImpersonatePrivilege";
  public const string SE_INC_BASE_PRIORITY_NAME = "SeIncreaseBasePriorityPrivilege";
  public const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
  public const string SE_INC_WORKING_SET_NAME = "SeIncreaseWorkingSetPrivilege";
  public const string SE_LOAD_DRIVER_NAME = "SeLoadDriverPrivilege";
  public const string SE_LOCK_MEMORY_NAME = "SeLockMemoryPrivilege";
  public const string SE_MACHINE_ACCOUNT_NAME = "SeMachineAccountPrivilege";
  public const string SE_MANAGE_VOLUME_NAME = "SeManageVolumePrivilege";
  public const string SE_PROF_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
  public const string SE_RELABEL_NAME = "SeRelabelPrivilege";
  public const string SE_REMOTE_SHUTDOWN_NAME = "SeRemoteShutdownPrivilege";
  public const string SE_RESTORE_NAME = "SeRestorePrivilege";
  public const string SE_SECURITY_NAME = "SeSecurityPrivilege";
  public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
  public const string SE_SYNC_AGENT_NAME = "SeSyncAgentPrivilege";
  public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
  public const string SE_SYSTEM_PROFILE_NAME = "SeSystemProfilePrivilege";
  public const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";
  public const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
  public const string SE_TCB_NAME = "SeTcbPrivilege";
  public const string SE_TIME_ZONE_NAME = "SeTimeZonePrivilege";
  public const string SE_TRUSTED_CREDMAN_ACCESS_NAME = "SeTrustedCredManAccessPrivilege";
  public const string SE_UNDOCK_NAME = "SeUndockPrivilege";
  public const string SE_UNSOLICITED_INPUT_NAME = "SeUnsolicitedInputPrivilege";       


  public static bool AddPrivilege(string privilege)
  {
   try
   {
    bool retVal;
    TokPriv1Luid tp;
    IntPtr hproc = GetCurrentProcess();
    IntPtr htok = IntPtr.Zero;
    retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
    tp.Count = 1;
    tp.Luid = 0;
    tp.Attr = SE_PRIVILEGE_ENABLED;
    retVal = LookupPrivilegeValue(null, privilege, ref tp.Luid);
    retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
    return retVal;
   }
   catch (Exception ex)
   {
    throw ex;
   }


  }
  public static bool RemovePrivilege(string privilege)
  {
   try
   {
    bool retVal;
    TokPriv1Luid tp;
    IntPtr hproc = GetCurrentProcess();
    IntPtr htok = IntPtr.Zero;
    retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
    tp.Count = 1;
    tp.Luid = 0;
    tp.Attr = SE_PRIVILEGE_DISABLED;
    retVal = LookupPrivilegeValue(null, privilege, ref tp.Luid);
    retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
    return retVal;
   }
   catch (Exception ex)
   {
    throw ex;
   }


  }
}
}
"@
 
 add-type $code
 
 $acl = Get-Acl $Path
 $acl.psbase.SetOwner($principal)
 $Ar = New-Object  system.security.accesscontrol.filesystemaccessrule("IIS_IUSRS","FullControl", "Allow")

 ## Check if Access already exists.
 #see http://cyrusbuilt.net/wordpress/?p=158
 if ($acl.Access | Where { $_.IdentityReference -eq $Principal}) {
  $accessModification = New-Object System.Security.AccessControl.AccessControlModification
  $accessModification.value__ = 2
  $modification = $false
  $acl.ModifyAccessRule($accessModification, $Ar, [ref]$modification) | Out-Null
 } else {
  $acl.AddAccessRule($Ar)
 }

 [void][WSG.Utils.PermissionsSetter]::AddPrivilege([WSG.Utils.PermissionsSetter]::SE_RESTORE_NAME)
 set-acl -Path $Path -AclObject $acl
 [void][WSG.Utils.PermissionsSetter]::RemovePrivilege([WSG.Utils.PermissionsSetter]::SE_RESTORE_NAME)
```

2. How to register Asp.Net & WCF in IIS?

```powershell
 $pathToFramework = "$env:windir\Microsoft.NET\Framework"
 if (test-path "$env:windir\Microsoft.NET\Framework64")
 {
  $pathToFramework = "$env:windir\Microsoft.NET\Framework64"
 }
 
 #start aspnet_regiis and ServiceModelReg
 $aspNet2 = Test-Path "$pathToFramework\v2.0.50727\aspnet_regiis.exe" -pathType leaf
 if (($aspNet2 -eq $true) -and ($aspNet2Reg -eq $false))
 {
  Write-Host -ForegroundColor green "`r`nInstall aspnet_regiis.exe v2.0.50727"
  & "$pathToFramework\v2.0.50727\aspnet_regiis.exe" -i -enable
 }

 $ServModReg3 = Test-Path "$pathToFramework\v3.0\Windows Communication Foundation\ServiceModelReg.exe" -pathType leaf
 if ($ServModReg3 -eq $true)
 {
  Write-Host -ForegroundColor green "`r`nInstall ServiceModelReg.exe v3.0"
  & "$pathToFramework\v3.0\Windows Communication Foundation\ServiceModelReg.exe" -iru
 }
  
 $ServModReg4 = Test-Path "$pathToFramework\v4.0.30319\ServiceModelReg.exe" -pathType leaf
 if ($ServModReg4 -eq $true)
 {
  Write-Host -ForegroundColor green "`r`nInstall ServiceModelReg.exe v4.0.30319"
  & "$pathToFramework\v4.0.30319\ServiceModelReg.exe" -ia -q -nologo
 }

 $AspNetRegIis4 = Test-Path "$pathToFramework\v4.0.30319\aspnet_regiis.exe" -pathType leaf
 if (($AspNetRegIis4 -eq $true) -and ($aspNet4Reg -eq $false))
 {
  Write-Host -ForegroundColor green "`r`nInstall aspnet_regiis.exe v4.0.30319"
  & "$pathToFramework\v4.0.30319\aspnet_regiis.exe" -ir -enable
 }
```

3. How to enable windows features?

```powershell
 #check the windows features
 $features = @(("IIS-ASPNET", "unknown"), ("IIS-HttpCompressionDynamic", "unknown"), ("IIS-ManagementScriptingTools", "unknown"), ("IIS-IIS6ManagementCompatibility", "unknown"), ("IIS-Metabase", "unknown"), ("IIS-WMICompatibility", "unknown"), ("IIS-LegacyScripts", "unknown"), ("IIS-LegacySnapIn", "unknown"))
 
 $dismPath = "$env:windir\System32\Dism.exe"
 if(test-path "$env:windir\Sysnative\Dism.exe")
 {
  $dismPath = "$env:windir\Sysnative\Dism.exe"
 }
 
 Write-Host -ForegroundColor green "`r`nGet windows features"
 $res = & "$dismPath" /online /Get-Features
 #take feature's states
 $writeNextStr = $false
 for ($i = 0; $i -lt $res.Count; $i++)
 {
  $str = $res[$i]
  foreach ($feature in $features)
  {
   if ($str.Contains($feature[0]))
   {
    $feature[1] = $res[$i+1]
    break
   }
  }
 }
 
 #show results
 Write-Host -ForegroundColor green "`r`nPlease see the states of features`r`n"
 foreach($feature in $features)
 {
  Write-Host -ForegroundColor yellow "$feature"
 }
 Write-Host -ForegroundColor green "`r`n"
 
 #enable features
 $needToRestart = $false
 Write-Host -ForegroundColor green "Started to enable all features`r`n"
 foreach($feature in $features)
 {
  if ($feature[1] -ne "State : Enabled")
  {
   $needToRestart  = $true
   $temp = $feature[0]
   Write-Host -ForegroundColor green "Try to enable $temp"
   & "$dismPath" /online /Enable-Feature /FeatureName:$temp /NoRestart
  }
 }
```

4. How to avoid exception "The OS handle’s position is not what FileStream expected"?

```powershell
#this code is for exception such as The OS handle’s position is not what FileStream expected
#see http://www.leeholmes.com/blog/2008/07/30/workaround-the-os-handles-position-is-not-what-filestream-expected/
$bindingFlags = [Reflection.BindingFlags] “Instance,NonPublic,GetField”
$objectRef = $host.GetType().GetField(“externalHostRef”, $bindingFlags).GetValue($host)
$bindingFlags = [Reflection.BindingFlags] “Instance,NonPublic,GetProperty”
$consoleHost = $objectRef.GetType().GetProperty(“Value”, $bindingFlags).GetValue($objectRef, @())
[void] $consoleHost.GetType().GetProperty(“IsStandardOutputRedirected”, $bindingFlags).GetValue($consoleHost, @())
$bindingFlags = [Reflection.BindingFlags] “Instance,NonPublic,GetField”
$field = $consoleHost.GetType().GetField(“standardOutputWriter”, $bindingFlags)
$field.SetValue($consoleHost, [Console]::Out)
$field2 = $consoleHost.GetType().GetField(“standardErrorWriter”, $bindingFlags)
$field2.SetValue($consoleHost, [Console]::Out)
```

5. How to load module?

```powershell
Import-Module WebAdministration
```

6. How to load another script file?

```powershell
#load external functions
. (Join-Path $curFolder \Functions\DevSetupFunctions.ps1)
```