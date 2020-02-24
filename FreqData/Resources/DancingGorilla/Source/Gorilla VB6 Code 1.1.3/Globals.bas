Attribute VB_Name = "Globals"
Option Explicit
Public ErrStr As String
Public FirstRun As Boolean
'all this to watch the app...
Public Const WAIT_INFINITE = -1&
Public Const SYNCHRONIZE = &H100000
Public Const PROCESS_TERMINATE = &H1

Public Declare Function OpenProcess Lib "kernel32" _
    (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, _
    ByVal dwProcessId As Long) As Long

Public Declare Function WaitForSingleObject Lib "kernel32" _
    (ByVal hHandle As Long, ByVal dwMilliseconds As Long) As Long
'lots declared to watch app.

'all this to kill an app...
Type PROCESSENTRY32
    dwSize As Long
    cntUsage As Long
    th32ProcessID As Long
    th32DefaultHeapID As Long
    th32ModuleID As Long
    cntThreads As Long
    th32ParentProcessID As Long
    pcPriClassBase As Long
    dwFlags As Long
    szexeFile As String * 260
End Type

Declare Function ProcessFirst Lib "kernel32.dll" Alias "Process32First" (ByVal hSnapshot As Long, _
uProcess As PROCESSENTRY32) As Long

Declare Function ProcessNext Lib "kernel32.dll" Alias "Process32Next" (ByVal hSnapshot As Long, _
uProcess As PROCESSENTRY32) As Long

Declare Function CreateToolhelpSnapshot Lib "kernel32.dll" Alias "CreateToolhelp32Snapshot" ( _
ByVal lFlags As Long, lProcessID As Long) As Long

Declare Function TerminateProcess Lib "kernel32.dll" (ByVal ApphProcess As Long, _
ByVal uExitCode As Long) As Long

Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As Long) As Long
'finished declarations to kill app.

'And to change priorityPrivate Const NORMAL_PRIORITY_CLASS = &H20
Public Const IDLE_PRIORITY_CLASS = &H40
Public Const HIGH_PRIORITY_CLASS = &H80
Public Const REALTIME_PRIORITY_CLASS = &H100
Public Declare Function SetPriorityClass& Lib "kernel32" (ByVal hProcess As Long, _
    ByVal dwPriorityClass As Long)
'all done

Public AllList As New List
Public DMExe As String
Public SMDir As String
Public DWIDir As String
Public RunDir As String
Public DefaultCredit As String
Public Defaults As Song
Public VERSION As String
Public DMVersion As String
Public DMVerNum As Single
Public FileVer As String
Public NeedToSave As Boolean
Public LocalDecimal As String

Public Function TestDMPath(Base As String) As Boolean
 Dim File As String
 File = Dir(Base, vbArchive)
 If File <> "" Then
  TestDMPath = True
 Else
  TestDMPath = False
 End If
End Function

Public Function TestSMPath(Base As String) As Boolean
 Dim File As String
 TestSMPath = False
  File = Dir(Base, vbArchive + vbDirectory)
  If File <> "" Then _
   If Dir(Base + "\Program\StepMania.exe", vbArchive) <> "" Then _
    If Dir(Base + "\Songs", vbArchive + vbDirectory) <> "" Then _
     If (GetAttr(Base + "\Songs") And vbDirectory) = vbDirectory Then _
      TestSMPath = True
End Function

Public Function TestDWIPath(Base As String) As Boolean
 Dim File As String
 TestDWIPath = False
 File = Dir(Base, vbArchive + vbDirectory)
 If File <> "" Then _
  If Dir(Base + "\DWI2.exe", vbArchive) <> "" Then _
   If Dir(Base + "\Songs", vbArchive + vbDirectory) <> "" Then _
    If (GetAttr(Base + "\Songs") And vbDirectory) = vbDirectory Then _
     TestDWIPath = True
End Function

Public Sub LoadDefault()
 On Error GoTo LostConfig
 ChDrive Left(RunDir, 1)
 ChDir (RunDir)
 If Dir("Settings.gor", vbArchive) <> "" Then
  Dim Tmp As String
  Dim FH As Long
  FH = FreeFile
  On Error GoTo DodgyFile
  Open "Settings.gor" For Input As FH
  Line Input #FH, Tmp
  FileVer = Left(Tmp, InStrRev(Tmp, "."))
  FileVer = Right(FileVer, Len(FileVer) - InStrRev(FileVer, "v"))
  If Left(Tmp, InStrRev(Tmp, ".")) = Left("Gorilla v" + VERSION, InStrRev("Gorilla v" + VERSION, ".")) Then
   If Right(Tmp, 15) = "- Configuration" Then
    Input #FH, DMExe
    Input #FH, SMDir
    Input #FH, DWIDir
    LoadSong FH, Defaults, True
   ElseIf Right(Tmp, 10) = "- SongList" Then
    MsgBox "File is a Song List, not a configuration file", vbCritical + vbOKOnly, "Gorilla - Error"
    Exit Sub
   Else
    MsgBox "Not a Configuration file!", vbCritical + vbOKOnly, "Gorilla - Error"
    Exit Sub
   End If
  Else
   Close #FH
   If Tmp = "Gorilla v0.9" Then
    Load About
    About.CanExit = "No"
    About.AboutBox = vbCrLf + vbCrLf + "Importing v0.9.x Settings..."
    About.Caption = "Gorilla - Importing"
    About.OK.Visible = False
    About.Height = 1700
    About.Show 0
    About.Refresh
    ImportV09Config
    Unload About
    MsgBox "Your v0.9.x Settings have been updated to v" + VERSION, vbInformation, "Gorilla"
    LoadDefault
   End If
   If FileVer = "0.10." Then
    Load About
    About.CanExit = "No"
    About.AboutBox = vbCrLf + vbCrLf + "Importing v0.10.x Settings..."
    About.Caption = "Gorilla - Importing"
    About.OK.Visible = False
    About.Height = 1700
    About.Show 0
    About.Refresh
    ImportV010Config
    Unload About
    MsgBox "Your v0.10.x Settings have been updated to v" + VERSION, vbInformation, "Gorilla"
    LoadDefault
   End If
   If FileVer = "0.11." Then
    Load About
    About.CanExit = "No"
    About.AboutBox = vbCrLf + vbCrLf + "Importing v0.11.x Settings..."
    About.Caption = "Gorilla - Importing"
    About.OK.Visible = False
    About.Height = 1700
    About.Show 0
    About.Refresh
    ImportV011Config
    Unload About
    MsgBox "Your v0.11.x Settings have been updated to v" + VERSION, vbInformation, "Gorilla"
    LoadDefault
   End If
  End If
  Close #FH
  On Error GoTo 0
 End If
 Exit Sub
LostConfig:
 On Error GoTo 0
 Exit Sub
DodgyFile:
 MsgBox "Failed to load data. File is corrupt!", vbCritical, "Gorilla - Fileload error"
 DMExe = ""
 SMDir = ""
 DWIDir = ""
 Defaults.Name = ""
 On Error GoTo 0
End Sub
  
Public Sub LoadSong(FH As Long, tSong As Song, Optional Force As Boolean = False)
 Dim Tmp As String
 With tSong
  Line Input #FH, Tmp
  .Name = Tmp
  Line Input #FH, Tmp
  .State = Tmp
  Line Input #FH, Tmp
  .IsDir = Tmp
  Line Input #FH, Tmp
  .Default = Tmp
  If .Default And Not Force Then Exit Sub
  Line Input #FH, Tmp
  .OutDir = Tmp
  Line Input #FH, Tmp
  .Delve = Tmp
  Line Input #FH, Tmp
  .CopyArt = Tmp
  Line Input #FH, Tmp
  .Turbo = Tmp
  Line Input #FH, Tmp
  .DirToGroup = Tmp
  Line Input #FH, Tmp
  .Group = Tmp
  Line Input #FH, Tmp
  .NoID = Tmp
  Line Input #FH, Tmp
  .CreditState = Tmp
  Line Input #FH, Tmp
  .Credit = Tmp
  Line Input #FH, Tmp
  .OutputType = Tmp
  Line Input #FH, Tmp
  .NoStops = Tmp
  Line Input #FH, Tmp
  .ErrorStops = Tmp
  Line Input #FH, Tmp
  .Length = Tmp
  Line Input #FH, Tmp
  .Fade = Tmp
  Line Input #FH, Tmp
  .Confidence = Tmp
  Line Input #FH, Tmp
  .BeatPerMeasure = Tmp
  Line Input #FH, Tmp
  .BPM_Min = Tmp
  Line Input #FH, Tmp
  .BPM_Max = Tmp
  Line Input #FH, Tmp
  .GapAdjust = Tmp
  Line Input #FH, Tmp
  .AddToSM = Tmp
  Line Input #FH, Tmp
  .AddToDWI = Tmp
  Line Input #FH, Tmp
  .Basic = Tmp
  Line Input #FH, Tmp
  .Medium = Tmp
  Line Input #FH, Tmp
  .Hard = Tmp
 End With
End Sub

Public Sub SaveSong(ByVal FH As Long, tSong As Song, Optional Force As Boolean = False)
 With tSong
  Print #FH, .Name
  Print #FH, .State
  Print #FH, .IsDir
  Print #FH, .Default
  If .Default And Not Force Then Exit Sub
  Print #FH, .OutDir
  Print #FH, .Delve
  Print #FH, .CopyArt
  Print #FH, .Turbo
  Print #FH, .DirToGroup
  Print #FH, .Group
  Print #FH, .NoID
  Print #FH, .CreditState
  Print #FH, .Credit
  Print #FH, .OutputType
  Print #FH, .NoStops
  Print #FH, .ErrorStops
  Print #FH, .Length
  Print #FH, .Fade
  Print #FH, .Confidence
  Print #FH, .BeatPerMeasure
  Print #FH, .BPM_Min
  Print #FH, .BPM_Max
  Print #FH, .GapAdjust
  Print #FH, .AddToSM
  Print #FH, .AddToDWI
  Print #FH, .Basic
  Print #FH, .Medium
  Print #FH, .Hard

 End With
End Sub

Public Sub InitialDefault()
 With Defaults
  .Name = "<default>"
  .Default = True
  .State = 0
  If DMExe <> "" Then .OutDir = DMDir + "Output\"
  .IsDir = True
  .Delve = True
  .CopyArt = True
  .Turbo = 2
  .DirToGroup = False
  .Group = "My Music"
  .NoID = False
  .CreditState = 0
  .Credit = DefaultCredit
  .OutputType = 1
  .NoStops = False
  .ErrorStops = 3
  .Length = 105
  .Fade = 5
  .Confidence = 10
  .BeatPerMeasure = 4
  .BPM_Min = 89
  .BPM_Max = 205
  .GapAdjust = 0
  .AddToSM = True
  .AddToDWI = True
  .Basic = 3
  .Medium = 5
  .Hard = 7
 End With
End Sub

Sub Initialize()
 On Error GoTo FatalError
 ErrStr = "Initialize; 1"
 LoadDefault
 ErrStr = "Initialize; 2"
 If Defaults.Name <> "<default>" Then InitialDefault
 ErrStr = "Initialize; 3"
 If (DMExe = "" Or Dir(DMExe, vbArchive) = "") _
       Or _
    (SMDir <> "" And Not TestSMPath(SMDir)) _
       Or _
    (DWIDir <> "" And Not TestDWIPath(DWIDir)) _
 Then
  ErrStr = "Initialize; 4"
  Load Config
  ErrStr = "Initialize; 5"
  Config.Show 0
  If (DMExe <> "") Then
   If ((SMDir <> "") Or (DWIDir <> "")) Then
    Dim junk As Integer
    ErrStr = "Initialize; 5.1"
    junk = MsgBox("There is enough information here. You can just exit this page if you don't want to customize Gorilla!", vbOKOnly, "Gorilla")
   End If
  End If
  Config.Hide
  ErrStr = "Initialize; 5.2"
  Config.Hide
  ErrStr = "Initialize; 5.3"
  Config.Show 1
  ErrStr = "Initialize; 6"
  Unload Config
 Else
  ErrStr = "Initialize; 7"
  GetDMVersion
 End If
 ErrStr = "Initialize; 8"
 If Defaults.CreditState = 0 Then Defaults.Credit = DefaultCredit
 Exit Sub
FatalError:
 Load Error
 Error.Detail.Caption = ErrStr
 Error.Show
End Sub

Sub GetDMVersion()
 Dim FH As Long
 Dim File As String
 Dim Length As Long
 Dim Ver As String
 Dim PID As Double
 Dim hProcess As Long
 File = "DancingMonkeys.ver"
 ChDrive Left(DMExe, 1)
 ChDir DMDir + "bin\win32"
 PID = Shell("cmd /c DancingMonkeys.exe -v > " + File, vbHide)
 hProcess = OpenProcess(SYNCHRONIZE, False, PID)
 WaitForSingleObject hProcess, WAIT_INFINITE
 Length = FileLen(File)
 FH = FreeFile
 Open File For Input As FH
 Ver = Input(Length - 2, #FH)
 Close #FH
 ChDrive Left(RunDir, 1)
 ChDir RunDir
 If InStr(Ver, "incorrect option ""-v"".") Then
  DMVersion = "DancingMonkeys pre-1.03"
  DMVerNum = 1
  MsgBox "Dancing Monkeys may be too old." + vbCrLf + _
   "Version expected is v1.xx (v1.03 or later)" + vbCrLf + _
   "Get a new version of DancingMonkeys from http://www.monket.net/dancing-monkeys/", vbCritical
 ElseIf InStr(Ver, "Version: Dancing Monkeys v1") Then
  DMVersion = Right(Ver, Len(Ver) - InStr(Ver, ": ") - 1)
  DMVerNum = CSng(Replace(Right(Ver, Len(Ver) - InStr(Ver, " v") - 1), ".", LocalDecimal))
 Else
  DMVersion = Right(Ver, Len(Ver) - InStr(Ver, ": ") - 1)
  DMVerNum = 0
  MsgBox "Dancing Monkeys may be too New. " + vbCrLf + _
   "Version expected is v1.xx (v1.03 or later)" + vbCrLf + _
   "Get a new version of Gorilla from http://www.monket.net/dancing-monkeys/", vbCritical
 End If
 DefaultCredit = "Gorilla v" + VERSION + " (" + DMVersion + ")"
End Sub

Function DMDir() As String
 On Error GoTo BadPath
 If DMExe <> "" Then
  DMDir = Left(DMExe, InStrRev(DMExe, "\"))
  DMDir = Left(DMDir, InStrRev(DMDir, "\", Len(DMDir) - 2))
  DMDir = Left(DMDir, InStrRev(DMDir, "\", Len(DMDir) - 2))
 Else
  DMDir = ""
 End If
 Exit Function
BadPath:
 DMDir = ""
 On Error GoTo 0
End Function

Sub SongCopy(FromSong As Song, ToSong As Song)
 With ToSong
  .State = FromSong.State
  .OutDir = FromSong.OutDir
  .Delve = FromSong.Delve
  .CopyArt = FromSong.CopyArt
'  .Turbo = FromSong.Turbo
  .DirToGroup = FromSong.DirToGroup
  .Group = FromSong.Group
  .NoID = FromSong.NoID
  .CreditState = FromSong.CreditState
  .Credit = FromSong.Credit
  .OutputType = FromSong.OutputType
  .NoStops = FromSong.NoStops
  .ErrorStops = FromSong.ErrorStops
  .Length = FromSong.Length
  .Fade = FromSong.Fade
  .Confidence = FromSong.Confidence
  .BeatPerMeasure = FromSong.BeatPerMeasure
  .BPM_Min = FromSong.BPM_Min
  .BPM_Max = FromSong.BPM_Max
  .GapAdjust = FromSong.GapAdjust
  .AddToSM = FromSong.AddToSM
  .AddToDWI = FromSong.AddToDWI
  .Basic = FromSong.Basic
  .Medium = FromSong.Medium
  .Hard = FromSong.Hard
 End With
End Sub

Function GetDMLog(FindSong As String, LogPath As String, Optional Start As Long = 0) As String
 Dim FH As Integer
 Dim lPath, Line, OneLog As String
 Dim Flag As Boolean
 GetDMLog = ""
 lPath = Left(LogPath, InStrRev(LogPath, "\") - 1)
 Flag = False
 FH = FreeFile
 Open lPath + "\dm_log.txt" For Input As FH
 OneLog = Input(Start, #FH)
 OneLog = ""
 Do While Not EOF(FH)
  Line Input #FH, Line
  If Line = "===============================================================================" Then
   If Flag Then GetDMLog = OneLog
   OneLog = ""
  End If
  OneLog = OneLog + CStr(Line) + vbCrLf
  If Left(Line, 28) = "Begin processing input file " Then Flag = False
  If Right(Line, Len(FindSong)) = FindSong Then Flag = True
 Loop
 Close #FH
 If Flag Then GetDMLog = OneLog
End Function

Public Function CanTurbo(Name As String) As String
 Dim File As String
 Dim Tmp As String
 On Error GoTo BadFile
 'No StepFiles for directories
 If Right(Name, 1) = "\" Or (GetAttr(Name) And vbDirectory) Then
  CanTurbo = -1
  Exit Function
 End If
 'look for StepFile in source directory
 File = Left(Name, InStrRev(Name, ".") - 1)
 If Dir(File + ".sm", vbArchive) <> "" Then
  File = File + ".sm"
 ElseIf Dir(File + ".dwi", vbArchive) <> "" Then
  File = File + ".dwi"
 Else
 'look for StepFile in target directory
  If AllList.SongName = Name Then
   Dim TestSong As Song
   Dim P2 As String
   If AllList.Default Then
    Set TestSong = Defaults
   Else
    Set TestSong = AllList.cSong
   End If
   With TestSong
    File = Left(.OutDir, Len(.OutDir) - 1)
    If .AddToSM Then 'Add to StepMania
     File = SMDir + "\Songs"
    ElseIf .AddToDWI Then 'No StepMania, Add to DWI
     File = DWIDir + "\Songs"
    End If
    If .AddToSM Or .AddToDWI Then
     If Dir(File, vbArchive + vbDirectory) = "" Then
      'no Target Directory; no StepFiles.
      CanTurbo = -1
      Exit Function
     End If
     If .DirToGroup Then 'StepMania/DWI Dir to Group
      P2 = Left(Name, InStrRev(Name, "\") - 1)
      P2 = Right(P2, Len(P2) - InStrRev(P2, "\"))
      File = File + "\" + P2
      If Dir(File, vbArchive + vbDirectory) = "" Then
       'no Target Directory; no StepFiles.
       CanTurbo = -1
       Exit Function
      End If
     Else 'Stepmania/DWI all in "my group"
      File = File + "\" + .Group
      If Dir(File, vbArchive + vbDirectory) = "" Then
       'no Target Directory; no StepFiles.
       CanTurbo = -1
       Exit Function
      End If
     End If
    End If
   End With
   P2 = Right(Name, Len(Name) - InStrRev(Name, "\"))
   P2 = Left(P2, InStrRev(P2, ".") - 1)
   File = File + "\" + P2
   If Dir(File, vbArchive + vbDirectory) = "" Then
    'no Target Directory; no StepFiles.
    CanTurbo = -1
    Exit Function
   End If
   File = File + "\" + P2
   If Dir(File + ".sm", vbArchive) <> "" Then
    File = File + ".sm"
   ElseIf Dir(File + ".dwi", vbArchive) <> "" Then
    File = File + ".dwi"
   Else
    'No target Stepfile??
    CanTurbo = -1
    Exit Function
   End If
  Else
   'Don't know where to look for StepFile; assume there is none
   CanTurbo = -1
   Exit Function
  End If
 End If
 
 'We have a Stepfile! Yay!
 Dim FH As Long
 FH = FreeFile
 On Error GoTo BadFile
 Open File For Input As FH
 Do While Not EOF(FH)
  Input #FH, Tmp
  If Left(Tmp, 5) = "#BPM:" Then 'DWI got BPM
   Tmp = Left(Tmp, Len(Tmp) - 1)
   CanTurbo = Right(Tmp, Len(Tmp) - 5)
   Close FH
   Exit Function
  End If
  If Left(Tmp, 6) = "#BPMS:" Then 'SM got BPM
   Tmp = Left(Tmp, Len(Tmp) - 1)
   CanTurbo = Right(Tmp, Len(Tmp) - InStrRev(Tmp, "="))
   Close FH
   Exit Function
  End If
 Loop
 Close FH
 CanTurbo = -1
 Exit Function
BadFile:
 CanTurbo = -1
 On Error GoTo 0
End Function

Public Function GetCommandLine(Optional MaxArgs = 10) As String()
   Dim C, CmdLine, CmdLnLen, InArg, Quoted, i, NumArgs
   ReDim ArgArray(MaxArgs) As String
   NumArgs = 0: InArg = False: Quoted = False
   CmdLine = Command()
   CmdLnLen = Len(CmdLine)
   For i = 1 To CmdLnLen
      C = Mid(CmdLine, i, 1)
      If (C <> " " And C <> vbTab) Then
         If Not InArg Then
            If NumArgs = MaxArgs Then Exit For
            NumArgs = NumArgs + 1
            InArg = True
            If (C = """") Then
                Quoted = True
                C = ""
            End If
         ElseIf (C = """" And Quoted) Then
            Quoted = False
            C = ""
         End If
         ArgArray(NumArgs) = ArgArray(NumArgs) & C
      Else
         If Not Quoted Then
            InArg = False
         Else
            ArgArray(NumArgs) = ArgArray(NumArgs) & C
         End If
      End If
   Next i
   ReDim Preserve ArgArray(NumArgs)
   GetCommandLine = ArgArray
End Function

Public Sub ExpandDirectory()
  Dim Tmp As Song
  Dim NewFile As String
  Dim OutPath As String
  Set Tmp = AllList.cSong
  Tmp.State = 4
  If Right(Tmp.Name, 1) <> "\" Then Tmp.Name = Tmp.Name + "\"
  If Tmp.Default Then
   OutPath = Left(Defaults.OutDir, Len(Defaults.OutDir) - 1)
   If Defaults.AddToSM Then
    OutPath = SMDir + "\Songs"
   ElseIf Defaults.AddToDWI Then
    OutPath = DWIDir + "\Songs"
   End If
  Else
   OutPath = Left(Tmp.OutDir, Len(Tmp.OutDir) - 1)
   If Tmp.AddToSM Then
    OutPath = SMDir + "\Songs"
   ElseIf Tmp.AddToDWI Then
    OutPath = DWIDir + "\Songs"
   End If
  End If
  If Left(Tmp.Name, Len(OutPath)) = OutPath Then
   Tmp.State = 4
   ExpandDirectory
   Exit Sub
  End If
  On Error GoTo BadFile
  NewFile = Dir(Tmp.Name, vbArchive + vbDirectory)
  Do While (NewFile <> "")
   If (NewFile <> ".") And _
      (NewFile <> "..") And _
      (Tmp.Name + NewFile + "\" <> DMDir) And _
      (Tmp.Name + NewFile + "\" <> SMDir) And _
      (Tmp.Name + NewFile + "\" <> DWIDir) _
   Then
    If (GetAttr(Tmp.Name + NewFile) And vbDirectory) = vbDirectory Then
     'It's a directory
     AllList.InsertAfter (Tmp.Name + NewFile + "\")
     If Tmp.Default Then
      SongCopy Defaults, AllList.cSong
      AllList.Default = Defaults.Default
     Else
      SongCopy Tmp, AllList.cSong
      AllList.Default = Tmp.Default
     End If
     AllList.IsDir = True
     AllList.State = 0
    Else
     'It's a file
     If (LCase(Right(NewFile, 3)) = "wav") Or (LCase(Right(NewFile, 3)) = "mp3") Then
      AllList.InsertAfter (Tmp.Name + NewFile)
      If Tmp.Default Then
       SongCopy Defaults, AllList.cSong
       AllList.Default = Defaults.Default
      Else
       SongCopy Tmp, AllList.cSong
       AllList.Default = Tmp.Default
      End If
      AllList.State = 0
      AllList.IsDir = False
     End If
    End If
   End If
BadFileResume:
   NewFile = Dir
  Loop
  Exit Sub
BadFile:
 Resume BadFileResume
End Sub

