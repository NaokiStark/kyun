VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Begin VB.Form Config 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Gorilla - Configuration"
   ClientHeight    =   8130
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   8715
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   8130
   ScaleWidth      =   8715
   StartUpPosition =   3  'Windows Default
   WhatsThisHelp   =   -1  'True
   Begin VB.CommandButton Ok 
      Caption         =   "&OK"
      Enabled         =   0   'False
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
      Left            =   7440
      TabIndex        =   34
      ToolTipText     =   "Save Directories and all default settings."
      Top             =   2160
      Width           =   1095
   End
   Begin VB.Frame DefaultsFrame 
      Caption         =   "Defaults"
      Height          =   5415
      Left            =   120
      TabIndex        =   42
      Top             =   2640
      Visible         =   0   'False
      Width           =   8535
      Begin VB.CheckBox Turbo 
         Caption         =   "Turbo"
         Height          =   255
         Left            =   8040
         TabIndex        =   27
         ToolTipText     =   "If a stepfile (.sm or .dwi) exists, skip BPM detection. This speeds DancingMonkeys by about 95%"
         Top             =   4200
         Width           =   255
      End
      Begin VB.CommandButton Factory 
         Caption         =   "Reset to Factory settings"
         Height          =   495
         Left            =   6240
         TabIndex        =   32
         Top             =   2400
         Width           =   1455
      End
      Begin VB.CommandButton Reload 
         Caption         =   "Reload Defaults from Settings.gor"
         Height          =   495
         Left            =   4200
         TabIndex        =   31
         Top             =   2400
         Width           =   1815
      End
      Begin VB.TextBox OutDir 
         Height          =   285
         Left            =   960
         TabIndex        =   6
         Top             =   480
         Width           =   6855
      End
      Begin VB.CommandButton GetOutDir 
         Caption         =   "..."
         Height          =   375
         Left            =   7920
         TabIndex        =   7
         Top             =   480
         Width           =   375
      End
      Begin VB.Frame OutputFrame 
         Caption         =   "Output to"
         Height          =   1455
         Left            =   120
         TabIndex        =   47
         Top             =   240
         Width           =   8295
         Begin VB.CheckBox CopyArtwork 
            Caption         =   "Check1"
            Height          =   195
            Left            =   7440
            TabIndex        =   12
            ToolTipText     =   "Use ""Windows Media Player"" artwork as song background and banner, if it exists  (StepMania Only)"
            Top             =   1080
            Width           =   255
         End
         Begin VB.CheckBox AddToDWI 
            Caption         =   "AddToDWI"
            Height          =   255
            Left            =   7560
            TabIndex        =   9
            ToolTipText     =   "Check this to output all step files generated to the directory specified by the Collection field below"
            Top             =   720
            Width           =   255
         End
         Begin VB.CheckBox AddToSM 
            Caption         =   "True"
            CausesValidation=   0   'False
            Height          =   255
            Left            =   3360
            TabIndex        =   8
            ToolTipText     =   "Check this to output all step files generated to the directory specified by the Collection field below"
            Top             =   720
            Width           =   255
         End
         Begin VB.CheckBox DirToGroup 
            Caption         =   "DirToGroup"
            Height          =   255
            Left            =   5520
            TabIndex        =   11
            ToolTipText     =   "Create a separate collection in StepMania/DWI for each directory"
            Top             =   1080
            Width           =   255
         End
         Begin VB.TextBox Group 
            Height          =   285
            Left            =   960
            TabIndex        =   10
            ToolTipText     =   "StepMania/DWI Collection to add song(s) to"
            Top             =   1080
            Width           =   2055
         End
         Begin VB.Label CopyArtLabel 
            Caption         =   "Copy Artwork"
            Height          =   255
            Left            =   6360
            TabIndex        =   69
            ToolTipText     =   "Use ""Windows Media Player"" artwork as song background and banner, if it exists  (StepMania Only)"
            Top             =   1080
            Width           =   1095
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
      End
      Begin VB.ComboBox Credit 
         Height          =   315
         ItemData        =   "Config.frx":0000
         Left            =   4680
         List            =   "Config.frx":000D
         Style           =   2  'Dropdown List
         TabIndex        =   22
         ToolTipText     =   "Who gets credit for the step file"
         Top             =   3120
         Width           =   1455
      End
      Begin VB.TextBox CustomCredit 
         Height          =   285
         Left            =   6240
         TabIndex        =   23
         Text            =   "Gorilla - Dancing Monkeys"
         ToolTipText     =   "Type your own credit here"
         Top             =   3120
         Visible         =   0   'False
         Width           =   2055
      End
      Begin VB.ComboBox OutputFormat 
         Height          =   315
         ItemData        =   "Config.frx":003B
         Left            =   1680
         List            =   "Config.frx":0048
         Style           =   2  'Dropdown List
         TabIndex        =   18
         ToolTipText     =   "Output as .wav, .mp3, or maintain original format"
         Top             =   3840
         Width           =   1455
      End
      Begin VB.CheckBox NoStops 
         Caption         =   "No Stops"
         Height          =   255
         Left            =   1680
         TabIndex        =   19
         ToolTipText     =   "Do not output any stops (pauses) in the steps file"
         Top             =   4200
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
         TabIndex        =   20
         ToolTipText     =   "If the number of stops found is >= #, then consider the process unsuccessful and exit"
         Top             =   4560
         Width           =   735
      End
      Begin VB.CheckBox AutoID3 
         Caption         =   "Auto ID3"
         Height          =   255
         Left            =   2400
         TabIndex        =   17
         ToolTipText     =   $"Config.frx":0064
         Top             =   3480
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
         TabIndex        =   21
         ToolTipText     =   $"Config.frx":0126
         Top             =   4920
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
         TabIndex        =   26
         ToolTipText     =   "Number of beats per measure of music"
         Top             =   4200
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
         TabIndex        =   28
         ToolTipText     =   "Minimum range to search the song for BPM"
         Top             =   4560
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
         TabIndex        =   29
         ToolTipText     =   "Maximum range to search the song for BPM"
         Top             =   4560
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
         TabIndex        =   30
         ToolTipText     =   $"Config.frx":0210
         Top             =   4920
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
         TabIndex        =   24
         ToolTipText     =   "Maximum song length. Songs longer than this value are truncated, faded out at the end. Default is 105 seconds."
         Top             =   3480
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
         TabIndex        =   25
         ToolTipText     =   "How far from the end of the song to start fading out the music, when the original track is too long"
         Top             =   3840
         Width           =   735
      End
      Begin VB.CheckBox Delve 
         Caption         =   "Delve"
         Height          =   255
         Left            =   3600
         TabIndex        =   16
         ToolTipText     =   "Check this if you want all subdirectories of  a particular directory searched for input songs"
         Top             =   2400
         Width           =   255
      End
      Begin VB.CommandButton AboutButton 
         Caption         =   "&About"
         Height          =   495
         Left            =   600
         TabIndex        =   33
         Top             =   2880
         Width           =   2415
      End
      Begin VB.Frame StepDificultyFrame 
         Caption         =   "Step Dificulty"
         Height          =   615
         Left            =   120
         TabIndex        =   43
         Top             =   1680
         Width           =   8175
         Begin VB.ComboBox Hard 
            Height          =   315
            ItemData        =   "Config.frx":02E1
            Left            =   7200
            List            =   "Config.frx":0300
            Style           =   2  'Dropdown List
            TabIndex        =   15
            Top             =   240
            Width           =   735
         End
         Begin VB.ComboBox Medium 
            Height          =   315
            ItemData        =   "Config.frx":031F
            Left            =   4200
            List            =   "Config.frx":033E
            Style           =   2  'Dropdown List
            TabIndex        =   14
            Top             =   240
            Width           =   735
         End
         Begin VB.ComboBox Basic 
            Height          =   315
            ItemData        =   "Config.frx":035D
            Left            =   1200
            List            =   "Config.frx":037C
            Style           =   2  'Dropdown List
            TabIndex        =   13
            Top             =   240
            Width           =   735
         End
         Begin VB.Label HardLabel 
            Caption         =   "Hard"
            Height          =   255
            Left            =   6360
            TabIndex        =   46
            Top             =   240
            Width           =   735
         End
         Begin VB.Label MediumLabel 
            Caption         =   "Medium"
            Height          =   255
            Left            =   3360
            TabIndex        =   45
            Top             =   240
            Width           =   735
         End
         Begin VB.Label BasicLabel 
            Caption         =   "Basic"
            Height          =   255
            Left            =   360
            TabIndex        =   44
            Top             =   240
            Width           =   735
         End
      End
      Begin VB.Label TurboLabel 
         Caption         =   "Turbo"
         Height          =   255
         Left            =   7440
         TabIndex        =   70
         ToolTipText     =   "If a stepfile (.sm or .dwi) exists, skip BPM detection. This speeds DancingMonkeys by about 95%"
         Top             =   4200
         Width           =   495
      End
      Begin VB.Label IsFile 
         Appearance      =   0  'Flat
         BackStyle       =   0  'Transparent
         BorderStyle     =   1  'Fixed Single
         Caption         =   "File"
         ForeColor       =   &H80000008&
         Height          =   255
         Left            =   120
         TabIndex        =   68
         Top             =   2400
         Visible         =   0   'False
         Width           =   495
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
         TabIndex        =   67
         Top             =   2400
         Visible         =   0   'False
         Width           =   735
      End
      Begin VB.Label NoStopLabel 
         Caption         =   "No Stops"
         Height          =   255
         Left            =   120
         TabIndex        =   66
         ToolTipText     =   "Do not output any stops (pauses) in the steps file"
         Top             =   4200
         Width           =   1455
      End
      Begin VB.Label ErrorStopsLabel 
         Caption         =   "Max Stops"
         Height          =   255
         Left            =   120
         TabIndex        =   65
         ToolTipText     =   "If the number of stops found is >= #, then consider the process unsuccessful and exit"
         Top             =   4560
         Width           =   1455
      End
      Begin VB.Label CreditLabel 
         Caption         =   "Credit"
         Height          =   255
         Left            =   3960
         TabIndex        =   64
         ToolTipText     =   "Who gets credit for the step file"
         Top             =   3120
         Visible         =   0   'False
         Width           =   495
      End
      Begin VB.Label OutputFormatLabel 
         Caption         =   "Output Format"
         Height          =   255
         Left            =   120
         TabIndex        =   63
         ToolTipText     =   "Output as .wav, .mp3, or maintain original format"
         Top             =   3840
         Width           =   1455
      End
      Begin VB.Label NoIDLabel 
         Caption         =   "Don't Auto Detect Title/Artist"
         Height          =   255
         Left            =   120
         TabIndex        =   62
         ToolTipText     =   $"Config.frx":039B
         Top             =   3480
         Width           =   2055
      End
      Begin VB.Label ConfidenceLabel 
         Caption         =   "Confidence"
         Height          =   255
         Left            =   120
         TabIndex        =   61
         ToolTipText     =   $"Config.frx":045D
         Top             =   4920
         Width           =   1455
      End
      Begin VB.Label BeatPerMeasureLabel 
         Caption         =   "Beats Per Measure"
         Height          =   255
         Left            =   4680
         TabIndex        =   60
         ToolTipText     =   "Number of beats per measure of music"
         Top             =   4200
         Width           =   1455
      End
      Begin VB.Label BPMLabel 
         Caption         =   "Beats Per Minute"
         Height          =   255
         Left            =   4680
         TabIndex        =   59
         ToolTipText     =   "Give a range to search the song for BPM"
         Top             =   4560
         Width           =   1455
      End
      Begin VB.Label BPMDash 
         Caption         =   "<x<"
         Height          =   255
         Left            =   7080
         TabIndex        =   58
         ToolTipText     =   "Give a range to search the song for BPM"
         Top             =   4560
         Width           =   255
      End
      Begin VB.Label GapAdjustLable 
         Caption         =   "Gap Adjustment"
         Height          =   255
         Left            =   4680
         TabIndex        =   57
         ToolTipText     =   $"Config.frx":0547
         Top             =   4920
         Width           =   1455
      End
      Begin VB.Label LengthLable 
         Caption         =   "Length"
         Height          =   255
         Left            =   4680
         TabIndex        =   56
         ToolTipText     =   "Maximum song length. Songs longer than this value are truncated, faded out at the end. Default is 105 seconds."
         Top             =   3480
         Width           =   1455
      End
      Begin VB.Label FadeLabel 
         Caption         =   "Fade"
         Height          =   255
         Left            =   4680
         TabIndex        =   55
         ToolTipText     =   "How far from the end of the song to start fading out the music, when the original track is too long"
         Top             =   3840
         Width           =   1455
      End
      Begin VB.Label DelveLabel 
         Caption         =   "Delve into subdirectories"
         Height          =   255
         Left            =   1680
         TabIndex        =   54
         ToolTipText     =   "Check this if you want all subdirectories of  a particular directory searched for input songs"
         Top             =   2400
         Width           =   1815
      End
   End
   Begin VB.Frame OptionalFrame 
      Caption         =   "Optional"
      Height          =   1095
      Left            =   120
      TabIndex        =   38
      Top             =   960
      Width           =   8535
      Begin VB.TextBox SMDir 
         Height          =   285
         Left            =   2400
         TabIndex        =   2
         Top             =   240
         Width           =   5535
      End
      Begin VB.TextBox DWIDir 
         Height          =   285
         Left            =   2400
         TabIndex        =   4
         Top             =   600
         Width           =   5535
      End
      Begin VB.CommandButton GetSMDir 
         Caption         =   "..."
         Height          =   375
         Left            =   8040
         TabIndex        =   3
         Top             =   240
         Width           =   375
      End
      Begin VB.CommandButton GetDWIDir 
         Caption         =   "..."
         Height          =   375
         Left            =   8040
         TabIndex        =   5
         Top             =   600
         Width           =   375
      End
      Begin VB.Label SMDirLabel 
         Caption         =   "StepMania Directory"
         Height          =   255
         Left            =   120
         TabIndex        =   40
         Top             =   240
         Width           =   1695
      End
      Begin VB.Label DWIDirLabel 
         Caption         =   "Dance With Intensity Directory"
         Height          =   255
         Left            =   120
         TabIndex        =   39
         Top             =   600
         Width           =   2175
      End
   End
   Begin VB.Frame RequiredFrame 
      Caption         =   "Required"
      Height          =   735
      Left            =   120
      TabIndex        =   35
      Top             =   120
      Width           =   8535
      Begin VB.TextBox DMExe 
         Height          =   285
         Left            =   1920
         TabIndex        =   0
         Top             =   240
         Width           =   6015
      End
      Begin VB.CommandButton GetDMDir 
         Caption         =   "..."
         Height          =   375
         Left            =   8040
         TabIndex        =   1
         Top             =   240
         Width           =   375
      End
      Begin VB.Label DMDirLabel 
         Caption         =   "DancingMonkeys EXE"
         ForeColor       =   &H000000FF&
         Height          =   255
         Left            =   120
         TabIndex        =   37
         Top             =   240
         Width           =   1695
      End
   End
   Begin VB.CommandButton Abort 
      Caption         =   "&Close Gorilla"
      Height          =   385
      Left            =   120
      TabIndex        =   36
      Top             =   2160
      Width           =   855
   End
   Begin MSComDlg.CommonDialog OpenDMFile 
      Left            =   6600
      Top             =   2040
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
      DialogTitle     =   "Select DancingMonkeys executable"
      Filter          =   "DancingMonkeys.exe|DancingMonkeys.exe"
   End
   Begin VB.Label Comment 
      Caption         =   "These settings have been auto-detected.                         If they are wrong, you will need to set them manually."
      Height          =   495
      Left            =   1320
      TabIndex        =   41
      Top             =   2160
      Width           =   4095
   End
End
Attribute VB_Name = "Config"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Updating As Boolean

Private Function ValidateSMDir() As Boolean
  If Dir(SMDir + "\Program\StepMania.exe", vbArchive) = "" Then
  MsgBox "Bad StepMania directory." + vbCrLf + _
   SMDir + "\Program\StepMania.exe must exist.", vbCritical, _
   "Gorilla - Bad SM Directory"
  ValidateSMDir = False
  Exit Function
 End If
 If Dir(SMDir + "\Songs", vbArchive + vbDirectory) <> "" Then
  If (GetAttr(SMDir + "\Songs") And vbDirectory) = vbDirectory Then
   ValidateSMDir = True
   Exit Function
  End If
 End If
  MsgBox "Bad StepMania directory." + vbCrLf + _
   SMDir + "\Songs directory not found.", vbCritical, _
   "Gorilla - Bad SM Directory"
 ValidateSMDir = False
End Function

Private Function ValidateDWIDir() As Boolean
 If Dir(DWIDir + "\DWI2.EXE", vbArchive) = "" Then
  MsgBox "Bad DanceWithIntensity directory." + vbCrLf + _
   DWIDir + "\DWI2.EXE must exist.", vbCritical, _
   "Gorilla - Bad DWI Directory"
  ValidateDWIDir = False
  Exit Function
 End If
 If Dir(DWIDir + "\Songs", vbArchive + vbDirectory) <> "" Then
  If (GetAttr(DWIDir + "\songs") And vbDirectory) = vbDirectory Then
   ValidateDWIDir = True
   Exit Function
  End If
 End If
  MsgBox "Bad DanceWithIntensity directory." + vbCrLf + _
   DWIDir + "\Songs directory not found.", vbCritical, _
   "Gorilla - Bad DWI Directory"
  ValidateDWIDir = False
End Function


Private Sub Abort_Click()
 End
End Sub

Private Sub AboutButton_Click()
 Load About
 About.Show 1, Me
End Sub

Private Sub AddToDWI_Click()
 If Updating Then Exit Sub
 ConfigUpdate
End Sub

Private Sub AddToSM_Click()
 If Updating Then Exit Sub
 ConfigUpdate
End Sub

Private Sub Credit_Click()
 If Updating Then Exit Sub
 ConfigUpdate
End Sub

Private Sub DirToGroup_Click()
 If Updating Then Exit Sub
 ConfigUpdate
End Sub

Private Sub Factory_Click()
 If MsgBox("Current settings will be lost!" + vbCrLf + "Are you sure?", vbYesNo + vbExclamation, "Reset to factory Defaults") = vbYes Then
  InitialDefault
  SongToForm Defaults, Me
  ConfigUpdate
 End If
End Sub

Private Sub Reload_Click()
 If MsgBox("Current settings will be lost!" + vbCrLf + "Are you sure?", vbYesNo + vbExclamation, "Reload Defaults from file") = vbYes Then
  LoadDefault
  SongToForm Defaults, Me
  ConfigUpdate
 End If
End Sub

Private Sub Form_Load()
 Updating = True
 Me.DMExe = Globals.DMExe
 Me.SMDir = Globals.SMDir
 Me.DWIDir = Globals.DWIDir
 SongToForm Defaults, Me
 ConfigUpdate
 If DMExe = "" Then
 'First run
 'Search for DancingMonkeys
  DMExe = RunDir + "\bin\win32\DancingMonkeys.exe"
  If Not TestDMPath(DMExe) Then
   DMExe = RunDir + "\..\bin\win32\DancingMonkeys.exe"
   If Not TestDMPath(DMExe) Then
    DMExe = "C:\Program Files\Dancing Monkeys\bin\win32\DancingMonkeys.exe"
    If Not TestDMPath(DMExe) Then
     DMExe = "C:\Dancing Monkeys\bin\win32\DancingMonkeys.exe"
     If Not TestDMPath(DMExe) Then
      DMExe = ""
     End If
    End If
   End If
  End If
  Globals.DMExe = DMExe
  If DMExe <> "" Then
   GetDMVersion
   Me.CustomCredit = Globals.DefaultCredit
   OK.Enabled = True
   DefaultsFrame.Visible = True
   DMDirLabel.ForeColor = "&H80000012"
   Defaults.OutDir = DMDir + "Output\"
   OutDir = Defaults.OutDir
  End If
 End If
'Search for StepMania
 If SMDir = "" Or Not TestSMPath(SMDir) Then
  SMDir = "C:\Program Files\StepMania"
  If Not TestSMPath(SMDir) Then
   SMDir = "C:\StepMania"
   If Not TestSMPath(SMDir) Then
    SMDir = "C:\Program Files\SM"
    If Not TestSMPath(SMDir) Then
     SMDir = "C:\SM"
     If Not TestSMPath(SMDir) Then
      SMDir = ""
      Me.AddToSM = 0
     End If
    End If
   End If
  End If
  Globals.SMDir = Me.SMDir
 End If
'Search for DanceWithIntensity
 If DWIDir = "" Or Not TestDWIPath(DWIDir) Then
  DWIDir = "C:\Program Files\DanceWithIntensity"
  If Not TestDWIPath(DWIDir) Then
   DWIDir = "C:\DanceWithIntensity"
   If Not TestDWIPath(DWIDir) Then
    DWIDir = "C:\Program Files\DWI"
    If Not TestDWIPath(DWIDir) Then
     DWIDir = "C:\DWI"
     If Not TestDWIPath(DWIDir) Then
      DWIDir = ""
      Me.AddToDWI = 0
     End If
    End If
   End If
  End If
  Globals.DWIDir = DWIDir
 End If
 ConfigUpdate
 Updating = False
End Sub

Private Sub GetDMDir_Click()
 If Updating Then Exit Sub
 OpenDMFile.ShowOpen
 If OpenDMFile.FileName = "" Then Exit Sub
 Me.DMExe = OpenDMFile.FileName
 Globals.DMExe = OpenDMFile.FileName
 If Globals.DefaultCredit = "" Then
  GetDMVersion
  Me.CustomCredit = Globals.DefaultCredit
 Else
  GetDMVersion
 End If
 If Defaults.OutDir = "" Then
  Defaults.OutDir = DMDir + "Output\"
  Me.OutDir = Defaults.OutDir
 End If
 ConfigUpdate
End Sub

Private Sub GetDWIDir_Click()
 If Updating Then Exit Sub
 Load GetDir
 Set GetDir.ReturnTo = Config.DWIDir
 GetDir.Caption = "Gorilla - Select 'Dance With Intensity' Directory"
 GetDir.Show 1, Me
 If ValidateDWIDir Then
    DWIDir = Me.DWIDir
    If (Defaults.AddToDWI) Then
        Me.AddToDWI = 1
    Else
        Me.AddToDWI = 0
    End If
 Else
    Me.DWIDir = ""
    Me.AddToDWI = 0
 End If
 ConfigUpdate
End Sub

Private Sub GetSMDir_Click()
 If Updating Then Exit Sub
 Load GetDir
 Set GetDir.ReturnTo = Config.SMDir
 GetDir.Caption = "Gorilla - Select 'StepMania' Directory"
 GetDir.Show 1, Me
 If ValidateSMDir Then
    Globals.SMDir = Me.SMDir
    If (Defaults.AddToSM) Then
        Me.AddToSM = 1
    Else
        Me.AddToSM = 0
    End If
 Else
    Me.SMDir = ""
    Me.AddToSM = 0
 End If
 ConfigUpdate
End Sub

Private Sub GetOutDir_Click()
 If Updating Then Exit Sub
 Load GetDir
 Set GetDir.ReturnTo = Me.OutDir
 GetDir.Caption = "Gorilla - Select Output Directory"
 GetDir.Show 1, Me
 If Right(Me.OutDir, 1) <> "\" Then Me.OutDir = Me.OutDir + "\"
 Defaults.OutDir = Me.OutDir
 ConfigUpdate
End Sub

Private Sub OK_Click()
Dim FH As Integer
Globals.DMExe = Me.DMExe
Globals.SMDir = Me.SMDir
Globals.DWIDir = Me.DWIDir
FormToSong Me, Defaults, True
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
 Unload Me
End Sub

Sub ConfigUpdate()
 If (DMExe <> "") Then 'configuration loaded
  OK.Enabled = True
  DefaultsFrame.Visible = True
  DMDirLabel.ForeColor = "&H00000000"
  Comment.Visible = False
 Else
  OK.Enabled = False
  DefaultsFrame.Visible = False
  DMDirLabel.ForeColor = "&HFF"
  Comment.Visible = True
 End If
 If Dir(RunDir + "Settings.gor", vbArchive) <> "" Then
  Reload.Visible = False
 Else
  Reload.Visible = True
 End If
 Update Me
 Confidence.Enabled = True
 ConfidenceLabel.Enabled = True
 BPM_Min.Enabled = True
 BPM_Max.Enabled = True
 TurboLabel.Enabled = True
 Turbo.Enabled = True
End Sub

Private Sub OutDir_LostFocus()
 If Dir(Me.OutDir, vbArchive + vbDirectory) = "" Then
  Me.OutDir = Defaults.OutDir
 Else
  If Right(Me.OutDir, 1) <> "\" Then Me.OutDir = Me.OutDir + "\"
  Defaults.OutDir = Me.OutDir
 End If
End Sub

Private Sub SMDir_LostFocus()
 If SMDir <> "" Then
  If ValidateSMDir Then
     Globals.SMDir = Me.SMDir
  Else
     Me.SMDir = Globals.SMDir
  End If
 End If
 ConfigUpdate
End Sub

Private Sub DWIDir_LostFocus()
 If DWIDir <> "" Then
  If ValidateDWIDir Then
     Globals.DWIDir = Me.DWIDir
  Else
     Me.DWIDir = Globals.DWIDir
  End If
 End If
 ConfigUpdate
End Sub
