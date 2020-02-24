VERSION 5.00
Begin VB.Form Help 
   Caption         =   "Help"
   ClientHeight    =   4065
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   7710
   LinkTopic       =   "Form1"
   ScaleHeight     =   4065
   ScaleWidth      =   7710
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton OtherButton 
      Caption         =   "Other options"
      Height          =   375
      Left            =   960
      TabIndex        =   2
      Top             =   3600
      Width           =   1335
   End
   Begin VB.CommandButton OK 
      Caption         =   "OK"
      Height          =   375
      Left            =   6600
      TabIndex        =   0
      Top             =   3600
      Width           =   735
   End
   Begin VB.Label HelpText 
      BeginProperty Font 
         Name            =   "Lucida Console"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   3255
      Left            =   120
      TabIndex        =   1
      Top             =   120
      Width           =   7455
   End
End
Attribute VB_Name = "Help"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Form_Load()
 HelpText.Caption = "Welcome to Gorilla!" & Chr(13) & _
    Chr(13) & _
    "The fastest way to get going is:" & Chr(13) & _
    "1] Open windows Explorer" & Chr(13) & _
    "2] Find some music you want to dance to" & Chr(13) & _
    "3] Drag and drop some songs or folders of songs " & _
    "to the white area at the top of Gorilla " & Chr(13) & _
    "4] Click [GO]" & Chr(13) & _
    Chr(13) & _
    "Beware that depending on the speed of your computer and " & _
    "the length of the songs, it can take many hours or even " & _
    "days to process loger lists." & Chr(13) & _
    Chr(13) & _
    "When Gorilla is done, just fire up your dance " & Chr(13) & _
    "Program, and your songs will be there!"
End Sub

Private Sub OK_Click()
 Unload Me
End Sub

Private Sub OtherButton_Click()
 If (OtherButton.Caption = "Fast Help") Then
    OtherButton.Caption = "Other Options"
    Form_Load
    Exit Sub
 End If
 If (OtherButton.Caption = "The Song List") Then
    OtherButton.Caption = "Fast Help"
    HelpText.Caption = "Welcome to Gorilla!" & Chr(13) & _
     Chr(13) & _
     "The song list shows all songs and folders ready to " & _
     "process, as well as songs already processed." & Chr(13) & _
     "If you exit from the 'Busy..' page before all songs " & _
     "have been completed, bot processed and un-processed " & _
     "Songs will eb displayed." & Chr(13) & _
     Chr(13) & _
     "Songs can have the following possible states:" & Chr(13) & _
     "[      ]  - Not Processed" & Chr(13) & _
     "[TURBO ]  - Already processed, but you want to process it again." & Chr(13) & _
     "            This allows you to skip the slowest part of processing" & Chr(13) & _
     "[ BUSY ]  - (Only in 'Busy..' screen) Song isbeig processed" & Chr(13) & _
     "[FAILED]  - DancingMonkeys could not create a stepfile for this song" & Chr(13) & _
     "[------]  - A directory that has been expanded. (You can safely" & Chr(13) & _
     "            Remove this, it only exists for your refrence)"
    Exit Sub
 End If
 OtherButton.Caption = "The Song List"
 HelpText.Caption = "Welcome to Gorilla!" & Chr(13) & _
    Chr(13) & _
    "You can also select songs to process by clicking on " & _
    "[Add Song] or select entire folders with [Add Directory]" & _
    Chr(13) & _
    "You can customize how DancingMonkeys will process " & _
    "each song (or folder) by clicking on [Customize] while " & _
    "the song (or folder) is hilighted at the top of Gorilla. " & _
    "However the defaults should work pretty well, so you " & _
    "probably won't need to change anything" & Chr(13) & _
    Chr(13) & _
    "Simply hovering the mouse over most options will " & _
    "Show a tooltip."
End Sub
