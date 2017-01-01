Imports System.Data.SqlClient
Imports System.Threading

Public Class Form1
    Dim _strConn As String = "Data Source=127.0.0.1;Initial Catalog='TSQL';Persist Security Info=True;User ID='';pwd='';Connect Timeout=3600"
    Dim ins As Object = New Object
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Visible = False

        For i = 1 To 1
            Dim ts As ThreadStart = New ThreadStart(AddressOf Insert_test)
            Dim t As Thread = New Thread(ts)
            t.Start()


            Dim ts2 As ThreadStart = New ThreadStart(AddressOf Insert_test)
            Dim t2 As Thread = New Thread(ts)
            t2.Start()
        Next

        Me.Close()
    End Sub

    Sub Insert_test()
        SyncLock ins
            Dim strSql As String = <a>
                                       <![CDATA[
                                            INSERT INTO Table_1 (id)
                                            VALUES
	                                            (@id)
                                        ]]>
                                   </a>.Value
            Dim ds As New Data.DataSet
            Using conn As SqlConnection = New SqlConnection(_strConn)
                conn.Open()
                Dim trans As SqlTransaction
                trans = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Using cmd As SqlCommand = New SqlCommand(strSql, conn)
                    cmd.Transaction = trans
                    cmd.Parameters.Add("id", SqlDbType.Char).Value = GetId()
                    cmd.ExecuteNonQuery()
                    trans.Commit()
                End Using
            End Using
        End SyncLock

    End Sub

    Function GetId()
        Dim td As String = Format(Now, "yyyyMMdd")
        Dim strSql As String = <a>
                                   <![CDATA[
                                        SELECT TOP 1 id
                                        FROM
	                                        Table_1
                                        WHERE
	                                        id LIKE @id
                                        ORDER BY
	                                        id DESC
                                    ]]>
                               </a>.Value
        Dim ds As New Data.DataSet
        Using conn As SqlConnection = New SqlConnection(_strConn)
            conn.Open()
            Using cmd As SqlCommand = New SqlCommand(strSql, conn)
                cmd.Parameters.Add("id", SqlDbType.Char).Value = td & "%"
                Using dr As SqlDataReader = cmd.ExecuteReader
                    If dr.HasRows Then
                        dr.Read()
                        GetId = td & (CInt(dr("id").ToString.Substring(8)) + 1).ToString.PadLeft(6, "0")
                    Else
                        GetId = td & "000001"
                    End If
                End Using
            End Using
        End Using
    End Function
End Class
