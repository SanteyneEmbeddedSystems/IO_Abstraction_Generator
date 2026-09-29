Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

Imports IO_Abstraction_Generator.Analog_Input.E_ANALOG_INPUT_RESOLUTION

Public Class Generator_Window
    Inherits IO_Abstraction_Generator_Form

    Private Shared ReadOnly Tool_Name As String = "IO abstraction generator"

    Private Loaded_IO_Abstraction As IO_Abstraction = Nothing
    Private IO_Abstraction_File_Fullpath As String = ""

    Private WithEvents Menu_Load As New ToolStripMenuItem
    Private WithEvents Menu_New As New ToolStripMenuItem
    Private WithEvents Menu_Save As New ToolStripMenuItem

    Private WithEvents Menu_Generate As New ToolStripMenuItem

    Private WithEvents IO_Abs_Name_TextBox As TextBox
    Private WithEvents Hardware_Selection_ComboBox As New ComboBox

    Private WithEvents Digital_Inputs_Table_View As DataGridView
    Private WithEvents Digital_Inputs_Add_Button As Button

    Private WithEvents Analog_Inputs_Table_View As DataGridView
    Private WithEvents Analog_Inputs_Add_Button As Button

    Private WithEvents Digital_Outputs_Table_View As DataGridView
    Private WithEvents Digital_Outputs_Add_Button As Button

    Private Const Add_Button_Width As Integer = 50
    Private Const Add_Button_Height As Integer = Item_Height
    Private Shared Add_Button_Size As New Size(Add_Button_Width, Add_Button_Height)

    Private Const Scroll_Bar_Width As Integer = 17

    Private Const Table_Width As Integer = Panel_Width - 2 * Marge

    Private Const Field_Label_Witdh As Integer = CInt(Panel_Width / 3 - Marge)
    Private Shared Field_Label_Size As New Size(Field_Label_Witdh, Label_Height)
    Private Const Field_Value_Witdh As Integer = CInt(Panel_Width * 2 / 3 - Marge)
    Private Shared Field_Value_Size As New Size(Field_Value_Witdh, Label_Height)


    Private Shared Grid_View_Style As New DataGridViewCellStyle With {
        .BackColor = Background_Color,
        .ForeColor = Foreground_Color}

    Public Sub New()

        '------------------------------------------------------------------------------------------'
        ' Create menu bar
        Dim main_menu As New MenuStrip
        Me.Controls.Add(main_menu)
        main_menu.BackColor = Background_Color
        main_menu.ForeColor = Foreground_Color
        Dim project_menu As ToolStripMenuItem
        project_menu = CType(main_menu.Items.Add("File"), ToolStripMenuItem)

        With Me.Menu_New
            .Text = "New"
            .BackColor = Background_Color
            .ForeColor = Foreground_Color
        End With
        project_menu.DropDownItems.Add(Me.Menu_New)

        With Me.Menu_Load
            .Text = "Load"
            .BackColor = Background_Color
            .ForeColor = Foreground_Color
        End With
        project_menu.DropDownItems.Add(Me.Menu_Load)

        With Me.Menu_Save
            .Text = "Save"
            .BackColor = Background_Color
            .ForeColor = Foreground_Color
        End With
        project_menu.DropDownItems.Add(Me.Menu_Save)

        With Me.Menu_Generate
            .Text = "Generate"
            .BackColor = Background_Color
            .ForeColor = Foreground_Color
        End With
        main_menu.Items.Add(Me.Menu_Generate)

        Dim item_y_pos As Integer = 3 * Marge
        Dim inner_item_y_pos As Integer


        '------------------------------------------------------------------------------------------'
        ' Add project data panel
        inner_item_y_pos = Marge

        Dim project_pannel As New Panel With {
            .Location = New Point(Marge, item_y_pos),
            .BorderStyle = BorderStyle.FixedSingle}
        Me.Controls.Add(project_pannel)

        Dim project_label As New Label With {
            .Text = "IO Abstraction",
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Label_Size}
        project_pannel.Controls.Add(project_label)

        inner_item_y_pos += project_label.Height + Marge

        Dim project_name_label As New Label With {
            .Text = "Name :",
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Field_Label_Size}
        project_pannel.Controls.Add(project_name_label)
        Me.IO_Abs_Name_TextBox = New TextBox With {
            .Enabled = False,
            .BackColor = Background_Color,
            .ForeColor = Foreground_Color,
            .Text = "",
            .Location = New Point(Marge + project_name_label.Width, inner_item_y_pos),
            .Size = Field_Value_Size}
        project_pannel.Controls.Add(Me.IO_Abs_Name_TextBox)

        inner_item_y_pos += Me.IO_Abs_Name_TextBox.Height + Marge

        Dim hardware_label As New Label With {
            .Text = "Hardware :",
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = Field_Label_Size}
        project_pannel.Controls.Add(hardware_label)
        Dim hardware_list As String() = [Enum].GetNames(GetType(IO_Abstraction.E_HARDWARE))
        With Me.Hardware_Selection_ComboBox
            .Enabled = False
            .BackColor = Background_Color
            .ForeColor = Foreground_Color
            .Items.AddRange(hardware_list)
            .Text = hardware_list(0)
            .Location = New Point(Marge + hardware_label.Width, inner_item_y_pos)
            .Size = Field_Value_Size
        End With
        project_pannel.Controls.Add(Me.Hardware_Selection_ComboBox)

        inner_item_y_pos += Me.Hardware_Selection_ComboBox.Height + Marge


        project_pannel.Size = New Size(Panel_Width, inner_item_y_pos)
        item_y_pos += project_pannel.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Add digital inputs panel
        inner_item_y_pos = Marge

        Dim di_panel As New Panel With {
                .Location = New Point(Marge, item_y_pos),
                .BorderStyle = BorderStyle.FixedSingle}
        Me.Controls.Add(di_panel)

        Dim di_label As New Label With {
                .Text = "Digital inputs",
                .Location = New Point(Marge, inner_item_y_pos),
                .Size = Label_Size}
        di_panel.Controls.Add(di_label)

        Me.Digital_Inputs_Add_Button = New Button With {
            .Enabled = False,
            .Text = "+ Add",
            .BackColor = Foreground_Color,
            .ForeColor = Background_Color,
            .Size = Add_Button_Size,
            .Location = New Point(Panel_Width - Add_Button_Width - Marge, inner_item_y_pos)}
        di_panel.Controls.Add(Me.Digital_Inputs_Add_Button)

        inner_item_y_pos += di_label.Height + Marge

        Me.Digital_Inputs_Table_View = Create_IO_GridView(inner_item_y_pos)

        Dim di_name_colomnn As New DataGridViewTextBoxColumn With {
            .HeaderText = "Name",
            .Name = "Name",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 2
        }
        Me.Digital_Inputs_Table_View.Columns.Add(di_name_colomnn)

        Dim di_pin_colomnn As New DataGridViewTextBoxColumn With {
            .HeaderText = "Pin",
            .Name = "Pin",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 5
        }
        Me.Digital_Inputs_Table_View.Columns.Add(di_pin_colomnn)

        Dim has_pullup_column As New DataGridViewComboBoxColumn With {
            .HeaderText = "Has pull-up",
            .Name = "Has pull-up",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 5
        }
        has_pullup_column.Items.Add("True")
        has_pullup_column.Items.Add("False")
        Me.Digital_Inputs_Table_View.Columns.Add(has_pullup_column)

        Dim di_delete_colomnn As New DataGridViewButtonColumn With {
            .HeaderText = "",
            .Text = "delete",
            .Name = "Delete",
            .UseColumnTextForButtonValue = True,
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 10
        }
        Me.Digital_Inputs_Table_View.Columns.Add(di_delete_colomnn)

        di_panel.Controls.Add(Me.Digital_Inputs_Table_View)
        inner_item_y_pos += Me.Digital_Inputs_Table_View.Height + Marge

        di_panel.Size = New Size(Panel_Width, inner_item_y_pos)
        item_y_pos += di_panel.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Add analog inputs panel
        inner_item_y_pos = Marge

        Dim ai_panel As New Panel With {
                .Location = New Point(Marge, item_y_pos),
                .BorderStyle = BorderStyle.FixedSingle}
        Me.Controls.Add(ai_panel)

        Dim ai_label As New Label With {
                .Text = "Analog inputs",
                .Location = New Point(Marge, inner_item_y_pos),
                .Size = Label_Size}
        ai_panel.Controls.Add(ai_label)

        Me.Analog_Inputs_Add_Button = New Button With {
            .Enabled = False,
            .Text = "+ Add",
            .BackColor = Foreground_Color,
            .ForeColor = Background_Color,
            .Size = Add_Button_Size,
            .Location = New Point(Panel_Width - Add_Button_Width - Marge, inner_item_y_pos)}
        ai_panel.Controls.Add(Me.Analog_Inputs_Add_Button)

        inner_item_y_pos += di_label.Height + Marge

        Me.Analog_Inputs_Table_View = Create_IO_GridView(inner_item_y_pos)

        Dim ai_name_colomnn As New DataGridViewTextBoxColumn With {
            .HeaderText = "Name",
            .Name = "Name",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 2
        }
        Me.Analog_Inputs_Table_View.Columns.Add(ai_name_colomnn)

        Dim ai_pin_colomnn As New DataGridViewTextBoxColumn With {
            .HeaderText = "Pin",
            .Name = "Pin",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 5
        }
        Me.Analog_Inputs_Table_View.Columns.Add(ai_pin_colomnn)

        Dim ai_resol_column As New DataGridViewComboBoxColumn With {
            .HeaderText = "Resolution",
            .Name = "Resolution",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 5
        }
        Dim ai_resol_list = [Enum].GetValues(Of Analog_Input.E_ANALOG_INPUT_RESOLUTION)()
        Dim data_source = (From v In ai_resol_list
                           Select New With {
                             .Texte = Get_Description(v),
                             .Valeur = v
                         }).ToList()
        ai_resol_column.DataSource = data_source
        ai_resol_column.DisplayMember = "Texte"
        ai_resol_column.ValueMember = "Valeur"
        Me.Analog_Inputs_Table_View.Columns.Add(ai_resol_column)

        Dim ai_delete_colomnn As New DataGridViewButtonColumn With {
            .HeaderText = "",
            .Text = "delete",
            .Name = "Delete",
            .UseColumnTextForButtonValue = True,
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 10
        }
        Me.Analog_Inputs_Table_View.Columns.Add(ai_delete_colomnn)

        ai_panel.Controls.Add(Me.Analog_Inputs_Table_View)
        inner_item_y_pos += Me.Analog_Inputs_Table_View.Height + Marge

        ai_panel.Size = New Size(Panel_Width, inner_item_y_pos)
        item_y_pos += di_panel.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Add digital outputs panel
        inner_item_y_pos = Marge

        Dim do_panel As New Panel With {
                .Location = New Point(Marge, item_y_pos),
                .BorderStyle = BorderStyle.FixedSingle}
        Me.Controls.Add(do_panel)

        Dim do_label As New Label With {
                .Text = "Digital outputs",
                .Location = New Point(Marge, inner_item_y_pos),
                .Size = Label_Size}
        do_panel.Controls.Add(do_label)

        Me.Digital_Outputs_Add_Button = New Button With {
            .Enabled = False,
            .Text = "+ Add",
            .BackColor = Foreground_Color,
            .ForeColor = Background_Color,
            .Size = Add_Button_Size,
            .Location = New Point(Panel_Width - Add_Button_Width - Marge, inner_item_y_pos)}
        do_panel.Controls.Add(Me.Digital_Outputs_Add_Button)

        inner_item_y_pos += do_label.Height + Marge

        Me.Digital_Outputs_Table_View = Create_IO_GridView(inner_item_y_pos)

        Dim do_name_colomnn As New DataGridViewTextBoxColumn With {
            .HeaderText = "Name",
            .Name = "Name",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 2
        }
        Me.Digital_Outputs_Table_View.Columns.Add(do_name_colomnn)

        Dim do_pin_colomnn As New DataGridViewTextBoxColumn With {
            .HeaderText = "Pin",
            .Name = "Pin",
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 5
        }
        Me.Digital_Outputs_Table_View.Columns.Add(do_pin_colomnn)

        Dim do_delete_colomnn As New DataGridViewButtonColumn With {
            .HeaderText = "",
            .Text = "delete",
            .Name = "Delete",
            .UseColumnTextForButtonValue = True,
            .Width = (Table_Width - Scroll_Bar_Width - Marge) / 10
        }
        Me.Digital_Outputs_Table_View.Columns.Add(do_delete_colomnn)

        do_panel.Controls.Add(Me.Digital_Outputs_Table_View)
        inner_item_y_pos += Me.Digital_Outputs_Table_View.Height + Marge

        do_panel.Size = New Size(Panel_Width, inner_item_y_pos)
        item_y_pos += do_panel.Height + Marge


        '------------------------------------------------------------------------------------------'
        ' Design Form
        Me.Text = Tool_Name
        Me.ClientSize = New Size(Form_Width, item_y_pos)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.BackColor = Background_Color
        Me.ForeColor = Foreground_Color

    End Sub

    Private Sub Enable_Form()
        Me.IO_Abs_Name_TextBox.Enabled = True
        Me.Digital_Inputs_Add_Button.Enabled = True
        Me.Analog_Inputs_Add_Button.Enabled = True
        Me.Digital_Outputs_Add_Button.Enabled = True
    End Sub

    Private Sub Disable_Form()
        Me.IO_Abs_Name_TextBox.Enabled = False
        Me.Analog_Inputs_Add_Button.Enabled = False
        Me.Digital_Inputs_Add_Button.Enabled = False
    End Sub

    Private Sub Load_Clicked() Handles Menu_Load.Click
        If Not IsNothing(Me.Loaded_IO_Abstraction) Then
            Me.Close_Loaded_IO_Abstraction()
        End If
        ' Open a form asking for a project file
        Dim load_prj_dialog = New OpenFileDialog With {
            .Title = "Select IO abstraction project file",
            .Filter = "IO_Abstraction file|*" & IO_Abstraction.Project_File_Extension,
            .CheckFileExists = True}
        Dim result As DialogResult = load_prj_dialog.ShowDialog()
        If result = DialogResult.OK Then
            Me.Close_Loaded_IO_Abstraction()
            ' Load the project from the file given by user
            Dim project_file_path As String = load_prj_dialog.FileName
            Me.Loaded_IO_Abstraction = IO_Abstraction.Load(project_file_path)
            If Not IsNothing(Me.Loaded_IO_Abstraction) Then
                Me.Enable_Form()
                Me.IO_Abstraction_File_Fullpath = project_file_path
                Me.IO_Abs_Name_TextBox.Text = Me.Loaded_IO_Abstraction.Name
                For Each di In Me.Loaded_IO_Abstraction.Digital_Inputs
                    Me.Digital_Inputs_Table_View.Rows.Add(di.Name, di.Pin_Id, di.Has_Pullup.ToString)
                Next
                For Each ai In Me.Loaded_IO_Abstraction.Analog_Inputs
                    Me.Analog_Inputs_Table_View.Rows.Add(ai.Name, ai.Pin_Id, ai.Resolution)
                Next
                For Each d_o In Me.Loaded_IO_Abstraction.Digital_Outputs
                    Me.Digital_Outputs_Table_View.Rows.Add(d_o.Name, d_o.Pin_Id)
                Next
            End If
        End If
    End Sub

    Private Sub New_Clicked() Handles Menu_New.Click
        If Not IsNothing(Me.Loaded_IO_Abstraction) Then
            Me.Close_Loaded_IO_Abstraction()
        End If

        Dim new_proj_form As New New_Project_Form
        Dim creation_result As DialogResult = new_proj_form.ShowDialog()
        If creation_result = DialogResult.OK Then
            Me.Loaded_IO_Abstraction = New IO_Abstraction(new_proj_form.Get_Project_Name())
            Me.Enable_Form()
            Me.IO_Abstraction_File_Fullpath = new_proj_form.Get_File_Full_Path()
            Me.IO_Abs_Name_TextBox.Text = new_proj_form.Get_Project_Name()
            Me.Loaded_IO_Abstraction.Save(new_proj_form.Get_File_Full_Path())
        End If
    End Sub

    Private Sub Save_Clicked() Handles Menu_Save.Click
        Me.Loaded_IO_Abstraction.Save(Me.IO_Abstraction_File_Fullpath)
    End Sub

    Private Sub Generate_Clicked() Handles Menu_Generate.Click
        If Not IsNothing(Me.Loaded_IO_Abstraction) Then
            Dim file_path As String = Path.GetDirectoryName(Me.IO_Abstraction_File_Fullpath)
            If Me.Loaded_IO_Abstraction.Hardware = IO_Abstraction.E_HARDWARE.ARDUINO Then
                Code_Generator.Generate_Code_Arduino(Me.Loaded_IO_Abstraction, file_path)
                MsgBox("Code generated for Arduino hardware.", MsgBoxStyle.Information, Tool_Name)
            End If
        End If
    End Sub

    Private Sub Close_Loaded_IO_Abstraction()
        Me.IO_Abs_Name_TextBox.Text = ""
        While Me.Digital_Inputs_Table_View.Rows.Count > 0
            Me.Digital_Inputs_Table_View.Rows.RemoveAt(0)
        End While
        While Me.Analog_Inputs_Table_View.Rows.Count > 0
            Me.Analog_Inputs_Table_View.Rows.RemoveAt(0)
        End While
        While Me.Digital_Outputs_Table_View.Rows.Count > 0
            Me.Digital_Outputs_Table_View.Rows.RemoveAt(0)
        End While
        Me.Disable_Form()
    End Sub

    Private Sub Name_Updated() Handles IO_Abs_Name_TextBox.TextChanged
        Me.Loaded_IO_Abstraction.Name = Me.IO_Abs_Name_TextBox.Text
    End Sub

    Private Sub Add_Digital_Input_Button_Clicked() Handles Digital_Inputs_Add_Button.Click
        Dim count As Integer = Me.Digital_Inputs_Table_View.Rows.Count
        Me.Digital_Inputs_Table_View.Rows.Add("pin_name", 0, "False")
        Dim di As New Digital_Input With {
            .Name = "pin_name",
            .Pin_Id = 0,
            .Has_Pullup = False}
        Me.Loaded_IO_Abstraction.Digital_Inputs.Add(di)
    End Sub

    Private Sub Digital_Input_Updated(sender As Object, e As DataGridViewCellEventArgs) Handles _
            Digital_Inputs_Table_View.CellValidated
        If e.RowIndex >= 0 Then
            Dim column_name As String = Digital_Inputs_Table_View.Columns(e.ColumnIndex).Name
            Dim row As DataGridViewRow = Me.Digital_Inputs_Table_View.Rows(e.RowIndex)
            Dim di As Digital_Input = Me.Loaded_IO_Abstraction.Digital_Inputs(e.RowIndex)
            If column_name = "Name" Then
                Dim new_name As String = row.Cells(0).Value
                di.Name = new_name
            ElseIf column_name = "Pin" Then
                Dim new_pin As UInteger = CUInt(row.Cells(1).Value)
                di.Pin_Id = new_pin
            ElseIf column_name = "Has pull-up" Then
                Dim new_has_pullup As Boolean = CBool(row.Cells(2).Value)
                di.Has_Pullup = new_has_pullup
            End If
        End If
    End Sub

    Private Sub DI_Table_Delete_Clicked(sender As Object, e As DataGridViewCellEventArgs) Handles _
            Digital_Inputs_Table_View.CellContentClick
        If e.RowIndex >= 0 Then
            If Digital_Inputs_Table_View.Columns(e.ColumnIndex).Name = "Delete" Then
                Me.Digital_Inputs_Table_View.Rows.RemoveAt(e.RowIndex)
                Me.Loaded_IO_Abstraction.Digital_Inputs.RemoveAt(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub Add_Analog_Input_Button_Clicked() Handles Analog_Inputs_Add_Button.Click
        Dim count As Integer = Me.Analog_Inputs_Table_View.Rows.Count
        Me.Analog_Inputs_Table_View.Rows.Add("pin_name", "A0", ANALOG_INPUT_10_BITS)
        Dim ai As New Analog_Input With {
            .Name = "pin_name",
            .Pin_Id = "A0",
            .Resolution = ANALOG_INPUT_10_BITS}
        Me.Loaded_IO_Abstraction.Analog_Inputs.Add(ai)
    End Sub

    Private Sub Analog_Input_Updated(sender As Object, e As DataGridViewCellEventArgs) Handles _
            Analog_Inputs_Table_View.CellValidated
        If e.RowIndex >= 0 Then
            Dim column_name As String = Analog_Inputs_Table_View.Columns(e.ColumnIndex).Name
            Dim row As DataGridViewRow = Me.Analog_Inputs_Table_View.Rows(e.RowIndex)
            Dim ai As Analog_Input = Me.Loaded_IO_Abstraction.Analog_Inputs(e.RowIndex)
            If column_name = "Name" Then
                Dim new_name As String = row.Cells(0).Value
                ai.Name = new_name
            ElseIf column_name = "Pin" Then
                Dim new_pin As String = row.Cells(1).Value
                ai.Pin_Id = new_pin
            ElseIf column_name = "Resolution" Then
                Dim new_resol As Analog_Input.E_ANALOG_INPUT_RESOLUTION = row.Cells(2).Value
                ai.Resolution = new_resol
            End If
        End If
    End Sub

    Private Sub AI_Table_Delete_Clicked(sender As Object, e As DataGridViewCellEventArgs) Handles _
            Analog_Inputs_Table_View.CellContentClick
        If e.RowIndex >= 0 Then
            If Analog_Inputs_Table_View.Columns(e.ColumnIndex).Name = "Delete" Then
                Me.Analog_Inputs_Table_View.Rows.RemoveAt(e.RowIndex)
                Me.Loaded_IO_Abstraction.Analog_Inputs.RemoveAt(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub Add_Digital_Output_Button_Clicked() Handles Digital_Outputs_Add_Button.Click
        Dim count As Integer = Me.Digital_Outputs_Table_View.Rows.Count
        Me.Digital_Outputs_Table_View.Rows.Add("pin_name", 0)
        Dim d_o As New Digital_Output With {
            .Name = "pin_name",
            .Pin_Id = 0}
        Me.Loaded_IO_Abstraction.Digital_Outputs.Add(d_o)
    End Sub

    Private Sub Digital_Output_Updated(sender As Object, e As DataGridViewCellEventArgs) Handles _
            Digital_Outputs_Table_View.CellValidated
        If e.RowIndex >= 0 Then
            Dim column_name As String = Digital_Outputs_Table_View.Columns(e.ColumnIndex).Name
            Dim row As DataGridViewRow = Me.Digital_Outputs_Table_View.Rows(e.RowIndex)
            Dim d_o As Digital_Output = Me.Loaded_IO_Abstraction.Digital_Outputs(e.RowIndex)
            If column_name = "Name" Then
                Dim new_name As String = row.Cells(0).Value
                d_o.Name = new_name
            ElseIf column_name = "Pin" Then
                Dim new_pin As UInteger = CUInt(row.Cells(1).Value)
                d_o.Pin_Id = new_pin
            End If
        End If
    End Sub

    Private Sub DO_Table_Delete_Clicked(sender As Object, e As DataGridViewCellEventArgs) Handles _
            Digital_Outputs_Table_View.CellContentClick
        If e.RowIndex >= 0 Then
            If Digital_Outputs_Table_View.Columns(e.ColumnIndex).Name = "Delete" Then
                Me.Digital_Outputs_Table_View.Rows.RemoveAt(e.RowIndex)
                Me.Loaded_IO_Abstraction.Digital_Outputs.RemoveAt(e.RowIndex)
            End If
        End If
    End Sub

    Private Shared Function Create_IO_GridView(inner_item_y_pos As Integer) As DataGridView
        Dim grid_view As New DataGridView With {
            .BackColor = Background_Color,
            .ForeColor = Foreground_Color,
            .BackgroundColor = Background_Color,
            .DefaultCellStyle = Grid_View_Style,
            .ColumnHeadersDefaultCellStyle = Grid_View_Style,
            .AllowUserToResizeRows = False,
            .AllowUserToResizeColumns = False,
            .Location = New Point(Marge, inner_item_y_pos),
            .Size = New Size(Table_Width, 100),
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = True,
            .ScrollBars = ScrollBars.Vertical,
            .ColumnHeadersVisible = True,
            .RowHeadersVisible = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None}
        Return grid_view
    End Function

End Class
