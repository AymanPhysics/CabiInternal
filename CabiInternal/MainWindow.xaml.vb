Imports System.Drawing
Imports System.Data
Imports System.IO
Imports System.Windows.Controls.Primitives
Imports System.Xml

Class MainWindow
    Dim bm As New BasicMethods
    Public Nlvl As Boolean = False
    Dim bol As Boolean = False
    Dim Copy As Boolean = False

    Public WMP As New WMPLib.WindowsMediaPlayer
    Private Sub MainWindow_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        'Application.Current.MainWindow.SizeToContent = Windows.SizeToContent.WidthAndHeight
        'Application.Current.MainWindow.SizeToContent = Windows.SizeToContent.WidthAndHeight
        'Topmost = True
        BringIntoView()
        'Dim s As String = bm.Decrypt("otJ8kXBQS1NqkmfDT1lDNQ==")
    End Sub

    Private Sub MainWindow_Deactivated(sender As Object, e As EventArgs) Handles Me.Deactivated
        'Topmost = False
    End Sub

    Private Sub MainWindow_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        SaveSetting("CabiInternal", "Lang", "LayoutType", layoutSwitcher.SelectedIndex)
        SaveSetting("CabiInternal", "Lang", "LangType", langSwitcher.SelectedIndex)
        If Nlvl Or bol Then Return
        If Copy = True Then
            bol = True
            Application.Current.Shutdown()
            Exit Sub
        End If
        bm.ClearTemp()
        If bm.ShowDeleteMSG("MsgExit") Then
            bol = True
            Md.FourceExit = True
            Application.Current.Shutdown()
        Else
            e.Cancel = True
            'Me.BringIntoView()
        End If
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        LoadResource()

        Dim frm As New Window With {.WindowState = WindowState.Minimized, .ShowInTaskbar = False}
        frm.Show()
        frm.Content = WMP
        frm.Hide()

        layoutSwitcher.SelectedIndex = Val(GetSetting("CabiInternal", "Lang", "LayoutType"))
        langSwitcher.SelectedIndex = Val(GetSetting("CabiInternal", "Lang", "LangType"))
        SaveSetting("CabiInternal", "Lang", "LangType", langSwitcher.SelectedIndex)
        ''SaveSetting("CabiInternal", "Lang", "En", GetSetting("CabiInternal", "Lang", "En"))
        'layoutSwitcher.Visibility = Windows.Visibility.Hidden

        

        If Not LoadConnection() Then Return
        LoadBarcodePrinter()
        LoadPonePrinter()

        If Not MyProjectType = ProjectType.PCs Then
            If Md.Demo Then
                If Val(bm.ExecuteScalar("select (select count(*) from Attendance)+(select count(*) from BankCash)+(select count(*) from Cases)+(select count(*) from CustomerInvoices)+(select count(*) from Employees)+(select count(*) from Invoices)+(select count(*) from Entry)+(select count(*) from SalaryHistory)+(select count(*) from SalesMaster)+(select count(*) from Services)+(select count(*) from Reservations)+(select count(*) from Buildings)")) > 100 Then
                    bm.ShowMSG("عفوا .. انتهت فترة الاستخدام المجانى")
                    Application.Current.Shutdown()
                End If
            Else
                bm.TestProtection()
            End If
        End If

        Dim v As Integer = Val(bm.ExecuteScalar("select LastVersion from LastVersion"))
        If v > Md.LastVersion Or v = 0 Then
            bm.ShowMSG("MsgLastVersion")
            Application.Current.Shutdown()
        End If

        If Md.LastVersion > v Then
            bm.ExcuteNonQuery("delete from LastVersion insert into LastVersion (LastVersion) select " & Md.LastVersion)
        End If

        bm.ClearTemp()
        Md.CompanyName = bm.ExecuteScalar("select CompanyName from Statics")
        Md.CompanyTel = bm.ExecuteScalar("select CompanyTel from Statics")
        btnChangeLanguage.Visibility = Windows.Visibility.Hidden
        langSwitcher.Visibility = Windows.Visibility.Hidden
        Dim L As New Login
        langSwitcher.Visibility = Windows.Visibility.Visible
        
        For i As Integer = 0 To langSwitcher.Items.Count - 1
            Try
                Md.MyDictionaries.Items.Add(New ResourceDictionary With {.Source = New Uri(TryCast(TryCast(langSwitcher.Items(i), XmlElement).Attributes("Dic"), XmlAttribute).Value, UriKind.Relative)})
            Catch ex As Exception
            End Try
        Next
        langSwitcher_SelectionChanged(Nothing, Nothing)

        bm.SetImage(L.Img, "Login.jpg")
        LoadTabs(L)

    End Sub

    Public Sub LoadTabs(G As Object)
        Try
            MainGrid.Children.Clear()
            MainGrid.Children.Add(New Frame With {.Content = G})
        Catch ex As Exception
        End Try
    End Sub

    Public Sub AddTabOLD(ByVal M As MenuItem, ByVal L As UserControl)
        Dim Tab As New TabItem
        Tab.Header = M.Header
        Tab.Name = "Tab" & M.Name
        Tab.Content = L
        For Each it As TabItem In TabControl1.Items
            If it.Name = Tab.Name Then
                Tab = it
                TabControl1.SelectedItem = Tab
                Return
            End If
        Next
        TabControl1.Items.Add(Tab)
        TabControl1.SelectedItem = Tab
    End Sub

  
    Function LoadConnection() As Boolean
        If con.State = ConnectionState.Open Then Return True
        Dim st As New StreamReader(Md.UdlName & ".udl")
        Dim s As String = ""
        st.ReadLine()
        st.ReadLine()
        s += st.ReadLine
        con.ConnectionString = s.Substring(20)
        Dim cb As New SqlClient.SqlConnectionStringBuilder(con.ConnectionString)
        Dim f As New Form1
        'con.ConnectionString = "Data Source=" & cb.DataSource & ";Initial Catalog=" & cb.InitialCatalog & ";Persist Security Info=True;User ID=" & cb.UserID & ";Password=" & cb.Password 'f.Password 
        con.ConnectionString = cb.ConnectionString
        Try
            con.Open()
        Catch ex As Exception
            bm.ShowMSG("Connection failed")
            bol = True
            Md.FourceExit = True
            Application.Current.Shutdown()
            Return False
        End Try
        cmd.Connection = con
        Return True
    End Function


    Sub LoadBarcodePrinter()
        Try
            Dim st As New StreamReader("BarcodePrinter.dll")
            Md.BarcodePrinter = st.ReadLine
            st.Close()
        Catch ex As Exception
        End Try
    End Sub

    Sub LoadPonePrinter()
        Try
            Dim st As New StreamReader("PonePrinter.dll")
            Md.PonePrinter = st.ReadLine
            st.Close()
        Catch ex As Exception
        End Try
    End Sub
    Public LogedIn As Boolean = False
    Public Flag As Integer = 1


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnExit.Click
        Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLogout.Click
        Try
            If Not bm.ShowDeleteMSG("MsgExit") Then Return
            Forms.Application.Restart()
            Application.Current.Shutdown()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnChangeLanguage_Click(sender As Object, e As RoutedEventArgs) Handles btnChangeLanguage.Click
        'If GetSetting("CabiInternal", "Lang", "En", True) = False Then
        '    FlowDirection = Windows.FlowDirection.LeftToRight
        '    Md.DictionaryCurrent = Md.DictionaryEn
        '    SaveSetting("CabiInternal", "Lang", "En", True)
        'Else
        '    FlowDirection = Windows.FlowDirection.RightToLeft
        '    Md.DictionaryCurrent = Md.DictionaryAr
        '    SaveSetting("CabiInternal", "Lang", "En", False)
        'End If
        Resources = Md.DictionaryCurrent
        Banner1.Resources = Md.DictionaryCurrent
        Banner2.Resources = Md.DictionaryCurrent
        If MainGrid.Children(0).GetType.ToString = "System.Windows.Controls.Frame" Then CType(MainGrid.Children(0), Frame).Refresh()

    End Sub

    Private Sub LoadResource()
        'Md.DictionaryAr.Source = New Uri("Dic_Ar.xaml", UriKind.Relative)
        'Md.DictionaryEn.Source = New Uri("Dic_En.xaml", UriKind.Relative)

        btnChangeLanguage.SetResourceReference(Button.ContentProperty, "Ar-En")
        btnLogout.SetResourceReference(Button.ContentProperty, "Logout")
        btnExit.SetResourceReference(Button.ContentProperty, "ExitApp")
    End Sub


    Private Sub MainWindow_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles Me.PreviewKeyDown
        Try
            If e.Key = System.Windows.Input.Key.Enter Then
                'e.Handled = True
                If FocusManager.GetFocusedElement(Me).GetType = GetType(Button) Then Return
                If FocusManager.GetFocusedElement(Me).GetType = GetType(Forms.Integration.WindowsFormsHost) Then Return
                If FocusManager.GetFocusedElement(Me).GetType = GetType(TextBox) Then
                    If CType(FocusManager.GetFocusedElement(Me), TextBox).VerticalScrollBarVisibility = ScrollBarVisibility.Visible Then Return
                End If
                Dim c As Control = FocusManager.GetFocusedElement(Me)
                'InputManager.Current.ProcessInput(New KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) With {.RoutedEvent = Keyboard.KeyDownEvent})
                c.Focus()
                InputManager.Current.ProcessInput(New KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) With {.RoutedEvent = Keyboard.KeyDownEvent})
                If FocusManager.GetFocusedElement(Me).GetType = GetType(TextBox) AndAlso Not CType(FocusManager.GetFocusedElement(Me), TextBox).VerticalScrollBarVisibility = ScrollBarVisibility.Visible Then CType(FocusManager.GetFocusedElement(Me), TextBox).SelectAll()
            End If
        Catch
        End Try
    End Sub

    Private Sub btnMinimize_Click(sender As Object, e As RoutedEventArgs) Handles btnMinimize.Click
        WindowState = Windows.WindowState.Minimized
    End Sub

    Private Sub langSwitcher_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles langSwitcher.SelectionChanged
        Try
            Dim e1 As XmlElement = TryCast(langSwitcher.SelectedItem, XmlElement)
            If e1 IsNot Nothing Then
                Md.DictionaryCurrent = Md.MyDictionaries.Items(langSwitcher.SelectedIndex)
                FlowDirection = TryCast(e1.Attributes("FlowDirection"), XmlAttribute).Value

                Resources = Md.DictionaryCurrent
                Banner1.Resources = Md.DictionaryCurrent
                Banner2.Resources = Md.DictionaryCurrent
                If MainGrid.Children(0).GetType.ToString = "System.Windows.Controls.Frame" Then CType(MainGrid.Children(0), Frame).Refresh()
            End If
        Catch ex As Exception
        End Try
    End Sub


    Private Sub layoutSwitcher_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles layoutSwitcher.SelectionChanged
        If layoutSwitcher.SelectedIndex = 1 Then
            WindowStyle = Windows.WindowStyle.ThreeDBorderWindow
        Else
            WindowStyle = Windows.WindowStyle.None
            WindowState = Windows.WindowState.Minimized
            WindowState = Windows.WindowState.Maximized
        End If
    End Sub


    'Private WithEvents oUsbFunkey As New usbFunkey
    'Protected Overrides Sub OnSourceInitialized(ByVal e As System.EventArgs)
    '    Try
    '        MyBase.OnSourceInitialized(e)
    '        Dim MainFormWinInteropHelper As New Interop.WindowInteropHelper(Me)
    '        Interop.HwndSource.FromHwnd(MainFormWinInteropHelper.Handle). _
    '            AddHook(AddressOf oUsbFunkey.HwndHandler)
    '        'oUsbFunkey.RegisterDeviceNotification(MainFormWinInteropHelper.Handle)
    '    Catch
    '    End Try
    'End Sub

    'Private Sub oUsbFunkey_AfterUSBInserted(ByVal sender As Object, ByVal e As System.EventArgs) Handles oUsbFunkey.AfterUSBInserted
    '    bm.SetModem()
    'End Sub

    'Private Sub oUsbFunkey_AfterUSBRemoved(ByVal sender As Object, ByVal e As System.EventArgs) Handles oUsbFunkey.AfterUSBRemoved
    '    'bm.SetModem()
    'End Sub

End Class
