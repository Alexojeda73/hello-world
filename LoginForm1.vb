Imports System.Data.SqlClient



Public Class LoginForm1

    Dim usuario, clave As String


    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnaceptar.Click, MyBase.Load

        Dim connection As New SqlConnection("Data Source=DESKTOP-8CSTBL8\EQUIPOJUNA;Initial Catalog=_login;Integrated Security=True")

        Dim Command As New SqlCommand("select*from usuario where nickname = @usuario and Contraseña = @contraseña", connection)

        Command.Parameters.Add("@usuario", SqlDbType.VarChar).Value = txtusuario.Text
        Command.Parameters.Add("@contraseña", SqlDbType.VarChar).Value = txtcontraseña.Text

        Dim Adapter As New SqlDataAdapter(Command)

        Dim table As New DataTable()

        Adapter.Fill(table)

        If table.Rows.Count() <= 0 Then

            MessageBox.Show("El nombre de usuario o la contraseña no son válidos")

        Else
            ' MessageBox.Show("Bienvenido a TUTTOR ")



            Me.Hide()

            Form1.Show()

        End If


    End Sub

    Private Sub btncancelar_Click(sender As Object, e As EventArgs) Handles btncancelar.Click
        Me.Close()
    End Sub

    Private Sub txtusuario_TextChanged(sender As Object, e As EventArgs) Handles txtusuario.TextChanged


    End Sub
End Class






