ICS Manager+
============

It is a simple command line tool for turning ICS (Internet Connection Sharing) on or off on Windows 10 and higher.

Acknowledgement
---------------

* This project is forked from [**utapyngo**](https://github.com/utapyngo/)'s [**IcsManager**](https://github.com/utapyngo/icsmanager).

* This project is licensed under [**GPL-3.0**](./gpl-3.0.txt), likewise [its original project](https://github.com/utapyngo/icsmanager).

* You need to download and install `System.Management.Automation` on `Visual Studio`.

Goals
-----

This project has two goals:

* Easify of building this program. It is NOTHING that I honestly know about `.NET` nor `C#`.
* Fixing a bug that mess up dhcp settings of home connection.

Requirements
------------

* `Windows 10` or higher.
* `Visual Studio Community 2019`.
* `.NET Framework 4.8`.
* `System.Management.Automation` [(NuGet Package)](https://www.nuget.org/packages/System.Management.Automation).

  * You can install it in `Visual Studio` by open **Tools** > **NuGet Package Manager** > **Package Manager Console** and type. (Shortcut to Open consle: **ctrl+`** )

    ```PowerShell
    PM> Install-Package System.Management.Automation -Version 7.2.1
    ```

Building
--------

Run `build.cmd`.

Usage
-----

All commands require administrative privileges.

---

```Command
icsm info
```

Display information about currently available connections:

* `Name`
* `GUID`
* `Status`
* `InterfaceType`
* `Gateway`
* `Unicast address`
* `Device`
* `SharingType`

---

```Command
icsm enable {GUID-OF-CONNECTION-TO-SHARE} {GUID-OF-HOME-CONNECTION} [force]
icsm enable "Name of connection to share" "Name of home connection" [force]
```

Enable connection sharing. Use the `force` argument if you want to automatically disable existing connection sharing.

---

```Command
icsm disable
```

Disable connection sharing.

---

PowerShell
----------

1. Import module:

    ```PowerShell
    Import-Module icsmLibrary.dll
    ```

1. List network connections:

    ```PowerShell
    Get-NetworkConnections
    ```

1. Start Internet Connection Sharing:

    ```PowerShell
    Enable-ICS "Connection to share" "Home connection"
    ```

1. Stop Internet Connection Sharing:

    ```PowerShell
    Disable-ICS
    ```
