Attribute VB_Name = "OldFormat"
Option Explicit
Public Sub ImportV09Config()
  Dim FH As Integer
  Dim T1 As Boolean
  Dim T2 As Boolean
  Dim Tmp As String
  InitialDefault
  FH = FreeFile
  Open "Settings.gor" For Input As FH
  Input #FH, Tmp 'v0.9 heading
  Input #FH, DMExe
  Input #FH, SMDir
  Input #FH, DWIDir
  With Defaults
   Input #FH, Tmp
   .Name = Tmp
   Input #FH, Tmp
   .OutDir = Tmp
   Input #FH, Tmp
   .IsDir = Tmp
   Input #FH, Tmp
   .Delve = Tmp
   Input #FH, Tmp
   .DirToGroup = Tmp
   Input #FH, Tmp
   .Group = Tmp
   Input #FH, Tmp
   .NoID = Tmp
   Input #FH, Tmp
   T1 = Tmp
   Input #FH, Tmp
   .Credit = Tmp
   If Left(.Credit, 13) = "Gorilla v0.9." Then
    .Credit = DefaultCredit
    .CreditState = 0
   Else
    .CreditState = 2
   End If
   If T1 Then .CreditState = 1
   Input #FH, Tmp
   T1 = Tmp
   Input #FH, Tmp
   T2 = Tmp
   .OutputType = 1
   If T1 Then .OutputType = 2
   If T2 Then .OutputType = 0
   Input #FH, Tmp
   .NoStops = Tmp
   Input #FH, Tmp
   .ErrorStops = Tmp
   Input #FH, Tmp
   .Length = Tmp
   Input #FH, Tmp
   .Fade = Tmp
   Input #FH, Tmp
   .Confidence = Tmp
   Input #FH, Tmp
   .BeatPerMeasure = Tmp
   Input #FH, Tmp
   .BPM_Min = Tmp
   Input #FH, Tmp
   .BPM_Max = Tmp
   Input #FH, Tmp
   .GapAdjust = Tmp
   Input #FH, Tmp
   .AddToSM = Tmp
   Input #FH, Tmp
   .AddToDWI = Tmp
   Input #FH, Tmp
   .Basic = Tmp
   Input #FH, Tmp
   .Medium = Tmp
   Input #FH, Tmp
   .Hard = Tmp
  End With
 
  Close #FH
  FH = FreeFile
  ChDrive Left(RunDir, 1)
  ChDir (RunDir)
  Open "Settings.gor" For Output As FH
   Print #FH, "Gorilla v" + VERSION + " - Configuration"
   Print #FH, DMExe
   Print #FH, SMDir
   Print #FH, DWIDir
   SaveSong FH, Defaults, True
  Close #FH
End Sub

Public Sub ImportV010Config()
 Dim Tmp As String
 Dim FH As Integer
 InitialDefault
 FH = FreeFile
 Open "Settings.gor" For Input As FH
 Input #FH, Tmp 'v0.10 heading
 Input #FH, DMExe
 Input #FH, SMDir
 Input #FH, DWIDir
 With Defaults
  Input #FH, Tmp
  .Name = Tmp
  Input #FH, Tmp
  .State = Tmp
  Input #FH, Tmp
  .IsDir = Tmp
  Input #FH, Tmp
  .Default = Tmp
  Input #FH, Tmp
  .OutDir = Tmp
  Input #FH, Tmp
  .Delve = Tmp
  Input #FH, Tmp
  .CopyArt = Tmp
  Input #FH, Tmp
  .DirToGroup = Tmp
  Input #FH, Tmp
  .Group = Tmp
  Input #FH, Tmp
  .NoID = Tmp
  Input #FH, Tmp
  .CreditState = Tmp
  Input #FH, Tmp
  .Credit = Tmp
  If Left(.Credit, 14) = "Gorilla v0.10." Then .Credit = DefaultCredit
  Input #FH, Tmp
  .OutputType = Tmp
  Input #FH, Tmp
  .NoStops = Tmp
  Input #FH, Tmp
  .ErrorStops = Tmp
  Input #FH, Tmp
  .Length = Tmp
  Input #FH, Tmp
  .Fade = Tmp
  Input #FH, Tmp
  .Confidence = Tmp
  Input #FH, Tmp
  .BeatPerMeasure = Tmp
  Input #FH, Tmp
  .BPM_Min = Tmp
  Input #FH, Tmp
  .BPM_Max = Tmp
  Input #FH, Tmp
  .GapAdjust = Tmp
  Input #FH, Tmp
  .AddToSM = Tmp
  Input #FH, Tmp
  .AddToDWI = Tmp
  Input #FH, Tmp
  .Basic = Tmp
  Input #FH, Tmp
  .Medium = Tmp
  Input #FH, Tmp
  .Hard = Tmp
 End With
 
 Close #FH
 FH = FreeFile
 ChDrive Left(RunDir, 1)
 ChDir (RunDir)
 Open "Settings.gor" For Output As FH
  Print #FH, "Gorilla v" + VERSION + " - Configuration"
  Print #FH, DMExe
  Print #FH, SMDir
  Print #FH, DWIDir
  SaveSong FH, Defaults, True
 Close #FH
End Sub

Public Sub ImportV011Config()
 Dim Tmp As String
 Dim FH As Integer
 InitialDefault
 FH = FreeFile
 Open "Settings.gor" For Input As FH
 Input #FH, Tmp 'v0.11 heading
 Input #FH, DMExe
 Input #FH, SMDir
 Input #FH, DWIDir
 With Defaults
  Input #FH, Tmp
  .Name = Tmp
  Input #FH, Tmp
  .State = Tmp
  Input #FH, Tmp
  .IsDir = Tmp
  Input #FH, Tmp
  .Default = Tmp
  Input #FH, Tmp
  .OutDir = Tmp
  Input #FH, Tmp
  .Delve = Tmp
  Input #FH, Tmp
  .CopyArt = Tmp
  Input #FH, Tmp
  .Turbo = Tmp
  Input #FH, Tmp
  .DirToGroup = Tmp
  Input #FH, Tmp
  .Group = Tmp
  Input #FH, Tmp
  .NoID = Tmp
  Input #FH, Tmp
  .CreditState = Tmp
  Input #FH, Tmp
  .Credit = Tmp
  If Left(.Credit, 14) = "Gorilla v0.11." Then .Credit = DefaultCredit
  Input #FH, Tmp
  .OutputType = Tmp
  Input #FH, Tmp
  .NoStops = Tmp
  Input #FH, Tmp
  .ErrorStops = Tmp
  Input #FH, Tmp
  .Length = Tmp
  Input #FH, Tmp
  .Fade = Tmp
  Input #FH, Tmp
  .Confidence = Tmp
  Input #FH, Tmp
  .BeatPerMeasure = Tmp
  Input #FH, Tmp
  .BPM_Min = Tmp
  Input #FH, Tmp
  .BPM_Max = Tmp
  Input #FH, Tmp
  .GapAdjust = Tmp
  Input #FH, Tmp
  .AddToSM = Tmp
  Input #FH, Tmp
  .AddToDWI = Tmp
  Input #FH, Tmp
  .Basic = Tmp
  Input #FH, Tmp
  .Medium = Tmp
  Input #FH, Tmp
  .Hard = Tmp
 End With
 
 Close #FH
 FH = FreeFile
 ChDrive Left(RunDir, 1)
 ChDir (RunDir)
 Open "Settings.gor" For Output As FH
  Print #FH, "Gorilla v" + VERSION + " - Configuration"
  Print #FH, DMExe
  Print #FH, SMDir
  Print #FH, DWIDir
  SaveSong FH, Defaults, True
 Close #FH
End Sub


Public Sub ImportV09List(File As String)
  Dim FH As Integer
  Dim T1 As Boolean
  Dim T2 As Boolean
  Dim Tmp As String
  Dim lLoop As Integer
  Do While AllList.Count > 0
   AllList.Pop
  Loop
  Load About
  About.CanExit = "No"
  About.AboutBox = vbCrLf + "Importing v0.9.x Settings " + vbCrLf + "Loading..." + vbCrLf + CStr("0")
  About.Caption = "Gorilla - Loading"
  About.Ok.Visible = False
  About.Height = 1700
  About.Show 0, Gorilla
  About.Refresh
  FH = FreeFile
  Open File For Input As FH
  Input #FH, Tmp 'v0.9 heading
  Input #FH, Tmp 'v0.9 DMDir
  Input #FH, Tmp 'v0.9 SMDir
  Input #FH, Tmp 'v0.9 DWIDir
  For lLoop = 1 To 25 'v0.9 had default settings entry
   Line Input #FH, Tmp
  Next
  Do While Not EOF(FH)
   AllList.Push ""
   SongCopy Defaults, AllList.cSong
   With AllList.cSong
    Input #FH, Tmp
    .Name = Tmp
    .State = 0
    Input #FH, Tmp
    .OutDir = Tmp
    Input #FH, Tmp
    .IsDir = Tmp
    .Default = False
    Input #FH, Tmp
    .Delve = Tmp
    .CopyArt = Defaults.CopyArt
    If CanTurbo(.Name) <> -1 Then
     .Turbo = Defaults.Turbo
    Else
     .Turbo = 0
    End If
    Input #FH, Tmp
    .DirToGroup = Tmp
    Input #FH, Tmp
    .Group = Tmp
    Input #FH, Tmp
    .NoID = Tmp
    Input #FH, Tmp
    T1 = Tmp
    Input #FH, Tmp
    .Credit = Tmp
    If Left(.Credit, 13) = "Gorilla v0.9." Then
     .CreditState = 0
    Else
     .CreditState = 2
    End If
    If T1 Then .CreditState = 1
    Input #FH, Tmp
    T1 = Tmp
    Input #FH, Tmp
    T2 = Tmp
    .OutputType = 1
    If T1 Then .OutputType = 2
    If T2 Then .OutputType = 0
    Input #FH, Tmp
    .NoStops = Tmp
    Input #FH, Tmp
    .ErrorStops = Tmp
    Input #FH, Tmp
    .Length = Tmp
    Input #FH, Tmp
    .Fade = Tmp
    Input #FH, Tmp
    .Confidence = Tmp
    Input #FH, Tmp
    .BeatPerMeasure = Tmp
    Input #FH, Tmp
    .BPM_Min = Tmp
    Input #FH, Tmp
    .BPM_Max = Tmp
    Input #FH, Tmp
    .GapAdjust = Tmp
    Input #FH, Tmp
    .AddToSM = Tmp
    Input #FH, Tmp
    .AddToDWI = Tmp
    Input #FH, Tmp
    .Basic = Tmp
    Input #FH, Tmp
    .Medium = Tmp
    Input #FH, Tmp
    .Hard = Tmp
   End With
   About.AboutBox = vbCrLf + "Importing v0.9.x Settings " + vbCrLf + "Loading..." + vbCrLf + CStr(AllList.Count)
   About.Refresh
  Loop
  
  Close #FH
   
  FH = FreeFile
  Open File For Output As FH
  Print #FH, "Gorilla v" + VERSION + " - SongList"
  AllList.FirstSong
  Do While Not AllList.cSong Is Nothing
   SaveSong FH, AllList.cSong, True
   AllList.NextSong
  Loop
  Close #FH
  Unload About
End Sub

Public Sub ImportV010List(File As String)
  Dim FH As Integer
  Dim Tmp As String
  Do While AllList.Count > 0
   AllList.Pop
  Loop
  Load About
  About.CanExit = "No"
  About.AboutBox = vbCrLf + "Importing v0.10.x Settings " + vbCrLf + "Loading..." + vbCrLf + CStr("0")
  About.Caption = "Gorilla - Loading"
  About.Ok.Visible = False
  About.Height = 1700
  About.Show 0, Gorilla
  About.Refresh
  FH = FreeFile
  Open File For Input As FH
  Input #FH, Tmp 'v0.10 heading
  Input #FH, Tmp 'v0.10 DMDir
  Input #FH, Tmp 'v0.10 SMDir
  Input #FH, Tmp 'v0.10 DWIDir
  Do While Not EOF(FH)
   AllList.Push ""
   SongCopy Defaults, AllList.cSong
   With AllList.cSong
    Input #FH, Tmp
    .Name = Tmp
    Input #FH, Tmp
    .State = Tmp
    Input #FH, Tmp
    .IsDir = Tmp
    Input #FH, Tmp
    .Default = Tmp
    If Not .Default Then
     Input #FH, Tmp
     .OutDir = Tmp
     Input #FH, Tmp
     .Delve = Tmp
     Input #FH, Tmp
     .CopyArt = Tmp
     If CanTurbo(.Name) <> -1 Then
      .Turbo = Defaults.Turbo
     Else
      .Turbo = 0
     End If
     Input #FH, Tmp
     .DirToGroup = Tmp
     Input #FH, Tmp
     .Group = Tmp
     Input #FH, Tmp
     .NoID = Tmp
     Input #FH, Tmp
     .CreditState = Tmp
     Input #FH, Tmp
     .Credit = Tmp
     If Left(.Credit, 14) = "Gorilla v0.10." Then .Credit = DefaultCredit
     Input #FH, Tmp
     .OutputType = Tmp
     Input #FH, Tmp
     .NoStops = Tmp
     Input #FH, Tmp
     .ErrorStops = Tmp
     Input #FH, Tmp
     .Length = Tmp
     Input #FH, Tmp
     .Fade = Tmp
     Input #FH, Tmp
     .Confidence = Tmp
     Input #FH, Tmp
     .BeatPerMeasure = Tmp
     Input #FH, Tmp
     .BPM_Min = Tmp
     Input #FH, Tmp
     .BPM_Max = Tmp
     Input #FH, Tmp
     .GapAdjust = Tmp
     Input #FH, Tmp
     .AddToSM = Tmp
     Input #FH, Tmp
     .AddToDWI = Tmp
     Input #FH, Tmp
     .Basic = Tmp
     Input #FH, Tmp
     .Medium = Tmp
     Input #FH, Tmp
     .Hard = Tmp
    End If
   End With
   About.AboutBox = vbCrLf + "Importing v0.10.x Settings " + vbCrLf + "Loading..." + vbCrLf + CStr(AllList.Count)
   About.Refresh
  Loop
  
  Close #FH
   
  FH = FreeFile
  Open File For Output As FH
  Print #FH, "Gorilla v" + VERSION + " - SongList"
  AllList.FirstSong
  Do While Not AllList.cSong Is Nothing
   SaveSong FH, AllList.cSong
   AllList.NextSong
  Loop
  Close #FH
  Unload About
End Sub

Public Sub ImportV011List(File As String)
  Dim FH As Integer
  Dim Tmp As String
  Do While AllList.Count > 0
   AllList.Pop
  Loop
  Load About
  About.CanExit = "No"
  About.AboutBox = vbCrLf + "Importing v0.11.x Settings " + vbCrLf + "Loading..." + vbCrLf + CStr("0")
  About.Caption = "Gorilla - Loading"
  About.Ok.Visible = False
  About.Height = 1700
  About.Show 0, Gorilla
  About.Refresh
  FH = FreeFile
  Open File For Input As FH
  Input #FH, Tmp 'v0.11 heading
  Input #FH, Tmp 'v0.11 DMDir
  Input #FH, Tmp 'v0.11 SMDir
  Input #FH, Tmp 'v0.11 DWIDir
  Do While Not EOF(FH)
   AllList.Push ""
   SongCopy Defaults, AllList.cSong
   With AllList.cSong
    Input #FH, Tmp
    .Name = Tmp
    Input #FH, Tmp
    .State = Tmp
    Input #FH, Tmp
    .IsDir = Tmp
    Input #FH, Tmp
    .Default = Tmp
    If Not .Default Then
     Input #FH, Tmp
     .OutDir = Tmp
     Input #FH, Tmp
     .Delve = Tmp
     Input #FH, Tmp
     .CopyArt = Tmp
     Input #FH, Tmp
     If CanTurbo(.Name) <> -1 Then
      .Turbo = Tmp
     Else
      .Turbo = 0
     End If
     Input #FH, Tmp
     .DirToGroup = Tmp
     Input #FH, Tmp
     .Group = Tmp
     Input #FH, Tmp
     .NoID = Tmp
     Input #FH, Tmp
     .CreditState = Tmp
     Input #FH, Tmp
     .Credit = Tmp
     If Left(.Credit, 14) = "Gorilla v0.11." Then .Credit = DefaultCredit
     Input #FH, Tmp
     .OutputType = Tmp
     Input #FH, Tmp
     .NoStops = Tmp
     Input #FH, Tmp
     .ErrorStops = Tmp
     Input #FH, Tmp
     .Length = Tmp
     Input #FH, Tmp
     .Fade = Tmp
     Input #FH, Tmp
     .Confidence = Tmp
     Input #FH, Tmp
     .BeatPerMeasure = Tmp
     Input #FH, Tmp
     .BPM_Min = Tmp
     Input #FH, Tmp
     .BPM_Max = Tmp
     Input #FH, Tmp
     .GapAdjust = Tmp
     Input #FH, Tmp
     .AddToSM = Tmp
     Input #FH, Tmp
     .AddToDWI = Tmp
     Input #FH, Tmp
     .Basic = Tmp
     Input #FH, Tmp
     .Medium = Tmp
     Input #FH, Tmp
     .Hard = Tmp
    End If
   End With
   About.AboutBox = vbCrLf + "Importing v0.11.x Settings " + vbCrLf + "Loading..." + vbCrLf + CStr(AllList.Count)
   About.Refresh
  Loop
  
  Close #FH
   
  FH = FreeFile
  Open File For Output As FH
  Print #FH, "Gorilla v" + VERSION + " - SongList"
  AllList.FirstSong
  Do While Not AllList.cSong Is Nothing
   SaveSong FH, AllList.cSong
   AllList.NextSong
  Loop
  Close #FH
  Unload About
End Sub
