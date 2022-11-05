Imports System.Data

Public Class BankCash_G2
    Public TableName As String = "BankCash_G2"
    Public MainId As String = "BankCash_G2TypeId"
    Public SubId As String = "Flag"
    Public SubId2 As String = "InvoiceNo"


    Dim dt As New DataTable
    Dim bm As New BasicMethods

    WithEvents G As New MyGrid
    Public Flag As Integer = 0
    Private Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        If Not Md.Manager Then
            btnDelete.Visibility = Windows.Visibility.Hidden
            btnFirst.Visibility = Windows.Visibility.Hidden
            btnPrevios.Visibility = Windows.Visibility.Hidden
            btnNext.Visibility = Windows.Visibility.Hidden
            btnLast.Visibility = Windows.Visibility.Hidden

            btnPrint.Visibility = Windows.Visibility.Hidden
            btnPrint2.Visibility = Windows.Visibility.Hidden
            DayDate.IsEnabled = False
        End If

        lblNotes.Visibility = Windows.Visibility.Hidden
        Notes.Visibility = Windows.Visibility.Hidden
        GroupBoxDed.Visibility = Windows.Visibility.Hidden
        btnPrint2.Visibility = Windows.Visibility.Hidden
        bm.Addcontrol_MouseDoubleClick({CheckNo, CheckBankId})


        bm.Fields = New String() {MainId, SubId, SubId2, "MainLinkFile", "BankId", "DayDate", "Canceled", "CurrencyId"}
        bm.control = New Control() {BankCash_G2TypeId, txtFlag, txtID, MainLinkFile, BankId, DayDate, Canceled, CurrencyId}
        bm.KeyFields = New String() {MainId, SubId, SubId2}
        bm.Table_Name = TableName

        bm.FillCombo("GetEmpBankCash_G2Types @Flag=" & Flag & ",@EmpId=" & Md.UserName & "", BankCash_G2TypeId)
        bm.FillCombo("CheckTypes", CheckTypeId, "", , True, True)
        bm.FillCombo("LinkFile", MainLinkFile, "", , True)
        bm.FillCombo("select Id,Name from Currencies order by Id", CurrencyId)
        If Not Md.ShowCurrency Then
            lblCurrencyId.Visibility = Windows.Visibility.Hidden
            CurrencyId.Visibility = Windows.Visibility.Hidden
            lblValue.Visibility = Windows.Visibility.Hidden
            Value.Visibility = Windows.Visibility.Hidden
        End If
        If Not Md.ShowCostCenter Then
            CostCenterName.Visibility = Windows.Visibility.Hidden
        End If
        LoadResource()
        LoadWFH()
        btnNew_Click(sender, e)
        BankId.Focus()

    End Sub



    Structure GC
        Shared DocNo As String = "DocNo"
        Shared MainValue As String = "MainValue"
        Shared Exchange As String = "Exchange"
        Shared Value As String = "Value"
        Shared LinkFile As String = "LinkFile"
        Shared SubAccNo As String = "SubAccNo"
        Shared SubAccName As String = "SubAccName"

        Shared CurrencyId2 As String = "CurrencyId2"
        Shared MainValue2 As String = "MainValue2"
        Shared Exchange2 As String = "Exchange2"

        Shared CostCenterId As String = "CostCenterId"

        Shared CostTypeId As String = "CostTypeId"
        Shared PurchaseAccNo As String = "PurchaseAccNo"
        Shared ImportMessageId As String = "ImportMessageId"
        Shared StoreId As String = "StoreId"
        Shared StoreInvoiceNo As String = "StoreInvoiceNo"

        Shared Notes As String = "Notes"
        Shared CheckTypeId As String = "CheckTypeId"
        Shared CheckNo As String = "CheckNo"
        Shared CheckDate As String = "CheckDate"
        Shared CheckBankId As String = "CheckBankId"

        Shared MainValue2Ded As String = "MainValue2Ded"
        Shared Value2Ded As String = "Value2Ded"
        Shared DedNotes As String = "DedNotes"
    End Structure


    Private Sub LoadWFH()
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue

        G.Columns.Add(GC.DocNo, Resources.Item("DocNo"))
        G.Columns.Add(GC.MainValue, Resources.Item("MainValue"))
        G.Columns.Add(GC.Exchange, Resources.Item("Exchange"))
        G.Columns.Add(GC.Value, Resources.Item("Value"))

        Dim GCLinkFile As New Forms.DataGridViewComboBoxColumn
        GCLinkFile.HeaderText = Resources.Item("LinkFile")
        GCLinkFile.Name = GC.LinkFile
        bm.FillCombo("select Id,Name from LinkFile union all select 0 Id,'-' Name order by Id", GCLinkFile)
        G.Columns.Add(GCLinkFile)

        G.Columns.Add(GC.SubAccNo, Resources.Item("SubAccNo"))
        G.Columns.Add(GC.SubAccName, Resources.Item("SubAccName"))

        Dim GCCurrencyId2 As New Forms.DataGridViewComboBoxColumn
        GCCurrencyId2.HeaderText = Resources.Item("CurrencyId2")
        GCCurrencyId2.Name = GC.CurrencyId2
        bm.FillCombo("select Id,Name from Currencies order by Id", GCCurrencyId2)
        G.Columns.Add(GCCurrencyId2)
        G.Columns.Add(GC.MainValue2, Resources.Item("MainValue2"))
        G.Columns.Add(GC.Exchange2, Resources.Item("Exchange2"))

        G.Columns.Add(GC.CostCenterId, Resources.Item("CostCenterId"))

        Dim GCCostTypeId As New Forms.DataGridViewComboBoxColumn
        GCCostTypeId.HeaderText = Resources.Item("CostTypeId")
        GCCostTypeId.Name = GC.CostTypeId
        bm.FillCombo("select Id,Name from CostTypes union all select 0 Id,'-' Name order by Id", GCCostTypeId)
        G.Columns.Add(GCCostTypeId)

        G.Columns.Add(GC.PurchaseAccNo, Resources.Item("PurchaseAccNo"))
        G.Columns.Add(GC.ImportMessageId, Resources.Item("ImportMessageId"))
        G.Columns.Add(GC.StoreId, Resources.Item("StoreId"))
        G.Columns.Add(GC.StoreInvoiceNo, Resources.Item("StoreInvoiceNo"))

        G.Columns.Add(GC.Notes, Resources.Item("Notes"))

        G.Columns.Add(GC.CheckTypeId, Resources.Item("CheckTypeId"))
        G.Columns.Add(GC.CheckNo, Resources.Item("CheckNo"))
        G.Columns.Add(GC.CheckDate, Resources.Item("CheckDate"))
        G.Columns.Add(GC.CheckBankId, Resources.Item("CheckBankId"))

        G.Columns.Add(GC.MainValue2Ded, bm.ExecuteScalar("select dbo.GetAccName((select BankCash_G2_Flag" & Flag & "_DedAcc from Statics))"))
        G.Columns.Add(GC.Value2Ded, Resources.Item("Value2Ded"))
        G.Columns.Add(GC.DedNotes, Resources.Item("DedNotes"))


        G.Columns(GC.Exchange).Visible = Md.ShowCurrency
        G.Columns(GC.Exchange2).Visible = Md.ShowCurrency
        G.Columns(GC.Value).Visible = Md.ShowCurrency
        G.Columns(GC.MainValue2).Visible = Md.ShowCurrency
        G.Columns(GC.CurrencyId2).Visible = Md.ShowCurrency

        G.Columns(GC.CheckTypeId).Visible = False
        G.Columns(GC.CheckNo).Visible = False
        G.Columns(GC.CheckDate).Visible = False
        G.Columns(GC.CheckBankId).Visible = False


        G.Columns(GC.MainValue2Ded).Visible = False
        G.Columns(GC.Value2Ded).Visible = False
        G.Columns(GC.DedNotes).Visible = False

        If Flag = 2 Then
            G.Columns(GC.Exchange).ReadOnly = True
        End If
        G.Columns(GC.Value).ReadOnly = True
        G.Columns(GC.SubAccName).ReadOnly = True

        G.Columns(GC.CurrencyId2).ReadOnly = True
        G.Columns(GC.Exchange2).ReadOnly = True

        G.Columns(GC.SubAccNo).FillWeight = 80
        G.Columns(GC.CostCenterId).FillWeight = 80

        G.Columns(GC.CurrencyId2).FillWeight = 140

        G.Columns(GC.CostTypeId).FillWeight = 240
        G.Columns(GC.SubAccName).FillWeight = 240
        G.Columns(GC.Notes).FillWeight = 200
        G.Columns(GC.DocNo).FillWeight = 100

        G.Columns(GC.Value2Ded).ReadOnly = True

        If Md.ShowCostCenter Then
            G.Columns(GC.CostCenterId).Visible = True
            CostCenterName.Visibility = Windows.Visibility.Visible
        Else
            G.Columns(GC.CostCenterId).Visible = False
            CostCenterName.Visibility = Windows.Visibility.Hidden
        End If

        G.Columns(GC.CostTypeId).Visible = False
        G.Columns(GC.PurchaseAccNo).Visible = False
        G.Columns(GC.ImportMessageId).Visible = False
        G.Columns(GC.StoreId).Visible = False
        G.Columns(GC.StoreInvoiceNo).Visible = False
        PurchaseAccName.Visibility = Windows.Visibility.Hidden
        ImportMessageName.Visibility = Windows.Visibility.Hidden
        StoreName.Visibility = Windows.Visibility.Hidden
     
        AddHandler G.CellEndEdit, AddressOf GridCalcRow
        AddHandler G.KeyDown, AddressOf GridKeyDown
        AddHandler G.CellBeginEdit, AddressOf G_CellBeginEdit
        AddHandler G.SelectionChanged, AddressOf G_SelectionChanged
    End Sub

    Private Sub btnLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLast.Click
        bm.FirstLast(New String() {MainId, SubId, SubId2}, "Max", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Sub FillControls()
        If lop Then Return
        lop = True
        bm.FillControls()
        BankId_LostFocus(Nothing, Nothing)

        Dim dt As DataTable = bm.ExcuteAdapter("select * from " & TableName & " where " & MainId & "=" & BankCash_G2TypeId.SelectedValue.ToString & " and " & SubId & "=" & txtFlag.Text.Trim & " and " & SubId2 & "=" & txtID.Text)

        G.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add()
            G.Rows(i).Cells(GC.MainValue).Value = dt.Rows(i)("MainValue").ToString
            G.Rows(i).Cells(GC.Exchange).Value = dt.Rows(i)("Exchange").ToString
            G.Rows(i).Cells(GC.Value).Value = dt.Rows(i)("Value").ToString
            G.Rows(i).Cells(GC.LinkFile).Value = dt.Rows(i)("LinkFile").ToString
            G.Rows(i).Cells(GC.SubAccNo).Value = dt.Rows(i)("SubAccNo").ToString
            lop = False
            GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.SubAccNo).Index, i))
            lop = True
            G.Rows(i).Cells(GC.CostCenterId).Value = dt.Rows(i)("CostCenterId").ToString
            GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.CostCenterId).Index, i))
            G.Rows(i).Cells(GC.CostTypeId).Value = dt.Rows(i)("CostTypeId").ToString
            G.Rows(i).Cells(GC.Notes).Value = dt.Rows(i)("Notes").ToString
            G.Rows(i).Cells(GC.DocNo).Value = dt.Rows(i)("DocNo").ToString
            G.Rows(i).Cells(GC.CheckTypeId).Value = dt.Rows(i)("CheckTypeId").ToString
            G.Rows(i).Cells(GC.CheckNo).Value = dt.Rows(i)("CheckNo").ToString
            G.Rows(i).Cells(GC.CheckDate).Value = dt.Rows(i)("CheckDate").ToString
            G.Rows(i).Cells(GC.CheckBankId).Value = dt.Rows(i)("CheckBankId").ToString
            
            G.Rows(i).Cells(GC.CurrencyId2).Value = dt.Rows(i)("CurrencyId2").ToString
            G.Rows(i).Cells(GC.MainValue2).Value = dt.Rows(i)("MainValue2").ToString
            G.Rows(i).Cells(GC.Exchange2).Value = dt.Rows(i)("Exchange2").ToString

            G.Rows(i).Cells(GC.PurchaseAccNo).Value = dt.Rows(i)("PurchaseAccNo").ToString
            G.Rows(i).Cells(GC.ImportMessageId).Value = dt.Rows(i)("ImportMessageId").ToString
            G.Rows(i).Cells(GC.StoreId).Value = dt.Rows(i)("StoreId").ToString
            G.Rows(i).Cells(GC.StoreInvoiceNo).Value = dt.Rows(i)("StoreInvoiceNo").ToString

            G.Rows(i).Cells(GC.MainValue2Ded).Value = dt.Rows(i)("MainValue2Ded").ToString
            G.Rows(i).Cells(GC.Value2Ded).Value = dt.Rows(i)("Value2Ded").ToString
            G.Rows(i).Cells(GC.DedNotes).Value = dt.Rows(i)("DedNotes").ToString

        Next
        G.CurrentCell = G.Rows(dt.Rows.Count).Cells(GC.DocNo)
        DayDate.Focus()
        G.RefreshEdit()
        lop = False
        CalcTotal()
    End Sub
    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        bm.NextPrevious(New String() {MainId, SubId, SubId2}, New String() {BankCash_G2TypeId.SelectedValue.ToString, txtFlag.Text, txtID.Text}, "Next", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        AllowSave = False
        If BankCash_G2TypeId.SelectedIndex < 1 Then
            bm.ShowMSG("برجاء تحديد اليومية")
            BankCash_G2TypeId.Focus()
            Return
        End If
        If Val(txtID.Text) = 0 Then
            bm.ShowMSG("برجاء تحديد المسلسل")
            txtID.Focus()
            Return
        End If
        If Val(BankId.Text) = 0 Then
            bm.ShowMSG("برجاء تحديد " & lblBank.Content)
            BankId.Focus()
            Return
        End If

        G.EndEdit()

        For i As Integer = 0 To G.Rows.Count - 1
            If Val(G.Rows(i).Cells(GC.MainValue).Value) = 0 Then
                Continue For
            End If
            If Val(G.Rows(i).Cells(GC.SubAccNo).Value) = 0 Then
                bm.ShowMSG("برجاء تحديد الفرعى بالسطر " & (i + 1).ToString)
                G.Focus()
                G.CurrentCell = G.Rows(i).Cells(GC.SubAccNo)
                Return
            End If
            If Val(G.Rows(i).Cells(GC.CheckNo).Value) = 0 AndAlso Val(G.Rows(i).Cells(GC.CheckTypeId).Value) <> 3 AndAlso Val(G.Rows(i).Cells(GC.CheckTypeId).Value) > 1 Then
                bm.ShowMSG("برجاء تحديد رقم الشيك بالسطر " & (i + 1).ToString)
                G.Focus()
                G.CurrentCell = G.Rows(i).Cells(GC.Notes)
                G.CurrentCell = G.Rows(i).Cells(GC.DocNo)
                Return
            End If
            If G.Columns(GC.CostTypeId).Visible AndAlso Val(G.Rows(i).Cells(GC.LinkFile).Value) = 9 AndAlso Val(G.Rows(i).Cells(GC.CostTypeId).Value) = 0 Then
                bm.ShowMSG("برجاء تحديد نوع التكلفة بالسطر " & (i + 1).ToString)
                G.Focus()
                G.CurrentCell = G.Rows(i).Cells(GC.CostTypeId)
                Return
            End If

        Next

        If Not IsDate(DayDate.SelectedDate) Then
            bm.ShowMSG("برجاء تحديد التاريخ")
            DayDate.Focus()
            Return
        End If


        bm.DefineValues()

        If Not bm.SaveGrid(G, TableName, New String() {MainId, SubId, SubId2}, New String() {BankCash_G2TypeId.SelectedValue.ToString, txtFlag.Text.Trim, txtID.Text}, New String() {"MainValue", "Exchange", "Value", "LinkFile", "SubAccNo", "CostCenterId", "CostTypeId", "Notes", "DocNo", "CheckTypeId", "CheckNo", "CheckDate", "CheckBankId", "PurchaseAccNo", "ImportMessageId", "StoreId", "StoreInvoiceNo", "CurrencyId2", "MainValue2", "Exchange2", "MainValue2Ded", "Value2Ded", "DedNotes"}, New String() {GC.MainValue, GC.Exchange, GC.Value, GC.LinkFile, GC.SubAccNo, GC.CostCenterId, GC.CostTypeId, GC.Notes, GC.DocNo, GC.CheckTypeId, GC.CheckNo, GC.CheckDate, GC.CheckBankId, GC.PurchaseAccNo, GC.ImportMessageId, GC.StoreId, GC.StoreInvoiceNo, GC.CurrencyId2, GC.MainValue2, GC.Exchange2, GC.MainValue2Ded, GC.Value2Ded, GC.DedNotes}, New VariantType() {VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.String, VariantType.String, VariantType.Integer, VariantType.String, VariantType.Date, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.String}, New String() {GC.SubAccNo}) Then Return

        If Not bm.Save(New String() {MainId, SubId, SubId2}, New String() {BankCash_G2TypeId.SelectedValue.ToString, txtFlag.Text.Trim, txtID.Text}) Then Return

        If Not DontClear Then btnNew_Click(sender, e)
        AllowSave = True
    End Sub

    Dim lop As Boolean = False

    Sub ClearRow(ByVal i As Integer)
        G.Rows(i).Cells(GC.MainValue).Value = Nothing
        G.Rows(i).Cells(GC.Exchange).Value = Nothing
        G.Rows(i).Cells(GC.Value).Value = Nothing
        G.Rows(i).Cells(GC.LinkFile).Value = Nothing
        G.Rows(i).Cells(GC.SubAccNo).Value = Nothing
        G.Rows(i).Cells(GC.CostCenterId).Value = Nothing
        G.Rows(i).Cells(GC.CostTypeId).Value = Nothing
        G.Rows(i).Cells(GC.Notes).Value = Nothing
        G.Rows(i).Cells(GC.DocNo).Value = Nothing
        G.Rows(i).Cells(GC.CheckTypeId).Value = 1
        G.Rows(i).Cells(GC.CheckNo).Value = Nothing
        G.Rows(i).Cells(GC.CheckDate).Value = Nothing
        G.Rows(i).Cells(GC.CheckBankId).Value = Nothing

        G.Rows(i).Cells(GC.CurrencyId2).Value = Nothing
        G.Rows(i).Cells(GC.MainValue2).Value = Nothing
        G.Rows(i).Cells(GC.Exchange2).Value = Nothing

        G.Rows(i).Cells(GC.PurchaseAccNo).Value = Nothing
        G.Rows(i).Cells(GC.ImportMessageId).Value = Nothing
        G.Rows(i).Cells(GC.StoreId).Value = Nothing
        G.Rows(i).Cells(GC.StoreInvoiceNo).Value = Nothing

        G.Rows(i).Cells(GC.MainValue2Ded).Value = Nothing
        G.Rows(i).Cells(GC.Value2Ded).Value = Nothing
        G.Rows(i).Cells(GC.DedNotes).Value = Nothing

    End Sub

    Private Sub GridCalcRow(ByVal sender As Object, ByVal e As Forms.DataGridViewCellEventArgs)
        If lop Then Return
        Try
            If G.Columns(e.ColumnIndex).Name = GC.MainValue OrElse G.Columns(e.ColumnIndex).Name = GC.Exchange Then

                If Val(G.Rows(e.RowIndex).Cells(GC.Exchange).Value) = 0 Then
                    G.Rows(e.RowIndex).Cells(GC.Exchange).Value = bm.ExecuteScalar("select dbo.GetCurrencyExchange(" & Val(BankId.Text) & "," & MainLinkFile.SelectedValue & "," & CurrencyId.SelectedValue.ToString & ",0,'" & bm.ToStrDate(DayDate.SelectedDate) & "')")
                End If

                G.Rows(e.RowIndex).Cells(GC.MainValue).Value = Val(G.Rows(e.RowIndex).Cells(GC.MainValue).Value)
                G.Rows(e.RowIndex).Cells(GC.Value).Value = Math.Round(Val(G.Rows(e.RowIndex).Cells(GC.Exchange).Value) * Val(G.Rows(e.RowIndex).Cells(GC.MainValue).Value), 4, MidpointRounding.AwayFromZero)

                GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.MainValue2).Index, G.CurrentRow.Index))
            ElseIf G.Columns(e.ColumnIndex).Name = GC.SubAccNo Then
                dt = bm.ExcuteAdapter("select * from LinkFile where Id=" & G.Rows(e.RowIndex).Cells(GC.LinkFile).Value)

                If dt.Rows.Count > 0 Then
                    bm.LostFocusGrid(G.Rows(e.RowIndex).Cells(GC.SubAccNo), G.Rows(e.RowIndex).Cells(GC.SubAccName), "select Name from " & dt.Rows(0)("TableName") & " where Id=" & Val(G.Rows(e.RowIndex).Cells(GC.SubAccNo).Value))
                    bm.LostFocusGrid(G.Rows(e.RowIndex).Cells(GC.SubAccNo), G.Rows(e.RowIndex).Cells(GC.CurrencyId2), "select CurrencyId from " & dt.Rows(0)("TableName") & " where Id=" & Val(G.Rows(e.RowIndex).Cells(GC.SubAccNo).Value))

                    If Val(G.Rows(e.RowIndex).Cells(GC.CurrencyId2).Value) = Val(CurrencyId.SelectedValue) Then
                        G.Rows(e.RowIndex).Cells(GC.MainValue2).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.Exchange2).Value = G.Rows(e.RowIndex).Cells(GC.Exchange).Value
                        G.Rows(e.RowIndex).Cells(GC.MainValue2).Value = G.Rows(e.RowIndex).Cells(GC.MainValue).Value
                    ElseIf Val(G.Rows(e.RowIndex).Cells(GC.CurrencyId2).Value) = 1 Then
                        G.Rows(e.RowIndex).Cells(GC.MainValue2).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.Exchange2).Value = 1
                        G.Rows(e.RowIndex).Cells(GC.MainValue2).Value = G.Rows(e.RowIndex).Cells(GC.Value).Value
                    Else
                        G.Rows(e.RowIndex).Cells(GC.MainValue2).ReadOnly = False
                        If Val(G.Rows(e.RowIndex).Cells(GC.Exchange2).Value) = 0 Or Val(G.Rows(e.RowIndex).Cells(GC.MainValue2).Value) = 0 Then
                            G.Rows(e.RowIndex).Cells(GC.Exchange2).Value = bm.ExecuteScalar("select dbo.GetCurrencyExchange(" & Val(G.Rows(e.RowIndex).Cells(GC.SubAccNo).Value) & "," & G.Rows(e.RowIndex).Cells(GC.LinkFile).Value & "," & G.Rows(e.RowIndex).Cells(GC.CurrencyId2).Value & ",0,'" & bm.ToStrDate(DayDate.SelectedDate) & "')")
                            G.Rows(e.RowIndex).Cells(GC.MainValue2).Value = Math.Round(G.Rows(e.RowIndex).Cells(GC.Value).Value / G.Rows(e.RowIndex).Cells(GC.Exchange2).Value, 4, MidpointRounding.AwayFromZero)
                        End If
                    End If
                Else
                    G.Rows(e.RowIndex).Cells(GC.SubAccNo).Value = ""
                    G.Rows(e.RowIndex).Cells(GC.SubAccName).Value = ""
                    G.Rows(e.RowIndex).Cells(GC.CurrencyId2).Value = Nothing
                    G.Rows(e.RowIndex).Cells(GC.MainValue2).Value = ""
                    G.Rows(e.RowIndex).Cells(GC.Exchange2).Value = ""
            End If
            ElseIf G.Columns(e.ColumnIndex).Name = GC.MainValue2 Then
                G.Rows(e.RowIndex).Cells(GC.Exchange2).Value = Val(G.Rows(e.RowIndex).Cells(GC.Value).Value) / Val(G.Rows(e.RowIndex).Cells(GC.MainValue2).Value)
                G.Rows(e.RowIndex).Cells(GC.Value2Ded).Value = Val(G.Rows(e.RowIndex).Cells(GC.MainValue2Ded).Value) * Val(G.Rows(e.RowIndex).Cells(GC.Exchange2).Value)
            ElseIf G.Columns(e.ColumnIndex).Name = GC.MainValue2Ded Then
                G.Rows(e.RowIndex).Cells(GC.Value2Ded).Value = Val(G.Rows(e.RowIndex).Cells(GC.MainValue2Ded).Value) * Val(G.Rows(e.RowIndex).Cells(GC.Exchange2).Value)
            ElseIf G.Columns(e.ColumnIndex).Name = GC.CostCenterId Then
                bm.CostCenterIdLostFocusGrid(G.Rows(e.RowIndex).Cells(GC.CostCenterId), CostCenterName)
                'ElseIf G.Columns(e.ColumnIndex).Name = GC.PurchaseAccNo Then
                '    bm.AccNoLostFocusGrid(G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo), PurchaseAccName)
            ElseIf G.Columns(e.ColumnIndex).Name = GC.PurchaseAccNo Then
                bm.LostFocusGrid(G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo), PurchaseAccName, "select Name from OrderTypes where Id=" & Val(G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).Value))
                'bm.AccNoLostFocusGrid(G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo), PurchaseAccName)
            ElseIf G.Columns(e.ColumnIndex).Name = GC.ImportMessageId Then
                bm.LostFocusGrid(G.Rows(e.RowIndex).Cells(GC.ImportMessageId), ImportMessageName, "select dbo.GetAccName(AccNo) from ImportMessages  where OrderTypeId='" & G.Rows(G.CurrentCell.RowIndex).Cells(GC.PurchaseAccNo).Value & "' and Id=" & Val(G.Rows(e.RowIndex).Cells(GC.ImportMessageId).Value))
            ElseIf G.Columns(e.ColumnIndex).Name = GC.StoreId Then
                bm.LostFocusGrid(G.Rows(e.RowIndex).Cells(GC.StoreId), StoreName, "select Name from Fn_EmpStores(" & Md.UserName & ") where Id=" & Val(G.Rows(e.RowIndex).Cells(GC.StoreId).Value))
            ElseIf G.Columns(e.ColumnIndex).Name = GC.StoreInvoiceNo Then
                'If Not G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).Value = Nothing AndAlso Not bm.IF_Exists("select InvoiceNo from SalesMaster where Temp=0 and StoreId=" & G.CurrentRow.Cells(GC.StoreId).Value & " and Flag=" & Sales.FlagState.الاستيراد & " and InvoiceNo=" & G.CurrentRow.Cells(GC.StoreInvoiceNo).Value) Then
                '    bm.ShowMSG("هذا الرقم غير صحيح")
                '    G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).Value = Nothing
                '    Exit Sub
                'End If
            ElseIf G.Columns(e.ColumnIndex).Name = GC.CostTypeId Then
                Select Case G.Rows(e.RowIndex).Cells(GC.CostTypeId).Value
                    Case 1
                        G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.ImportMessageId).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.StoreId).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).ReadOnly = True
                    Case 2
                        G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.ImportMessageId).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.StoreId).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).ReadOnly = True
                    Case 3
                        G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.ImportMessageId).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.StoreId).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).ReadOnly = True
                    Case 4
                        G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.ImportMessageId).ReadOnly = True
                        G.Rows(e.RowIndex).Cells(GC.StoreId).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).ReadOnly = False
                    Case Else
                        G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.ImportMessageId).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.StoreId).ReadOnly = False
                        G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).ReadOnly = False
                End Select

                If G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).ReadOnly Then G.Rows(e.RowIndex).Cells(GC.PurchaseAccNo).Value = ""
                If G.Rows(e.RowIndex).Cells(GC.ImportMessageId).ReadOnly Then G.Rows(e.RowIndex).Cells(GC.ImportMessageId).Value = ""
                If G.Rows(e.RowIndex).Cells(GC.StoreId).ReadOnly Then G.Rows(e.RowIndex).Cells(GC.StoreId).Value = ""
                If G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).ReadOnly Then G.Rows(e.RowIndex).Cells(GC.StoreInvoiceNo).Value = ""

            End If

            loplop = True
            Try
                If Val(G.Rows(e.RowIndex).Cells(GC.CheckTypeId).Value) < 1 Then G.Rows(e.RowIndex).Cells(GC.CheckTypeId).Value = 1
                CheckTypeId.SelectedValue = G.Rows(e.RowIndex).Cells(GC.CheckTypeId).Value
                CheckNo.Text = G.Rows(e.RowIndex).Cells(GC.CheckNo).Value

                MainValue2Ded.Text = G.Rows(e.RowIndex).Cells(GC.MainValue2Ded).Value
                Value2Ded.Text = G.Rows(e.RowIndex).Cells(GC.Value2Ded).Value
                DedNotes.Text = G.Rows(e.RowIndex).Cells(GC.DedNotes).Value

                CheckBankId.Text = G.Rows(e.RowIndex).Cells(GC.CheckBankId).Value
                CheckBankId_LostFocus(Nothing, Nothing)
                CheckDate.SelectedDate = Nothing
                If G.Rows(e.RowIndex).Cells(GC.CheckDate).Value Is Nothing Then
                    G.Rows(e.RowIndex).Cells(GC.CheckDate).Value = Nothing
                Else
                    CheckDate.SelectedDate = DateTime.Parse(G.Rows(e.RowIndex).Cells(GC.CheckDate).Value)
                End If
            Catch ex As Exception
            End Try
            loplop = False
            TestEnable()

            CalcTotal()
            G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
        Catch ex As Exception
        End Try
    End Sub
    Dim loplop As Boolean = False

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click
        bm.FirstLast(New String() {MainId, SubId, SubId2}, "Min", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        bm.ClearControls(False)
        ClearControls()
    End Sub

    Sub ClearControls()
        If lop OrElse lv Then Return
        lop = True

        DayDate.SelectedDate = bm.MyGetDate()
        G.Rows.Clear()
        CalcTotal()

        CheckTypeId.SelectedValue = 1
        CheckNo.Clear()
        CheckDate.SelectedDate = Nothing
        CheckBankId.Clear()
        CheckBankName.Clear()
        TestEnable()

        CostCenterName.Content = ""
        ImportMessageName.Content = ""
        StoreName.Content = ""
        Value.Clear()
        MainValue.Clear()
        BankId_LostFocus(Nothing, Nothing)
        Dim MyNow As DateTime = bm.MyGetDate()
        DayDate.SelectedDate = MyNow
        txtFlag.Text = Flag
        If BankCash_G2TypeId.SelectedIndex < 1 Then
            lop = False
            Return
        End If
        txtID.Text = bm.ExecuteScalar("select max(" & SubId2 & ")+1 from " & TableName & " where " & MainId & "=" & BankCash_G2TypeId.SelectedValue.ToString & " and " & SubId & "=" & txtFlag.Text)
        dt = bm.ExcuteAdapter("select FromInvoiceNo,ToInvoiceNo from BankCash_G2Types where Flag=" & Flag & " and Id=" & BankCash_G2TypeId.SelectedValue.ToString)
        If dt.Rows.Count = 0 Then
            lop = False
            Return
        End If
        If txtID.Text = "" Then txtID.Text = dt.Rows(0)("FromInvoiceNo")
        If Val(txtID.Text) > dt.Rows(0)("ToInvoiceNo") Then txtID.Text = dt.Rows(0)("ToInvoiceNo")
        'DayDate.Focus()
        txtID.Focus()
        txtID.SelectAll()
        lop = False

    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExcuteNonQuery("delete from " & TableName & " where " & MainId & "=" & BankCash_G2TypeId.SelectedValue.ToString & " and " & SubId & "='" & txtFlag.Text.Trim & "' and " & SubId2 & "=" & txtID.Text)
            btnNew_Click(sender, e)
        End If
    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {MainId, SubId, SubId2}, New String() {BankCash_G2TypeId.SelectedValue.ToString, txtFlag.Text, txtID.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub
    Dim lv As Boolean = False

    Private Sub txtID_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.LostFocus
        If lv Then
            Return
        End If
        lv = True

        bm.DefineValues()
        Dim dt As New DataTable
        bm.RetrieveAll(New String() {MainId, SubId, SubId2}, New String() {BankCash_G2TypeId.SelectedValue.ToString, txtFlag.Text.Trim, txtID.Text}, dt)
        If dt.Rows.Count = 0 Then
            ClearControls()

            dt = bm.ExcuteAdapter("select FromInvoiceNo,ToInvoiceNo from BankCash_G2Types where Flag=" & Flag & " and Id=" & BankCash_G2TypeId.SelectedValue.ToString)
            If dt.Rows.Count > 0 Then
                If Val(txtID.Text) < dt.Rows(0)("FromInvoiceNo") OrElse Val(txtID.Text) > dt.Rows(0)("ToInvoiceNo") Then txtID.Text = ""
            End If

            lv = False
            Return
        End If
        FillControls()
        lv = False
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub txtID_KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Value.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub

    Private Sub BankId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BankId.LostFocus
        Try
            If Val(BankId.Text.Trim) = 0 Then
                BankId.Clear()
                BankName.Clear()
                Return
            End If

            dt = bm.ExcuteAdapter("select * from LinkFile where Id=" & MainLinkFile.SelectedValue)
            bm.LostFocus(BankId, BankName, "select Name from Fn_EmpPermissions(" & MainLinkFile.SelectedValue & "," & Md.UserName & ") where Id=" & BankId.Text.Trim())
            CurrencyId.SelectedValue = bm.ExecuteScalar("select CurrencyId from " & dt.Rows(0)("TableName") & " where Id=" & BankId.Text.Trim())
            CurrencyId_SelectionChanged(Nothing, Nothing)
        Catch
        End Try
    End Sub
    Private Sub BankId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles BankId.KeyUp
        dt = bm.ExcuteAdapter("select * from LinkFile where Id=" & MainLinkFile.SelectedValue)
        If dt.Rows.Count > 0 AndAlso bm.ShowHelp(dt.Rows(0)("TableName"), BankId, BankName, e, "select cast(Id as varchar(100)) Id,Name from Fn_EmpPermissions(" & MainLinkFile.SelectedValue & "," & Md.UserName & ")") Then
            BankId_LostFocus(Nothing, Nothing)
        End If
    End Sub


    Private Sub CheckBankId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBankId.LostFocus
        If Val(CheckBankId.Text.Trim) = 0 Then
            CheckBankId.Clear()
            CheckBankName.Clear()
            Return
        End If
        bm.LostFocus(CheckBankId, CheckBankName, "select Name from CheckBanks where Id=" & CheckBankId.Text.Trim())
    End Sub
    Private Sub CheckBankId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CheckBankId.KeyUp
        If bm.ShowHelp("Banks", CheckBankId, CheckBankName, e, "select cast(Id as varchar(100)) Id,Name from CheckBanks", "CheckBanks") Then
            CheckBankId_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub CheckNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CheckNo.KeyUp
        Dim str As String = "GetCheckStates "
        If Flag = 2 Then
            str &= " @LinkFile=" & Val(MainLinkFile.SelectedValue) & ",@AccNo=" & Val(BankId.Text) & ",@LinkFile2=" & Val(G.CurrentRow.Cells(GC.LinkFile).Value) & ",@AccNo2=" & Val(G.CurrentRow.Cells(GC.SubAccNo).Value)
        Else
            str &= " @LinkFile2=" & Val(MainLinkFile.SelectedValue) & ",@AccNo2=" & Val(BankId.Text) & ",@LinkFile=" & Val(G.CurrentRow.Cells(GC.LinkFile).Value) & ",@AccNo=" & Val(G.CurrentRow.Cells(GC.SubAccNo).Value)
        End If

        If bm.ShowHelpMultiColumns("الشيكات", CheckNo, CheckNo, e, str) Then
            'CheckNo.Text = bm.SelectedRow(0)
            CheckNo_LostFocus(Nothing, Nothing)
            CheckBankId_LostFocus(Nothing, Nothing)

            Try
                G.CurrentRow.Cells(GC.MainValue).Value = bm.SelectedRow("المتبقي")
                GridCalcRow(Nothing, New System.Windows.Forms.DataGridViewCellEventArgs(G.Columns(GC.MainValue).Index, G.CurrentRow.Index))
                G.CurrentRow.Cells(GC.MainValue2).Value = 0
                GridCalcRow(Nothing, New System.Windows.Forms.DataGridViewCellEventArgs(G.Columns(GC.SubAccNo).Index, G.CurrentRow.Index))
            Catch
            End Try

        End If

    End Sub

    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")

        lblID.SetResourceReference(Label.ContentProperty, "Id")

        lblDayDate.SetResourceReference(Label.ContentProperty, "DayDate")
        lblNotes.SetResourceReference(Label.ContentProperty, "Notes")
        lblNotes2.SetResourceReference(Label.ContentProperty, "Notes")

        lblMain.SetResourceReference(Label.ContentProperty, "DailyMotion")
        lblID.SetResourceReference(Label.ContentProperty, "Id")
        lblDayDate.SetResourceReference(Label.ContentProperty, "DayDate")
        lblCheckType.SetResourceReference(Label.ContentProperty, "Status")
        lblCheckNo.SetResourceReference(Label.ContentProperty, "CheckNo")
        lblCheckDayDate.SetResourceReference(Label.ContentProperty, "DayDate")
        lblCheckBankId.SetResourceReference(Label.ContentProperty, "Bank")
        lblParty.SetResourceReference(Label.ContentProperty, "Party")
        lblCurrencyId.SetResourceReference(Label.ContentProperty, "Currency")
        lblTotal.SetResourceReference(Label.ContentProperty, "Total")
        btnChangeCheckNo.SetResourceReference(Button.ContentProperty, "ChangeCheckNo")
        btnPrint2.SetResourceReference(Button.ContentProperty, "PrintDeduction")
        Canceled.SetResourceReference(Label.ContentProperty, "Canceled")
        lblBank.SetResourceReference(Label.ContentProperty, "Sub")
        btnPrint.SetResourceReference(Button.ContentProperty, "Print")
        btnDeleteRow.SetResourceReference(Button.ContentProperty, "DeleteRow")
        'lbl.SetResourceReference(Label.ContentProperty, "")
        'lbl.SetResourceReference(Label.ContentProperty, "")
        'lbl.SetResourceReference(Label.ContentProperty, "")
        'lbl.SetResourceReference(Label.ContentProperty, "")
        'lbl.SetResourceReference(Label.ContentProperty, "")
        'lbl.SetResourceReference(Label.ContentProperty, "")

    End Sub

    Private Sub btnDeleteRow_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDeleteRow.Click
        Try
            If Not G.CurrentRow.ReadOnly AndAlso bm.ShowDeleteMSG("MsgDeleteRow") Then
                G.Rows.Remove(G.CurrentRow)
                CalcTotal()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Dim LopCalc As Boolean = False
    Private Sub CalcTotal()
        If LopCalc Or lop Then Return
        LopCalc = True
        Try
            Value.Text = Math.Round(0, 4)
            MainValue.Text = Math.Round(0, 4)
            For i As Integer = 0 To G.Rows.Count - 1
                Value.Text += Val(G.Rows(i).Cells(GC.Value).Value)
                MainValue.Text += Val(G.Rows(i).Cells(GC.MainValue).Value)
            Next
        Catch ex As Exception
        End Try
        LopCalc = False
    End Sub


    Private Sub GridKeyDown(ByVal sender As Object, ByVal e As Forms.KeyEventArgs)
        'e.Handled = True
        If G.CurrentCell Is Nothing OrElse G.CurrentCell.ReadOnly Then Return
        Try
            If G.CurrentCell.RowIndex = G.Rows.Count - 1 Then
                Dim c = G.CurrentCell.RowIndex
                G.Rows.Add()
                G.CurrentCell = G.Rows(c).Cells(G.CurrentCell.ColumnIndex)
            End If
            If G.CurrentCell.ColumnIndex = G.Columns(GC.SubAccNo).Index Then
                dt = bm.ExcuteAdapter("select * from LinkFile where Id=" & G.Rows(G.CurrentCell.RowIndex).Cells(GC.LinkFile).Value)

                If dt.Rows.Count > 0 AndAlso bm.ShowHelpGrid(dt.Rows(0)("TableName"), G.Rows(G.CurrentCell.RowIndex).Cells(GC.SubAccNo), G.Rows(G.CurrentCell.RowIndex).Cells(GC.SubAccName), e, "select cast(Id as varchar(100)) Id,Name from " & dt.Rows(0)("TableName")) Then
                    If G.Columns(GC.CostCenterId).Visible Then
                        G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.CostCenterId)
                    ElseIf G.Columns(GC.MainValue2).Visible Then
                        G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.MainValue2)
                    Else
                        G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Notes)
                    End If
                End If
            ElseIf G.CurrentCell.ColumnIndex = G.Columns(GC.CostCenterId).Index Then
                If bm.ShowHelpGrid("CostCenters", G.Rows(G.CurrentCell.RowIndex).Cells(GC.CostCenterId), CostCenterName, e, "select cast(Id as varchar(100)) Id,Name from CostCenters where SubType=1") Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Notes)
                End If
            ElseIf G.CurrentCell.ColumnIndex = G.Columns(GC.PurchaseAccNo).Index Then
                If bm.ShowHelpGrid("OrderTypes", G.Rows(G.CurrentCell.RowIndex).Cells(GC.PurchaseAccNo), PurchaseAccName, e, "select cast(Id as varchar(100)) Id,Name from OrderTypes") Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.ImportMessageId)
                End If

                'If bm.AccNoShowHelpGrid(G.CurrentRow.Cells(GC.PurchaseAccNo), PurchaseAccName, e, 1, , True) Then
                '    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.ImportMessageId)
                'End If
            ElseIf G.CurrentCell.ColumnIndex = G.Columns(GC.ImportMessageId).Index Then
                If bm.ShowHelpGrid("ImportMessages", G.Rows(G.CurrentCell.RowIndex).Cells(GC.ImportMessageId), ImportMessageName, e, "select cast(Id as varchar(100)) 'رقم الرسالة',dbo.GetShipperName(ShipperId) الشاحن from ImportMessages where OrderTypeId='" & G.Rows(G.CurrentCell.RowIndex).Cells(GC.PurchaseAccNo).Value & "'", "", "رقم الرسالة", "الشاحن") Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.StoreId)
                End If
            ElseIf G.CurrentCell.ColumnIndex = G.Columns(GC.StoreId).Index Then
                If bm.ShowHelpGrid("Stores", G.Rows(G.CurrentCell.RowIndex).Cells(GC.StoreId), StoreName, e, "select cast(Id as varchar(100)) Id,Name from Fn_EmpStores(" & Md.UserName & ")") Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.StoreInvoiceNo)
                End If
            ElseIf G.CurrentCell.ColumnIndex = G.Columns(GC.StoreInvoiceNo).Index Then
                'If bm.ShowHelpGridMultiColumns("الفواتير", G.CurrentRow.Cells(GC.StoreInvoiceNo), G.CurrentRow.Cells(GC.StoreInvoiceNo), e, "select cast(M.InvoiceNo as varchar(100)) 'الفاتورة',dbo.GetSupplierName(M.ToId) 'المورد',M.DocNo 'رقم عقد المورد',cast(TotalAfterDiscount as nvarchar(100)) 'إجمالي الفاتورة',cast(M.OrderTypeId as nvarchar(100)) 'مسلسل الطلبية',dbo.GetOrderTypes(M.OrderTypeId) 'اسم الطلبية',(case when isnull(MM.IsDelivered,0)=1 then 'تم الاستلام' else 'لم يتم الاستلام' end) 'الحالة' from SalesMaster M left join ImportMessagesDetails DD on(M.OrderTypeId=DD.OrderTypeId and M.StoreId=DD.StoreId and M.InvoiceNo=DD.InvoiceNo) left join ImportMessages MM on(MM.OrderTypeId=DD.OrderTypeId and MM.Id=DD.Id) where M.Temp=0 and M.StoreId=" & G.CurrentRow.Cells(GC.StoreId).Value & " and M.Flag=" & Sales.FlagState.الاستيراد & IIf(G.CurrentRow.Cells(GC.LinkFile).Value = 2, " and M.ToId=" & G.CurrentRow.Cells(GC.SubAccNo).Value, "")) Then
                '    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Notes)
                'End If
            End If
        Catch ex As Exception
        End Try
        G.CommitEdit(Forms.DataGridViewDataErrorContexts.Commit)
    End Sub


    Private Sub G_CellBeginEdit(sender As Object, e As Forms.DataGridViewCellCancelEventArgs)
        If CType(G.Rows(e.RowIndex).Cells(GC.LinkFile), System.Windows.Forms.DataGridViewComboBoxCell).Value Is Nothing Then
            CType(G.Rows(e.RowIndex).Cells(GC.LinkFile), System.Windows.Forms.DataGridViewComboBoxCell).Value = "0"
        End If
        If CType(G.Rows(e.RowIndex).Cells(GC.CostTypeId), System.Windows.Forms.DataGridViewComboBoxCell).Value Is Nothing Then
            CType(G.Rows(e.RowIndex).Cells(GC.CostTypeId), System.Windows.Forms.DataGridViewComboBoxCell).Value = "0"
        End If
    End Sub

    Private Sub G_SelectionChanged(sender As Object, e As EventArgs)
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.LinkFile).Index, G.CurrentRow.Index))
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.SubAccNo).Index, G.CurrentRow.Index))
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.CostCenterId).Index, G.CurrentRow.Index))
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.PurchaseAccNo).Index, G.CurrentRow.Index))
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.ImportMessageId).Index, G.CurrentRow.Index))
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.StoreId).Index, G.CurrentRow.Index))
    End Sub

    Private Sub BankCash_G2TypeId_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles BankCash_G2TypeId.SelectionChanged
        btnNew_Click(Nothing, Nothing)
    End Sub

    Private Sub Exchange_TextChanged(sender As Object, e As TextChangedEventArgs) 'Handles Exchange.TextChanged
        For i As Integer = 0 To G.Rows.Count - 1
            If Val(G.Rows(i).Cells(GC.MainValue).Value) <> 0 Then
                GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.MainValue).Index, i))
            End If
        Next
    End Sub

    Private Sub CheckTypeId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CheckTypeId.LostFocus
        If loplop Then Return
        If G.CurrentRow Is Nothing Then
            G.CurrentCell = G.Rows(G.Rows.Add).Cells(GC.DocNo)
        End If

        Try
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckTypeId).Value = CheckTypeId.SelectedValue
        Catch ex As Exception
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckTypeId).Value = 1
        End Try
        TestEnable()
    End Sub

    Private Sub CheckNo_LostFocus(sender As Object, e As RoutedEventArgs) Handles CheckNo.LostFocus
        Try
            dt = bm.ExcuteAdapter("select top 1 dbo.ToStrDate(CheckDate),CheckBankId from BankCash_G2 where CheckNo='" & CheckNo.Text & "' Order by Daydate")
            If dt.Rows.Count > 0 Then
                CheckDate.SelectedDate = CType(dt.Rows(0)(0), DateTime)
                CheckBankId.Text = dt.Rows(0)(1)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub CheckNo_TextChanged(sender As Object, e As TextChangedEventArgs) Handles CheckNo.TextChanged
        If loplop OrElse G.CurrentRow Is Nothing Then Return
        If G.CurrentRow.Index = G.NewRowIndex Then
            Dim i As Integer = G.Rows.Add
            G.CurrentCell = G.Rows(i).Cells(G.CurrentCell.ColumnIndex)
        End If
        Try
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckNo).Value = CheckNo.Text
        Catch ex As Exception
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckNo).Value = ""
        End Try
    End Sub

    Private Sub CheckDate_TextChanged(sender As Object, e As SelectionChangedEventArgs) Handles CheckDate.SelectedDateChanged
        If loplop OrElse G.CurrentRow Is Nothing Then Return
        Try
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckDate).Value = CheckDate.SelectedDate.Value.Date
        Catch ex As Exception
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckDate).Value = Nothing
        End Try
    End Sub

    Private Sub CheckBankId_TextChanged(sender As Object, e As TextChangedEventArgs) Handles CheckBankId.TextChanged
        If loplop OrElse G.CurrentRow Is Nothing Then Return
        Try
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckBankId).Value = CheckBankId.Text
        Catch ex As Exception
            G.Rows(G.CurrentRow.Index).Cells(GC.CheckBankId).Value = ""
        End Try
        CheckBankId_LostFocus(Nothing, Nothing)
    End Sub


    Private Sub Ded_TextChanged(sender As Object, e As TextChangedEventArgs) Handles MainValue2Ded.TextChanged, Value2Ded.TextChanged, DedNotes.TextChanged
        If loplop OrElse G.CurrentRow Is Nothing Then Return
        Try
            G.Rows(G.CurrentRow.Index).Cells(GC.MainValue2Ded).Value = MainValue2Ded.Text
            G.Rows(G.CurrentRow.Index).Cells(GC.Value2Ded).Value = Value2Ded.Text
            G.Rows(G.CurrentRow.Index).Cells(GC.DedNotes).Value = DedNotes.Text
        Catch ex As Exception
            G.Rows(G.CurrentRow.Index).Cells(GC.MainValue2Ded).Value = Nothing
            G.Rows(G.CurrentRow.Index).Cells(GC.Value2Ded).Value = Nothing
            G.Rows(G.CurrentRow.Index).Cells(GC.DedNotes).Value = Nothing
        End Try
        GridCalcRow(G, New Forms.DataGridViewCellEventArgs(G.Columns(GC.MainValue2Ded).Index, G.CurrentRow.Index))
    End Sub


    Private Sub TestEnable()
        If CheckTypeId.SelectedValue = 1 Then
            CheckNo.IsReadOnly = False
            CheckNo.IsEnabled = False
            CheckDate.IsEnabled = False
            CheckBankId.IsEnabled = False
            CheckNo.Clear()
            CheckDate.SelectedDate = Nothing
            CheckBankId.Clear()
            CheckBankName.Clear()
        ElseIf CheckTypeId.SelectedValue = 2 OrElse CheckTypeId.SelectedValue = 3 Then
            CheckNo.IsReadOnly = False
            CheckNo.IsEnabled = True
            CheckDate.IsEnabled = True
            CheckBankId.IsEnabled = True
            If CheckNo.Text.Trim = "" AndAlso CheckTypeId.SelectedValue = 3 Then
                lop = True
                CheckNo.Text = Flag & "-" & BankCash_G2TypeId.SelectedValue & "-" & txtID.Text & "-" & (G.CurrentRow.Index + 1)
                lop = False
            End If
        Else
            CheckNo.IsReadOnly = True
            CheckNo.IsEnabled = True
            CheckDate.IsEnabled = False
            CheckBankId.IsEnabled = False
        End If

    End Sub

    Private Sub CurrencyId_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CurrencyId.SelectionChanged
        If lop Then Return
        Try
            'G.Rows(e.RowIndex).Cells(GC.Exchange).Value= bm.ExecuteScalar("select dbo.GetCurrencyExchange(" & Val(BankId.Text) & "," & MainLinkFile.SelectedValue & "," & CurrencyId.SelectedValue.ToString & ",0,'"  & bm.ToStrDate(DayDate .SelectedDate ) & "')")
        Catch ex As Exception
        End Try
    End Sub

    Dim AllowSave As Boolean = False
    Dim DontClear As Boolean = False
    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        DontClear = True
        btnSave_Click(sender, e)
        DontClear = False
        Dim rpt As New ReportViewer
        rpt.Header = CType(Parent, Page).Title
        rpt.paraname = New String() {"@BankCash_G2TypeId", "@Flag", "@InvoiceNo", "Header"}
        rpt.paravalue = New String() {BankCash_G2TypeId.SelectedValue.ToString, Flag, txtID.Text, CType(Parent, Page).Title}
        rpt.Rpt = "BankCash_G21.rpt"
        rpt.Show()
    End Sub

    Private Sub btnPrint2_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint2.Click
        DontClear = True
        btnSave_Click(sender, e)
        DontClear = False
        Dim rpt As New ReportViewer
        rpt.Header = CType(Parent, Page).Title
        rpt.paraname = New String() {"@BankCash_G2TypeId", "@Flag", "@InvoiceNo", "Header"}
        rpt.paravalue = New String() {BankCash_G2TypeId.SelectedValue.ToString, Flag, txtID.Text, G.Columns(GC.MainValue2Ded).HeaderText}
        rpt.Rpt = "BankCash_G22.rpt"
        rpt.Show()
    End Sub

    Private Sub btnChangeCheckNo_Click(sender As Object, e As RoutedEventArgs) Handles btnChangeCheckNo.Click
        Dim frm As New Window 'With {.SizeToContent = True}
        frm.Content = New ChangeCheckNo With {.MyCheckNo = CheckNo.Text, .txtCheck = CheckNo}
        frm.ShowDialog()
        If CType(frm.Content, ChangeCheckNo).AllowChange Then
            'CheckNo.Text = CType(frm.Content, ChangeCheckNo).MyCheckNo
            CheckNo_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub MainLinkFile_LostFocus(sender As Object, e As RoutedEventArgs) Handles MainLinkFile.LostFocus
        BankId_LostFocus(Nothing, Nothing)
    End Sub
End Class
