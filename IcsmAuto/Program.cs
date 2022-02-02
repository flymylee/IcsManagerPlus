using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using IcsmLibrary;

namespace IcsmAuto
{
  class Program
  {
    static void Info()
    {
      foreach (var nic in IcsmPlus.GetAllIPv4Interfaces())
      {
        Console.WriteLine("Name .......... : {0}", nic.Name);
        Console.WriteLine("GUID .......... : {0}", nic.Id);
        Console.WriteLine("Status ........ : {0}", nic.OperationalStatus);
        Console.WriteLine("InterfaceType . : {0}", nic.NetworkInterfaceType);

        if (nic.OperationalStatus == OperationalStatus.Up)
        {
          var ipprops = nic.GetIPProperties();
          foreach (var a in ipprops.UnicastAddresses)
          {
            if (a.Address.AddressFamily == AddressFamily.InterNetwork)
              Console.WriteLine(
                  "Unicast address : {0}/{1}", a.Address, a.IPv4Mask);
          }
          foreach (var a in ipprops.GatewayAddresses)
          {
            Console.WriteLine(
                "Gateway ....... : {0}", a.Address);
          }
        }
        try
        {
          var connection = IcsmPlus.GetConnectionById(nic.Id);
          if (connection != null)
          {
            var props = IcsmPlus.GetProperties(connection);
            Console.WriteLine(
                "Device ........ : {0}", props.DeviceName);
            var sc = IcsmPlus.GetConfiguration(connection);
            if (sc.SharingEnabled)
              Console.WriteLine(
                  "SharingType ... : {0}", sc.SharingConnectionType);
          }
        }
        catch (UnauthorizedAccessException)
        {
          Console.WriteLine("Please run this program with Admin rights to see all properties");
        }
        catch (NotImplementedException e)
        {
          Console.WriteLine("This feature is not supported on your operating system.");
          Console.WriteLine(e.StackTrace);
        }
        Console.WriteLine();
      }
    }

    static void EnableICS(string shared, string home, bool force)
    {
      var connectionToShare = IcsmPlus.FindConnectionByIdOrName(shared);
      if (connectionToShare == null)
      {
        Console.WriteLine("Connection not found: {0}", shared);
        return;
      }
      var homeConnection = IcsmPlus.FindConnectionByIdOrName(home);
      if (homeConnection == null)
      {
        Console.WriteLine("Connection not found: {0}", home);
        return;
      }
/*
      var currentShare = IcsmPlus.GetCurrentlySharedConnections();
      if (currentShare.Exists)
      {
        Console.WriteLine("Internet Connection Sharing is already enabled:");
        Console.WriteLine(currentShare);
        if (!force)
        {
          Console.WriteLine("Please disable it if you want to configure sharing for other connections");
          return;
        }
        Console.WriteLine("Sharing will be disabled first.");
      }
*/
      IcsmPlus.ShareConnection(null, null);
      IcsmPlus.ShareConnection(connectionToShare, homeConnection);
    }

    static void DisableICS()
    {
      var currentShare = IcsmPlus.GetCurrentlySharedConnections();
      if (!currentShare.Exists)
      {
        Console.WriteLine("Internet Connection Sharing is already disabled");
        return;
      }
      Console.WriteLine("Internet Connection Sharing will be disabled:");
      Console.WriteLine(currentShare);
      IcsmPlus.ShareConnection(null, null);
    }

    static void SetDHCP(string connection)
    {
      // Try to find NetworkInterface by name
      var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(network => network.Name == connection);
      var networkInterfaceName = connection;

      // If not found then find NetworkInterface by id
      if (networkInterface == null)
      {
        networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(network => network.Id == connection);
        networkInterfaceName = networkInterface.Name;
      }

      // If still cannot found by name nor id, print error message and terminate this program.
      if (networkInterface == null)
      {
        Console.WriteLine("Connection not found: {0}", connection);
        return;
      }

      //var networkInterfaceGuid = networkInterface.;
      var ipProperties = networkInterface.GetIPProperties();
      var isDHCPenabled = ipProperties.GetIPv4Properties().IsDhcpEnabled;

      if (isDHCPenabled)
        return;    // no change necessary

      var process = new Process
      {
        StartInfo = new ProcessStartInfo("netsh", $"interface ip set address \"{networkInterfaceName}\" dhcp")
      };
      process.Start();

      // var successful = process.ExitCode == 0;
      process.Dispose();

      return;
    }

    static void Usage()
    {
      var appName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
      appName = appName == null ? "" : appName.ToLower();
      Console.WriteLine("Usage: ");
      Console.WriteLine("  {0} info", appName);
      Console.WriteLine("  {0} enable {{GUID-OF-CONNECTION-TO-SHARE}} {{GUID-OF-HOME-CONNECTION}} [force]", appName);
      Console.WriteLine("  {0} enable \"Name of connection to share\" \"Name of home connection\" [force]", appName);
      Console.WriteLine("  {0} disable", appName);
      Console.WriteLine("  {0} dhcp \"Name of connection to enable DHCP\"", appName);
      Console.WriteLine("  {0} dhcp {{GUID-OF-CONNECTION-TO-ENABLE-DHCP}}", appName);
    }

    static void Main(string[] args)
    {
      try
      {
        if (args.Length == 0)
        {
          Info();
          Usage();
          return;
        }

        var command = args[0];

        if (command == "info")
        {
          Info();
        }
        else if (command == "enable")
        {
          var force = false;
          if ((args.Length == 4) && (args[3] == "force"))
          {
            force = true;
          }
          try
          {
            var shared = args[1];
            var home = args[2];
            EnableICS(shared, home, force);
            SetDHCP(home);
          }
          catch (IndexOutOfRangeException)
          {
            Usage();
          }
          catch (UnauthorizedAccessException)
          {
            Console.WriteLine("This operation requires elevation.");
          }

        }
        else if (command == "disable")
        {
          try
          {
            DisableICS();
          }
          catch (UnauthorizedAccessException)
          {
            Console.WriteLine("This operation requires elevation.");
          }
        }
        else if (command == "dhcp")
        {
          try
          {
            var connection = args[1];

            bool IsElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (IsElevated)
              SetDHCP(connection);
            else
              Console.WriteLine("This operation requires elevation.");
          }
          catch (IndexOutOfRangeException)
          {
            Usage();
          }
        }
      }
      catch (NotImplementedException)
      {
        Console.WriteLine("This program is not supported on your operating system.");
      }
      finally
      {
#if (DEBUG)
        Console.ReadKey();
#endif
      }
    }
  }
}
