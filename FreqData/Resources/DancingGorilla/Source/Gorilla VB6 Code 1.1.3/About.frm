VERSION 5.00
Begin VB.Form About 
   Caption         =   "Gorilla - About"
   ClientHeight    =   3090
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   4680
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   ScaleHeight     =   3090
   ScaleWidth      =   4680
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
   Begin VB.TextBox CanExit 
      Height          =   285
      Left            =   480
      TabIndex        =   2
      Top             =   2520
      Visible         =   0   'False
      Width           =   975
   End
   Begin VB.CommandButton Ok 
      Caption         =   "OK"
      Height          =   495
      Left            =   1920
      TabIndex        =   0
      Top             =   2520
      Width           =   735
   End
   Begin VB.TextBox AboutBox 
      Alignment       =   2  'Center
      Appearance      =   0  'Flat
      BackColor       =   &H8000000F&
      BorderStyle     =   0  'None
      CausesValidation=   0   'False
      Height          =   2175
      Left            =   720
      Locked          =   -1  'True
      MultiLine       =   -1  'True
      TabIndex        =   1
      TabStop         =   0   'False
      Top             =   120
      Width           =   3135
   End
End
Attribute VB_Name = "About"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Sub Aboutbox_click()
 If CanExit = "No" Then Exit Sub
 Unload Me
End Sub

Private Sub Form_Click()
 If CanExit = "No" Then Exit Sub
 Unload Me
End Sub

Private Sub Form_Load()
AboutBox.Text = ""
AboutBox.Text = AboutBox.Text + "Gorilla" + vbCrLf
AboutBox.Text = AboutBox.Text + vbCrLf
AboutBox.Text = AboutBox.Text + "The DancingMonkeys 1.xx GUI" + vbCrLf
AboutBox.Text = AboutBox.Text + vbCrLf
AboutBox.Text = AboutBox.Text + vbCrLf
AboutBox.Text = AboutBox.Text + VERSION + vbCrLf
AboutBox.Text = AboutBox.Text + vbCrLf
AboutBox.Text = AboutBox.Text + vbCrLf
AboutBox.Text = AboutBox.Text + "By: David Flink" + vbCrLf
AboutBox.Text = AboutBox.Text + "eMail: DancingGorilla@gmail.com" + vbCrLf
AboutBox.Text = AboutBox.Text + vbCrLf
End Sub

Private Sub Ok_Click()
 Unload Me
End Sub
