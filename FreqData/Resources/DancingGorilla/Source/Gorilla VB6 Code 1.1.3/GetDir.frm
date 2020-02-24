VERSION 5.00
Begin VB.Form GetDir 
   BorderStyle     =   0  'None
   Caption         =   "GetDir"
   ClientHeight    =   90
   ClientLeft      =   0
   ClientTop       =   0
   ClientWidth     =   90
   LinkTopic       =   "Form1"
   ScaleHeight     =   90
   ScaleWidth      =   90
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows Default
End
Attribute VB_Name = "GetDir"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public ReturnTo As TextBox

Private Type BROWSEINFO
   hOwner           As Long
   pidlRoot         As Long
   pszDisplayName   As String
   lpszTitle        As String
   ulFlags          As Long
   lpfn             As Long
   lParam           As Long
   iImage           As Long
End Type

Private Const BIF_RETURNONLYFSDIRS = &H1
Private Const BIF_DONTGOBELOWDOMAIN = &H2
Private Const BIF_STATUSTEXT = &H4
Private Const BIF_RETURNFSANCESTORS = &H8
Private Const BIF_BROWSEFORCOMPUTER = &H1000
Private Const BIF_BROWSEFORPRINTER = &H2000
Private Const MAX_PATH As Long = 260

Private Declare Function SHGetPathFromIDList Lib "shell32" _
   Alias "SHGetPathFromIDListA" _
  (ByVal pidl As Long, _
   ByVal pszPath As String) As Long

Private Declare Function SHBrowseForFolder Lib "shell32" _
   Alias "SHBrowseForFolderA" _
  (lpBrowseInfo As BROWSEINFO) As Long

Private Declare Sub CoTaskMemFree Lib "ole32" _
   (ByVal pv As Long)
   



Private Sub Form_GotFocus()
  Me.Width = 0
  Me.Height = 0
  
  Dim bi As BROWSEINFO
  Dim pidl As Long
  Dim path As String
  Dim Pos As Long
      
  bi.hOwner = Me.hWnd
  bi.pidlRoot = 0&
  bi.lpszTitle = Me.Caption
  bi.ulFlags = BIF_RETURNONLYFSDIRS
    
  pidl = SHBrowseForFolder(bi)
   
  path = Space$(MAX_PATH)
    
  If SHGetPathFromIDList(ByVal pidl, ByVal path) Then
     Pos = InStr(path, Chr$(0))
     ReturnTo = Left(path, Pos - 1)
  End If
  Call CoTaskMemFree(pidl)
  Unload Me
End Sub
