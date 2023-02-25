Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports NativeSharp

Public Class InjectorTester

#Region " PInvoke "

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Public Shared Function LoadLibrary(ByVal lpFileName As String) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Public Shared Function OpenProcess(ByVal processAccess As UInteger, ByVal bInheritHandle As Boolean, ByVal processId As Integer) As IntPtr
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True)>
    Public Shared Function GetProcAddress(ByVal hModule As IntPtr, ByVal procName As String) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, ExactSpelling:=True)>
    Public Shared Function VirtualAllocEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As UInteger, ByVal flAllocationType As UInteger, ByVal flProtect As UInteger) As IntPtr
    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function WriteProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Byte(), ByVal nSize As Int32, <Out> ByRef lpNumberOfBytesWritten As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function CreateRemoteThread(ByVal hProcess As IntPtr, ByVal lpThreadAttributes As IntPtr, ByVal dwStackSize As UInteger, ByVal lpStartAddress As IntPtr, ByVal lpParameter As IntPtr, ByVal dwCreationFlags As UInteger, <Out> ByRef lpThreadId As IntPtr) As IntPtr
    End Function

#End Region

#Region " Flags "

    ' OpenProcess
    <Flags>
    Public Enum ProcessAccessFlags As UInteger
        All = &H1F0FFF
        Terminate = &H1
        CreateThread = &H2
        VirtualMemoryOperation = &H8
        VirtualMemoryRead = &H10
        VirtualMemoryWrite = &H20
        DuplicateHandle = &H40
        CreateProcess = &H80
        SetQuota = &H100
        SetInformation = &H200
        QueryInformation = &H400
        QueryLimitedInformation = &H1000
        Synchronize = &H100000
    End Enum

    'VirtualAllocEx
    <Flags>
    Public Enum AllocationType
        Commit = &H1000
        Reserve = &H2000
        Decommit = &H4000
        Release = &H8000
        Reset = &H80000
        Physical = &H400000
        TopDown = &H100000
        WriteWatch = &H200000
        LargePages = &H20000000
    End Enum

    <Flags>
    Public Enum MemoryProtection
        Execute = &H10
        ExecuteRead = &H20
        ExecuteReadWrite = &H40
        ExecuteWriteCopy = &H80
        NoAccess = &H1
        [ReadOnly] = &H2
        ReadWrite = &H4
        WriteCopy = &H8
        GuardModifierflag = &H100
        NoCacheModifierflag = &H200
        WriteCombineModifierflag = &H400
    End Enum


#End Region

    Private Sub InjectorTester_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListProcess()
    End Sub

#Region " GUI Misc "

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Dim CurrentSelected As String = Guna2ComboBox1.Text
        Dim startIndex As Integer = CurrentSelected.IndexOf("[") + 1
        Dim length As Integer = CurrentSelected.IndexOf("]") - startIndex
        Dim ProcessID As String = CurrentSelected.Substring(startIndex, length)

        Try
            Dim TargetProcess As Process = Process.GetProcessById(ProcessID)
            If IO.File.Exists(Guna2TextBox1.Text) = True Then

                Dim InjectToProcess As Boolean = Inject(TargetProcess, Guna2TextBox1.Text)
                If InjectToProcess = True Then
                    Label8.Text = "The library has been correctly injected!"
                    Label8.ForeColor = Color.Lime
                Else
                    Label8.Text = "Failed to Inject! " & ExInfo
                    Label8.ForeColor = Color.Red
                End If
            Else
                Throw New IO.FileNotFoundException
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        ListProcess()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Guna2TextBox1.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Process.Start("https://toolslib.net/downloads/viewdownload/1926-dll-injector-hacker/")
    End Sub

#End Region

#Region " Inject "

    Private ExInfo As String = String.Empty

    Public Function Inject(ByVal Target As Process, ByVal DllPath As String)

        '  Dim ModuleHandle As IntPtr = LoadLibrary(DllPath)

        '  If ModuleHandle = IntPtr.Zero Then
        '  ExInfo = "Error In LoadLibrary"
        '  Return False
        '  End If

        Dim ProcHandle As IntPtr = OpenProcess(ProcessAccessFlags.All, False, Target.Id)

        If ProcHandle = IntPtr.Zero Then
            ExInfo = "Error In OpenProcess"
            Return False
        End If

        Dim lpLLAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA") 'GetProcAddress(ModuleHandle, "TestFunction1") 'GetModuleHandle("kernel32.dll")
        If lpLLAddress = IntPtr.Zero Then
            ExInfo = "Error In GetProcAddress"
            Return False
        End If

        Dim Adress As IntPtr = VirtualAllocEx(ProcHandle, Nothing, DllPath.Length, AllocationType.Commit Or AllocationType.Reserve, MemoryProtection.ExecuteReadWrite)

        If Adress = IntPtr.Zero Then
            ExInfo = "Error In VirtualAllocEx"
            Return False
        End If

        Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(DllPath)
        Dim ipTmp As IntPtr = IntPtr.Zero
        WriteProcessMemory(ProcHandle, Adress, bytes, CUInt(bytes.Length), ipTmp)

        If ipTmp = IntPtr.Zero Then
            ExInfo = "Error In WriteProcessMemory"
            Return False
        End If

        Dim ThreadStart As IntPtr = CreateRemoteThread(ProcHandle, Nothing, IntPtr.Zero, lpLLAddress, Adress, Nothing, 0)

        If ThreadStart = IntPtr.Zero Then
            ExInfo = "Error In CreateRemoteThread"
            Return False
        End If

        Return True
    End Function

#End Region

#Region " Private Methods "

    Public Sub ListProcess()
        Guna2ComboBox1.Items.Clear()
        Dim ProcessAll As List(Of Process) = Process.GetProcesses.ToList

        If Guna2ToggleSwitch1.Checked = True Then
            ProcessAll = RemoveDuplicate(ProcessAll)
        End If

        Dim ProcFullList As New List(Of String)

        For Each Proc As Process In ProcessAll
            Dim ProcessStr As String = Proc.ProcessName & "  -  [" & Proc.Id & "]"
            ProcFullList.Add(ProcessStr)
        Next

        ProcFullList.Sort()

        Guna2ComboBox1.Items.AddRange(ProcFullList.ToArray)

        If Not Guna2ComboBox1.Items.Count = 0 Then
            Guna2ComboBox1.SelectedIndex = 0
        End If

    End Sub

    Private Function RemoveDuplicate(ByVal TheList As List(Of Process)) As List(Of Process)
        Dim Result As New List(Of Process)

        Dim Exist As Boolean = False
        For Each ElementString As Process In TheList
            Exist = False
            For Each ElementStringInResult As Process In Result
                If ElementString.ProcessName = ElementStringInResult.ProcessName Then
                    Exist = True
                    Exit For
                End If
            Next
            If Not Exist Then
                Result.Add(ElementString)
            End If
        Next

        Return Result
    End Function

#End Region

End Class