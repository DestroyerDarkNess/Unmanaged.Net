Imports System.Text.RegularExpressions
Imports System.Net
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Data = DInvoke.Data
Imports DynamicInvoke = DInvoke.DynamicInvoke
Imports ManualMap = DInvoke.ManualMap
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Core
    Public Class Utils

#Region " Resize Image "

        ' [ Save Resize Image Function ]
        '
        ' Examples :
        '
        ' PictureBox1.Image = Resize_Image(System.Drawing.Image.FromFile("C:\Image.png"), 256, 256)

        Public Shared Function Resize_Image(ByVal img As Image, ByVal Width As Int32, ByVal Height As Int32) As Bitmap
            Dim Bitmap_Source As New Bitmap(img)
            Dim Bitmap_Dest As New Bitmap(CInt(Width), CInt(Height))
            Dim Graphic As Graphics = Graphics.FromImage(Bitmap_Dest)
            Graphic.DrawImage(Bitmap_Source, 0, 0, Bitmap_Dest.Width + 1, Bitmap_Dest.Height + 1)
            Return Bitmap_Dest
        End Function

#End Region

#Region " Set Control Hint Function "

        ' [ Set Control Hint Function ]
        '
        ' Examples :
        ' Set_Control_Hint(TextBox1, "Put text here...")

        <System.Runtime.InteropServices.DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)>
        Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)> ByVal lParam As String) As Int32
        End Function

        Public Shared Function Set_Control_Hint(ByVal control As Control, ByVal text As String) As Boolean
            Try
                SendMessage(control.Handle, &H1501, 0, text)
                Return True
            Catch ex As Exception
                Return False
                ' Throw New Exception(ex.Message)
            End Try
        End Function

        Public Shared Function Set_Object_Hint(ByVal control As Object, ByVal text As String) As Boolean
            Try
                SendMessage(control.Handle, &H1501, 0, text)
                Return True
            Catch ex As Exception
                Return False
                ' Throw New Exception(ex.Message)
            End Try
        End Function

#End Region

        Public Shared Function CenterForm(ByVal ParentForm As Form, ByVal Form_to_Center As Form, ByVal Form_Location As Point) As Point
            Dim FormLocation As New Point
            FormLocation.X = (ParentForm.Left + (ParentForm.Width - Form_to_Center.Width) / 2) ' set the X coordinates.
            FormLocation.Y = (ParentForm.Top + (ParentForm.Height - Form_to_Center.Height) / 2) ' set the Y coordinates.
            Return FormLocation ' return the Location to the Form it was called from.
        End Function

        Public Shared Function OpenFiles() As List(Of String)
            Dim OpenFileDialog1 As New OpenFileDialog
            ' OpenFileDialog1.DefaultExt = "txt"
            OpenFileDialog1.Multiselect = True
            OpenFileDialog1.FileName = ""
            '  OpenFileDialog1.InitialDirectory = "c:\"
            OpenFileDialog1.Title = "Select file"
            OpenFileDialog1.Filter = "Dll Files|*.dll|" & "ASI (GTAs Mod)|*.asi|" &
                "All files Suported|*.dll;*.asi"


            Dim ListFiles As New List(Of String)

            If Not OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then
                ListFiles.AddRange(OpenFileDialog1.FileNames)
                Return ListFiles
            End If

            Return Nothing

        End Function

#Region " Mutation File "

        Public Shared Function MutateFile(ByVal TargetFile As String, ByVal DestinationDir As String) As String
            Try
                Dim FileExtension As String = IO.Path.GetExtension(TargetFile)
                Dim SourceData() As Byte = IO.File.ReadAllBytes(TargetFile)
                Dim RandomBytes() As Byte = System.Text.ASCIIEncoding.UTF8.GetBytes(RandomString(TimeOfDay.Second))
                Dim MutatedArray() As Byte = ArrayConcat(SourceData, RandomBytes)
                Dim OuputFile As String = IO.Path.Combine(DestinationDir, RandomString(5) & FileExtension)
                If My.Computer.FileSystem.FileExists(OuputFile) Then
                    My.Computer.FileSystem.DeleteFile(OuputFile)
                End If
                My.Computer.FileSystem.WriteAllBytes(OuputFile, MutatedArray, False)
                Return OuputFile
            Catch ex As Exception
                Return String.Empty
            End Try
        End Function

        Private Shared Function ArrayConcat(ByVal x() As Byte, ByVal y() As Byte) As Byte()
            Dim newx(x.Length + y.Length - 1) As Byte
            x.CopyTo(newx, 0)
            y.CopyTo(newx, x.Length)
            Return newx
        End Function

        Public Shared Function RandomString(ByVal length As Integer) As String
            Dim random As New Random()
            Dim charOutput As Char() = New Char(length - 1) {}
            For i As Integer = 0 To length - 1
                Dim selector As Integer = random.[Next](65, 101)
                If selector > 90 Then
                    selector -= 43
                End If
                charOutput(i) = Convert.ToChar(selector)
            Next
            Return New String(charOutput)
        End Function

#End Region

#Region " Launch Process "

        Public Shared Function ExecuteProgram(ByVal Executable As String, ByVal ArgsInfo As String, ByVal Shell As Boolean) As Process
            Try
                Dim MyProcess As Process =
          New Process With {.StartInfo =
              New ProcessStartInfo With {
                  .UseShellExecute = Shell
              }
          }

                With MyProcess

                    .StartInfo.FileName = Executable
                    .StartInfo.Arguments = ArgsInfo
                    .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    .Start()

                End With

                Return MyProcess
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

#End Region

#Region " GrayScale Image Function "

        ' [ GrayScale Image Function ]
        '
        ' Examples:
        '
        ' PictureBox1.Image = GrayScale_Image(PictureBox1.Image, GrayScale.Light_Gray)
        ' PictureBox1.Image = GrayScale_Image(PictureBox1.Image, GrayScale.Mid_Gray)
        ' PictureBox1.Image = GrayScale_Image(PictureBox1.Image, GrayScale.Dark_Gray)

        Enum GrayScale
            Light_Gray
            Mid_Gray
            Dark_Gray
        End Enum

        Public Shared Function GrayScale_Image(ByVal Image As Image, ByVal Gray_Tone As GrayScale) As Bitmap
            Dim Image_Bitmap As Bitmap = New Bitmap(Image.Width, Image.Height)
            Dim Image_Graphic As Graphics = Graphics.FromImage(Image_Bitmap)
            Dim Color_Matrix As System.Drawing.Imaging.ColorMatrix = Nothing
            Select Case Gray_Tone
                Case GrayScale.Light_Gray : Color_Matrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0.5, 0.5, 0.5, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
                Case GrayScale.Mid_Gray : Color_Matrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0.5, 0.5, 0.5, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
                Case GrayScale.Dark_Gray : Color_Matrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
            End Select
            Dim Image_Attributes As System.Drawing.Imaging.ImageAttributes = New System.Drawing.Imaging.ImageAttributes()
            Image_Attributes.SetColorMatrix(Color_Matrix)
            Image_Graphic.DrawImage(Image, New Rectangle(0, 0, Image.Width, Image.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, Image_Attributes)
            Image_Graphic.Dispose()
            Return Image_Bitmap
        End Function

#End Region

#Region " Center Form To Desktop "

        ' [ Center Form To Desktop ]
        '
        ' // By Elektro H@cker
        '
        ' Examples :
        ' Center_Form_To_Desktop(Me)

        Public Shared Sub Center_Form_To_Desktop(ByVal Form As Form)
            Dim Desktop_RES As System.Windows.Forms.Screen = System.Windows.Forms.Screen.PrimaryScreen
            Form.Location = New Point((Desktop_RES.Bounds.Width - Form.Width) / 2, (Desktop_RES.Bounds.Height - Form.Height) / 2)
        End Sub

        Public Shared Function IsFolder(ByVal path As String) As Boolean
            Return ((IO.File.GetAttributes(path) And IO.FileAttributes.Directory) = IO.FileAttributes.Directory)
        End Function

#End Region

#Region " Base64 Functions "

        Public Shared Function ConvertImageToBase64String(ByVal ImageL As Image) As String
            Using ms As New MemoryStream()
                ImageL.Save(ms, System.Drawing.Imaging.ImageFormat.Png) 'We load the image from first PictureBox in the MemoryStream
                Dim obyte = ms.ToArray() 'We tranform it to byte array..

                Return Convert.ToBase64String(obyte) 'We then convert the byte array to base 64 string.
            End Using
        End Function

        Public Shared Function ConvertBase64ToByteArray(base64 As String) As Byte()
            Return Convert.FromBase64String(base64) 'Convert the base64 back to byte array.
        End Function

        'Here's the part of your code (which works)
        Public Shared Function convertbytetoimage(ByVal BA As Byte())
            Dim ms As MemoryStream = New MemoryStream(BA)
            Dim image = System.Drawing.Image.FromStream(ms)
            Return image
        End Function

#End Region

        <DllImport("psapi.dll", SetLastError:=True)>
        Public Shared Function EnumProcessModules(ByVal hProcess As IntPtr, <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.U4)> <[In]()> <Out()> ByVal lphModule As IntPtr, ByVal cb As UInteger, <MarshalAs(UnmanagedType.U4)> ByRef lpcbNeeded As UInteger) As Boolean
        End Function

        <DllImport("psapi.dll")>
        Public Shared Function GetModuleFileNameEx(ByVal hProcess As IntPtr, ByVal hModule As IntPtr, <Out()> ByVal lpBaseName As StringBuilder, <[In]()> <MarshalAs(UnmanagedType.U4)> ByVal nSize As Integer) As UInteger
        End Function


        Public Shared Function GetNativeProcessModules(ByVal Process As Process) As List(Of String)
            Dim DataS As New List(Of String)
            Try
                Dim hMods As IntPtr() = New IntPtr(1023) {}
                Dim gch As GCHandle = GCHandle.Alloc(hMods, GCHandleType.Pinned)
                Dim pModules As IntPtr = gch.AddrOfPinnedObject()
                Dim uiSize As UInteger = CUInt((Marshal.SizeOf(GetType(IntPtr)) * (hMods.Length)))
                Dim cbNeeded As UInteger = 0

                If EnumProcessModules(Process.Handle, pModules, uiSize, cbNeeded) = 1 Then
                    Dim uiTotalNumberofModules As Int32 = CType((cbNeeded / (Marshal.SizeOf(GetType(IntPtr)))), Int32)


                    For i As Integer = 0 To CInt(uiTotalNumberofModules) - 1

                        Dim strbld As StringBuilder = New StringBuilder(1024)
                        GetModuleFileNameEx(Process.Handle, hMods(i), strbld, CUInt((strbld.Capacity)))

                        DataS.Add(strbld.ToString)

                    Next
                    gch.Free()
                    Return DataS

                End If
                gch.Free()
                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Shared Function Is64BitPE(ByVal filePath As String) As Boolean
            Try

                Using stream = New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

                    Using reader = New BinaryReader(stream)
                        reader.BaseStream.Position = &H3C
                        Dim peOffset As UInteger = reader.ReadUInt32()
                        reader.BaseStream.Position = peOffset + &H4
                        Dim machine As UShort = reader.ReadUInt16()
                        If machine <> &H14C AndAlso machine <> &H8664 Then Throw New InvalidDataException()
                        Return (machine = &H8664)
                    End Using
                End Using

                Return False
            Catch
                Return False
            End Try
        End Function


        <DllImport("kernel32.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi)>
        Public Shared Function IsWow64Process(
    <[In]> ByVal hProcess As IntPtr, <Out> ByRef lpSystemInfo As Boolean) As Boolean
        End Function

        Public Shared Function Is64Bit(ByVal Proc As Process) As Boolean
            Dim retVal As Boolean
            IsWow64Process(Proc.Handle, retVal)
            Return retVal
        End Function

        Public Shared Sub RunAsAdmin(ByVal DirPathA As String, Optional Argument As String = "", Optional WindowsStyle As ProcessWindowStyle = ProcessWindowStyle.Normal)
            Try
                Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo()
                processStartInfo.FileName = DirPathA

                processStartInfo.Verb = "runas"
                If Not Argument = "" Then
                    processStartInfo.Arguments = Argument
                End If
                processStartInfo.WindowStyle = WindowsStyle
                processStartInfo.UseShellExecute = True
                Dim process As Process = Process.Start(processStartInfo)
                process.WaitForExit()
                '  Core.Instances.MainInstance.StartPluginManager()
            Catch ex As Exception
                Process.Start(DirPathA, Argument)
            End Try
        End Sub

        Public Shared Function GetCornerScreen(ByVal Frm As Form) As Point
            Dim num As Integer = Screen.PrimaryScreen.WorkingArea.Width - Frm.Width
            Dim num2 As Integer = 4 ' Screen.PrimaryScreen.WorkingArea.Height  - Frm.Height
            Dim result As Point = New Point(num - 4, num2)
            Return result
        End Function

        Public Shared Function OpenFile(Optional ByVal Filter As String = "All files|*.*") As String
            Dim OpenFileDialog1 As New OpenFileDialog
            OpenFileDialog1.Multiselect = False
            ' OpenFileDialog1.DefaultExt = "txt"
            OpenFileDialog1.FileName = ""
            '  OpenFileDialog1.InitialDirectory = "c:\"
            OpenFileDialog1.Title = "Select file"
            OpenFileDialog1.Filter = Filter

            If Not OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then
                Return OpenFileDialog1.FileName
            End If

            Return Nothing

        End Function


        Public Shared Async Function DownloadImageAsync(ByVal UrlImg As String) As Task(Of Image)
            Try
                Dim WebpImageData() As Byte = New System.Net.WebClient().DownloadData(UrlImg)
                Dim Webpstream As IO.MemoryStream = New IO.MemoryStream(WebpImageData)
                Dim ToBitmap As Bitmap = New Bitmap(Webpstream)
                Return ToBitmap
            Catch ex As Exception
                Exeptions = ex.Message
                Return Nothing
            End Try
        End Function

#Region " My Application Is Already Running "

        ' [ My Application Is Already Running Function ]
        '
        ' // By Elektro H@cker
        '
        ' Examples :
        ' MsgBox(My_Application_Is_Already_Running)
        ' If My_Application_Is_Already_Running() Then Application.Exit()

        Public Declare Function CreateMutexA Lib "Kernel32.dll" (ByVal lpSecurityAttributes As Integer, ByVal bInitialOwner As Boolean, ByVal lpName As String) As Integer
        Public Declare Function GetLastError Lib "Kernel32.dll" () As Integer

        Public Shared Function My_Application_Is_Already_Running() As Boolean
            'Attempt to create defualt mutex owned by process
            CreateMutexA(0, True, Process.GetCurrentProcess().MainModule.ModuleName.ToString)
            Return (GetLastError() = 183) ' 183 = ERROR_ALREADY_EXISTS
        End Function

#End Region

        Public Shared Exeptions As String = String.Empty


        Public Shared Function GetNum(value As String) As Integer
            Dim text As String = String.Empty
            Dim matchCollection As MatchCollection = Regex.Matches(value, "\d+")
            Dim enumerator As IEnumerator = matchCollection.GetEnumerator()
            While enumerator.MoveNext()
                Dim match As Match = CType(enumerator.Current, Match)
                text += match.ToString()
            End While
            Return Convert.ToInt32(text)
        End Function

        Public Shared Function IsBinary(ByVal filePath As String, ByVal Optional requiredConsecutiveNul As Integer = 1) As Boolean
            Const charsToCheck As Integer = 8000
            Const nulChar As Char = vbNullChar
            Dim nulCount As Integer = 0

            Using streamReaderEx = New StreamReader(filePath)

                For i = 0 To charsToCheck - 1
                    If streamReaderEx.EndOfStream Then Return False

                    If Microsoft.VisualBasic.ChrW(streamReaderEx.Read()) = nulChar Then
                        nulCount += 1
                        If nulCount >= requiredConsecutiveNul Then Return True
                    Else
                        nulCount = 0
                    End If
                Next
            End Using

            Return False
        End Function

        Public Shared Function IsDocument(ByVal filePath As String) As Boolean
            Try

                Select Case LCase(IO.Path.GetExtension(filePath))
                    Case ".txt" : Return True
                    Case ".doc" : Return True
                    Case ".docx" : Return True
                    Case ".wpd" : Return True
                    Case ".php" : Return True
                    Case ".vbs" : Return True
                    Case ".odt" : Return True
                    Case ".pdf" : Return True
                    Case ".xls" : Return True
                    Case ".xlsx" : Return True
                    Case ".ods" : Return True
                    Case ".ppt" : Return True
                    Case ".pptx" : Return True
                    Case ".wp5" : Return True
                    Case Else
                        Return False
                End Select

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function FileReadText(ByVal FileDir As String) As String
            Try
                Dim swEx As New IO.StreamReader(FileDir, False)
                Dim ReadAllText As String = swEx.ReadToEnd
                swEx.Close()
                Return ReadAllText
            Catch ex As Exception
                Return String.Empty
            End Try
        End Function

        Public Shared Function FileWriteText(ByVal FileDir As String, Optional ByVal ContentText As String = "") As Boolean
            Try
                Dim swEx As New IO.StreamWriter(FileDir, False)
                swEx.Write(ContentText)
                swEx.Close()
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

#Region " Is Connectivity Avaliable? function "

        ' [ Is Connectivity Avaliable? Function ]
        '
        ' // By Elektro H@cker
        '
        ' Examples :
        ' MsgBox(Is_Connectivity_Avaliable())
        ' While Not Is_Connectivity_Avaliable() : Application.DoEvents() : End While

        Private Function Is_Connectivity_Avaliable()

            Dim WebSites() As String = {"Google.com", "Facebook.com", "Microsoft.com"}

            If My.Computer.Network.IsAvailable Then
                For Each WebSite In WebSites
                    Try
                        My.Computer.Network.Ping(WebSite)
                        Return True ' Network connectivity is OK.
                    Catch : End Try
                Next
                Return False ' Network connectivity is down.
            Else
                Return False ' No network adapter is connected.
            End If

        End Function

#End Region

#Region " Sleep "

        ' [ Sleep ]
        '
        ' // By Elektro H@cker
        '
        ' Examples :
        ' Sleep(5) : MsgBox("Test")
        ' Sleep(5, Measure.Seconds) : MsgBox("Test")

        Public Enum Measure
            Milliseconds = 1
            Seconds = 2
            Minutes = 3
            Hours = 4
        End Enum

        Public Shared Sub Sleep(ByVal Duration As Int64, Optional ByVal Measure As Measure = Measure.Seconds)

            Dim Starttime = DateTime.Now

            Select Case Measure
                Case Measure.Milliseconds : Do While (DateTime.Now - Starttime).TotalMilliseconds < Duration : Application.DoEvents() : Loop
                Case Measure.Seconds : Do While (DateTime.Now - Starttime).TotalSeconds < Duration : Application.DoEvents() : Loop
                Case Measure.Minutes : Do While (DateTime.Now - Starttime).TotalMinutes < Duration : Application.DoEvents() : Loop
                Case Measure.Hours : Do While (DateTime.Now - Starttime).TotalHours < Duration : Application.DoEvents() : Loop
                Case Else
            End Select

        End Sub

#End Region

#Region " CenterForm function "

        Public Shared Function CenterForm(ByVal ParentForm As Form, ByVal Form_to_Center As Form) As Point
            Try
                Dim FormLocation As New Point
                FormLocation.X = (ParentForm.Left + (ParentForm.Width - Form_to_Center.Width) / 2) ' set the X coordinates.
                FormLocation.Y = (ParentForm.Top + (ParentForm.Height - Form_to_Center.Height) / 2) ' set the Y coordinates.
                Return FormLocation ' return the Location to the Form it was called from.
            Catch ex As Exception
                Return New Point((Screen.PrimaryScreen.WorkingArea.Width / 2) - Form_to_Center.Width, (Screen.PrimaryScreen.WorkingArea.Height / 2) - Form_to_Center.Height)
            End Try
        End Function

        Public Shared Function CenterControl(ByVal ParentForm As Control, ByVal Form_to_Center As Control, ByVal Form_Location As Point) As Point
            Dim FormLocation As New Point
            FormLocation.X = (ParentForm.Left + (ParentForm.Width - Form_to_Center.Width) / 2) ' set the X coordinates.
            FormLocation.Y = (ParentForm.Top + (ParentForm.Height - Form_to_Center.Height) / 2) ' set the Y coordinates.
            Return FormLocation ' return the Location to the Form it was called from.
        End Function

#End Region

#Region " PE Checker "

        ' Usage Examples:
        '
        ' MsgBox(IsNetAssembly("C:\File.exe"))
        ' MsgBox(IsNetAssembly("C:\File.dll"))

        ''' <summary>
        ''' Gets the common language runtime (CLR) version information of the specified file, using the specified buffer.
        ''' </summary>
        ''' <param name="filepath">Indicates the filepath of the file to be examined.</param>
        ''' <param name="buffer">Indicates the buffer allocated for the version information that is returned.</param>
        ''' <param name="buflen">Indicates the size, in wide characters, of the buffer.</param>
        ''' <param name="written">Indicates the size, in bytes, of the returned buffer.</param>
        ''' <returns>System.Int32.</returns>
        <System.Runtime.InteropServices.DllImport("mscoree.dll",
        CharSet:=System.Runtime.InteropServices.CharSet.Unicode)>
        Private Shared Function GetFileVersion(
                          ByVal filepath As String,
                          ByVal buffer As System.Text.StringBuilder,
                          ByVal buflen As Integer,
                          ByRef written As Integer
        ) As Integer
        End Function

        ''' <summary>
        ''' Determines whether an exe/dll file is an .Net assembly.
        ''' </summary>
        ''' <param name="File">Indicates the exe/dll file to check.</param>
        ''' <returns><c>true</c> if file is an .Net assembly; otherwise, <c>false</c>.</returns>
        Public Shared Function IsNetAssembly(ByVal [File] As String) As Boolean
            Try
                Dim assembly As AssemblyName = AssemblyName.GetAssemblyName([File])
                Return True
            Catch ex As Exception
                Return False
            End Try
            '  Dim sb = New System.Text.StringBuilder(256)
            '  Dim written As Integer = 0
            '  Dim hr = GetFileVersion([File], sb, sb.Capacity, written)
            ' Return hr = 0
        End Function

        Public Shared Function GetProcessModules() As List(Of String)
            '  Dim Modules As New List(Of String)
            '  Dim 
            ' Modules.Add()
        End Function

#End Region

        Public Shared Function GetDataPage(ByVal Url As String) As String
            Try
                Dim UrlHost As String = New Uri(Url).Host
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                Dim cookieJar As CookieContainer = New CookieContainer()
                Dim request As HttpWebRequest = CType(WebRequest.Create(Url), HttpWebRequest)
                request.UseDefaultCredentials = True
                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials
                request.CookieContainer = cookieJar
                request.Accept = "text/html, application/xhtml+xml, */*"
                request.Referer = "https://" + UrlHost + "/"
                request.Headers.Add("Accept-Language", "en-GB")
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)"
                request.Host = UrlHost
                Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Dim htmlString As String = String.Empty

                Using reader = New StreamReader(response.GetResponseStream())
                    htmlString = reader.ReadToEnd()
                End Using

                Return htmlString
            Catch ex As Exception
                Return String.Empty
            End Try
        End Function

        Public Shared Function BytesToHex(ByVal Input As Byte()) As String
            Dim Result As New System.Text.StringBuilder(Input.Length * 2)
            Dim ItemCounter As Integer = 0
            For Each b As Byte In Input

                If Not Result.ToString = "" Then Result.Append(",")

                If ItemCounter >= 11 Then
                    Result.Append(vbNewLine)
                    ItemCounter = 0
                Else
                    ItemCounter += 1
                End If

                Dim Part As String = Conversion.Hex(b)
                If Part.Length = 1 Then Part = "0" & Part
                Result.Append("0x" & Part)
            Next
            Return Result.ToString()
        End Function

    End Class
End Namespace

