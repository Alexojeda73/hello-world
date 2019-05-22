
Imports DPFP
Imports DPFP.Capture
Imports System.Text
Imports System.IO
Imports System.Data.SqlClient

Public Class Form1

    Implements DPFP.Capture.EventHandler

    Private Captura As DPFP.Capture.Capture
    Private _enroller As DPFP.Processing.Enrollment
    Private template As DPFP.Template
    Public model As DPFP.Data
    Private Delegate Sub _delgadoMuestra(ByVal texto As String)

    Dim conexion As Conexion = New Conexion()

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        PararCaptura()
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        PararCaptura()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: esta línea de código carga datos en la tabla 'ControlAlumnosDataSet.Alumnos' Puede moverla o quitarla según sea necesario.
        Me.AlumnosTableAdapter.Fill(Me.ControlAlumnosDataSet.Alumnos)
        conexion.conectar()
        MostrarDatos()

        CheckForIllegalCrossThreadCalls = False
        Init()
        IniciaCaptura()
    End Sub
    Public Sub MostrarDatos()
        conexion.consulta("select*from Alumnos", "Alumnos")
        dgvListado.DataSource = conexion.ds.Tables("Alumnos")

    End Sub

#Region "Inicia/Detener Lector"
    Protected Overridable Sub Init()
        Try
            Captura = New Capture()
            If Not IsNothing(Captura) Then
                Captura.EventHandler = Me
                _enroller = New DPFP.Processing.Enrollment()
                Dim text As New StringBuilder()
                text.AppendFormat("Escanear el dedo {0} veces", _enroller.FeaturesNeeded)
                Me.lblEscaneoDedo.Text = text.ToString()
            Else
                muestramesanje("No se puede inicializar el lector....")
            End If
        Catch ex As Exception
            muestramesanje("No se puede inicializar el lector: " & ex.Message)
        End Try
    End Sub
    Protected Sub IniciaCaptura()
        Try
            If Not IsNothing(Captura) Then
                Captura.StartCapture()
            Else
                muestramesanje("No se puede inicializar el lector....")
            End If
        Catch ex As Exception
            muestramesanje("No se puede inicializar el lector: " & ex.Message)
        End Try
    End Sub
    Protected Sub PararCaptura()
        Try
            If Not IsNothing(Captura) Then
                Captura.StopCapture()
            End If
        Catch ex As Exception
            muestramesanje("No se puede detener la captura: " & ex.Message)
        End Try
    End Sub
#End Region

    Protected Function ConvertirHuella_a_Imagen(ByVal sample As DPFP.Sample) As Bitmap
        Dim convertidor As New DPFP.Capture.SampleConversion()
        Dim mapaBits As Bitmap = Nothing

        Try
            convertidor.ConvertToPicture(sample, mapaBits)
        Catch ex As Exception
            'error al convertir huella a imagen
            muestramesanje("Error al convertir huella a imagen: " & ex.Message)
        Finally
            convertidor = Nothing
            GC.Collect()
        End Try

        Return mapaBits

    End Function
    Private Sub ObtieneImagen(ByVal imagen As Bitmap)
        Me.imgHuella.Image = imagen
        muestramesanje("Huella leida correctamente...")
    End Sub

    Public Sub OnComplete(Capture As Object, ReaderSerialNumber As String, Sample As Sample) Implements EventHandler.OnComplete
        ObtieneImagen(ConvertirHuella_a_Imagen(Sample))
        Procesar(Sample)
    End Sub

    Public Sub OnFingerGone(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnFingerGone

    End Sub

    Public Sub OnFingerTouch(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnFingerTouch
        muestramesanje("Leyendo huella....")
    End Sub

    Public Sub OnReaderConnect(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnReaderConnect
        muestramesanje("Conectado...")
    End Sub

    Public Sub OnReaderDisconnect(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnReaderDisconnect
        muestramesanje("Sin conexión...")
    End Sub

    Public Sub OnSampleQuality(Capture As Object, ReaderSerialNumber As String, CaptureFeedback As CaptureFeedback) Implements EventHandler.OnSampleQuality

    End Sub

    Protected Function ExtraerCaracteristicas(ByVal Sample As DPFP.Sample, ByVal Purpose As DPFP.Processing.DataPurpose) As DPFP.FeatureSet
        Dim extractor As New DPFP.Processing.FeatureExtraction()
        Dim alimentacion As DPFP.Capture.CaptureFeedback = DPFP.Capture.CaptureFeedback.None
        Dim caracteristicas As New DPFP.FeatureSet()

        Try
            extractor.CreateFeatureSet(Sample, Purpose, alimentacion, caracteristicas)
            If alimentacion = DPFP.Capture.CaptureFeedback.Good Then
                Return caracteristicas
            Else
                Return Nothing
            End If
        Catch ex As Exception
            muestramesanje("Error al extraer caracteristicas de la huella: " & ex.Message)
            Return Nothing
        End Try

    End Function
    Protected Sub Procesar(ByVal Sample As DPFP.Sample)
        Dim caracteristicas As DPFP.FeatureSet = ExtraerCaracteristicas(Sample, DPFP.Processing.DataPurpose.Enrollment)
        Try
            If Not IsNothing(caracteristicas) Then
                _enroller.AddFeatures(caracteristicas)
            Else
                muestramesanje("No se puede extraer las característias de la huella.")
            End If
        Catch ex As Exception
            muestramesanje("Error al procesar la huella: " & ex.Message)
        Finally
            Dim text As New StringBuilder()
            text.AppendFormat("Escanear el dedo {0} veces", _enroller.FeaturesNeeded)
            MostrarConteoEscaneo(text.ToString())
            Select Case _enroller.TemplateStatus
                Case DPFP.Processing.Enrollment.Status.Ready
                    'agregar imagen para mostrar que ya fue suficiente el escaneo
                    template = _enroller.Template
                    Me.btnGuardarHuella.Enabled = True
                    PararCaptura()
                Case DPFP.Processing.Enrollment.Status.Failed
                    _enroller.Clear()
                    PararCaptura()
                    IniciaCaptura()
            End Select
        End Try
    End Sub
    Private Sub MostrarConteoEscaneo(ByVal texto As String)
        If Me.lblEscaneoDedo.InvokeRequired Then
            Dim _delegado As New _delgadoMuestra(AddressOf MostrarConteoEscaneo)
            Me.Invoke(_delegado, New Object() {texto})
        Else
            Me.lblEscaneoDedo.Text = texto
        End If
    End Sub

    Private Sub btnGuardarHuella_Click(sender As Object, e As EventArgs) Handles btnGuardarHuella.Click

        Dim sqlcom As New SqlConnection("Data Source=DESKTOP-8CSTBL8\EQUIPOJUNA;Initial Catalog=ControlAlumnos;Integrated Security=True")
        sqlcom.Open()

        Dim agregar As String = "insert into Alumnos (rut,nombre,apellido,fechanacimiento)values ( " + txtRut.Text + ",' " + txtNombre.Text + " ' ,' " + txtApellidos.Text + " ',' " + dtFechaNacimiento.Value + " ') "
        Dim agrega As String = "insert into Huella(Huella)Values (" + imgHuella.ToString + " ')"
        If (conexion.insertar(agregar)) Then
            MessageBox.Show("Datos Agregados Correctamente")
            MostrarDatos()
        Else
            MessageBox.Show("Error al agregar datos")
        End If

        Dim db As New db_Class
        Dim query As String = Nothing

        Try
            If ValidaUsuario() Then

                query = "update Empleado Set"
                query += " HuellaDigital = @huella"
                query += " Where IDEmpleado = '" & Me.txtRut.Text & "' and IDCompania = " & IDCompania

                Dim oDR As System.Data.SqlClient.SqlDataReader
                Dim oCom As System.Data.SqlClient.SqlCommand
                Dim oConn As System.Data.SqlClient.SqlConnection
                oConn = New System.Data.SqlClient.SqlConnection(dblocal)
                oConn.Open()
                oCom = New System.Data.SqlClient.SqlCommand()
                oCom.Connection = oConn
                oCom.CommandText = query

                Using fm As New MemoryStream(template.Bytes)
                    oCom.Parameters.AddWithValue("@huella", fm.ToArray())
                End Using

                oCom.CommandTimeout = 0
                oDR = oCom.ExecuteReader()
                oConn.Close()
                oConn.Dispose()

                muestramesanje("Huella guardada correctamente.")
                Me.btnGuardarHuella.Enabled = False
                Me.txtRut.Text = Nothing
            Else
                muestramesanje("El Usuario no existe.")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            db = Nothing
        End Try

    End Sub
    Public Function ValidaUsuario() As Boolean
        Dim bandera As Boolean = False
        Dim query As String = Nothing
        Dim db As New db_Class
        Dim cont As Integer = 0

        Try
            query = "Select Count(*) From Alumnos Where Rut = '" & Me.txtRut.Text & "'"
            db.Seleccionar(query, cont)
            If cont > 0 Then
                bandera = True
            End If
        Catch ex As Exception
            bandera = False
        Finally
            db = Nothing
            GC.Collect()
        End Try

        Return bandera
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        PararCaptura()
        AccesoGeneral.InicioRemoto()
        Me.Close()
    End Sub

    Public Sub muestramesanje(ByVal msg As String)
        If AccesoGeneral.ListBox1.Items.Count > 1000 Then AccesoGeneral.ListBox1.Items.Clear()
        Dim dato As String = Date.Now & ":: " & msg
        AccesoGeneral.ListBox1.Items.Add(dato)
    End Sub

    Private Sub AlumnosBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles AlumnosBindingSource.CurrentChanged

    End Sub

    Private Sub dtFechaNacimiento_ValueChanged(sender As Object, e As EventArgs) Handles dtFechaNacimiento.ValueChanged

    End Sub

    Private Sub txtRut_TextChanged(sender As Object, e As EventArgs) Handles txtRut.TextChanged

    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If (conexion.Eliminar(" Alumnos ", " rut = " + txtRut.Text)) Then
            MessageBox.Show("Datos Eliminados Correctamente")
            MostrarDatos()
        Else
            MessageBox.Show("Error al eliminar")
        End If
    End Sub

    Private Sub dgvListado_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvListado.CellContentClick
        Dim dgv As DataGridViewRow = dgvListado.Rows(e.RowIndex)
        txtRut.Text = dgv.Cells(0).Value.ToString()
        txtNombre.Text = dgv.Cells(1).Value.ToString()
        txtApellidos.Text = dgv.Cells(2).Value.ToString()

    End Sub

    Private Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        Dim Actualizar As String = "rut ='" + txtRut.Text + "', nombre= '" + txtNombre.Text + "', apellido = '" + txtApellidos.Text + "'"
        If (conexion.Actualizar(" Alumnos ", Actualizar, " rut = " + txtRut.Text)) Then
            MessageBox.Show("Datos actualizados correctamente")
            MostrarDatos()
        Else
            MessageBox.Show("Error al Actualizar")

        End If
    End Sub

    Private Sub imgHuella_Click(sender As Object, e As EventArgs) Handles imgHuella.Click

    End Sub

    Private Sub txtBuscarAlumnos_KeyPress(sender As Object, e As KeyPressEventArgs)

    End Sub

    Private Sub txtBuscarAlumnos_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub lblStatus_Click(sender As Object, e As EventArgs) Handles lblStatus.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub lblProceso_Click(sender As Object, e As EventArgs) Handles lblProceso.Click

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnregistro_Click(sender As Object, e As EventArgs) Handles btnregistro.Click
        Me.Hide()
        Form2.Show()

    End Sub
End Class
