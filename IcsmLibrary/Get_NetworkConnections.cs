using System.Management.Automation;

namespace IcsmLibrary
{
  [Cmdlet(VerbsCommon.Get, "NetworkConnections")]
  public class Get_NetworkConnections : PSCmdlet
  {
    protected override void ProcessRecord()
    {
      foreach (var nic in IcsmPlus.GetIPv4EthernetAndWirelessInterfaces())
      {
        var connection = IcsmPlus.GetConnectionById(nic.Id);
        var properties = IcsmPlus.GetProperties(connection);
        var configuration = IcsmPlus.GetConfiguration(connection);
        var record = new
        {
          Name = nic.Name,
          GUID = nic.Id,
          MAC = nic.GetPhysicalAddress(),
          Description = nic.Description,
          SharingEnabled = configuration.SharingEnabled,
          NetworkAdapter = nic,
          Configuration = configuration,
          Properties = properties,
        };
        WriteObject(record);
      }
    }
  }
}
