Public Class PageSidebarMenu
    Public list_Parent As New List(Of cls_Parent)
    Public list_Child As New List(Of cls_Child)
    Private wlIcons As New wlIcons

    Public Sub Ini_Icons()
        wlIcons.LoadIcons()
    End Sub

    Public Sub AddParentMenu(ByVal ID As Integer, _
                                  ByVal MenuName As String, _
                                  ByVal MenuLink As String, _
                                  ByVal MenuIcon As wlIcons.wl_icons, _
                                  ByVal HasChild As Boolean)
        Dim Parent As New cls_Parent

        With Parent
            ._ID = ID
            ._Menu_Name = MenuName
            ._Menu_Link = MenuLink
            ._Menu_Icon = wlIcons.SearchIcons(MenuIcon)
            ._Has_Child = HasChild
        End With

        With list_Parent
            .Add(Parent)
        End With
    End Sub

    Public Sub AddChildMenu(ByVal ID As Integer, _
                            ByVal MenuName As String, _
                            ByVal MenuLink As String)
        Dim Child As New cls_Child

        With Child
            ._ID = ID
            ._Menu_Name = MenuName
            ._Menu_Link = MenuLink
        End With
        list_Child.Add(Child)
    End Sub
End Class

#Region "wlIcons"
Public Class wlIcons
    Public Enum wl_icons
        enm_i_house
        enm_i_cog_4
        enm_i_graph
        enm_i_books
        enm_i_airplane
    End Enum

    Private wl_icon As New Dictionary(Of wl_icons, String)

    Public Function LoadIcons() As Dictionary(Of wl_icons, String)
        With wl_icon
            .Add(wl_icons.enm_i_house, "i_house")
            .Add(wl_icons.enm_i_cog_4, "i_cog_4")
            .Add(wl_icons.enm_i_graph, "i_graph")
            .Add(wl_icons.enm_i_books, "i_books")
            .Add(wl_icons.enm_i_airplane, "i_airplane")
        End With
        Return wl_icon
    End Function

    Public Function SearchIcons(ByVal searchIcon As wl_icons) As String
        Dim iconString As String

        iconString = wl_icon.Item(searchIcon).ToString
        Return iconString
    End Function
End Class
#End Region

#Region "Parent and Child Class"
Public Class cls_Parent
    Private int_ID As Integer
    Private str_Menu_Name As String
    Private str_Menu_Link As String
    Private str_Menu_Icon As String
    Private bl_Has_Child As Boolean

    Public Property _ID() As Integer
        Get
            Return int_ID
        End Get
        Set(ByVal value As Integer)
            int_ID = value
        End Set
    End Property

    Public Property _Menu_Name() As String
        Get
            Return str_Menu_Name
        End Get
        Set(ByVal value As String)
            str_Menu_Name = value
        End Set
    End Property

    Public Property _Menu_Link() As String
        Get
            Return str_Menu_Link
        End Get
        Set(ByVal value As String)
            str_Menu_Link = value
        End Set
    End Property

    Public Property _Menu_Icon() As String
        Get
            Return str_Menu_Icon
        End Get
        Set(ByVal value As String)
            str_Menu_Icon = value
        End Set
    End Property

    Public Property _Has_Child() As Boolean
        Get
            Return bl_Has_Child
        End Get
        Set(ByVal value As Boolean)
            bl_Has_Child = value
        End Set
    End Property

End Class

Public Class cls_Child
    Private int_ID As Integer
    Private str_Menu_Name As String
    Private str_Menu_Link As String

    Public Property _ID() As Integer
        Get
            Return int_ID
        End Get
        Set(ByVal value As Integer)
            int_ID = value
        End Set
    End Property

    Public Property _Menu_Name() As String
        Get
            Return str_Menu_Name
        End Get
        Set(ByVal value As String)
            str_Menu_Name = value
        End Set
    End Property

    Public Property _Menu_Link() As String
        Get
            Return str_Menu_Link
        End Get
        Set(ByVal value As String)
            str_Menu_Link = value
        End Set
    End Property
End Class
#End Region

Public Class WebErpSideBar

    Public Shared Sub Init(ByRef oParents As List(Of cls_Parent), ByRef oChildren As List(Of cls_Child))

        Dim PageMenu As New PageSidebarMenu
        With PageMenu
            .Ini_Icons()
            '.AddParentMenu(1, "Dashboard", "", wlIcons.wl_icons.enm_i_house, True)
            '.AddChildMenu(1, "Branch", "BranchDashboard")
            '.AddChildMenu(1, "Customer Service Level", "Reports/CustomerServiceLevel")
            '.AddParentMenu(2, "DRP", "drp/StockSched", wlIcons.wl_icons.enm_i_airplane, False)
            '.AddParentMenu(3, "Reports", "reports", wlIcons.wl_icons.enm_i_graph, False)
            '.AddParentMenu(4, "Libraries", "", wlIcons.wl_icons.enm_i_books, True)
            '.AddChildMenu(4, "Currencies", "LibCurrencies")
            '.AddParentMenu(5, "CRM", "CRM", wlIcons.wl_icons.enm_i_books, False)
            .AddParentMenu(1, "Sales", "Sales", wlIcons.wl_icons.enm_i_airplane, False)
            .AddParentMenu(2, "Marketing", "Marketing", wlIcons.wl_icons.enm_i_airplane, False)
            .AddParentMenu(3, "Purchasing", "Purchasing", wlIcons.wl_icons.enm_i_airplane, False)
            .AddParentMenu(4, "Warehouse and Logistics", "WarehouseAndLogistics", wlIcons.wl_icons.enm_i_airplane, False)
            .AddParentMenu(5, "Human Resource", "HumanResource", wlIcons.wl_icons.enm_i_airplane, False)
            .AddParentMenu(6, "Management", "Management", wlIcons.wl_icons.enm_i_airplane, False)
            oParents = .list_Parent
            oChildren = .list_Child
        End With

    End Sub
End Class