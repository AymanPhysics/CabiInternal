Imports System.Data
Imports System.Windows
Imports System.Windows.Media
Imports System.Management

Public Class Sales

    Public MainTableName As String = "Stores"
    Public MainSubId As String = "Id"
    Public MainSubName As String = "Name"

    Public TableName As String = "SalesMaster"
    Public TableDetailsName As String = "SalesDetails"

    Public MainId As String = "StoreId"
    Public SubId As String = "InvoiceNo"

    Dim dv As New DataView
    Dim HelpDt As New DataTable
    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Dim StaticsDt As New DataTable
    WithEvents G As New MyGrid
    Public Flag As Integer
    Public Receive As Boolean = False
    Public FirstColumn As String = "الكـــــود", SecondColumn As String = "الاســــــــــــم", ThirdColumn As String = "السعــــر", Statement As String = ""
    Dim Gp As String = "المجموعات", Tp As String = "الأنواع", It As String = "الأصناف"

    Public Structure FlagState
        'Don't forget to edit RPTs and Stored Procedures after Editing this structure
        Shared أرصدة_افتتاحية As Integer = 1
        Shared إضافة As Integer = 2
        Shared تسوية_إضافة As Integer = 3
        Shared صرف As Integer = 4
        Shared تسوية_صرف As Integer = 5
        Shared هدايا As Integer = 6
        Shared هالك As Integer = 7
        Shared تحويل_إلى_مخزن As Integer = 8
        Shared مشتريات As Integer = 9
        Shared مردودات_مشتريات As Integer = 10
        Shared مبيعات_الصالة As Integer = 11
        Shared مردودات_مبيعات_الصالة As Integer = 12
        Shared المبيعات As Integer = 13 'مبيعات_التيك_أواى
        Shared مردودات_المبيعات As Integer = 14 'مردودات_مبيعات_التيك_أواى
        Shared مبيعات_التوصيل As Integer = 15
        Shared مردودات_مبيعات_التوصيل As Integer = 16
        Shared المستهلكات As Integer = 17
        Shared مردودات_المستهلكات As Integer = 18
        Shared الاستيراد As Integer = 19
        Shared مردودات_الاستيراد As Integer = 20
        Shared مبيعات_الجملة As Integer = 21
        Shared مردودات_مبيعات_الجملة As Integer = 22
        Shared مبيعات_نصف_الجملة As Integer = 23
        Shared مردودات_مبيعات_نصف_الجملة As Integer = 24
        Shared عينات As Integer = 25
        Shared عرض_أسعار As Integer = 26
        Shared أمر_شراء As Integer = 27

    End Structure

    Function MainFlag() As String
        Select Case Flag
            Case FlagState.مردودات_الاستيراد
                Return FlagState.الاستيراد
            Case FlagState.مردودات_المبيعات
                Return FlagState.المبيعات
            Case FlagState.مردودات_المستهلكات
                Return FlagState.المستهلكات
            Case FlagState.مردودات_مبيعات_التوصيل
                Return FlagState.مبيعات_التوصيل
            Case FlagState.مردودات_مبيعات_الجملة
                Return FlagState.مبيعات_الجملة
            Case FlagState.مردودات_مبيعات_الصالة
                Return FlagState.مبيعات_الصالة
            Case FlagState.مردودات_مبيعات_نصف_الجملة
                Return FlagState.مبيعات_نصف_الجملة
            Case FlagState.مردودات_مشتريات
                Return FlagState.مشتريات
            Case Else
                Return 0
        End Select
    End Function



    Sub NewId()
        InvoiceNo.Clear()
        InvoiceNo.IsEnabled = False
    End Sub

    Sub UndoNewId()
        InvoiceNo.IsEnabled = True
    End Sub

    Private Sub Sales_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.FillCombo("select Id,Name from Currencies order by Id", CurrencyId)
        LoadResource()
        TabItem1.Height = 0
        TabItemDelivery.Height = 0
        TabItemTables.Height = 0

        Hide()
        bm.FillCombo("Shifts", Shift, "")
        bm.FillCombo("PaymentMethods", PaymentMethodId, "")
        bm.FillCombo("ShippingMethods", ShippingMethodId, "")

        Shift.SelectedValue = Md.CurrentShiftId
        DayDate.SelectedDate = Nothing
        DayDate.SelectedDate = bm.MyGetDate() 'Md.CurrentDate
        If Md.ShowShifts Then
            DayDate.SelectedDate = Md.CurrentDate
            Shift.SelectedValue = Md.CurrentShiftId
        Else
            lblShift.Visibility = Visibility.Hidden
            Shift.Visibility = Visibility.Hidden
        End If

        LoadWFH()
        LoadCbo()
        lblSaveId.Visibility = Windows.Visibility.Hidden
        SaveId.Visibility = Windows.Visibility.Hidden
        SaveName.Visibility = Windows.Visibility.Hidden
        SaveId.IsEnabled = Md.Manager

        StaticsDt = bm.ExcuteAdapter("select top 1 S_AccNo,R_S_AccNo,P_AccNo,R_P_AccNo,S_AccNo1,S_Per1,S_AccType1,S_AccNo2,S_Per2,S_AccType2,S_AccNo3,S_Per3,S_AccType3,S_AccNo4,S_Per4,S_AccType4,P_AccNo1,P_Per1,P_AccType1,P_AccNo2,P_Per2,P_AccType2,P_AccNo3,P_Per3,P_AccType3,P_AccNo4,P_Per4,P_AccType4 from Statics")

        bm.FillCombo("AccTypes", AccType1, "")
        bm.FillCombo("AccTypes", AccType2, "")
        bm.FillCombo("AccTypes", AccType3, "")
        bm.FillCombo("AccTypes", AccType4, "")

        RdoGrouping_Checked(Nothing, Nothing)

        TabItem1.Header = "" ' TryCast(TryCast(Me.Parent, TabItem).Header, TabsHeader).MyTabHeader

        LoadVisibility()

        bm.Fields = New String() {"Flag", MainId, SubId, "DayDate", "Shift", "ToId", "ReservToId", "WaiterId", "TableId", "TableSubId", "NoOfPersons", "WithTax", "Taxvalue", "WithService", "ServiceValue", "CancelMinPerPerson", "MinPerPerson", "PaymentType", "CashValue", "DiscountPerc", "DiscountValue", "Notes", "IsClosed", "IsCashierPrinted", "Cashier", "DeliverymanId", "Total", "TotalAfterDiscount", "DocNo", "AccNo1", "AccNo2", "AccNo3", "AccNo4", "AccType1", "AccType2", "AccType3", "AccType4", "Per1", "Per2", "Per3", "Per4", "Val1", "Val2", "Val3", "Val4", "SaveId", "Temp", "OrderTypeId", "AccNo", "CurrencyId", "Shipping", "Freight", "CustomClearance", "PaymentMethodId", "ShippingMethodId", "ContractTerms", "DeliveryDate", "VersionNo"}
        bm.control = New Control() {txtFlag, StoreId, InvoiceNo, DayDate, Shift, ToId, ReservToId, WaiterId, TableId, TableSubId, NoOfPersons, WithTax, Taxvalue, WithService, ServiceValue, CancelMinPerPerson, MinPerPerson, PaymentType, CashValue, DiscountPerc, DiscountValue, Notes, IsClosed, IsCashierPrinted, CashierId, DeliverymanId, Total, TotalAfterDiscount, DocNo, AccNo1, AccNo2, AccNo3, AccNo4, AccType1, AccType2, AccType3, AccType4, Per1, Per2, Per3, Per4, Val1, Val2, Val3, Val4, SaveId, Temp, OrderTypeId, AccNo, CurrencyId, Shipping, Freight, CustomClearance, PaymentMethodId, ShippingMethodId, ContractTerms, DeliveryDate, VersionNo}
        bm.KeyFields = New String() {"Flag", MainId, SubId}

        bm.Table_Name = TableName

        LoadGroups()
        LoadAllItems()
        RdoCash_Checked(Nothing, Nothing)
        txtFlag.Text = Flag
        If Receive Then
            ToId.Text = Md.DefaultStore
            ToId_LostFocus(Nothing, Nothing)
        Else
            StoreId.Text = Md.DefaultStore
            StoreId_LostFocus(Nothing, Nothing)
        End If

        ComboBox1.SelectedValue = Flag

        RdoFuture.IsChecked = True
        RdoCash.IsChecked = True
        btnNew_Click(Nothing, Nothing)
    End Sub


    Structure GC
        Shared SalesInvoiceNo As String = "SalesInvoiceNo"
        Shared Barcode As String = "Barcode"
        Shared Id As String = "Id"
        Shared Name As String = "Name"
        Shared Color As String = "Color"
        Shared Size As String = "Size"
        Shared UnitId As String = "UnitId"
        Shared UnitQty As String = "UnitQty"
        Shared CurrentBal As String = "CurrentBal"
        Shared Qty As String = "Qty"
        Shared Qty2 As String = "Qty2"
        Shared Qty3 As String = "Qty3"
        Shared ReceivedQty As String = "ReceivedQty"
        Shared Price As String = "Price"
        Shared UnitSub As String = "UnitSub"
        Shared QtySub As String = "QtySub"
        Shared PriceSub As String = "PriceSub"
        Shared ItemDiscountPerc As String = "ItemDiscountPerc"
        Shared ItemDiscount As String = "ItemDiscount"
        Shared Value As String = "Value"
        Shared IsPrinted As String = "IsPrinted"
        Shared SalesPrice As String = "SalesPrice"
        Shared FlagType As String = "FlagType"
        Shared SerialNo As String = "SerialNo"
    End Structure


    Private Sub LoadWFH()
        'WFH.Background = New SolidColorBrush(Colors.LightSalmon)
        'WFH.Foreground = New SolidColorBrush(Colors.Red)
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue

        G.Columns.Add(GC.SalesInvoiceNo, "رقم الفاتورة")
        G.Columns.Add(GC.Barcode, "الباركود")
        G.Columns.Add(GC.Id, "كود الصنف")
        G.Columns.Add(GC.Name, "اسم الصنف")

        Dim GCColor As New Forms.DataGridViewComboBoxColumn
        GCColor.HeaderText = "اللون"
        GCColor.Name = GC.Color
        bm.FillCombo("select 0 Id,'' Name", GCColor)
        G.Columns.Add(GCColor)

        Dim GCSize As New Forms.DataGridViewComboBoxColumn
        GCSize.HeaderText = "المقاس"
        GCSize.Name = GC.Size
        bm.FillCombo("select 0 Id,'' Name", GCSize)
        G.Columns.Add(GCSize)

        Dim GCUnitId As New Forms.DataGridViewComboBoxColumn
        GCUnitId.HeaderText = "الوحدة"
        GCUnitId.Name = GC.UnitId
        bm.FillCombo("select 0 Id,'' Name", GCUnitId)
        G.Columns.Add(GCUnitId)

        G.Columns.Add(GC.UnitQty, "عدد الفرعى")

        G.Columns.Add(GC.CurrentBal, "الرصيد")
        G.Columns.Add(GC.Qty, "الكمية")
        G.Columns.Add(GC.Qty2, "العدد/عبوة")
        G.Columns.Add(GC.Qty3, "عدد العبوات")
        G.Columns.Add(GC.ReceivedQty, "الكمية المستلمة")
        G.Columns.Add(GC.Price, "السعر")
        G.Columns.Add(GC.UnitSub, "الوحدة (فرعى)")
        G.Columns.Add(GC.QtySub, "الكمية (فرعى)")
        G.Columns.Add(GC.PriceSub, "السعر (فرعى)")
        G.Columns.Add(GC.ItemDiscountPerc, "نسبة الخصم")
        G.Columns.Add(GC.ItemDiscount, "قيمة الخصم")
        G.Columns.Add(GC.Value, "القيمة")
        G.Columns.Add(GC.IsPrinted, "طباعة للمطبخ")
        G.Columns.Add(GC.SalesPrice, "سعر البيع")

        Dim GCFlagType As New Forms.DataGridViewComboBoxColumn
        GCFlagType.HeaderText = "النوع"
        GCFlagType.Name = GC.FlagType
        If Flag = FlagState.المبيعات Then
            bm.FillCombo("select 0 Id,'-' Name union all select 13 Id,'المبيعات' Name union all select 6 Id,'هدايا' Name union all select 25 Id,'عينات' Name", GCFlagType)
        Else
            bm.FillCombo("select 0 Id,'-' Name union all select 6 Id,'هدايا' Name union all select 25 Id,'عينات' Name", GCFlagType)
        End If
        G.Columns.Add(GCFlagType)

        G.Columns.Add(GC.SerialNo, "رقم الإذن")

        G.Columns(GC.Barcode).FillWeight = 150
        G.Columns(GC.Id).FillWeight = 110
        G.Columns(GC.Name).FillWeight = 280

        G.Columns(GC.Name).ReadOnly = True
        G.Columns(GC.UnitQty).ReadOnly = True
        G.Columns(GC.Qty3).ReadOnly = True
        G.Columns(GC.Price).ReadOnly = ReadOnlyState()
        G.Columns(GC.UnitSub).ReadOnly = True
        G.Columns(GC.PriceSub).ReadOnly = ReadOnlyState()
        G.Columns(GC.ItemDiscountPerc).ReadOnly = ReadOnlyState()
        G.Columns(GC.ItemDiscount).ReadOnly = ReadOnlyState()
        G.Columns(GC.Value).ReadOnly = True
        'G.Columns(GC.SerialNo).ReadOnly = True
        G.Columns(GC.CurrentBal).ReadOnly = True

        G.Columns(GC.CurrentBal).Visible = False
        G.Columns(GC.UnitQty).Visible = False
        G.Columns(GC.UnitSub).Visible = False
        G.Columns(GC.QtySub).Visible = False
        G.Columns(GC.PriceSub).Visible = False
        G.Columns(GC.IsPrinted).Visible = False

        G.Columns(GC.ReceivedQty).Visible = False
        If Receive Then
            G.AllowUserToAddRows = False
            G.AllowUserToDeleteRows = False
            G.Columns(GC.ReceivedQty).Visible = True
            For i As Integer = 0 To G.Columns.Count - 1
                G.Columns(i).ReadOnly = True
            Next
            G.Columns(GC.ReceivedQty).ReadOnly = False
        End If

        If Receive OrElse Flag = FlagState.تحويل_إلى_مخزن OrElse TestImportAndReturn() Then
            G.Columns(GC.Qty2).Visible = False
            G.Columns(GC.Qty3).Visible = False
        End If

        G.Columns(GC.UnitId).Visible = Md.ShowQtySub
        G.Columns(GC.Color).Visible = Md.ShowColorAndSize
        G.Columns(GC.Size).Visible = Md.ShowColorAndSize

        G.Columns(GC.FlagType).Visible = False
        G.Columns(GC.SerialNo).Visible = False
        G.Columns(GC.ItemDiscountPerc).Visible = False
        G.Columns(GC.ItemDiscount).Visible = False


        Select Case Flag
            Case FlagState.مردودات_الاستيراد, FlagState.مردودات_المبيعات, FlagState.مردودات_المستهلكات, FlagState.مردودات_مبيعات_التوصيل, FlagState.مردودات_مبيعات_الجملة, FlagState.مردودات_مبيعات_الصالة, FlagState.مردودات_مبيعات_نصف_الجملة, FlagState.مردودات_مشتريات
            Case Else
                G.Columns(GC.SalesInvoiceNo).Visible = False
        End Select


        G.Columns(GC.Price).ReadOnly = True


        G.BarcodeIndex = G.Columns(GC.Barcode).Index
        If Not Md.ShowBarcode Then
            G.Columns(GC.Barcode).Visible = False
            btnPrint.Visibility = Windows.Visibility.Hidden
        End If


            G.Columns(GC.Qty2).Visible = False
        G.Columns(GC.Qty3).Visible = False



        If Flag = FlagState.تحويل_إلى_مخزن Then
            G.Columns(GC.Price).Visible = False
            G.Columns(GC.Value).Visible = False
        End If

        AddHandler G.CellEndEdit, AddressOf GridCalcRow
        AddHandler G.KeyDown, AddressOf GridKeyDown
        AddHandler G.SelectionChanged, AddressOf G_SelectionChanged
    End Sub

    Function ReadOnlyState() As Boolean
       If Md.Manager OrElse Md.EditPrices Then
            Return False
       ElseIf TestSalesAndReturn() And Not Md.AllowCashierToEditPrice Then
            Return True
        Else
            Return False
        End If
    End Function

    Function Fm() As Integer
        Select Case Flag
            Case FlagState.مبيعات_الصالة, FlagState.مردودات_مبيعات_الصالة
                Return 1
            Case FlagState.المبيعات, FlagState.مردودات_المبيعات
                Return 2
            Case FlagState.مبيعات_التوصيل, FlagState.مردودات_مبيعات_التوصيل
                Return 3
            Case FlagState.المستهلكات, FlagState.مردودات_المستهلكات
                Return 4
            Case Else
                Return 0
        End Select
    End Function

    Sub LoadGroups()
        Try
            WGroups.Children.Clear()
            WTypes.Children.Clear()
            WItems.Children.Clear()
            TabGroups.Header = Gp
            TabTypes.Header = Tp
            TabItems.Header = It

            Dim dt As DataTable = bm.ExcuteAdapter("LoadGroups2", New String() {"Form"}, New String() {Fm()})
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim x As New Button
                SetStyle(x)
                'bm.SetImage(x, CType(dt.Rows(i)("Image"), Byte()))
                x.Name = "TabItem_" & dt.Rows(i)("Id").ToString
                x.Tag = dt.Rows(i)("Id").ToString
                x.Content = dt.Rows(i)("Name").ToString
                x.ToolTip = dt.Rows(i)("Name").ToString
                WGroups.Children.Add(x)
                AddHandler x.Click, AddressOf LoadTypes
            Next
        Catch
        End Try
    End Sub

    Sub LoadTables()
        Try
            WTables.Children.Clear()
            WSubTables.Children.Clear()
            Dim dt As DataTable = bm.ExcuteAdapter("LoadTables", New String() {"StoreId"}, New String() {StoreId.Text})
            Dim dtInv As DataTable = bm.ExcuteAdapter("select InvoiceNo,TableId,TableSubId,dbo.ToStrTime(OpennedDate) OpennedTime,NoOfPersons,IsCashierPrinted from SalesMaster where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and IsClosed=0")
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim x As New Button
                x.Name = "Table_" & dt.Rows(i)("Id").ToString
                x.Tag = dt.Rows(i)("Id").ToString
                x.Width = 100
                x.Height = 100
                x.Cursor = Input.Cursors.Pen
                x.Content = dt.Rows(i)("Name").ToString
                x.ToolTip = x.Content
                WTables.Children.Add(x)

                If dtInv.Select("TableId=" & x.Tag & " and TableSubId>1").Length > 0 Then
                    x.Background = System.Windows.Media.Brushes.MediumSpringGreen
                    x.Content &= vbCrLf & "مائدة مقسمة"
                ElseIf dtInv.Select("TableId=" & x.Tag).Length > 0 Then
                    If dtInv.Select("TableId=" & x.Tag)(0)("IsCashierPrinted") = 1 Then
                        x.Background = System.Windows.Media.Brushes.Magenta
                    Else
                        x.Background = System.Windows.Media.Brushes.Red
                    End If
                    x.Content &= vbCrLf & dtInv.Select("TableId=" & x.Tag)(0).Item("OpennedTime").ToString & vbCrLf & "العدد: " & dtInv.Select("TableId=" & x.Tag)(0).Item("NoOfPersons").ToString
                Else
                    x.Background = System.Windows.Media.Brushes.LimeGreen
                End If

                AddHandler x.Click, AddressOf btnTableClick
            Next
        Catch
        End Try
    End Sub

    Sub LoadUnPaiedInvoices()
        Try
            WDelivery.Children.Clear()
            Dim dt As DataTable = bm.ExcuteAdapter("select InvoiceNo,dbo.ToStrTime(OpennedDate) OpennedTime from SalesMaster where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and IsClosed=0")
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim x As New Button
                x.Name = "Delivery_" & dt.Rows(i)("InvoiceNo").ToString
                x.Tag = dt.Rows(i)("InvoiceNo").ToString
                x.Width = 100
                x.Height = 100
                x.Cursor = Input.Cursors.Pen
                x.Content = dt.Rows(i)("InvoiceNo").ToString & vbCrLf & vbCrLf & dt.Rows(i)("OpennedTime").ToString
                x.ToolTip = x.Content
                WDelivery.Children.Add(x)
                x.Background = System.Windows.Media.Brushes.Red
                AddHandler x.Click, AddressOf btnDeliveryClick
            Next
        Catch
        End Try
    End Sub

    Private Sub LoadTypes(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim xx As Button = sender
            WTypes.Tag = xx.Tag
            WTypes.Children.Clear()
            WItems.Children.Clear()

            TabTypes.Header = Tp & " - " & xx.Content.ToString
            TabItems.Header = It

            Dim dt As DataTable = bm.ExcuteAdapter("LoadTypes2", New String() {"GroupId", "Form"}, New String() {xx.Tag.ToString, Fm()})
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim x As New Button
                SetStyle(x)
                'bm.SetImage(x, CType(dt.Rows(i)("Image"), Byte()))
                x.Name = "TabItem_" & xx.Tag.ToString & "_" & dt.Rows(i)("Id").ToString
                x.Tag = dt.Rows(i)("Id").ToString
                x.Content = dt.Rows(i)("Name").ToString
                x.ToolTip = dt.Rows(i)("Name").ToString
                WTypes.Children.Add(x)
                AddHandler x.Click, AddressOf LoadItems
            Next
        Catch
        End Try
    End Sub


    Sub LoadAllItems()
        Try
            HelpDt = bm.ExcuteAdapter("Select cast(Id as nvarchar(100))Id,Name," & PriceFieldName(GC.Price, 0) & " Price From Items  where IsStopped=0 " & ItemWhere())
            HelpDt.TableName = "tbl"
            HelpDt.Columns(0).ColumnName = FirstColumn
            HelpDt.Columns(1).ColumnName = SecondColumn
            HelpDt.Columns(2).ColumnName = ThirdColumn

            dv.Table = HelpDt
            HelpGD.ItemsSource = dv
            HelpGD.Columns(0).Width = 75
            HelpGD.Columns(1).Width = 220
            HelpGD.Columns(2).Width = 75

            HelpGD.SelectedIndex = 0
        Catch
        End Try

    End Sub

    Private Sub txtId_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.GotFocus
        Try
            dv.Sort = FirstColumn
        Catch
        End Try
    End Sub

    Private Sub txtName_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtName.GotFocus
        Try
            dv.Sort = SecondColumn
        Catch
        End Try
    End Sub

    Private Sub txtPrice_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPrice.GotFocus
        Try
            dv.Sort = ThirdColumn
        Catch
        End Try
    End Sub

    Private Sub txtId_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.TextChanged, txtName.TextChanged, txtPrice.TextChanged
        Try
            dv.RowFilter = " [" & FirstColumn & "] like '" & txtID.Text.Trim & "%' and [" & SecondColumn & "] like '%" & txtName.Text & "%' and [" & ThirdColumn & "] >=" & IIf(txtPrice.Text.Trim = "", 0, txtPrice.Text) & ""
        Catch
        End Try
    End Sub


    Private Sub HelpGD_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.PreviewKeyDown, txtName.PreviewKeyDown, txtPrice.PreviewKeyDown
        Try
            If e.Key = Input.Key.Up Then
                HelpGD.SelectedIndex = HelpGD.SelectedIndex - 1
            ElseIf e.Key = Input.Key.Down Then
                HelpGD.SelectedIndex = HelpGD.SelectedIndex + 1
            End If
        Catch ex As Exception
        End Try
    End Sub


    Private Sub HelpGD_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles HelpGD.MouseDoubleClick
        Try
            AddItem(HelpGD.Items(HelpGD.SelectedIndex)(0))
        Catch ex As Exception
        End Try
    End Sub



    Function ItemWhere() As String
        Dim st As String = ""
        st = " and ItemType in(0,1,2,3) "

        If Flag = FlagState.المستهلكات OrElse Flag = FlagState.مردودات_المستهلكات Then
            st &= " and (Flag=1 or IsService=1) "
        End If

        If Not TestSalesAndReturn() AndAlso Not TestConsumablesAndReturn() Then
            st &= " and IsService=0 "
        End If
        Return st
    End Function
    Private Sub LoadItems(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim xx As Button = sender
            WItems.Tag = xx.Tag
            WItems.Children.Clear()

            TabItems.Header = It & " - " & xx.Content.ToString

            Dim dt As DataTable = bm.ExcuteAdapter("Select * From Items_View  where IsStopped=0 " & ItemWhere() & " and GroupId=" & WTypes.Tag.ToString & " and TypeId=" & xx.Tag.ToString & " order by Name")
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim x As New Button
                SetStyle(x)
                'bm.SetImage(x, CType(dt.Rows(i)("Image"), Byte()))
                x.Tag = dt.Rows(i)("Id").ToString
                x.Content = dt.Rows(i)("Name").ToString
                x.ToolTip = dt.Rows(i)("Name").ToString
                WItems.Children.Add(x)
                AddHandler x.Click, AddressOf TabItem
            Next
        Catch
        End Try
    End Sub

    Private Sub TabItem(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim x As Button = sender
        AddItem(x.Tag)
    End Sub

    Sub AddItem(ByVal Id As String, Optional ByVal i As Integer = -1, Optional ByVal Add As Decimal = 1)
        Try
            G.EndEdit()
            If Not TabControl1.SelectedIndex = 0 Then TabControl1.SelectedIndex = 0
            Dim Exists As Boolean = False
            Dim Move As Boolean = False
            If i = -1 Then Move = True

            G.AutoSizeColumnsMode = Forms.DataGridViewAutoSizeColumnsMode.Fill
            If i = -1 Then
                For x As Integer = 0 To G.Rows.Count - 1
                    If Not G.Rows(x).Cells(GC.Id).Value Is Nothing AndAlso G.Rows(x).Cells(GC.Id).Value.ToString = Id.ToString AndAlso Not G.Rows(x).ReadOnly AndAlso Not G.Rows(x).Cells(GC.IsPrinted).Value = 1 Then
                        bm.ShowMSG("تم تكرار هذا الصنف بالسطر رقم " + (x + 1).ToString)
                        Exit For
                        'i = x
                        'Exists = True
                        'GoTo Br
                    End If
                Next
                i = G.Rows.Add()
                G.CurrentCell = G.Rows(i).Cells(GC.Name)

Br:
            End If

            GetItemNameAndBal(i, Id)

            'G.Rows(i).Cells(GC.Unit).Value = dr(0)(GC.Unit)
            LoadItemUint(i)

            If Val(G.Rows(i).Cells(GC.Qty).Value) = 0 Then Add = 1
            G.Rows(i).Cells(GC.Qty).Value = Add + Val(G.Rows(i).Cells(GC.Qty).Value)

            If Val(G.Rows(i).Cells(GC.Qty2).Value) = 0 Then G.Rows(i).Cells(GC.Qty2).Value = 1
            If Val(G.Rows(i).Cells(GC.ReceivedQty).Value) = 0 Then G.Rows(i).Cells(GC.ReceivedQty).Value = 0

            LoadItemPrice(i)
            G.Rows(i).Cells(GC.UnitSub).Value = 0 'dr(0)(GC.UnitSub)
            G.Rows(i).Cells(GC.QtySub).Value = 0
            G.Rows(i).Cells(GC.PriceSub).Value = 0 'dr(0)(PriceFieldName(GC.PriceSub))
            If G.Rows(i).Cells(GC.IsPrinted).Value <> 1 Then G.Rows(i).Cells(GC.IsPrinted).Value = 0

            If G.Rows(i).Cells(GC.FlagType).Value Is Nothing Then G.Rows(i).Cells(GC.FlagType).Value = "0"



            CalcRow(i)
            If Move Then
                G.Focus()
                G.Rows(i).Selected = True
                G.FirstDisplayedScrollingRowIndex = i
                G.CurrentCell = G.Rows(i).Cells(GC.Qty)
                G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
                G.BeginEdit(True)
            End If
            If Exists Then
                G.Rows(i).Selected = True
                G.FirstDisplayedScrollingRowIndex = i
                G.CurrentCell = G.Rows(i).Cells(GC.Price)
                G.CurrentCell = G.Rows(i).Cells(GC.Qty)
                G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
                G.BeginEdit(True)
            End If
        Catch
            If i <> -1 Then
                ClearRow(i)
            End If
        End Try
    End Sub

    Dim lop As Boolean = False
    Sub CalcRow(ByVal i As Integer)
        Try
            If G.Rows(i).Cells(GC.Id).Value Is Nothing OrElse G.Rows(i).Cells(GC.Id).Value.ToString = "" Then
                ClearRow(i)
                CalcTotal()
                Return
            End If
            G.Rows(i).Cells(GC.Qty).Value = Val(G.Rows(i).Cells(GC.Qty).Value)

            If Not lop AndAlso (Flag = FlagState.المبيعات OrElse Flag = FlagState.تحويل_إلى_مخزن OrElse Flag = FlagState.عرض_أسعار) AndAlso Val(G.Rows(i).Cells(GC.Qty).Value) > Val(G.Rows(i).Cells(GC.CurrentBal).Value) Then
                    bm.ShowMSG("رصيد الصنف لا يسمح")
                    G.Rows(i).Cells(GC.Qty).Value = 0
            End If
            G.Rows(i).Cells(GC.Qty2).Value = Val(G.Rows(i).Cells(GC.Qty2).Value)
            G.Rows(i).Cells(GC.Qty3).Value = Math.Round(Val(G.Rows(i).Cells(GC.Qty).Value) / Val(G.Rows(i).Cells(GC.Qty2).Value), 2, MidpointRounding.AwayFromZero)
            G.Rows(i).Cells(GC.Price).Value = Val(G.Rows(i).Cells(GC.Price).Value)
            G.Rows(i).Cells(GC.QtySub).Value = Val(G.Rows(i).Cells(GC.QtySub).Value)
            G.Rows(i).Cells(GC.PriceSub).Value = Val(G.Rows(i).Cells(GC.PriceSub).Value)
            G.Rows(i).Cells(GC.SalesPrice).Value = Val(G.Rows(i).Cells(GC.SalesPrice).Value)

            'G.Rows(i).Cells(GC.Value).Value = Math.Round(Val(G.Rows(i).Cells(GC.Qty).Value) * Val(G.Rows(i).Cells(GC.Price).Value) + Val(G.Rows(i).Cells(GC.QtySub).Value) * Val(G.Rows(i).Cells(GC.PriceSub).Value), 2)
            G.Rows(i).Cells(GC.Value).Value = Val(G.Rows(i).Cells(GC.Qty).Value) * (Val(G.Rows(i).Cells(GC.Price).Value) - Val(G.Rows(i).Cells(GC.ItemDiscount).Value)) + Val(G.Rows(i).Cells(GC.QtySub).Value) * Val(G.Rows(i).Cells(GC.PriceSub).Value)
            ItemBal.Text = G.CurrentRow.Cells(GC.CurrentBal).Value
        Catch ex As Exception
            ClearRow(i)
        End Try
        CalcTotal()
    End Sub

    Sub ClearRow(ByVal i As Integer)
        'G.Rows(i).Cells(GC.SalesInvoiceNo).Value = Nothing
        G.Rows(i).Cells(GC.Barcode).Value = Nothing
        G.Rows(i).Cells(GC.Id).Value = Nothing
        G.Rows(i).Cells(GC.Name).Value = Nothing
        G.Rows(i).Cells(GC.Color).Value = Nothing
        G.Rows(i).Cells(GC.Size).Value = Nothing
        G.Rows(i).Cells(GC.UnitId).Value = Nothing
        G.Rows(i).Cells(GC.UnitQty).Value = Nothing
        G.Rows(i).Cells(GC.Qty).Value = Nothing
        G.Rows(i).Cells(GC.Qty2).Value = Nothing
        G.Rows(i).Cells(GC.Qty3).Value = Nothing
        G.Rows(i).Cells(GC.ReceivedQty).Value = Nothing
        G.Rows(i).Cells(GC.Price).Value = Nothing
        G.Rows(i).Cells(GC.UnitSub).Value = Nothing
        G.Rows(i).Cells(GC.QtySub).Value = Nothing
        G.Rows(i).Cells(GC.PriceSub).Value = Nothing
        G.Rows(i).Cells(GC.ItemDiscountPerc).Value = Nothing
        G.Rows(i).Cells(GC.ItemDiscount).Value = Nothing
        G.Rows(i).Cells(GC.Value).Value = Nothing
        G.Rows(i).Cells(GC.IsPrinted).Value = Nothing
        G.Rows(i).Cells(GC.SalesPrice).Value = Nothing
        G.Rows(i).Cells(GC.FlagType).Value = Nothing
        G.Rows(i).Cells(GC.SerialNo).Value = Nothing
        ItemBal.Clear()
    End Sub
    Private Sub RdoCash_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles RdoCash.Checked, RdoVisa.Checked, RdoCashVisa.Checked, RdoFuture.Checked, RdoCashFuture.Checked, RdoEmployees.Checked
        Try
            lblGroupBoxPaymentType.Content = "طريقة الدفع : " & CType(sender, RadioButton).Content
            PaymentType.Text = 0
            If RdoCash.IsChecked Then
                PaymentType.Text = 1
            ElseIf RdoVisa.IsChecked Then
                PaymentType.Text = 2
            ElseIf RdoCashVisa.IsChecked Then
                PaymentType.Text = 3
            ElseIf RdoFuture.IsChecked Then
                PaymentType.Text = 4
            ElseIf RdoCashFuture.IsChecked Then
                PaymentType.Text = 5
            ElseIf RdoEmployees.IsChecked Then
                PaymentType.Text = 6
            End If
        Catch ex As Exception
        End Try

        If GroupBoxPaymentType.Visibility = Windows.Visibility.Hidden Then Return

        Try
            If RdoCashVisa.IsChecked OrElse RdoCashFuture.IsChecked Then
                CashValue.Visibility = Windows.Visibility.Visible
                lblCashValue.Visibility = Windows.Visibility.Visible
            Else
                CashValue.Visibility = Windows.Visibility.Hidden
                lblCashValue.Visibility = Windows.Visibility.Hidden
                CashValue.Text = 0
            End If
        Catch ex As Exception
        End Try

        Try
            If Not RdoFuture.IsChecked AndAlso (TestPurchaseAndReturn() OrElse TestSalesAndReturn()) Then
                lblSaveId.Visibility = Windows.Visibility.Visible
                SaveId.Visibility = Windows.Visibility.Visible
                SaveName.Visibility = Windows.Visibility.Visible
            Else
                lblSaveId.Visibility = Windows.Visibility.Hidden
                SaveId.Visibility = Windows.Visibility.Hidden
                SaveName.Visibility = Windows.Visibility.Hidden
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GridCalcRow(ByVal sender As Object, ByVal e As Forms.DataGridViewCellEventArgs)
        Try
            If G.CurrentCell.ColumnIndex = G.Columns(GC.SalesInvoiceNo).Index Then
                If Val(G.Rows(G.CurrentCell.RowIndex).Cells(GC.SalesInvoiceNo).Value) > 0 Then
                    dt = bm.ExcuteAdapter("select ToId from SalesMaster where StoreId=" & StoreId.Text & " and Flag=" & MainFlag() & " and InvoiceNo=" & G.Rows(G.CurrentCell.RowIndex).Cells(GC.SalesInvoiceNo).Value & " and ToId=" & Val(ToId.Text))
                    If dt.Rows.Count = 0 Then
                        bm.ShowMSG("هذه الفاتورة لا تخص هذا " & lblToId.Content)
                        G.Rows(G.CurrentCell.RowIndex).Cells(GC.SalesInvoiceNo).Value = ""
                    End If
                End If
            ElseIf G.Columns(e.ColumnIndex).Name = GC.Barcode AndAlso Not G.Rows(e.RowIndex).Cells(GC.Barcode).Value Is Nothing Then
                Dim BC As String = G.Rows(e.RowIndex).Cells(GC.Barcode).Value.ToString
              If Not G.Rows(e.RowIndex).Cells(GC.Barcode).Value = Nothing Then
                    'G.Rows(e.RowIndex).Cells(GC.Id).Value = bm.ExecuteScalar("select Id from Items where IsStopped=0 and left(Barcode,12)='" & Val(BC) & "'")
                    G.Rows(e.RowIndex).Cells(GC.Id).Value = bm.ExecuteScalar("select Id from Items where IsStopped=0 and Barcode='" & BC & "'")
                    AddItem(G.Rows(e.RowIndex).Cells(GC.Id).Value, e.RowIndex, 0)
                    Exit Sub
                End If
            ElseIf G.Columns(e.ColumnIndex).Name = GC.Id Then
                AddItem(G.Rows(e.RowIndex).Cells(GC.Id).Value, e.RowIndex, 0)
                G_SelectionChanged(Nothing, Nothing)
            ElseIf G.Columns(e.ColumnIndex).Name = GC.UnitId OrElse G.Columns(e.ColumnIndex).Name = GC.Size Then
                LoadItemPrice(e.RowIndex)
            ElseIf G.Columns(e.ColumnIndex).Name = GC.ItemDiscountPerc AndAlso Val(G.Rows(e.RowIndex).Cells(GC.ItemDiscountPerc).Value) > 0 Then
                G.Rows(e.RowIndex).Cells(GC.ItemDiscount).Value = Val(G.Rows(e.RowIndex).Cells(GC.Price).Value) * Val(G.Rows(e.RowIndex).Cells(GC.ItemDiscountPerc).Value) / 100
            ElseIf G.Columns(e.ColumnIndex).Name = GC.SerialNo Then
                If G.Rows(e.RowIndex).Cells(GC.SerialNo).Value <> "" Then
                    G.Rows(e.RowIndex).Cells(GC.SerialNo).Value = Val(G.Rows(e.RowIndex).Cells(GC.SerialNo).Value)
                End If
                Dim x As Integer = 1
                Select Case Flag
                    Case FlagState.تحويل_إلى_مخزن
                        x = 2
                    Case FlagState.المبيعات, FlagState.صرف
                        x = 3
                    Case FlagState.مردودات_مشتريات
                        x = 4
                    Case FlagState.مردودات_المبيعات
                        x = 5
                End Select
                Dim ss As Integer = Val(bm.ExecuteScalar("select dbo.GetSalesNewSerial(" & StoreId.Text & "," & x & ")"))
                If Val(G.Rows(e.RowIndex).Cells(GC.SerialNo).Value) > 0 AndAlso Val(G.Rows(e.RowIndex).Cells(GC.SerialNo).Value) < ss Then
                    If Val(bm.ExecuteScalar("select dbo.GetSalesNewSerial2(" & StoreId.Text & "," & InvoiceNo.Text & "," & x & "," & Val(G.Rows(e.RowIndex).Cells(GC.SerialNo).Value) & ")")) > 0 Then
                        bm.ShowMSG("يجب ألا يقل رقم الإذن عن " & ss)
                        G.Rows(e.RowIndex).Cells(GC.SerialNo).Value = ss
                    End If
                End If
            End If
            G.EditMode = Forms.DataGridViewEditMode.EditOnEnter
            CalcRow(e.RowIndex)
        Catch ex As Exception
        End Try

    End Sub


    Private Sub StoreId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles StoreId.KeyUp
        Dim str As String = " where 1=1 "
        If Flag = FlagState.المستهلكات OrElse Flag = FlagState.مردودات_المستهلكات Then
            str &= " and Flag=1 "
        End If
        If bm.ShowHelp("Stores", StoreId, StoreName, e, "select cast(Id as varchar(100)) Id,Name from Fn_EmpStores(" & Md.UserName & ")" & str) Then
            StoreId_LostFocus(StoreId, Nothing)
        End If
    End Sub

    Dim StoreUnitId As Integer = 0
    Private Sub StoreId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles StoreId.LostFocus
        Dim str As String = ""
        If Flag = FlagState.المستهلكات OrElse Flag = FlagState.مردودات_المستهلكات Then
            str = " and Flag=1"
        End If
        bm.LostFocus(StoreId, StoreName, "select Name from Fn_EmpStores(" & Md.UserName & ") where Id=" & StoreId.Text.Trim() & str)

        If Md.ShowQtySub Then
            StoreUnitId = Val(bm.ExecuteScalar("select StoreUnitId from Stores where Id=" & StoreId.Text))
        End If
        ClearControls()
        If Md.ShowShiftForEveryStore Then
            dt = bm.ExcuteAdapter("select CurrentDate,CurrentShift from Fn_EmpStores(" & Md.UserName & ") where Id=" & StoreId.Text.Trim())
            If dt.Rows.Count > 0 Then
                DayDate.SelectedDate = dt.Rows(0)("CurrentDate")
                Shift.SelectedValue = dt.Rows(0)("CurrentShift")
            End If
        End If

    End Sub

    Private Sub SaveId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveId.LostFocus
        bm.LostFocus(SaveId, SaveName, "select Name from Fn_EmpPermissions(5," & Md.UserName & ") where Id=" & SaveId.Text.Trim(), True)
    End Sub

    Private Sub OrderTypeId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles OrderTypeId.LostFocus
        bm.LostFocus(OrderTypeId, OrderTypeName, "select Name from OrderTypes where Id=" & OrderTypeId.Text.Trim(), True)
        If TestImportAndReturn() Then bm.LostFocus(OrderTypeId, AccNo, "select AccNo1 from OrderTypes where Id=" & OrderTypeId.Text.Trim(), True)
    End Sub

    Private Sub SaveId_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveId.KeyUp
        If bm.ShowHelp("Saves", SaveId, SaveName, e, "select cast(Id as varchar(100)) Id,Name from Fn_EmpPermissions(5," & Md.UserName & ")") Then
            SaveId_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub OrderTypeId_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles OrderTypeId.KeyUp
        If bm.ShowHelp("OrderTypes", OrderTypeId, OrderTypeName, e, "select cast(Id as varchar(100)) Id,Name from OrderTypes") Then
            OrderTypeId_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub ToId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles ToId.KeyUp
        Dim Title, tbl As String
        If Flag = FlagState.تحويل_إلى_مخزن Then
            tbl = "Stores"
            Title = "المخازن"
            If bm.ShowHelp(Title, ToId, ToName, e, "select cast(Id as varchar(100)) Id,Name from " & tbl) Then
                ToId_LostFocus(sender, Nothing)
            End If
        ElseIf ((TestPurchaseAndReturn() OrElse TestImportAndReturn()) AndAlso Not ReservToId.IsChecked) OrElse (TestSalesAndReturn() AndAlso ReservToId.IsChecked) Then
            tbl = "Suppliers"
            Title = "الموردين"
            If bm.ShowHelp(Title, ToId, ToName, e, "select cast(Id as varchar(100)) Id,Name from " & tbl) Then
                ToId_LostFocus(sender, Nothing)
            End If
        ElseIf (TestSalesAndReturn() AndAlso Not ReservToId.IsChecked) OrElse ((TestPurchaseAndReturn() OrElse TestImportAndReturn()) AndAlso ReservToId.IsChecked) Then
            If bm.ShowHelpCustomers(ToId, ToName, e) Then
                ToId_LostFocus(sender, Nothing)
            End If
        End If
    End Sub

    Function TestSalesAndReturn() As Boolean
        Return (Flag = FlagState.مبيعات_الصالة OrElse Flag = FlagState.مردودات_مبيعات_الصالة _
                OrElse Flag = FlagState.المبيعات OrElse Flag = FlagState.مردودات_المبيعات _
                OrElse Flag = FlagState.مبيعات_التوصيل OrElse Flag = FlagState.مردودات_مبيعات_التوصيل _
                OrElse Flag = FlagState.مبيعات_الجملة OrElse Flag = FlagState.مردودات_مبيعات_الجملة _
                OrElse Flag = FlagState.مبيعات_نصف_الجملة OrElse Flag = FlagState.مردودات_مبيعات_نصف_الجملة _
                OrElse Flag = FlagState.عرض_أسعار)
    End Function

    Function TestSalesOnly() As Boolean
        Return (Flag = FlagState.مبيعات_الصالة _
                OrElse Flag = FlagState.المبيعات _
                OrElse Flag = FlagState.مبيعات_التوصيل _
                OrElse Flag = FlagState.مبيعات_الجملة _
                OrElse Flag = FlagState.مبيعات_نصف_الجملة _
                OrElse Flag = FlagState.عرض_أسعار)
    End Function

    Function TestPurchaseAndReturn() As Boolean
        Return (Flag = FlagState.مشتريات OrElse Flag = FlagState.مردودات_مشتريات OrElse Flag = FlagState.أمر_شراء)
    End Function

    Function TestImportAndReturn() As Boolean
        Return (Flag = FlagState.الاستيراد OrElse Flag = FlagState.مردودات_الاستيراد)
    End Function

    Function TestConsumablesAndReturn() As Boolean
        Return (Flag = FlagState.المستهلكات OrElse Flag = FlagState.مردودات_المستهلكات)
    End Function

    Function TestPurchaseOnly() As Boolean
        Return (Flag = FlagState.مشتريات OrElse Flag = FlagState.أمر_شراء)
    End Function

    Function TestImportOnly() As Boolean
        Return (Flag = FlagState.الاستيراد)
    End Function

    Function TestDelivary() As Boolean
        Return (Flag = FlagState.مبيعات_التوصيل OrElse Flag = FlagState.مردودات_مبيعات_التوصيل)
    End Function

    Private Sub ToId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ToId.LostFocus
        'If GroupBoxPaymentType.Visibility = Windows.Visibility.Hidden Then Return
        Dim tbl As String
        If Flag = FlagState.تحويل_إلى_مخزن Then
            tbl = "Stores"
        ElseIf ((TestPurchaseAndReturn() OrElse TestImportAndReturn()) AndAlso Not ReservToId.IsChecked) OrElse (TestSalesAndReturn() AndAlso ReservToId.IsChecked) Then
            tbl = "Suppliers"
        ElseIf (TestSalesAndReturn() AndAlso Not ReservToId.IsChecked) OrElse ((TestPurchaseAndReturn() OrElse TestImportAndReturn()) AndAlso ReservToId.IsChecked) Then
            tbl = "Customers"
        ElseIf Flag = FlagState.المستهلكات OrElse Flag = FlagState.مردودات_المستهلكات Then
            bm.LostFocus(ToId, ToName, "select " & Resources.Item("CboName") & " Name from Cases where Id=" & ToId.Text.Trim() & " and InOut=1")
            ToId.ToolTip = ""
            ToName.ToolTip = ""
            Dim dt As DataTable = bm.ExcuteAdapter("select HomePhone,Mobile from Cases where Id=" & ToId.Text.Trim() & " and InOut=1")
            If dt.Rows.Count > 0 Then
                ToId.ToolTip = Resources.Item("Id") & ": " & ToId.Text & vbCrLf & Resources.Item("Name") & ": " & ToName.Text & vbCrLf & Resources.Item("HomePhone") & ": " & dt.Rows(0)("HomePhone").ToString & vbCrLf & Resources.Item("Mobile") & ": " & dt.Rows(0)("Mobile").ToString
                ToName.ToolTip = ToId.ToolTip
            End If
            Return
        Else
            Return
        End If
        bm.LostFocus(ToId, ToName, "select Name from " & tbl & " where Id=" & ToId.Text.Trim())
        Dim s As String = ""
        If (TestSalesAndReturn() AndAlso Not ReservToId.IsChecked) OrElse ((TestPurchaseAndReturn()) AndAlso ReservToId.IsChecked) Then
            If bm.ExecuteScalar("select Type from Customers where Id=" & ToId.Text.Trim()) = "1" OrElse Flag = FlagState.عرض_أسعار OrElse Flag = FlagState.أمر_شراء Then
                RdoCash.IsChecked = True
            Else
                RdoFuture.IsChecked = True
            End If

            Dim dt As DataTable = bm.ExcuteAdapter("GetCustomerData", New String() {"Id"}, New String() {Val(ToId.Text)})
            If dt.Rows.Count > 0 Then
                If Not lop Then DiscountPerc.Text = Val(dt.Rows(0)("DescPerc").ToString)
                For i As Integer = 0 To dt.Columns.Count - 2
                    s &= dt.Rows(0)(i).ToString & IIf(i = dt.Columns.Count - 1, "", vbCrLf)
                Next
            End If
        End If
        If TestImportAndReturn() Then
            RdoCash.IsChecked = True
            RdoFuture.IsChecked = True
        End If
        If CurrencyId.Visibility = Windows.Visibility.Visible Then
            CurrencyId.SelectedIndex = 0
            Dim x As Integer = Val(bm.ExecuteScalar("select CurrencyId from " & tbl & " where Id=" & ToId.Text.Trim()))
            If x > 0 Then
                CurrencyId.SelectedValue = x
            End If
        End If
        ToId.ToolTip = s
        ToName.ToolTip = s
    End Sub

    Private Sub WaiterId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles WaiterId.KeyUp
        bm.ShowHelp("المندوبين", WaiterId, WaiterName, e, "select cast(Id as varchar(100)) Id," & Resources.Item("CboName") & " Name from Employees where Waiter=1 and Stopped=0")
    End Sub

    Private Sub WaiterId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles WaiterId.LostFocus
        bm.LostFocus(WaiterId, WaiterName, "select " & Resources.Item("CboName") & " Name from Employees where Waiter=1 and Id=" & WaiterId.Text.Trim() & " and Stopped=0")
    End Sub

    Private Sub DeliverymanId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles DeliverymanId.KeyUp
        bm.ShowHelp("الطيارين", DeliverymanId, DeliverymanName, e, "select cast(Id as varchar(100)) Id," & Resources.Item("CboName") & " Name from Employees where Deliveryman=1 and Stopped=0")
    End Sub

    Private Sub DeliverymanId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DeliverymanId.LostFocus
        bm.LostFocus(DeliverymanId, DeliverymanName, "select EnName Name from Employees where Deliveryman=1 and Id=" & DeliverymanId.Text.Trim() & " and Stopped=0")
    End Sub


    Private Sub CashierId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CashierId.KeyUp
        bm.ShowHelp("Employees", CashierId, CashierName, e, "select cast(Id as varchar(100)) Id," & Resources.Item("CboName") & " Name from Employees where Doctor=0 and Stopped=0")
    End Sub

    Private Sub CashierId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CashierId.LostFocus
        bm.LostFocus(CashierId, CashierName, "select " & Resources.Item("CboName") & " Name from Employees where Doctor=0 and Id=" & CashierId.Text.Trim() & " and Stopped=0 ")
    End Sub


    Sub FillControls()
        If lop Then Return
        lop = True
        UndoNewId()
        G.Rows.Clear()
        bm.FillControls()

        PaymentType_TextChanged(Nothing, Nothing)
        ToId_LostFocus(Nothing, Nothing)
        CashierId_LostFocus(Nothing, Nothing)
        WaiterId_LostFocus(Nothing, Nothing)
        DeliverymanId_LostFocus(Nothing, Nothing)
        OrderTypeId_LostFocus(Nothing, Nothing)
        SaveId_LostFocus(Nothing, Nothing)
        TId_LostFocus(TableId, Nothing)
        TId_LostFocus(TableSubId, Nothing)
        TId_LostFocus(NoOfPersons, Nothing)

        AccNo_LostFocus(Nothing, Nothing)
        AccNo1_LostFocus(Nothing, Nothing)
        AccNo2_LostFocus(Nothing, Nothing)
        AccNo3_LostFocus(Nothing, Nothing)
        AccNo4_LostFocus(Nothing, Nothing)

        bm.FillControls()

        If Flag = FlagState.الاستيراد Then
            MessageId.Text = bm.ExecuteScalar("Select top 1 Id from ImportMessagesDetails where OrderTypeId='" & OrderTypeId.Text & "' and InvoiceNo='" & InvoiceNo.Text & "'")
        End If

        Dim dt As DataTable = bm.ExcuteAdapter("select SD.* ,It.Name It_Name from SalesDetails SD left join Items It on(SD.ItemId=It.Id) where SD.StoreId=" & StoreId.Text & " and SD.InvoiceNo=" & InvoiceNo.Text & " and SD.Flag=" & Flag)


        If dt.Rows.Count > 0 Then G.Rows.Add(dt.Rows.Count)
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows(i).HeaderCell.Value = (i + 1).ToString
            G.Rows(i).Cells(GC.SalesInvoiceNo).Value = dt.Rows(i)("SalesInvoiceNo").ToString
            G.Rows(i).Cells(GC.Barcode).Value = dt.Rows(i)("Barcode").ToString
            G.Rows(i).Cells(GC.Id).Value = dt.Rows(i)("ItemId").ToString
            G.Rows(i).Cells(GC.Name).Value = dt.Rows(i)("It_Name").ToString
            GetItemNameAndBal(i, dt.Rows(i)("ItemId").ToString)
            LoadItemUint(i)
            G.Rows(i).Cells(GC.Color).Value = dt.Rows(i)("Color")
            G.Rows(i).Cells(GC.Size).Value = dt.Rows(i)("Size")
            G.Rows(i).Cells(GC.UnitId).Value = dt.Rows(i)("UnitId")
            G.Rows(i).Cells(GC.UnitQty).Value = dt.Rows(i)("UnitQty").ToString
            G.Rows(i).Cells(GC.Qty).Value = dt.Rows(i)("Qty").ToString

            G.Rows(i).Cells(GC.CurrentBal).Value += G.Rows(i).Cells(GC.Qty).Value

            G.Rows(i).Cells(GC.Qty2).Value = dt.Rows(i)("Qty2").ToString
            G.Rows(i).Cells(GC.Qty3).Value = dt.Rows(i)("Qty3").ToString
            G.Rows(i).Cells(GC.ReceivedQty).Value = dt.Rows(i)("ReceivedQty").ToString
            G.Rows(i).Cells(GC.Price).Value = dt.Rows(i)("Price").ToString
            G.Rows(i).Cells(GC.UnitSub).Value = "" 'dt.Rows(i)("UnitSub").ToString
            G.Rows(i).Cells(GC.QtySub).Value = 0 ' 'dt.Rows(i)("QtySub").ToString
            G.Rows(i).Cells(GC.PriceSub).Value = dt.Rows(i)("PriceSub").ToString
            G.Rows(i).Cells(GC.ItemDiscountPerc).Value = dt.Rows(i)("ItemDiscountPerc").ToString
            G.Rows(i).Cells(GC.ItemDiscount).Value = dt.Rows(i)("ItemDiscount").ToString
            G.Rows(i).Cells(GC.Value).Value = dt.Rows(i)("Value").ToString
            G.Rows(i).Cells(GC.IsPrinted).Value = dt.Rows(i)("IsPrinted").ToString
            G.Rows(i).Cells(GC.SalesPrice).Value = dt.Rows(i)("SalesPrice").ToString
            G.Rows(i).Cells(GC.FlagType).Value = dt.Rows(i)("FlagType").ToString
            G.Rows(i).Cells(GC.SerialNo).Value = dt.Rows(i)("SerialNo").ToString
            CalcRow(i)
            'If Not Md.Manager AndAlso TestSalesAndReturn() Then
            '    G.Rows(i).ReadOnly = True
            '    G.Rows(i).DefaultCellStyle.BackColor = System.Drawing.Color.PeachPuff
            '    btnDelete.IsEnabled = False
            'End If
        Next
        G.CurrentCell = G.Rows(G.Rows.Count - 1).Cells(GC.Id)
        CalcTotal()
        Notes.Focus()
        G.RefreshEdit()
        lop = False
        CalcTotalEnd()
    End Sub

    Private Sub btnLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLast.Click
        bm.FirstLast(New String() {"Flag", MainId, SubId}, "Max", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        bm.NextPrevious(New String() {"Flag", MainId, SubId}, New String() {Flag, StoreId.Text, InvoiceNo.Text}, "Next", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPrint.Click, btnPrint2.Click, btnPrint3.Click, btnPrint4.Click, btnPrintImage.Click
        DontClear = True
        btnSave_Click(sender, e)
        DontClear = False
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If StoreId.Text.Trim = "" Then Return
        If Not CType(sender, Button).IsEnabled Then Return


        Select Case Md.MyProjectType
            Case Else
                For i As Integer = 0 To G.Rows.Count - 1
                    If Val(G.Rows(i).Cells(GC.Id).Value) > 0 Then
                        Exit For
                    End If
                    If i = G.Rows.Count - 1 Then Return
                Next
        End Select

        If AccNo.Visibility = Windows.Visibility.Visible AndAlso Val(AccNo.Text) = 0 Then
            bm.ShowMSG("برجاء تحديد الحساب ")
            AccNo.Focus()
            Return
        End If
        If ToId.Visibility = Windows.Visibility.Visible AndAlso ToId.Text.Trim = "" AndAlso Not TestSalesAndReturn() Then
            bm.ShowMSG("برجاء تحديد " & lblToId.Content)
            ToId.Focus()
            Return
        End If
        If OrderTypeId.Visibility = Windows.Visibility.Visible AndAlso OrderTypeId.Text.Trim = "" AndAlso TestImportAndReturn() Then
            bm.ShowMSG("برجاء تحديد " & lblOrderTypeId.Content)
            OrderTypeId.Focus()
            Return
        End If
        If TableId.Visibility = Windows.Visibility.Visible AndAlso TableId.Text.Trim = "" Then
            bm.ShowMSG("برجاء تحديد رقم المائدة")
            TableId.Focus()
            Return
        End If
        If TableSubId.Visibility = Windows.Visibility.Visible AndAlso TableSubId.Text.Trim = "" Then
            bm.ShowMSG("برجاء تحديد رقم الفرعى من المائدة")
            TableSubId.Focus()
            Return
        End If
        If NoOfPersons.Visibility = Windows.Visibility.Visible AndAlso NoOfPersons.Text.Trim = "" Then
            bm.ShowMSG("برجاء تحديد عدد الأفراد")
            NoOfPersons.Focus()
            Return
        End If
        If CashierId.Visibility = Windows.Visibility.Visible AndAlso CashierId.Text.Trim = "" Then
            bm.ShowMSG("برجاء تحديد " & lblCashier.Content)
            CashierId.Focus()
            Return
        End If
        If WaiterId.Visibility = Windows.Visibility.Visible AndAlso WaiterId.Text.Trim = "" Then
            bm.ShowMSG("برجاء تحديد المندوب")
            WaiterId.Focus()
            Return
        End If
        If DeliverymanId.Visibility = Windows.Visibility.Visible AndAlso DeliverymanId.Text.Trim = "" Then
            bm.ShowMSG("برجاء تحديد الطيار")
            DeliverymanId.Focus()
            Return
        End If
        If Flag = FlagState.تحويل_إلى_مخزن AndAlso ToId.Text.Trim = StoreId.Text Then
            bm.ShowMSG("لا يمكن التحويل لنفس المخزن")
            ToId.Focus()
            Return
        End If


        If AccNo1.Visibility = Windows.Visibility.Visible AndAlso AccNo1.Text.Trim = "" AndAlso Val(Val1.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد الحساب")
            AccNo1.Focus()
            Return
        End If
        If AccNo2.Visibility = Windows.Visibility.Visible AndAlso AccNo2.Text.Trim = "" AndAlso Val(Val2.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد الحساب")
            AccNo2.Focus()
            Return
        End If
        If AccNo3.Visibility = Windows.Visibility.Visible AndAlso AccNo3.Text.Trim = "" AndAlso Val(Val3.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد الحساب")
            AccNo3.Focus()
            Return
        End If
        If AccNo4.Visibility = Windows.Visibility.Visible AndAlso AccNo4.Text.Trim = "" AndAlso Val(Val4.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد الحساب")
            AccNo4.Focus()
            Return
        End If

        If AccType1.Visibility = Windows.Visibility.Visible AndAlso AccType1.SelectedIndex < 1 AndAlso Val(Val1.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد النوع")
            AccType1.Focus()
            Return
        End If
        If AccType2.Visibility = Windows.Visibility.Visible AndAlso AccType2.SelectedIndex < 1 AndAlso Val(Val2.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد النوع")
            AccType2.Focus()
            Return
        End If
        If AccType3.Visibility = Windows.Visibility.Visible AndAlso AccType3.SelectedIndex < 1 AndAlso Val(Val3.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد النوع")
            AccType3.Focus()
            Return
        End If
        If AccType4.Visibility = Windows.Visibility.Visible AndAlso AccType4.SelectedIndex < 1 AndAlso Val(Val4.Text) <> 0 Then
            bm.ShowMSG("برجاء تحديد النوع")
            AccType4.Focus()
            Return
        End If

        G.EndEdit()
        Try
            CalcRow(G.CurrentRow.Index)
        Catch ex As Exception
        End Try

        TableId.Text = Val(TableId.Text)
        TableSubId.Text = Val(TableSubId.Text)
        NoOfPersons.Text = Val(NoOfPersons.Text)
        MinPerPerson.Text = Val(MinPerPerson.Text)
        ServiceValue.Text = Val(ServiceValue.Text)
        Taxvalue.Text = Val(Taxvalue.Text)
        PaymentType.Text = Val(PaymentType.Text)
        CashValue.Text = Val(CashValue.Text)

        DiscountPerc.Text = Val(DiscountPerc.Text)
        DiscountValue.Text = Val(DiscountValue.Text)

        ToId.Text = Val(ToId.Text)
        WaiterId.Text = Val(WaiterId.Text)

        Per1.Text = Val(Per1.Text)
        Per2.Text = Val(Per2.Text)
        Per3.Text = Val(Per3.Text)
        Per4.Text = Val(Per4.Text)

        Val1.Text = Val(Val1.Text)
        Val2.Text = Val(Val2.Text)
        Val3.Text = Val(Val3.Text)
        Val4.Text = Val(Val4.Text)

        Shipping.Text = Val(Shipping.Text)
        Freight.Text = Val(Freight.Text)
        CustomClearance.Text = Val(CustomClearance.Text)

        'If Not Md.Manager Then
        'DayDate.SelectedDate = bm.MyGetDate() 'Md.CurrentDate
        'Shift.SelectedValue = Md.CurrentShiftId
        'End If
        If Md.ShowShifts AndAlso Not Md.Manager Then
            DayDate.SelectedDate = Md.CurrentDate
            Shift.SelectedValue = Md.CurrentShiftId
        End If


        Dim State As BasicMethods.SaveState = BasicMethods.SaveState.Update
        If InvoiceNo.Text.Trim = "" Then
            InvoiceNo.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName & " where " & MainId & "='" & StoreId.Text & "'" & " and Flag=" & Flag)
            If InvoiceNo.Text = "" Then InvoiceNo.Text = "1"
            lblLastEntry.Text = InvoiceNo.Text
            'lblLastEntry.Foreground = System.Windows.Media.Brushes.Red
            'System.Threading.Thread.Sleep(500)
            'lblLastEntry.Foreground = System.Windows.Media.Brushes.Blue
            State = BasicMethods.SaveState.Insert
        End If

        MinPerPerson.Text = Val(MinPerPerson.Text)
        bm.DefineValues()
        If Not bm.Save(New String() {"Flag", MainId, SubId}, New String() {Flag, StoreId.Text, InvoiceNo.Text.Trim}) Then
            If State = BasicMethods.SaveState.Insert Then
                InvoiceNo.Text = ""
                lblLastEntry.Text = ""
            End If
            Return
        End If

        bm.SaveGrid(G, "SalesDetails", New String() {"Flag", "StoreId", "InvoiceNo"}, New String() {Flag, StoreId.Text, InvoiceNo.Text}, New String() {"SalesInvoiceNo", "Barcode", "ItemId", "ItemName", "Color", "Size", "UnitId", "UnitQty", "Qty", "Qty2", "Qty3", "ReceivedQty", "Price", "QtySub", "PriceSub", "ItemDiscountPerc", "ItemDiscount", "Value", "IsPrinted", "SalesPrice", "FlagType", "SerialNo"}, New String() {GC.SalesInvoiceNo, GC.Barcode, GC.Id, GC.Name, GC.Color, GC.Size, GC.UnitId, GC.UnitQty, GC.Qty, GC.Qty2, GC.Qty3, GC.ReceivedQty, GC.Price, GC.QtySub, GC.PriceSub, GC.ItemDiscountPerc, GC.ItemDiscount, GC.Value, GC.IsPrinted, GC.SalesPrice, GC.FlagType, GC.SerialNo}, New VariantType() {VariantType.Integer, VariantType.String, VariantType.Integer, VariantType.String, VariantType.Integer, VariantType.Integer, VariantType.Integer, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Decimal, VariantType.Integer, VariantType.Decimal, VariantType.Integer, VariantType.String}, New String() {GC.Id})

        If State = BasicMethods.SaveState.Insert AndAlso TestPurchaseOnly() Then
            bm.ExcuteNonQuery("UpdateItemPurchasePrice", New String() {"Flag", "StoreId", "InvoiceNo"}, New String() {Flag, StoreId.Text, InvoiceNo.Text})
        ElseIf State = BasicMethods.SaveState.Insert AndAlso TestImportOnly Then
            bm.ExcuteNonQuery("UpdateItemImportPrice", New String() {"Flag", "StoreId", "InvoiceNo"}, New String() {Flag, StoreId.Text, InvoiceNo.Text})
        End If

        'If DocNo.Text.Trim = "" AndAlso State = BasicMethods.SaveState.Insert AndAlso Flag = FlagState.المستهلكات Then
        '    bm.ExcuteNonQuery("UpdateDocNo", New String() {"Flag", "StoreId", "InvoiceNo"}, New String() {Flag, StoreId.Text, InvoiceNo.Text})
        'End If

        bm.ExcuteNonQuery("UpdateSalesDetailsComponants", New String() {"Flag", "StoreId", "InvoiceNo"}, New String() {Flag, StoreId.Text, InvoiceNo.Text})

        Select Case CType(sender, Button).Name
            Case btnPrint.Name, btnPrint2.Name, btnPrint3.Name, btnPrint4.Name, btnPrint5.Name
                State = BasicMethods.SaveState.Print
            Case btnCloseTable.Name
                State = BasicMethods.SaveState.Close
        End Select

        TraceInvoice(State.ToString)

        'If TestSalesAndOnly() Then PrintPone(sender, 1)
        If sender Is btnPrint OrElse sender Is btnPrint2 OrElse sender Is btnPrint3 OrElse sender Is btnPrint4 OrElse sender Is btnPrint5 OrElse sender Is btnPrintImage OrElse (sender Is btnCloseTable And btnPrint.IsEnabled) Then
            PrintPone(sender, 0)
            'txtID_Leave(Nothing, Nothing)
            'AllowClose = True
            'Return
        End If

        If Not DontClear Then btnNew_Click(sender, e)
        AllowClose = True
    End Sub

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click
        bm.FirstLast(New String() {"Flag", MainId, SubId}, "Min", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        ClearControls()
        TableId.Focus()
    End Sub

    Dim SalesSerialNoCount As Integer = 0
    Sub ClearControls()
        Try
            If looop Then Return
            NewId()
            Dim d As DateTime = Nothing
            Try
                If d.Year = 1 Then d = bm.MyGetDate
                d = DayDate.SelectedDate
            Catch ex As Exception
            End Try

            Dim s As String = 0
            Try
                s = Shift.SelectedValue.ToString()
            Catch ex As Exception
            End Try
            Dim st As String = StoreId.Text

            bm.ClearControls(False)

            SalesSerialNoCount = Val(bm.ExecuteScalar("Select top 1 SalesSerialNoCount from Statics"))
            Payed.Clear()
            Remaining.Clear()
            txtFlag.Text = Flag
            SaveId.Text = Md.DefaultSave
            CashierId.Text = Md.UserName

            Dim dt As DataTable = bm.ExcuteAdapter("select S_AccNo,R_S_AccNo,P_AccNo,R_P_AccNo from Statics")
            Select Case Flag
                Case FlagState.مشتريات
                    AccNo.Text = dt.Rows(0)("P_AccNo")
                Case FlagState.مردودات_مشتريات
                    AccNo.Text = dt.Rows(0)("R_P_AccNo")
                Case FlagState.المبيعات, FlagState.مبيعات_التوصيل, FlagState.مبيعات_الصالة, FlagState.مبيعات_الجملة, FlagState.مبيعات_نصف_الجملة, FlagState.المستهلكات
                    AccNo.Text = dt.Rows(0)("S_AccNo")
                Case FlagState.مردودات_المبيعات, FlagState.مردودات_مبيعات_التوصيل, FlagState.مردودات_مبيعات_الصالة, FlagState.مردودات_مبيعات_الجملة, FlagState.مردودات_مبيعات_نصف_الجملة, FlagState.مردودات_المستهلكات
                    AccNo.Text = dt.Rows(0)("R_S_AccNo")
            End Select

            OrderTypeId_LostFocus(Nothing, Nothing)
            SaveId_LostFocus(Nothing, Nothing)
            CashierId_LostFocus(Nothing, Nothing)
            ToId_LostFocus(Nothing, Nothing)
            WaiterId_LostFocus(Nothing, Nothing)
            AccNo_LostFocus(Nothing, Nothing)
            DeliverymanId_LostFocus(Nothing, Nothing)
            TId_LostFocus(TableId, Nothing)
            TId_LostFocus(TableSubId, Nothing)
            TId_LostFocus(NoOfPersons, Nothing)

            If TestPurchaseAndReturn() Then

                AccNo1.Text = StaticsDt.Rows(0)("P_AccNo1")
                Per1.Text = StaticsDt.Rows(0)("P_Per1")
                AccType1.SelectedValue = StaticsDt.Rows(0)("P_AccType1")

                AccNo2.Text = StaticsDt.Rows(0)("P_AccNo2")
                Per2.Text = StaticsDt.Rows(0)("P_Per2")
                AccType2.SelectedValue = StaticsDt.Rows(0)("P_AccType2")

                AccNo3.Text = StaticsDt.Rows(0)("P_AccNo3")
                Per3.Text = StaticsDt.Rows(0)("P_Per3")
                AccType3.SelectedValue = StaticsDt.Rows(0)("P_AccType3")

                AccNo4.Text = StaticsDt.Rows(0)("P_AccNo4")
                Per4.Text = StaticsDt.Rows(0)("P_Per4")
                AccType4.SelectedValue = StaticsDt.Rows(0)("P_AccType4")

            ElseIf TestSalesAndReturn() Then

                AccNo1.Text = StaticsDt.Rows(0)("S_AccNo1")
                Per1.Text = StaticsDt.Rows(0)("S_Per1")
                AccType1.SelectedValue = StaticsDt.Rows(0)("S_AccType1")

                AccNo2.Text = StaticsDt.Rows(0)("S_AccNo2")
                Per2.Text = StaticsDt.Rows(0)("S_Per2")
                AccType2.SelectedValue = StaticsDt.Rows(0)("S_AccType2")

                AccNo3.Text = StaticsDt.Rows(0)("S_AccNo3")
                Per3.Text = StaticsDt.Rows(0)("S_Per3")
                AccType3.SelectedValue = StaticsDt.Rows(0)("S_AccType3")

                AccNo4.Text = StaticsDt.Rows(0)("S_AccNo4")
                Per4.Text = StaticsDt.Rows(0)("S_Per4")
                AccType4.SelectedValue = StaticsDt.Rows(0)("S_AccType4")

                If Not Md.Manager Then
                    AccNo1.IsEnabled = False
                    AccNo2.IsEnabled = False
                    AccNo3.IsEnabled = False
                    AccNo4.IsEnabled = False
                End If
            End If

            AccNo_LostFocus(Nothing, Nothing)

            AccNo1_LostFocus(Nothing, Nothing)
            AccNo2_LostFocus(Nothing, Nothing)
            AccNo3_LostFocus(Nothing, Nothing)
            AccNo4_LostFocus(Nothing, Nothing)


            Temp.Visibility = Windows.Visibility.Visible
            Temp.Content = "ملغى"


            PaymentType.Text = 1

            If Not Md.Manager Then
                DayDate.SelectedDate = bm.MyGetDate() 'Md.CurrentDate
                Shift.SelectedValue = Md.CurrentShiftId
                If Md.ShowShifts Then
                    DayDate.SelectedDate = Md.CurrentDate
                    Shift.SelectedValue = Md.CurrentShiftId
                End If

                CashierId.Text = Md.UserName
                CashierId_LostFocus(Nothing, Nothing)
            Else
                DayDate.SelectedDate = d
                Shift.SelectedValue = s
            End If

            'DayDate.SelectedDate = bm.MyGetDate()
            'Shift.SelectedValue = Md.CurrentShiftId

            StoreId.Text = st

            txtFlag.Text = Flag

            G.Rows.Clear()
            G_SelectionChanged(Nothing, Nothing)
            CalcTotal()
            'InvoiceNo.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName & " where " & MainId & "='" & StoreId.Text & "'" & " and Flag=" & Flag)
            'If InvoiceNo.Text = "" Then InvoiceNo.Text = "1"

            If TableSubId.Visibility = Visibility.Visible Then TableSubId.Text = 1
            If NoOfPersons.Visibility = Visibility.Visible Then NoOfPersons.Text = 1

            WithService.IsChecked = (WithService.Visibility = Visibility.Visible)
            WithTax.IsChecked = (WithTax.Visibility = Visibility.Visible)
        Catch
        End Try
        If Flag = FlagState.مبيعات_الصالة Then TabControl1.SelectedItem = TabItemTables

    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            TraceInvoice("Deleted")
            bm.ExcuteNonQuery("delete from " & TableName & " where " & SubId & "='" & InvoiceNo.Text.Trim & "' and " & MainId & " ='" & StoreId.Text & "'" & " and Flag=" & Flag)

            bm.ExcuteNonQuery("delete from " & TableDetailsName & " where " & SubId & "='" & InvoiceNo.Text.Trim & "' and " & MainId & " ='" & StoreId.Text & "'" & " and Flag=" & Flag)

            btnNew_Click(sender, e)
        End If
    End Sub

    Sub TraceInvoice(ByVal State As String)
        bm.ExcuteNonQuery("BeforeDeleteSales", New String() {"Flag", "StoreId", "InvoiceNo", "UserDelete", "State"}, New String() {Flag, StoreId.Text, InvoiceNo.Text, Md.UserName, State})
    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {"Flag", MainId, SubId}, New String() {Flag, StoreId.Text, InvoiceNo.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub
    Dim lv As Boolean = False

    Private Sub InvoiceNo_KeyUp(sender As Object, e As KeyEventArgs) Handles InvoiceNo.KeyUp
        If TestImportAndReturn() Then
            If bm.ShowHelpMultiColumns("Header", InvoiceNo, InvoiceNo, e, "select cast(InvoiceNo as nvarchar(100))'رقم الفاتورة',cast(ToId as nvarchar(100))'كود المورد',dbo.GetSupplierName(ToId)'اسم المورد',dbo.GetOrderTypes(OrderTypeId)'رقم الطلبية',DocNo'رقم عقد المورد',dbo.ToStrDate(DayDate)'التاريخ' from SalesMaster where StoreId=" & Val(StoreId.Text) & " and Flag=" & Flag.ToString) Then
                'InvoiceNo.Text = bm.SelectedRow(0)
                InvoiceNo_Leave(Nothing, Nothing)
            End If
        ElseIf TestPurchaseAndReturn() Then
            If bm.ShowHelpMultiColumns("Header", InvoiceNo, InvoiceNo, e, "select cast(InvoiceNo as nvarchar(100))'رقم الفاتورة',cast(ToId as nvarchar(100))'كود المورد',dbo.GetSupplierName(ToId)'رقم عقد المورد',dbo.ToStrDate(DayDate)'التاريخ' from SalesMaster where StoreId=" & Val(StoreId.Text) & " and Flag=" & Flag.ToString) Then
                'InvoiceNo.Text = bm.SelectedRow(0)
                InvoiceNo_Leave(Nothing, Nothing)
            End If
        ElseIf TestSalesAndReturn() Then
            If bm.ShowHelpMultiColumns(CType(Parent, Page).Title, InvoiceNo, InvoiceNo, e, "select cast(InvoiceNo as nvarchar(100))'رقم الفاتورة',cast(ToId as nvarchar(100))'كود العميل',dbo.GetCustomerName(ToId)'اسم العميل',dbo.ToStrDate(DayDate)'التاريخ' from SalesMaster where StoreId=" & Val(StoreId.Text) & " and Flag=" & Flag.ToString) Then
                'InvoiceNo.Text = bm.SelectedRow(0)
                InvoiceNo_Leave(Nothing, Nothing)
            End If
        End If

    End Sub
    Private Sub InvoiceNo_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InvoiceNo.LostFocus
        If lv Then
            Return
        End If
        lv = True

        bm.DefineValues()
        Dim dt As New DataTable
        bm.RetrieveAll(New String() {"Flag", MainId, SubId}, New String() {Flag, StoreId.Text, InvoiceNo.Text.Trim}, dt)
        If dt.Rows.Count = 0 Then
            ClearControls()
            lv = False
            Return
        End If
        FillControls()
        lv = False
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles StoreId.KeyDown, InvoiceNo.KeyDown, ToId.KeyDown, WaiterId.KeyDown, TableId.KeyDown, TableSubId.KeyDown, NoOfPersons.KeyDown, txtID.KeyDown, CashierId.KeyDown, DeliverymanId.KeyDown, AccNo1.KeyDown, AccNo2.KeyDown, AccNo3.KeyDown, AccNo4.KeyDown, OrderTypeId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub txtID_KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Taxvalue.KeyDown, ServiceValue.KeyDown, MinPerPerson.KeyDown, CashValue.KeyDown, DiscountPerc.KeyDown, DiscountValue.KeyDown, txtPrice.KeyDown, Per1.KeyDown, Per2.KeyDown, Per3.KeyDown, Per4.KeyDown, Val1.KeyDown, Val2.KeyDown, Val3.KeyDown, Val4.KeyDown, Shipping.KeyDown, Freight.KeyDown, CustomClearance.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub

    Dim AllowClose As Boolean = False
    'Private Sub MyBase_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
    '    If Not btnSave.Enabled Then Exit Sub
    '    Select Case bm.RequestDelete
    '        Case BasicMethods.CloseState.Yes
    '            AllowClose = False
    '            btnSave_Click(Nothing, Nothing)
    '            If Not AllowClose Then e.Cancel = True
    '        Case BasicMethods.CloseState.No

    '        Case BasicMethods.CloseState.Cancel
    '            e.Cancel = True
    '    End Select
    'End Sub

    Private Sub PaymentType_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles PaymentType.TextChanged
        Try
            If PaymentType.Text = 1 Then
                RdoCash.IsChecked = True
            ElseIf PaymentType.Text = 2 Then
                RdoVisa.IsChecked = True
            ElseIf PaymentType.Text = 3 Then
                RdoCashVisa.IsChecked = True
            ElseIf PaymentType.Text = 4 Then
                RdoFuture.IsChecked = True
            ElseIf PaymentType.Text = 5 Then
                RdoCashFuture.IsChecked = True
            ElseIf PaymentType.Text = 6 Then
                RdoEmployees.IsChecked = True
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TableId_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TableId.KeyUp
        If bm.ShowHelp("الموائد", TableId, TableIdName, e, "select cast(Id as varchar(100)) Id,Name from Tables where StoreId='" & StoreId.Text & "'") Then
            TId_LostFocus(TableId, Nothing)
        End If
    End Sub



    Private Sub TId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles TableId.LostFocus, TableSubId.LostFocus, NoOfPersons.LostFocus
        If CType(sender, TextBox).Text.Trim = "" Or CType(sender, TextBox).Text.Trim = "0" Then CType(sender, TextBox).Clear()

        If sender Is TableId Then
            bm.LostFocus(TableId, TableIdName, "select Name from Tables where StoreId='" & StoreId.Text & "' and Id=" & TableId.Text.Trim())
            TestDoublicatinInTables(False)
        ElseIf sender Is TableSubId Then
            Dim x As Integer = Val(bm.ExecuteScalar("select MaxSubTable from Statics"))
            If (x < Val(TableSubId.Text)) Then
                bm.ShowMSG("الحد الأقصى للفرعى هو " & x)
                TableSubId.Clear()
            End If
            TestDoublicatinInTables(True)
        End If
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

    Function PriceFieldName(ByVal str As String, i As Integer, Optional ForceSales As Boolean = False) As String
        If ForceSales OrElse TestSalesAndReturn() OrElse TestConsumablesAndReturn() Then
            str = "Sales" & str
        ElseIf TestImportAndReturn() Then
            str = "Import" & str
        Else
            str = "Purchase" & str
        End If

        Select Case i
            Case 1
                Return str & "Sub"
            Case 2
                Return str & "Sub2"
            Case Else
                Return str
        End Select
    End Function

    Function UnitCount(dt As DataTable, i As Integer) As String
        Select Case i
            Case 1
                Return dt.Rows(0)("UnitCount")
            Case 2
                Return dt.Rows(0)("UnitCount2")
            Case Else
                Return 1
        End Select
    End Function

    Private Sub PrintPone(ByVal sender As System.Object, ByVal NewItemsOnly As Integer)
        Dim rpt As New ReportViewer
        rpt.paraname = New String() {"@FromDate", "@ToDate", "@Shift", "@Flag", "@StoreId", "@FromInvoiceNo", "@ToInvoiceNo", "@InvoiceNo", "@NewItemsOnly", "@RPTFlag1", "@RPTFlag2", "@PrintingGroupId", "Header", "Remaining", "Payed"}

        If NewItemsOnly = 0 Then
            rpt.paravalue = New String() {DayDate.SelectedDate, DayDate.SelectedDate, Shift.SelectedValue.ToString, Flag, StoreId.Text, InvoiceNo.Text, InvoiceNo.Text, InvoiceNo.Text, NewItemsOnly, 0, 0, 0, CType(Parent, Page).Title, Val(Remaining.Text), Val(Payed.Text)}

            'rpt.Rpt = IIf(Md.MyProjectType = ProjectType.NawarGroup AndAlso TestImportAndReturn(), "SalesPone_N.rpt", "SalesPone.rpt")
            Select Case Md.MyProjectType
                Case Else
                    rpt.Rpt = "SalesPone.rpt"
            End Select

            'If sender Is btnPrint2 Then rpt.Rpt = "SalesPone2.rpt"
            If sender Is btnPrint3 Then rpt.Rpt = "SalesPone3.rpt"
            If sender Is btnPrint4 Then rpt.Rpt = "SalesPone4.rpt"

            If sender Is btnPrint2 Then
                Dim i As Integer = 1
                rpt.Rpt = "SalesPone2.rpt"
                 rpt.Print(".", Md.PonePrinter, i)
                Return
            End If

            If sender Is btnPrint5 Then
                rpt.Rpt = "PrintBarcodeFromSalesDetails.rpt"

                rpt.Print(".", Md.BarcodePrinter, 1)
            Return
        End If

            If sender Is btnPrintImage Then
                rpt.Rpt = "SalesPone_N_Image.rpt"
            End If

            'If Md.MyProjectType = ProjectType.NawarGroup Then
            rpt.Show()
            'Else

            '    If TestSalesOnly() OrElse TestPurchaseOnly() Then
            '        rpt.Print(, , 1)
            '    Else
            '        rpt.Print(, , 2)
            '    End If
            'End If
        ElseIf Not TestSalesAndReturn() Then
            rpt.Rpt = "SalesPoneKitchen.rpt"
            For i As Integer = 0 To G.Rows.Count - 1
                Try
                    If G.Rows(i).Cells(GC.IsPrinted).Value.ToString = 0 Then
                        Dim dt As DataTable = bm.ExcuteAdapter("GetPrinters", New String() {"Shift", "Flag", "StoreId", "InvoiceNo"}, New String() {Shift.SelectedValue.ToString, Flag, StoreId.Text, InvoiceNo.Text})
                        For x As Integer = 0 To dt.Rows.Count - 1
                            rpt.paravalue = New String() {DayDate.SelectedDate, DayDate.SelectedDate, Shift.SelectedValue.ToString, Flag, StoreId.Text, InvoiceNo.Text, InvoiceNo.Text, NewItemsOnly, 0, 0, dt.Rows(x)("PrintingGroupId")}


                            rpt.Print(dt.Rows(x)("ServerName"), dt.Rows(x)("PrinterName"))

                        Next
                        Exit For
                    End If
                Catch
                End Try
            Next
        End If

    End Sub


    Private Sub RdoGrouping_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles RdoGrouping.Checked, RdoSearch.Checked
        Try
            If RdoGrouping.IsChecked Then
                txtID.Visibility = Visibility.Hidden
                txtName.Visibility = Visibility.Hidden
                txtPrice.Visibility = Visibility.Hidden
                HelpGD.Visibility = Visibility.Hidden
                PanelGroups.Visibility = Visibility.Visible
                PanelTypes.Visibility = Visibility.Visible
                PanelItems.Visibility = Visibility.Visible
            ElseIf RdoSearch.IsChecked Then
                txtID.Visibility = Visibility.Visible
                txtName.Visibility = Visibility.Visible
                txtPrice.Visibility = Visibility.Visible
                HelpGD.Visibility = Visibility.Visible
                PanelGroups.Visibility = Visibility.Hidden
                PanelTypes.Visibility = Visibility.Hidden
                PanelItems.Visibility = Visibility.Hidden
            End If
        Catch
        End Try
    End Sub


    Dim LopCalc As Boolean = False
    Private Sub CalcTotal() Handles Total.TextChanged, DiscountPerc.TextChanged, DiscountValue.TextChanged, Taxvalue.TextChanged, ServiceValue.TextChanged, MinPerPerson.TextChanged, NoOfPersons.TextChanged, WithTax.Checked, WithTax.Unchecked, WithService.Checked, WithService.Unchecked, CancelMinPerPerson.Checked, CancelMinPerPerson.Unchecked, ToId.LostFocus
        If LopCalc Or lop Then Return
        Try
            LopCalc = True
            MinPerPerson.Text = Math.Round(0, 2)
            Total.Text = Math.Round(0, 2)
            Taxvalue.Text = Math.Round(0, 2)
            ServiceValue.Text = Math.Round(0, 2)

            If CancelMinPerPerson.IsChecked Then
                MinPerPerson.Text = Math.Round(0, 2)
            Else
                MinPerPerson.Text = Val(bm.ExecuteScalar("select dbo.GetMinValuePerPerson(" & StoreId.Text & ")"))
            End If
            For i As Integer = 0 To G.Rows.Count - 1
                Total.Text += Val(G.Rows(i).Cells(GC.Value).Value)
            Next

            If Val(DiscountPerc.Text) > 0 Then
                'DiscountValue.Text = Math.Round(Val(Total.Text) * Val(DiscountPerc.Text) / 100, 2)
                DiscountValue.Text = Val(Total.Text) * Val(DiscountPerc.Text) / 100
            End If


            If Not lop Or Not IsClosed.IsChecked Then

                If Val(Total.Text) < Val(MinPerPerson.Text) * Val(NoOfPersons.Text) Then
                    'Total.Text = Math.Round(Val(MinPerPerson.Text) * Val(NoOfPersons.Text), 2)
                    Total.Text = Val(MinPerPerson.Text) * Val(NoOfPersons.Text)
                End If

                If Val(DiscountPerc.Text) > 0 Then
                    'DiscountValue.Text = Math.Round(Val(Total.Text) * Val(DiscountPerc.Text) / 100, 2)
                    DiscountValue.Text = Val(Total.Text) * Val(DiscountPerc.Text) / 100
                End If


                If WithTax.IsChecked Then
                    Taxvalue.Text = 0.01 * (Val(Total.Text) - Val(DiscountValue.Text)) * Val(bm.ExecuteScalar("select dbo.GetTaxPerc(" & StoreId.Text & ")"))
                Else
                    Taxvalue.Text = Math.Round(0, 2)
                End If
                If WithService.IsChecked Then
                    If TestDelivary() Then
                        ServiceValue.Text = Val(bm.ExecuteScalar("select dbo.GetDelivaryCost(" & Val(StoreId.Text) & "," & Val(ToId.Text) & ")"))
                    Else
                        'ServiceValue.Text = Math.Round((Val(Total.Text) - Val(DiscountValue.Text)) * Val(bm.ExecuteScalar("select dbo.GetServicePerc(" & StoreId.Text & ")")) / 100, 2)
                        ServiceValue.Text = (Val(Total.Text) - Val(DiscountValue.Text)) * Val(bm.ExecuteScalar("select dbo.GetServicePerc(" & StoreId.Text & ")")) / 100
                    End If
                Else
                    ServiceValue.Text = Math.Round(0, 2)
                End If

            End If

            LopCalc = False
            CalcTotalEnd()
        Catch ex As Exception
        End Try
    End Sub

    Sub CalcTotalEnd() Handles Per1.TextChanged, Per2.TextChanged, Per3.TextChanged, Per4.TextChanged, Val1.LostFocus, Val2.LostFocus, Val3.LostFocus, Val4.LostFocus, AccType1.SelectionChanged, AccType2.SelectionChanged, AccType3.SelectionChanged, AccType4.SelectionChanged, Shipping.LostFocus, Freight.LostFocus, CustomClearance.LostFocus
        'Val1.Text = Math.Round(Val(Total.Text) * Val(Per1.Text) / 100, 2)
        'Val2.Text = Math.Round(Val(Total.Text) * Val(Per2.Text) / 100, 2)
        'Val3.Text = Math.Round(Val(Total.Text) * Val(Per3.Text) / 100, 2)
        'Val4.Text = Math.Round(Val(Total.Text) * Val(Per4.Text) / 100, 2)

        Val1.IsEnabled = Val(Per1.Text) = 0
        Val2.IsEnabled = Val(Per2.Text) = 0
        Val3.IsEnabled = Val(Per3.Text) = 0
        Val4.IsEnabled = Val(Per4.Text) = 0

        If Val(Per1.Text) <> 0 Then Val1.Text = (Val(Total.Text) - Val(DiscountValue.Text)) * Val(Per1.Text) / 100
        If Val(Per2.Text) <> 0 Then Val2.Text = (Val(Total.Text) - Val(DiscountValue.Text)) * Val(Per2.Text) / 100
        If Val(Per3.Text) <> 0 Then Val3.Text = (Val(Total.Text) - Val(DiscountValue.Text)) * Val(Per3.Text) / 100
        If Val(Per4.Text) <> 0 Then Val4.Text = (Val(Total.Text) - Val(DiscountValue.Text)) * Val(Per4.Text) / 100

        Dim d1 As Decimal = Val(Val1.Text)
        Dim d2 As Decimal = Val(Val2.Text)
        Dim d3 As Decimal = Val(Val3.Text)
        Dim d4 As Decimal = Val(Val4.Text)

        If AccType1.SelectedValue = 1 Then d1 *= -1
        If AccType2.SelectedValue = 1 Then d2 *= -1
        If AccType3.SelectedValue = 1 Then d3 *= -1
        If AccType4.SelectedValue = 1 Then d4 *= -1

        If AccType1.SelectedIndex < 1 Then d1 = 0
        If AccType2.SelectedIndex < 1 Then d2 = 0
        If AccType3.SelectedIndex < 1 Then d3 = 0
        If AccType4.SelectedIndex < 1 Then d4 = 0

        'TotalAfterDiscount.Text = Math.Round(Val(Total.Text) - Val(DiscountValue.Text) + Val(Taxvalue.Text) + Val(ServiceValue.Text) + d1 + d2 + d3 + d4, 2)
        TotalAfterDiscount.Text = Val(Total.Text) - Val(DiscountValue.Text) + Val(Taxvalue.Text) + Val(ServiceValue.Text) + d1 + d2 + d3 + d4 + Val(Shipping.Text) + Val(Freight.Text) + Val(CustomClearance.Text)


    End Sub

    Dim DontClear As Boolean = False
    Private Sub btnCloseTable_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCloseTable.Click
        If btnPrint.IsEnabled Then
            AllowClose = False
            DontClear = True
            btnSave_Click(btnCloseTable, e)
            DontClear = False
            If Not AllowClose Then Return
        End If
        'If Not bm.ExcuteNonQuery("update SalesMaster set IsClosed=1,ClosedDate=getdate(),DayDate='" & bm.ToStrDate(Md.CurrentDate) & "',Shift=" & Md.CurrentShiftId & " where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and InvoiceNo=" & InvoiceNo.Text) Then Return
        If Not bm.ExcuteNonQuery("update SalesMaster set IsClosed=1,ClosedDate=getdate(),DayDate='" & bm.ToStrDate(bm.MyGetDate()) & "',Shift=" & Md.CurrentShiftId & " where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and InvoiceNo=" & InvoiceNo.Text) Then Return
        btnNew_Click(sender, e)
    End Sub

    Private Sub IsClosed_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles IsClosed.Checked, IsClosed.Unchecked
        btnCloseTable.IsEnabled = Not IsClosed.IsChecked
    End Sub


    Private Sub IsCashierPrinted_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles IsCashierPrinted.Checked, IsCashierPrinted.Unchecked
        btnSave.IsEnabled = Not (IsCashierPrinted.IsChecked And Not Md.Manager)
        btnPrint.IsEnabled = Not (IsCashierPrinted.IsChecked And Not Md.Manager)
        btnDelete.IsEnabled = Not (IsCashierPrinted.IsChecked And Not Md.Manager)
    End Sub

    Private Sub btnTableClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim x As Button = sender
        If ChkSplite.IsChecked Then
            LoadSubTables(x.Tag)
        Else
            GetTable(x.Tag, 1)
        End If
    End Sub

    Sub GetTable(ByVal MainTable As Integer, ByVal SubTable As Integer)
        InvoiceNo.Text = bm.ExecuteScalar("select InvoiceNo from SalesMaster where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and TableId=" & MainTable & " and TableSubId=" & SubTable & " and IsClosed=0")
        InvoiceNo_Leave(Nothing, Nothing)
        TableId.Text = MainTable
        TableSubId.Text = SubTable
        TId_LostFocus(TableId, Nothing)
        TabControl1.SelectedItem = TabItem1
    End Sub


    Private Sub TabControl1_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles TabControl1.SelectionChanged
        If e.AddedItems.Count = 0 Then Return
        If e.AddedItems(0) Is TabItemTables Then
            LoadTables()
        ElseIf e.AddedItems(0) Is TabItemDelivery Then
            LoadUnPaiedInvoices()
        End If
    End Sub

    Private Sub btnDeliveryClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim x As Button = sender
        InvoiceNo.Text = x.Tag
        InvoiceNo_Leave(Nothing, Nothing)
        TId_LostFocus(TableId, Nothing)
        TabControl1.SelectedItem = TabItem1
    End Sub

    Private Sub TestDoublicatinInTables(ByVal msg As Boolean)
        If TableId.Text.Trim = "" Or IsClosed.IsChecked Then Return
        Dim s As String = bm.ExecuteScalar("select InvoiceNo from SalesMaster where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and TableId=" & TableId.Text & " and TableSubId=" & TableSubId.Text & " and IsClosed=0")
        If s <> "" AndAlso s <> InvoiceNo.Text Then
            If msg Then bm.ShowMSG("هذه المائدة مفتوحة بمسلسل " & s)
            TableSubId.Clear()
        End If
    End Sub

    Private Sub ChkSplite_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkSplite.Checked
        SpliteScrollViewer.Visibility = Visibility.Visible
        WSubTables.Children.Clear()
    End Sub
    Private Sub ChkSplite_UnChecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ChkSplite.Unchecked
        SpliteScrollViewer.Visibility = Visibility.Hidden
        WSubTables.Children.Clear()
    End Sub

    Private Sub LoadSubTables(ByVal MyTag As Integer)
        WSubTables.Children.Clear()
        Dim z As Integer = Val(bm.ExecuteScalar("select top 1 MaxSubTable from Statics"))
        Dim dtInv As DataTable = bm.ExcuteAdapter("select InvoiceNo,TableId,TableSubId,dbo.ToStrTime(OpennedDate) OpennedTime,NoOfPersons,IsCashierPrinted from SalesMaster where Flag=" & Flag & " and StoreId=" & StoreId.Text & " and IsClosed=0")
        For i As Integer = 1 To z
            Try
                Dim x As New Button
                x.Name = "SubTable_" & i
                x.Tag = MyTag
                x.Width = 50
                x.Height = 50
                x.Cursor = Input.Cursors.Pen
                x.Content = i
                WSubTables.Children.Add(x)

                If dtInv.Select("TableId=" & x.Tag & " and TableSubId=" & i).Length > 0 Then
                    If dtInv.Select("TableId=" & x.Tag & " and TableSubId=" & i)(0)("IsCashierPrinted") = 1 Then
                        x.Background = System.Windows.Media.Brushes.Magenta
                    Else
                        x.Background = System.Windows.Media.Brushes.Red
                    End If
                Else
                    x.Background = System.Windows.Media.Brushes.LimeGreen
                End If

                AddHandler x.Click, AddressOf btnSubTableClick
            Catch
            End Try
        Next

    End Sub

    Private Sub btnSubTableClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim x As Button = CType(sender, Button)
        GetTable(x.Tag, x.Name.Replace("SubTable_", ""))
    End Sub

    Private Sub GridKeyDown(ByVal sender As Object, ByVal e As Forms.KeyEventArgs)
        e.Handled = True
        If Receive Then Return
        Try
            If G.CurrentCell.RowIndex = G.Rows.Count - 1 Then
                Dim c = G.CurrentCell.RowIndex
                G.Rows.Add()
                G.CurrentCell = G.Rows(c).Cells(G.CurrentCell.ColumnIndex)
            End If
            If G.CurrentCell.ColumnIndex = G.Columns(GC.SalesInvoiceNo).Index Then
                If e.KeyCode = Forms.Keys.F1 AndAlso OrderTypeId.Visibility = Windows.Visibility.Visible AndAlso Val(OrderTypeId.Text) = 0 Then
                    bm.ShowMSG("برجاء تحديد الطلبية")
                    OrderTypeId.Focus()
                    Return
                End If
                If bm.ShowHelpGrid("Invoices", G.CurrentRow.Cells(GC.SalesInvoiceNo), G.CurrentRow.Cells(GC.SalesInvoiceNo), e, "select InvoiceNo,dbo.ToStrDate(DayDate) DayDate from SalesMaster where StoreId=" & StoreId.Text & " and Flag=" & MainFlag() & " and ToId=" & Val(ToId.Text) & " and OrderTypeId=" & Val(OrderTypeId.Text), , "الفاتورة", "التاريخ") Then
                    GridCalcRow(sender, New Forms.DataGridViewCellEventArgs(G.Columns(GC.SalesInvoiceNo).Index, G.CurrentCell.RowIndex))
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Id)
                End If
            ElseIf G.CurrentCell.ColumnIndex = G.Columns(GC.Id).Index OrElse G.CurrentCell.ColumnIndex = G.Columns(GC.Name).Index Then
                Dim str As String = "select cast(Id as varchar(100)) Id,Name," & PriceFieldName(GC.Price, 0) & " 'السعر' from Items where IsStopped=0 " & ItemWhere()
                If Val(G.Rows(G.CurrentCell.RowIndex).Cells(GC.SalesInvoiceNo).Value) > 0 Then
                    str = "select cast(ItemId as varchar(100)) Id,ItemName Name,Price 'السعر' from SalesDetails where StoreId=" & StoreId.Text & " and Flag=" & Flag - 1 & " and InvoiceNo=" & Val(G.Rows(G.CurrentCell.RowIndex).Cells(GC.SalesInvoiceNo).Value)
                End If
                If bm.ShowHelpGrid("Items", G.CurrentRow.Cells(GC.Id), G.CurrentRow.Cells(GC.Name), e, str) Then
                    GridCalcRow(sender, New Forms.DataGridViewCellEventArgs(G.Columns(GC.Id).Index, G.CurrentCell.RowIndex))
                    If G.Rows(G.CurrentCell.RowIndex).Cells(GC.UnitId).Visible Then
                        G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.UnitId)
                    ElseIf G.Rows(G.CurrentCell.RowIndex).Cells(GC.Color).Visible Then
                        G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Color)
                    ElseIf G.Rows(G.CurrentCell.RowIndex).Cells(GC.Qty).Visible Then
                        G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Qty)
                    End If

                End If
            End If


            If bm.ShowHelpGridItemBal(G.CurrentRow.Cells(GC.Id), G.CurrentRow.Cells(GC.Name), e, "GetItemCurrentBal " & Val(G.CurrentRow.Cells(GC.Id).Value)) Then
                GridCalcRow(sender, New Forms.DataGridViewCellEventArgs(G.Columns(GC.Id).Index, G.CurrentCell.RowIndex))
                If G.Rows(G.CurrentCell.RowIndex).Cells(GC.UnitId).Visible Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.UnitId)
                ElseIf G.Rows(G.CurrentCell.RowIndex).Cells(GC.Color).Visible Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Color)
                ElseIf G.Rows(G.CurrentCell.RowIndex).Cells(GC.Qty).Visible Then
                    G.CurrentCell = G.Rows(G.CurrentCell.RowIndex).Cells(GC.Qty)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Sub SetStyle(ByVal x As Button)
        x.Style = Application.Current.FindResource("GlossyCloseButton")
        x.VerticalContentAlignment = Windows.VerticalAlignment.Center
        x.Width = 85
        x.Height = 30
        x.Margin = New Thickness(5, 5, 0, 0)
    End Sub


    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")


    End Sub

    Private Sub HideAcc()
        'PanelItems.Margin = New Thickness(PanelItems.Margin.Left, PanelItems.Margin.Top, PanelItems.Margin.Right, 8)
        'HelpGD.Margin = New Thickness(HelpGD.Margin.Left, HelpGD.Margin.Top, HelpGD.Margin.Right, 8)
        'PanelAcc.Visibility = Windows.Visibility.Collapsed
        Tab1.Visibility = Windows.Visibility.Collapsed

        GroupBoxPaymentType.Visibility = Visibility.Hidden
    End Sub

    Private Sub HideAcc2()
        Tab2.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub HideAcc3()
        Tab3.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub AccNo1_KeyUp(sender As Object, e As KeyEventArgs) Handles AccNo1.KeyUp
        bm.AccNoShowHelp(AccNo1, AccName1, e, , , )
    End Sub
    Private Sub AccNo2_KeyUp(sender As Object, e As KeyEventArgs) Handles AccNo2.KeyUp
        bm.AccNoShowHelp(AccNo2, AccName2, e, , , )
    End Sub
    Private Sub AccNo3_KeyUp(sender As Object, e As KeyEventArgs) Handles AccNo3.KeyUp
        bm.AccNoShowHelp(AccNo3, AccName3, e, , , )
    End Sub
    Private Sub AccNo4_KeyUp(sender As Object, e As KeyEventArgs) Handles AccNo4.KeyUp
        bm.AccNoShowHelp(AccNo4, AccName4, e, , , )
    End Sub

    Private Sub AccNo1_LostFocus(sender As Object, e As RoutedEventArgs) Handles AccNo1.LostFocus
        bm.AccNoLostFocus(AccNo1, AccName1, , , )
    End Sub
    Private Sub AccNo2_LostFocus(sender As Object, e As RoutedEventArgs) Handles AccNo2.LostFocus
        bm.AccNoLostFocus(AccNo2, AccName2, , , )
    End Sub
    Private Sub AccNo3_LostFocus(sender As Object, e As RoutedEventArgs) Handles AccNo3.LostFocus
        bm.AccNoLostFocus(AccNo3, AccName3, , , )
    End Sub
    Private Sub AccNo4_LostFocus(sender As Object, e As RoutedEventArgs) Handles AccNo4.LostFocus
        bm.AccNoLostFocus(AccNo4, AccName4, , , )
    End Sub

    Private Sub LoadItemUint(i As Integer)
        Dim Id As Integer = Val(G.Rows(i).Cells(GC.Id).Value)
        'Dim dt As DataTable = bm.ExcuteAdapter("Select * From Items where Id='" & Id & "' and " & ItemWhere() & "")

        If G.Columns(GC.UnitId).Visible Then bm.FillCombo("select 0 Id,Unit Name From Items where Id='" & Id & "' " & ItemWhere() & " union select 1 Id,UnitSub Name From Items where Id='" & Id & "' " & ItemWhere() & " union select 2 Id,UnitSub2 Name From Items where Id='" & Id & "' " & ItemWhere() & "", G.Rows(i).Cells(GC.UnitId))

        If G.Columns(GC.Color).Visible Then bm.FillCombo("select 0 Id,'-' Name union select Id,Name from ColorsDetails where ColorId=(select It.ColorId from Items It where It.Id='" & Id & "' " & ItemWhere() & ") order by Id", G.Rows(i).Cells(GC.Size))

        If G.Columns(GC.UnitId).Visible Then bm.FillCombo("select 0 Id,'-' Name union select Id,Name from SizesDetails where SizeId=(select It.SizeId from Items It where It.Id='" & Id & "' " & ItemWhere() & ") order by Id", G.Rows(i).Cells(GC.Size))


        If G.Rows(i).Cells(GC.UnitId).Value Is Nothing Then
            If Md.ShowQtySub Then
                G.Rows(i).Cells(GC.UnitId).Value = StoreUnitId
            Else
                G.Rows(i).Cells(GC.UnitId).Value = 0
            End If
        End If
        If G.Rows(i).Cells(GC.Color).Value Is Nothing Then G.Rows(i).Cells(GC.Color).Value = 0
        If G.Rows(i).Cells(GC.Size).Value Is Nothing Then G.Rows(i).Cells(GC.Size).Value = 0

        'If TestConsumablesAndReturn() Then
        '    'G.Rows(i).Cells(GC.UnitId).Value = 2
        'End If

    End Sub

    Private Sub LoadItemPrice(i As Integer)
        Dim dt As DataTable = bm.ExcuteAdapter("Select * From Items_View where Id='" & G.Rows(i).Cells(GC.Id).Value & "' " & ItemWhere())
        If dt.Rows.Count = 0 Then Return
        If Val(G.Rows(i).Cells(GC.Price).Value) = 0 OrElse G.CurrentCell.ColumnIndex = G.Columns(GC.UnitId).Index Then
            G.Rows(i).Cells(GC.Price).Value = dt.Rows(0)(PriceFieldName(GC.Price, G.Rows(i).Cells(GC.UnitId).Value))
        End If
        G.Rows(i).Cells(GC.UnitQty).Value = UnitCount(dt, G.Rows(i).Cells(GC.UnitId).Value)
        G.Rows(i).Cells(GC.SalesPrice).Value = dt.Rows(0)(PriceFieldName(GC.Price, G.Rows(i).Cells(GC.UnitId).Value, True))
        If TestSalesAndReturn() AndAlso Val(G.Rows(i).Cells(GC.Size).Value) > 0 Then
            dt = bm.ExcuteAdapter("Select * From ItemSizes where ItemId='" & G.Rows(i).Cells(GC.Id).Value & "' and Id='" & G.Rows(i).Cells(GC.Size).Value & "'")
            If dt.Rows.Count = 0 Then Return
            If Val(G.Rows(i).Cells(GC.Price).Value) = 0 Then
                G.Rows(i).Cells(GC.Price).Value = dt.Rows(0)("SalesPrice")
            End If
        End If
        If TestPurchaseOnly() AndAlso Val(G.Rows(i).Cells(GC.Size).Value) > 0 Then
            dt = bm.ExcuteAdapter("Select * From ItemSizes where ItemId='" & G.Rows(i).Cells(GC.Id).Value & "' and Id='" & G.Rows(i).Cells(GC.Size).Value & "'")
            If dt.Rows.Count = 0 Then Return
            If Val(G.Rows(i).Cells(GC.Price).Value) = 0 Then
                G.Rows(i).Cells(GC.Price).Value = dt.Rows(0)("PurchasePrice")
            End If
            G.Rows(i).Cells(GC.SalesPrice).Value = dt.Rows(0)("SalesPrice")
        End If

        If Val(G.Rows(i).Cells(GC.SalesInvoiceNo).Value) > 0 Then
            dt = bm.ExcuteAdapter("select Price from SalesDetails where StoreId=" & StoreId.Text & " and Flag=" & MainFlag() & " and InvoiceNo=" & G.Rows(i).Cells(GC.SalesInvoiceNo).Value & " and ItemId=" & G.Rows(i).Cells(GC.Id).Value)
            If dt.Rows.Count = 0 Then
                bm.ShowMSG("هذا الصنف غير موجود بالفاتورة")
                ClearRow(i)
                Return
            End If
            Dim x As Decimal = Val(dt.Rows(0)(0))
            If x > 0 Then G.Rows(i).Cells(GC.Price).Value = x
        End If

    End Sub


    Private Sub ComboBox1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ComboBox1.SelectionChanged
        If ComboBox1.SelectedIndex = -1 Then Return
        Flag = ComboBox1.SelectedValue
        txtFlag.Text = ComboBox1.SelectedValue
        CType(Parent, Page).Title = CType(ComboBox1.ItemsSource, DataView).Item(ComboBox1.SelectedIndex)("Name")
        Md.Currentpage = CType(ComboBox1.ItemsSource, DataView).Item(ComboBox1.SelectedIndex)("Name")
        LoadVisibility()
        btnNew_Click(Nothing, Nothing)
    End Sub

    Dim looop As Boolean = False
    Private Sub LoadCbo()
        looop = True
        Dim FF = Flag
        Dim dt As New DataTable("tbl")
        dt.Columns.Add("Id")
        dt.Columns.Add("Name")
        Select Case Flag
            Case 1, 2, 3, 4, 5, 6, 7, 8
                dt.Rows.Add(New String() {1, "أرصدة افتتاحية"})
                dt.Rows.Add(New String() {2, "إضافة"})
                dt.Rows.Add(New String() {3, "تسوية إضافة"})
                dt.Rows.Add(New String() {4, "صرف"})
                dt.Rows.Add(New String() {5, "تسوية صرف"})
                dt.Rows.Add(New String() {6, "هدايا"})
                dt.Rows.Add(New String() {7, "هالك"})
                dt.Rows.Add(New String() {8, "تحويل إلى مخزن"})
            Case 9, 10
                dt.Rows.Add(New String() {9, "مشتريات"})
                dt.Rows.Add(New String() {10, "مردودات مشتريات"})
            Case 11, 12, 13, 14, 15, 16
                'dt.Rows.Add(New String() {11, "مبيعات الصالة"})
                'dt.Rows.Add(New String() {12, "مردودات مبيعات الصالة"})
                'dt.Rows.Add(New String() {13, "مبيعات التيك أواى"})
                'dt.Rows.Add(New String() {14, "مردودات مبيعات التيك أواى"})
                'dt.Rows.Add(New String() {15, "مبيعات التوصيل"})
                'dt.Rows.Add(New String() {16, "مردودات مبيعات التوصيل"})
                'IsClosedOnly.Visibility = Visibility.Visible
                dt.Rows.Add(New String() {13, "المبيعات"})
                dt.Rows.Add(New String() {14, "مردودات المبيعات"})
        End Select

        Dim dv As New DataView
        dv.Table = dt
        dv.Sort = "Id"
        ComboBox1.ItemsSource = dv
        ComboBox1.SelectedValuePath = "Id"
        ComboBox1.DisplayMemberPath = "Name"
        ComboBox1.SelectedIndex = 0

        ComboBox1.SelectedValue = FF
        ComboBox1_SelectionChanged(Nothing, Nothing)
        looop = False
    End Sub

    Private Sub btnPrint5_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint5.Click
        btnSave_Click(sender, e)
    End Sub

    Private Sub Payed_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Payed.TextChanged, TotalAfterDiscount.TextChanged
        Remaining.Clear()
        If Val(Payed.Text) = 0 Then Return
        Remaining.Text = Val(Payed.Text) - IIf(Val(CashValue.Text) > 0, Val(CashValue.Text), Val(TotalAfterDiscount.Text))
    End Sub

    Private Sub LoadVisibility()
        'Dim x As Integer = AccNo.Margin.Top
        'lblOrderTypeId.Margin = New Thickness(lblOrderTypeId.Margin.Left, x, lblOrderTypeId.Margin.Right, lblOrderTypeId.Margin.Bottom)
        'OrderTypeId.Margin = New Thickness(OrderTypeId.Margin.Left, x, OrderTypeId.Margin.Right, OrderTypeId.Margin.Bottom)
        'OrderTypeName.Margin = New Thickness(OrderTypeName.Margin.Left, x, OrderTypeName.Margin.Right, OrderTypeName.Margin.Bottom)

        'lblVersionNo.Margin = New Thickness(lblVersionNo.Margin.Left, x, lblVersionNo.Margin.Right, lblVersionNo.Margin.Bottom)
        'VersionNo.Margin = New Thickness(VersionNo.Margin.Left, x, VersionNo.Margin.Right, VersionNo.Margin.Bottom)

        'lblDeliveryDate.Margin = New Thickness(lblDeliveryDate.Margin.Left, x, lblDeliveryDate.Margin.Right, lblDeliveryDate.Margin.Bottom)
        'DeliveryDate.Margin = New Thickness(DeliveryDate.Margin.Left, x, DeliveryDate.Margin.Right, DeliveryDate.Margin.Bottom)


        lblOrderTypeId.Visibility = Windows.Visibility.Hidden
        OrderTypeId.Visibility = Windows.Visibility.Hidden
        OrderTypeName.Visibility = Windows.Visibility.Hidden

        btnDelete.Visibility = Windows.Visibility.Visible
        btnFirst.Visibility = Windows.Visibility.Visible
        btnLast.Visibility = Windows.Visibility.Visible
        btnNext.Visibility = Windows.Visibility.Visible
        btnPrevios.Visibility = Windows.Visibility.Visible
        btnPrint.Visibility = Windows.Visibility.Visible
        btnPrint3.Visibility = Windows.Visibility.Visible
        btnPrint4.Visibility = Windows.Visibility.Visible
        btnPrint5.Visibility = Windows.Visibility.Visible
        CashierId.Visibility = Visibility.Hidden
        CashierName.Visibility = Visibility.Hidden
        ComboBox1.Visibility = Windows.Visibility.Visible
        DayDate.Visibility = Visibility.Visible
        DiscountPerc.Visibility = Visibility.Visible
        DiscountValue.Visibility = Visibility.Visible
        GroupBoxPaymentType.Visibility = Visibility.Visible
        lblCashier.Visibility = Visibility.Hidden
        lblComboBox1.Visibility = Windows.Visibility.Visible
        lblDayDate.Visibility = Visibility.Visible
        lblDiscount.Visibility = Visibility.Visible
        lblDiscount_Copy.Visibility = Visibility.Visible
        lblDiscount_Copy1.Visibility = Visibility.Visible
        DocNo.Visibility = Visibility.Visible
        lblDocNo.Visibility = Visibility.Visible
        lblLE.Visibility = Visibility.Visible
        lblPayed.Visibility = Windows.Visibility.Visible
        lblPerc.Visibility = Visibility.Visible
        lblRemaining.Visibility = Windows.Visibility.Visible
        ReservToId.Visibility = Visibility.Hidden
        Payed.Visibility = Windows.Visibility.Visible
        Remaining.Visibility = Windows.Visibility.Visible
        lblToId.Visibility = Visibility.Hidden
        ToId.Visibility = Visibility.Hidden
        ToName.Visibility = Visibility.Hidden
        WP.Visibility = Windows.Visibility.Visible
        Tab2.Visibility = Windows.Visibility.Collapsed
        Tab3.Visibility = Windows.Visibility.Collapsed

        lblDeliveryDate.Visibility = Windows.Visibility.Hidden
        DeliveryDate.Visibility = Windows.Visibility.Hidden
        lblVersionNo.Visibility = Windows.Visibility.Hidden
        VersionNo.Visibility = Windows.Visibility.Hidden
        lblMessageId.Visibility = Windows.Visibility.Hidden
        MessageId.Visibility = Windows.Visibility.Hidden

        btnPrint3.Content = "طباعة كميات"
        Select Case Flag
            Case FlagState.الاستيراد, FlagState.مردودات_الاستيراد, FlagState.عرض_أسعار, FlagState.أمر_شراء
                btnPrintImage.Visibility = Windows.Visibility.Visible
            Case Else
                btnPrintImage.Visibility = Windows.Visibility.Hidden
        End Select


        If Flag = FlagState.أرصدة_افتتاحية Then
            'lblDayDate.Visibility = Visibility.Hidden
            'DayDate.Visibility = Visibility.Hidden
            lblShift.Visibility = Visibility.Hidden
            Shift.Visibility = Visibility.Hidden

            lblDocNo.Visibility = Visibility.Hidden
            DocNo.Visibility = Visibility.Hidden
        ElseIf Flag = FlagState.تحويل_إلى_مخزن Then
            lblCashier.Visibility = Visibility.Visible
            CashierId.Visibility = Visibility.Visible
            CashierName.Visibility = Visibility.Visible
            lblCashier.Content = "المستلم"

            lblToId.Visibility = Visibility.Visible
            ToId.Visibility = Visibility.Visible
            ToName.Visibility = Visibility.Visible
            lblToId.Content = "المخزن المحول إليه"

            lblDocNo.Visibility = Visibility.Hidden
            DocNo.Visibility = Visibility.Hidden

            lblTotal.Visibility = Visibility.Hidden
            Total.Visibility = Visibility.Hidden

            btnPrint3.Content = "إذن صرف"
        End If

        If TestPurchaseAndReturn() Then

            If Flag <> FlagState.أمر_شراء Then
                lblCashier.Visibility = Visibility.Visible
                CashierId.Visibility = Visibility.Visible
                CashierName.Visibility = Visibility.Visible
                lblCashier.Content = "الطالب"

                GroupBoxPaymentType.Visibility = Visibility.Visible

                lblSaveId.Visibility = Windows.Visibility.Visible
                SaveId.Visibility = Windows.Visibility.Visible
                SaveName.Visibility = Windows.Visibility.Visible
            Else
                lblCashier.Visibility = Visibility.Hidden
                CashierId.Visibility = Visibility.Hidden
                CashierName.Visibility = Visibility.Hidden

                GroupBoxPaymentType.Visibility = Visibility.Hidden

                lblSaveId.Visibility = Windows.Visibility.Hidden
                SaveId.Visibility = Windows.Visibility.Hidden
                SaveName.Visibility = Windows.Visibility.Hidden

                btnPrint3.Visibility = Windows.Visibility.Hidden

            End If

            ReservToId.Visibility = Visibility.Visible
            lblToId.Visibility = Visibility.Visible
            ToId.Visibility = Visibility.Visible
            ToName.Visibility = Visibility.Visible
            lblToId.Content = "المورد"

            If Md.ShowCurrency Then
                lblCurrencyId.Visibility = Windows.Visibility.Visible
                CurrencyId.Visibility = Windows.Visibility.Visible
            End If

            btnPrint3.Content = "إذن إضافة"
            If Flag = FlagState.مردودات_مشتريات Then
                btnPrint3.Content = "إذن صرف"
            End If
        ElseIf TestImportAndReturn() Then
            lblCashier.Visibility = Visibility.Visible
            CashierId.Visibility = Visibility.Visible
            CashierName.Visibility = Visibility.Visible
            lblCashier.Content = "الطالب"

            'ReservToId.Visibility = Visibility.Visible
            lblToId.Visibility = Visibility.Visible
            ToId.Visibility = Visibility.Visible
            ToName.Visibility = Visibility.Visible
            lblToId.Content = "المورد"

            lblDocNo.Visibility = Visibility.Visible
            DocNo.Visibility = Visibility.Visible
            lblDocNo.Content = "رقم عقد المورد"


            'lblAccNo.Visibility = Windows.Visibility.Visible
            'AccNo.Visibility = Windows.Visibility.Visible
            'AccName.Visibility = Windows.Visibility.Visible

            lblOrderTypeId.Visibility = Windows.Visibility.Visible
            OrderTypeId.Visibility = Windows.Visibility.Visible
            OrderTypeName.Visibility = Windows.Visibility.Visible

            lblVersionNo.Visibility = Windows.Visibility.Visible
            VersionNo.Visibility = Windows.Visibility.Visible

            If Flag = FlagState.الاستيراد Then
                lblMessageId.Visibility = Windows.Visibility.Visible
                MessageId.Visibility = Windows.Visibility.Visible
            End If

            'Tab1.Visibility = Windows.Visibility.Collapsed
            Tab3.Visibility = Windows.Visibility.Visible
            PanelAcc.SelectedItem = Tab3
            If Md.ShowCurrency Then
                lblCurrencyId.Visibility = Windows.Visibility.Visible
                CurrencyId.Visibility = Windows.Visibility.Visible
            End If

            RdoFuture.IsChecked = True
            GroupBoxPaymentType.Visibility = Windows.Visibility.Hidden

            btnPrint3.Content = "إذن إضافة"
            'HideAcc()
        ElseIf TestSalesAndReturn() Then


            If Flag <> FlagState.عرض_أسعار Then
                lblCashier.Visibility = Visibility.Visible
                CashierId.Visibility = Visibility.Visible
                CashierName.Visibility = Visibility.Visible
                lblCashier.Content = "البائع"

                GroupBoxPaymentType.Visibility = Visibility.Visible

                lblSaveId.Visibility = Windows.Visibility.Visible
                SaveId.Visibility = Windows.Visibility.Visible
                SaveName.Visibility = Windows.Visibility.Visible
            Else
                lblCashier.Visibility = Visibility.Hidden
                CashierId.Visibility = Visibility.Hidden
                CashierName.Visibility = Visibility.Hidden

                GroupBoxPaymentType.Visibility = Visibility.Hidden

                lblSaveId.Visibility = Windows.Visibility.Hidden
                SaveId.Visibility = Windows.Visibility.Hidden
                SaveName.Visibility = Windows.Visibility.Hidden

                btnPrint3.Visibility = Windows.Visibility.Hidden

                lblDeliveryDate.Visibility = Windows.Visibility.Hidden
                DeliveryDate.Visibility = Windows.Visibility.Hidden

            End If


            ReservToId.Visibility = Visibility.Visible
            lblToId.Visibility = Visibility.Visible
            ToId.Visibility = Visibility.Visible
            ToName.Visibility = Visibility.Visible
            lblToId.Content = "العميل"

            If Md.ShowCurrency Then
                lblCurrencyId.Visibility = Windows.Visibility.Visible
                CurrencyId.Visibility = Windows.Visibility.Visible
            End If

                lblWaiter.Visibility = Windows.Visibility.Visible
                WaiterId.Visibility = Windows.Visibility.Visible
                WaiterName.Visibility = Windows.Visibility.Visible
            
            btnPrint3.Content = "إذن صرف"
            If Flag = FlagState.مردودات_المبيعات Then
                btnPrint3.Content = "إذن إضافة"
            End If
        ElseIf TestConsumablesAndReturn() Then
            lblCashier.Visibility = Visibility.Visible
            CashierId.Visibility = Visibility.Visible
            CashierName.Visibility = Visibility.Visible
            lblCashier.Content = "الممرضة"

            GroupBoxPaymentType.Visibility = Visibility.Visible

            'lblDiscount.Visibility = Visibility.Visible
            'lblPerc.Visibility = Visibility.Visible
            'lblLE.Visibility = Visibility.Visible
            'DiscountPerc.Visibility = Visibility.Visible
            'DiscountValue.Visibility = Visibility.Visible

            lblToId.Visibility = Visibility.Visible
            ToId.Visibility = Visibility.Visible
            ToName.Visibility = Visibility.Visible
            lblToId.Content = "المريض"

            'DocNo.IsEnabled = False

            btnPrint3.Content = "إذن صرف"
            If Flag = FlagState.مردودات_المستهلكات Then
                btnPrint3.Content = "إذن إضافة"
            End If

            If Not Md.Manager Then

                lblDiscount.Visibility = Visibility.Hidden
                lblDiscount_Copy.Visibility = Visibility.Hidden
                lblDiscount_Copy1.Visibility = Visibility.Hidden
                DiscountPerc.Visibility = Visibility.Hidden
                DiscountValue.Visibility = Visibility.Hidden

                btnPrint3.Visibility = Visibility.Hidden
                'btnPrint2.Visibility = Visibility.Hidden
                btnPrint5.Visibility = Visibility.Hidden

                G.Columns(GC.Price).ReadOnly = True
            End If
            HideAcc()
        Else
            HideAcc()
        End If

        btnItemsSearch.Visibility = Windows.Visibility.Hidden
        btnBalSearch.Visibility = Windows.Visibility.Hidden
        btnPrint4.Visibility = Windows.Visibility.Hidden
        'CashierId.IsEnabled = Md.Manager = 1
        ComboBox1.Visibility = Windows.Visibility.Hidden
        lblComboBox1.Visibility = Windows.Visibility.Hidden

        If Not Md.Manager Then
            btnDelete.Visibility = Windows.Visibility.Hidden
            btnFirst.Visibility = Windows.Visibility.Hidden
            btnPrevios.Visibility = Windows.Visibility.Hidden
            btnNext.Visibility = Windows.Visibility.Hidden
            btnLast.Visibility = Windows.Visibility.Hidden

            If Flag = FlagState.تحويل_إلى_مخزن Then
                btnPrint.Visibility = Windows.Visibility.Hidden
                btnPrint2.Visibility = Windows.Visibility.Hidden
            End If

            DayDate.IsEnabled = False
            Shift.IsEnabled = False
            If Md.DefaultStore > 0 Then
                StoreId.IsEnabled = False
            End If
        End If

        If Not TestSalesAndReturn() OrElse Flag = FlagState.عرض_أسعار OrElse Flag = FlagState.أمر_شراء Then
            Payed.Visibility = Windows.Visibility.Hidden
            lblPayed.Visibility = Windows.Visibility.Hidden
            Remaining.Visibility = Windows.Visibility.Hidden
            lblRemaining.Visibility = Windows.Visibility.Hidden
        End If

        If TestSalesAndReturn() OrElse TestPurchaseAndReturn() OrElse TestConsumablesAndReturn() OrElse TestImportAndReturn() Then
            lblDiscount.Visibility = Windows.Visibility.Visible
            lblDiscount_Copy.Visibility = Windows.Visibility.Visible
            DiscountPerc.Visibility = Windows.Visibility.Visible
            lblDiscount_Copy1.Visibility = Windows.Visibility.Visible
            DiscountValue.Visibility = Windows.Visibility.Visible
            lblTotalAfterDiscount.Visibility = Visibility.Visible
            TotalAfterDiscount.Visibility = Visibility.Visible
        Else
            lblDiscount.Visibility = Windows.Visibility.Hidden
            lblDiscount_Copy.Visibility = Windows.Visibility.Hidden
            DiscountPerc.Visibility = Windows.Visibility.Hidden
            lblDiscount_Copy1.Visibility = Windows.Visibility.Hidden
            DiscountValue.Visibility = Windows.Visibility.Hidden
            lblTotalAfterDiscount.Visibility = Visibility.Hidden
            TotalAfterDiscount.Visibility = Visibility.Hidden
        End If


        If Flag = FlagState.عرض_أسعار OrElse Flag = FlagState.أمر_شراء Then
            lblWaiter.Visibility = Windows.Visibility.Hidden
            WaiterId.Visibility = Windows.Visibility.Hidden
            WaiterName.Visibility = Windows.Visibility.Hidden
        End If

        If Flag <> FlagState.الاستيراد AndAlso Flag <> FlagState.مردودات_الاستيراد Then
            DocNo.Visibility = Visibility.Hidden
            lblDocNo.Visibility = Visibility.Hidden
        End If


        If Receive Then
            btnReceive.Visibility = Windows.Visibility.Visible
            btnReceiveAll.Visibility = Windows.Visibility.Visible
            btnReceiveSave.Visibility = Windows.Visibility.Visible
            StoreId.IsEnabled = False
            DayDate.IsEnabled = False
            CashierId.IsEnabled = False

            btnSave.Visibility = Windows.Visibility.Hidden
            btnDelete.Visibility = Windows.Visibility.Hidden
            btnDeleteRow.Visibility = Windows.Visibility.Hidden
            btnFirst.Visibility = Windows.Visibility.Hidden
            btnNext.Visibility = Windows.Visibility.Hidden
            btnPrevios.Visibility = Windows.Visibility.Hidden
            btnLast.Visibility = Windows.Visibility.Hidden

            btnPrint.Visibility = Windows.Visibility.Hidden
            btnPrint2.Visibility = Windows.Visibility.Hidden
            btnPrint3.Visibility = Windows.Visibility.Hidden
            btnPrint4.Visibility = Windows.Visibility.Hidden
            btnPrint5.Visibility = Windows.Visibility.Hidden

            lblLastEntry.Visibility = Windows.Visibility.Hidden
            Label1.Visibility = Windows.Visibility.Hidden
        End If

    End Sub

    Private Sub btnItemsSearch_Click(sender As Object, e As RoutedEventArgs) Handles btnItemsSearch.Click
        Try
            If G.CurrentRow Is Nothing Then G.CurrentCell = G.Rows(G.Rows.Count - 1).Cells(GC.Id)
            G.Focus()
            G.CurrentCell = G.Rows(G.CurrentRow.Index).Cells(GC.Id)
            GridKeyDown(G, New System.Windows.Forms.KeyEventArgs(Forms.Keys.F1))
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnBalSearch_Click(sender As Object, e As RoutedEventArgs) Handles btnBalSearch.Click
        Try
            If G.CurrentRow Is Nothing Then G.CurrentCell = G.Rows(G.Rows.Count - 1).Cells(GC.Id)
            G.Focus()
            G.CurrentCell = G.Rows(G.CurrentRow.Index).Cells(GC.Id)
            GridKeyDown(G, New System.Windows.Forms.KeyEventArgs(Forms.Keys.F12))
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Hide()
        lblTableId.Visibility = Windows.Visibility.Hidden
        TableId.Visibility = Windows.Visibility.Hidden
        lblTableSubId.Visibility = Windows.Visibility.Hidden
        TableSubId.Visibility = Windows.Visibility.Hidden
        lblNoOfPersons.Visibility = Windows.Visibility.Hidden
        NoOfPersons.Visibility = Windows.Visibility.Hidden
        lblMinPerPerson.Visibility = Windows.Visibility.Hidden
        MinPerPerson.Visibility = Windows.Visibility.Hidden
        CancelMinPerPerson.Visibility = Windows.Visibility.Hidden
        WithTax.Visibility = Windows.Visibility.Hidden
        WithService.Visibility = Windows.Visibility.Hidden
        ServiceValue.Visibility = Windows.Visibility.Hidden
        Taxvalue.Visibility = Windows.Visibility.Hidden
        RdoEmployees.Visibility = Windows.Visibility.Hidden
        PaymentType.Visibility = Windows.Visibility.Hidden
        ToName.Visibility = Windows.Visibility.Hidden
        ReservToId.Visibility = Windows.Visibility.Hidden
        lblToId.Visibility = Windows.Visibility.Hidden
        ToId.Visibility = Windows.Visibility.Hidden
        lblWaiter.Visibility = Windows.Visibility.Hidden
        WaiterId.Visibility = Windows.Visibility.Hidden
        WaiterName.Visibility = Windows.Visibility.Hidden
        txtFlag.Visibility = Windows.Visibility.Hidden
        TableIdName.Visibility = Windows.Visibility.Hidden
        btnCloseTable.Visibility = Windows.Visibility.Hidden
        IsClosed.Visibility = Windows.Visibility.Hidden
        IsCashierPrinted.Visibility = Windows.Visibility.Hidden
        CashierName.Visibility = Windows.Visibility.Hidden
        lblCashier.Visibility = Windows.Visibility.Hidden
        CashierId.Visibility = Windows.Visibility.Hidden
        lblPerc.Visibility = Windows.Visibility.Hidden
        lblLE.Visibility = Windows.Visibility.Hidden
        lblDeliveryman.Visibility = Windows.Visibility.Hidden
        DeliverymanId.Visibility = Windows.Visibility.Hidden
        DeliverymanName.Visibility = Windows.Visibility.Hidden
        TabItemTables.Visibility = Windows.Visibility.Hidden
        TabItemDelivery.Visibility = Windows.Visibility.Hidden

        lblAccNo.Visibility = Windows.Visibility.Hidden
        AccNo.Visibility = Windows.Visibility.Hidden
        AccName.Visibility = Windows.Visibility.Hidden

        CurrencyId.Visibility = Windows.Visibility.Hidden
        lblCurrencyId.Visibility = Windows.Visibility.Hidden

        IsCashierPrinted.Visibility = Windows.Visibility.Hidden
        btnReceive.Visibility = Windows.Visibility.Hidden
        btnReceiveAll.Visibility = Windows.Visibility.Hidden
        btnReceiveSave.Visibility = Windows.Visibility.Hidden

    End Sub

    Private Sub AccNo_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles AccNo.LostFocus
        bm.AccNoLostFocus(AccNo, AccName, , 0, True)
    End Sub

    Private Sub AccNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles AccNo.KeyUp
        bm.AccNoShowHelp(AccNo, AccName, e, , 0, True)
    End Sub

    Private Sub btnReceive_Click(sender As Object, e As RoutedEventArgs) Handles btnReceive.Click
        StoreId.Focus()
        If bm.ShowHelp("التحويلات", StoreId, StoreId, Nothing, "select cast(StoreId as varchar(100)) 'كود المخزن',dbo.GetStoreName(StoreId) 'اسم المخزن',cast(InvoiceNo as varchar(100)) 'رقم الإذن' from SalesMaster where Flag=" & Flag & " and ToId=" & Val(ToId.Text), , , , "كود المخزن", "اسم المخزن") Then
            StoreId_LostFocus(Nothing, Nothing)
            ToId.Focus()
            InvoiceNo.Text = bm.SelectedRow(2)
            InvoiceNo_Leave(Nothing, Nothing)
            InvoiceNo.IsEnabled = False
        End If
    End Sub

    Private Sub btnReceiveAll_Click(sender As Object, e As RoutedEventArgs) Handles btnReceiveAll.Click
        For i As Integer = 0 To G.Rows.Count - 1
            G.Rows(i).Cells(GC.ReceivedQty).Value = G.Rows(i).Cells(GC.Qty).Value
        Next
    End Sub

    Private Sub btnReceiveSave_Click(sender As Object, e As RoutedEventArgs) Handles btnReceiveSave.Click
        btnSave_Click(btnSave, Nothing)
    End Sub

    Private Sub ReservToId_Checked(sender As Object, e As RoutedEventArgs) Handles ReservToId.Checked, ReservToId.Unchecked
        If ((TestPurchaseAndReturn() OrElse TestImportAndReturn()) AndAlso Not ReservToId.IsChecked) OrElse (TestSalesAndReturn() AndAlso ReservToId.IsChecked) Then
            lblToId.Content = "المورد"
        ElseIf (TestSalesAndReturn() AndAlso Not ReservToId.IsChecked) OrElse ((TestPurchaseAndReturn() OrElse TestImportAndReturn()) AndAlso ReservToId.IsChecked) Then
            lblToId.Content = "العميل"
        End If
        ToId_LostFocus(Nothing, Nothing)

    End Sub

    Private Sub G_SelectionChanged(sender As Object, e As EventArgs)
        If lop Then Return
        Try
            ItemBal.Text = G.CurrentRow.Cells(GC.CurrentBal).Value
        Catch ex As Exception
            ItemBal.Clear()
        End Try
        Try
            bm.GetImage("Items", New String() {"Id"}, New String() {G.CurrentRow.Cells(GC.Id).Value}, "Image", Image1)
        Catch ex As Exception
            bm.SetNoImage(Image1)
        End Try
    End Sub

    Private Sub GetItemNameAndBal(i As Integer, Id As String)
        Dim dt As DataTable = bm.ExcuteAdapter("Select dbo.GetStoreItemBal('" & StoreId.Text & "','" & Id & "',0,'" & G.Rows(i).Cells(GC.Size).Value & "','" & bm.ToStrDate(DayDate.SelectedDate) & "')Bal,* From Items_View  where /*IsStopped=0 and*/ Id='" & Id & "' " & ItemWhere())
        Dim dr() As DataRow = dt.Select("Id='" & Id & "'")
        If dr.Length = 0 Then
            If Not G.Rows(i).Cells(GC.Id).Value Is Nothing Or G.Rows(i).Cells(GC.Id).Value <> "" Then bm.ShowMSG("هذا الصنف غير موجود")
            ClearRow(i)
            CalcTotal()
            Return
        End If
        G.Rows(i).Cells(GC.Id).Value = dr(0)(GC.Id)
        G.Rows(i).Cells(GC.Name).Value = dr(0)(GC.Name)
        G.Rows(i).Cells(GC.CurrentBal).Value = dr(0)("Bal")
        G_SelectionChanged(Nothing, Nothing)

    End Sub


End Class
