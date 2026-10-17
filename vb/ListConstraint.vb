Public Enum eListConstraintType
    Include
    Exclude
End Enum

Public Class ListConstraint
    Implements IRouteConstraint

    Dim mListConstraintType As eListConstraintType
    Dim mList As String()

    Public Sub New(vListConstraintType As eListConstraintType, ByVal ParamArray vList As String())

        mListConstraintType = vListConstraintType
        mList = vList

    End Sub

    Public Function Match(httpContext As System.Web.HttpContextBase, route As System.Web.Routing.Route, parameterName As String, values As System.Web.Routing.RouteValueDictionary, routeDirection As System.Web.Routing.RouteDirection) As Boolean Implements System.Web.Routing.IRouteConstraint.Match

        Dim pMatches = (From pValue As String In mList
            Where pValue.ToLower = values(parameterName.ToLower).ToString.ToLower).ToList()

        If mListConstraintType = eListConstraintType.Include Then
            Return pMatches.Count > 0
        Else
            Return pMatches.Count = 0
        End If

    End Function
End Class

' no use for now
'Private Class ListConstraints

'    Dim mDummy As ListConstraint
'    Public Property Dummy As ListConstraint
'        Get

'            If mDummy Is Nothing Then
'                mDummy = New ListConstraint
'            End If

'            Return mDummy

'        End Get
'        Set(value As ListConstraint)
'            Stop2()
'        End Set
'    End Property

'End Class

' no use for now
'Private Class ProRouteHandler
'    Implements IRouteHandler

'    Public Function GetHttpHandler(requestContext As System.Web.Routing.RequestContext) As System.Web.IHttpHandler _
'        Implements System.Web.Routing.IRouteHandler.GetHttpHandler
'        ' http://stackoverflow.com/questions/4137878/one-controller-for-whole-asp-net-mvc-application/4139028#4139028

'        Dim pHandler As IRouteHandler
'        Dim pValues As RouteValueDictionary
'        Dim pOut As IHttpHandler

'        pValues = requestContext.RouteData.Values
'        pHandler = New MvcRouteHandler
'        pOut = pHandler.GetHttpHandler(requestContext)
'        If requestContext.HttpContext.Handler Is Nothing Then
'            pValues.Remove("action")
'            pValues("action") = "Index"
'        End If

'        Return pOut

'    End Function
'End Class
