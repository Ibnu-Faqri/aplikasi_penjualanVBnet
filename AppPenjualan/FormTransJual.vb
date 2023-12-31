﻿Imports System.Data.Odbc
Public Class FormTransJual
    Dim TglMySQL As String
    Sub tampil_data()
        Da = New OdbcDataAdapter("SELECT * FROM TBL_BARANG", Conn)
        Ds = New DataSet
        Da.Fill(Ds)
        DataGridView2.DataSource = Ds.Tables(0)
        DataGridView2.ReadOnly = True


    End Sub
    Sub KondisiAwal()
        LBLNamaPlg.Text = ""
        LBLAlamat.Text = ""
        LBLTelepon.Text = ""
        LBLTanggal.Text = Today
        LBLAdmin.Text = FormMenuUtama.STLabel4.Text
        LBLKembali.Text = ""
        TextBox2.Text = ""
        LBLNamaBarang.Text = ""
        LBLHargaBarang.Text = ""
        TextBox3.Text = ""
        TextBox3.Enabled = False
        LBLItem.Text = ""
        Call MunculKodePelanggan()
        Call NomorOtomatis()
        Call BuatKolom()
        Label9.Text = "0"
        TextBox1.Text = ""
        ComboBox1.Text = ""
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        LBLJam.Text = TimeOfDay
    End Sub

    Private Sub FormTransJual_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call KondisiAwal()
        Call MunculKodePelanggan()
        Call NomorOtomatis()
        Call BuatKolom()
        Call tampil_data()
        DataGridView2.Columns(0).Width = 110
        DataGridView2.Columns(0).Width = 110


       
    End Sub
    Sub MunculKodePelanggan()
        Call Koneksi()
        ComboBox1.Items.Clear()
        Cmd = New OdbcCommand("Select * from tbl_Pelanggan", Conn)
        Rd = Cmd.ExecuteReader
        Do While Rd.Read
            ComboBox1.Items.Add(Rd.Item(0))
        Loop
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Call Koneksi()
        Cmd = New OdbcCommand("Select * from tbl_Pelanggan where kodepelanggan ='" & ComboBox1.Text & "'", Conn)
        Rd = Cmd.ExecuteReader
        Rd.Read()
        If Rd.HasRows Then
            LBLNamaPlg.Text = Rd!NamaPelanggan
            LBLAlamat.Text = Rd!AlamatPelanggan
            LBLTelepon.Text = Rd!TelpPelanggan
        End If
    End Sub
    Sub NomorOtomatis()
        Call Koneksi()
        Cmd = New OdbcCommand("Select * from tbl_jual where nojual in (select max(nojual) from tbl_jual)", Conn)
        Dim UrutanKode As String
        Dim Hitung As Long
        Rd = Cmd.ExecuteReader
        Rd.Read()
        If Not Rd.HasRows Then
            UrutanKode = "J" + Format(Now, "yyMMdd") + "001"
        Else
            Hitung = Microsoft.VisualBasic.Right(Rd.GetString(0), 9) + 1
            UrutanKode = "J" + Format(Now, "yyMMdd") + Microsoft.VisualBasic.Right("000" & Hitung, 3)
        End If
        LBLNoJual.Text = UrutanKode
    End Sub
    Sub BuatKolom()
        DataGridView1.Columns.Clear()
        DataGridView1.Columns.Add("Kode", "Kode")
        DataGridView1.Columns.Add("Nama", "Nama Barang")
        DataGridView1.Columns.Add("Harga", "Harga")
        DataGridView1.Columns.Add("Jumlah", "Jumlah")
        DataGridView1.Columns.Add("Subtotal", "Subtotal")
    End Sub
  
    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If e.KeyChar = Chr(13) Then
            Call Koneksi()
            Cmd = New OdbcCommand("Select * From tbl_barang where kodebarang='" & TextBox2.Text & "'", Conn)
            Rd = Cmd.ExecuteReader
            Rd.Read()
            If Not Rd.HasRows Then
                MsgBox("Kode barang Tidak Ada")
            Else
                TextBox2.Text = Rd.Item("KodeBarang")
                LBLNamaBarang.Text = Rd.Item("NamaBarang")
                LBLHargaBarang.Text = Rd.Item("hargaBarang")
                LBLJumlahBrg.Text = Rd.Item("JumlahBarang")
                'TextBox4.Text = Rd.Item("jumlahBarang")
                'ComboBox1.Text = Rd.Item("satuanBarang")
                TextBox3.Enabled = True
            End If
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If LBLNamaBarang.Text = "" Or TextBox3.Text = "" Then
            MsgBox("Silahkan Masukkan Kode Barang dan Tekan ENTER!")
        Else
            If Val(LBLJumlahBrg.Text) < Val(TextBox3.Text) Then
                MsgBox("Barang Kurang!")
            Else
                DataGridView1.Rows.Add(New String() {TextBox2.Text, LBLNamaBarang.Text, LBLHargaBarang.Text, TextBox3.Text, Val(LBLHargaBarang.Text) * Val(TextBox3.Text)})
                Call RumusSubtotal()
                TextBox2.Text = ""
                LBLNamaBarang.Text = ""
                LBLHargaBarang.Text = ""
                TextBox3.Text = ""
                TextBox3.Enabled = False
                Call RumusCariItem()
            End If
           
        End If

    End Sub
    Sub RumusSubtotal()
        Dim hitung As Integer = 0
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            hitung = hitung + DataGridView1.Rows(i).Cells(4).Value
            Label9.Text = hitung
            Label9.Text = Format(hitung, "###,###,###")
           

        Next
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        If e.KeyChar = Chr(13) Then
            If Val(TextBox1.Text) < Val(Label9.Text) Then
                MsgBox("Pembayaran Kurang!")
            ElseIf Val(TextBox1.Text) = Val(Label9.Text) Then
                LBLKembali.Text = 0
            ElseIf Val(TextBox1.Text) > Val(Label9.Text) Then
                LBLKembali.Text = (TextBox1.Text) - (Label9.Text)
                Button1.Focus()

                'If TextBox1.Text < Label9.Text Then
                '    MsgBox("Pembayaran Kurang!")
                'ElseIf TextBox1.Text = Label9.Text Then
                '    LBLKembali.Text = 0
                'ElseIf TextBox1.Text > Label9.Text Then
                '    LBLKembali.Text = TextBox1.Text - Label9.Text
                '    Button1.Focus()
            End If
        End If
    End Sub
    Sub RumusCariItem()
        Dim HitungItem As Integer = 0
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            HitungItem = HitungItem + DataGridView1.Rows(i).Cells(3).Value
            LBLItem.Text = HitungItem
        Next
    End Sub



    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If LBLKembali.Text = "" Or LBLNamaPlg.Text = "" Or Label9.Text = "" Then
            MsgBox("Transaksi Tidak Ada, Silahkan Lakukan Transaksi Terlebih Dahulu")
        Else
            TglMySQL = Format(Today, "yyy-MM-dd")
            Dim SimpanJual As String = "Insert into tbl_jual values('" & LBLNoJual.Text & "', '" & TglMySQL & "', '" & LBLJam.Text & "', '" & LBLItem.Text & "', '" & Label9.Text & "', '" & TextBox1.Text & "', '" & LBLKembali.Text & "', '" & ComboBox1.Text & "','" & FormMenuUtama.STLabel2.Text & "') "
            Cmd = New OdbcCommand(SimpanJual, Conn)
            Cmd.ExecuteNonQuery()

            For Baris As Integer = 0 To DataGridView1.Rows.Count - 2
                Dim SimpanDetail As String = "Insert into tbl_detailjual values('" & LBLNoJual.Text & "', '" & DataGridView1.Rows(Baris).Cells(0).Value & "', '" & DataGridView1.Rows(Baris).Cells(1).Value & "', '" & DataGridView1.Rows(Baris).Cells(2).Value & "', '" & DataGridView1.Rows(Baris).Cells(3).Value & "', '" & DataGridView1.Rows(Baris).Cells(4).Value & "')"
                Cmd = New OdbcCommand(SimpanDetail, Conn)
                Cmd.ExecuteNonQuery()

                Cmd = New OdbcCommand("select * from tbl_barang where kodebarang='" & DataGridView1.Rows(Baris).Cells(0).Value & "'", Conn)
                Rd = Cmd.ExecuteReader
                Rd.Read()
                Dim KurangiStok As String = "Update tbl_barang set JumlahBarang = '" & Rd.Item("JumlahBarang") - DataGridView1.Rows(Baris).Cells(3).Value & "' where KodeBarang='" & DataGridView1.Rows(Baris).Cells(0).Value & "'"
                Cmd = New OdbcCommand(KurangiStok, Conn)
                Cmd.ExecuteNonQuery()
            Next

            If MessageBox.Show("Apakah ingin cetak nota...?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then

                AxCrystalReport1.SelectionFormula = "totext({tbl_Jual.NoJual})='" & LBLNoJual.Text & "'"
                AxCrystalReport1.ReportFileName = "notajual.rpt"
                AxCrystalReport1.WindowState = Crystal.WindowStateConstants.crptMaximized
                AxCrystalReport1.RetrieveDataFiles()
                AxCrystalReport1.Action = 1
                Call KondisiAwal()
            Else
                Call KondisiAwal()
                MsgBox("Transaksi Telah Berhasil Disimpan")

            End If
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Call KondisiAwal()
    End Sub

    Private Sub DataGridView2_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        Dim i As Integer
        i = DataGridView2.CurrentRow.Index

        '    Call Koneksi()
        '    Cmd = New OdbcCommand("Select * From tbl_barang where kodebarang='" & TextBox2.Text & "'", Conn)
        '    Rd = Cmd.ExecuteReader
        '    Rd.Read()
        '    If Not Rd.HasRows Then
        '        MsgBox("Kode barang Tidak Ada")
        '    Else
        On Error Resume Next

        TextBox2.Text = DataGridView2.Item(0, i).Value
        TextBox2.Focus()



        '        TextBox2.Text = Rd.Item("KodeBarang")

        '        TextBox3.Enabled = True
        '    End If
    End Sub

End Class