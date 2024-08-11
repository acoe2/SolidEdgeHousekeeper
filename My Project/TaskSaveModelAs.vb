﻿Option Strict On
Imports Microsoft.WindowsAPICodePack.Dialogs

Public Class TaskSaveModelAs

    Inherits Task

    Public Property NewFileTypeName As String  ' eg, 'Parasolid (*.xt)'
    Public Property SaveInOriginalDirectory As Boolean
    Public Property NewDir As String
    Public Property UseSubdirectoryFormula As Boolean
    Public Property Formula As String
    Public Property CropImage As Boolean
    Public Property ImageFileTypeNames As New List(Of String)
    Public Property HideConstructions As Boolean
    Public Property FitView As Boolean
    Public Property Isometric As Boolean
    Public Property Dimetric As Boolean
    Public Property Trimetric As Boolean
    Public Property ChangeViewStyle As Boolean
    Public Property ViewStyleName As String



    Enum ControlNames
        NewFileTypeName
        NewFileTypeLabel
        SaveInOriginalDirectory
        BrowseNewDir
        NewDir
        UseSubdirectoryFormula
        Formula
        CropImage
        HideConstructions
        FitView
        Isometric
        Dimetric
        Trimetric
        ChangeViewStyle
        ViewStyleName
        ViewStyleNameLabel
        HideOptions
    End Enum


    Public Sub New()
        Me.Name = Me.ToString.Replace("Housekeeper.", "")
        Me.Description = GenerateLabelText()
        Me.HelpText = GetHelpText()
        Me.RequiresSave = False
        Me.AppliesToAssembly = True
        Me.AppliesToPart = True
        Me.AppliesToSheetmetal = True
        Me.AppliesToDraft = False
        Me.HasOptions = True
        Me.HelpURL = GenerateHelpURL(Description)
        Me.Image = My.Resources.TaskSaveAs
        Me.Category = "Output"
        SetColorFromCategory(Me)

        GenerateTaskControl()
        TaskOptionsTLP = GenerateTaskOptionsTLP()
        Me.TaskControl.AddTaskOptionsTLP(TaskOptionsTLP)

        ' Options
        Me.NewFileTypeName = ""
        Me.SaveInOriginalDirectory = False
        Me.NewDir = ""
        Me.UseSubdirectoryFormula = False
        Me.Formula = ""
        Me.CropImage = False
        Me.HideConstructions = False
        Me.FitView = False
        Me.Isometric = False
        Me.Dimetric = False
        Me.Trimetric = False
        Me.ChangeViewStyle = False
        Me.ViewStyleName = ""

    End Sub

    Public Overrides Function Process(
        ByVal SEDoc As SolidEdgeFramework.SolidEdgeDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As Dictionary(Of Integer, List(Of String))

        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))

        ErrorMessage = InvokeSTAThread(
                               Of SolidEdgeFramework.SolidEdgeDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               Dictionary(Of Integer, List(Of String)))(
                                   AddressOf ProcessInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessage

    End Function

    Public Overrides Function Process(ByVal FileName As String) As Dictionary(Of Integer, List(Of String))

        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))

        Return ErrorMessage

    End Function

    Private Function ProcessInternal(
        ByVal SEDoc As SolidEdgeFramework.SolidEdgeDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As Dictionary(Of Integer, List(Of String))

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As Integer = 0
        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))

        Dim SupplementalErrorMessage As New Dictionary(Of Integer, List(Of String))

        Dim ImageExtensions As List(Of String) = {".bmp", ".jpg", ".png", ".tif"}.ToList

        Dim ExitMessage As String = ""

        Dim NewFilename As String = ""
        Dim NewExtension As String = ""
        Dim NewFileFormat As String = ""
        Dim Filename As String = SEDoc.FullName

        Dim Proceed As Boolean = True

        Dim UC As New UtilsCommon
        Dim DocType As String = UC.GetDocType(SEDoc)

        ' Configuration("ComboBoxSaveAsAssemblyFileType") format examples
        ' IGES (*.igs)
        ' Parasolid text (*.x_b)
        ' Copy (*.*)

        Dim IsSaveCopyAs As Boolean = Me.NewFileTypeName.ToLower.Contains("copy")

        If Not IsSaveCopyAs Then
            NewExtension = Me.NewFileTypeName
            NewExtension = NewExtension.Split("*"c)(1)  ' "Parasolid text (*.xt)" -> ".xt)"
            NewExtension = NewExtension.Split(")"c)(0)  ' "Parasolid text (*.xt)" -> ".xt"
        Else
            NewExtension = String.Format(".{0}", DocType)
        End If

        NewFileFormat = Me.NewFileTypeName
        NewFileFormat = NewFileFormat.Split("("c)(0)  ' "Parasolid text (*.xt)" -> "Parasolid text "

        Select Case DocType
            Case = "asm"
                Dim tmpSEDoc = CType(SEDoc, SolidEdgeAssembly.AssemblyDocument)

                Dim Members As SolidEdgeAssembly.AssemblyFamilyMembers
                Dim Member As SolidEdgeAssembly.AssemblyFamilyMember
                Dim msg As String = ""

                If (Not tmpSEDoc.IsFileFamilyByDocument) Or (IsSaveCopyAs) Then  ' SaveCopyAs doesn't need a new file for every member

                    NewFilename = GenerateNewFilename(SEDoc, NewExtension)

                    FileIO.FileSystem.CreateDirectory(System.IO.Path.GetDirectoryName(NewFilename))

                    Try
                        If Not ImageExtensions.Contains(NewExtension) Then  ' Saving as a model, not an image.
                            SupplementalErrorMessage = SaveAsModel(SEDoc, NewFilename, SEApp)
                            AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                        Else  ' Saving as image
                            SupplementalErrorMessage = SaveAsImage(SEDoc, NewFilename, SEApp, Configuration, NewExtension)
                            AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                        End If
                    Catch ex As Exception
                        ExitStatus = 1
                        ErrorMessageList.Add(String.Format("Error saving {0}", NewFilename))
                    End Try

                Else
                    Members = tmpSEDoc.AssemblyFamilyMembers
                    For Each Member In Members
                        Members.ActivateMember(Member.MemberName)

                        NewFilename = GenerateNewFilename(SEDoc, NewExtension, Member.MemberName)

                        FileIO.FileSystem.CreateDirectory(System.IO.Path.GetDirectoryName(NewFilename))

                        Try
                            If Not ImageExtensions.Contains(NewExtension) Then  ' Saving as a model, not an image.
                                SupplementalErrorMessage = SaveAsModel(SEDoc, NewFilename, SEApp)
                                AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                            Else  ' Saving as image
                                SupplementalErrorMessage = SaveAsImage(SEDoc, NewFilename, SEApp, Configuration, NewExtension)
                                AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                            End If
                        Catch ex As Exception
                            ExitStatus = 1
                            ErrorMessageList.Add(String.Format("Error saving {0}", NewFilename))
                        End Try

                    Next
                End If


            Case = "par"
                'Dim tmpSEDoc = CType(SEDoc, SolidEdgePart.PartDocument)

                NewFilename = GenerateNewFilename(SEDoc, NewExtension)

                FileIO.FileSystem.CreateDirectory(System.IO.Path.GetDirectoryName(NewFilename))

                Try
                    If Not ImageExtensions.Contains(NewExtension) Then  ' Saving as a model, not an image.
                        SupplementalErrorMessage = SaveAsModel(SEDoc, NewFilename, SEApp)
                        AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                    Else  ' Saving as image
                        SupplementalErrorMessage = SaveAsImage(SEDoc, NewFilename, SEApp, Configuration, NewExtension)
                        AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                    End If
                Catch ex As Exception
                    ExitStatus = 1
                    ErrorMessageList.Add(String.Format("Error saving {0}", NewFilename))
                End Try


            Case = "psm"
                Dim tmpSEDoc = CType(SEDoc, SolidEdgePart.SheetMetalDocument)

                'Dim DraftTasks As New DraftTasks

                Dim DraftFilename As String
                Dim SEDraftDoc As SolidEdgeDraft.DraftDocument = Nothing

                NewFilename = GenerateNewFilename(SEDoc, NewExtension)

                FileIO.FileSystem.CreateDirectory(System.IO.Path.GetDirectoryName(NewFilename))

                Try
                    If Not ImageExtensions.Contains(NewExtension) Then  ' Saving as a model, not an image.

                        If NewExtension = ".dxf" Then
                            SupplementalErrorMessage = SaveAsFlatDXF(tmpSEDoc, NewFilename, SEApp)
                            AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                        ElseIf NewExtension = ".pdf" Then
                            DraftFilename = System.IO.Path.ChangeExtension(tmpSEDoc.FullName, ".dft")
                            If Not FileIO.FileSystem.FileExists(DraftFilename) Then
                                ExitStatus = 1
                                ErrorMessageList.Add(String.Format("Draft document not found '{0}'", DraftFilename))
                            Else
                                SEDraftDoc = CType(SEApp.Documents.Open(DraftFilename), SolidEdgeDraft.DraftDocument)
                                SEApp.DoIdle()

                                SupplementalErrorMessage = SaveAsDrawing(SEDraftDoc, NewFilename, SEApp)
                                AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                            End If

                        Else
                            SupplementalErrorMessage = SaveAsModel(SEDoc, NewFilename, SEApp)
                            AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)
                        End If


                    Else  ' Saving as image
                        SupplementalErrorMessage = SaveAsImage(SEDoc, NewFilename, SEApp, Configuration, NewExtension)
                        AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)

                    End If
                Catch ex As Exception
                    ExitStatus = 1
                    ErrorMessageList.Add(String.Format("Error saving {0}", NewFilename))
                End Try

                Try
                    If Not SEDraftDoc Is Nothing Then
                        SEDraftDoc.Close(False)
                        SEApp.DoIdle()
                    End If
                Catch ex As Exception
                End Try

                'Case = "dft"
                '    Dim tmpSEDoc = CType(SEDoc, SolidEdgeDraft.DraftDocument)

                '    NewFilename = GenerateNewFilename(SEDoc, NewExtension)

                '    FileIO.FileSystem.CreateDirectory(System.IO.Path.GetDirectoryName(NewFilename))

                '    If Me.AddWatermark Then
                '        SupplementalErrorMessage = AddWatermarkToSheets(tmpSEDoc, NewFilename, SEApp)
                '        AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)
                '    End If

                '    SupplementalErrorMessage = SaveAsDrawing(tmpSEDoc, NewFilename, SEApp)
                '    AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)
            Case Else
                MsgBox(String.Format("{0} DocType '{1}' not recognized", Me.Name, DocType))

        End Select

        ErrorMessage(ExitStatus) = ErrorMessageList
        Return ErrorMessage
    End Function


    Private Function GenerateNewFilename(
        SEDoc As SolidEdgeFramework.SolidEdgeDocument,
        NewExtension As String,
        Optional Suffix As String = "") As String

        ' Example conversions
        ' NewExtension: ".stp"
        ' Suffix: Not supplied
        ' "C:\Projects\part.par" -> "C:\Projects\part.stp"

        ' NewExtension: ".png"
        ' Suffix: "Member1"
        ' "C:\Projects\assembly.asm!Master" -> "C:\Projects\assembly-Member1.png"

        Dim NewFilename As String = ""
        Dim NewDirectory As String = ""
        Dim NewSubDirectory As String = ""

        Dim s As String

        Dim OldFullFilename As String = ""   ' "C:\Projects\part.par", "C:\Projects\assembly.asm!Master"
        Dim OldDirectoryName As String = ""  ' "C:\Projects"
        Dim OldFilenameWOExt As String = ""  ' "part"
        Dim OldExtension As String = ""      ' ".par"

        Dim SplitDict As New Dictionary(Of String, String)

        Dim UC As New UtilsCommon
        Dim UFC As New UtilsFilenameCharmap()

        OldFullFilename = UC.SplitFOAName(SEDoc.FullName)("Filename")
        'If OldFullFilename.Contains("!") Then
        '    OldFullFilename = TC.SplitFOAName(OldFullFilename)("Filename")
        'End If

        OldDirectoryName = System.IO.Path.GetDirectoryName(OldFullFilename)
        OldFilenameWOExt = System.IO.Path.GetFileNameWithoutExtension(OldFullFilename)
        OldExtension = IO.Path.GetExtension(OldFullFilename)

        If Me.SaveInOriginalDirectory Then
            If Suffix = "" Then
                NewFilename = System.IO.Path.ChangeExtension(OldFullFilename, NewExtension)
            Else
                NewFilename = String.Format("{0}\{1}-{2}{3}", OldDirectoryName, OldFilenameWOExt, Suffix, NewExtension)
            End If
        Else
            NewDirectory = Me.NewDir

            If Not Me.UseSubdirectoryFormula Then
                If Suffix = "" Then
                    NewFilename = String.Format("{0}\{1}{2}", NewDirectory, OldFilenameWOExt, NewExtension)
                Else
                    NewFilename = String.Format("{0}\{1}-{2}{3}", NewDirectory, OldFilenameWOExt, Suffix, NewExtension)
                End If
            Else
                NewSubDirectory = UC.SubstitutePropertyFormula(SEDoc, Nothing, SEDoc.FullName, Me.Formula, ValidFilenameRequired:=True)

                If Suffix = "" Then
                    NewFilename = String.Format("{0}\{1}\{2}{3}", NewDirectory, NewSubDirectory, OldFilenameWOExt, NewExtension)
                Else
                    NewFilename = String.Format("{0}\{1}\{2}-{3}{4}", NewDirectory, NewSubDirectory, OldFilenameWOExt, Suffix, NewExtension)
                End If
            End If
        End If

        s = System.IO.Path.GetFileNameWithoutExtension(NewFilename)
        NewFilename = NewFilename.Replace(s, UFC.SubstituteIllegalCharacters(s))

        Return NewFilename
    End Function

    Private Function SaveAsDrawing(
        SEDoc As SolidEdgeDraft.DraftDocument,
        NewFilename As String,
        SEApp As SolidEdgeFramework.Application
        ) As Dictionary(Of Integer, List(Of String))

        Dim ExitStatus As Integer = 0
        Dim ErrorMessageList As New List(Of String)
        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))

        Dim SaveAsPDFOptions As SolidEdgeFramework.ApplicationGlobalConstants
        SaveAsPDFOptions = SolidEdgeFramework.ApplicationGlobalConstants.seApplicationGlobalDraftSaveAsPDFSheetOptions

        Try
            If Not Me.NewFileTypeName.ToLower.Contains("copy") Then
                If Not Me.NewFileTypeName.ToLower.Contains("pdf per sheet") Then
                    SEDoc.SaveAs(NewFilename)
                    SEApp.DoIdle()

                Else  ' Save as one pdf file per sheet

                    'Dim PreviousSetting As Object = Nothing
                    'Dim SheetList As New List(Of SolidEdgeDraft.Sheet)
                    'Dim Sheet As SolidEdgeDraft.Sheet
                    'Dim SheetName As String
                    'Dim tmpNewFilename As String

                    '' seApplicationGlobalDraftSaveAsPDFSheetOptions (same mapping as SolidEdgeConstants.DraftSaveAsPDFSheetOptionsConstants)
                    '' 0: Active sheet only
                    '' 1: All sheets
                    '' 2: Sheets: (sheet number and/or ranges)

                    'SEApp.GetGlobalParameter(SaveAsPDFOptions, PreviousSetting)
                    'SEApp.SetGlobalParameter(SaveAsPDFOptions, 0)

                    'Dim TC As New Task_Common
                    'SheetList = TC.GetSheets(SEDoc, "Working")

                    'tmpNewFilename = NewFilename

                    'Dim FCD As New FilenameCharmapDoctor

                    'For Each Sheet In SheetList
                    '    Sheet.Activate()

                    '    SheetName = String.Format("-{0}", Sheet.Name)
                    '    SheetName = FCD.SubstituteIllegalCharacters(SheetName)
                    '    If Me.PDFPerSheetSuppressSheetname Then
                    '        If SheetList.Count = 1 Then
                    '            SheetName = ""
                    '        End If

                    '    End If

                    '    NewFilename = tmpNewFilename.Substring(0, tmpNewFilename.Count - 4)
                    '    NewFilename = String.Format("{0}{1}.pdf", NewFilename, SheetName)
                    '    SEDoc.SaveAs(NewFilename)
                    '    SEApp.DoIdle()

                    'Next

                    'SEApp.SetGlobalParameter(SaveAsPDFOptions, PreviousSetting)

                End If
            Else
                If Me.SaveInOriginalDirectory Then
                    SEDoc.SaveCopyAs(NewFilename)
                    SEApp.DoIdle()
                Else
                    ExitStatus = 1
                    ErrorMessageList.Add("Can not SaveCopyAs to the original directory")
                End If
            End If

        Catch ex As Exception
            ExitStatus = 1
            ErrorMessageList.Add(String.Format("Error saving file {0}", NewFilename))
        End Try

        Return ErrorMessage
    End Function

    Private Function SaveAsFlatDXF(
        SEDoc As SolidEdgePart.SheetMetalDocument,
        NewFilename As String,
        SEApp As SolidEdgeFramework.Application
        ) As Dictionary(Of Integer, List(Of String))

        Dim ExitStatus As Integer = 0
        Dim ErrorMessageList As New List(Of String)
        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))

        Dim SupplementalErrorMessage As New Dictionary(Of Integer, List(Of String))

        Dim Models As SolidEdgePart.Models

        Models = SEDoc.Models
        Try
            Models.SaveAsFlatDXFEx(NewFilename, Nothing, Nothing, Nothing, True)
            SEApp.DoIdle()
        Catch ex As Exception
            ExitStatus = 1
            ErrorMessageList.Add(String.Format("Error saving '{0}'.  Please verify a flat pattern is present.", NewFilename))
        End Try

        Return ErrorMessage
    End Function

    Private Function SaveAsModel(
        SEDoc As SolidEdgeFramework.SolidEdgeDocument,
        NewFilename As String,
        SEApp As SolidEdgeFramework.Application
        ) As Dictionary(Of Integer, List(Of String))

        Dim ExitStatus As Integer = 0
        Dim ErrorMessageList As New List(Of String)
        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))

        If Not Me.NewFileTypeName.ToLower.Contains("copy") Then
            SEDoc.SaveAs(NewFilename)
            SEApp.DoIdle()
        Else
            If Not Me.SaveInOriginalDirectory Then
                SEDoc.SaveCopyAs(NewFilename)
                SEApp.DoIdle()
            Else
                ExitStatus = 1
                ErrorMessageList.Add("Can not SaveCopyAs to the original directory")
            End If
        End If

        ErrorMessage(ExitStatus) = ErrorMessageList
        Return ErrorMessage
    End Function

    Private Function SaveAsImage(
        SEDoc As SolidEdgeFramework.SolidEdgeDocument,
        NewFilename As String,
        SEApp As SolidEdgeFramework.Application,
        Configuration As Dictionary(Of String, String),
        NewExtension As String
        ) As Dictionary(Of Integer, List(Of String))

        Dim ExitStatus As Integer = 0
        Dim ErrorMessageList As New List(Of String)
        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))
        Dim SupplementalErrorMessage As New Dictionary(Of Integer, List(Of String))

        Dim ExitMessage As String

        Dim Window As SolidEdgeFramework.Window
        Dim View As SolidEdgeFramework.View

        Dim UC As New UtilsCommon

        If Me.HideConstructions Then
            Dim TaskHideConstructions As New TaskHideConstructions
            SupplementalErrorMessage = TaskHideConstructions.Process(SEDoc, Configuration, SEApp)
            AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)
        End If

        If Me.FitView Then
            Dim TaskFitView As New TaskFitView
            TaskFitView.Isometric = Me.Isometric
            TaskFitView.Dimetric = Me.Dimetric
            TaskFitView.Trimetric = Me.Trimetric
            SupplementalErrorMessage = TaskFitView.Process(SEDoc, Configuration, SEApp)
            AddSupplementalErrorMessage(ExitStatus, ErrorMessageList, SupplementalErrorMessage)
        End If

        Window = CType(SEApp.ActiveWindow, SolidEdgeFramework.Window)
        View = Window.View

        If Me.ChangeViewStyle Then

            Try
                Dim ViewStyles As SolidEdgeFramework.ViewStyles = Nothing
                Dim ViewStyle As SolidEdgeFramework.ViewStyle = Nothing

                Select Case UC.GetDocType(SEDoc)
                    Case "asm"
                        Dim tmpSEDoc = CType(SEDoc, SolidEdgeAssembly.AssemblyDocument)
                        ViewStyles = CType(tmpSEDoc.ViewStyles, SolidEdgeFramework.ViewStyles)
                    Case "par"
                        Dim tmpSEDoc = CType(SEDoc, SolidEdgePart.PartDocument)
                        ViewStyles = CType(tmpSEDoc.ViewStyles, SolidEdgeFramework.ViewStyles)
                    Case "psm"
                        Dim tmpSEDoc = CType(SEDoc, SolidEdgePart.SheetMetalDocument)
                        ViewStyles = CType(tmpSEDoc.ViewStyles, SolidEdgeFramework.ViewStyles)
                End Select

                For Each VS As SolidEdgeFramework.ViewStyle In ViewStyles
                    If VS.StyleName = ViewStyleName Then
                        ViewStyle = VS
                        Exit For
                    End If
                Next

                View.ViewStyle = ViewStyle
                View.Update()
                SEApp.DoIdle()

            Catch ex As Exception
                ExitStatus = 1
                ErrorMessageList.Add(String.Format("Error changing to view style '{0}'", ViewStyleName))
            End Try
        End If

        If Not NewExtension = ".png" Then
            View.SaveAsImage(NewFilename)
        Else
            ExitMessage = UC.SaveAsPNG(View, NewFilename)
            If Not ExitMessage = "" Then
                ExitStatus = 1
                ErrorMessageList.Add(ExitMessage)
            End If
        End If

        If Me.CropImage Then
            ExitMessage = DoCropImage(SEDoc, NewFilename, NewExtension, Window.Height, Window.Width)
            If Not ExitMessage = "" Then
                ExitStatus = 1
                ErrorMessageList.Add(ExitMessage)
            End If
        End If

        ErrorMessage(ExitStatus) = ErrorMessageList
        Return ErrorMessage
    End Function

    Private Function GetNewFileTypeNames() As List(Of String)
        Dim NewFileTypeNames As New List(Of String)

        NewFileTypeNames.Add("Step (*.stp)")
        NewFileTypeNames.Add("IGES (*.igs)")
        NewFileTypeNames.Add("Parasolid Text (*.x_t)")
        NewFileTypeNames.Add("Parasolid Binary (*.x_b)")
        NewFileTypeNames.Add("OBJ (*.obj)")
        NewFileTypeNames.Add("STL (*.stl)")
        NewFileTypeNames.Add("JT Document (*.jt)")
        NewFileTypeNames.Add("bmp (*.bmp)")
        NewFileTypeNames.Add("jpg (*.jpg)")
        NewFileTypeNames.Add("png (*.png)")
        NewFileTypeNames.Add("tif (*.tif)")
        NewFileTypeNames.Add("Copy (*.*)")
        NewFileTypeNames.Add("PDF Drawing (*.pdf)")
        NewFileTypeNames.Add("DXF Flat (*.dxf)")

        ImageFileTypeNames.Clear()
        ImageFileTypeNames.Add("bmp (*.bmp)")
        ImageFileTypeNames.Add("jpg (*.jpg)")
        ImageFileTypeNames.Add("png (*.png)")
        ImageFileTypeNames.Add("tif (*.tif)")



        Return NewFileTypeNames
    End Function

    Public Function DoCropImage(
        SEDoc As SolidEdgeFramework.SolidEdgeDocument,
        NewFilename As String,
        NewExtension As String,
        WindowH As Integer,
        WindowW As Integer
        ) As String

        Dim ModelX As Double
        Dim ModelY As Double
        Dim ModelZ As Double

        Dim ImageW As Double
        Dim ImageH As Double
        Dim ImageAspectRatio As Double

        Dim CropW As Integer
        Dim CropH As Integer

        Dim TempFilename As String

        Dim ExitMessage As String = ""

        Dim WindowAspectRatio As Double = WindowH / WindowW

        Dim UC As New UtilsCommon

        Dim Range As New List(Of Double)

        Range = UC.GetDocRange(SEDoc)
        ModelX = Range(0)
        ModelY = Range(1)
        ModelZ = Range(2)

        ImageW = 0.557 * ModelX + 0.830667 * ModelY
        ImageH = 0.325444 * ModelX + 0.217778 * ModelY + 0.920444 * ModelZ

        'If Configuration("RadioButtonPictorialViewIsometric").ToLower = "true" Then
        '    ImageW = 0.707 * ModelX + 0.707 * ModelY
        '    ImageH = 0.40833 * ModelX + 0.40833 * ModelY + 0.81689 * ModelZ
        'ElseIf Configuration("RadioButtonPictorialViewDimetric").ToLower = "true" Then
        '    ImageW = 0.9356667 * ModelX + 0.353333 * ModelY
        '    ImageH = 0.117222 * ModelX + 0.311222 * ModelY + 0.942444 * ModelZ
        'Else
        '    ImageW = 0.557 * ModelX + 0.830667 * ModelY
        '    ImageH = 0.325444 * ModelX + 0.217778 * ModelY + 0.920444 * ModelZ
        'End If

        ImageAspectRatio = ImageH / ImageW

        If WindowAspectRatio > ImageAspectRatio Then
            CropH = CInt(Math.Round(WindowW * ImageAspectRatio))
            CropW = WindowW
        Else
            CropH = WindowH
            CropW = CInt(Math.Round(WindowH / ImageAspectRatio))
        End If

        TempFilename = NewFilename.Replace(NewExtension, String.Format("-Housekeeper{0}", NewExtension))

        Dim LocX = (WindowW - CropW) / 2
        Dim LocY = (WindowH - CropH) / 2
        Dim CropRect As New Rectangle(CInt(LocX), CInt(LocY), CropW, CropH)
        Dim OriginalImage = System.Drawing.Image.FromFile(NewFilename)
        Dim xCropImage = New Bitmap(CropRect.Width, CropRect.Height)

        Try

            Using grp = Graphics.FromImage(xCropImage)
                grp.DrawImage(OriginalImage, New Rectangle(0, 0, CropRect.Width, CropRect.Height), CropRect, GraphicsUnit.Pixel)
                OriginalImage.Dispose()
                xCropImage.Save(TempFilename)
            End Using

            Try
                System.IO.File.Delete(NewFilename)
                FileSystem.Rename(TempFilename, NewFilename)
            Catch ex As Exception
                ExitMessage = String.Format("Unable to save cropped image '{0}'", NewFilename)
            End Try

        Catch ex As Exception
            ExitMessage = String.Format("Unable to save cropped image '{0}'", TempFilename)
        End Try

        Return ExitMessage

    End Function



    Private Function GenerateTaskOptionsTLP() As ExTableLayoutPanel
        Dim tmpTLPOptions = New ExTableLayoutPanel

        Dim RowIndex As Integer
        Dim CheckBox As CheckBox
        Dim ComboBox As ComboBox
        Dim ComboBoxItems As List(Of String) = GetNewFileTypeNames()
        Dim TextBox As TextBox
        Dim Label As Label
        Dim Button As Button
        Dim ControlWidth As Integer = 150
        Dim NewFileTypeLabelText = "(Sheetmetal only: PDF, DXF)"

        'Dim IU As New InterfaceUtilities

        FormatTLPOptionsEx(tmpTLPOptions, "TLPOptions", 7, 75, 75)

        RowIndex = 0

        ComboBox = FormatOptionsComboBox(ControlNames.NewFileTypeName.ToString, ComboBoxItems, "DropDownList")
        ComboBox.Anchor = CType(AnchorStyles.Left + AnchorStyles.Right, AnchorStyles)
        AddHandler ComboBox.SelectedIndexChanged, AddressOf ComboBoxOptions_SelectedIndexChanged
        tmpTLPOptions.Controls.Add(ComboBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(ComboBox, 2)
        ControlsDict(ComboBox.Name) = ComboBox

        Label = FormatOptionsLabel(ControlNames.NewFileTypeLabel.ToString, NewFileTypeLabelText)
        tmpTLPOptions.Controls.Add(Label, 2, RowIndex)
        ControlsDict(Label.Name) = Label

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.SaveInOriginalDirectory.ToString, "Save in original directory")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        Button = FormatOptionsButton(ControlNames.BrowseNewDir.ToString, "Directory")
        AddHandler Button.Click, AddressOf ButtonOptions_Click
        tmpTLPOptions.Controls.Add(Button, 0, RowIndex)
        ControlsDict(Button.Name) = Button

        TextBox = FormatOptionsTextBox(ControlNames.NewDir.ToString, "")
        TextBox.BackColor = Color.FromArgb(255, 240, 240, 240)
        AddHandler TextBox.TextChanged, AddressOf TextBoxOptions_Text_Changed
        tmpTLPOptions.Controls.Add(TextBox, 1, RowIndex)
        tmpTLPOptions.SetColumnSpan(TextBox, 2)
        ControlsDict(TextBox.Name) = TextBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.UseSubdirectoryFormula.ToString, "Use subdirectory formula")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        TextBox = FormatOptionsTextBox(ControlNames.Formula.ToString, "")
        AddHandler TextBox.TextChanged, AddressOf TextBoxOptions_Text_Changed
        AddHandler TextBox.GotFocus, AddressOf TextBox_GotFocus
        tmpTLPOptions.Controls.Add(TextBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(TextBox, 3)
        TextBox.Visible = False
        ControlsDict(TextBox.Name) = TextBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.CropImage.ToString, "Crop image to model size")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.HideConstructions.ToString, "Hide constructions")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.FitView.ToString, "Fit view")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.Isometric.ToString, "Isometric")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.Dimetric.ToString, "Dimetric")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.Trimetric.ToString, "Trimetric")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.ChangeViewStyle.ToString, "Change view style")
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        CheckBox.Visible = False
        ControlsDict(CheckBox.Name) = CheckBox
        CheckBox.Visible = False

        RowIndex += 1

        Label = FormatOptionsLabel(ControlNames.ViewStyleNameLabel.ToString, "Style name")
        tmpTLPOptions.Controls.Add(Label, 0, RowIndex)
        Label.Visible = False
        ControlsDict(Label.Name) = Label

        TextBox = FormatOptionsTextBox(ControlNames.ViewStyleName.ToString, "")
        AddHandler TextBox.TextChanged, AddressOf TextBoxOptions_Text_Changed
        AddHandler TextBox.GotFocus, AddressOf TextBox_GotFocus
        tmpTLPOptions.Controls.Add(TextBox, 1, RowIndex)
        tmpTLPOptions.SetColumnSpan(TextBox, 2)
        TextBox.Visible = False
        ControlsDict(TextBox.Name) = TextBox

        RowIndex += 1

        CheckBox = FormatOptionsCheckBox(ControlNames.HideOptions.ToString, ManualOptionsOnlyString)
        'CheckBox.Checked = True
        AddHandler CheckBox.CheckedChanged, AddressOf CheckBoxOptions_Check_Changed
        tmpTLPOptions.Controls.Add(CheckBox, 0, RowIndex)
        tmpTLPOptions.SetColumnSpan(CheckBox, 3)
        ControlsDict(CheckBox.Name) = CheckBox

        Return tmpTLPOptions
    End Function

    Public Overrides Function CheckStartConditions(
        PriorErrorMessage As Dictionary(Of Integer, List(Of String))
        ) As Dictionary(Of Integer, List(Of String))

        Dim PriorExitStatus As Integer = PriorErrorMessage.Keys(0)

        Dim ErrorMessage As New Dictionary(Of Integer, List(Of String))
        Dim ExitStatus As Integer = 0
        Dim ErrorMessageList = PriorErrorMessage(PriorExitStatus)
        Dim Indent = "    "

        If Me.IsSelectedTask Then
            ' Check start conditions.
            If Not (Me.IsSelectedAssembly Or Me.IsSelectedPart Or Me.IsSelectedSheetmetal Or Me.IsSelectedDraft) Then
                If Not ErrorMessageList.Contains(Me.Description) Then
                    ErrorMessageList.Add(Me.Description)
                End If
                ExitStatus = 1
                ErrorMessageList.Add(String.Format("{0}Select at least one type of file to process", Indent))
            End If

            If Me.NewFileTypeName = "" Then
                If Not ErrorMessageList.Contains(Me.Description) Then
                    ErrorMessageList.Add(Me.Description)
                End If
                ExitStatus = 1
                ErrorMessageList.Add(String.Format("{0}Output file type not detected", Indent))
            End If

            If (Me.NewFileTypeName.ToLower.Contains("copy")) And (Me.SaveInOriginalDirectory) Then
                If Not ErrorMessageList.Contains(Me.Description) Then
                    ErrorMessageList.Add(Me.Description)
                End If
                ExitStatus = 1
                ErrorMessageList.Add(String.Format("{0}Cannot save copy to the original directory", Indent))
            End If

            If (Me.NewDir = "") And (Not Me.SaveInOriginalDirectory) Then
                If Not ErrorMessageList.Contains(Me.Description) Then
                    ErrorMessageList.Add(Me.Description)
                End If
                ExitStatus = 1
                ErrorMessageList.Add(String.Format("{0}Enter a valid directory", Indent))
            End If

            If (Me.Formula = "") And (Me.UseSubdirectoryFormula) Then
                If Not ErrorMessageList.Contains(Me.Description) Then
                    ErrorMessageList.Add(Me.Description)
                End If
                ExitStatus = 1
                ErrorMessageList.Add(String.Format("{0}Enter a subdirectory formula", Indent))
            End If

            'ImageFileTypeNames.Contains(Me.NewFileTypeName) And Me.FitView And Me.FitView
            If Me.FitView And ImageFileTypeNames.Contains(Me.NewFileTypeName) Then
                If Not (Me.Isometric Or Me.Dimetric Or Me.Trimetric) Then
                    If Not ErrorMessageList.Contains(Me.Description) Then
                        ErrorMessageList.Add(Me.Description)
                    End If
                    ExitStatus = 1
                    ErrorMessageList.Add(String.Format("{0}Select a view orientation", Indent))

                End If

            End If

            If Me.ChangeViewStyle And ImageFileTypeNames.Contains(Me.NewFileTypeName) Then
                If Me.ViewStyleName = "" Then
                    If Not ErrorMessageList.Contains(Me.Description) Then
                        ErrorMessageList.Add(Me.Description)
                    End If
                    ExitStatus = 1
                    ErrorMessageList.Add(String.Format("{0}Enter a view style name", Indent))
                End If
            End If

        End If

        If ExitStatus > 0 Then  ' Start conditions not met.
            ErrorMessage(ExitStatus) = ErrorMessageList
            Return ErrorMessage
        Else
            Return PriorErrorMessage
        End If

    End Function


    Public Sub ButtonOptions_Click(sender As System.Object, e As System.EventArgs)
        Dim Button = CType(sender, Button)
        Dim Name = Button.Name
        Dim TextBox As TextBox

        Select Case Name
            Case ControlNames.BrowseNewDir.ToString
                Dim tmpFolderDialog As New CommonOpenFileDialog
                tmpFolderDialog.IsFolderPicker = True

                If tmpFolderDialog.ShowDialog() = DialogResult.OK Then
                    Me.NewDir = tmpFolderDialog.FileName

                    TextBox = CType(ControlsDict(ControlNames.NewDir.ToString), TextBox)
                    TextBox.Text = Me.NewDir

                End If
            Case Else
                MsgBox(String.Format("{0} Name '{1}' not recognized", Me.Name, Name))

        End Select

    End Sub

    Public Sub CheckBoxOptions_Check_Changed(sender As System.Object, e As System.EventArgs)
        Dim Checkbox = CType(sender, CheckBox)
        Dim Name = Checkbox.Name

        Dim ParticipatingCheckBoxes As New List(Of CheckBox)
        ParticipatingCheckBoxes.Add(CType(ControlsDict(ControlNames.Isometric.ToString), CheckBox))
        ParticipatingCheckBoxes.Add(CType(ControlsDict(ControlNames.Dimetric.ToString), CheckBox))
        ParticipatingCheckBoxes.Add(CType(ControlsDict(ControlNames.Trimetric.ToString), CheckBox))

        Select Case Name

            Case ControlNames.SaveInOriginalDirectory.ToString
                Me.SaveInOriginalDirectory = Checkbox.Checked

                CType(ControlsDict(ControlNames.BrowseNewDir.ToString), Button).Visible = Not Me.SaveInOriginalDirectory
                CType(ControlsDict(ControlNames.NewDir.ToString), TextBox).Visible = Not Me.SaveInOriginalDirectory

                Dim CheckBox2 = CType(ControlsDict(ControlNames.UseSubdirectoryFormula.ToString), CheckBox)
                CheckBox2.Visible = Not Me.SaveInOriginalDirectory
                Dim tf = (CheckBox2.Checked) And (Not Me.SaveInOriginalDirectory)
                CType(ControlsDict(ControlNames.Formula.ToString), TextBox).Visible = tf


            Case ControlNames.UseSubdirectoryFormula.ToString
                Me.UseSubdirectoryFormula = Checkbox.Checked

                CType(ControlsDict(ControlNames.Formula.ToString), TextBox).Visible = Me.UseSubdirectoryFormula

            Case ControlNames.CropImage.ToString
                Me.CropImage = Checkbox.Checked

            Case ControlNames.HideConstructions.ToString
                Me.HideConstructions = Checkbox.Checked

            Case ControlNames.FitView.ToString
                Me.FitView = Checkbox.Checked

                CType(ControlsDict(ControlNames.Isometric.ToString), CheckBox).Visible = Me.FitView
                CType(ControlsDict(ControlNames.Dimetric.ToString), CheckBox).Visible = Me.FitView
                CType(ControlsDict(ControlNames.Trimetric.ToString), CheckBox).Visible = Me.FitView

            Case ControlNames.Isometric.ToString
                Me.Isometric = Checkbox.Checked
                If Me.Isometric Then
                    HandleMutuallyExclusiveCheckBoxes(Me.TaskOptionsTLP, Checkbox, ParticipatingCheckBoxes)
                End If

            Case ControlNames.Dimetric.ToString
                Me.Dimetric = Checkbox.Checked
                If Me.Dimetric Then
                    HandleMutuallyExclusiveCheckBoxes(Me.TaskOptionsTLP, Checkbox, ParticipatingCheckBoxes)
                End If

            Case ControlNames.Trimetric.ToString
                Me.Trimetric = Checkbox.Checked
                If Me.Trimetric Then
                    HandleMutuallyExclusiveCheckBoxes(Me.TaskOptionsTLP, Checkbox, ParticipatingCheckBoxes)
                End If

            Case ControlNames.ChangeViewStyle.ToString
                Me.ChangeViewStyle = Checkbox.Checked

                CType(ControlsDict(ControlNames.ViewStyleName.ToString), TextBox).Visible = Me.ChangeViewStyle
                CType(ControlsDict(ControlNames.ViewStyleNameLabel.ToString), Label).Visible = Me.ChangeViewStyle

            Case ControlNames.HideOptions.ToString
                HandleHideOptionsChange(Me, Me.TaskOptionsTLP, Checkbox)

            Case Else
                MsgBox(String.Format("{0} Name '{1}' not recognized", Me.Name, Name))
        End Select

    End Sub

    Public Sub ComboBoxOptions_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        Dim ComboBox = CType(sender, ComboBox)
        Dim Name = ComboBox.Name
        Dim CheckBox As CheckBox = Nothing
        Dim TextBox As TextBox = Nothing
        Dim Label As Label = Nothing

        Select Case Name
            Case ControlNames.NewFileTypeName.ToString
                Me.NewFileTypeName = ComboBox.Text

                CheckBox = CType(ControlsDict(ControlNames.CropImage.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName)

                CheckBox = CType(ControlsDict(ControlNames.HideConstructions.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName)

                CheckBox = CType(ControlsDict(ControlNames.FitView.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName)

                CheckBox = CType(ControlsDict(ControlNames.Isometric.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName) And Me.FitView

                CheckBox = CType(ControlsDict(ControlNames.Dimetric.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName) And Me.FitView

                CheckBox = CType(ControlsDict(ControlNames.Trimetric.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName) And Me.FitView

                CheckBox = CType(ControlsDict(ControlNames.ChangeViewStyle.ToString), CheckBox)
                CheckBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName)

                TextBox = CType(ControlsDict(ControlNames.ViewStyleName.ToString), TextBox)
                TextBox.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName) And Me.ChangeViewStyle

                Label = CType(ControlsDict(ControlNames.ViewStyleNameLabel.ToString), Label)
                Label.Visible = ImageFileTypeNames.Contains(Me.NewFileTypeName) And Me.ChangeViewStyle





            Case Else
                MsgBox(String.Format("{0} Name '{1}' not recognized", Me.Name, Name))

        End Select

    End Sub

    Public Sub TextBoxOptions_Text_Changed(sender As System.Object, e As System.EventArgs)
        Dim TextBox = CType(sender, TextBox)
        Dim Name = TextBox.Name

        Select Case Name
            Case ControlNames.Formula.ToString
                Me.Formula = TextBox.Text

            Case ControlNames.NewDir.ToString
                Me.NewDir = TextBox.Text

            Case ControlNames.ViewStyleName.ToString
                Me.ViewStyleName = TextBox.Text

            Case Else
                MsgBox(String.Format("{0} Name '{1}' not recognized", Me.Name, Name))
        End Select

    End Sub


    Private Function GetHelpText() As String
        Dim HelpString As String
        HelpString = "Exports the file to either a non-Solid Edge format, or the same format in a different directory. "

        HelpString += vbCrLf + vbCrLf + "![Save Model As](My%20Project/media/save_model_as.png)"

        HelpString += vbCrLf + vbCrLf + "Select the file type using the combobox. "
        HelpString += "Select the directory using the `Browse` button, "
        HelpString += "or check the `Original Directory` checkbox. "

        HelpString += vbCrLf + vbCrLf + "You can optionally create subdirectories using a formula similar to the Property Text Callout. For example: "
        HelpString += vbCrLf + "`Material %{System.Material} Gage %{System.Sheet Metal Gage}`. "
        HelpString += vbCrLf + "You can create nested subdirectories if desired. Simply use the `\` in the formula. For example: "
        HelpString += vbCrLf + "`Material %{System.Material}\Gage %{System.Sheet Metal Gage}`. "

        HelpString += vbCrLf + vbCrLf + "As illustrated in the examples, a `Property set`, either `System` or `Custom`, is required. "
        HelpString += "For more information, refer to the **Property Filter** section in this Readme file. "
        HelpString += vbCrLf + vbCrLf + "It is possible that a property contains a character that cannot be used in a file name. "
        HelpString += "If that happens, a replacement is read from `filename_charmap.txt` in the `Preferences` directory in the Housekeeper root folder. "
        HelpString += "You can/should edit it to change the replacement characters to your preference. "
        HelpString += "The file is created the first time you run Housekeeper.  For details, see the header comments in that file. "

        HelpString += vbCrLf + vbCrLf + "Sheetmetal files have two additional options -- `DXF Flat (*.dxf)` and `PDF Drawing (*.pdf)`. "
        HelpString += "The `DXF Flat` option saves the flat pattern of the sheet metal file. "
        HelpString += vbCrLf + vbCrLf + "The `PDF Drawing` option saves the drawing of the sheet metal file. "
        HelpString += "The drawing must have the same name as the model, and be in the same directory. "
        HelpString += "A more flexible option may be to use `Save Drawing As` command, "
        HelpString += "using a `Property Filter` if needed. "

        HelpString += vbCrLf + vbCrLf + "For image file formats there are additional options. "
        HelpString += "You can hide constructions and/or fit the view.  For Fit, choose an orientation, either `Isometric`, `Dimetric`, or `Trimetric`. "
        HelpString += "You can also crop images to the aspect ratio of the model, rather than the window. "
        HelpString += "The option is called `Crop image to model size`. "
        HelpString += "On tall skinny parts cropping works a little *too* well.  You might need to resort to Photoshop for those. "
        HelpString += "Finally, you can change the view style by selecting that option and entering its name in the textbox provided. "


        Return HelpString
    End Function


End Class
