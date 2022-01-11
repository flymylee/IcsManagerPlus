using System;
using System.Net.NetworkInformation;
using IcsmLibrary;
using NETCONLib;

namespace IcsmGUI
{
  internal class ConnectionItem
  {
    public NetworkInterface Nic;

    public INetConnection Connection
    {
      get
      {
        return IcsmPlus.GetConnectionById(Nic.Id);
      }
    }

    public ConnectionItem(NetworkInterface nic)
    {
      Nic = nic;
    }

    override public String ToString()
    {
      return Nic.Name;
    }
  }
}
