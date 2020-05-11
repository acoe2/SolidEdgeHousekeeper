﻿Imports SolidEdgeCommunity

Public Class SheetmetalTasks
    Inherits IsolatedTaskProxy

    Public Function FailedOrWarnedFeatures(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf FailedOrWarnedFeaturesInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function FailedOrWarnedFeaturesInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim Models As SolidEdgePart.Models
        Dim Model As SolidEdgePart.Model
        Dim Features As SolidEdgePart.Features
        Dim Status As SolidEdgeConstants.FeatureStatusConstants

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Models = SEDoc.Models

        If (Models.Count > 0) And (Models.Count < 10) Then
            For Each Model In Models
                Features = Model.Features
                'Some Sync part features don't have a Status field.
                Try
                    For i As Integer = 0 To Features.Count - 1
                        Status = Features(i).Status
                        If Status.ToString = "igFeatureFailed" Then
                            ExitStatus = "1"
                            ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                        End If
                        If Status.ToString = "igFeatureWarned" Then
                            ExitStatus = "1"
                            ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                        End If
                    Next
                Catch ex As Exception
                End Try
            Next
        ElseIf Models.Count >= 10 Then
            ExitStatus = "1"
            ErrorMessage += "  " + Models.Count.ToString + " models in file exceeds maximum to process" + Chr(13)
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function


    Public Function SuppressedOrRolledBackFeatures(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf SuppressedOrRolledBackFeaturesInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function SuppressedOrRolledBackFeaturesInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim Models As SolidEdgePart.Models
        Dim Model As SolidEdgePart.Model
        Dim Features As SolidEdgePart.Features
        Dim Status As SolidEdgeConstants.FeatureStatusConstants

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Models = SEDoc.Models

        If (Models.Count > 0) And (Models.Count < 10) Then
            For Each Model In Models
                Features = Model.Features
                'Some Sync part features don't have a Status field.
                Try
                    For i As Integer = 0 To Features.Count - 1
                        Status = Features(i).Status
                        If Status.ToString = "igFeatureSuppressed" Then
                            ExitStatus = "1"
                            ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                        End If
                        If Status.ToString = "igFeatureRolledBack" Then
                            ExitStatus = "1"
                            ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                        End If
                    Next
                Catch ex As Exception
                End Try
            Next
        ElseIf Models.Count >= 10 Then
            ExitStatus = "1"
            ErrorMessage += "  " + Models.Count.ToString + " models in file exceeds maximum to process" + Chr(13)
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function


    Public Function UnderconstrainedProfiles(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf UnderconstrainedProfilesInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function UnderconstrainedProfilesInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim ProfileSets As SolidEdgePart.ProfileSets = SEDoc.ProfileSets
        Dim ProfileSet As SolidEdgePart.ProfileSet

        ' Not applicable to sync models
        If SEDoc.ModelingMode.ToString = "seModelingModeOrdered" Then
            For Each ProfileSet In ProfileSets
                If ProfileSet.IsUnderDefined Then
                    ExitStatus = "1"
                End If
            Next
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function


    Public Function FlatPatternMissingOrOutOfDate(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf FlatPatternMissingOrOutOfDateInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function FlatPatternMissingOrOutOfDateInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim Models As SolidEdgePart.Models = SEDoc.Models
        Dim FlatpatternModels As SolidEdgePart.FlatPatternModels = SEDoc.FlatPatternModels

        If FlatpatternModels.Count > 0 Then
            If Not FlatpatternModels.Item(1).IsUpToDate Then
                ExitStatus = "1"
                'ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
            End If
        Else
            ExitStatus = "1"
            'ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function


    Public Function MaterialNotInMaterialTable(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf MaterialNotInMaterialTableInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function MaterialNotInMaterialTableInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim MatTable As SolidEdgeFramework.MatTable

        Dim MaterialLibList As Object = Nothing
        Dim NumMaterialLibraries As Integer
        Dim MaterialList As Object = Nothing
        Dim NumMaterials As Integer

        Dim MatTableProps As Array
        Dim MatTableProp As SolidEdgeConstants.MatTablePropIndex
        Dim DocPropValue As Object = Nothing
        Dim LibPropValue As Object = Nothing

        'Dim ActiveMaterialLibrary As String = System.IO.Path.GetFileNameWithoutExtension(TextBoxActiveMaterialLibrary.Text)
        Dim ActiveMaterialLibrary As String = System.IO.Path.GetFileNameWithoutExtension(Configuration("TextBoxActiveMaterialLibrary"))
        Dim ActiveMaterialLibraryPresent As Boolean = False
        Dim CurrentMaterialName As String = ""
        Dim MatTableMaterial As Object
        Dim CurrentMaterialNameInLibrary As Boolean = False
        Dim CurrentMaterialMatchesLibMaterial As Boolean = True

        Dim msg As String

        Dim Models As SolidEdgePart.Models

        Models = SEDoc.Models

        If Models.Count > 0 Then

            MatTable = SEApp.GetMaterialTable()
            MatTable.GetCurrentMaterialName(SEDoc, CurrentMaterialName)
            MatTable.GetMaterialLibraryList(MaterialLibList, NumMaterialLibraries)
            MatTableProps = System.Enum.GetValues(GetType(SolidEdgeConstants.MatTablePropIndex))

            'Make sure the ActiveMaterialLibrary in settings.txt is present
            For Each MatTableMaterial In MaterialLibList
                If MatTableMaterial.ToString = ActiveMaterialLibrary Then
                    ActiveMaterialLibraryPresent = True
                    Exit For
                End If
            Next

            If Not ActiveMaterialLibraryPresent Then
                msg = "ActiveMaterialLibrary " + Configuration("TextBoxActiveMaterialLibrary") + " not found.  Exiting..." + Chr(13)
                msg += "Please update the Material Table on the Configuration tab." + Chr(13)
                MsgBox(msg)
                SEApp.Quit()
                End
            End If

            'See if the CurrentMaterialName is in the ActiveLibrary
            MatTable.GetMaterialListFromLibrary(ActiveMaterialLibrary, NumMaterials, MaterialList)
            For Each MatTableMaterial In MaterialList
                If MatTableMaterial.ToString.ToLower.Trim = CurrentMaterialName.ToLower.Trim Then
                    CurrentMaterialNameInLibrary = True

                    'The names match.  Check if their properties do, too.
                    MatTableProps = System.Enum.GetValues(GetType(SolidEdgeConstants.MatTablePropIndex))
                    For Each MatTableProp In MatTableProps
                        MatTable.GetMaterialPropValueFromLibrary(MatTableMaterial.ToString, ActiveMaterialLibrary, MatTableProp, LibPropValue)
                        MatTable.GetMaterialPropValueFromDoc(SEDoc, MatTableProp, DocPropValue)
                        If DocPropValue <> LibPropValue Then
                            ' Double types may have insignificant differences.
                            If (DocPropValue.GetType = GetType(Double)) And (DocPropValue.GetType = GetType(Double)) Then
                                Dim PercentDifference As Double = (DocPropValue - LibPropValue) / ((DocPropValue + LibPropValue) / 2)
                                If Not Math.Abs(PercentDifference) < 0.001 Then
                                    CurrentMaterialMatchesLibMaterial = False
                                    Exit For
                                End If
                            Else
                                CurrentMaterialMatchesLibMaterial = False
                                Exit For
                            End If
                        End If
                        DocPropValue = Nothing
                        LibPropValue = Nothing
                    Next

                    If Not CurrentMaterialMatchesLibMaterial Then
                        MatTable.ApplyMaterialToDoc(SEDoc, MatTableMaterial.ToString, ActiveMaterialLibrary)
                        SEDoc.Save()
                        SEApp.DoIdle()
                        Exit For
                    End If
                End If
                If CurrentMaterialNameInLibrary Then
                    Exit For
                End If
            Next

            If Not CurrentMaterialNameInLibrary Then
                ExitStatus = "1"
                If CurrentMaterialName = "" Then
                    ErrorMessage = "    Material " + "'None'" + " not in " + ActiveMaterialLibrary + Chr(13)
                Else
                    ErrorMessage = "    Material '" + CurrentMaterialName + "' not in " + ActiveMaterialLibrary + Chr(13)
                End If
            End If
        End If



        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function


    Public Function PartNumberDoesNotMatchFilename(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf PartNumberDoesNotMatchFilenameInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function PartNumberDoesNotMatchFilenameInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim PropertySets As SolidEdgeFramework.PropertySets = Nothing
        Dim Properties As SolidEdgeFramework.Properties = Nothing
        Dim Prop As SolidEdgeFramework.Property = Nothing

        Dim PartNumber As String = ""
        Dim PartNumberPropertyFound As Boolean = False
        Dim TF As Boolean
        Dim Filename As String

        'Get the bare file name without directory information
        Filename = System.IO.Path.GetFileName(SEDoc.FullName)

        Dim msg As String = ""

        PropertySets = SEDoc.Properties

        For Each Properties In PropertySets
            msg += Properties.Name + Chr(13)
            For Each Prop In Properties
                TF = (Configuration("ComboBoxPartNumberPropertySet").ToLower = "custom") And (Properties.Name.ToLower = "custom")
                If TF Then
                    If Prop.Name = Configuration("TextBoxPartNumberPropertyName") Then
                        'If Prop.Name = TextBoxPartNumberPropertyName.Text Then
                        PartNumber = Prop.Value.Trim
                        PartNumberPropertyFound = True
                        Exit For
                    End If
                Else
                    If Prop.Name = Configuration("TextBoxPartNumberPropertyName") Then
                        PartNumber = Prop.Value.Trim
                        PartNumberPropertyFound = True
                        Exit For
                    End If
                End If
            Next
            If PartNumberPropertyFound Then
                Exit For
            End If
        Next

        If PartNumberPropertyFound Then
            If PartNumber.Trim = "" Then
                ExitStatus = "1"
                ErrorMessage = "    Part number not assigned" + Chr(13)
            End If
            If Not Filename.Contains(PartNumber) Then
                ExitStatus = "1"
                ErrorMessage = "    Part number '" + PartNumber
                ErrorMessage += "' not found in filename '" + Filename + "'" + Chr(13)
            End If
        Else
            ExitStatus = "1"
            ErrorMessage = "    PartNumberPropertyName: '" + Configuration("TextBoxPartNumberPropertyName") + "'"
            ErrorMessage += " not found in PartNumberPropertySet: '" + Configuration("ComboBoxPartNumberPropertySet") + "'" + Chr(13)
            If Configuration("TextBoxPartNumberPropertyName") = "" Then
                ErrorMessage += "    Check the Configuration tab for valid entries"
            End If
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function


    Public Function GenerateLaserDXFAndPDF(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf GenerateLaserDXFAndPDFInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function GenerateLaserDXFAndPDFInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim DraftTasks As New DraftTasks

        Dim DraftFilename As String
        Dim DraftFileMissing As Boolean = False
        Dim DraftOutOfDate As Boolean = False
        Dim FlatPatternOutOfDate As Boolean = False
        Dim DXFFilename As String
        Dim PDFFilename As String
        Dim SheetmetalBaseFilename As String

        Dim SEDraftDoc As SolidEdgeDraft.DraftDocument = Nothing
        Dim Models As SolidEdgePart.Models = Nothing

        Dim msg As String = ""

        If Not FileIO.FileSystem.DirectoryExists(Configuration("TextBoxLaserOutputDirectory")) Then
            msg = "Laser output directory not found: '" + Configuration("TextBoxLaserOutputDirectory") + "'.  "
            msg += "Please select a valid directory on the Sheetmetal Tab." + Chr(13)
            msg += "Exiting..."
            MsgBox(msg)
            SEApp.Quit()
            End
        End If

        DraftFilename = System.IO.Path.ChangeExtension(SEDoc.FullName, ".dft")
        If Not FileIO.FileSystem.FileExists(DraftFilename) Then
            DraftFileMissing = True
            ExitStatus = "1"
            ErrorMessage += "    Draft document not found: " + TruncateFullPath(DraftFilename, Configuration) + Chr(13)
        Else
            SEDraftDoc = SEApp.Documents.Open(DraftFilename)
        End If

        SheetmetalBaseFilename = System.IO.Path.GetFileName(SEDoc.FullName)

        DXFFilename = Configuration("TextBoxLaserOutputDirectory") + "\" + System.IO.Path.ChangeExtension(SheetmetalBaseFilename, ".dxf")
        PDFFilename = Configuration("TextBoxLaserOutputDirectory") + "\" + System.IO.Path.ChangeExtension(SheetmetalBaseFilename, ".pdf")

        ErrorMessageList = FlatPatternMissingOrOutOfDate(SEDoc, Configuration, SEApp)
        If ExitStatus = "0" Then
            ExitStatus = ErrorMessageList(0)
        End If
        If ErrorMessageList(0) = "1" Then
            FlatPatternOutOfDate = True
            ErrorMessage += "    Flat pattern missing or out of date" + Chr(13)
        End If

        If Not DraftFileMissing Then
            ErrorMessageList = DraftTasks.DrawingViewsOutOfDate(SEDraftDoc, Configuration, SEApp)
            If ExitStatus = "0" Then
                ExitStatus = ErrorMessageList(0)
            End If
            If ErrorMessageList(0) = "1" Then
                DraftOutOfDate = True
                ErrorMessage += "    Draft views out of date" + Chr(13)
            End If
        End If

        'Save flat pattern even if there is no drawing
        'Don't save if there is a drawing but it is out of date
        If (Not FlatPatternOutOfDate) And (Not DraftOutOfDate) Then
            Models = SEDoc.Models
            Models.SaveAsFlatDXFEx(DXFFilename, Nothing, Nothing, Nothing, True)
            SEApp.DoIdle()
        End If

        'Don't save PDF if the flat pattern or draft is out of date
        If (Not DraftFileMissing) And (Not FlatPatternOutOfDate) And (Not DraftOutOfDate) Then
            SEDraftDoc.SaveAs(PDFFilename)
            SEApp.DoIdle()
        End If

        If SEDraftDoc IsNot Nothing Then
            SEDraftDoc.Close(False)
            SEApp.DoIdle()
        End If

        ErrorMessageList.Clear()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function


    Public Function UpdateFaceAndViewStylesFromTemplate(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf UpdateFaceAndViewStylesFromTemplateInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function UpdateFaceAndViewStylesFromTemplateInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim SETemplateDoc As SolidEdgePart.SheetMetalDocument
        Dim Windows As SolidEdgeFramework.Windows
        Dim Window As SolidEdgeFramework.Window
        Dim View As SolidEdgeFramework.View
        Dim ViewStyles As SolidEdgeFramework.ViewStyles
        Dim ViewStyle As SolidEdgeFramework.ViewStyle

        'Dim TemplateFilename As String = TextBoxTemplateSheetmetal.Text
        Dim TemplateFilename As String = Configuration("TextBoxTemplateSheetmetal")
        Dim TemplateActiveStyleName As String = ""
        Dim TempViewStyleName As String = ""
        Dim ViewStyleAlreadyPresent As Boolean
        Dim TemplateSkyboxName(5) As String
        Dim msg As String = ""

        SEDoc.ImportStyles(TemplateFilename, True)

        ' Find the active ViewStyle in the template file.
        SETemplateDoc = SEApp.Documents.Open(TemplateFilename)

        Windows = SETemplateDoc.Windows
        For Each Window In Windows
            View = Window.View
            TemplateActiveStyleName = View.Style.ToString
        Next

        ViewStyles = SETemplateDoc.ViewStyles

        For Each ViewStyle In ViewStyles
            If ViewStyle.StyleName = TemplateActiveStyleName Then
                For i As Integer = 0 To 5
                    TemplateSkyboxName(i) = ViewStyle.GetSkyboxSideFilename(i)
                Next
            End If
        Next

        SETemplateDoc.Close(False)
        SEApp.DoIdle()

        ' If a style by the same name exists in the target file, delete it.
        ViewStyleAlreadyPresent = False
        ViewStyles = SEDoc.ViewStyles
        For Each ViewStyle In ViewStyles
            If ViewStyle.StyleName = TemplateActiveStyleName Then
                ViewStyleAlreadyPresent = True
            Else
                TempViewStyleName = ViewStyle.StyleName
            End If
        Next

        SEApp.DoIdle()

        Windows = SEDoc.Windows

        If ViewStyleAlreadyPresent Then ' Hopefully deactivate the desired ViewStyle so it can be removed
            For Each Window In Windows
                View = Window.View
                View.Style = TempViewStyleName
            Next
            ViewStyles.Remove(TemplateActiveStyleName)
        End If

        ViewStyles.AddFromFile(TemplateFilename, TemplateActiveStyleName)

        For Each ViewStyle In ViewStyles
            If ViewStyle.StyleName = TemplateActiveStyleName Then
                ViewStyle.SkyboxType = SolidEdgeFramework.SeSkyboxType.seSkyboxTypeSkybox
                For i As Integer = 0 To 5
                    ViewStyle.SetSkyboxSideFilename(i, TemplateSkyboxName(i))
                Next
            End If
        Next

        For Each Window In Windows
            View = Window.View
            View.Style = TemplateActiveStyleName
        Next


        SEDoc.Save()
        SEApp.DoIdle()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function


    Public Function FitIsometricView(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)

        ErrorMessageList = InvokeSTAThread(
                               Of SolidEdgePart.SheetMetalDocument,
                               Dictionary(Of String, String),
                               SolidEdgeFramework.Application,
                               List(Of String))(
                                   AddressOf FitIsometricViewInternal,
                                   SEDoc,
                                   Configuration,
                                   SEApp)

        Return ErrorMessageList

    End Function

    Private Function FitIsometricViewInternal(
        ByVal SEDoc As SolidEdgePart.SheetMetalDocument,
        ByVal Configuration As Dictionary(Of String, String),
        ByVal SEApp As SolidEdgeFramework.Application
        ) As List(Of String)

        Dim RefPlanes As SolidEdgePart.RefPlanes
        Dim RefPlane As SolidEdgePart.RefPlane

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        RefPlanes = SEDoc.RefPlanes
        For Each RefPlane In RefPlanes
            RefPlane.Visible = False
        Next

        SEDoc.Constructions.Visible = False
        SEDoc.CoordinateSystems.Visible = False

        SEApp.StartCommand(SolidEdgeConstants.PartCommandConstants.PartViewISOView)
        SEApp.StartCommand(SolidEdgeConstants.PartCommandConstants.PartViewFit)

        SEDoc.Save()
        SEApp.DoIdle()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function


    Private Function TruncateFullPath(ByVal Path As String,
        Configuration As Dictionary(Of String, String)
        ) As String

        Dim Length As Integer = Len(Configuration("TextBoxInputDirectory"))
        Dim NewPath As String

        If Path.Contains(Configuration("TextBoxInputDirectory")) Then
            NewPath = Path.Remove(0, Length)
            NewPath = "~" + NewPath
        Else
            NewPath = Path
        End If
        Return NewPath
    End Function

End Class
