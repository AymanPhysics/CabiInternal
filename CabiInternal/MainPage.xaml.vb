' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

Imports System.Text
Imports System.Windows.Media.Animation
Imports System.IO
Imports System.Windows.Threading
Imports System.Data
Imports System.Xml
Imports System.IO.Ports
Imports System.Threading

Partial Public Class MainPage
    Inherits Page
    Public NLevel As Boolean = False
    Dim m As MainWindow = Application.Current.MainWindow
    Dim bm As New BasicMethods

    Private sampleGridOpacityAnimation As DoubleAnimation
    Private sampleGridTranslateTransformAnimation As DoubleAnimation
    Private borderTranslateDoubleAnimation As DoubleAnimation

    Public Sub New()
        InitializeComponent()

        Dim widthBinding As New Binding("ActualWidth")
        widthBinding.Source = Me

        sampleGridOpacityAnimation = New DoubleAnimation()
        sampleGridOpacityAnimation.To = 0
        sampleGridOpacityAnimation.Duration = New Duration(TimeSpan.FromSeconds(0.15))

        sampleGridTranslateTransformAnimation = New DoubleAnimation()
        sampleGridTranslateTransformAnimation.BeginTime = TimeSpan.FromSeconds(0.15)
        sampleGridTranslateTransformAnimation.Duration = New Duration(TimeSpan.FromSeconds(0.15))

        borderTranslateDoubleAnimation = New DoubleAnimation()
        borderTranslateDoubleAnimation.Duration = New Duration(TimeSpan.FromSeconds(0.3))
        borderTranslateDoubleAnimation.BeginTime = TimeSpan.FromSeconds(0)

        'If Md.MyProject = Client.ClothesRed Then
        '    bm.SetColor(SampleDisplayBorder)
        '    btnBack.Background = System.Windows.Media.Brushes.White
        'End If
    End Sub
    Private Shared _packUri As New Uri("pack://application:,,,/")

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        borderTranslateDoubleAnimation.From = 0
        borderTranslateDoubleAnimation.To = -ActualWidth
        SampleDisplayBorderTranslateTransform.BeginAnimation(TranslateTransform.XProperty, borderTranslateDoubleAnimation)
        GridSampleViewer_Loaded(Nothing, Nothing)
        Md.Currentpage = ""
    End Sub

    Private Sub selectedSampleChanged(ByVal sender As Object, ByVal args As RoutedEventArgs)

        If TypeOf args.Source Is RadioButton Then
            Dim theButton As RadioButton = CType(args.Source, RadioButton)

            Dim theFrame
            If TypeOf theButton.Tag Is Page Then
                theFrame = CType(theButton.Tag, Page)
                theFrame.Title = Resources.Item(CType(CType(args.Source, RadioButton).Content, TranslateTextAnimationExample).RealText.Tag)
            ElseIf TypeOf theButton.Tag Is Window Then
                theFrame = CType(theButton.Tag, MyWindow)
                theFrame.Title = Resources.Item(CType(CType(args.Source, RadioButton).Content, TranslateTextAnimationExample).RealText.Tag)
            End If

            theButton.IsTabStop = False
            CType(args.Source, RadioButton).IsChecked = False

            If TypeOf theButton.Tag Is Window Then
                CType(theFrame, MyWindow).Show()
            ElseIf m.layoutSwitcher.SelectedIndex = 1 Then
                Dim frm As New MyWindow With {.Title = Resources.Item(CType(CType(args.Source, RadioButton).Content, TranslateTextAnimationExample).RealText.Tag), .WindowState = WindowState.Maximized}

                frm.Content = theButton.Tag
                frm.Show()
            Else

                SampleDisplayFrame.Content = theButton.Tag
                SampleDisplayBorder.Visibility = Visibility.Visible
                Try
                    theFrame.Tag = CType(CType(args.Source, RadioButton).Content, TranslateTextAnimationExample).RealText.Tag
                Catch ex As Exception
                End Try
                sampleDisplayFrameLoaded(theFrame, args)

            End If

        End If

    End Sub

    Private Sub sampleDisplayFrameLoaded(ByVal sender As Object, ByVal args As EventArgs)
        If TypeOf sender Is MyWindow Then
            Try
                If Not Resources.Item(CType(sender, MyWindow).Tag) Is Nothing Then
                    CType(sender, MyWindow).Title = Resources.Item(CType(sender, MyWindow).Tag)
                    Md.Currentpage = CType(sender, MyWindow).Title
                End If
            Catch ex As Exception
            End Try
        ElseIf TypeOf sender Is Page Then
            Try
                CType(sender, Page).Title = Resources.Item(CType(sender, Page).Tag)
                Md.Currentpage = CType(sender, Page).Title
            Catch ex As Exception
            End Try
        ElseIf TypeOf CType(sender, Frame).Content Is Page Then
            Try
                If Not Resources.Item(CType(CType(sender, Frame).Content, Page).Tag) Is Nothing Then
                    CType(CType(sender, Frame).Content, Page).Title = Resources.Item(CType(CType(sender, Frame).Content, Page).Tag)
                    Md.Currentpage = CType(CType(sender, Frame).Content, Page).Title
                End If
            Catch ex As Exception
            End Try
            Try
                CType(sender, Page).Title = Resources.Item(CType(sender, Page).Tag)
                Md.Currentpage = CType(sender, Page).Title
            Catch ex As Exception
            End Try
        End If

        sampleGridTranslateTransformAnimation.To = -ActualWidth
        borderTranslateDoubleAnimation.From = -ActualWidth
        borderTranslateDoubleAnimation.To = 0

        SampleDisplayBorder.Visibility = Visibility.Visible
        SampleGrid.BeginAnimation(Grid.OpacityProperty, sampleGridOpacityAnimation)
        SampleGridTranslateTransform.BeginAnimation(TranslateTransform.XProperty, sampleGridTranslateTransformAnimation)
        SampleDisplayBorderTranslateTransform.BeginAnimation(TranslateTransform.XProperty, borderTranslateDoubleAnimation)
    End Sub

    Private Sub galleryLoaded(ByVal sender As Object, ByVal args As RoutedEventArgs)
        If bm.TestIsLoaded(Me, True) Then Return
        tab.Margin = New Thickness(0)
        tab.HorizontalAlignment = HorizontalAlignment.Stretch
        tab.VerticalAlignment = VerticalAlignment.Stretch

        Load()

        SampleDisplayBorderTranslateTransform.X = -ActualWidth
        SampleDisplayBorder.Visibility = Visibility.Hidden
    End Sub

    Private Sub pageSizeChanged(ByVal sender As Object, ByVal args As SizeChangedEventArgs)
        SampleDisplayBorderTranslateTransform.X = Me.ActualWidth
    End Sub

    Dim DesignDt As New DataTable
    Sub LoadLabel(ByVal G As WrapPanel, Ttl As String)
        CurrentMenuitem += 1
        'If Md.MyProject = Client.Clothes Then Return

        For i As Integer = 0 To m.langSwitcher.Items.Count - 1
            Try
                If TryCast(TryCast(m.langSwitcher.Items(i), XmlElement).Attributes("Visibility"), XmlAttribute).Value = "2" Then Continue For
                Dim rd As ResourceDictionary = Md.MyDictionaries.Items(i)
                While rd.Item(Ttl).Length < 16
                    rd.Item(Ttl) = " " & rd.Item(Ttl) & " "
                End While
            Catch ex As Exception
            End Try
        Next

        Dim lbl0 As New Label With {.Height = ActualHeight, .Margin = New Windows.Thickness(24, 0, 0, 0), .Background = New SolidColorBrush With {.Opacity = 0, .Color = Color.FromRgb(0, 0, 0)}}
        G.Children.Add(lbl0)

        Dim lbl As New Label With {.Name = "menuitem" & CurrentMenuitem, .FontFamily = New System.Windows.Media.FontFamily("Times New Roman"), .FontSize = 24, .HorizontalContentAlignment = Windows.HorizontalAlignment.Center, .Foreground = New SolidColorBrush With {.Color = Color.FromRgb(0, 0, 0)}, .Background = New SolidColorBrush With {.Color = Color.FromRgb(0, 0, 0), .Opacity = 0}, .FontWeight = FontWeight.FromOpenTypeWeight(800), .Height = 70}
        lbl.SetResourceReference(Label.ContentProperty, Ttl)
        G.Children.Add(lbl)

        If Ttl = "" Then lbl.Height = 0
    End Sub

    'Function AddHandler LoadRadio(ByVal G As WrapPanel, ByVal frm As UserControl, ByVal Ttl As String) As RadioButton
    Function LoadRadio(ByVal G As WrapPanel, ByVal Ttl As String) As RadioButton
        CurrentMenuitem += 1

        For i As Integer = 0 To m.langSwitcher.Items.Count - 1
            Try
                If TryCast(TryCast(m.langSwitcher.Items(i), XmlElement).Attributes("Visibility"), XmlAttribute).Value = "2" Then Continue For
                Dim rd As ResourceDictionary = Md.MyDictionaries.Items(i)
                While rd.Item(Ttl).Length < 16
                    rd.Item(Ttl) = " " & rd.Item(Ttl) & " "
                End While
            Catch ex As Exception
            End Try
        Next

        Dim RName As String = "menuitem" & CurrentMenuitem
        Dim r As New RadioButton With {.Name = RName, .Style = Application.Current.FindResource("GlassRadioButtonStyle"), .Width = 140, .Height = 70} 

        Dim t As New TranslateTextAnimationExample
        t.RealText.Tag = Ttl
        t.RealText.SetResourceReference(TextBlock.TextProperty, Ttl)
        'r.SetResourceReference(RadioButton.BackgroundProperty, "SC")
        't.SetResourceReference(RadioButton.BackgroundProperty, "SC")

        r.Content = t
        G.Children.Add(r)

        r.SetResourceReference(RadioButton.ToolTipProperty, Ttl)
        Return r
    End Function


    Private Sub GridSampleViewer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        bm.TestIsLoaded(Me)
    End Sub

    Private Sub ResizeHeader(G As WrapPanel)
        If Lvl Then Return
        Dim Ttl As String = CType(CType(G.Parent, ScrollViewer).Parent, TabItem).Header
        While Md.DictionaryCurrent.Item(Ttl).Length < 16
            Md.DictionaryCurrent.Item(Ttl) = " " & Md.DictionaryCurrent.Item(Ttl) & " "
        End While
    End Sub


    Public Lvl As Boolean = False
    Dim CurrentTab As Integer = 0
    Dim CurrentMenuitem As Integer = 0
    Public Sub Load()

        DesignDt = bm.ExcuteAdapter("select * from PLevels where id='" & Md.UserName & "'")

        If MyProjectType = ProjectType.PCs Then
            LoadGPCs()
            Return
        End If

        LoadTabs()

        If Not Lvl Then
            Dim dt As DataTable = bm.ExcuteAdapter("select * from nlevels where id=" & Md.LevelId)
            If dt.Rows.Count = 0 Then Return

            For i As Integer = 0 To tab.Items.Count - 1
                Dim item As TabItem = CType(tab.Items(i), TabItem)

                If dt.Rows(0)(CType(tab.Items(i), TabItem).Name).ToString = "" Then
                    item.Visibility = Windows.Visibility.Collapsed
                Else
                    item.Visibility = IIf(dt.Rows(0)(item.Name), Visibility.Visible, Visibility.Collapsed)
                End If
                item.Content.Visibility = item.Visibility

                For x As Integer = 0 To CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children.Count - 1
                    If CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children(x).GetType = GetType(RadioButton) Then
                        Dim t As RadioButton = CType(CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children(x), RadioButton)
                        If dt.Rows(0)(t.Name).ToString = "" Then
                            t.Visibility = Windows.Visibility.Collapsed
                        Else
                            t.Visibility = IIf(dt.Rows(0)(t.Name), Visibility.Visible, Visibility.Collapsed)
                        End If
                    ElseIf CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children(x).GetType = GetType(Label) Then
                        Dim t As Label = CType(CType(CType(item.Content, ScrollViewer).Content, WrapPanel).Children(x), Label)
                        If t.Name = "" Then
                            t.Visibility = Windows.Visibility.Visible
                        ElseIf dt.Rows(0)(t.Name).ToString = "" Then
                            t.Visibility = Windows.Visibility.Collapsed
                        Else
                            t.Visibility = IIf(dt.Rows(0)(t.Name), Visibility.Visible, Visibility.Collapsed)
                        End If
                    End If
                Next
            Next

            For i As Integer = 0 To tab.Items.Count - 1
                If CType(tab.Items(i), TabItem).Visibility = Windows.Visibility.Visible Then
                    CType(tab.Items(i), TabItem).IsSelected = True
                    Exit For
                End If
            Next

        End If

    End Sub

    Private Sub PrintTbl(ByVal Header As String, ByVal tbl As String, Optional ByVal maintbl As String = "", Optional ByVal mainfield As String = "")
        Dim frm As New ReportViewer
        frm.Rpt = IIf(maintbl = "", "PrintTbl.rpt", "PrintTbl2.rpt")
        frm.paraname = {"Header", "@tbl", "@maintbl", "@mainfield"}
        frm.paravalue = {Header, tbl, maintbl, mainfield}
        frm.ShowDialog()
    End Sub

    Function MakePanel(MyHeader As String, ImagePath As String) As WrapPanel
        CurrentTab += 1
        Dim SV As New MyScrollViewer
        bm.SetImage(SV.Img, ImagePath)
        Dim t As New TabItem With {.Content = SV, .Name = "tab" & CurrentTab, .Header = MyHeader, .Tag = MyHeader}

        'Template.ControlTemplate().Grid().Border().TextBlock()
        'FontFamily="khalaad al-arabeh 2" FontSize="12"
        t.Style = FindResource("MyTabItem")

        tab.Items.Add(t)
        Dim G As WrapPanel = SV.MyWrapPanel

        G.AddHandler(System.Windows.Controls.Primitives.ToggleButton.CheckedEvent, New System.Windows.RoutedEventHandler(AddressOf Me.selectedSampleChanged))
        ResizeHeader(G)
        t.SetResourceReference(TabItem.HeaderProperty, t.Header)
        Return G
    End Function

    Private Sub LoadGPCs()
        Dim G As WrapPanel = MakePanel("File", "MainCabiInternal.jpg")

        AddHandler LoadRadio(G, "PCs").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                    Dim frm As New BasicForm With {.TableName = "PCs"}
                                                    bm.SetImage(CType(frm, BasicForm).Img, "password.jpg")
                                                    frm.txtName.MaxLength = 1000
                                                    m.TabControl1.Items.Clear()
                                                    sender.Tag = New Page With {.Content = frm}
                                                End Sub

    End Sub

    Private Sub LoadGFile()
        Dim G As WrapPanel = MakePanel("File", "MainCabiInternal.jpg")
        Dim frm As UserControl

        AddHandler LoadRadio(G, "Employees").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          sender.Tag = New Page With {.Content = New Employees}
                                                      End Sub

        AddHandler LoadRadio(G, "Countries").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New BasicForm With {.TableName = "Countries"}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "Cities").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                       frm = New BasicForm2 With {.MainTableName = "Countries", .MainSubId = "Id", .MainSubName = "Name", .lblMain_Content = "Country", .TableName = "Cities", .MainId = "CountryId", .SubId = "Id", .SubName = "Name"}
                                                       bm.SetImage(CType(frm, BasicForm2).Img, "MainCabiInternal.jpg")
                                                       sender.Tag = New Page With {.Content = frm}
                                                   End Sub

        AddHandler LoadRadio(G, "Areas").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New BasicForm3 With {.MainTableName = "Countries", .MainSubId = "Id", .MainSubName = "Name", .lblMain_Content = "Country", .Main2TableName = "Cities", .Main2MainId = "CountryId", .Main2SubId = "Id", .Main2SubName = "Name", .lblMain2_Content = "City", .TableName = "Areas", .MainId = "CountryId", .MainId2 = "CityId", .SubId = "Id", .SubName = "Name"}
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub 

        AddHandler LoadRadio(G, "MainJobs").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                         frm = New BasicForm With {.TableName = "MainJobs"}
                                                         sender.Tag = New Page With {.Content = frm}
                                                     End Sub

        AddHandler LoadRadio(G, "SubJobs").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New BasicForm2 With {.MainTableName = "MainJobs", .MainSubId = "Id", .MainSubName = "Name", .lblMain_Content = "MainJob", .TableName = "SubJobs", .MainId = "MainJobId", .SubId = "Id", .SubName = "Name"}
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub
        
        'AddHandler LoadRadio(G, "Departments").Checked, Sub(sender As Object, e As RoutedEventArgs)
        '                                                    frm = New BasicForm With {.TableName = "Departments"}
        '                                                    sender.Tag = New Page With {.Content = frm}
        '                                                End Sub
        
        AddHandler LoadRadio(G, "Attachment Types").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                 frm = New BasicForm With {.TableName = "AttachmentTypes"}
                                                                 sender.Tag = New Page With {.Content = frm}
                                                             End Sub

        If Md.ShowShifts Then
            AddHandler LoadRadio(G, "Shifts").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                           frm = New BasicForm With {.TableName = "Shifts"}
                                                           sender.Tag = New Page With {.Content = frm}
                                                       End Sub
        End If


        AddHandler LoadRadio(G, "KnownUsTypes").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                             frm = New BasicForm With {.TableName = "KnownUsTypes"}
                                                             sender.Tag = New Page With {.Content = frm}
                                                         End Sub
    End Sub

    Private Sub LoadGOperation()
        Dim G As WrapPanel = MakePanel("Operation", "MainCabiInternal.jpg")
        Dim frm As UserControl

        LoadLabel(G, "File")

        AddHandler LoadRadio(G, "OrderStates").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New OrderStates
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub

        AddHandler LoadRadio(G, "Groups").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                       frm = New BasicForm With {.TableName = "CallCenterCategories"}
                                                       sender.Tag = New Page With {.Content = frm}
                                                   End Sub

        AddHandler LoadRadio(G, "Types").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New BasicForm2 With {.MainTableName = "CallCenterCategories", .MainSubId = "Id", .MainSubName = "Name", .lblMain_Content = "Group", .TableName = "CallCenterSubCategories", .MainId = "CategoryId", .SubId = "Id", .SubName = "Name"}
                                                      bm.SetImage(CType(frm, BasicForm2).Img, "MainCabiInternal.jpg")
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        AddHandler LoadRadio(G, "CustomerServiceQuestions").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                         frm = New BasicForm With {.TableName = "CustomerServiceQuestions"}
                                                                         sender.Tag = New Page With {.Content = frm}
                                                                     End Sub

        AddHandler LoadRadio(G, "TripPrices").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                           frm = New TripPrices
                                                           sender.Tag = New Page With {.Content = frm}
                                                       End Sub

        AddHandler LoadRadio(G, "Delivery Pikes").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New Cars With {.Flag = 1} 'ديليفري
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub

        AddHandler LoadRadio(G, "Limousine Cars").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New Cars With {.Flag = 3} 'ليموزين
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub

        AddHandler LoadRadio(G, "Limo Pikes").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                           frm = New Cars With {.Flag = 4} 'ليمو بايك
                                                           sender.Tag = New Page With {.Content = frm}
                                                       End Sub

        LoadLabel(G, "Daily Motion")

        AddHandler LoadRadio(G, "CallCenter").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                           Dim MyContent As New CallCenter With {.Flag = 1}
                                                           Dim wn As New MyWindow With {.Title = "CallCenter", .WindowState = WindowState.Maximized}
                                                           wn.Content = MyContent
                                                           sender.Tag = wn
                                                       End Sub

        AddHandler LoadRadio(G, "Operation").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          Dim MyContent As New CallCenter With {.Flag = 2}
                                                          Dim wn As New MyWindow With {.Title = "CallCenter", .WindowState = WindowState.Maximized}
                                                          wn.Content = MyContent
                                                          sender.Tag = wn
                                                      End Sub

        AddHandler LoadRadio(G, "CustomerService").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                Dim MyContent As New CallCenter With {.Flag = 3}
                                                                Dim wn As New MyWindow With {.Title = "CallCenter", .WindowState = WindowState.Maximized}
                                                                wn.Content = MyContent
                                                                sender.Tag = wn
                                                            End Sub

        LoadLabel(G, "OutCome")
        
        AddHandler LoadRadio(G, "OutCome").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New EmpOutcome
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub

        AddHandler LoadRadio(G, "EmpComplaints").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New EmpComplaints
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

    End Sub

    Private Sub LoadGHR()
        Dim s As String = ""
        Dim G As WrapPanel = MakePanel("HR", "MainCabiInternal.jpg")
        Dim frm As UserControl

        AddHandler LoadRadio(G, "OfficialHolidays").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                 frm = New OfficialHolidays
                                                                 sender.Tag = New Page With {.Content = frm}
                                                             End Sub

        AddHandler LoadRadio(G, "Import Attendance").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                  frm = New CalcSalary With {.Flag = 4}
                                                                  sender.Tag = New Page With {.Content = frm}
                                                              End Sub

        AddHandler LoadRadio(G, "Edit Attendance").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New EditAttendance
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

        AddHandler LoadRadio(G, "Loans").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New Loans With {.TableName = "Loans"}
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        LoadLabel(G, "Employees Motion")

        AddHandler LoadRadio(G, "DirectBonus").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New DirectBonusCut With {.TableName = "DirectBonus"}
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub

        AddHandler LoadRadio(G, "DirectCut").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New DirectBonusCut With {.TableName = "DirectCut"}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "LeaveRequests").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New LeaveRequests With {.TableName = "LeaveRequests"}
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

        AddHandler LoadRadio(G, "LeaveRequests2").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New LeaveRequests2 With {.TableName = "LeaveRequests2"}
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub

        LoadLabel(G, "Evaluations")

        AddHandler LoadRadio(G, "GeneralEvaluationQuestions").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                           frm = New JobEvaluationQuestions With {.IsCommon = True}
                                                                           sender.Tag = New Page With {.Content = frm}
                                                                       End Sub

        AddHandler LoadRadio(G, "JobEvaluationQuestions").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                       frm = New JobEvaluationQuestions
                                                                       sender.Tag = New Page With {.Content = frm}
                                                                   End Sub

        LoadLabel(G, "Calculation")

        AddHandler LoadRadio(G, "Calc Salary").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New CalcSalary With {.Flag = 1}
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub


    End Sub

    Private Sub LoadGAccountants()
        Dim s As String = "MainCabiInternal.jpg"

        Dim G As WrapPanel = MakePanel("Accounts", s)
        Dim frm As UserControl

        AddHandler LoadRadio(G, "Chart").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New Chart
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        If Md.ShowCostCenter Then
            AddHandler LoadRadio(G, "CostCenters").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New CostCenters
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub
        End If

        If Md.ShowCurrency Then
            AddHandler LoadRadio(G, "Currencies").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New BasicForm1_2 With {.TableName = "Currencies", .lblName2_text = "الرمز", .SubName2 = "Sign"}
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub
        End If

        AddHandler LoadRadio(G, "CheckBanks").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                           frm = New BasicForm With {.TableName = "CheckBanks"}
                                                           sender.Tag = New Page With {.Content = frm}
                                                       End Sub

        AddHandler LoadRadio(G, "Income Daily Motion Types").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                          frm = New BankCash_G2Types With {.Flag = 1}
                                                                          sender.Tag = New Page With {.Content = frm}
                                                                      End Sub

        AddHandler LoadRadio(G, "Outcome Daily Motion Types").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                           frm = New BankCash_G2Types With {.Flag = 2}
                                                                           sender.Tag = New Page With {.Content = frm}
                                                                       End Sub

        LoadLabel(G, "File")

        AddHandler LoadRadio(G, "Assets").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                       frm = New CreditsDebits With {.TableName = "Assets", .MyLinkFile = 12}
                                                       sender.Tag = New Page With {.Content = frm}
                                                   End Sub

        AddHandler LoadRadio(G, "Customers").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New Customers
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "Suppliers").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New Suppliers
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "Debits").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                       frm = New CreditsDebits With {.TableName = "Debits", .MyLinkFile = 3}
                                                       sender.Tag = New Page With {.Content = frm}
                                                   End Sub

        AddHandler LoadRadio(G, "Credits").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New CreditsDebits With {.TableName = "Credits", .MyLinkFile = 4}
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub

        AddHandler LoadRadio(G, "Saves").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New CreditsDebits With {.TableName = "Saves", .MyLinkFile = 5}
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        If Md.ShowBanks Then
            AddHandler LoadRadio(G, "Banks").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New CreditsDebits With {.TableName = "Banks", .MyLinkFile = 6}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub
        End If

        AddHandler LoadRadio(G, "Sellers").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New CreditsDebits With {.TableName = "Sellers", .MyLinkFile = 7}
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub

        AddHandler LoadRadio(G, "OutComeTypes").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                             frm = New CreditsDebits With {.TableName = "OutComeTypes", .MyLinkFile = 9}
                                                             sender.Tag = New Page With {.Content = frm}
                                                         End Sub

        AddHandler LoadRadio(G, "InComeTypes").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New CreditsDebits With {.TableName = "InComeTypes", .MyLinkFile = 10}
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub

        LoadLabel(G, "Daily Motion")

        AddHandler LoadRadio(G, "Adjustments").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New Entry2 With {.Flag = 1}
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub

        AddHandler LoadRadio(G, "Entry").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New Entry
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        LoadLabel(G, "Income and Outcome")

        AddHandler LoadRadio(G, "Income").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                       frm = New BankCash_G2 With {.Flag = 1}
                                                       sender.Tag = New Page With {.Content = frm}
                                                   End Sub

        AddHandler LoadRadio(G, "Outcome").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New BankCash_G2 With {.Flag = 2}
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub
        If Md.ShowBanks Then
            AddHandler LoadRadio(G, "Bank Transfer").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                  frm = New BankCash_G3
                                                                  sender.Tag = New Page With {.Content = frm}
                                                              End Sub
        End If

        AddHandler LoadRadio(G, "Checks Tracing").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New ChecksTracingNew
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub
    End Sub

    Private Sub LoadGSecurity()
        Dim s As String = "MainCabiInternal.jpg"

        Dim G As WrapPanel = MakePanel("Options", s)
        Dim frm As UserControl

        AddHandler LoadRadio(G, "Change Password").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New ChangePassword
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

        AddHandler LoadRadio(G, "Levels").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                       frm = New Levels
                                                       sender.Tag = New Page With {.Content = frm}
                                                   End Sub

        AddHandler LoadRadio(G, "Attachement").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New Attachments
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub

        'frm = New PhoneIndex
        'AddHandler LoadRadio(G,  "Contacts")

        If Md.ShowShifts Then
            AddHandler LoadRadio(G, "Close Shift").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New CalcSalary With {.Flag = 6}
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub
        End If
        AddHandler LoadRadio(G, "Schedule").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                         frm = New Schedule
                                                         sender.Tag = New Page With {.Content = frm}
                                                     End Sub
    End Sub

    Private Sub LoadGOperationReports()
        Dim G As WrapPanel = MakePanel("Operation Reports", "MainCabiInternal.jpg")
        Dim frm As UserControl

        LoadLabel(G, "Daily Motion")

        AddHandler LoadRadio(G, "CallCenterDetailed").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                   frm = New RPT1 With {.Flag = 1}
                                                                   sender.Tag = New Page With {.Content = frm}
                                                               End Sub

        AddHandler LoadRadio(G, "CallCenterBrief").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New RPT1 With {.Flag = 6}
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

        AddHandler LoadRadio(G, "CallCenterTotal").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New RPT1 With {.Flag = 2}
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

        AddHandler LoadRadio(G, "NetIncome").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New RPT1 With {.Flag = 3}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "Shift Closing").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New RPT1 With {.Flag = 9}
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

        AddHandler LoadRadio(G, "CallCenterDetailedDeleted").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                          frm = New RPT1 With {.Flag = 1, .Cancel = 1}
                                                                          sender.Tag = New Page With {.Content = frm}
                                                                      End Sub

        AddHandler LoadRadio(G, "KnownUsTypeId").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New RPT1 With {.Flag = 5}
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

        LoadLabel(G, "OutCome")

        AddHandler LoadRadio(G, "OutCome").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New RPT1 With {.Flag = 7}
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub

        AddHandler LoadRadio(G, "EmpComplaints").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New RPT1 With {.Flag = 8}
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

        AddHandler LoadRadio(G, "Customers").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New CalcSalary With {.Flag = 9}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub
    End Sub

    Private Sub LoadGHRReports()
        Dim G As WrapPanel = MakePanel("HR Reports", "MainCabiInternal.jpg")
        Dim frm As UserControl

        AddHandler LoadRadio(G, "Employees").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New CalcSalary With {.Flag = 8}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "Salary Detailed").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New RPT9 With {.Flag = 1}
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

        AddHandler LoadRadio(G, "Salary Total").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                             frm = New RPT9 With {.Flag = 2}
                                                             sender.Tag = New Page With {.Content = frm}
                                                         End Sub

        AddHandler LoadRadio(G, "Attendance").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                           frm = New RPT9 With {.Flag = 3}
                                                           sender.Tag = New Page With {.Content = frm}
                                                       End Sub

        AddHandler LoadRadio(G, "Loans").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New RPT25 With {.Flag = 1}
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        AddHandler LoadRadio(G, "Loans Status").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                             frm = New RPT25 With {.Flag = 6}
                                                             sender.Tag = New Page With {.Content = frm}
                                                         End Sub

        LoadLabel(G, "Employees Motion")

        AddHandler LoadRadio(G, "DirectBonus").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                            frm = New RPT25 With {.Flag = 2}
                                                            sender.Tag = New Page With {.Content = frm}
                                                        End Sub

        AddHandler LoadRadio(G, "DirectCut").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New RPT25 With {.Flag = 3}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "LeaveRequests").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New RPT25 With {.Flag = 4}
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

        AddHandler LoadRadio(G, "LeaveRequests2").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New RPT25 With {.Flag = 5}
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub

    End Sub

    Private Sub LoadGAccountsReports()
        Dim s As String = "MainCabiInternal.jpg"

        Dim G As WrapPanel = MakePanel("Accounts Reports", s)
        Dim frm As UserControl

        AddHandler LoadRadio(G, "Chart").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                      frm = New RPT26 With {.Flag = 1}
                                                      sender.Tag = New Page With {.Content = frm}
                                                  End Sub

        AddHandler LoadRadio(G, "Account Motion").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                               frm = New RPT2 With {.Flag = 1, .MyLinkFile = -1}
                                                               sender.Tag = New Page With {.Content = frm}
                                                           End Sub

        If Md.ShowBankCash_G Then
            AddHandler LoadRadio(G, "Income View").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New RPT21 With {.Flag = 1}
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

            AddHandler LoadRadio(G, "Outcome View").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                 frm = New RPT21 With {.Flag = 2}
                                                                 sender.Tag = New Page With {.Content = frm}
                                                             End Sub

        Else
            AddHandler LoadRadio(G, "Safe Income View").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                     frm = New RPT4 With {.Flag = 1, .MyLinkFile = 5}
                                                                     sender.Tag = New Page With {.Content = frm}
                                                                 End Sub

            AddHandler LoadRadio(G, "Safe Outcome View").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                      frm = New RPT4 With {.Flag = 2, .MyLinkFile = 5}
                                                                      sender.Tag = New Page With {.Content = frm}
                                                                  End Sub

            AddHandler LoadRadio(G, "Bank Income View").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                     frm = New RPT4 With {.Flag = 3, .MyLinkFile = 6}
                                                                     sender.Tag = New Page With {.Content = frm}
                                                                 End Sub

            AddHandler LoadRadio(G, "Bank Outcome View").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                      frm = New RPT4 With {.Flag = 4, .MyLinkFile = 6}
                                                                      sender.Tag = New Page With {.Content = frm}
                                                                  End Sub
        End If

        If Md.ShowCostCenter Then
            AddHandler LoadRadio(G, "CostCenterOutCome").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                      frm = New RPT14 With {.Flag = 1}
                                                                      sender.Tag = New Page With {.Content = frm}
                                                                  End Sub

            AddHandler LoadRadio(G, "CostCenterOutComeToTal").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                           frm = New RPT14 With {.Flag = 2}
                                                                           sender.Tag = New Page With {.Content = frm}
                                                                       End Sub
        End If

        AddHandler LoadRadio(G, "Save Daily Motion").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                  frm = New RPT2 With {.Flag = 2, .MyLinkFile = 5}
                                                                  sender.Tag = New Page With {.Content = frm}
                                                              End Sub


        If Md.ShowCurrency Then
            AddHandler LoadRadio(G, "Currency Basket").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                    frm = New RPT28 With {.Flag = 1}
                                                                    sender.Tag = New Page With {.Content = frm}
                                                                End Sub
        End If

        AddHandler LoadRadio(G, "Statement of account").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                     frm = New RPT2 With {.Flag = 1}
                                                                     sender.Tag = New Page With {.Content = frm}
                                                                 End Sub

        AddHandler LoadRadio(G, "Balances").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                         frm = New RPT11 With {.Flag = 1}
                                                         sender.Tag = New Page With {.Content = frm}
                                                     End Sub


        AddHandler LoadRadio(G, "Assistant").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New RPT2 With {.Flag = 3}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        LoadLabel(G, "Final Accounts")

        AddHandler LoadRadio(G, "Account Balance").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                frm = New RPT20 With {.Flag = 1}
                                                                sender.Tag = New Page With {.Content = frm}
                                                            End Sub

        AddHandler LoadRadio(G, "Operating").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                          frm = New RPT27 With {.Flag = 1, .MyEndType = 0}
                                                          sender.Tag = New Page With {.Content = frm}
                                                      End Sub

        AddHandler LoadRadio(G, "Trading").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                        frm = New RPT27 With {.Flag = 1, .MyEndType = 1}
                                                        sender.Tag = New Page With {.Content = frm}
                                                    End Sub

        AddHandler LoadRadio(G, "Gains and losses").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                 frm = New RPT27 With {.Flag = 1, .MyEndType = 2}
                                                                 sender.Tag = New Page With {.Content = frm}
                                                             End Sub

        AddHandler LoadRadio(G, "Balance Sheet").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                              frm = New RPT27 With {.Flag = 1, .MyEndType = 3}
                                                              sender.Tag = New Page With {.Content = frm}
                                                          End Sub

        AddHandler LoadRadio(G, "Income Statement").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                 frm = New RPT27 With {.Flag = 2, .MyEndType = 2, .IsIncomeStatement = 1}
                                                                 sender.Tag = New Page With {.Content = frm}
                                                             End Sub

        AddHandler LoadRadio(G, "Financial Position").Checked, Sub(sender As Object, e As RoutedEventArgs)
                                                                   frm = New RPT27 With {.Flag = 3, .MyEndType = 3}
                                                                   sender.Tag = New Page With {.Content = frm}
                                                               End Sub

    End Sub


    Private Sub LoadTabs()

        LoadGFile()

        LoadGOperation()
        LoadGHR()
        LoadGAccountants()

        LoadGSecurity()

        LoadGOperationReports()
        LoadGHRReports()
        LoadGAccountsReports()

        bm.SetModem()

        'bm.GetTasks()
    End Sub


End Class

