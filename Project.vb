Imports System.ComponentModel
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization

Public Class IO_Abstraction
    Public Name As String
    Public Hardware As E_HARDWARE
    Public Digital_Inputs As New List(Of Digital_Input)
    Public Analog_Inputs As New List(Of Analog_Input)
    Public Digital_Outputs As New List(Of Digital_Output)

    Public Enum E_HARDWARE
        ARDUINO
    End Enum

    Public Const Project_File_Extension As String = ".ioab"

    Private Shared ReadOnly Project_Serializer As New XmlSerializer(GetType(IO_Abstraction))


    Public Sub New()
    End Sub

    Public Sub New(name As String)
        Me.Name = name
    End Sub

    Public Shared Function Load(project_file_path As String) As IO_Abstraction
        Dim new_proj As IO_Abstraction = Nothing
        Dim reader As New XmlTextReader(project_file_path)
        Try
            new_proj = CType(IO_Abstraction.Project_Serializer.Deserialize(reader), IO_Abstraction)
        Catch
            MsgBox("The project file is invalid !", MsgBoxStyle.Critical)
        End Try
        reader.Close()
        Return new_proj
    End Function

    Public Sub Save(project_file_path As String)
        Dim writer As New XmlTextWriter(project_file_path, Encoding.UTF8) With {
            .Indentation = 2,
            .IndentChar = " "c,
            .Formatting = Formatting.Indented}
        IO_Abstraction.Project_Serializer.Serialize(writer, Me)
        writer.Close()
    End Sub

End Class

Public Class Digital_Input
    Public Name As String
    Public Pin_Id As UInteger
    Public Has_Pullup As Boolean

    Public Sub New()
    End Sub

    Public Sub New(name As String, pin As UInteger, has_pullup As Boolean)
        Me.Name = name
        Me.Pin_Id = pin
        Me.Has_Pullup = has_pullup
    End Sub
End Class

Public Class Analog_Input
    Public Name As String
    Public Pin_Id As String
    Public Resolution As E_ANALOG_INPUT_RESOLUTION

    Public Enum E_ANALOG_INPUT_RESOLUTION
        <Description("10 bits")> ANALOG_INPUT_10_BITS
        <Description("12 bits")> ANALOG_INPUT_12_BITS
        <Description("14 bits")> ANALOG_INPUT_14_BITS
        <Description("16 bits")> ANALOG_INPUT_16_BITS
    End Enum

    Public Sub New()
    End Sub

    Public Sub New(name As String, pin As String, resol As E_ANALOG_INPUT_RESOLUTION)
        Me.Name = name
        Me.Pin_Id = pin
        Me.Resolution = resol
    End Sub

End Class

Public Class Digital_Output
    Public Name As String
    Public Pin_Id As UInteger

    Public Sub New()
    End Sub

    Public Sub New(name As String, pin As UInteger)
        Me.Name = name
        Me.Pin_Id = pin
    End Sub
End Class


Module Utilities
    Public Function Get_Description(enumeral As [Enum]) As String
        Dim type As Type = enumeral.GetType()
        Dim field_info As FieldInfo = type.GetField(enumeral.ToString())

        Dim attributs As DescriptionAttribute() =
                DirectCast(
                    field_info.GetCustomAttributes(GetType(DescriptionAttribute), False),
                    DescriptionAttribute())

        If attributs.Length > 0 Then
            Return attributs(0).Description
        Else
            Return enumeral.ToString()
        End If
    End Function
End Module
