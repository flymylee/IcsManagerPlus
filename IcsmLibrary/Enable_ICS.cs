using System;
using System.Management.Automation;

namespace IcsmLibrary
{
  [Cmdlet(VerbsLifecycle.Enable, "ICS")]
  public class Enable_ICS : PSCmdlet
  {
    [Parameter(HelpMessage = "Connection to share (name or GUID)",
               Mandatory = true,
               Position = 0)]
    public string Shared_connection;

    [Parameter(HelpMessage = "Home connection (name or GUID)",
               Mandatory = true,
               Position = 1)]
    public string Home_connection;

    [Parameter(HelpMessage = "Force disabling ICS if already enabled",
               Mandatory = false)]
    public bool force = false;

    protected override void ProcessRecord()
    {
      var connectionToShare = IcsmPlus.FindConnectionByIdOrName(Shared_connection);
      if (connectionToShare == null)
      {
        WriteError(new ErrorRecord(new PSArgumentException("Connection not found"), "",
            ErrorCategory.InvalidArgument, Shared_connection));
        return;
      }

      var homeConnection = IcsmPlus.FindConnectionByIdOrName(Home_connection);
      if (homeConnection == null)
      {
        WriteError(new ErrorRecord(new PSArgumentException("Connection not found"),
                                   "",
                                   ErrorCategory.InvalidArgument,
                                   Home_connection));
        return;
      }

      var currentShare = IcsmPlus.GetCurrentlySharedConnections();
      if (currentShare.Exists)
      {
        WriteWarning("Internet Connection Sharing is already enabled: " + currentShare);
        if (!force)
        {
          WriteError(new ErrorRecord(
              new PSInvalidOperationException("Please disable existing ICS if you want to enable it for other connections, or set the force flag to true"),
              "",
              ErrorCategory.InvalidOperation,
              null));
          return;
        }
        Console.WriteLine("Sharing will be disabled first.");
      }

      IcsmPlus.ShareConnection(null, null);  // 인터넷 공유 초기화 무조건 수행 후 재할당
      IcsmPlus.ShareConnection(connectionToShare, homeConnection);
    }
  }
}
