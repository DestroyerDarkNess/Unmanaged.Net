Public Class Form1

#Region " Declare "

    Public HomeForm As Home = New Home With {.Name = "HomeForm", .TopLevel = False, .Visible = True}
    Public Injector As InjectorTester = New InjectorTester With {.Name = "Injector", .TopLevel = False, .Visible = False}
    Public AboutForm As About = New About With {.Name = "About", .TopLevel = False, .Visible = False}

#End Region

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartUI()
    End Sub

#Region " GUI "

    Public Sub StartUI()

        PanelContainer.Controls.Add(HomeForm)
        PanelContainer.Controls.Add(Injector)
        PanelContainer.Controls.Add(AboutForm)

    End Sub

    Private Sub Guna2Button1_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2Button1.CheckedChanged
        If Guna2Button1.Checked = True Then
            HideControlPanel(HomeForm.Name)
        End If
    End Sub

    Private Sub Guna2Button2_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2Button2.CheckedChanged
        If Guna2Button2.Checked = True Then
            HideControlPanel(AboutForm.Name)
        End If
    End Sub

    Private Sub Guna2Button3_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2Button3.CheckedChanged
        If Guna2Button3.Checked = True Then
            HideControlPanel(Injector.Name)
        End If
    End Sub

    Private Sub HideControlPanel(Optional ByVal NameToExclude As String = Nothing)
        For Each Ccontrol As Control In PanelContainer.Controls
            If Ccontrol.Name = NameToExclude Then
                Ccontrol.Visible = True
            Else
                Ccontrol.Visible = False
            End If
        Next
    End Sub

#End Region



End Class
