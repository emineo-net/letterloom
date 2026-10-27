

cd C:\Users\steph\source\repos\NetAi.Vacancy\NetAi.LetterLoom
powershell -ExecutionPolicy Bypass -File .\build-inno-setups.ps1


//**************************






cd C:\Users\steph\source\repos\NetAi.Vacancy\NetAi.LetterLoom
/////////////dotnet publish -c Release -r win-x64 --self-contained true -o .\publish
dotnet publish -c Release


im project root:
"C:\Users\steph\source\repos\NetAi.Vacancy\NetAi.LetterLoom\setup.iss"

; ============================================================
;  Inno Setup Script für NetAi.LetterLoom (.NET 10 WPF)
;  Voraussetzung: dotnet publish -c Release -r win-x64
;                 --self-contained true -o .\publish
; ============================================================

#define MyAppName "LetterLoom"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "NetAi"
#define MyAppURL "https://www.netai.example"
#define MyAppExeName "NetAi.LetterLoom.exe"
#define MyPublishDir "publish"

[Setup]
AppId={{7E2B4F1C-9D3A-4B8E-A5F2-1C6D8E3A9B70}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoVersion={#MyAppVersion}
////////////////////////nur wenn vorhanden///////////////////////////////SetupIconFile=app.ico

DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
AllowNoIcons=yes

PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog

OutputDir=Output
OutputBaseFilename={#MyAppName}_Setup_{#MyAppVersion}
UninstallDisplayIcon={app}\{#MyAppExeName}

Compression=lzma2/ultra64
SolidCompression=yes

WizardStyle=modern
WizardSizePercent=120

CloseApplications=yes
RestartApplications=no

ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "de"; MessagesFile: "compiler:Languages\German.isl"
Name: "en"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent


//*******************************

🧙‍♂️ Schritt 2: setup.iss in Inno Setup öffnen
Inno Setup Compiler starten.

Menü: File -> Open... (oder Strg+O).

Navigiere zu:

text
C:\Users\steph\source\repos\NetAi.Vacancy\NetAi.LetterLoom\setup.iss


Dannach starten (F9)


fertige .exe :
"C:\Users\steph\source\repos\NetAi.Vacancy\NetAi.LetterLoom\Output\LetterLoom_Setup_1.0.0.exe"


🎯 Optional danach
Code Signing (verhindert SmartScreen-Warnung) - sag Bescheid, dann richten wir das signtool ein.

Version automatisch aus der .csproj ziehen - statt #define MyAppVersion "1.0.0" manuell zu pflegen.

build-installer.ps1 - ein PowerShell-Skript, das dotnet publish + ISCC.exe in einem Rutsch ausführt.

Portable-Variante - zweite .iss, die nur einen Ordner ohne Installation entpackt.



Einstellung	setup.iss (Install)	setup-portable.iss
AppId	7E2B4F1C-...	B8C5E3D2-... (zwingend anders!)
PrivilegesRequired	admin	lowest
DefaultDirName	{autopf}\LetterLoom	{userdocs}\LetterLoom Portable
Uninstallable	Standard (yes)	no
Registry	Uninstall-Eintrag	keine
Startmenü	ja	nein
OutputBaseFilename	LetterLoom_Setup_1.0.0.exe	LetterLoom Portable_v1.0.0.exe
Warum ist die andere AppId so wichtig? Windows erkennt installierte Programme an der AppId. 
Mit derselben ID würde die Portable-Variante die installierte Version in Apps & Features „überschreiben" bzw.
als Update behandeln. Zwei separate IDs = zwei unabhängige Produkte, die parallel existieren können.



3. Portable-Marker in der App auswerten (falls noch nicht geschehen).



