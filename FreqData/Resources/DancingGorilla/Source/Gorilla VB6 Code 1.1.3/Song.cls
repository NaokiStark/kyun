VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Song"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit

Public UID As Integer
Public Name As String
Public Default As Boolean
Public State As Integer
Public CopyArt As Boolean
Public Turbo As Integer
Public OutDir As String
Public IsDir As Boolean
Public Delve As Boolean
Public DirToGroup As Boolean
Public Group As String
Public NoID As Boolean
Public CreditState As Integer
Public Credit As String
Public OutputType As Integer
Public NoStops As Boolean
Public ErrorStops As Integer
Public Length As Integer
Public Fade As Integer
Public Confidence As Integer
Public BeatPerMeasure As Integer
Public BPM_Min As Integer
Public BPM_Max As Integer
Public GapAdjust As Single
Public AddToSM As Boolean
Public AddToDWI As Boolean
Public Basic As Integer
Public Medium As Integer
Public Hard As Integer

Public NextSong As Song
Public PrevSong As Song


