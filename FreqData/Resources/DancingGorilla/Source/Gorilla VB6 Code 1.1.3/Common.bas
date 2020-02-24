Attribute VB_Name = "Common"
Option Explicit
Sub SongToForm(ThisSong As Song, ThisForm As Form)
 If ThisSong Is Nothing Then Exit Sub
 With ThisForm
  .OutDir = ThisSong.OutDir
  .AddToSM = Abs(CInt(ThisSong.AddToSM))
  .AddToDWI = Abs(CInt(ThisSong.AddToDWI))
  .Delve = Abs(CInt(ThisSong.Delve))
  .CopyArtwork = Abs(CInt(ThisSong.CopyArt))
  If ThisSong.Turbo = 0 Then
   .Turbo = 0
   .Turbo.Enabled = False
   .TurboLabel.Enabled = False
   .Confidence.Enabled = True
   .ConfidenceLabel.Enabled = True
   .BPM_Min.Enabled = True
   .BPM_Max.Enabled = True
  ElseIf ThisSong.Turbo = 1 Then
   .Turbo = 0
   .Turbo.Enabled = True
   .Confidence.Enabled = True
   .ConfidenceLabel.Enabled = True
   .TurboLabel.Enabled = True
   .BPM_Min.Enabled = True
   .BPM_Max.Enabled = True
  Else
   .Turbo = 1
   .Turbo.Enabled = True
   .TurboLabel.Enabled = True
   .Confidence.Enabled = False
   .ConfidenceLabel.Enabled = False
   .BPM_Min.Enabled = False
   .BPM_Max.Enabled = False
  End If
  .DirToGroup = Abs(CInt(ThisSong.DirToGroup))
  .Group = ThisSong.Group
  If ThisSong.IsDir Then
   .IsDir.Enabled = True
   .IsFile.Enabled = False
   .DelveLabel.Visible = True
   .Delve.Visible = True
  Else
   .IsDir.Enabled = False
   .IsFile.Enabled = True
   .DelveLabel.Visible = False
   .Delve.Visible = False
  End If
  .AutoID3 = Abs(CInt(ThisSong.NoID))
  .Credit.ListIndex = ThisSong.CreditState
  .CustomCredit = ThisSong.Credit
  .OutputFormat.ListIndex = ThisSong.OutputType
  .NoStops = Abs(CInt(ThisSong.NoStops))
  .ErrorStops = ThisSong.ErrorStops
  .Length = ThisSong.Length
  .Fade = ThisSong.Fade
  .Confidence = ThisSong.Confidence
  .BeatPerMeasure = ThisSong.BeatPerMeasure
  .BPM_Min = ThisSong.BPM_Min
  .BPM_Max = ThisSong.BPM_Max
  .GapAdjust = ThisSong.GapAdjust
  .AddToSM = Abs(CInt(ThisSong.AddToSM))
  .AddToDWI = Abs(CInt(ThisSong.AddToDWI))
  .Basic = ThisSong.Basic
  .Medium = ThisSong.Medium
  .Hard = ThisSong.Hard
 End With
End Sub


Sub FormToSong(ThisForm As Form, ThisSong As Song, Optional Force As Boolean = False)
 If ThisSong Is Nothing Then Exit Sub
 If ThisSong.Default = True And Not Force Then Exit Sub
 With ThisSong
  If (ThisForm.AddToSM = 0 And ThisForm.AddToDWI = 0) Then .OutDir = ThisForm.OutDir
  .AddToSM = (ThisForm.AddToSM = 1)
  .AddToDWI = (ThisForm.AddToDWI = 1)
  .Delve = (ThisForm.Delve = 1)
  .CopyArt = (ThisForm.CopyArtwork = 1)
  If Not ThisForm.Turbo.Enabled Then
   .Turbo = 0
  ElseIf ThisForm.Turbo = 0 Then
   .Turbo = 1
  Else
   .Turbo = 2
  End If
  .DirToGroup = (ThisForm.DirToGroup = 1)
  .Group = ThisForm.Group
  .NoID = ThisForm.AutoID3
  .CreditState = ThisForm.Credit.ListIndex
  .Credit = ThisForm.CustomCredit
  .OutputType = ThisForm.OutputFormat.ListIndex
  .NoStops = (ThisForm.NoStops = 1)
  .ErrorStops = ThisForm.ErrorStops
  .Length = ThisForm.Length
  .Fade = ThisForm.Fade
  .Confidence = ThisForm.Confidence
  .BeatPerMeasure = ThisForm.BeatPerMeasure
  .BPM_Min = ThisForm.BPM_Min
  .BPM_Max = ThisForm.BPM_Max
  .GapAdjust = ThisForm.GapAdjust
  .AddToSM = ThisForm.AddToSM
  .AddToDWI = ThisForm.AddToDWI
  .Basic = ThisForm.Basic
  .Medium = ThisForm.Medium
  .Hard = ThisForm.Hard
 End With
End Sub

Sub Update(ThisForm As Form)
 With ThisForm
  If .Credit.ListIndex = 2 Then
   .CustomCredit.Visible = True
'   .CustomCredit = DefaultCredit
  Else
   .CustomCredit.Visible = False
  End If
  If SMDir <> "" Then
   .AddToSM.Enabled = True
   .AddToSMLabel.Enabled = True
  Else
   .AddToSM.Enabled = False
   .AddToSMLabel.Enabled = False
  End If
  If DWIDir <> "" Then
   .AddToDWI.Enabled = True
   .AddToDWILabel.Enabled = True
  Else
   .AddToDWI.Enabled = False
   .AddToDWILabel.Enabled = False
  End If
  If (.AddToSM = 0) And (.AddToDWI = 0) Then
   If Dir(.OutDir, vbArchive + vbDirectory) = "" Then .OutDir = Defaults.OutDir
   .DirToGroupLabel.Visible = False
   .DirToGroup.Visible = False
   .GroupLabel.Visible = False
   .Group.Visible = False
   .OutDir.Enabled = True
  Else
   .OutDir = ""
   If .AddToSM = 1 Then .OutDir = "<StepMania Default>  "
   If .AddToDWI = 1 Then .OutDir = .OutDir + "<DWI Default>"
   .OutDir.Enabled = False
   .DirToGroupLabel.Visible = True
   .DirToGroup.Visible = True
   If (.DirToGroup) Then
    .GroupLabel.Visible = False
    .Group.Visible = False
   Else
    .GroupLabel.Visible = True
    .Group.Visible = True
   End If
  End If
 End With
End Sub
