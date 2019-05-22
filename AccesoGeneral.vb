

Imports DPFP
Imports DPFP.Capture
Imports System.Text
Imports System.IO
Imports System.Data.SqlClient

Public Class AccesoGeneral
    Implements DPFP.Capture.EventHandler

    Private Captura As DPFP.Capture.Capture
    Private template As DPFP.Template
    Private verificador As DPFP.Verification.Verification
    Public _banderaClose As Boolean

    Private Sub AccesoGeneral_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _banderaClose = False Then
            e.Cancel = True
        Else
            PararCaptura()
        End If
    End Sub
    Private Sub AccesoGeneral_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        Init()
        IniciaCaptura()
        imgHuella.Height = 0
        imgHuella.Width = 0
        _banderaClose = False
    End Sub

#Region "Inicia/Detener Lector"
    Protected Overridable Sub Init()
        Try
            Captura = New Capture()
            If Not IsNothing(Captura) Then
                Captura.EventHandler = Me
                verificador = New DPFP.Verification.Verification()
                template = New Template()
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
    Public Sub InicioRemoto()
        Init()
        IniciaCaptura()
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
        'Me.lblProceso.Text = "Huella leida correctamente..."
    End Sub

    Public Sub OnComplete(Capture As Object, ReaderSerialNumber As String, Sample As Sample) Implements EventHandler.OnComplete
        Dim db As New db_Class
        Dim query As String = Nothing
        Dim IDRegistro As String = Nothing
        Dim Nombre As String = Nothing


        Try
            ObtieneImagen(ConvertirHuella_a_Imagen(Sample))
            Dim caract As DPFP.FeatureSet = ExtraerCaracteristicas(Sample, DPFP.Processing.DataPurpose.Verification)
            If Not IsNothing(caract) Then
                Dim result As New DPFP.Verification.Verification.Result

                Dim conexion As New SqlConnection(My.Settings.sqlConn)
                conexion.Open()
                Dim cmd As New SqlCommand()
                cmd = conexion.CreateCommand()
                cmd.CommandText = "Select * From Empleado where huelladigital is not null and IDCompania = " & IDCompania
                Dim read As SqlDataReader
                read = cmd.ExecuteReader()
                Dim verificado As Boolean = False

                While (read.Read())

                    Dim memoria As New MemoryStream(CType(read("HuellaDigital"), Byte()))
                    template.DeSerialize(memoria.ToArray())
                    verificador.Verify(caract, template, result)

                    If result.Verified Then
                        IDRegistro = read("IDRegistro")
                        verificado = True
                        Exit While
                    End If

                End While

                If verificado = True Then
                    RegistraAcceso(read("IDEmpleado"))
                    muestramesanje("Registro encontrado: " & read("Nombre") & " " & read("Paterno") & " " & read("Materno"))
                Else
                    muestramesanje("No encontrado")
                End If

                read.Dispose()
                cmd.Dispose()
                conexion.Close()
                conexion.Dispose()

            End If
        Catch ex As Exception
            muestramesanje(ex.Message)
        End Try

    End Sub

    Public Sub OnFingerGone(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnFingerGone
        ' Me.lblProceso.Text = "..."
    End Sub

    Public Sub OnFingerTouch(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnFingerTouch
        'Me.lblProceso.Text = "Leyendo huella...."
    End Sub

    Public Sub OnReaderConnect(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnReaderConnect
        'Me.lblStatus.Text = "Conectado..."
    End Sub

    Public Sub OnReaderDisconnect(Capture As Object, ReaderSerialNumber As String) Implements EventHandler.OnReaderDisconnect
        'Me.lblStatus.Text = "Sin conexión..."
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

    Private Sub btnAlta_Click(sender As Object, e As EventArgs) Handles btnAlta.Click
        LoginForm1.Show()


    End Sub

    Private Sub btnCerrar_Click(sender As Object, e As EventArgs)


    End Sub
    Public Sub CierraSistema()
        PararCaptura()
        _banderaClose = True

        Me.Close()
    End Sub
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Me.lblHora.Text = Date.Now.ToLongTimeString
    End Sub
    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            If Not BackgroundWorker1.IsBusy Then
                BackgroundWorker1.RunWorkerAsync()
            End If
        Catch ex As Exception
        End Try
    End Sub
    Public Sub RegistraAcceso(ByVal IDEmpleado As String)
        Dim query As String = Nothing
        Dim db As New db_Class

        Try
            query = "Insert Into EmpleadoAcceso (IDCompania,IDEmpleado,Fecha)VALUES("
            query += IDCompania & ",'" & IDEmpleado & "',GetDate())"
            db.afectar(query)
        Catch ex As Exception
            muestramesanje(ex.Message)
        Finally
            db = Nothing
            GC.Collect()
        End Try

    End Sub
    Public Sub muestramesanje(ByVal msg As String)
        If ListBox1.Items.Count > 1000 Then ListBox1.Items.Clear()
        Dim dato As String = Date.Now & ":: " & msg
        ListBox1.Items.Add(dato)
    End Sub

    Private Sub btncerrar_Click_1(sender As Object, e As EventArgs)
        Me.Hide()
        Close()
    End Sub

    Private Sub lblHora_Click(sender As Object, e As EventArgs) Handles lblHora.Click

    End Sub
End Class