Imports System.Runtime.CompilerServices

Public Class Code_Generator
    Public Shared Sub Generate_Code_Arduino(io_ab As IO_Abstraction, file_path As String)
        Dim component_name As String = io_ab.Name & "_IO_Harware_Abstraction"

        Dim header_file_name As String = component_name & ".h"
        Dim header_file_full_path As String
        header_file_full_path = file_path & Path.DirectorySeparatorChar & header_file_name
        Dim header_writer As New StreamWriter(header_file_full_path, False)

        Dim source_file_name As String = component_name & ".c"
        Dim source_file_full_path As String
        source_file_full_path = file_path & Path.DirectorySeparatorChar & source_file_name
        Dim source_writer As New StreamWriter(source_file_full_path, False)

        ' Add inclusion in header file
        If io_ab.Digital_Inputs.Count > 0 Then
            header_writer.WriteLine("#include ""Digital_Input.h""")
        End If
        If io_ab.Analog_Inputs.Count > 0 Then
            header_writer.WriteLine("#include ""Analog_Input_5.h""")
        End If
        If io_ab.Digital_Outputs.Count > 0 Then
            header_writer.WriteLine("#include ""Digital_Output.h""")
        End If
        header_writer.WriteLine("")

        ' Add inclusion in source file
        source_writer.WriteLine("#include """ & header_file_name & """")
        source_writer.WriteLine("#include ""Arduino.h""")
        source_writer.WriteLine("")

        ' Add Configuration function if needed
        If io_ab.Digital_Inputs.Count > 0 Or io_ab.Digital_Outputs.Count > 0 Then
            header_writer.WriteLine("void Configure_" & component_name & "(void);")
            header_writer.WriteLine("")

            source_writer.WriteLine("void Configure_" & component_name & "(void)")
            source_writer.WriteLine("{")
            For Each di In io_ab.Digital_Inputs
                Dim mode As String = "INPUT"
                If di.Has_Pullup = True Then
                    mode = "INPUT_PULLUP"
                End If
                source_writer.WriteLine("    pinMode( " & di.Pin_Id & ", " & mode & " );")
            Next
            For Each digital_output In io_ab.Digital_Outputs

                source_writer.WriteLine("    pinMode( " & digital_output.Pin_Id & ", OUTPUT );")
            Next
            source_writer.WriteLine("}")
            source_writer.WriteLine("")
        End If

        ' Add interfaces for digital inputs
        If io_ab.Digital_Inputs.Count > 0 Then
            source_writer.WriteLine("/* Digital inputs */")
            source_writer.WriteLine("static void Get_Level( int pin, E_IO_Level* level )")
            source_writer.WriteLine("{")
            source_writer.WriteLine(
                "    (HIGH==digitalRead(pin)) ? (*level=IO_LEVEL_HIGH) : (*level=IO_LEVEL_LOW);")
            source_writer.WriteLine("}")
            source_writer.WriteLine("")
        End If
        For Each di In io_ab.Digital_Inputs
            header_writer.WriteLine("extern const Digital_Input " & di.Name & "__Digital_Input;")

            source_writer.WriteLine(
                "static void " & di.Name & "__Digital_Input_Get_Level( E_IO_Level* level )")
            source_writer.WriteLine("{")
            source_writer.WriteLine("    Get_Level(" & di.Pin_Id & ",level);")
            source_writer.WriteLine("}")
            source_writer.WriteLine("const Digital_Input " & di.Name & "__Digital_Input = ")
            source_writer.WriteLine("{")
            source_writer.WriteLine("    .Get_Level = " & di.Name & "__Digital_Input_Get_Level")
            source_writer.WriteLine("};")
            source_writer.WriteLine("")
        Next

        ' Add interfaces for analog inputs
        If io_ab.Analog_Inputs.Count > 0 Then
            source_writer.WriteLine("/* Analog inputs */")
            source_writer.WriteLine("static void Get_Voltage( int pin, T_Pin_Voltage_5* voltage )")
            source_writer.WriteLine("{")
            source_writer.WriteLine(
                "    *voltage = analogRead( pin );")
            source_writer.WriteLine("}")
            source_writer.WriteLine("")
        End If
        For Each ai In io_ab.Analog_Inputs
            header_writer.WriteLine("extern const Analog_Input_5 " & ai.Name & "__Analog_Input;")

            source_writer.WriteLine(
                "static void " & ai.Name & "__Analog_Input_Get_Voltage( T_Pin_Voltage_5* voltage )")
            source_writer.WriteLine("{")
            source_writer.WriteLine("    Get_Voltage(" & ai.Pin_Id & ",voltage);")
            source_writer.WriteLine("}")
            source_writer.WriteLine("const Analog_Input_5 " & ai.Name & "__Analog_Input = ")
            source_writer.WriteLine("{")
            source_writer.WriteLine("    .Get_Voltage = " & ai.Name & "__Analog_Input_Get_Voltage")
            source_writer.WriteLine("};")
            source_writer.WriteLine("")
        Next

        ' Add interfaces for digital outputs
        If io_ab.Digital_Outputs.Count > 0 Then
            source_writer.WriteLine("/* Digital outputs */")
            source_writer.WriteLine("static void Set_Level( int pin, E_IO_Level level )")
            source_writer.WriteLine("{")
            source_writer.WriteLine(
                "    level==IO_LEVEL_LOW ? digitalWrite(pin, LOW) : digitalWrite(pin, HIGH);")
            source_writer.WriteLine("}")
            source_writer.WriteLine("")
        End If
        For Each d_o In io_ab.Digital_Outputs
            header_writer.WriteLine("extern const Digital_Output " & d_o.Name & "__Digital_Output;")

            source_writer.WriteLine(
                "static void " & d_o.Name & "__Digital_Output_Set_Level( E_IO_Level level )")
            source_writer.WriteLine("{")
            source_writer.WriteLine("    Set_Level(" & d_o.Pin_Id & ",level);")
            source_writer.WriteLine("}")
            source_writer.WriteLine("const Digital_Output " & d_o.Name & "__Digital_Output = ")
            source_writer.WriteLine("{")
            source_writer.WriteLine("    .Set_Level = " & d_o.Name & "__Digital_Output_Set_Level")
            source_writer.WriteLine("};")
            source_writer.WriteLine("")
        Next

        header_writer.Close()
        source_writer.Close()
    End Sub
End Class
