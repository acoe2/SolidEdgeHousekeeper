Option Strict On

Imports System.Runtime.InteropServices

Partial Class Form1

    Private Sub SEStart()
        ' Start Solid Edge.
        TextBoxStatus.Text = "Starting Solid Edge..."

        'Try
        '    SEApp = CType(GetObject(, "SolidEdge.Application"), SolidEdgeFramework.Application)
        '    SEApp.Visible = True
        '    SEApp.DisplayAlerts = False
        '    'SEApp.DisplayAlerts = True
        '    SEApp.WindowState = 2  'Maximizes Solid Edge
        'Catch ex As Exception
        'End Try

        Try
            SEApp = CType(CreateObject("SolidEdge.Application"), SolidEdgeFramework.Application)
            ' Make Solid Edge visible and turn off popups.
            SEApp.Visible = True
            SEApp.DisplayAlerts = False
            'SEApp.DisplayAlerts = True
            SEApp.WindowState = 2  'Maximizes Solid Edge
        Catch ex As Exception
            Activate()
            MsgBox("Could not start Solid Edge.  Exiting...")
            End
        End Try

        Activate()

    End Sub

    Private Sub SEStop()
        TextBoxStatus.Text = "Closing Solid Edge..."
        If (Not (SEApp Is Nothing)) Then
            Try
                SEApp.Quit()
            Catch ex As Exception
                SEKillProcess("edge")
            End Try
        End If
        SEGarbageCollect(SEApp)
        System.Threading.Thread.Sleep(100)

        If Not SEApp Is Nothing Then
            SEApp = Nothing
        End If

        System.Threading.Thread.Sleep(3000)
    End Sub

    Private Sub SEGarbageCollect(ByVal obj As Object)
        Try
            '******* Added because of .NET
            If Not (obj Is Nothing) Then
                'Marshal.ReleaseComObject(obj)
                Marshal.FinalReleaseComObject(obj)
            End If

            GC.Collect(GC.MaxGeneration)
            GC.WaitForPendingFinalizers()
            GC.Collect(GC.MaxGeneration)
            GC.WaitForPendingFinalizers()
            '******* Added because of .NET
        Catch ex As Exception
            obj = Nothing
            GC.Collect(GC.MaxGeneration)
            GC.WaitForPendingFinalizers()
            GC.Collect(GC.MaxGeneration)
            GC.WaitForPendingFinalizers()

        End Try
    End Sub

    Private Function SEKillProcess(ByVal Name As String) As Long

        Dim LocalProcs As Process()
        Dim Proc As Process
        Dim i As Integer
        Dim blnProcessTerminated As Boolean
        blnProcessTerminated = False

        LocalProcs = System.Diagnostics.Process.GetProcesses
        For Each Proc In LocalProcs
            If UCase(Proc.ProcessName) = UCase(Name) Then
                Try
                    Proc.Kill()
                    SEKillProcess = 0
                    blnProcessTerminated = True
                Catch ex As System.Exception
                    SEKillProcess = -1
                    LocalProcs = Nothing
                    Exit Function
                End Try
            End If
            i += 1
        Next

        If blnProcessTerminated = True Then
            SEKillProcess = 0
            Exit Function
        End If

        If blnProcessTerminated = False Then
            SEKillProcess = -2
            Exit Function
        End If

        SEKillProcess = -1

    End Function

    Private Function SEIsRunning() As Boolean
        Try
            SEApp = CType(GetObject(, "SolidEdge.Application"), SolidEdgeFramework.Application)
        Catch ex As Exception
        End Try

        If Not SEApp Is Nothing Then
            SEApp = Nothing
            Return True
        Else
            Return False
        End If
    End Function

End Class