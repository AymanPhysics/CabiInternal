Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO
Imports System.Windows.Threading
Imports System.Threading.Tasks

Public Class CallCenter
    Dim bm As New BasicMethods
    
    Public Flag As Integer = 0
    Dim dv As New DataView 

    Dim _CurrentLine As Integer = 0
    Public Property CurrentLine As Integer
        Set(value As Integer)
            _CurrentLine = value
            lblCurrentLine.Content = IIf(_CurrentLine = 0, "", _CurrentLine)
            Dim dt As DataTable = bm.ExcuteAdapter("select * from dbo.GetCallCenterNewState(" & _CurrentLine & ")")
            btnChangeState.Visibility = Windows.Visibility.Hidden
            If dt.Rows.Count > 0 Then
                If Flag <> 1 Then btnChangeState.Visibility = Windows.Visibility.Visible
                btnChangeState.Tag = dt.Rows(0)(0)
                btnChangeState.Content = dt.Rows(0)(1)
            End If

        End Set
        Get
            Return _CurrentLine
        End Get
    End Property
    Public MyCallerId As String = ""


    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        If Flag = 1 OrElse Flag = 3 Then btnChangeState.Visibility = Visibility.Collapsed
        If Flag = 1 OrElse Flag = 2 Then btnEvaluate.Visibility = Visibility.Collapsed
        If Flag = 2 OrElse Flag = 3 Then OldOrders.Visibility = Visibility.Collapsed

        lblDeliveryPrice.Visibility = Visibility.Hidden
        DeliveryPrice.Visibility = Visibility.Hidden
        btnGetDeliveryMan.Visibility = Windows.Visibility.Hidden

        'btnViewHistory.Visibility = Windows.Visibility.Hidden

        bm.FillCombo("CallCenterCategories", CategoryId, "")
        bm.FillCombo("CallCenterCategories", SearchCategoryId, "")
        CategoryId.SelectedValue = 1
        CategoryId_LostFocus(Nothing, Nothing)

        bm.FillCombo("KnownUsTypes", KnownUsTypeId, "")

        bm.FillCombo("HalfHourIndex", WaitingIndex, "", , True)
        bm.FillCombo("HoursAll", HH, "", , True, True)
        bm.FillCombo("MinutesAll", MM, "", , True, True)

        bm.FillCombo("Shifts", Shift, "")

        bm.Addcontrol_MouseDoubleClick({EmpId, CallerId, CarOtherId})
        DayDate.SelectedDate = Md.CurrentDate
        Shift.SelectedValue = Md.CurrentShiftId
        OrderDate.SelectedDate = DayDate.SelectedDate
        'OrderDate.Visibility = Windows.Visibility.Collapsed

        EmpId.Text = Md.UserName
        EmpId_LostFocus(Nothing, Nothing)

        'DayDate.IsEnabled = False
        EmpId.IsEnabled = False
        StartTime.Content = bm.MyGetTime

        HH.SelectedValue = Val(bm.GetDate.Hour)
        MM.SelectedIndex = 0

        If Not Md.Manager Then btnCancel.Visibility = Windows.Visibility.Hidden

        CallerId.Text = MyCallerId
        CallerId_LostFocus(Nothing, Nothing)
         
        SetFalg()
        If Flag <> 1 Then
            btnCancel.IsEnabled = False
            btnEvaluate.IsEnabled = False
            'btnChangeState.IsEnabled = False
            btnPrint.IsEnabled = False
            btnPrint2.IsEnabled = False
            btnSave.IsEnabled = False
        End If

        ServiceAmountPre.IsEnabled = Md.Manager

        FillList()
        SetTimer()
        CarId_LostFocus(Nothing, Nothing)
    End Sub

    'Private Sub FillList()
    '    If Flag = 1 Then Return
    '    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(
    '        Async Sub()
    '            Try
    '                dv.Table = Await bm.ExcuteAdapterAsync("GetCallCenterList", {"Flag"}, {Flag})
    '                'Dim x
    '                'If Not MyGrid.CurrentItem Is Nothing Then x = MyGrid.CurrentItem(0)
    '                MyGrid.DataContext = dv
    '                'If Not x Is Nothing Then
    '                '    For i As Integer = 0 To MyGrid.Items.Count - 1
    '                '        If MyGrid.CurrentItem(0) = x Then
    '                '            MyGrid.CurrentCell = MyGrid.Items(i)("Line")
    '                '            MyGrid.CurrentCellInfo = New GridViewCellInfo(GridView.Items(5), GridView.Columns("Text"))
    '                '            MyGrid.Focus()
    '                '            Exit For
    '                '        End If
    '                '    Next
    '                'End If
    '                MyGrid.Columns(3).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(4).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(5).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(6).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(7).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(8).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(9).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(10).Visibility = Visibility.Collapsed
    '                MyGrid.Columns(11).Visibility = Visibility.Collapsed
    '                MyGrid.IsReadOnly = True
    '                MyGrid.CanUserAddRows = False
    '                MyGrid.CanUserDeleteRows = False
    '                MyGrid.CanUserSortColumns = False

    '                SearchInvoiceNo_TextChanged(Nothing, Nothing)
    '            Catch
    '            End Try
    '        End Sub))
    'End Sub

    Private Sub FillList()
        'If Flag = 1 Then Return
        Try
            dv.Table = bm.ExcuteAdapter("GetCallCenterList", {"Flag"}, {Flag})
            MyGrid.DataContext = dv
            MyGrid.Visibility = Windows.Visibility.Visible

            MyGrid.IsReadOnly = True
            MyGrid.CanUserAddRows = False
            MyGrid.CanUserDeleteRows = False
            MyGrid.CanUserSortColumns = False
            SearchInvoiceNo_TextChanged(Nothing, Nothing)
        Catch
        End Try
    End Sub


    Sub SetTimer()
        Dim MyTimer As New Timers.Timer(1000)
        AddHandler MyTimer.Elapsed, Sub(sender As Object, e As Timers.ElapsedEventArgs)
                                        Try
                                            MyTimer.Interval = 180000
                                            MyTimer.Stop()
                                            'FillList()
                                            BackgroundWorker1.RunWorkerAsync()
                                            MyTimer.Start()
                                        Catch
                                        End Try
                                    End Sub
        MyTimer.Start()
    End Sub

    Sub SetFalg()
        Select Case Flag
            Case 1
                
                'MyGrid.Visibility = Visibility.Collapsed
                lblDeliverymanId.Visibility = Windows.Visibility.Hidden
                DeliverymanId.Visibility = Windows.Visibility.Hidden
                DeliverymanOther.Visibility = Windows.Visibility.Hidden
                CarId.Visibility = Windows.Visibility.Hidden
                'btnRefresh.Visibility = Windows.Visibility.Hidden

                'lblSearchInvoiceNo.Visibility = Windows.Visibility.Hidden
                'SearchInvoiceNo.Visibility = Windows.Visibility.Hidden
                'lblSearchMob.Visibility = Windows.Visibility.Hidden
                'SearchMob.Visibility = Windows.Visibility.Hidden
                'lblSearchName.Visibility = Windows.Visibility.Hidden
                'SearchName.Visibility = Windows.Visibility.Hidden

                'ServiceAmount.IsReadOnly = True
            Case 2
                'CategoryId.IsEnabled = False
                'SubCategoryId.IsEnabled = False
                'CallerId.IsReadOnly = True
                'CallerName.IsReadOnly = True
                'Address.IsReadOnly = True
                'Notes.IsReadOnly = True

                'MobFrom.IsReadOnly = True
                'AddressFrom.IsReadOnly = True
                'MobTo.IsReadOnly = True
                'AddressTo.IsReadOnly = True
                'MobTo2.IsReadOnly = True
                'AddressTo2.IsReadOnly = True
                'ServiceAmount.IsReadOnly = True
                'OrderAmount.IsReadOnly = True
                'btnCopy.IsEnabled = False
                'btnCopy1.IsEnabled = False
                'btnCopy2.IsEnabled = False
            Case 3
                
                'CategoryId.IsEnabled = False
                'SubCategoryId.IsEnabled = False
                'CallerId.IsReadOnly = True
                'CallerName.IsReadOnly = True
                'Address.IsReadOnly = True
                'Notes.IsReadOnly = True

                'MobFrom.IsReadOnly = True
                'AddressFrom.IsReadOnly = True
                'MobTo.IsReadOnly = True
                'AddressTo.IsReadOnly = True
                'MobTo2.IsReadOnly = True
                'AddressTo2.IsReadOnly = True
                'ServiceAmount.IsReadOnly = True
                'OrderAmount.IsReadOnly = True
                'btnCopy.IsEnabled = False
                'btnCopy1.IsEnabled = False
                'btnCopy2.IsEnabled = False
        End Select
    End Sub

    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        lblEmpId.SetResourceReference(Label.ContentProperty, "Employee")
        'lblDeliverymanId.SetResourceReference(Label.ContentProperty, "Deliveryman")
        lblDayDate.SetResourceReference(Label.ContentProperty, "DayDate")
        lblOrderDate.SetResourceReference(Label.ContentProperty, "OrderDate")

        lblStartTime.SetResourceReference(Label.ContentProperty, "Start Time")
        lblEndTime.SetResourceReference(Label.ContentProperty, "End Time")
        lblCategoryId.SetResourceReference(Label.ContentProperty, "Group")
        lblTripPriceId.SetResourceReference(Label.ContentProperty, "TripType")
        lblCallerId.SetResourceReference(Label.ContentProperty, "Caller Tel")
        lblCallerName.SetResourceReference(Label.ContentProperty, "Caller Name")
        lblAddress.SetResourceReference(Label.ContentProperty, "Address")
        lblNotes.SetResourceReference(Label.ContentProperty, "Details")
        'btnViewHistory.SetResourceReference(Button.ContentProperty, "View History")

        lblAddressFrom.SetResourceReference(Label.ContentProperty, "From")
        lblAddressTo.SetResourceReference(Label.ContentProperty, "To")
        lblAddressTo2.SetResourceReference(Label.ContentProperty, "To2")
        lblServiceAmount.SetResourceReference(Label.ContentProperty, "ServiceAmount")
        'lblOrderAmount.SetResourceReference(Label.ContentProperty, "OrderAmount")
        lblTotalAmount.SetResourceReference(Label.ContentProperty, "TotalAmount")
        lblDeliveryPrice.SetResourceReference(Label.ContentProperty, "DeliveryPrice")
        btnCopy.SetResourceReference(Button.ContentProperty, "CopyFromCustomer")
        btnCopy1.SetResourceReference(Button.ContentProperty, "CopyFromCustomer")
        btnCopy2.SetResourceReference(Button.ContentProperty, "CopyFromCustomer")
        btnPrint.SetResourceReference(Button.ContentProperty, "Print 1")
        btnPrint2.SetResourceReference(Button.ContentProperty, "Print 2")
        lblKnownUsTypeId.SetResourceReference(Button.ContentProperty, "KnownUsTypeId")

        lblServiceAmountPre.SetResourceReference(Button.ContentProperty, "ServiceAmountPre")
        lblKMCount.SetResourceReference(Button.ContentProperty, "KMCount")
        lblKMPrice.SetResourceReference(Button.ContentProperty, "KMPrice")
        lblWaiting.SetResourceReference(Button.ContentProperty, "Waiting")
        lblMoneyTransfer.SetResourceReference(Button.ContentProperty, "Money Transfer Cost")
    End Sub

    Public Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        If CategoryId.SelectedIndex < 1 Then
            bm.ShowMSG("Please, Choose a Category ..")
            CategoryId.Focus()
            Return
        End If
        If SubCategoryId.SelectedIndex < 1 Then
            bm.ShowMSG("Please, Choose a Sub Category ..")
            SubCategoryId.Focus()
            Return
        End If
        If TripPriceId.SelectedIndex < 1 Then
            bm.ShowMSG("Please, Choose a Type of Trip ..")
            TripPriceId.Focus()
            Return
        End If

        If CallerId.Text.Trim.Length < 7 Then
            bm.ShowMSG("Please, Insert a Mobile No..")
            CallerId.Focus()
            Return
        End If

        If KnownUsTypeId.Visibility = Windows.Visibility.Visible AndAlso KnownUsTypeId.SelectedIndex < 1 Then
            bm.ShowMSG("Please, Choose Known Us..")
            KnownUsTypeId.Focus()
            Return
        End If

        If CarOtherId.Visibility = Windows.Visibility.Visible AndAlso Val(CarOtherId.Text) = 0 Then
            bm.ShowMSG("Please, Choose Ecternal..")
            CarOtherId.Focus()
            Return
        End If

        If CurrentLine = 0 Then EndTime.Content = bm.MyGetTime
        CallerId.Text = CallerId.Text.Trim.Replace("'", "''")
        CallerName.Text = CallerName.Text.Trim.Replace("'", "''")
        Address.Text = Address.Text.Trim.Replace("'", "''")
        Notes.Text = Notes.Text.Trim.Replace("'", "''")
        MobFrom.Text = MobFrom.Text.Trim.Replace("'", "''")
        NameFrom.Text = NameFrom.Text.Trim.Replace("'", "''")
        AddressFrom.Text = AddressFrom.Text.Trim.Replace("'", "''")
        MobTo.Text = MobTo.Text.Trim.Replace("'", "''")
        NameTo.Text = NameTo.Text.Trim.Replace("'", "''")
        AddressTo.Text = AddressTo.Text.Trim.Replace("'", "''")
        MobTo2.Text = MobTo2.Text.Trim.Replace("'", "''")
        NameTo2.Text = NameTo2.Text.Trim.Replace("'", "''")
        AddressTo2.Text = AddressTo2.Text.Trim.Replace("'", "''")
        ServiceAmountPre.Text = ServiceAmountPre.Text.Trim.Replace("'", "''")
        ServiceAmount.Text = ServiceAmount.Text.Trim.Replace("'", "''")
        KMCount.Text = KMCount.Text.Trim.Replace("'", "''")
        KMPrice.Text = KMPrice.Text.Trim.Replace("'", "''")
        WaitingValue.Text = WaitingValue.Text.Trim.Replace("'", "''")
        MoneyTransfer.Text = MoneyTransfer.Text.Trim.Replace("'", "''")
        OrderAmount.Text = OrderAmount.Text.Trim.Replace("'", "''")
        TotalAmount.Text = TotalAmount.Text.Trim.Replace("'", "''")
        DeliveryPrice.Text = DeliveryPrice.Text.Trim.Replace("'", "''")
        DeliverymanOther.Text = DeliverymanOther.Text.Trim.Replace("'", "''")
        Payment1.Text = Val(Payment1.Text)
        Payment2.Text = Val(Payment2.Text)
        Payment3.Text = Val(Payment3.Text)

        If CurrentLine = 0 Then
            Dim NewLine As String = "dbo.GetCallCenterNewLine()"
            'If DayDate.SelectedDate <> OrderDate.SelectedDate Then NewLine = "dbo.GetCallCenterNewLinePending()"
            CurrentLine = Val(bm.ExecuteScalar("declare @new bigint=" & NewLine & "  insert CallCenter(Line,EmpId,DayDate,OrderDate,HH,MM,Shift,StartTime,EndTime,CategoryId,SubCategoryId,TripPriceId,DeliveryManId,DeliverymanOther,CarId,CarOtherId,CallerId,CallerName,Address,Notes,IsDelivered,IsEvaluated,MobFrom,NameFrom,AddressFrom,MobTo,NameTo,AddressTo,MobTo2,NameTo2,AddressTo2,ServiceAmount,ServiceAmountPre,KMCount,KMPrice,WaitingIndex,WaitingValue,MoneyTransfer,OrderAmount,TotalAmount,DeliveryPrice,Payment1,Payment2,Payment3,KnownUsTypeId,KnownUsTypeNotes,UserName,MyGetDate) select @new,'" & EmpId.Text & "','" & bm.ToStrDate(DayDate.SelectedDate) & "','" & bm.ToStrDate(OrderDate.SelectedDate) & "','" & HH.SelectedValue & "','" & MM.SelectedValue & "','" & Val(Shift.SelectedValue.ToString) & "','" & StartTime.Content & "','" & EndTime.Content & "','" & Val(CategoryId.SelectedValue) & "','" & Val(SubCategoryId.SelectedValue) & "','" & Val(TripPriceId.SelectedValue) & "','" & Val(DeliverymanId.SelectedValue) & "','" & DeliverymanOther.Text & "','" & Val(CarId.SelectedValue) & "','" & Val(CarOtherId.Text) & "','" & CallerId.Text & "','" & CallerName.Text & "','" & Address.Text & "','" & Notes.Text & "',0,0,'" & MobFrom.Text & "','" & NameFrom.Text & "','" & AddressFrom.Text & "','" & MobTo.Text & "','" & NameTo.Text & "','" & AddressTo.Text & "','" & MobTo2.Text & "','" & NameTo2.Text & "','" & AddressTo2.Text & "','" & ServiceAmount.Text & "','" & ServiceAmountPre.Text & "','" & KMCount.Text & "','" & KMPrice.Text & "','" & WaitingIndex.SelectedValue.ToString & "','" & WaitingValue.Text & "','" & MoneyTransfer.Text & "','" & OrderAmount.Text & "','" & TotalAmount.Text & "','" & DeliveryPrice.Text & "','" & Payment1.Text & "','" & Payment2.Text & "','" & Payment3.Text & "','" & KnownUsTypeId.SelectedValue & "','" & KnownUsTypeNotes.Text & "','" & Md.UserName & "',GetDate()  select @new"))
            If CurrentLine <> 0 Then
                If Not (sender Is btnPrint OrElse sender Is btnPrint2) AndAlso Not Parent Is Nothing Then
                    Try
                        bm.ShowMSG("Done Successfuly")
                        CType(Parent, Window).Close()
                    Catch ex As Exception
                    End Try
                End If
            End If
        Else
            Dim Str As String = "update CallCenter set "
            Dim Str2 As String = " select " & CurrentLine
            If CurrentLine < 0 AndAlso DeliverymanId.SelectedValue <> 0 Then
                Str = "declare @new bigint=dbo.GetCallCenterNewLine() update CallCenter set Line=@new, "
                Str2 = " select @new"
            End If
            CurrentLine = bm.ExecuteScalar(Str & " DayDate='" & bm.ToStrDate(DayDate.SelectedDate) & "',HH='" & HH.SelectedValue & "',MM='" & MM.SelectedValue & "',DeliveryManId='" & Val(DeliverymanId.SelectedValue) & "',DeliverymanOther='" & DeliverymanOther.Text & "',CarId='" & Val(CarId.SelectedValue) & "',CarOtherId='" & Val(CarOtherId.Text) & "',StartDeliveryTime=isnull(StartDeliveryTime,GetDate()),CategoryId='" & Val(CategoryId.SelectedValue) & "',SubCategoryId='" & Val(SubCategoryId.SelectedValue) & "',TripPriceId='" & Val(TripPriceId.SelectedValue) & "',CallerId='" & CallerId.Text & "',CallerName='" & CallerName.Text & "',Address='" & Address.Text & "',Notes='" & Notes.Text & "',MobFrom='" & MobFrom.Text & "',NameFrom='" & NameFrom.Text & "',AddressFrom='" & AddressFrom.Text & "',MobTo='" & MobTo.Text & "',NameTo='" & NameTo.Text & "',AddressTo='" & AddressTo.Text & "',MobTo2='" & MobTo2.Text & "',NameTo2='" & NameTo2.Text & "',AddressTo2='" & AddressTo2.Text & "',ServiceAmount='" & ServiceAmount.Text & "',ServiceAmountPre='" & ServiceAmountPre.Text & "',KMCount='" & KMCount.Text & "',KMPrice='" & KMPrice.Text & "',WaitingIndex='" & WaitingIndex.SelectedValue.ToString & "',WaitingValue='" & WaitingValue.Text & "',MoneyTransfer='" & MoneyTransfer.Text & "',OrderAmount='" & OrderAmount.Text & "',TotalAmount='" & TotalAmount.Text & "',DeliveryPrice='" & DeliveryPrice.Text & "',Payment1='" & Payment1.Text & "',Payment2='" & Payment2.Text & "',Payment3='" & Payment3.Text & "',KnownUsTypeId='" & KnownUsTypeId.SelectedValue & "',KnownUsTypeNotes='" & KnownUsTypeNotes.Text & "',UserName='" & Md.UserName & "',MyGetDate=GetDate() where Line=" & CurrentLine & Str2)
            If Not (sender Is btnPrint OrElse sender Is btnPrint2 OrElse sender Is btnChangeState) Then btnbtnNew_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        If bm.ShowHelp("Employees", EmpId, EmpName, e, "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees") Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        If Val(EmpId.Text.Trim) = 0 Then
            EmpId.Clear()
            EmpName.Clear()
            Return
        End If
        bm.LostFocus(EmpId, EmpName, "select Name from Employees where Id=" & EmpId.Text.Trim())
    End Sub

    Private Sub CallerId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CallerId.LostFocus
        Dim dt As DataTable = bm.ExcuteAdapter("select top 1 * from CallCenter where CallerId='" & CallerId.Text.Trim & "' order by mygetdate desc")
        OldOrders.DataContext = Nothing
        If dt.Rows.Count > 0 Then
            CallerName.Text = dt.Rows(0)("CallerName")
            Address.Text = dt.Rows(0)("Address")
            Notes.Focus()
        End If
        FillBalance()
        FilOldOrders()
    End Sub

    Private Sub btnViewHistory_Click(sender As Object, e As RoutedEventArgs) Handles btnViewHistory.Click
        'Dim rpt As New ReportViewer
        'rpt.Header = CType(Parent, MyWindow).Title
        'rpt.paraname = New String() {"@FromDate", "@ToDate", "@EmpId", "@CategoryId", "@CallerId", "@DeliverymanId", "Header", "@Line", "@Cancel", "@Shift"}
        'rpt.paravalue = New String() {New DateTime(1900, 1, 1, 0, 0, 0), New DateTime(1900, 1, 1, 0, 0, 0), 0, 0, CallerId.Text.Trim, 0, "Call Center", IIf(sender Is btnPrint OrElse sender Is btnPrint2, CurrentLine, 0), 0, 0}
        'rpt.Rpt = "CallCenter.rpt"
        'rpt.Show()

        Dim rpt As New ReportViewer
        If TypeOf (Parent) Is MyWindow Then
            rpt.Header = CType(Parent, MyWindow).Title
        ElseIf TypeOf (Parent) Is Window Then
            rpt.Header = CType(Parent, Window).Title
        End If

        rpt.paraname = New String() {"@CallerId", "Header"}
        rpt.paravalue = New String() {CallerId.Text.Trim, btnViewHistory.Content}
        rpt.Rpt = "CallerBalance.rpt"
        rpt.Show()
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click, btnPrint2.Click
         If sender Is btnPrint OrElse sender Is btnPrint2 Then
            btnSave_Click(sender, Nothing)
            If CurrentLine = 0 Then Return
        End If
        Dim rpt As New ReportViewer
        If TypeOf (Parent) Is MyWindow Then
            rpt.Header = CType(Parent, MyWindow).Title
        ElseIf TypeOf (Parent) Is Window Then
            rpt.Header = CType(Parent, Window).Title
        End If

        rpt.paraname = New String() {"@FromDate", "@ToDate", "@EmpId", "@DeliverymanId", "@CategoryId", "@CarId", "@CarOtherId", "@CallerId", "Header", "@Line", "@Cancel", "@Shift"}
        rpt.paravalue = New String() {New DateTime(1900, 1, 1, 0, 0, 0), New DateTime(1900, 1, 1, 0, 0, 0), 0, 0, 0, 0, 0, CallerId.Text.Trim, "Call Center", IIf(sender Is btnPrint OrElse sender Is btnPrint2, CurrentLine, 0), 0, 0}
        rpt.Rpt = "CallCenter.rpt"
        rpt.Print(, , IIf(sender Is btnPrint2, 2, 1))
    End Sub

    Private Sub CategoryId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CategoryId.LostFocus
        bm.FillCombo("CallCenterSubCategories", SubCategoryId, " where CategoryId=" & Val(CategoryId.SelectedValue))
        If SubCategoryId.Items.Count = 2 Then SubCategoryId.SelectedIndex = 1

        bm.FillCombo("TripPrices", TripPriceId, " where CategoryId=" & Val(CategoryId.SelectedValue))
        If TripPriceId.Items.Count = 2 Then TripPriceId.SelectedIndex = 1
        TripPriceId_LostFocus(Nothing, Nothing)

        bm.FillCombo("select Id,Name,dbo.GetCarState(CategoryId,Id)State From Cars where CategoryId=" & Val(CategoryId.SelectedValue) & " union select 0 Id,'-' Name,'#FFFFFFFF' union select -1 Id,'External' Name,'#FFFFFFFF'", CarId)
        
        bm.FillCombo("select Id,Name+' '+Mobile+'-'+HomePhone Name,dbo.GetEmpState(CategoryId,Id)State From Employees where Deliveryman=1 and Stopped=0 and CategoryId=" & Val(CategoryId.SelectedValue) & " union select 0 Id,'-' Name,'#FFFFFFFF' union select -1 Id,'Other' Name,'#FFFFFFFF'", DeliverymanId)
        DeliverymanId.SelectedValue = 0

        CarId_LostFocus(Nothing, Nothing)
        CarOtherId_LostFocus(Nothing, Nothing)
    End Sub

    Private Sub btnChangeState_Click(sender As Object, e As RoutedEventArgs) Handles btnChangeState.Click
        If Val(DeliverymanId.SelectedValue) = 0 Then
            bm.ShowMSG("Please, Select a deliveryman..")
            DeliverymanId.Focus()
            Return
        End If
        If CarId.Items.Count > 2 AndAlso Val(CarId.SelectedValue) = 0 Then
            bm.ShowMSG("Please, Select a Car..")
            CarId.Focus()
            Return
        End If
        If Val(CarId.SelectedValue) = -1 AndAlso Val(CarOtherId.Text) = 0 Then
            bm.ShowMSG("Please, Select an External Car..")
            CarOtherId.Focus()
            Return
        End If
        If CurrentLine > 0 AndAlso bm.ShowDeleteMSG(btnChangeState.Content) Then
            btnSave_Click(btnChangeState, Nothing)

            If Val(bm.ExecuteScalar("select max(Id) from OrderStates")) = Val(btnChangeState.Tag) Then
                Dim frm As New KMHelp With {.Line = CurrentLine}
                frm.ShowDialog()
                If Not frm.IsSaved Then Return
            End If

            'bm.ExcuteNonQuery("update CallCenter set UserNameDelivered=" & Md.UserName & ",IsDelivered=1,DeliveredTime=GETDATE() where Line=" & CurrentLine) ' sender.Tag
            bm.ExcuteNonQuery("insert CallCenterState(Line,State,UserName,MyGetDate) select " & CurrentLine & "," & btnChangeState.Tag & "," & Md.UserName & ",GETDATE()")
            FillList()
            'btnbtnNew_Click(Nothing, Nothing)
            GetData(CurrentLine)
        End If
    End Sub

    Private Sub btnBindingEvaluation_Click(sender As Object, e As RoutedEventArgs) Handles btnEvaluate.Click
        Dim MyContent As New CallCenterEvaluation With {.CurrentLine = CurrentLine} 'sender.Tag
        Dim wn As New MyWindow With {.Title = "Evaluation", .WindowState = WindowState.Maximized}
        wn.Content = MyContent
        wn.ShowDialog()
        FillList()
    End Sub

    Private Sub GetData(i As Integer)
        Dim dt As DataTable = bm.ExcuteAdapter("select * from CallCenter where Line=" & i)
        CurrentLine = dt.Rows(0)("Line")
        EmpId.Text = dt.Rows(0)("EmpId")

        DayDate.SelectedDate = dt.Rows(0)("DayDate")
        OrderDate.SelectedDate = dt.Rows(0)("OrderDate")
        HH.SelectedValue = dt.Rows(0)("HH")
        MM.SelectedValue = dt.Rows(0)("MM")
        Shift.SelectedValue = dt.Rows(0)("Shift")
        StartTime.Content = dt.Rows(0)("StartTime")
        EndTime.Content = dt.Rows(0)("EndTime")
        CategoryId.SelectedValue = dt.Rows(0)("CategoryId")
        CategoryId_LostFocus(Nothing, Nothing)
        SubCategoryId.SelectedValue = dt.Rows(0)("SubCategoryId")
        TripPriceId.SelectedValue = dt.Rows(0)("TripPriceId")
        DeliverymanId.SelectedValue = dt.Rows(0)("DeliverymanId").ToString
        DeliverymanId_LostFocus(Nothing, Nothing)
        DeliverymanOther.Text = dt.Rows(0)("DeliverymanOther")
        CarId.SelectedValue = dt.Rows(0)("CarId")
        CarId_LostFocus(Nothing, Nothing)
        CarOtherId.Text = dt.Rows(0)("CarOtherId")
        CarOtherId_LostFocus(Nothing, Nothing)
        CallerId.Text = dt.Rows(0)("CallerId").ToString
        CallerName.Text = dt.Rows(0)("CallerName").ToString
        FillBalance()
        Address.Text = dt.Rows(0)("Address").ToString
        Notes.Text = dt.Rows(0)("Notes").ToString
        MobFrom.Text = dt.Rows(0)("MobFrom").ToString
        NameFrom.Text = dt.Rows(0)("NameFrom").ToString
        AddressFrom.Text = dt.Rows(0)("AddressFrom").ToString
        MobTo.Text = dt.Rows(0)("MobTo").ToString
        NameTo.Text = dt.Rows(0)("NameTo").ToString
        AddressTo.Text = dt.Rows(0)("AddressTo").ToString
        MobTo2.Text = dt.Rows(0)("MobTo2").ToString
        NameTo2.Text = dt.Rows(0)("NameTo2").ToString
        AddressTo2.Text = dt.Rows(0)("AddressTo2").ToString
        ServiceAmount.Text = dt.Rows(0)("ServiceAmount").ToString
        ServiceAmountPre.Text = dt.Rows(0)("ServiceAmountPre").ToString
        KMCount.Text = dt.Rows(0)("KMCount").ToString
        KMPrice.Text = dt.Rows(0)("KMPrice").ToString
        WaitingIndex.SelectedValue = dt.Rows(0)("WaitingIndex")
        WaitingValue.Text = dt.Rows(0)("WaitingValue").ToString
        MoneyTransfer.Text = dt.Rows(0)("MoneyTransfer").ToString
        OrderAmount.Text = dt.Rows(0)("OrderAmount").ToString
        TotalAmount.Text = dt.Rows(0)("TotalAmount").ToString
        DeliveryPrice.Text = dt.Rows(0)("DeliveryPrice").ToString
        Payment1.Text = dt.Rows(0)("Payment1").ToString
        Payment2.Text = dt.Rows(0)("Payment2").ToString
        KnownUsTypeId.SelectedValue = dt.Rows(0)("KnownUsTypeId")
        KnownUsTypeNotes.Text = dt.Rows(0)("KnownUsTypeNotes").ToString

        btnCancel.IsEnabled = True
        If CurrentLine > 0 Then
            btnEvaluate.IsEnabled = True
            'btnChangeState.IsEnabled = True
        End If
        btnPrint.IsEnabled = True
        btnPrint2.IsEnabled = True
        btnSave.IsEnabled = True

        FilOldOrders()
        EnableDisableControls(False)
    End Sub

    Private Sub GetDataForOldOrder(i As Integer)
        If i <= 0 Then Return
        Dim MyDt As DataTable = bm.ExcuteAdapter("select * from CallCenter where Line=" & i)

        CategoryId.SelectedValue = MyDt.Rows(0)("CategoryId")
        CategoryId_LostFocus(Nothing, Nothing)
        SubCategoryId.SelectedValue = MyDt.Rows(0)("SubCategoryId")
        Notes.Text = MyDt.Rows(0)("Notes").ToString
        MobFrom.Text = MyDt.Rows(0)("MobFrom").ToString
        NameFrom.Text = MyDt.Rows(0)("NameFrom").ToString
        AddressFrom.Text = MyDt.Rows(0)("AddressFrom").ToString
        MobTo.Text = MyDt.Rows(0)("MobTo").ToString
        NameTo.Text = MyDt.Rows(0)("NameTo").ToString
        AddressTo.Text = MyDt.Rows(0)("AddressTo").ToString
        MobTo2.Text = MyDt.Rows(0)("MobTo2").ToString
        NameTo2.Text = MyDt.Rows(0)("NameTo2").ToString
        AddressTo2.Text = MyDt.Rows(0)("AddressTo2").ToString
        ServiceAmount.Text = MyDt.Rows(0)("ServiceAmount").ToString
        ServiceAmountPre.Text = MyDt.Rows(0)("ServiceAmountPre").ToString
        KMCount.Text = MyDt.Rows(0)("KMCount").ToString
        KMPrice.Text = MyDt.Rows(0)("KMPrice").ToString
        WaitingIndex.SelectedValue = MyDt.Rows(0)("WaitingIndex")
        WaitingValue.Text = MyDt.Rows(0)("WaitingValue").ToString
        MoneyTransfer.Text = MyDt.Rows(0)("MoneyTransfer").ToString
        OrderAmount.Text = MyDt.Rows(0)("OrderAmount").ToString
        Payment1.Text = MyDt.Rows(0)("Payment1").ToString
        Payment2.Text = MyDt.Rows(0)("Payment2").ToString
        Payment3.Text = MyDt.Rows(0)("Payment3").ToString
        KnownUsTypeId.SelectedValue = MyDt.Rows(0)("KnownUsTypeId")
        KnownUsTypeNotes.Text = MyDt.Rows(0)("KnownUsTypeNotes").ToString

    End Sub

    Private Sub btnCopy_Click(sender As Object, e As RoutedEventArgs) Handles btnCopy.Click
        MobFrom.Text = CallerId.Text
        NameFrom.Text = CallerName.Text
        AddressFrom.Text = Address.Text
    End Sub

    Private Sub btnCopy1_Click(sender As Object, e As RoutedEventArgs) Handles btnCopy1.Click
        MobTo.Text = CallerId.Text
        NameTo.Text = CallerName.Text
        AddressTo.Text = Address.Text
    End Sub

    Private Sub btnCopy2_Click(sender As Object, e As RoutedEventArgs) Handles btnCopy2.Click
        MobTo2.Text = CallerId.Text
        NameTo2.Text = CallerName.Text
        AddressTo2.Text = Address.Text
    End Sub

    Private Sub ServiceAmount_TextChanged(sender As Object, e As TextChangedEventArgs) Handles ServiceAmount.TextChanged, OrderAmount.TextChanged
        TotalAmount.Text = Val(ServiceAmount.Text) + Val(OrderAmount.Text)
    End Sub

    Private Sub txtID_KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles ServiceAmount.KeyDown, ServiceAmountPre.KeyDown, KMCount.KeyDown, KMPrice.KeyDown, WaitingValue.KeyDown, MoneyTransfer.KeyDown, OrderAmount.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub

    Private Sub SearchInvoiceNo_TextChanged(sender As Object, e As TextChangedEventArgs) Handles SearchInvoiceNo.TextChanged, SearchMob.TextChanged, SearchName.TextChanged
        Try
            dv.RowFilter = "MyLine like '%" & SearchInvoiceNo.Text.Trim & "%' and CallerId like '%" & SearchMob.Text.Trim & "%' and CallerName like '%" & SearchName.Text.Trim & "%'"
            If Not SearchDayDate.SelectedDate Is Nothing Then
                dv.RowFilter &= " and DayDate='" & bm.ToStrDate(SearchDayDate.SelectedDate) & "'"
            End If
            If SearchCategoryId.SelectedValue > 0 Then
                dv.RowFilter &= " and CategoryId=" & SearchCategoryId.SelectedValue
            End If

        Catch
        End Try
    End Sub

    Private Sub TripPriceId_LostFocus(sender As Object, e As RoutedEventArgs) Handles TripPriceId.LostFocus
        Dim MyDt As DataTable = bm.ExcuteAdapter("select * from TripPrices where Id=" & Val(TripPriceId.SelectedValue) & " and CategoryId=" & Val(CategoryId.SelectedValue))
        If MyDt.Rows.Count > 0 Then
            'ServiceAmount.Text = dt.Rows(0)("Price")
            ServiceAmountPre.Text = MyDt.Rows(0)("Price")
            KMPrice.Text = MyDt.Rows(0)("AdditionalKM")

            DeliveryPrice.Text = MyDt.Rows(0)("DeliveryPrice")
            Payment1.Text = ServiceAmount.Text
            Payment2.Text = 0
            Payment3.Text = 0
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        If bm.ShowDeleteMSG("هل تريد الإلغاء؟") Then
            bm.ExcuteNonQuery("update CallCenter set UserNameCancel=" & Md.UserName & ",Cancel=1,CancelDate=GetDate() where Line=" & CurrentLine)
            FillList()
        End If
    End Sub

    Private Sub MyGrid_AutoGeneratedColumns(sender As Object, e As EventArgs) Handles MyGrid.AutoGeneratedColumns
        Dim s As Integer = MyGrid.Columns.Count
        MyGrid.Columns(5).Visibility = Visibility.Collapsed
        MyGrid.Columns(7).Visibility = Visibility.Collapsed
        MyGrid.Columns(8).Visibility = Visibility.Collapsed
        MyGrid.Columns(9).Visibility = Visibility.Collapsed
        MyGrid.Columns(10).Visibility = Visibility.Collapsed
        MyGrid.Columns(11).Visibility = Visibility.Collapsed
        MyGrid.Columns(12).Visibility = Visibility.Collapsed
        MyGrid.Columns(13).Visibility = Visibility.Collapsed
        MyGrid.Columns(14).Visibility = Visibility.Collapsed
        MyGrid.Columns(15).Visibility = Visibility.Collapsed
        MyGrid.Columns(17).Visibility = Visibility.Collapsed
    End Sub

    Private Sub MyGrid_PreviewMouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles MyGrid.PreviewMouseDoubleClick
        If MyGrid.CurrentItem Is Nothing Then Return
        Try
            GetData(MyGrid.CurrentItem("Line"))
        Catch ex As Exception
            Try
                GetData(MyGrid.CurrentItem(0))
            Catch ex2 As Exception
            End Try
        End Try
    End Sub

    Private Sub btnbtnNew_Click(sender As Object, e As RoutedEventArgs) Handles btnbtnNew.Click
        CurrentLine = 0
        EmpId.Clear()
        DayDate.SelectedDate = Nothing
        Shift.SelectedValue = 0
        StartTime.Content = 0
        EndTime.Content = ""
        CategoryId.SelectedValue = 0
        CategoryId_LostFocus(Nothing, Nothing)
        SubCategoryId.SelectedValue = 0
        TripPriceId.SelectedValue = 0
        DeliverymanId.SelectedValue = 0
        DeliverymanOther.Clear()
        CarId.SelectedValue = 0
        CarId_LostFocus(Nothing, Nothing)
        CallerId.Clear()
        CallerName.Clear()
        FillBalance()
        Address.Clear()
        Notes.Clear()
        MobFrom.Clear()
        NameFrom.Clear()
        AddressFrom.Clear()
        MobTo.Clear()
        NameTo.Clear()
        AddressTo.Clear()
        MobTo2.Clear()
        NameTo2.Clear()
        AddressTo2.Clear()
        ServiceAmount.Clear()
        ServiceAmountPre.Clear()
        KMCount.Clear()
        KMPrice.Clear()
        WaitingIndex.SelectedIndex = 0
        WaitingValue.Clear()
        MoneyTransfer.Clear()
        OrderAmount.Clear()
        TotalAmount.Clear()
        DeliveryPrice.Clear()
        Payment1.Clear()
        Payment2.Clear()
        Payment3.Clear()
        KnownUsTypeId.SelectedValue = 0
        KnownUsTypeNotes.Clear()

        EnableDisableControls(True)
        UserControl_Loaded(Nothing, Nothing)
    End Sub

    Private Sub FilOldOrders()
        bm.FillCombo("select 0 Id,'عدد الطلبات السابقة ( '+(select cast(count(*)as nvarchar(100)) from CallCenter where CallerId='" & CallerId.Text.Trim & "')+' )' Name,0 Sort union all select Line,cast(Line as nvarchar(100))+' - '+dbo.ToStrDate(daydate),1 Sort from CallCenter where CallerId='" & CallerId.Text.Trim & "' order by Sort,Id desc", OldOrders)
        OldOrders.SelectedIndex = 0
        If OldOrders.Items.Count > 1 Then
            lblKnownUsTypeId.Visibility = Windows.Visibility.Hidden
            KnownUsTypeId.Visibility = Windows.Visibility.Hidden
            KnownUsTypeNotes.Visibility = Windows.Visibility.Hidden
        Else
            lblKnownUsTypeId.Visibility = Windows.Visibility.Visible
            KnownUsTypeId.Visibility = Windows.Visibility.Visible
            KnownUsTypeNotes.Visibility = Windows.Visibility.Visible
        End If
    End Sub
     
    Private Sub OldOrders_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles OldOrders.SelectionChanged
        If OldOrders.SelectedValue Is Nothing Then Return
        If OldOrders.SelectedIndex <= 0 Then Return
        GetDataForOldOrder(OldOrders.SelectedValue)
    End Sub

    Private Sub CallerId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CallerId.KeyUp
        Dim str As String = "Select distinct cast(CallerId as varchar(100))Id,CallerName Name from CallCenter where 1=1 "
        If bm.ShowHelp("Callers", CallerId, CallerName, e, str) Then
            CallerId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub btnGetDeliveryMan_Click(sender As Object, e As RoutedEventArgs) Handles btnGetDeliveryMan.Click
        Try
            DeliverymanId.SelectedValue = bm.ExecuteScalar("GetNewDeliveryMan", {}, {})
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ServiceAmountPre_TextChanged(sender As Object, e As TextChangedEventArgs) Handles ServiceAmountPre.TextChanged, KMCount.TextChanged, KMPrice.TextChanged, WaitingValue.TextChanged, MoneyTransfer.TextChanged
        ServiceAmount.Text = Val(ServiceAmountPre.Text) + Val(KMCount.Text) * Val(KMPrice.Text) + Val(WaitingValue.Text) + Val(MoneyTransfer.Text)
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As RoutedEventArgs) Handles btnRefresh.Click
        FillList()
    End Sub

    Private Sub WaitingIndex_LostFocus(sender As Object, e As RoutedEventArgs) Handles WaitingIndex.LostFocus
        WaitingValue.Text = Val(WaitingIndex.SelectedValue) * 5
    End Sub


    Dim WithEvents BackgroundWorker1 As New System.ComponentModel.BackgroundWorker
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        FillList()
    End Sub

    Private Sub btnEditBalance_Click(sender As Object, e As RoutedEventArgs) Handles btnEditBalance.Click
        Dim frm As New CallerBalance With {.CurrentCallerId = CallerId.Text, .CurrentCallerName = CallerName.Text}
        frm.ShowDialog()
        FillBalance()
    End Sub

    Private Sub FillBalance()
        Balance.Content = Val(bm.ExecuteScalar("select top 1 CurrentBal from CallerBalance where CallerId='" & CallerId.Text & "' order by Line Desc"))
    End Sub

    Private Sub btnPrintScreen_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintScreen.Click
        bm.PrintScreen()
    End Sub

    Sub EnableDisableControls(MyIsEnabled As Boolean)
        If Flag <> 1 Then Return
        DayDate.IsEnabled = MyIsEnabled
        HH.IsEnabled = MyIsEnabled
        MM.IsEnabled = MyIsEnabled
        CategoryId.IsEnabled = MyIsEnabled
        SubCategoryId.IsEnabled = MyIsEnabled
        TripPriceId.IsEnabled = MyIsEnabled
        CallerId.IsEnabled = MyIsEnabled
        CallerName.IsEnabled = MyIsEnabled
        Address.IsEnabled = MyIsEnabled
        Notes.IsEnabled = MyIsEnabled
        MobFrom.IsEnabled = MyIsEnabled
        NameFrom.IsEnabled = MyIsEnabled
        AddressFrom.IsEnabled = MyIsEnabled
        MobTo.IsEnabled = MyIsEnabled
        NameTo.IsEnabled = MyIsEnabled
        AddressTo.IsEnabled = MyIsEnabled
        MobTo2.IsEnabled = MyIsEnabled
        NameTo2.IsEnabled = MyIsEnabled
        AddressTo2.IsEnabled = MyIsEnabled
        KMCount.IsEnabled = MyIsEnabled
        WaitingIndex.IsEnabled = MyIsEnabled
        MoneyTransfer.IsEnabled = MyIsEnabled
        DeliveryPrice.IsEnabled = MyIsEnabled
        OrderAmount.IsEnabled = MyIsEnabled
        Payment1.IsEnabled = MyIsEnabled
        Payment2.IsEnabled = MyIsEnabled
        Payment3.IsEnabled = MyIsEnabled
        OldOrders.IsEnabled = MyIsEnabled
        btnEditBalance.IsEnabled = MyIsEnabled
        btnViewHistory.IsEnabled = MyIsEnabled
    End Sub

    Private Sub CarId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CarId.LostFocus
        Try
            Dim x As Integer = Val(bm.ExecuteScalar("select DriverId from Cars where CategoryId=" & CategoryId.SelectedValue.ToString & " and Id=" & CarId.SelectedValue.ToString))
            If x > 0 Then DeliverymanId.SelectedValue = x
        Catch
        End Try
        If Val(CarId.SelectedValue) = -1 Then
            lblCarOtherId.Visibility = Windows.Visibility.Visible
            CarOtherId.Visibility = Windows.Visibility.Visible
            CarOtherName.Visibility = Windows.Visibility.Visible
        Else
            lblCarOtherId.Visibility = Windows.Visibility.Hidden
            CarOtherId.Visibility = Windows.Visibility.Hidden
            CarOtherName.Visibility = Windows.Visibility.Hidden
            CarOtherId.Clear()
            CarOtherName.Clear()
        End If
    End Sub

    Private Sub DeliverymanId_LostFocus(sender As Object, e As RoutedEventArgs) Handles DeliverymanId.LostFocus
        If Val(DeliverymanId.SelectedValue) = -1 Then
            DeliverymanOther.Visibility = Windows.Visibility.Visible
        Else
            DeliverymanOther.Visibility = Windows.Visibility.Hidden
            DeliverymanOther.Clear()
        End If
    End Sub

    Private Sub btnSendSMS_Click(sender As Object, e As RoutedEventArgs) Handles btnSendSMS.Click
        btnSendSMS.IsEnabled = False
        bm.SetModemMessage(CallerId.Text, CallerName.Text)
        btnSendSMS.IsEnabled = True
    End Sub

    Private Sub SearchDayDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles SearchDayDate.SelectedDateChanged
        SearchInvoiceNo_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub SearchCategoryId_LostFocus(sender As Object, e As RoutedEventArgs) Handles SearchCategoryId.LostFocus
        SearchInvoiceNo_TextChanged(Nothing, Nothing)
    End Sub


    Private Sub CarOtherId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CarOtherId.LostFocus
        If Val(CategoryId.SelectedValue) = 0 Then
            CarOtherId.Clear()
            CarOtherName.Clear()
            Return
        End If
        bm.LostFocus(CarOtherId, CarOtherName, "select Name from CarOthers where Id=" & CarOtherId.Text.Trim() & " and CategoryId=" & Val(CategoryId.SelectedValue))
    End Sub

    Private Sub CarOtherId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CarOtherId.KeyUp
        bm.ShowHelp("External Cars", CarOtherId, CarOtherName, e, "select cast(Id as varchar(100)) Id,Name from CarOthers  where CategoryId=" & Val(CategoryId.SelectedValue), "CarOthers", {"CategoryId"}, {Val(CategoryId.SelectedValue)})
    End Sub


End Class