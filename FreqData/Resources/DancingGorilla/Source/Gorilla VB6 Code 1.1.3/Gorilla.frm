VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Begin VB.Form Gorilla 
   Caption         =   "Gorilla"
   ClientHeight    =   7560
   ClientLeft      =   1680
   ClientTop       =   1575
   ClientWidth     =   8490
   DrawStyle       =   1  'Dash
   Icon            =   "Gorilla.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   7560
   ScaleWidth      =   8490
   Begin VB.CommandButton RedoAll 
      Caption         =   "Bulk Changes to Job List"
      Height          =   435
      Left            =   4320
      TabIndex        =   5
      Top             =   2040
      Visible         =   0   'False
      Width           =   1215
   End
   Begin VB.CommandButton ViewLogFile 
      Caption         =   "View Log"
      Height          =   375
      Left            =   4920
      TabIndex        =   6
      ToolTipText     =   "Show DancingMonkey's output when it processed this file."
      Top             =   2640
      Visible         =   0   'False
      Width           =   855
   End
   Begin VB.CommandButton Exit 
      Caption         =   "Exit"
      Height          =   255
      Left            =   7440
      TabIndex        =   10
      Top             =   2760
      Width           =   975
   End
   Begin VB.CommandButton Redo 
      Caption         =   "Redo This File"
      Height          =   390
      Left            =   3960
      TabIndex        =   4
      Top             =   2640
      Visible         =   0   'False
      Width           =   855
   End
   Begin VB.CommandButton LoadList 
      Caption         =   "Load Job List"
      Height          =   375
      Left            =   5880
      TabIndex        =   8
      ToolTipText     =   "Load a previously saved list of songs for (re)processing."
      Top             =   2640
      Width           =   1215
   End
   Begin VB.CommandButton SaveList 
      Caption         =   "Save Job List"
      Height          =   375
      Left            =   5880
      TabIndex        =   7
      ToolTipText     =   "Save current list of songs for future (re)processing."
      Top             =   2040
      Width           =   1215
   End
   Begin VB.CommandButton AboutButton 
      Caption         =   "&About"
      Height          =   255
      Left            =   3240
      TabIndex        =   38
      Top             =   2760
      Width           =   615
   End
   Begin VB.CommandButton Configure 
      Caption         =   "&Configure"
      Height          =   375
      Left            =   7440
      TabIndex        =   9
      ToolTipText     =   "Set Program directories and other defaults."
      Top             =   2040
      Width           =   975
   End
   Begin VB.CommandButton Go 
      Caption         =   "&GO"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   12
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   495
      Left            =   600
      TabIndex        =   37
      ToolTipText     =   "Begin processing songs"
      Top             =   2640
      Width           =   2415
   End
   Begin VB.CommandButton Delete 
      Caption         =   "D&elete"
      Height          =   375
      Left            =   3000
      TabIndex        =   3
      ToolTipText     =   "Delete the hilighted entry formt he process list"
      Top             =   2040
      Width           =   975
   End
   Begin VB.CommandButton AddDir 
      Caption         =   "Add &Directory"
      Height          =   375
      Left            =   1320
      TabIndex        =   2
      ToolTipText     =   "Add a folder (and it's sub-folders) to process"
      Top             =   2040
      Width           =   1335
   End
   Begin VB.CommandButton AddFile 
      Caption         =   "Add &Song"
      Height          =   375
      Left            =   120
      TabIndex        =   1
      ToolTipText     =   "Add a single song to be processed"
      Top             =   2040
      Width           =   1095
   End
   Begin MSComDlg.CommonDialog OpenFile 
      Left            =   0
      Top             =   2520
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
      DialogTitle     =   "Gorilla - Add Music File"
      Filter          =   "All Music|*.mp3; *.wav|MP3|*.mp3||*.wav"
      PrinterDefault  =   0   'False
   End
   Begin VB.ListBox SongList 
      BeginProperty Font 
         Name            =   "Lucida Console"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1710
      ItemData        =   "Gorilla.frx":08CA
      Left            =   120
      List            =   "Gorilla.frx":08CC
      OLEDropMode     =   1  'Manual
      TabIndex        =   0
      ToolTipText     =   "List of songs/folders. Check""Fast Help"" more info."
      Top             =   120
      Width           =   8295
   End
   Begin VB.Frame DefaultFrame 
      BorderStyle     =   0  'None
      Height          =   4335
      Left            =   0
      TabIndex        =   39
      Top             =   3240
      Width           =   8535
      Begin VB.CommandButton HelpButton 
         Caption         =   "Fast Help"
         Height          =   495
         Left            =   3960
         TabIndex        =   69
         ToolTipText     =   "Four Easy steps to dancing."
         Top             =   1200
         Width           =   975
      End
      Begin VB.CommandButton Customize 
         Caption         =   "Customize"
         Height          =   735
         Left            =   2880
         TabIndex        =   40
         Top             =   3360
         Width           =   1455
      End
   End
   Begin VB.Frame CustomFrame 
      BorderStyle     =   0  'None
      Height          =   4335
      Left            =   0
      TabIndex        =   41
      Top             =   3240
      Width           =   8415
      Begin VB.CheckBox Turbo 
         Caption         =   "Turbo"
         Height          =   255
         Left            =   8040
         TabIndex        =   32
         ToolTipText     =   "If a stepfile (.sm or .dwi) exists, skip BPM detection. This speeds DancingMonkeys by about 95%"
         Top             =   3240
         Width           =   255
      End
      Begin VB.Frame OutputFrame 
         Caption         =   "Output to"
         Height          =   1455
         Left            =   120
         TabIndex        =   46
         Top             =   0
         Width           =   8295
         Begin VB.TextBox OutDir 
            Height          =   285
            Left            =   840
            TabIndex        =   11
            Top             =   240
            Width           =   6975
         End
         Begin VB.CommandButton GetOutDir 
            Caption         =   "..."
            Height          =   375
            Left            =   7800
            TabIndex        =   12
            Top             =   120
            Width           =   375
         End
         Begin VB.CheckBox AddToDWI 
            Caption         =   "AddToDWI"
            Height          =   255
            Left            =   7560
            TabIndex        =   14
            ToolTipText     =   "Check this to output all step files generated to the directory specified by the Collection field below"
            Top             =   720
            Width           =   255
         End
         Begin VB.CheckBox AddToSM 
            Caption         =   "True"
            CausesValidation=   0   'False
            Height          =   255
            Left            =   3360
            TabIndex        =   13
            ToolTipText     =   "Check this to output all step files generated to the directory specified by the Collection field below"
            Top             =   720
            Width           =   255
         End
         Begin VB.CheckBox DirToGroup 
            Caption         =   "DirToGroup"
            Height          =   255
            Left            =   5520
            TabIndex        =   16
            ToolTipText     =   "Create a separate collection in StepMania/DWI for each directory"
            Top             =   1080
            Width           =   255
         End
         Begin VB.TextBox Group 
            Height          =   285
            Left            =   960
            TabIndex        =   15
            ToolTipText     =   "StepMania/DWI Collection to add song(s) to"
            Top             =   1080
            Width           =   2055
         End
         Begin VB.CheckBox CopyArtwork 
            Caption         =   "Check1"
            Height          =   195
            Left            =   7440
            TabIndex        =   17
            ToolTipText     =   "Use ""Windows Media Player"" artwork as song background and banner, if it exists  (StepMania Only)"
            Top             =   1080
            Width           =   255
         End
         Begin VB.Label OutDirLabel 
            Caption         =   "Directory"
            Height          =   255
            Left            =   120
            TabIndex        =   53
            Top             =   240
            Width           =   735
         End
         Begin VB.Label OrLabel 
            Caption         =   "-Or-"
            Height          =   255
            Left            =   240
            TabIndex        =   52
            Top             =   480
            Width           =   375
         End
         Begin VB.Label AddToDWILabel 
            Caption         =   "Automatically add to DWI Collection"
            Height          =   255
            Left            =   4320
            TabIndex        =   51
            ToolTipText     =   "Check this to output all step files generated to the directory specified by the Collection field below"
            Top             =   720
            Width           =   3135
         End
         Begin VB.Label AddToSMLabel 
            Caption         =   "Automatically add to StepMania Collection"
            Height          =   255
            Left            =   120
            TabIndex        =   50
            ToolTipText     =   "Check this to output all step files generated to the directory specified by the Collection field below"
            Top             =   720
            Width           =   3135
         End
         Begin VB.Label DirToGroupLabel 
            Caption         =   "Directory to collection"
            Height          =   255
            Left            =   3840
            TabIndex        =   49
            ToolTipText     =   "Create a separate collection in StepMania/DWI for each directory"
            Top             =   1080
            Width           =   1695
         End
         Begin VB.Label GroupLabel 
            Caption         =   "Collection                                                        -Or-"
            Height          =   255
            Left            =   120
            TabIndex        =   48
            ToolTipText     =   "StepMania/DWI Collection to add song(s) to"
            Top             =   1080
            Width           =   3495
         End
         Begin VB.Label CopyArtLabel 
            Caption         =   "Copy Artwork"
            Height          =   255
            Left            =   6360
            TabIndex        =   47
            ToolTipText     =   "Use ""Windows Media Player"" artwork as song background and banner, if it exists  (StepMania Only)"
            Top             =   1080
            Width           =   1095
         End
      End
      Begin VB.ComboBox Credit 
         Height          =   315
         ItemData        =   "Gorilla.frx":08CE
         Left            =   4680
         List            =   "Gorilla.frx":08DB
         Style           =   2  'Dropdown List
         TabIndex        =   27
         ToolTipText     =   "Who gets credit for the step file"
         Top             =   2160
         Width           =   1455
      End
      Begin VB.TextBox CustomCredit 
         Height          =   285
         Left            =   6240
         TabIndex        =   28
         Text            =   "Gorilla - Dancing Monkeys"
         ToolTipText     =   "Type your own credit here"
         Top             =   2160
         Visible         =   0   'False
         Width           =   2055
      End
      Begin VB.ComboBox OutputFormat 
         Height          =   315
         ItemData        =   "Gorilla.frx":0909
         Left            =   1680
         List            =   "Gorilla.frx":0916
         Style           =   2  'Dropdown List
         TabIndex        =   23
         ToolTipText     =   "Output as .wav, .mp3, or maintain original format"
         Top             =   2880
         Width           =   1455
      End
      Begin VB.CheckBox NoStops 
         Caption         =   "No Stops"
         Height          =   255
         Left            =   1680
         TabIndex        =   24
         ToolTipText     =   "Do not output any stops (pauses) in the steps file"
         Top             =   3240
         Width           =   255
      End
      Begin VB.TextBox ErrorStops 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   1680
         TabIndex        =   25
         ToolTipText     =   "If the number of stops found is >= #, then consider the process unsuccessful and exit"
         Top             =   3600
         Width           =   735
      End
      Begin VB.CheckBox AutoID3 
         Caption         =   "Auto ID3"
         Height          =   255
         Left            =   2400
         TabIndex        =   22
         ToolTipText     =   $"Gorilla.frx":0932
         Top             =   2520
         Width           =   255
      End
      Begin VB.TextBox Confidence 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   1680
         TabIndex        =   26
         ToolTipText     =   $"Gorilla.frx":09F4
         Top             =   3960
         Width           =   735
      End
      Begin VB.TextBox BeatPerMeasure 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   6240
         TabIndex        =   31
         ToolTipText     =   "Number of beats per measure of music"
         Top             =   3240
         Width           =   735
      End
      Begin VB.TextBox BPM_Min 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   6240
         TabIndex        =   33
         ToolTipText     =   "Minimum range to search the song for BPM"
         Top             =   3600
         Width           =   855
      End
      Begin VB.TextBox BPM_Max 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   7440
         TabIndex        =   34
         ToolTipText     =   "Maximum range to search the song for BPM"
         Top             =   3600
         Width           =   855
      End
      Begin VB.TextBox GapAdjust 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0.00000"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   6240
         TabIndex        =   35
         ToolTipText     =   $"Gorilla.frx":0ADE
         Top             =   3960
         Width           =   2055
      End
      Begin VB.TextBox Length 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   6240
         TabIndex        =   29
         ToolTipText     =   "Maximum song length. Songs longer than this value are truncated, faded out at the end. Default is 105 seconds."
         Top             =   2520
         Width           =   735
      End
      Begin VB.TextBox Fade 
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "#,##0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   2057
            SubFormatType   =   1
         EndProperty
         Height          =   285
         Left            =   6240
         TabIndex        =   30
         ToolTipText     =   "How far from the end of the song to start fading out the music, when the original track is too long"
         Top             =   2880
         Width           =   735
      End
      Begin VB.CheckBox Delve 
         Caption         =   "Delve"
         Height          =   255
         Left            =   3600
         TabIndex        =   21
         ToolTipText     =   "Check this if you want all subdirectories of  a particular directory searched for input songs"
         Top             =   2160
         Visible         =   0   'False
         Width           =   255
      End
      Begin VB.Frame StepDificultyFrame 
         Caption         =   "Step Dificulty"
         Height          =   615
         Left            =   120
         TabIndex        =   42
         Top             =   1440
         Width           =   8175
         Begin VB.ComboBox Hard 
            Height          =   315
            ItemData        =   "Gorilla.frx":0BAF
            Left            =   7200
            List            =   "Gorilla.frx":0BCE
            Style           =   2  'Dropdown List
            TabIndex        =   20
            Top             =   240
            Width           =   735
         End
         Begin VB.ComboBox Medium 
            Height          =   315
            ItemData        =   "Gorilla.frx":0BED
            Left            =   4200
            List            =   "Gorilla.frx":0C0C
            Style           =   2  'Dropdown List
            TabIndex        =   19
            Top             =   240
            Width           =   735
         End
         Begin VB.ComboBox Basic 
            Height          =   315
            ItemData        =   "Gorilla.frx":0C2B
            Left            =   1200
            List            =   "Gorilla.frx":0C4A
            Style           =   2  'Dropdown List
            TabIndex        =   18
            Top             =   240
            Width           =   735
         End
         Begin VB.Label HardLabel 
            Caption         =   "Hard"
            Height          =   255
            Left            =   6360
            TabIndex        =   45
            Top             =   240
            Width           =   735
         End
         Begin VB.Label MediumLabel 
            Caption         =   "Medium"
            Height          =   255
            Left            =   3360
            TabIndex        =   44
            Top             =   240
            Width           =   735
         End
         Begin VB.Label BasicLabel 
            Caption         =   "Basic"
            Height          =   255
            Left            =   360
            TabIndex        =   43
            Top             =   240
            Width           =   735
         End
      End
      Begin VB.CommandButton Revert 
         Caption         =   "Revert to Defaults"
         Height          =   735
         Left            =   2880
         TabIndex        =   36
         Top             =   3360
         Width           =   1455
      End
      Begin VB.Label IsDir 
         Appearance      =   0  'Flat
         BackColor       =   &H80000005&
         BackStyle       =   0  'Transparent
         BorderStyle     =   1  'Fixed Single
         Caption         =   "Directory"
         Enabled         =   0   'False
         ForeColor       =   &H80000008&
         Height          =   255
         Left            =   840
         TabIndex        =   68
         Top             =   2160
         Width           =   735
      End
      Begin VB.Label NoStopLabel 
         Caption         =   "No Stops"
         Height          =   255
         Left            =   120
         TabIndex        =   67
         ToolTipText     =   "Do not output any stops (pauses) in the steps file"
         Top             =   3240
         Width           =   1455
      End
      Begin VB.Label ErrorStopsLabel 
         Caption         =   "Max Stops"
         Height          =   255
         Left            =   120
         TabIndex        =   66
         ToolTipText     =   "If the number of stops found is >= #, then consider the process unsuccessful and exit"
         Top             =   3600
         Width           =   1455
      End
      Begin VB.Label OutputFormatLabel 
         Caption         =   "Output Format"
         Height          =   255
         Left            =   120
         TabIndex        =   65
         ToolTipText     =   "Output as .wav, .mp3, or maintain original format"
         Top             =   2880
         Width           =   1455
      End
      Begin VB.Label NoIDLabel 
         Caption         =   "Don't Auto Detect Title/Artist"
         Height          =   255
         Left            =   120
         TabIndex        =   64
         ToolTipText     =   $"Gorilla.frx":0C69
         Top             =   2520
         Width           =   2055
      End
      Begin VB.Label ConfidenceLabel 
         Caption         =   "Confidence"
         Height          =   255
         Left            =   120
         TabIndex        =   63
         ToolTipText     =   $"Gorilla.frx":0D2B
         Top             =   3960
         Width           =   1455
      End
      Begin VB.Label BeatPerMeasureLabel 
         Caption         =   "Beats Per Measure"
         Height          =   255
         Left            =   4680
         TabIndex        =   62
         ToolTipText     =   "Number of beats per measure of music"
         Top             =   3240
         Width           =   1455
      End
      Begin VB.Label BPMLabel 
         Caption         =   "Beats Per Minute"
         Height          =   255
         Left            =   4680
         TabIndex        =   61
         ToolTipText     =   "Give a range to search the song for BPM"
         Top             =   3600
         Width           =   1455
      End
      Begin VB.Label BPMDash 
         Caption         =   "<x<"
         Height          =   255
         Left            =   7080
         TabIndex        =   60
         ToolTipText     =   "Give a range to search the song for BPM"
         Top             =   3600
         Width           =   255
      End
      Begin VB.Label GapAdjustLable 
         Caption         =   "Gap Adjustment"
         Height          =   255
         Left            =   4680
         TabIndex        =   59
         ToolTipText     =   $"Gorilla.frx":0E15
         Top             =   3960
         Width           =   1455
      End
      Begin VB.Label LengthLable 
         Caption         =   "Length"
         Height          =   255
         Left            =   4680
         TabIndex        =   58
         ToolTipText     =   "Maximum song length. Songs longer than this value are truncated, faded out at the end. Default is 105 seconds."
         Top             =   2520
         Width           =   1455
      End
      Begin VB.Label FadeLabel 
         Caption         =   "Fade"
         Height          =   255
         Left            =   4680
         TabIndex        =   57
         ToolTipText     =   "How far from the end of the song to start fading out the music, when the original track is too long"
         Top             =   2880
         Width           =   1455
      End
      Begin VB.Label DelveLabel 
         Caption         =   "Delve into subdirectories"
         Height          =   255
         Left            =   1680
         TabIndex        =   56
         ToolTipText     =   "Check this if you want all subdirectories of  a particular directory searched for input songs"
         Top             =   2160
         Visible         =   0   'False
         Width           =   1815
      End
      Begin VB.Label IsFile 
         Appearance      =   0  'Flat
         BackStyle       =   0  'Transparent
         BorderStyle     =   1  'Fixed Single
         Caption         =   "File"
         ForeColor       =   &H80000008&
         Height          =   255
         Left            =   120
         TabIndex        =   55
         Top             =   2160
         Width           =   495
      End
      Begin VB.Label TurboLabel 
         Caption         =   "Turbo"
         Height          =   255
         Left            =   7440
         TabIndex        =   54
         ToolTipText     =   "If a stepfile (.sm or .dwi) exists, skip BPM detection. This speeds DancingMonkeys by about 95%"
         Top             =   3240
         Width           =   495
      End
   End
End
Attribute VB_Name = "Gorilla"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private FH As Long
Private Updating As Boolean
Private OldOut As String
Private CurTop As Long
Private OldListIndex As Integer

Private Sub Form_Load()
 VERSION = "1.1.4"
 RunDir = App.path
 ErrStr = "Startup Failed 1"
 On Error GoTo FatalError
 LocalDecimal = Mid$(CSng(0.1), 2, Len(Str(CSng(0.1))) - 2)
 ErrStr = "Startup Failed 2"
 Set Defaults = New Song
 Set AllList = New List
 NeedToSave = False
 Me.Show
 ErrStr = "Startup Failed 2"
 Updating = True
 Initialize
 ErrStr = "Startup Failed 3"
 GorillaUpdate
 ErrStr = "Startup Failed 4"
 Dim ArgV() As String
 ArgV() = GetCommandLine()
 If UBound(ArgV) > 0 Then
  Dim Tmp As String
  Dim lLoop As Integer
  Dim Count As Integer
  On Error GoTo BadDropFile
  For lLoop = 1 To UBound(ArgV)
   If Dir(ArgV(lLoop), vbArchive + vbDirectory + vbHidden) <> "" Then
   If GetAttr(ArgV(lLoop)) And vbDirectory Then
    Tmp = ArgV(lLoop)
    If (Right(Tmp, 1) <> "\") Then Tmp = Tmp + "\"
    AllList.Push Tmp
    SongCopy Defaults, AllList.cSong
    AllList.IsDir = True
    AllList.Default = True
    Go.Enabled = True
    NeedToSave = True
   Else
    Tmp = UCase(Right(ArgV(lLoop), 3))
    If Tmp = "MP3" Or Tmp = "WAV" Then
     AllList.Push ArgV(lLoop)
     SongCopy Defaults, AllList.cSong
     AllList.IsDir = False
     AllList.Default = True
     Go.Enabled = True
     NeedToSave = True
     If CanTurbo(ArgV(lLoop)) <> -1 Then
      AllList.Turbo = Defaults.Turbo
     Else
      AllList.Turbo = 0
     End If
    ElseIf Tmp = "GOR" Then
     LoadJob ArgV(lLoop), False
    End If
   End If
   End If
BadDropFileResume:
  Next
  ErrStr = "Startup Failed 5"
  On Error GoTo FatalError
  AllList.FirstSong
  UpdateList
  ErrStr = "Startup Failed 6"
  If Not AllList.cSong Is Nothing Then
   Do While (Not AllList.cSong.NextSong Is Nothing) And AllList.State <> 0
    Count = Count + 1
    AllList.NextSong
   Loop
  End If
  ErrStr = "Startup Failed 7"
  If Count > 0 Then
   SongList.ListIndex = Count
   If Count > 5 Then SongList.TopIndex = Count - 5
   If Count = SongList.ListCount - 1 And AllList.cSong.State <> 0 Then _
         Go.Enabled = False
  End If
  ErrStr = "Startup Failed 8"
  GorillaUpdate
  SongList.SetFocus
 End If
 Updating = False
 Exit Sub
BadDropFile:
 Resume BadDropFileResume
FatalError:
 Load Error
 Error.Detail.Caption = ErrStr & Chr(13) & " Error # " & _
 Str(Err.Number) & " generated by " & Err.Source & Chr(13) & _
 Err.Description
 Error.Show
End Sub

Private Sub Form_Resize()
 On Error GoTo NoResize
 If Gorilla.Width < 8610 Then Gorilla.Width = 8610
 If Gorilla.Height < 8070 Then Gorilla.Height = 8070
 SongList.Width = Gorilla.Width - 315
 OutDir.Width = Gorilla.Width - 1635
 GetOutDir.Left = Gorilla.Width - 750
 OutputFrame.Width = Gorilla.Width - 300
 DefaultFrame.Width = Gorilla.Width - 195
 CustomFrame.Width = Gorilla.Width
NoResize:
 On Error GoTo 0
End Sub

Private Sub AboutButton_Click()
 FormToSong Me, AllList.cSong
 Load About
 About.Show 1, Me
End Sub

Private Sub AddToDWI_Click()
 If Updating Then Exit Sub
 Updating = True
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub

Private Sub AddToSM_Click()
 If Updating Then Exit Sub
 Updating = True
 If AddToSM = 0 Then OutDir = Defaults.OutDir
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub

Private Sub Credit_Click()
 If Updating Then Exit Sub
 Update Me
End Sub

Private Sub DirToGroup_Click()
 If Updating Then Exit Sub
 Updating = True
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub

Private Sub Form_Unload(Cancel As Integer)
 If AllList.Count > 0 And NeedToSave Then
  Dim Tmp As Integer
  Tmp = MsgBox("Do you want to Save your Job list first?", vbYesNoCancel, "Exit Gorilla")
  If Tmp = vbYes Then
   SaveList_Click
  ElseIf Tmp = vbCancel Then
   Cancel = True
  Exit Sub
  End If
 End If
End Sub

Private Sub GetOutDir_GotFocus()
 If AllList.Default Then Customize.SetFocus
End Sub

Private Sub Group_LostFocus()
 If Updating Then Exit Sub
 Updating = True
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub

Private Sub HelpButton_Click()
 Load Help
 Help.Show 1, Me
End Sub

Private Sub OutDir_LostFocus()
 If Updating Then Exit Sub
 If AllList.cSong Is Nothing Then Exit Sub
 Updating = True
 If Dir(OutDir, vbArchive + vbDirectory) = "" Then
  OutDir = OldOut
  If (Right(OutDir, 1) <> "\") Then OutDir = OutDir + "\"
  Updating = False
  Exit Sub
 End If
 If (Right(OutDir, 1) <> "\") Then OutDir = OutDir + "\"
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub

Private Sub OutDir_GotFocus()
 If AllList.cSong Is Nothing Then
  SongList.SetFocus
  Exit Sub
 ElseIf AllList.Default Then
  Customize.SetFocus
 Else
  OldOut = OutDir
 End If
End Sub

Private Sub GetOutDir_Click()
 If Updating Then Exit Sub
 Updating = True
 Load GetDir
 Set GetDir.ReturnTo = Gorilla.OutDir
 GetDir.Caption = "Gorilla - Select Output Directory"
 GetDir.Show 1, Me
 If (Right(OutDir, 1) <> "\") Then OutDir = OutDir + "\"
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub

Private Sub Redo_Click()
 If Updating Then Exit Sub
 Dim Index, Top As Integer
 Index = SongList.ListIndex
 Top = SongList.TopIndex
 Updating = True
 Customize.Enabled = False
 Revert.Enabled = False
 If AllList.State = 2 Then
  AllList.State = 0
  Turbo.Enabled = True
  TurboLabel.Enabled = True
  AllList.Turbo = Defaults.Turbo
  If AllList.Turbo = 2 Then Turbo = 1 Else Turbo = 0
 End If
 If AllList.State = 3 Then
  AllList.State = 0
  Turbo.Enabled = False
  TurboLabel.Enabled = False
  Turbo = 0
 End If
 If AllList.State = 0 Then
  Customize.Enabled = True
  Revert.Enabled = True
 End If
 UpdateList
 SongList.ListIndex = Index
 SongList.TopIndex = Top
 Redo.Visible = False
 ViewLogFile.Visible = False
 SongList.SetFocus
 NeedToSave = True
 Go.Enabled = True
 Updating = False
End Sub

Private Sub RedoAll_Click()
 If Updating Then Exit Sub
 Dim Index, Top As Integer
 Index = SongList.ListIndex
 Top = SongList.TopIndex
 Updating = True
 NeedToSave = True
 FormToSong Me, AllList.cSong
 
 Load RedoForm
 RedoForm.Show 1, Me
 
 UpdateList
 If Index >= SongList.ListCount Then Index = SongList.ListCount - 1
 SongList.ListIndex = Index
 If Top >= SongList.ListCount Then Top = SongList.ListCount - 7
 If Top < 0 Then Top = 0
 SongList.TopIndex = Top
 AllList.FirstSong
 Do While Not AllList.cSong Is Nothing
  If AllList.State = 0 Then
   Go.Enabled = True
   AllList.LastSong
  End If
  AllList.NextSong
 Loop
 GorillaUpdate
 Updating = False
End Sub

Private Sub SongList_Click()
 If Updating Then Exit Sub
 Updating = True
 If Not DefaultFrame.Visible Then FormToSong Me, AllList.cSong
 GorillaUpdate
 Updating = False
End Sub

Private Sub Configure_Click()
 Dim sLoop  As Integer
 If Updating Then Exit Sub
 Updating = True
 Load Config
 Config.Show 1, Me
 If DMExe = "" Then
  Form_Load
  Exit Sub
 End If
 Unload Config
 If SongList.ListCount > 0 Then
  AllList.FirstSong
  sLoop = 0
  Do While Not AllList.cSong Is Nothing
   If AllList.Default And AllList.State = 0 Then
    If Defaults.Turbo = 1 Then
     SongList.List(sLoop) = "[      ]" + _
      Right(SongList.List(sLoop), _
      Len(SongList.List(sLoop)) - 8)
    ElseIf CanTurbo(AllList.SongName) <> -1 Then
     SongList.List(sLoop) = "[TURBO ]" + _
      Right(SongList.List(sLoop), _
      Len(SongList.List(sLoop)) - 8)
    Else
     SongList.List(sLoop) = "[      ]" + _
      Right(SongList.List(sLoop), _
      Len(SongList.List(sLoop)) - 8)
    End If
   End If
   AllList.NextSong
   sLoop = sLoop + 1
  Loop
  GorillaUpdate
 End If
 SongList.SetFocus
 Updating = False
End Sub

Private Sub Customize_Click()
 If Updating Then Exit Sub
 Updating = True
 AllList.Default = False
 SongCopy Defaults, AllList.cSong
 GorillaUpdate
 SongList.SetFocus
 Updating = False
End Sub

Private Sub Revert_Click()
 If Updating Then Exit Sub
 Updating = True
 AllList.Default = True
 GorillaUpdate
 SongList.SetFocus
 Updating = False
End Sub

Private Sub Exit_Click()
 If Updating Then Exit Sub
 Unload Me
End Sub

Private Sub Delete_Click()
 If Updating Then Exit Sub
 If SongList.ListCount < 1 Then Exit Sub
 Dim Point, TopPoint As Integer
 Updating = True
 Point = Gorilla.SongList.ListIndex
 TopPoint = Gorilla.SongList.TopIndex
 AllList.Delete
 UpdateList
 If SongList.ListCount <= Point Then
  SongList.ListIndex = AllList.Count - 1
 Else
  SongList.ListIndex = Point
 End If
 If AllList.Count > 0 Then
  If SongList.ListCount <= TopPoint Then
   SongList.TopIndex = AllList.Count - 1
  Else
   SongList.TopIndex = TopPoint
  End If
 End If
 GorillaUpdate
 SongList.SetFocus
 NeedToSave = True
 Updating = False
End Sub

Private Sub AddDir_Click()
 Updating = True
 If Not DefaultFrame.Visible Then FormToSong Me, AllList.cSong
 OutDir = "__NONE__"
 Load GetDir
 Set GetDir.ReturnTo = OutDir
 GetDir.Caption = "Gorilla - Directory to Add"
 GetDir.Show 1, Me
 ChDrive Left(RunDir, 1)
 ChDir (RunDir)
 If OutDir = "__NONE__" Then
  If AllList.Count = 0 Then
   OutDir = ""
  Else
   OutDir = AllList.OutDir
  End If
  Updating = False
  Exit Sub
 End If
 On Error GoTo BadFile
 GetAttr (OutDir)
 If (Right(OutDir, 1) <> "\") Then OutDir = OutDir + "\"
 AllList.Push OutDir
 SongCopy Defaults, AllList.cSong
 AllList.IsDir = True
 AllList.Default = True
 UpdateList
 SongList.ListIndex = AllList.Count - 1
BadFile:
 On Error GoTo 0
 GorillaUpdate
 SongList.SetFocus
 Go.Enabled = True
 NeedToSave = True
 Updating = False
End Sub

Private Sub AddFile_Click()
 Updating = True
 If Not DefaultFrame.Visible Then FormToSong Me, AllList.cSong
 OpenFile.DialogTitle = "Gorilla - Add Music file"
 OpenFile.Filter = "All Music|*.mp3;*.wav|MP3|*.mp3|WAV|*.wav"
 OpenFile.ShowOpen
 ChDrive Left(RunDir, 1)
 ChDir (RunDir)
 If OpenFile.FileName = "" Then
  Updating = False
  Exit Sub
 End If
 On Error GoTo BadFile
 GetAttr (OpenFile.FileName)
 AllList.Push OpenFile.FileName
 SongCopy Defaults, AllList.cSong
 AllList.IsDir = False
 AllList.Default = True
 If CanTurbo(OpenFile.FileName) <> -1 Then
  AllList.Turbo = Defaults.Turbo
 Else
  AllList.Turbo = 0
 End If
 UpdateList
 SongList.ListIndex = AllList.Count - 1
BadFile:
 On Error GoTo 0
 GorillaUpdate
 SongList.SetFocus
 OpenFile.FileName = ""
 Go.Enabled = True
 NeedToSave = True
 Updating = False
End Sub

Private Sub SongList_DblClick()
 If Updating Then Exit Sub
 Call SongList_Click
 If (Not AllList.IsDir) Then Exit Sub
 If (AllList.State = 4) Then Exit Sub
 Dim Point, TopPoint As Integer
 Point = Gorilla.SongList.ListIndex
 TopPoint = Gorilla.SongList.TopIndex
 Updating = True
 ExpandDirectory
 UpdateList
 If SongList.ListCount <= Point Then
  SongList.ListIndex = AllList.Count - 1
 Else
  SongList.ListIndex = Point
 End If
 If Point > 5 Then
   SongList.TopIndex = Point - 3
 Else
   SongList.TopIndex = 0
 End If
 GorillaUpdate
 NeedToSave = True
 Updating = False
End Sub

Private Sub SongList_KeyUp(KeyCode As Integer, Shift As Integer)
 If KeyCode = vbKeyDelete Then Delete_Click
End Sub

Private Sub SongList_OLEDragDrop(Data As DataObject, Effect As Long, Button As Integer, Shift As Integer, x As Single, y As Single)
 Updating = True
 If Not DefaultFrame.Visible Then FormToSong Me, AllList.cSong
 Dim lLoop As Integer
 Dim Tmp As String
 Dim Count As Integer
 Effect = vbDropEffectCopy
 On Error GoTo BadFile
 For lLoop = 1 To Data.Files.Count
  If GetAttr(Data.Files(lLoop)) And vbDirectory Then
   Tmp = Data.Files(lLoop)
   If (Right(Tmp, 1) <> "\") Then Tmp = Tmp + "\"
   AllList.Push Tmp
   SongCopy Defaults, AllList.cSong
   AllList.IsDir = True
   AllList.Default = True
   Go.Enabled = True
  Else
   Tmp = UCase(Right(Data.Files(lLoop), 3))
   If Tmp = "MP3" Or Tmp = "WAV" Then
    AllList.Push Data.Files(lLoop)
    SongCopy Defaults, AllList.cSong
    AllList.IsDir = False
    AllList.Default = True
    Go.Enabled = True
    If CanTurbo(Data.Files(lLoop)) <> -1 Then
     AllList.Turbo = Defaults.Turbo
    Else
     AllList.Turbo = 0
    End If
   ElseIf Tmp = "GOR" Then
     LoadJob Data.Files(lLoop), False
   End If
  End If
BadFileResume:
 Next
 On Error GoTo 0
 AllList.FirstSong
 UpdateList
 If Not AllList.cSong Is Nothing Then
  Do While (Not AllList.cSong.NextSong Is Nothing) And AllList.State <> 0
   Count = Count + 1
   AllList.NextSong
  Loop
 End If
 If Count <> 0 Then
  SongList.ListIndex = Count
  If Count > 5 Then SongList.TopIndex = Count - 5
  If Count = SongList.ListCount - 1 And AllList.cSong.State <> 0 Then _
        Go.Enabled = False
 End If
 GorillaUpdate
 SongList.SetFocus
 Updating = False
 Exit Sub
BadFile:
 Resume BadFileResume
End Sub

Private Sub LoadList_Click()
 If Updating Then Exit Sub
 Updating = True
 If AllList.Count > 0 And NeedToSave Then
  Dim sTmp As Integer
   sTmp = MsgBox("Do you want to Save your current Job List first?", vbYesNoCancel, "Gorilla - Save before Load?")
  If sTmp = vbYes Then
   Updating = False
   SaveList_Click
   Updating = True
  ElseIf sTmp = vbCancel Then
   Updating = False
   Exit Sub
  End If
 End If
 FormToSong Me, AllList.cSong
 Dim Tmp As String
 Dim Count As Integer
 OpenFile.DialogTitle = "Gorilla - Load Job List"
 OpenFile.Filter = ".gor File|*.gor;"
 OpenFile.CancelError = True
 On Error GoTo CancelLoad
 OpenFile.ShowOpen
 ChDrive Left(RunDir, 1)
 ChDir RunDir
 If OpenFile.FileName = "" Then
  Updating = False
  Exit Sub
 End If
 
 LoadJob OpenFile.FileName, True
  
 AllList.FirstSong
 UpdateList
 If Not AllList.cSong Is Nothing Then
  Do While (Not AllList.cSong.NextSong Is Nothing) And AllList.State <> 0
   Count = Count + 1
   AllList.NextSong
  Loop
 End If
 SongList.ListIndex = Count
 If Count > 5 Then SongList.TopIndex = Count - 5
 If Count = SongList.ListCount - 1 And AllList.cSong.State <> 0 Then _
       Go.Enabled = False
 GorillaUpdate
 SongList.SetFocus
 NeedToSave = False
CancelLoad:
 Updating = False
 On Error GoTo 0
End Sub

Private Sub SaveList_Click()
 Dim Tmp As Integer
 If Updating Then Exit Sub
 If AllList.Count = 0 Then
  MsgBox "Cannot save empty list!", vbInformation + vbOKOnly, "Error Savig empty list"
  Exit Sub
 End If
 FormToSong Me, AllList.cSong
 OpenFile.DialogTitle = "Gorilla - Save Job List"
 OpenFile.Filter = ".gor File|*.gor;"
 OpenFile.Orientation = cdlLandscape
 OpenFile.CancelError = True
 On Error GoTo CancelSave
 OpenFile.ShowSave
 If Dir(OpenFile.FileName, vbArchive) <> "" Then
  If MsgBox("File Already exists!" + vbCrLf + "Are you sure you want to overwirte it?", vbOKCancel + vbDefaultButton2, "Confirm Overwrite") = vbCancel Then
   MsgBox "Settings NOT Saved!", vbExclamation
   Exit Sub
  End If
 End If
 ChDrive Left(RunDir, 1)
 ChDir RunDir
 If OpenFile.FileName = "" Then Exit Sub
 FH = FreeFile
 Open OpenFile.FileName For Output As FH
 Print #FH, "Gorilla v" + VERSION + " - SongList"
 AllList.FirstSong
 Do While Not AllList.cSong Is Nothing
  SaveSong FH, AllList.cSong
  AllList.NextSong
 Loop
 Close #FH
 MsgBox "Settings saved succesfully!", vbInformation
 SongList.SetFocus
 AllList.FirstSong
 For Tmp = 1 To SongList.ListIndex
  AllList.NextSong
 Next
 NeedToSave = False
 On Error GoTo 0
 Exit Sub
CancelSave:
 On Error GoTo 0
End Sub

Private Sub Go_Click()
 If Updating Then Exit Sub
 On Error GoTo FatalError
 ErrStr = "Can't go; 1"
 Updating = True
 OldListIndex = SongList.ListIndex
 ErrStr = "Can't go; 2"
 FormToSong Me, AllList.cSong
 ErrStr = "Can't go; 3"
 Load Busy
 ErrStr = "Can't go; 4"
 CurTop = Me.Top
 ErrStr = "Can't go; 5"
 Me.Top = 0 - Me.Height - 500
 ErrStr = "Can't go; 6"
 Me.Hide
 ErrStr = "Can't go; 7"
 Busy.Show
 ErrStr = "Can't go; 8"
 Exit Sub
FatalError:
 Load Error
 Error.Detail.Caption = ErrStr & Chr(13) & " Error # " & _
 Str(Err.Number) & " generated by " & Err.Source & Chr(13) & _
 Err.Description
 Error.Show
End Sub

Public Sub Recover()
 Dim Count As Integer
 Me.Show
 Me.Top = CurTop
 ChDrive Left(RunDir, 1)
 ChDir RunDir
 UpdateList
 GorillaUpdate
 Count = 0
 Do While (Not AllList.cSong.NextSong Is Nothing) And AllList.State <> 0
  Count = Count + 1
  AllList.NextSong
 Loop
 If Count > SongList.ListIndex Then Count = SongList.ListIndex
 SongList.ListIndex = Count
 SongToForm AllList.cSong, Me
 If Count > 5 Then SongList.TopIndex = Count - 5
 SongList.SetFocus
 If Count = SongList.ListCount - 1 And AllList.cSong.State <> 0 Then _
        Go.Enabled = False
 If OldListIndex <> SongList.ListIndex Then NeedToSave = True
 Updating = False
End Sub

Private Sub UpdateList()
 If SongList.ListCount > 0 And Not DefaultFrame.Visible Then _
  FormToSong Me, AllList.cSong
 Gorilla.SongList.Clear
 AllList.Populate Gorilla.SongList, 1
 If SongList.ListCount <> 0 Then SongList.ListIndex = 0
End Sub

Private Sub GorillaUpdate()
 If AllList.Count > 0 Then
  Dim Tmp
  AllList.FirstSong
  For Tmp = 1 To SongList.ListIndex
   AllList.NextSong
  Next
  If AllList.Default Then
   CustomFrame.Enabled = False
   DefaultFrame.Visible = True
   Customize.Enabled = True
   If AllList.IsDir Then
    Customize.Caption = "Customize this Directory"
   Else
    Customize.Caption = "Customize this Song"
   End If
  Else
   CustomFrame.Enabled = True
   DefaultFrame.Visible = False
   SongToForm AllList.cSong, Me
   Update Me
  End If
  If AllList.State <> 0 Then
   Revert.Enabled = False
   Customize.Enabled = False
  Else
   Revert.Enabled = True
   Customize.Enabled = True
  End If
  Redo.Visible = (AllList.State <> 0 And AllList.State <> 4)
  RedoAll.Visible = AllList.CanRedoAll
  ViewLogFile.Visible = Redo.Visible
  If ((Not AllList.Default And AllList.Turbo = 2) Or (AllList.Default And Defaults.Turbo = 2)) _
    And AllList.State = 0 And CanTurbo(AllList.SongName) <> -1 Then
   SongList.List(SongList.ListIndex) = "[TURBO ]" + _
    Right(SongList.List(SongList.ListIndex), _
    Len(SongList.List(SongList.ListIndex)) - 8)
  ElseIf AllList.State = 0 Then
   SongList.List(SongList.ListIndex) = "[      ]" + _
    Right(SongList.List(SongList.ListIndex), _
    Len(SongList.List(SongList.ListIndex)) - 8)
  End If
 Else
  Go.Enabled = False
  CustomFrame.Enabled = False
  DefaultFrame.Visible = True
  Redo.Visible = False
  ViewLogFile.Visible = False
  Customize.Enabled = False
  Customize.Caption = "Customize"
 End If
End Sub

Private Sub SongList_Scroll()
 Dim Tmp As Integer
 If SongList.ListIndex < SongList.TopIndex Then _
  SongList.ListIndex = SongList.TopIndex
 If SongList.ListIndex > SongList.TopIndex + 9 Then
  Tmp = SongList.TopIndex
  SongList.ListIndex = SongList.TopIndex + 9
  SongList.TopIndex = Tmp
 End If
End Sub

Private Sub ViewLogFile_Click()
 If Updating Then Exit Sub
 Load ViewLog
 ViewLog.ShowLog AllList.cSong
 ViewLog.Show 1, Me
 Unload ViewLog
 SongList.SetFocus
End Sub

Private Sub Turbo_Click()
 If Updating Then Exit Sub
 Updating = True
 If Turbo Then
  Confidence.Enabled = False
  ConfidenceLabel.Enabled = False
  BPM_Min.Enabled = False
  BPM_Max.Enabled = False
 Else
  Confidence.Enabled = True
  ConfidenceLabel.Enabled = True
  BPM_Min.Enabled = True
  BPM_Max.Enabled = True
 End If
 FormToSong Me, AllList.cSong
 If CanTurbo(AllList.SongName) = -1 Then
  AllList.Turbo = 0
 Else
  If AllList.Turbo = 0 Then
   AllList.Turbo = Defaults.Turbo
  End If
 End If
 GorillaUpdate
 Updating = False
End Sub
Sub LoadJob(FileName As String, ClearOldList As Boolean)
 Dim Tmp As String
 FH = FreeFile
 On Error GoTo DodgyFile
 Open FileName For Input As FH
 Input #FH, Tmp
 FileVer = Left(Tmp, InStrRev(Tmp, "."))
 FileVer = Right(FileVer, Len(FileVer) - InStrRev(FileVer, "v"))
 If Tmp = "Gorilla v0.9" Then
  Close #FH
  ImportV09List (FileName)
  MsgBox "Your v0.9.x List has been updated to v" + VERSION, vbInformation, "Gorilla - Updated"
  GoTo FileLoaded
 ElseIf FileVer = "0.10." Then
  Close #FH
  ImportV010List (FileName)
  MsgBox "Your v0.10.x List has been updated to v" + VERSION, vbInformation, "Gorilla - Updated"
  GoTo FileLoaded
 ElseIf FileVer = "0.11." Then
  Close #FH
  ImportV011List (FileName)
  MsgBox "Your v0.11.x List has been updated to v" + VERSION, vbInformation, "Gorilla - Updated"
  GoTo FileLoaded
 ElseIf FileVer = Left(VERSION, InStrRev(VERSION, ".")) And Right(Tmp, 10) = "- SongList" Then
  Load About
  About.CanExit = "No"
  About.AboutBox = vbCrLf + vbCrLf + "Loading..."
  About.Caption = "Gorilla - Loading"
  About.OK.Visible = False
  About.Height = 1700
  About.Show 0, Me
  About.Refresh
  Go.Enabled = False
  If ClearOldList Then
   While AllList.Count > 0
    AllList.Pop
   Wend
  End If
  Do While Not EOF(FH)
   AllList.Push ""
   SongCopy Defaults, AllList.cSong
   LoadSong FH, AllList.cSong
   If AllList.State = 0 Then
    Go.Enabled = True
   End If
   If Turbo <> 1 Then
    If CanTurbo(AllList.SongName) <> -1 Then
     AllList.Turbo = Defaults.Turbo
    Else
     AllList.Turbo = 0
    End If
   End If
  About.AboutBox = vbCrLf + vbCrLf + "Loading..." + vbCrLf + CStr(AllList.Count)
  About.Refresh
  Loop
  Close #FH
 ElseIf Right(Tmp, 15) = "- Configuration" Then
  Close #FH
  MsgBox "File is a Configuration file, not a Song List", vbCritical + vbOKOnly, "Gorilla - Error"
 Else
  Close #FH
  MsgBox "Bad File!", vbCritical
 End If
FileLoaded:
 Unload About
 Exit Sub
DodgyFile:
 Close #FH
 While AllList.Count > 0
  AllList.Pop
 Wend
 MsgBox "Failed to load data. File is corrupt!", vbCritical, "Gorilla - Fileload error"
 Unload About
End Sub
