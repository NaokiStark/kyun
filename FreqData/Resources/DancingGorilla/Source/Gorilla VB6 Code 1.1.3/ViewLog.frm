VERSION 5.00
Begin VB.Form ViewLog 
   Caption         =   "Gorilla - View Log"
   ClientHeight    =   5160
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   8010
   ClipControls    =   0   'False
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   ScaleHeight     =   5160
   ScaleWidth      =   8010
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Back 
      Caption         =   "Back"
      Height          =   255
      Left            =   7320
      TabIndex        =   1
      Top             =   4800
      Width           =   615
   End
   Begin VB.TextBox Log 
      Height          =   4695
      Left            =   0
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      ScrollBars      =   3  'Both
      TabIndex        =   0
      Top             =   0
      Width           =   7935
   End
   Begin VB.Label LogName 
      Height          =   255
      Left            =   240
      TabIndex        =   2
      Top             =   4800
      Width           =   6975
   End
End
Attribute VB_Name = "ViewLog"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Sub Back_Click()
 Unload Me
End Sub

Public Sub ShowLog(ShowSong As Song)
 Dim Tmp As String
 Dim P2, LogFile, DMLogPath As String
 Dim FileSize As Integer
 Dim FH As Long
 
 With ShowSong
  LogName = .Name
  On Error GoTo DodgyFile
 
  DMLogPath = Left(.OutDir, Len(.OutDir) - 1)
  If .AddToSM Then 'Add to StepMania
   DMLogPath = SMDir + "\Songs"
  ElseIf .AddToDWI Then 'No StepMania, Add to DWI
   DMLogPath = DWIDir + "\Songs"
  End If
  If .AddToSM Or .AddToDWI Then
   If .DirToGroup Then 'StepMania/DWI Dir to Group
    P2 = Left(.Name, InStrRev(.Name, "\") - 1)
    DMLogPath = DMLogPath + "\" + Right(P2, Len(P2) - InStrRev(P2, "\"))
   Else 'Stepmania/DWI all in "my group"
    DMLogPath = DMLogPath + "\" + .Group
   End If
  End If
  P2 = Right(.Name, Len(.Name) - InStrRev(.Name, "\"))
  DMLogPath = DMLogPath + "\" + Left(P2, Len(P2) - 4)
  
  Log = "DancingMonkeys Log:" + vbCrLf + "===================" + vbCrLf
  Log = Log + GetDMLog(.Name, DMLogPath) + vbCrLf + vbCrLf
  
  P2 = Right(LogName, Len(LogName) - InStrRev(LogName, "\"))
  LogFile = DMDir + "Output\" + Left(P2, Len(P2) - 4) + ".log"
  FileSize = FileLen(LogFile)
  FH = FreeFile
  Open LogFile For Input As FH
  Tmp = Input(FileSize, #FH)
  Close #FH
  Log = Log + "Gorilla Log:" + vbCrLf + "============" + vbCrLf + Tmp
 End With
 Exit Sub
DodgyFile:
 MsgBox "Error Loading Logs!", vbCritical
End Sub

Private Sub Form_Resize()
 Log.Width = Me.Width - 105
 Log.Height = Me.Height - 880
 LogName.Width = Me.Width - 1180
 LogName.Top = Me.Height - 780
 Back.Left = Me.Width - 840
 Back.Top = Me.Height - 780
End Sub
