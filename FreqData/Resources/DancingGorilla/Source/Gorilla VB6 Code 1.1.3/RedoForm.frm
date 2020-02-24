VERSION 5.00
Begin VB.Form RedoForm 
   Caption         =   "Gorilla - Bulk Changes"
   ClientHeight    =   4695
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   9735
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   ScaleHeight     =   4695
   ScaleWidth      =   9735
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton ClearDirectories 
      Caption         =   "Remove all expanded directories"
      Height          =   495
      Left            =   120
      TabIndex        =   3
      ToolTipText     =   "Expanded Directories are completely ignored by Gorilla. Removing them or leaving them is purely for user's convenience."
      Top             =   3120
      Width           =   1695
   End
   Begin VB.CommandButton Cancel 
      Caption         =   "Cancel"
      Height          =   495
      Left            =   8280
      TabIndex        =   4
      Top             =   4080
      Width           =   1335
   End
   Begin VB.CommandButton Both 
      Caption         =   "Remove all ""OK"" and all ""Failed"" marks"
      Height          =   495
      Left            =   120
      TabIndex        =   2
      ToolTipText     =   "This is a quick way to redo an entire Job list without having to click the other two buttons."
      Top             =   2280
      Width           =   1695
   End
   Begin VB.CommandButton Failed 
      Caption         =   "Remove all ""Failed"" marks"
      Height          =   495
      Left            =   120
      TabIndex        =   1
      ToolTipText     =   "Use this option to force DancingMonkeys to make step files for songs that failed in this job list, even if they are bad."
      Top             =   1440
      Width           =   1695
   End
   Begin VB.CommandButton Passed 
      Caption         =   "Remove all ""OK"" marks"
      Height          =   495
      Left            =   120
      TabIndex        =   0
      ToolTipText     =   "Use this option to quickly make new stepfiles for all songs already processed in this job list."
      Top             =   720
      Width           =   1695
   End
   Begin VB.Label Label1 
      Caption         =   $"RedoForm.frx":0000
      Height          =   615
      Left            =   1920
      TabIndex        =   9
      Top             =   3120
      Width           =   7575
   End
   Begin VB.Label OkAndFailedLabel 
      Caption         =   "This is a quick way to remove all the ""OK"" and the ""Failed"" marks."
      Height          =   495
      Left            =   1920
      TabIndex        =   8
      Top             =   2280
      Width           =   7455
   End
   Begin VB.Label FailedLabel 
      Caption         =   $"RedoForm.frx":00EB
      Height          =   615
      Left            =   1920
      TabIndex        =   7
      Top             =   1440
      Width           =   7455
   End
   Begin VB.Label OkLabel 
      Caption         =   $"RedoForm.frx":01FA
      Height          =   495
      Left            =   1920
      TabIndex        =   6
      Top             =   720
      Width           =   7335
   End
   Begin VB.Label Instructions 
      Caption         =   $"RedoForm.frx":02C5
      Height          =   375
      Left            =   240
      TabIndex        =   5
      Top             =   120
      Width           =   6135
   End
End
Attribute VB_Name = "RedoForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Both_Click()
 AllList.FirstSong
 Do While Not AllList.cSong Is Nothing
  If AllList.State = 3 Then
   AllList.State = 0
   If AllList.Default Then
    SongCopy Defaults, AllList.cSong
    AllList.Default = False
   End If
   AllList.Confidence = 0
  End If
  If AllList.State = 2 Then
   AllList.State = 0
   AllList.Turbo = 2
  End If
  AllList.NextSong
 Loop
 Unload Me
End Sub

Private Sub Cancel_Click()
 NeedToSave = False
 Unload Me
End Sub

Private Sub ClearDirectories_Click()
 AllList.FirstSong
 Do While Not AllList.cSong Is Nothing
  If AllList.State = 4 Then
   AllList.Delete
   NeedToSave = True
  End If
  AllList.NextSong
 Loop
 AllList.FirstSong
 If Not AllList.cSong Is Nothing Then
  Do While AllList.State = 4
   AllList.Delete
  Loop
 Else
  Gorilla.RedoAll.Visible = False
 End If
 Unload Me
End Sub

Private Sub Failed_Click()
 AllList.FirstSong
 Do While Not AllList.cSong Is Nothing
  If AllList.State = 3 Then
   AllList.State = 0
   If AllList.Default Then
    SongCopy Defaults, AllList.cSong
    AllList.Default = False
   End If
   AllList.Confidence = 0
  End If
  AllList.NextSong
 Loop
 Unload Me
End Sub

Private Sub Passed_Click()
 AllList.FirstSong
 Do While Not AllList.cSong Is Nothing
  If AllList.State = 2 Then
   AllList.State = 0
   AllList.Turbo = 2
  End If
  AllList.NextSong
 Loop
 Unload Me
End Sub
