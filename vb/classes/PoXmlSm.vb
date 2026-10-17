Imports System.Xml.Serialization
Namespace PoXmlSm

    <XmlRoot("document")> _
    Public Class PoXmlSm
        Public header As HeaderClass
        Public Class HeaderClass
            Public DocumentType As String
            Public CompanyName As String
        End Class
    End Class

    '''<remarks/>
    <System.SerializableAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True), _
     System.Xml.Serialization.XmlRootAttribute([Namespace]:="", IsNullable:=False)> _
    Partial Public Class xml

        Private documentField As xmlDocument

        '''<remarks/>
        Public Property document() As xmlDocument
            Get
                Return Me.documentField
            End Get
            Set(value As xmlDocument)
                Me.documentField = Value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.SerializableAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class xmlDocument

        Private headerField As xmlDocumentHeader

        Private bodyField() As xmlDocumentArticle

        Private footerField As xmlDocumentFooter

        '''<remarks/>
        Public Property header() As xmlDocumentHeader
            Get
                Return Me.headerField
            End Get
            Set(value As xmlDocumentHeader)
                Me.headerField = Value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlArrayItemAttribute("article", IsNullable:=False)> _
        Public Property body() As xmlDocumentArticle()
            Get
                Return Me.bodyField
            End Get
            Set(value As xmlDocumentArticle())
                Me.bodyField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property footer() As xmlDocumentFooter
            Get
                Return Me.footerField
            End Get
            Set(value As xmlDocumentFooter)
                Me.footerField = Value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.SerializableAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class xmlDocumentHeader

        Private documentTypeField As String

        Private companyNameField As String

        Private postDateField As String

        Private postTimeField As Date

        Private vendorNameField As String

        Private vendorCodeField As UInteger

        Private termsDiscountField As String

        Private siteCodeField As Byte

        Private siteNameField As String

        Private siteAddressField As String

        Private siteTINField As String

        Private documentNumberField As UInteger

        Private dateEntryField As String

        Private dateCancelField As String

        Private dateReceiptField As String

        Private articleCountField As Byte

        Private deliveryMessageField As String

        '''<remarks/>
        Public Property DocumentType() As String
            Get
                Return Me.documentTypeField
            End Get
            Set(value As String)
                Me.documentTypeField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property CompanyName() As String
            Get
                Return Me.companyNameField
            End Get
            Set(value As String)
                Me.companyNameField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property PostDate() As String
            Get
                Return Me.postDateField
            End Get
            Set(value As String)
                Me.postDateField = Value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="time")> _
        Public Property PostTime() As Date
            Get
                Return Me.postTimeField
            End Get
            Set(value As Date)
                Me.postTimeField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property VendorName() As String
            Get
                Return Me.vendorNameField
            End Get
            Set(value As String)
                Me.vendorNameField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property VendorCode() As UInteger
            Get
                Return Me.vendorCodeField
            End Get
            Set(value As UInteger)
                Me.vendorCodeField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property TermsDiscount() As String
            Get
                Return Me.termsDiscountField
            End Get
            Set(value As String)
                Me.termsDiscountField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property SiteCode() As Byte
            Get
                Return Me.siteCodeField
            End Get
            Set(value As Byte)
                Me.siteCodeField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property SiteName() As String
            Get
                Return Me.siteNameField
            End Get
            Set(value As String)
                Me.siteNameField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property SiteAddress() As String
            Get
                Return Me.siteAddressField
            End Get
            Set(value As String)
                Me.siteAddressField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property SiteTIN() As String
            Get
                Return Me.siteTINField
            End Get
            Set(value As String)
                Me.siteTINField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property DocumentNumber() As UInteger
            Get
                Return Me.documentNumberField
            End Get
            Set(value As UInteger)
                Me.documentNumberField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property DateEntry() As String
            Get
                Return Me.dateEntryField
            End Get
            Set(value As String)
                Me.dateEntryField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property DateCancel() As String
            Get
                Return Me.dateCancelField
            End Get
            Set(value As String)
                Me.dateCancelField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property DateReceipt() As String
            Get
                Return Me.dateReceiptField
            End Get
            Set(value As String)
                Me.dateReceiptField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property ArticleCount() As Byte
            Get
                Return Me.articleCountField
            End Get
            Set(value As Byte)
                Me.articleCountField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property DeliveryMessage() As String
            Get
                Return Me.deliveryMessageField
            End Get
            Set(value As String)
                Me.deliveryMessageField = Value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.SerializableAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class xmlDocumentArticle

        Private lineNumberField As Byte

        Private sKUMatCodeField As UInteger

        Private articleDescriptionField As xmlDocumentArticleArticleDescription

        Private uPCField As ULong

        Private buyQtyField As Decimal

        Private buyCostField As String

        Private buyUMField As String

        Private packageField As Integer

        Private sellUMField As String

        '''<remarks/>
        Public Property LineNumber() As Byte
            Get
                Return Me.lineNumberField
            End Get
            Set(value As Byte)
                Me.lineNumberField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property SKUMatCode() As UInteger
            Get
                Return Me.sKUMatCodeField
            End Get
            Set(value As UInteger)
                Me.sKUMatCodeField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property ArticleDescription() As xmlDocumentArticleArticleDescription
            Get
                Return Me.articleDescriptionField
            End Get
            Set(value As xmlDocumentArticleArticleDescription)
                Me.articleDescriptionField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property UPC() As ULong
            Get
                Return Me.uPCField
            End Get
            Set(value As ULong)
                Me.uPCField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property BuyQty() As Decimal
            Get
                Return Me.buyQtyField
            End Get
            Set(value As Decimal)
                Me.buyQtyField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property BuyCost() As String
            Get
                Return Me.buyCostField
            End Get
            Set(value As String)
                Me.buyCostField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property BuyUM() As String
            Get
                Return Me.buyUMField
            End Get
            Set(value As String)
                Me.buyUMField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property Package() As Integer
            Get
                Return Me.packageField
            End Get
            Set(value As Integer)
                Me.packageField = value
            End Set
        End Property

        '''<remarks/>
        Public Property SellUM() As String
            Get
                Return Me.sellUMField
            End Get
            Set(value As String)
                Me.sellUMField = Value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.SerializableAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class xmlDocumentArticleArticleDescription

        Private descriptionField As String

        Private discountField As String

        '''<remarks/>
        Public Property Description() As String
            Get
                Return Me.descriptionField
            End Get
            Set(value As String)
                Me.descriptionField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property Discount() As String
            Get
                Return Me.discountField
            End Get
            Set(value As String)
                Me.discountField = Value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.SerializableAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class xmlDocumentFooter

        Private totalAmountField() As String

        Private importantNotificationField As Object

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("TotalAmount")> _
        Public Property TotalAmount() As String()
            Get
                Return Me.totalAmountField
            End Get
            Set(value As String())
                Me.totalAmountField = Value
            End Set
        End Property

        '''<remarks/>
        Public Property ImportantNotification() As Object
            Get
                Return Me.importantNotificationField
            End Get
            Set(value As Object)
                Me.importantNotificationField = Value
            End Set
        End Property
    End Class



End Namespace
