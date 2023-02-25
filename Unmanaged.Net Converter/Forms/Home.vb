Imports System.IO
Imports dnlib.DotNet
Imports dnlib.DotNet.Writer
Imports Greensoft.ByteTools.Encodings

Public Class Home

#Region " Declare "

    Private AssemblyModule As ModuleDefMD = Nothing


#End Region


    Private Sub Home_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartUI()
    End Sub


#Region " GUI "


    Public Sub StartUI()

        Guna2ComboBox1.Items.Clear()
        Guna2ComboBox3.Items.Clear()

        Dim MachineTypeList As List(Of Object) = [Enum].GetValues(GetType(dnlib.PE.Machine)).Cast(Of Object)().ToList()
        Guna2ComboBox3.Items.AddRange(MachineTypeList.ToArray)
        Guna2ComboBox3.SelectedIndex = 0

        WriteLog("[INFO] Unmanaged.Net Converter By S4Lsalsoft - Copyright ©  2021
[INFO] Discord : Destroyer#8328
[INFO] Email : S4Lsalsoft@gmail.com", Color.White)

    End Sub

    Dim Loading As Boolean = False

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click

        If Loading = True Then Exit Sub

        Guna2TextBox2.Text = ""
        Dim TextBox1 As String = Guna2TextBox1.Text
        Dim ComboBox1 As String = Guna2ComboBox1.Text
        Dim ComboBox2 As String = Guna2ComboBox2.Text
        Dim ComboBox3Index As Integer = Guna2ComboBox3.SelectedIndex

        Dim Asynctask As New Task(New Action(Sub()
                                                 Loading = True
                                                 If AssemblyModule IsNot Nothing Then

                                                     Try

                                                         Dim Args As String = ""
                                                         If Not LCase(ComboBox2) = "exe" Then Args = " -shared"

                                                         Dim TargetSaveName As String = IO.Path.GetFileNameWithoutExtension(TextBox1) & "Exported." & LCase(ComboBox2)
                                                         Dim SavePath As String = IO.Path.Combine(IO.Path.GetDirectoryName(TextBox1), TargetSaveName)
                                                         Dim MachineTypeList As List(Of dnlib.PE.Machine) = [Enum].GetValues(GetType(dnlib.PE.Machine)).Cast(Of dnlib.PE.Machine)().ToList()
                                                         WriteLog("DEBUG] Checking Strong Name...", Color.Orange)
                                                         Dim ClassMethod As String() = ComboBox1.Split(".")

                                                         Dim TypeCl As TypeDef = AssemblyModule.GetTypes().FirstOrDefault(Function(t) t.Name = ClassMethod.FirstOrDefault)
                                                         Dim MethodEx As MethodDef = TypeCl.Methods.FirstOrDefault(Function(m) m.Name = ClassMethod.LastOrDefault)

                                                         WriteLog("DEBUG] Exporting...", Color.Orange)

                                                         MethodEx.ExportInfo = New MethodExportInfo()
                                                         MethodEx.IsUnmanagedExport = True

                                                         WriteLog("DEBUG] Module Writing Options...", Color.Orange)

                                                         Dim opts As ModuleWriterOptions = New ModuleWriterOptions(AssemblyModule)
                                                         opts.PEHeadersOptions.Machine = MachineTypeList(ComboBox3Index)
                                                         opts.Cor20HeaderOptions.Flags = 0

                                                         WriteLog("[INFO] Writing module '" & IO.Path.GetFileName(SavePath) & "'...", Color.Orange)

                                                         WriteLog("[INFO] Exporting...", Color.YellowGreen)

                                                         Dim TempFileName As String = IO.Path.Combine(IO.Path.GetTempPath, TargetSaveName)
                                                         AssemblyModule.Write(TempFileName, opts)

                                                         Dim Input As New FileStream(TempFileName, FileMode.Open, FileAccess.Read)
                                                         WriteLog("[INFO] Exporting and Reading...", Color.YellowGreen)

                                                         Dim Reader As New BinaryReader(Input)
                                                         WriteLog("[INFO] Reading Byte x Byte...", Color.YellowGreen)

                                                         Dim FileBytes() As Byte = Reader.ReadBytes(CInt(Input.Length))
                                                         WriteLog("[INFO] Readed!", Color.YellowGreen)


                                                         Dim ListHexStr As List(Of String) = Core.Encodings.HexEncoding.GetString(FileBytes) 'Core.Utils.BytesToHex(FileBytes)

                                                         Dim StringMemory As New System.Text.StringBuilder()
                                                         Dim MaxStr As Integer = 0
                                                         Dim Separator As String = ", "
                                                         Dim IntCount As Integer = 0
                                                         For Each HexByte As String In ListHexStr
                                                             If MaxStr = ListHexStr.Count Then
                                                                 Separator = String.Empty
                                                             End If
                                                             If IntCount >= 20 Then
                                                                 StringMemory.Append("0x" & HexByte & Separator & vbNewLine)
                                                                 IntCount = 0
                                                             Else
                                                                 StringMemory.Append("0x" & HexByte & Separator)
                                                                 IntCount += 1
                                                             End If
                                                             MaxStr += 1
                                                         Next

                                                         WriteLog("[INFO] Hex Converting...", Color.YellowGreen)

                                                         Dim StubStr As String = Core.Utils.FileReadText("Stub.c")
                                                         WriteLog("[INFO] Creating Stub...", Color.YellowGreen)

                                                         If StubStr = String.Empty Then
                                                             StubStr = My.Resources.BaseLinker
                                                             Core.Utils.FileWriteText("Stub.c", StubStr)
                                                         End If

                                                         StubStr = StubStr.Replace("$DataLength$", FileBytes.Length)
                                                         StubStr = StubStr.Replace("$DataByte$", StringMemory.ToString())
                                                         StubStr = StubStr.Replace("$DllName$", TargetSaveName)
                                                         StubStr = StubStr.Replace("$DllMain$", MethodEx.Name)

                                                         Dim StubTempFile As String = IO.Path.Combine(IO.Path.GetTempPath, "StubTemp.c")
                                                         If IO.File.Exists(StubTempFile) Then IO.File.Delete(StubTempFile)

                                                         Core.Utils.FileWriteText(StubTempFile, StubStr)
                                                         WriteLog("[INFO] Writing Stub...", Color.YellowGreen)

                                                         Dim Debuger As IO.FileInfo = New IO.FileInfo(IO.Path.Combine("Bin", "tcc.exe"))

                                                         WriteLog("[Executing] Compiling... " & Debuger.FullName, Color.White)

                                                         If IO.File.Exists(Debuger.FullName) = False Then
                                                             WriteLog("[Error]  Engine Tcc Not Found", Color.Red)
                                                             Loading = False
                                                             Exit Sub
                                                         End If

                                                         Dim TccResult As String = RuntccHost(Debuger.FullName, StubTempFile, SavePath, Args)

                                                         WriteLog("[DEBUG] " & TccResult, Color.White)

                                                         WriteLog("[DEBUG] Saving to '" & SavePath & "'...", Color.Violet)
                                                         WriteLog("[INFO] Done.", Color.Lime)

                                                     Catch ex As Exception
                                                         WriteLog(ex.Message, Color.Red)
                                                     End Try

                                                 End If

                                                 Loading = False
                                             End Sub), TaskCreationOptions.PreferFairness)
        Asynctask.Start()


    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Dim FileEx As New IO.FileInfo(OpenFileDialog1.FileName)
        AssemblyModule = ModuleDefMD.Load(FileEx.FullName)

        If AssemblyModule.IsILOnly = True Then

            WriteLog("[INFO] Assembly Module Is IL Only", Color.LimeGreen)

            Guna2ComboBox1.Items.Clear()
            Guna2TextBox1.Text = FileEx.FullName

            WriteLog("[INFO] Loading Methods...", Color.Silver)

            Dim TypesClass As IEnumerable(Of TypeDef) = AssemblyModule.GetTypes()
            Dim MethodListStr As New List(Of String)

            For Each Classes As TypeDef In TypesClass

                Dim MethodList As IList(Of MethodDef) = Classes.Methods

                For Each Method As MethodDef In MethodList

                    If Method.IsStatic = True Then

                        WriteLog("[INFO] Method: " & Method.Name.ToString & " Loaded!", Color.White)
                        MethodListStr.Add(Classes.Name.ToString & "." & Method.Name.ToString)

                    End If

                Next

            Next

            If MethodListStr.Count = 0 Then

                WriteLog("[Error] No Methods Found.", Color.Red)

            Else

                Dim MethodsSorted() As String = MethodListStr.OrderBy(Function(x) x).ToArray

                Guna2ComboBox1.Items.AddRange(MethodsSorted)

                If Not Guna2ComboBox1.Items.Count = 0 Then
                    Guna2ComboBox1.SelectedIndex = 0
                End If

            End If



        Else
            WriteLog("[Error] File is not IL Only", Color.Red)
        End If


        '  Classes.Methods.FirstOrDefault(Function(m) m.Name = "Execute")


    End Sub

#End Region

#Region " Private Methods "

    Public Function RuntccHost(ByVal tcc As String, ByVal Stub As String, ByVal Out As String, Optional ByVal Argument As String = "") As String
        Try
            Dim FullArguments As String = """" & Stub & """" & Argument & " -o " & """" & Out & """"
            Dim cmdProcess As New Process
            With cmdProcess
                .StartInfo = New ProcessStartInfo(tcc, FullArguments)
                With .StartInfo
                    .CreateNoWindow = True
                    .UseShellExecute = False
                    .RedirectStandardOutput = True
                    .RedirectStandardError = True
                End With
                .Start()
                .WaitForExit()
            End With

            Dim HostOutput As String = cmdProcess.StandardOutput.ReadToEnd.ToString & vbNewLine & cmdProcess.StandardError.ReadToEnd.ToString

            Return HostOutput.ToString
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private Sub WriteLog(ByVal text As String,
                          ByVal forecolor As Color,
                          Optional ByVal backcolor As Color = Nothing,
                          Optional ByVal font As Font = Nothing)
        Me.BeginInvoke(Sub()
                           Dim index As Int32 = Guna2TextBox2.TextLength

                           '  Guna2TextBox2.Text += text
                           Guna2TextBox2.AppendText(text & vbNewLine)
                           Guna2TextBox2.SelectionStart = index
                           Guna2TextBox2.SelectionLength = Guna2TextBox2.TextLength - index
                           Guna2TextBox2.ForeColor = forecolor

                           If Not backcolor = Nothing _
                           Then Guna2TextBox2.BackColor = backcolor _
                           Else Guna2TextBox2.BackColor = DefaultBackColor

                           If font IsNot Nothing Then Guna2TextBox2.Font = font

                           ' Reset selection
                           Guna2TextBox2.SelectionStart = Guna2TextBox2.TextLength
                           Guna2TextBox2.SelectionLength = 0
                       End Sub)
    End Sub

#End Region

End Class