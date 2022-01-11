using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Linq;
using NETCONLib;
using IcsmLibrary;

namespace IcsmGUI
{
  public partial class IcsmForm : Form
  {
    public IcsmForm()
    {
      InitializeComponent();
    }

    private void FormSharingManager_Load(object sender, EventArgs e)
    {
      try
      {
        RefreshConnections();
      }
      catch (UnauthorizedAccessException)
      {
        MessageBox.Show("Please restart this program with administrative priviliges.");
        Close();
      }
      catch (NotImplementedException)
      {
        MessageBox.Show("This program is not supported on your operating system.");
        Close();
      }
    }

    private void AddNic(NetworkInterface nic)
    {
      var connItem = new ConnectionItem(nic);
      cbSharedConnection.Items.Add(connItem);
      cbHomeConnection.Items.Add(connItem);
      var netShareConnection = connItem.Connection;
      if (netShareConnection != null)
      {
        var sc = IcsmPlus.GetConfiguration(netShareConnection);
        if (sc.SharingEnabled)
        {
          switch (sc.SharingConnectionType)
          {
            case tagSHARINGCONNECTIONTYPE.ICSSHARINGTYPE_PUBLIC:
              cbSharedConnection.SelectedIndex = cbSharedConnection.Items.Count - 1;
              break;
            case tagSHARINGCONNECTIONTYPE.ICSSHARINGTYPE_PRIVATE:
              cbHomeConnection.SelectedIndex = cbSharedConnection.Items.Count - 1;
              break;
          }
        }
      }
    }

    static void SetDHCP(NetworkInterface networkInterface)
    {
      // find the name of NetworkInterface
      var networkInterfaceName = networkInterface.Name;

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

    private void RefreshConnections()
    {
      cbSharedConnection.Items.Clear();
      cbHomeConnection.Items.Clear();
      foreach (var nic in IcsmPlus.GetAllIPv4Interfaces())
      {
        AddNic(nic);
      }
    }
    
    private void buttonRefresh_Click(object sender, EventArgs e)
    {
      RefreshConnections();
    }

    private void buttonStopSharing_Click(object sender, EventArgs e)
    {
      IcsmPlus.ShareConnection(null, null);
      RefreshConnections();
    }

    private void ButtonApply_Click(object sender, EventArgs e)
    {
      var sharedConnectionItem = cbSharedConnection.SelectedItem as ConnectionItem;
      var homeConnectionItem = cbHomeConnection.SelectedItem as ConnectionItem;
      if ((sharedConnectionItem == null) || (homeConnectionItem == null))
      {
        MessageBox.Show(@"Please select both connections.");
        return;
      }
      if (sharedConnectionItem.Connection == homeConnectionItem.Connection)
      {
        MessageBox.Show(@"Please select different connections.");
        return;
      }
      IcsmPlus.ShareConnection(null, null);
      IcsmPlus.ShareConnection(sharedConnectionItem.Connection, homeConnectionItem.Connection);
      SetDHCP(homeConnectionItem.Nic);
      RefreshConnections();
    }
    private void ButtonClose_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}
