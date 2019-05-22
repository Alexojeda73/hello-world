Imports System.Data
Imports System.Data.SqlClient
Imports System.Windows.Forms





Public Class Conexion

    Public Conexion As SqlConnection = New SqlConnection("Data Source=DESKTOP-8CSTBL8\EQUIPOJUNA;Initial Catalog=ControlAlumnos;Integrated Security=True")
    Private cmb As SqlCommandBuilder
    Public ds As DataSet = New DataSet()
    Public da As SqlDataAdapter
    Public comando As SqlCommand


    Public Sub conectar()
        Try
            Conexion.Open()
            MessageBox.Show("Bienvenido a TUTTOR,ya estas conectado")

        Catch ex As Exception
            MessageBox.Show("Error al conectar")

        Finally
            Conexion.Close()
        End Try
    End Sub

    Public Sub consulta(ByVal sql As String, ByVal tabla As String)
        ds.Tables.Clear()
        da = New SqlDataAdapter(sql, Conexion)
        cmb = New SqlCommandBuilder(da)
        da.Fill(ds, tabla)

    End Sub

    Function insertar(ByVal sql)
        Conexion.Open()
        comando = New SqlCommand(sql, Conexion)
        Dim i As Integer = comando.ExecuteNonQuery()
        Conexion.Close()
        If (i > 0) Then
            Return True
        Else
            Return False
        End If

    End Function

    Function Eliminar(ByVal tabla, ByVal condicion)
        Conexion.Open()
        Dim elimina As String = "delete from " + tabla + " where " + condicion
        comando = New SqlCommand(elimina, Conexion)
        Dim i As Integer = comando.ExecuteNonQuery()
        Conexion.Close()
        If (i > 0) Then
            Return True
        Else
            Return False
        End If


    End Function


    Function Actualizar(ByVal tabla, ByVal campos, ByVal condicion)
        Conexion.Open()
        Dim actualiza As String = "update " + tabla + " set " + campos + " where " + condicion
        comando = New SqlCommand(actualiza, Conexion)
        Dim i As Integer = comando.ExecuteNonQuery()
        Conexion.Close()
        If (i > 0) Then
            Return True
        Else
            Return False
        End If


    End Function


End Class
