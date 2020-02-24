VERSION 5.00
Begin VB.Form Error 
   Caption         =   "Gorilla Critical Error"
   ClientHeight    =   2310
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   3510
   LinkTopic       =   "Form1"
   ScaleHeight     =   2310
   ScaleWidth      =   3510
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Exit 
      Caption         =   "Exit"
      Height          =   495
      Left            =   960
      TabIndex        =   2
      Top             =   1560
      Width           =   975
   End
   Begin VB.Label Detail 
      Caption         =   "Label2"
      Height          =   855
      Left            =   240
      TabIndex        =   1
      Top             =   480
      Width           =   3135
   End
   Begin VB.Label Comment 
      Caption         =   "Critical Error"
      Height          =   255
      Left            =   240
      TabIndex        =   0
      Top             =   120
      Width           =   975
   End
End
Attribute VB_Name = "Error"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Exit_Click()
 End
End Sub
