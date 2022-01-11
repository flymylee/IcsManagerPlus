@ECHO OFF
SetLocal

IF "%PROCESSOR_ARCHITECTURE%" EQU "AMD64" (
  FOR /D %%d IN ("%ProgramFiles(x86)%\Microsoft Visual Studio\2019"\*) DO SET "msbuild=%%d\MSBuild\Current\Bin\MSBuild.exe"
) ELSE (
  FOR /D %%d IN ("%ProgramFiles%\Microsoft Visual Studio\2019"\*) DO SET "msbuild=%%d\MSBuild\Current\Bin\MSBuild.exe"
)

IF not EXIST "%msbuild%" (
	ECHO Both `Visual Studio Community` 2019 and `.NET Framework v4.8` are required to build this program.
	PAUSE
	GoTo :EOF
)

@ECHO ON
"%msbuild%" -p:Configuration=Release;Platform=x64;DebugType=None;OutDir=..\Out\

@ECHO OFF
Del .\Out\*.config
FOR /D %%d IN (.\Icsm*) DO (
  Rd /S /Q %%d\bin
  Rd /S /Q %%d\obj
)

EndLocal
