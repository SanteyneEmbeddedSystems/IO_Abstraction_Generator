Imports System.Drawing
Imports System.Environment
Imports System.Windows.Forms

Public MustInherit Class IO_Abstraction_Generator_Form
    Inherits Form

    Protected Const Marge As Integer = 10
    Protected Const Form_Width As Integer = 600
    Protected Const Item_Height As Integer = 2 * Marge
    Protected Const Panel_Width As Integer = Form_Width - 2 * Marge

    Protected Shared ReadOnly Background_Color As Color = Color.Black
    Protected Shared ReadOnly Foreground_Color As Color = Color.Gray

    Protected Const Label_Width As Integer = (Panel_Width - 2 * Marge) / 2
    Protected Const Label_Height As Integer = Item_Height
    Protected Shared Label_Size As New Size(Label_Width, Label_Height)

    Protected Shared Sub Select_Directory(
        title As String,
        ByRef directory_textbox As TextBox)
        Dim dialog_box = New FolderBrowserDialog
        If Directory.Exists(directory_textbox.Text) Then
            dialog_box.SelectedPath = directory_textbox.Text
        Else
            dialog_box.SelectedPath = GetFolderPath(SpecialFolder.UserProfile)
        End If
        dialog_box.Description = title
        Dim result As DialogResult = dialog_box.ShowDialog()
        If result = DialogResult.OK Then
            directory_textbox.Text = dialog_box.SelectedPath
        End If
    End Sub

End Class
