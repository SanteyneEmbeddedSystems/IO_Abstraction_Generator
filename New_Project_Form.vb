Imports System.Drawing
Imports System.Windows.Forms

Public Class New_Project_Form
    Inherits IO_Abstraction_Generator_Form

    Private File_Name_TextBox As TextBox
    Private Directory_TextBox As TextBox
    Private WithEvents Directory_Button As Button
    Private WithEvents OK_Button As Button

    Private Shared ReadOnly Project_File_Extension As String = ".ioab"

    Private Const Path_Button_Width As Integer = 30
    Private Const Path_Button_Height As Integer = Item_Height
    Private Shared Path_Button_Size As New Size(Path_Button_Width, Path_Button_Height)
    Private Const Path_Button_X_Pos As Integer = Marge + Path_Text_Width + Marge
    Private Const Path_Text_Width As Integer = Panel_Width - Path_Button_Width - 3 * Marge
    Private Const Path_Text_Height As Integer = Item_Height
    Private Shared Path_Text_Size As New Size(Path_Text_Width, Path_Text_Height)

    Private Const Button_Width As Integer = 100
    Private Const Button_Height As Integer = 3 * Marge
    Private Shared Button_Size As New Size(Button_Width, Button_Height)


    Public Sub New()


        Dim item_y_pos As Integer = 0
        Dim inner_item_y_pos As Integer = Marge

        '------------------------------------------------------------------------------------------'
        ' Add file directory selection panel
        inner_item_y_pos = Marge

        Dim dir_label As New Label With {
            .Text = "Directory",
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Label_Size}
        inner_item_y_pos += dir_label.Height

        Me.Directory_TextBox = New TextBox With {
            .BackColor = Background_Color,
            .ForeColor = Foreground_Color,
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Path_Text_Size,
            .Text = ""}

        Me.Directory_Button = New Button With {
            .BackColor = Foreground_Color,
            .ForeColor = Background_Color,
            .Location = New Point(Path_Button_X_Pos, inner_item_y_pos),
            .Size = Path_Button_Size,
            .Text = "..."}
        inner_item_y_pos += Me.Directory_TextBox.Height + Marge

        Dim dir_panel As New Panel With {
            .Location = New Point(Marge, item_y_pos),
            .BorderStyle = BorderStyle.FixedSingle,
            .Size = New Size(Panel_Width, inner_item_y_pos)}
        With dir_panel.Controls
            .Add(dir_label)
            .Add(Me.Directory_TextBox)
            .Add(Me.Directory_Button)
        End With
        Me.Controls.Add(dir_panel)
        item_y_pos += dir_panel.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Add file name panel
        inner_item_y_pos = Marge

        Dim file_label As New Label With {
            .Text = "File name",
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Label_Size}
        inner_item_y_pos += file_label.Height

        Me.File_Name_TextBox = New TextBox With {
            .BackColor = Background_Color,
            .ForeColor = Foreground_Color,
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Label_Size,
            .Text = "new_project"}
        inner_item_y_pos += Me.File_Name_TextBox.Height + Marge

        Dim file_panel As New Panel With {
            .Location = New Point(Marge, item_y_pos),
            .BorderStyle = BorderStyle.FixedSingle,
            .Size = New Size(Panel_Width, inner_item_y_pos)}
        With file_panel.Controls
            .Add(file_label)
            .Add(Me.File_Name_TextBox)
        End With
        Me.Controls.Add(file_panel)
        item_y_pos += file_panel.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Design Create button
        Me.OK_Button = New Button With {
            .Text = "OK",
            .BackColor = Foreground_Color,
            .ForeColor = Background_Color,
            .Size = Button_Size,
            .Location = New Point((Form_Width - Button_Width) \ 2, item_y_pos)}
        Me.Controls.Add(Me.OK_Button)

        item_y_pos += Me.OK_Button.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Design Form
        Me.ClientSize = New Size(Form_Width, item_y_pos)

    End Sub

    Public Function Get_File_Full_Path() As String
        Return Me.Directory_TextBox.Text _
            & Path.DirectorySeparatorChar _
            & Me.File_Name_TextBox.Text _
            & Project_File_Extension
    End Function

    Public Function Get_Project_Name() As String
        Return Me.File_Name_TextBox.Text
    End Function

    Private Sub Path_Button_Clicked() Handles Directory_Button.Click
        Select_Directory("Choose directory", Me.Directory_TextBox)
    End Sub

    Private Function Check_Directory() As Boolean
        Dim directory_is_valid As Boolean = True
        If File.Exists(Me.Directory_TextBox.Text & Path.DirectorySeparatorChar &
                Me.File_Name_TextBox.Text) Then
            MsgBox(
                Me.File_Name_TextBox.Text & " already exists in " & Me.Directory_TextBox.Text,
                MsgBoxStyle.Critical)
            directory_is_valid = False
        ElseIf Not path.Exists(Me.Directory_TextBox.Text) Then
            MsgBox(
                "Directory does not exist",
                MsgBoxStyle.Critical)
            directory_is_valid = False
        End If
        Return directory_is_valid
    End Function

    Private Sub OK_Button_Clicked() Handles OK_Button.Click
        If True = Check_Directory() Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

End Class
