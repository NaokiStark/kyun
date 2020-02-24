VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "List"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"

Private fSong As Song
Public cSong As Song
Private lSong As Song
Public Count As Integer
Private UIDTrack As Integer
Public CanRedoAll As Boolean

Public Property Let UID(vData As Integer)
    cSong.UID = vData
End Property


Public Property Get UID() As Integer
    UID = cSong.UID
End Property

Public Property Let SongName(vData As String)
    cSong.Name = vData
End Property


Public Property Get SongName() As String
    SongName = cSong.Name
End Property

Public Property Let Default(vData As Boolean)
    cSong.Default = vData
End Property


Public Property Get Default() As Boolean
    Default = cSong.Default
End Property

Public Property Let CopyArt(vData As Boolean)
    cSong.CopyArt = vData
End Property


Public Property Get CopyArt() As Boolean
    CopyArt = cSong.CopyArt
End Property

Public Property Let Turbo(vData As Integer)
    cSong.Turbo = vData
End Property


Public Property Get Turbo() As Integer
    Turbo = cSong.Turbo
End Property

Public Property Let State(vData As Integer)
    cSong.State = vData
End Property


Public Property Get State() As Integer
    State = cSong.State
End Property

Public Property Let OutDir(vData As String)
    cSong.OutDir = vData
End Property


Public Property Get OutDir() As String
    OutDir = cSong.OutDir
End Property

Public Property Let IsDir(vData As Boolean)
    cSong.IsDir = vData
End Property


Public Property Get IsDir() As Boolean
    IsDir = cSong.IsDir
End Property

Public Property Let Delve(vData As Boolean)
    cSong.Delve = vData
End Property


Public Property Get Delve() As Boolean
    Delve = cSong.Delve
End Property

Public Property Let DirToGroup(vData As Boolean)
    cSong.DirToGroup = vData
End Property


Public Property Get DirToGroup() As Boolean
    DirToGroup = cSong.DirToGroup
End Property

Public Property Let Group(vData As String)
    cSong.Group = vData
End Property


Public Property Get Group() As String
    Group = cSong.Group
End Property

Public Property Let NoID(vData As Boolean)
    cSong.NoID = vData
End Property


Public Property Get NoID() As Boolean
    NoID = cSong.NoID
End Property

Public Property Let CreditState(vData As Integer)
    cSong.CreditState = vData
End Property


Public Property Get CreditState() As Integer
    CreditState = cSong.CreditState
End Property

Public Property Let Credit(vData As String)
    cSong.Credit = vData
End Property


Public Property Get Credit() As String
    Credit = cSong.Credit
End Property

Public Property Let OutputType(vData As Integer)
    cSong.OutputType = vData
End Property


Public Property Get OutputType() As Integer
    OutputType = cSong.OutputType
End Property

Public Property Let NoStops(vData As Boolean)
    cSong.NoStops = vData
End Property

Public Property Get NoStops() As Boolean
    NoStops = cSong.NoStops
End Property

Public Property Let ErrorStops(vData As Integer)
    cSong.ErrorStops = vData
End Property

Public Property Get ErrorStops() As Integer
    ErrorStops = cSong.ErrorStops
End Property

Public Property Let Length(vData As Integer)
    cSong.Length = vData
End Property


Public Property Get Length() As Integer
    Length = cSong.Length
End Property

Public Property Let Fade(vData As Integer)
    cSong.Fade = vData
End Property


Public Property Get Fade() As Integer
    Fade = cSong.Fade
End Property

Public Property Let Confidence(vData As Integer)
    cSong.Confidence = vData
End Property


Public Property Get Confidence() As Integer
    Confidence = cSong.Confidence
End Property

Public Property Let BeatPerMeasure(vData As Integer)
    cSong.BeatPerMeasure = vData
End Property


Public Property Get BeatPerMeasure() As Integer
    BeatPerMeasure = cSong.BeatPerMeasure
End Property

Public Property Let BPM_Min(vData As Integer)
    cSong.BPM_Min = vData
End Property


Public Property Get BPM_Min() As Integer
    BPM_Min = cSong.BPM_Min
End Property

Public Property Let BPM_Max(vData As Integer)
    cSong.BPM_Max = vData
End Property


Public Property Get BPM_Max() As Integer
    BPM_Max = cSong.BPM_Max
End Property

Public Property Let GapAdjust(vData As Single)
    cSong.GapAdjust = vData
End Property

Public Property Get GapAdjust() As Single
    GapAdjust = cSong.GapAdjust
End Property

Public Property Let AddToSM(vData As Boolean)
    cSong.AddToSM = vData
End Property

Public Property Get AddToSM() As Boolean
  AddToSM = cSong.AddToSM
End Property

Public Property Let AddToDWI(vData As Boolean)
    cSong.AddToDWI = vData
End Property

Public Property Get AddToDWI() As Boolean
  AddToDWI = cSong.AddToDWI
End Property

Public Property Let Basic(vData As Integer)
    cSong.Basic = vData
End Property


Public Property Get Basic() As Integer
    Basic = cSong.Basic
End Property

Public Property Let Medium(vData As Integer)
    cSong.Medium = vData
End Property


Public Property Get Medium() As Integer
    Medium = cSong.Medium
End Property

Public Property Let Hard(vData As Integer)
    cSong.Hard = vData
End Property


Public Property Get Hard() As Integer
    Hard = cSong.Hard
End Property

Public Sub FirstSong()
 Set cSong = fSong
End Sub

Public Sub LastSong()
 Set cSong = lSong
End Sub

Public Sub NextSong()
 If Not cSong Is Nothing Then _
  Set cSong = cSong.NextSong
End Sub

Public Sub PrevSong()
 If Not cSong Is Nothing Then _
  Set cSong = cSong.PrevSong
End Sub

Private Sub Class_Initialize()
 Set cSong = Nothing
 Set fSong = Nothing
 Set lSong = Nothing
 'set default
 Count = 0
End Sub

Public Sub Push(ByVal newName As String)
 Count = Count + 1
 UIDTrack = UIDTrack + 1
 Set cSong = New Song
 cSong.Name = newName
 cSong.Default = True
 cSong.UID = UIDTrack
 Set cSong.PrevSong = lSong
 If Not (lSong Is Nothing) Then _
    Set lSong.NextSong = cSong
 If (fSong Is Nothing) Then _
    Set fSong = cSong
 Set lSong = cSong
End Sub

Public Function Pop() As Song
 If Count = 0 Then
  Set Pop = Nothing
  Exit Function
 End If
 Count = Count - 1
 Set Pop = lSong
 If Not lSong.PrevSong Is Nothing Then _
  Set lSong.PrevSong.NextSong = Nothing
 If cSong.UID = lSong.UID Then Set cSong = lSong.PrevSong
 Set lSong = lSong.PrevSong
 If lSong Is Nothing Then Set fSong = lSong
End Function

Public Sub Unshift(ByVal newName As String)
 Count = Count + 1
 UIDTrack = UIDTrack + 1
 Set cSong = New Song
 cSong.Name = newName
 cSong.Default = True
 cSong.UID = UIDTrack
 Set cSong.NextSong = fSong
 If Not (fSong Is Nothing) Then _
  Set fSong.PrevSong = cSong
 If (lSong Is Nothing) Then _
  Set lSong = cSong
 Set fSong = cSong
End Sub

Public Function Shift() As Song
 If Count = 0 Then
  Set Pop = Nothing
  Exit Function
 End If
 Count = Count - 1
 Set Pop = fSong
 If Not fSong.NextSong Is Nothing Then _
  Set fSong.NextSong.PrevSong = Nothing
 If cSong.UID = fSong.UID Then Set cSong = fSong.NextSong
 Set fSong = fSong.NextSong
End Function

Public Sub Insert(ByVal newName As String)
 Dim Tmp As Song
 Count = Count + 1
 UIDTrack = UIDTrack + 1
 Set Tmp = New Song
 Tmp.Name = newName
 Tmp.Default = True
 Tmp.UID = UIDTrack
 If cSong Is Nothing Then
  Set fSong = Tmp
  Set lSong = Tmp
  Set cSong = Tmp
 Else
  Set Tmp.NextSong = cSong
  Set Tmp.PrevSong = cSong.PrevSong
  If Not Tmp.PrevSong Is Null Then Set Tmp.PrevSong.NextSong = Tmp
  Set cSong.PrevSong = Tmp
  Set cSong = Tmp
  If cSong.PrevSong Is Nothing Then Set fSong = cSong
  If cSong.NextSong Is Nothing Then Set lSong = cSong
 End If
End Sub

Public Sub InsertAfter(ByVal newName As String)
 Dim Tmp As Song
 Count = Count + 1
 UIDTrack = UIDTrack + 1
 Set Tmp = New Song
 Tmp.Name = newName
 Tmp.Default = True
 Tmp.UID = UIDTrack
 If cSong Is Nothing Then
  Set fSong = Tmp
  Set lSong = Tmp
  Set cSong = Tmp
 Else
  Set Tmp.NextSong = cSong.NextSong
  Set Tmp.PrevSong = cSong
  If Not cSong.NextSong Is Nothing Then Set cSong.NextSong.PrevSong = Tmp
  Set cSong.NextSong = Tmp
  Set cSong = Tmp
  If cSong.PrevSong Is Nothing Then Set fSong = cSong
  If cSong.NextSong Is Nothing Then Set lSong = cSong
 End If
End Sub

Public Sub Delete()
 Dim old As Song
 Set old = cSong
 If Not old.PrevSong Is Nothing Then
  Set old.PrevSong.NextSong = old.NextSong
  Set cSong = old.PrevSong
 Else
  Set cSong = old.NextSong
 End If
 If Not old.NextSong Is Nothing Then
  Set old.NextSong.PrevSong = old.PrevSong
 End If
 If fSong.UID = old.UID Then Set fSong = cSong
 If lSong.UID = old.UID Then Set lSong = cSong
 Set old = Nothing
 Count = Count - 1
End Sub

Public Sub SetDefaults(vData As Song)
 With Defaults
  vData.State = .State
  vData.OutDir = .OutDir
  vData.IsDir = .IsDir
  vData.Delve = .Delve
  vData.CopyArt = .CopyArt
  vData.DirToGroup = .DirToGroup
  vData.Group = .Group
  vData.NoID = .NoID
  vData.CreditState = .CreditState
  vData.Credit = .Credit
  vData.OutputType = .OutputType
  vData.NoStops = .NoStops
  vData.ErrorStops = .ErrorStops
  vData.Length = .Length
  vData.Fade = .Fade
  vData.Confidence = .Confidence
  vData.BeatPerMeasure = .BeatPerMeasure
  vData.BPM_Min = .BPM_Min
  vData.BPM_Max = .BPM_Max
  vData.GapAdjust = .GapAdjust
  vData.AddToSM = .AddToSM
  vData.AddToDWI = .AddToDWI
  vData.Basic = .Basic
  vData.Medium = .Medium
  vData.Hard = .Hard
 End With
End Sub

Public Sub Copy(vData As Song)
 With vData
  cSong.State = .State
  cSong.OutDir = .OutDir
  cSong.IsDir = .IsDir
  cSong.Delve = .Delve
  cSong.CopyArt = .CopyArt
  cSong.DirToGroup = .DirToGroup
  cSong.Group = .Group
  cSong.NoID = .NoID
  cSong.CreditState = .CreditState
  cSong.Credit = .Credit
  cSong.OutputType = .OutputType
  cSong.NoStops = .NoStops
  cSong.ErrorStops = .ErrorStops
  cSong.Length = .Length
  cSong.Fade = .Fade
  cSong.Confidence = .Confidence
  cSong.BeatPerMeasure = .BeatPerMeasure
  cSong.BPM_Min = .BPM_Min
  cSong.BPM_Max = .BPM_Max
  cSong.GapAdjust = .GapAdjust
  cSong.AddToSM = .AddToSM
  cSong.AddToDWI = .AddToDWI
  cSong.Basic = .Basic
  cSong.Medium = .Medium
  cSong.Hard = .Hard
 End With
End Sub

Public Sub Populate(sList As ListBox, Flag As Integer)
 Dim Counter As Integer
 Dim tSong As Song
 CanRedoAll = False
 Counter = 0
 Set tSong = fSong
 Do Until tSong Is Nothing
  If Flag Then
   If tSong.State = 0 Then
    If tSong.Turbo = 2 Then
     sList.AddItem "[TURBO ]  " + tSong.Name, Counter
    Else
     sList.AddItem "[      ]  " + tSong.Name, Counter
    End If
   ElseIf tSong.State = 1 Then
    sList.AddItem "[ BUSY ]  " + tSong.Name, Counter
   ElseIf tSong.State = 2 Then
    sList.AddItem "[  OK  ]  " + tSong.Name, Counter
    CanRedoAll = True
   ElseIf tSong.State = 3 Then
    sList.AddItem "[FAILED]  " + tSong.Name, Counter
    CanRedoAll = True
    ElseIf tSong.State = 4 Then
    sList.AddItem "[------]  " + tSong.Name, Counter
    CanRedoAll = True
   End If
  Else
   sList.AddItem tSong.Name, Counter
  End If
  Counter = Counter + 1
  Set tSong = tSong.NextSong
 Loop
End Sub

