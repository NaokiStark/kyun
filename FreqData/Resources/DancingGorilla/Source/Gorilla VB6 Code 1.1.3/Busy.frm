VERSION 5.00
Begin VB.Form Busy 
   Caption         =   "Gorilla - Busy"
   ClientHeight    =   4935
   ClientLeft      =   165
   ClientTop       =   555
   ClientWidth     =   8310
   Icon            =   "Busy.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   4935
   ScaleWidth      =   8310
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton KillDMBtn 
      Caption         =   "Stop Processing &Now"
      Height          =   255
      Left            =   120
      TabIndex        =   6
      ToolTipText     =   $"Busy.frx":08CA
      Top             =   3000
      Width           =   2055
   End
   Begin VB.Timer WaveTimer 
      Interval        =   100
      Left            =   6480
      Top             =   0
   End
   Begin VB.TextBox Output 
      Appearance      =   0  'Flat
      Height          =   1515
      Left            =   2280
      MultiLine       =   -1  'True
      ScrollBars      =   3  'Both
      TabIndex        =   5
      Top             =   3000
      Width           =   5895
   End
   Begin VB.CommandButton AbortAfter 
      Caption         =   "Stop After Current Song"
      Height          =   255
      Left            =   120
      TabIndex        =   3
      ToolTipText     =   "Wait untill Dancing Monkeys finishes the current song, then return to the Gorilla main page."
      Top             =   3360
      Width           =   2055
   End
   Begin VB.Timer AbortTimer 
      Enabled         =   0   'False
      Interval        =   100
      Left            =   7680
      Top             =   0
   End
   Begin VB.Timer BusyTimer 
      Enabled         =   0   'False
      Interval        =   100
      Left            =   7080
      Top             =   0
   End
   Begin VB.ListBox BusyList 
      BeginProperty Font 
         Name            =   "Lucida Console"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2370
      Left            =   0
      TabIndex        =   1
      Top             =   480
      Width           =   8295
   End
   Begin VB.Label ETA 
      Caption         =   "--:--:--"
      Height          =   255
      Left            =   5280
      TabIndex        =   12
      Top             =   0
      Width           =   855
   End
   Begin VB.Label ETALabel 
      Caption         =   "ETA"
      Height          =   255
      Left            =   4800
      TabIndex        =   11
      Top             =   0
      Width           =   375
   End
   Begin VB.Label AllSong 
      Alignment       =   1  'Right Justify
      Height          =   255
      Left            =   3600
      TabIndex        =   10
      Top             =   240
      Width           =   735
   End
   Begin VB.Label CurrentSong 
      Alignment       =   1  'Right Justify
      Height          =   255
      Left            =   3480
      TabIndex        =   9
      Top             =   0
      Width           =   855
   End
   Begin VB.Label AllSongLabel 
      Caption         =   "All Songs"
      Height          =   255
      Left            =   2400
      TabIndex        =   8
      Top             =   240
      Width           =   1095
   End
   Begin VB.Label CurrentLabel 
      Caption         =   "Current Song"
      Height          =   255
      Left            =   2400
      TabIndex        =   7
      Top             =   0
      Width           =   975
   End
   Begin VB.Label Boared2 
      Height          =   200
      Left            =   120
      TabIndex        =   4
      Top             =   4080
      Width           =   1935
   End
   Begin VB.Label Boared 
      Height          =   200
      Left            =   120
      TabIndex        =   2
      Top             =   3840
      Width           =   1935
   End
   Begin VB.Label BusyLabel 
      Caption         =   "Busy..."
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   13.5
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   720
      TabIndex        =   0
      Top             =   0
      Width           =   855
   End
   Begin VB.Menu VLMenu 
      Caption         =   "View Log..."
      Visible         =   0   'False
      Begin VB.Menu VLog 
         Caption         =   "View Log"
      End
   End
End
Attribute VB_Name = "Busy"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Pattern As String
Private PatternLoop As Integer
Private LogFileLoop As Integer
Private LogFileSize As Integer
Private hProcess As Long
Private DMPID As Double
Private Abort As Boolean
Private LogFile As String
Private ListNum As Integer
Private OutPath As String
Private LogFileLoopMax As Integer
Private DMLogSize As Long
Private StartTime As Date
Private AllTime As Date
Private TrackText(54) As String
Private TrackTime(54) As Integer
Private TrackCount As Integer
Private DropPriority As Boolean

Private Function NextJob() As Integer
 On Error GoTo FatalError
 ErrStr = "NextJob; 1"
 Dim DMCommand As String
 DropPriority = True
 SetTrack
 ETA.Caption = "--:--:--"
 TrackCount = 3
 If Abort Then
  NextJob = 0
  Exit Function
 End If
 ErrStr = "NextJob; 2"
 Do While (Not AllList.cSong Is Nothing)
  If AllList.State = 0 Then Exit Do
  ListNum = ListNum + 1
  AllList.NextSong
 Loop
 ErrStr = "NextJob; 3"
 If ListNum + 1 < BusyList.ListCount Then
  BusyList.ListIndex = ListNum
 Else
  BusyList.ListIndex = -1
 End If
 ErrStr = "NextJob; 4"
 If AllList.cSong Is Nothing Then
  Abort = True
  NextJob = 0
  Exit Function
 End If
 
 LogFileLoop = 0
 LogFileSize = 0
 Output = ""
' We have a directory....
 If AllList.IsDir Then
 ErrStr = "NextJob; 5"
  ExpandDirectory
  BusyList.Clear
 ErrStr = "NextJob; 6"
  AllList.Populate Busy.BusyList, 1
  If ListNum + 1 < BusyList.ListCount Then
   BusyList.ListIndex = ListNum + 1
  Else
   BusyList.ListIndex = -1
  End If
 ErrStr = "NextJob; 7"
  AllList.FirstSong
  ListNum = 0 ' need to find the first entry. sadly it's been lost.
 ErrStr = "NextJob; 8"
  NextJob = NextJob()
  Exit Function
 End If
 
 NextJob = 1 'We Have a song to process!
 
 If AllList.Default Then
 ErrStr = "NextJob; 9"
  DMCommand = BuildCommand(AllList.SongName, Defaults)
 Else
 ErrStr = "NextJob; 10"
  DMCommand = BuildCommand(AllList.SongName, AllList.cSong)
 End If
 If Dir(Left(OutPath, InStrRev(OutPath, "\") - 1) + "\dm_log.txt", vbArchive) <> "" Then
 ErrStr = "NextJob; 11"
  DMLogSize = FileLen(Left(OutPath, InStrRev(OutPath, "\") - 1) + "\dm_log.txt")
 Else
  DMLogSize = 0
 End If
 ErrStr = "NextJob; 12"
 ChDrive Left(DMDir, 1)
 ChDir DMDir + "\bin\win32"
 DMPID = Shell("cmd /c """ + DMCommand + """", vbHide)
 ErrStr = "NextJob; 13"
 hProcess = OpenProcess(SYNCHRONIZE, False, DMPID)
 StartTime = Time
 ErrStr = "NextJob; 14"
 ChDrive Left(RunDir, 1)
 ChDir RunDir
 Exit Function
FatalError:
 Load ERROR
 ERROR.Detail.Caption = ErrStr & Chr(13) & " Error # " & _
 Str(Err.Number) & " generated by " & Err.Source & Chr(13) & _
 Err.Description
 ERROR.Show
End Function

Private Sub AbortAfter_Click()
 If Abort Then
  If MsgBox("Are you sure you want processing to continue?" + vbCrLf + "(The remaining songs will be processed as normal)", vbYesNo + vbDefaultButton2 + vbExclamation, "Gorilla - Don't stop after curent") Then
   Abort = False
   AbortAfter.ToolTipText = "Wait untill Dancing Monkeys finishes the current song, then return to the Gorilla main page."
   AbortAfter.Caption = "Stop After Current Song"
   Pattern = "¸¸.·´¯`·."
   PatternLoop = Len(Pattern)
  End If
 Else
  If MsgBox("Are you sure you want to stop after the current song?" + vbCrLf + "(Remaining songs will be listed in Gorilla's main panel)", vbYesNo + vbDefaultButton2 + vbExclamation, "Gorilla - Stop after current") = vbYes Then
   Abort = True
   AbortAfter.ToolTipText = "The ramianing songs will be processed as normal."
   AbortAfter.Caption = "Don't Stop"
   Pattern = "S t o p p i n g . . .  "
   PatternLoop = Len(Pattern)
  End If
 End If
End Sub

Private Sub AbortTimer_Timer()
 Unload Me
End Sub

Private Sub BusyList_MouseUp(Button As Integer, Shift As Integer, x As Single, y As Single)
 If BusyList.ListIndex + 1 < BusyList.ListCount Then
  If Left(BusyList, 8) <> "[      ]" And Left(BusyList, 8) <> "[------]" Then
   PopupMenu VLMenu
  End If
 End If
End Sub

Private Sub BusyTimer_Timer()
  If DropPriority Then
   DropPriority = Not SetDMPriorityClass
  End If
  'Show DM outout
  If LogFileLoop = 0 Then
   Dim Tmp As String
   Dim FH As Long
   Dim lLoop As Integer
   Tmp = FileLen(LogFile)
   If (Tmp <> LogFileSize) Then
    LogFileLoopMax = 10
    LogFileSize = Tmp
    FH = FreeFile
    Open LogFile For Input As FH
    Tmp = Input(LogFileSize, #FH)
    Close #FH
    Output = Tmp
    'Calculate ETA
    For lLoop = TrackCount To 54
     If InStrRev(Tmp, TrackText(lLoop)) <> 0 Then
      TrackCount = lLoop + 1
      TrackText(lLoop) = "__NULL__"
      ETA.Caption = Format((Time - StartTime) * 3576 / TrackTime(lLoop), "HH:NN:SS")
     End If
    Next
    'End of ETA calculation
    Tmp = InStrRev(Output.Text, vbCrLf)
    If (Tmp = LogFileSize - 1) Then
     Output.SelStart = LogFileSize
    Else
     Output.SelStart = InStrRev(Output.Text, vbCrLf, LogFileSize - 1)
    End If
    Output.SelLength = 0
   Else
    If LogFileLoopMax < 100 Then LogFileLoopMax = LogFileLoopMax + 1
   End If
   LogFileLoop = LogFileLoopMax
  End If
  LogFileLoop = LogFileLoop - 1
  ' Is Current job done?
  If (hProcess = 0) Or (WaitForSingleObject(hProcess, 1) = 0) Then
   BusyTimer.Enabled = False
   'Did the job work?
   Tmp = GetDMLog(AllList.SongName, OutPath, DMLogSize)
   If InStr(1, Tmp, "FAILURE", vbTextCompare) = 0 And _
      InStr(1, Tmp, "ERROR", vbTextCompare) = 0 And _
      InStr(1, Tmp, "End Time: ", vbTextCompare) <> 0 Then
    AllList.State = 2
    If AllList.CopyArt Then
     Dim SourceFile
     SourceFile = Left(AllList.SongName, InStrRev(AllList.SongName, "\") - 1)
     Tmp = Dir(SourceFile + "\AlbumArt_{*}_Large.jpg", vbArchive + vbHidden + vbSystem)
     If Tmp <> "" Then
      On Error Resume Next
      If Dir(OutPath + "\Banner.jpg", vbArchive) = "" Then _
       FileCopy SourceFile + "\" + Tmp, OutPath + "\Banner.jpg"
      If Dir(OutPath + "\*Background*.jpg", vbArchive) = "" Then _
       FileCopy SourceFile + "\" + Tmp, OutPath + "\Background.jpg"
      On Error GoTo 0
     End If
    End If
    If AllList.AddToSM And AllList.AddToDWI Then _
     SMtoDWICopy
   Else
    AllList.State = 3
   End If
   BusyList.Clear
   AllList.Populate Busy.BusyList, 1
   If (NextJob = 0) Then
    Unload Me
    Exit Sub
   End If
   BusyTimer.Enabled = True
  End If
End Sub

Private Sub Form_Load()
 On Error GoTo FatalError
 ErrStr = "Busy start; 1"
 Abort = False
 ListNum = 0
 LogFileLoopMax = 10
 AllTime = Time
 ErrStr = "Busy start; 1"
 AllList.FirstSong
 ErrStr = "Busy start; 2"
 Busy.BusyList.Clear
 ErrStr = "Busy start; 3"
 AllList.Populate Busy.BusyList, 1
 
 Pattern = "¸¸.·´¯`·."
 PatternLoop = Len(Pattern)
 If (NextJob <> 0) Then
  ErrStr = "Busy start; 4"
  BusyTimer.Enabled = True
 Else
  AbortTimer.Enabled = True
  BusyTimer.Enabled = False
 End If
 Exit Sub
FatalError:
 Load ERROR
 ERROR.Detail.Caption = ErrStr & Chr(13) & " Error # " & _
 Str(Err.Number) & " generated by " & Err.Source & Chr(13) & _
 Err.Description
 ERROR.Show
End Sub


Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
 If Not Abort Then
  If MsgBox("Are you sure you want to Kill Dancing Monkeys?" + vbCrLf + "You will lose " + Format(Time - StartTime, "HH:NN:SS") + " work!", vbYesNo + vbDefaultButton2 + vbExclamation, "Gorilla - Abort Current Song") = vbYes Then
   BusyTimer.Enabled = False
   KillDM
   Unload Me
  Else
   Cancel = True
  End If
 End If
End Sub

Private Sub Form_Resize()
 If Me.Width > 3000 Then BusyList.Width = Me.Width - 135
 If Me.Width > 3000 Then Output.Width = Me.Width - 2500
 If Me.Height > 5145 Then Output.Height = Me.Height - 3570
End Sub

Private Sub Form_Unload(Cancel As Integer)
 Dim DMhandle As Long
 Dim ExitCode As Long
 DMhandle = GetDMHandle
 If (DMhandle <> -1) Then
  TerminateProcess DMhandle, ExitCode
  Call CloseHandle(DMhandle)
  Call CloseHandle(hProcess)
 End If
 ChDrive Left(RunDir, 1)
 ChDir RunDir
 Gorilla.Recover
End Sub

Private Sub KillDMBtn_Click()
 Unload Me
End Sub

Private Sub SMtoDWICopy()
 Dim dwiPath As String
 Dim dwiFileList As String
 Dim P2 As String
 dwiPath = DWIDir + "\Songs"
 If AllList.DirToGroup Then 'StepMania/DWI Dir to Group
  P2 = Left(AllList.SongName, InStrRev(AllList.SongName, "\") - 1)
  dwiPath = dwiPath + Right(P2, Len(P2) - InStrRev(P2, "\"))
  If Dir(dwiPath, vbArchive + vbDirectory) = "" Then MkDir dwiPath
 Else 'Stepmania/DWI all in "my group"
  dwiPath = dwiPath + AllList.Group
  If Dir(dwiPath, vbArchive + vbDirectory) = "" Then MkDir dwiPath
 End If
 P2 = Right(AllList.SongName, Len(AllList.SongName) - InStrRev(AllList.SongName, "\"))
 dwiPath = dwiPath + Left(P2, Len(P2) - 4)
 If Dir(dwiPath, vbArchive + vbDirectory) = "" Then MkDir dwiPath
 dwiFileList = Dir(OutPath + "\", vbArchive + vbDirectory)
 On Error Resume Next
 Do While dwiFileList <> ""
  If dwiFileList <> "." And dwiFileList <> ".." Then _
   FileCopy OutPath + "\" + dwiFileList, dwiPath + "\" + dwiFileList
   dwiFileList = Dir
 Loop
 On Error GoTo 0
End Sub

Private Function BuildCommand(Name As String, ThisSong As Song)
 Dim FH As Integer
 Dim P2 As String
 Dim CT As String
 Dim Counter As String
 Dim Pos As Integer
 With ThisSong
  BuildCommand = "'" + DMExe + "'"
  BuildCommand = "DancingMonkeys.exe"
  BuildCommand = BuildCommand + " """ + Name + """"
  BuildCommand = BuildCommand + " " + CStr(.Basic)
  BuildCommand = BuildCommand + " " + CStr(.Medium)
  BuildCommand = BuildCommand + " " + CStr(.Hard)
  If .NoID Then BuildCommand = BuildCommand + " -n"
  If .OutputType = 0 Then BuildCommand = BuildCommand + " -omw"
' If .OutputType = 1 Then buildcommand=buildcommand 'MP3 is default output
  If .OutputType = 2 Then BuildCommand = BuildCommand + " -oms"
  If .CreditState = 1 Then BuildCommand = BuildCommand + " -onc "
  If .CreditState = 0 Then P2 = DefaultCredit
  If .CreditState = 2 Then P2 = .Credit
  If .CreditState = 0 Or .CreditState = 2 Then
   Pos = InStr(1, P2, """")
   Do While Pos <> 0
    P2 = CStr(Left(P2, Pos - 1)) + "''" + CStr(Right(P2, Len(P2) - Pos))
    Pos = InStr(Pos + 2, P2, """")
   Loop
   BuildCommand = BuildCommand + " -oc """ + P2 + """"
  End If
  If .NoStops Then BuildCommand = BuildCommand + " -ons"
  BuildCommand = BuildCommand + " -es " + CStr(.ErrorStops)
  BuildCommand = BuildCommand + " -l " + CStr(.Length)
  BuildCommand = BuildCommand + " -f " + CStr(.Fade)
  BuildCommand = BuildCommand + " -m " + CStr(.BeatPerMeasure)
  If DMVerNum >= 1.05 Then BuildCommand = BuildCommand + " -x 1"
  If .Turbo = 2 Then
   CT = CanTurbo(Name)
   If CT <> -1 Then
    BuildCommand = BuildCommand + " -c 0"
    BuildCommand = BuildCommand + " -b " + CStr(CT) + ":" + CStr(CT)
   Else
    BuildCommand = BuildCommand + " -c " + CStr(.Confidence)
    BuildCommand = BuildCommand + " -b " + CStr(.BPM_Min) + ":" + CStr(.BPM_Max)
   End If
  Else
   BuildCommand = BuildCommand + " -c " + CStr(.Confidence)
   BuildCommand = BuildCommand + " -b " + CStr(.BPM_Min) + ":" + CStr(.BPM_Max)
  End If
  BuildCommand = BuildCommand + " -g " + CStr(.GapAdjust)
  OutPath = Left(.OutDir, Len(.OutDir) - 1)
  If .AddToSM Then 'Add to StepMania
   OutPath = SMDir + "\Songs"
  ElseIf .AddToDWI Then 'No StepMania, Add to DWI
   OutPath = DWIDir + "\Songs"
  End If
  If .AddToSM Or .AddToDWI Then
   If Dir(OutPath, vbArchive + vbDirectory) = "" Then MkDir OutPath
   If .DirToGroup Then 'StepMania/DWI Dir to Group
    P2 = Left(Name, InStrRev(Name, "\") - 1)
    OutPath = OutPath + "\" + Right(P2, Len(P2) - InStrRev(P2, "\"))
    If Dir(OutPath, vbArchive + vbDirectory) = "" Then MkDir OutPath
   Else 'Stepmania/DWI all in "my group"
    OutPath = OutPath + "\" + .Group
    If Dir(OutPath, vbArchive + vbDirectory) = "" Then MkDir OutPath
   End If
  End If
  BuildCommand = BuildCommand + " """ + OutPath + """"
  P2 = Right(Name, Len(Name) - InStrRev(Name, "\"))
  OutPath = OutPath + "\" + Left(P2, Len(P2) - 4)
  Counter = ""
  FH = FreeFile
TryAgain:
  LogFile = DMDir + "Output\" + Left(P2, Len(P2) - 4) + Counter + ".log"
  GoTo LogOK
CycleLog:
  Close #FH
  If Counter = "" Then Counter = ".0" Else Counter = "." + CStr(CInt(Mid(Counter, 2)) + 1)
  GoTo TryAgain
LogOK:
' add the command to the top of the log file
  On Error GoTo CycleLog
  Open LogFile For Output As FH
  Print #FH, "Command:"
  Print #FH, BuildCommand + vbCrLf
  Print #FH, "Output:"
  Close #FH
' now get dm to log to that file.
  BuildCommand = BuildCommand + " >> """ + LogFile + """"
'  If Dir(SMPAth, vbArchive + vbDirectory) = "" Then MkDir SMPAth
 End With
 Exit Function
End Function

Private Sub ViewLog_Click()
 MsgBox "boogey"
End Sub

Private Sub VLog_Click()
 Dim CurrentSong As Integer
 Dim lLoop As Integer
 CurrentSong = AllList.cSong.UID
 AllList.FirstSong
 lLoop = 0
 Do While lLoop < BusyList.ListIndex And (Not AllList.cSong Is Nothing)
  AllList.NextSong
  lLoop = lLoop + 1
 Loop
 Load ViewLog
 ViewLog.ShowLog AllList.cSong
 lLoop = 0
 AllList.FirstSong
 Do While AllList.cSong.UID <> CurrentSong And (Not AllList.cSong Is Nothing)
  AllList.NextSong
  lLoop = lLoop + 1
 Loop
 ViewLog.Show 1, Me
 Unload ViewLog
End Sub

Private Sub WaveTimer_Timer()
  Busy.Boared.Caption = Right(Pattern, PatternLoop) _
        + Pattern + Pattern + Pattern + Pattern + Pattern
  Busy.Boared2.Caption = Right(Pattern, Len(Pattern) - PatternLoop) _
        + Pattern + Pattern + Pattern + Pattern + Pattern
  If PatternLoop = 1 Then PatternLoop = Len(Pattern) Else PatternLoop = PatternLoop - 1
  AllSong.Caption = Format(Time - AllTime, "HH:NN:SS")
  CurrentSong.Caption = Format(Time - StartTime, "HH:NN:SS")
  If ETA.Caption <> "--:--:--" Then
   If (CDate(Time - StartTime) >= CDate(ETA.Caption)) Then
    ETA.Caption = Format((Time - StartTime) * 3576 / TrackTime(TrackCount - 1), "HH:NN:SS")
   End If
  End If
End Sub

Private Function GetDMHandle() As Long
Const PROCESS_ALL_ACCESS = &H1F0FFF
Const TH32CS_SNAPPROCESS As Long = 2&
Dim uProcess  As PROCESSENTRY32
Dim RProcessFound As Long
Dim hSnapshot As Long
Dim SzExename As String
Dim i As Integer
Dim NameProcess As String
       
NameProcess = "DancingMonkeys.exe"

uProcess.dwSize = Len(uProcess)
hSnapshot = CreateToolhelpSnapshot(TH32CS_SNAPPROCESS, 0&)
RProcessFound = ProcessFirst(hSnapshot, uProcess)

Do
    i = InStr(1, uProcess.szexeFile, Chr(0))
    SzExename = LCase$(Left$(uProcess.szexeFile, i - 1))
       
    If Right$(SzExename, Len(NameProcess)) = LCase$(NameProcess) _
      And _
     uProcess.th32ParentProcessID = DMPID Then
        GetDMHandle = OpenProcess(PROCESS_ALL_ACCESS, False, uProcess.th32ProcessID)
        Call CloseHandle(hSnapshot)
        Exit Function
    End If
    RProcessFound = ProcessNext(hSnapshot, uProcess)
Loop While RProcessFound
Call CloseHandle(hSnapshot)
GetDMHandle = -1

End Function

Private Sub KillDM()
 Dim DMhandle As Long
 Dim ExitCode As Long
 DMhandle = GetDMHandle
 If (DMhandle = -1) Then
  MsgBox "ERROR- Could not find DancingMonkeys.exe to kill!", vbCritical, "Gorilla ERROR"
 Else
  TerminateProcess DMhandle, ExitCode
  Call CloseHandle(DMhandle)
  Call CloseHandle(hProcess)
 End If
End Sub

Private Function SetDMPriorityClass() As Boolean
 Dim DMhandle As Long
 DMhandle = GetDMHandle
 If (DMhandle <> -1) Then
  SetDMPriorityClass = True
  Call SetPriorityClass(DMhandle, IDLE_PRIORITY_CLASS)
 Else
  SetDMPriorityClass = False
 End If
End Function

Private Sub SetTrack()
TrackText(1) = "Begin processing input file "
TrackText(2) = "Song file successfully read."
TrackText(3) = "Normalized song."
TrackText(4) = "Monoed song."
TrackText(5) = "Smoothed song."
TrackText(6) = "Normalised mono and smoothed data."
TrackText(7) = "Thresholded data."
TrackText(8) = "Found peaks and troughs."
TrackText(9) = "Found beat positions."
TrackText(10) = "Calculated peak to beat offset."
TrackText(11) = "Interval testing:  10% done, interval"
TrackText(12) = "Interval testing:  20% done, interval"
TrackText(13) = "Interval testing:  30% done, interval"
TrackText(14) = "Interval testing:  40% done, interval"
TrackText(15) = "Interval testing:  50% done, interval"
TrackText(16) = "Interval testing:  60% done, interval"
TrackText(17) = "Interval testing:  70% done, interval"
TrackText(18) = "Interval testing:  80% done, interval"
TrackText(19) = "Interval testing:  90% done, interval"
TrackText(20) = "Check fitness of BPMs."
TrackText(21) = "Fitness testing:  10% done, interval"
TrackText(22) = "Fitness testing:  20% done, interval"
TrackText(23) = "Fitness testing:  30% done, interval"
TrackText(24) = "Fitness testing:  40% done, interval"
TrackText(25) = "Fitness testing:  50% done, interval"
TrackText(26) = "Fitness testing:  60% done, interval"
TrackText(27) = "Fitness testing:  70% done, interval"
TrackText(28) = "Fitness testing:  80% done, interval"
TrackText(29) = "Fitness testing:  90% done, interval"
TrackText(30) = "Brute forced the interval tests."
TrackText(31) = "Calculated Energy."
TrackText(32) = "Computed Self Similarity."
TrackText(33) = "Calculated Bar Similarity."
TrackText(34) = "Calculated BPM:"
TrackText(35) = "Gap in seconds:"
TrackText(36) = "Confidence:"
TrackText(37) = "Divided song into groups."
TrackText(38) = "Found pauses."
TrackText(39) = "Found freeze arrow positions."
TrackText(40) = "Foot Rating:"
TrackText(41) = "Foot Rating:"
TrackText(42) = "Foot Rating:"
TrackText(43) = "Created arrow patterns for each difficulty level."
TrackText(44) = "Created output song directory"
TrackText(45) = "Outputted truncated normalized song."
TrackText(46) = "Output directory:"
TrackText(47) = "Created .dwi step file."
TrackText(48) = "Created .sm step file."
TrackText(49) = "Results:"
TrackText(50) = "Song file name:"
TrackText(51) = "BPM:"
TrackText(52) = "Gap:"
TrackText(53) = "Confidence:"
TrackText(54) = "End Time:"

TrackTime(1) = 0
TrackTime(2) = 7
TrackTime(3) = 152
TrackTime(4) = 153
TrackTime(5) = 157
TrackTime(6) = 179
TrackTime(7) = 185
TrackTime(8) = 191
TrackTime(9) = 192
TrackTime(10) = 192
TrackTime(11) = 460
TrackTime(12) = 725
TrackTime(13) = 960
TrackTime(14) = 1175
TrackTime(15) = 1381
TrackTime(16) = 1576
TrackTime(17) = 1762
TrackTime(18) = 1941
TrackTime(19) = 2115
TrackTime(20) = 2283
TrackTime(21) = 3354
TrackTime(22) = 3406
TrackTime(23) = 3406
TrackTime(24) = 3406
TrackTime(25) = 3406
TrackTime(26) = 3406
TrackTime(27) = 3406
TrackTime(28) = 3406
TrackTime(29) = 3406
TrackTime(30) = 3429
TrackTime(31) = 3430
TrackTime(32) = 3437
TrackTime(33) = 3440
TrackTime(34) = 3440
TrackTime(35) = 3440
TrackTime(36) = 3440
TrackTime(37) = 3444
TrackTime(38) = 3444
TrackTime(39) = 3519
TrackTime(40) = 3523
TrackTime(41) = 3524
TrackTime(42) = 3525
TrackTime(43) = 3525
TrackTime(44) = 3527
TrackTime(45) = 3576
TrackTime(46) = 3576
TrackTime(47) = 3576
TrackTime(48) = 3576
TrackTime(49) = 3576
TrackTime(50) = 3576
TrackTime(51) = 3576
TrackTime(52) = 3576
TrackTime(53) = 3576
TrackTime(54) = 3576
End Sub

