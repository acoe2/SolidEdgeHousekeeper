Partial Class Form1

    Private Sub LoadDefaults()
        'See format example in SaveDefaults()

        Dim StartupPath As String = System.Windows.Forms.Application.StartupPath()
        Dim Defaults As String() = Nothing
        Dim Key As String
        Dim Value As String

        DefaultsFilename = StartupPath + "\" + "defaults.txt"

        Try
            Defaults = IO.File.ReadAllLines(DefaultsFilename)
            Dim msg As String = ""
            For Each KVPair As String In Defaults
                Key = KVPair.Split("=")(0)
                Value = KVPair.Split("=")(1)

                If Key = TextBoxInputDirectory.Name Then
                    If FileIO.FileSystem.DirectoryExists(Value) Then
                        TextBoxInputDirectory.Text = Value
                    Else
                        TextBoxInputDirectory.Text = Application.StartupPath
                    End If

                ElseIf Key = TextBoxTemplateAssembly.Name Then
                    If FileIO.FileSystem.FileExists(Value) Then
                        TextBoxTemplateAssembly.Text = Value
                    Else
                        TextBoxTemplateAssembly.Text = ""
                    End If

                ElseIf Key = TextBoxTemplatePart.Name Then
                    If FileIO.FileSystem.FileExists(Value) Then
                        TextBoxTemplatePart.Text = Value
                    Else
                        TextBoxTemplatePart.Text = ""
                    End If

                ElseIf Key = TextBoxTemplateSheetmetal.Name Then
                    If FileIO.FileSystem.FileExists(Value) Then
                        TextBoxTemplateSheetmetal.Text = Value
                    Else
                        TextBoxTemplateSheetmetal.Text = ""
                    End If

                ElseIf Key = TextBoxTemplateDraft.Name Then
                    If FileIO.FileSystem.FileExists(Value) Then
                        TextBoxTemplateDraft.Text = Value
                    Else
                        TextBoxTemplateDraft.Text = ""
                    End If

                ElseIf Key = TextBoxActiveMaterialLibrary.Name Then
                    If FileIO.FileSystem.FileExists(Value) Then
                        TextBoxActiveMaterialLibrary.Text = Value
                    Else
                        TextBoxActiveMaterialLibrary.Text = ""
                    End If

                ElseIf Key = ComboBoxPartNumberPropertySet.Name Then
                    ComboBoxPartNumberPropertySet.Text = Value

                ElseIf Key = TextBoxPartNumberPropertyName.Name Then
                    TextBoxPartNumberPropertyName.Text = Value

                ElseIf Key = TextBoxRestartAfter.Name Then
                    If Value = "" Then
                        TextBoxRestartAfter.Text = 50
                    Else
                        Try
                            TextBoxRestartAfter.Text = Int(Value)
                        Catch ex As Exception
                            TextBoxRestartAfter.Text = 50
                        End Try
                    End If

                ElseIf Key = TextBoxLaserOutputDirectory.Name Then
                    If FileIO.FileSystem.DirectoryExists(Value) Then
                        TextBoxLaserOutputDirectory.Text = Value
                    Else
                        TextBoxLaserOutputDirectory.Text = ""
                    End If

                Else
                    PopulateCheckboxDefault(KVPair)
                End If
            Next
        Catch ex As Exception
            TextBoxInputDirectory.Text = Application.StartupPath
            'MsgBox("Exception" + ex.ToString)
        End Try

        ReconcileFormChanges()

    End Sub

    Private Sub SaveDefaults()
        'Format Example
        'TextBoxInputDirectory=D:\CAD\scripts\test_files
        'TextBoxTemplateAssembly=C:\Program Files\Siemens\Solid Edge 2020\Template\ANSI Inch\ansi inch assembly.asm
        'TextBoxTemplatePart=C:\Program Files\Siemens\Solid Edge 2020\Template\ANSI Inch\ansi inch part.par
        'TextBoxTemplateSheetmetal=C:\Program Files\Siemens\Solid Edge 2020\Template\ANSI Inch\ansi inch sheet metal.psm
        'TextBoxTemplateDraft=C:\Program Files\Siemens\Solid Edge 2020\Template\ANSI Inch\Hallmark_B.dft
        'TextBoxActiveMaterialLibrary=C:\Program Files\Siemens\Solid Edge 2020\Preferences\Materials\hmk_Materials.mtl
        'ComboBoxPartNumberPropertySet=Custom
        'TextBoxPartNumberPropertyName=hmk_Part_Number
        'TextBoxRestartAfter=50
        'TextBoxLaserOutputDirectory=D:\CAD\scripts\test_files\20181212_auto_cube_fill\laser_files_robert
        'Assembly.Occurrence missing file=Checked
        'Assembly.Occurrence outside project directory=Unchecked
        'Assembly.Failed relationships=Unchecked

        Dim Defaults As New List(Of String)
        Dim msg As String

        Defaults.Add(TextBoxInputDirectory.Name + "=" + TextBoxInputDirectory.Text)
        Defaults.Add(TextBoxTemplateAssembly.Name + "=" + TextBoxTemplateAssembly.Text)
        Defaults.Add(TextBoxTemplatePart.Name + "=" + TextBoxTemplatePart.Text)
        Defaults.Add(TextBoxTemplateSheetmetal.Name + "=" + TextBoxTemplateSheetmetal.Text)
        Defaults.Add(TextBoxTemplateDraft.Name + "=" + TextBoxTemplateDraft.Text)
        Defaults.Add(TextBoxActiveMaterialLibrary.Name + "=" + TextBoxActiveMaterialLibrary.Text)
        Defaults.Add(ComboBoxPartNumberPropertySet.Name + "=" + ComboBoxPartNumberPropertySet.Text)
        Defaults.Add(TextBoxPartNumberPropertyName.Name + "=" + TextBoxPartNumberPropertyName.Text)
        Defaults.Add(TextBoxRestartAfter.Name + "=" + TextBoxRestartAfter.Text)
        Defaults.Add(TextBoxLaserOutputDirectory.Name + "=" + TextBoxLaserOutputDirectory.Text)

        For idx = 0 To CheckedListBoxAssembly.Items.Count - 1
            msg = "Assembly." + CheckedListBoxAssembly.Items(idx).ToString + "="
            msg += CheckedListBoxAssembly.GetItemCheckState(idx).ToString()
            Defaults.Add(msg)
        Next
        For idx = 0 To CheckedListBoxPart.Items.Count - 1
            msg = "Part." + CheckedListBoxPart.Items(idx).ToString + "="
            msg += CheckedListBoxPart.GetItemCheckState(idx).ToString()
            Defaults.Add(msg)
        Next
        For idx = 0 To CheckedListBoxSheetmetal.Items.Count - 1
            msg = "Sheetmetal." + CheckedListBoxSheetmetal.Items(idx).ToString + "="
            msg += CheckedListBoxSheetmetal.GetItemCheckState(idx).ToString()
            Defaults.Add(msg)
        Next
        For idx = 0 To CheckedListBoxDraft.Items.Count - 1
            msg = "Draft." + CheckedListBoxDraft.Items(idx).ToString + "="
            msg += CheckedListBoxDraft.GetItemCheckState(idx).ToString()
            Defaults.Add(msg)
        Next

        IO.File.WriteAllLines(DefaultsFilename, Defaults)

    End Sub

    Private Sub PopulateCheckboxDefault(KVPair As String)
        'See format example in SaveDefaults

        Dim KeyAndName As String
        Dim Key As String
        Dim Value As String
        Dim CheckBoxName As String
        Dim ValueAsBoolean As Boolean
        Dim idx As Integer

        KeyAndName = KVPair.Split("=")(0)
        Key = KeyAndName.Split(".")(0)
        CheckBoxName = KeyAndName.Split(".")(1)
        Value = KVPair.Split("=")(1)

        If Value = "Checked" Then
            ValueAsBoolean = True
        Else
            ValueAsBoolean = False
        End If

        If Key = "Assembly" Then
            For idx = 0 To CheckedListBoxAssembly.Items.Count - 1
                If CheckedListBoxAssembly.Items(idx).ToString = CheckBoxName Then
                    CheckedListBoxAssembly.SetItemChecked(idx, ValueAsBoolean)
                End If
            Next
        End If
        If Key = "Part" Then
            For idx = 0 To CheckedListBoxPart.Items.Count - 1
                If CheckedListBoxPart.Items(idx).ToString = CheckBoxName Then
                    CheckedListBoxPart.SetItemChecked(idx, ValueAsBoolean)
                End If
            Next
        End If
        If Key = "Sheetmetal" Then
            For idx = 0 To CheckedListBoxSheetmetal.Items.Count - 1
                If CheckedListBoxSheetmetal.Items(idx).ToString = CheckBoxName Then
                    CheckedListBoxSheetmetal.SetItemChecked(idx, ValueAsBoolean)
                End If
            Next
        End If
        If Key = "Draft" Then
            For idx = 0 To CheckedListBoxDraft.Items.Count - 1
                If CheckedListBoxDraft.Items(idx).ToString = CheckBoxName Then
                    CheckedListBoxDraft.SetItemChecked(idx, ValueAsBoolean)
                End If
            Next
        End If
    End Sub

    Private Sub LoadTextBoxReadme()
        Dim msg As String
        Dim msg1 As New List(Of String)
        Dim msg2 As New List(Of String)
        Dim msg3 As New List(Of String)
        Dim ReadmeFileName As String = ""
        Dim FilenameList As New List(Of String)

        Dim CheckBoxList As New List(Of CheckedListBox)
        Dim Names As New List(Of String)

        CheckBoxList.Add(CheckedListBoxAssembly)
        CheckBoxList.Add(CheckedListBoxPart)
        CheckBoxList.Add(CheckedListBoxSheetmetal)
        CheckBoxList.Add(CheckedListBoxDraft)

        Names.Add("### Assembly")
        Names.Add("### Part")
        Names.Add("### Sheetmetal")
        Names.Add("### Draft")

        msg = "# Solid Edge Housekeeper"
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "Robert McAnany 2020"
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "Portions adapted from code by Jason Newell, Greg Chasteen, Tushar Suradkar, and others.  Most of the rest copied verbatim from Jason's repo and Tushar's blog."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "## DESCRIPTION"
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "This tool is designed to help you find annoying little errors in your project.  It can identify failed features in 3D models, detached dimensions in drawings, missing parts in assemblies, and more.  It can also update certain individual file settings to match those in a template you specify."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "## INSTALLATION"
        msg1.Add(msg)
        msg = "There is no installation per se.  The preferred method is to download or clone the project and compile it yourself."
        msg1.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg = "Not everyone can do that, however, so the compiled executable is provided.  To get it, click the bin/Release folder above and select Housekeeper.exe.  On that page, click Download and save it to a convenient location.  Double-click the executable to run."
        msg1.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg = "## OPERATION"
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "Select which errors to detect by clicking the appropriate checkboxes on each file type's tab.  Select which files to process, on the General tab, by navigating to the desired input folder, and then clicking the desired search type."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "If any errors are found, a text file will be written to the input folder.  It will identify each error and the file in which it occurred.  When processing is complete, a message box will give you the file name."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "The first time you use the program, you need to supply some user-specific information.  This includes the location of your templates, material table, and the like.  These are accessed on the Configuration Tab."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "## CAVEATS"
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "Since the program can process a large number of files in a short amount of time, it can be very taxing on Solid Edge.  To maintain a clean environment, the program restarts Solid Edge periodically.  This is by design and does not necessarily indicate a problem.  "
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "However, problems can arise.  Those cases will also be reported in the text file with the message 'Error processing file'.  A stack trace will be included, which may be useful for program debugging."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "Please note this is not a perfect program.  It is not guaranteed not to mess up your files.  Back up any files before using it."
        msg1.Add(msg)
        msg2.Add(msg)
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "## DETAILS"
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "List of implemented tests and actions."
        msg1.Add(msg)
        msg2.Add(msg)
        For i As Integer = 0 To 3
            msg1.Add(Names(i))
            msg2.Add(Names(i))
            For Each Item In CheckBoxList(i).Items
                msg1.Add("    " + Item)
                msg2.Add("    " + Item)
            Next
        Next
        msg = ""
        msg1.Add(msg)
        msg2.Add(msg)
        msg = "## CODE ORGANIZATION"
        msg1.Add(msg)
        msg = "Processing starts in Form1.vb.  A short description of the code's organization can be found there."
        msg1.Add(msg)

        For Each s As String In msg2
            msg3.Add(s.Replace("# ", "").Replace("#", ""))
        Next
        TextBoxReadme.Lines = msg3.ToArray

        ' Create README.md.  Need to remove the last two directories of the location of the executable.
        FilenameList = System.IO.Path.GetDirectoryName(DefaultsFilename).Split("\").ToList
        For i As Integer = 0 To 1  ' Remove the last two directories
            FilenameList.RemoveAt(FilenameList.Count - 1)
        Next
        For i As Integer = 0 To FilenameList.Count - 1
            ReadmeFileName += FilenameList(i) + "\"
        Next
        ReadmeFileName += "README.md"

        IO.File.WriteAllLines(ReadmeFileName, msg1)

    End Sub

End Class