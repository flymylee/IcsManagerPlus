ICS 매니저+
===========

윈도우 10에서 명령어로 인터넷 공유(ICS)를 켜고 끌 수 있는 간단한 CLI 프로그램입니다.

Acknowledgement
---------------

* 이 프로젝트는 [**utapyngo**](https://github.com/utapyngo/) 님의 [**IcsManager**](https://github.com/utapyngo/icsmanager)를 포킹하였습니다.

* 이 프로젝트는 [**IcsManager**](https://github.com/utapyngo/icsmanager)와 마찬가지로 [**GPL-3.0**](./gpl-3.0.txt) 라이센스를 적용하였습니다.

* 아래 요구사항 중 `System.Management.Automation`는 직접 다운받고 `Visual Studio`에서 설치해야 합니다.

Goals
-----

이 프로젝트의 목표는 다음과 같습니다.

1. **닷넷 알못도 쉽게 빌딩하기!** 참고로 저는 `.NET`이나 `C#`에 대해 아무 것도 모릅니다.

1. **버그 수정**. 공유받을 연결의 dhcp 설정이 풀리고, 이상한 사설 IP가 설정되는 문제가 있습니다.

Requirements
------------

* `Windows 10` 이상.
* `Visual Studio Community 2019`.
* `.NET Framework 4.8`.
* `System.Management.Automation` [(NuGet 패키지)](https://www.nuget.org/packages/System.Management.Automation).

  * `Visual Studio` 에서 **도구(T)** > **NuGet 패키지 관리자(N)** > **패키지 관리자 콘솔(D)**을 선택하고 다음 명령어를 입력합니다. (콘솔 열기 단축키: **ctrl+`**)

    ```PowerShell
    PM> Install-Package System.Management.Automation -Version 7.2.1
    ```

Building
--------

`build.cmd` 파일을 실행하면 프로그램이 빌드됩니다.

Usage
-----

info 명령어 외에는 모두 관리자 권한이 필요합니다.

---

```Command
icsm info
```

현재 이용 가능한 연결에 대해 다음 정보를 표시합니다.

* `Name` (연결 이름)
* `GUID` (연결 GUID(uuid))
* `Status` (연결 상태)
* `InterfaceType` (장치 종류)
* `Gateway` (게이트웨이 주소)
* `Unicast address` (연결 주소/서브넷 마스크)
* `Device` (장치명)
* `SharingType` (인터넷 공유 상태)

---

```Command
icsm enable {GUID-OF-CONNECTION-TO-SHARE} {GUID-OF-HOME-CONNECTION} [force]
icsm enable "Name of connection to share" "Name of home connection" [force]
```

인터넷 공유를 활성화합니다. `force` 인수를 추가하면 기존 인터넷 공유를 해제한 후 새로 활성화합니다.

---

```Command
icsm disable
```

인터넷 공유 해제.

---

PowerShell 모듈 사용방법
------------------------

1. `PowerShell` 모듈 추가:

    ```PowerShell
    Import-Module icsmLibrary.dll
    ```

1. 네트워크 연결(목록) 보기:

    ```PowerShell
    Get-NetworkConnections
    ```

1. 인터넷 공유 활성화:

    ```PowerShell
    Enable-ICS "공유할 연결(인터넷)" "공유받은 연결(내부망)"
    ```

1. 인터넷 공유 해제:

    ```PowerShell
    Disable-ICS
    ```
