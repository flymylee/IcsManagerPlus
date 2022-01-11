using System.Management.Automation;

namespace IcsmLibrary
{
  [Cmdlet(VerbsLifecycle.Disable, "ICS")]
  public class Disable_ICS : PSCmdlet
  {
    protected override void ProcessRecord()
    {
      IcsmPlus.ShareConnection(null, null);
    }
  }
}
