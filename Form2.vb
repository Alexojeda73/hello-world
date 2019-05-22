Imports System.Data.SqlClient


Public Class Form2
    Dim obj As New Busqueda
    Dim connection As New SqlConnection("Data Source=DESKTOP-8CSTBL8\EQUIPOJUNA;Initial Catalog=ControlAlumnos;Integrated Security=True")

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: esta línea de código carga datos en la tabla 'ControlAlumnosDataSet3.RegistroAtrasos' Puede moverla o quitarla según sea necesario.

        ' buscarData("")
        obj.llenargrilla(DataGridView1)
    End Sub

    'Public Sub buscarData(valuetosearch As String)

    'Dim buscarcodigo As String = "select * from RegistroAtrasos where fechaRegistro (fechaRegistro,horaIngreso,idAlumno,idPersonal,motivo)like '%" & TextBox1.Text & " %'"

    ' Dim command As New SqlCommand(buscarcodigo, connection)
    'Dim adapter As New SqlDataAdapter(command)
    'Dim table As New DataTable()

    '   adapter.Fill(table)
    '  DataGridView1.DataSource = table


    'End Sub
    Private Sub btncerrarRegistro_Click(sender As Object, e As EventArgs) Handles btncerrarRegistro.Click
        Me.Close()
    End Sub

    Private Sub btnbuscar_Click(sender As Object, e As EventArgs) Handles btnbuscar.Click
        ' buscarData(TextBox1.Text)
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        obj.consultadinamica(TextBox1.Text, DataGridView1)

    End Sub
End Class