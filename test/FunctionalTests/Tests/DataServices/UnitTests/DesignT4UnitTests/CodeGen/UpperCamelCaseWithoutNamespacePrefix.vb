'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.34014
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On


'Generation date: 2/9/2015 3:43:43 PM
Namespace [Namespace].Foo
    '''<summary>
    '''There are no comments for BaseTypeSingle in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("baseTypeSingle")>  _
    Partial Public Class BaseTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of BaseType)
        ''' <summary>
        ''' Initialize a new BaseTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new BaseTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new BaseTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of BaseType))
            MyBase.New(query)
        End Sub
    End Class
    '''<summary>
    '''There are no comments for BaseType in the schema.
    '''</summary>
    '''<KeyProperties>
    '''KeyProp
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("keyProp")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("baseType")>  _
    Partial Public Class BaseType
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        '''<summary>
        '''Create a new BaseType object.
        '''</summary>
        '''<param name="keyProp">Initial value of KeyProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Public Shared Function CreateBaseType(ByVal keyProp As Integer) As BaseType
            Dim baseType As BaseType = New BaseType()
            baseType.KeyProp = keyProp
            Return baseType
        End Function
        '''<summary>
        '''There are no comments for Property KeyProp in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("keyProp")>  _
        Public Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
    End Class
    '''<summary>
    '''There are no comments for TestTypeSingle in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("testTypeSingle")>  _
    Partial Public Class TestTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType)
        ''' <summary>
        ''' Initialize a new TestTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new TestTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new TestTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType))
            MyBase.New(query)
        End Sub
        '''<summary>
        '''There are no comments for SingleType in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("singleType")>  _
        Public ReadOnly Property SingleType() As [Namespace].Foo.SingleTypeSingle
            Get
                If Not Me.IsComposable Then
                    Throw New Global.System.NotSupportedException("The previous function is not composable.")
                End If
                If (Me._SingleType Is Nothing) Then
                    Me._SingleType = New [Namespace].Foo.SingleTypeSingle(Me.Context, GetPath("singleType"))
                End If
                Return Me._SingleType
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _SingleType As [Namespace].Foo.SingleTypeSingle
    End Class
    '''<summary>
    '''There are no comments for TestType in the schema.
    '''</summary>
    '''<KeyProperties>
    '''KeyProp
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("keyProp")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("testType")>  _
    Partial Public Class TestType
        Inherits BaseType
        '''<summary>
        '''Create a new TestType object.
        '''</summary>
        '''<param name="keyProp">Initial value of KeyProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Public Shared Function CreateTestType(ByVal keyProp As Integer) As TestType
            Dim testType As TestType = New TestType()
            testType.KeyProp = keyProp
            Return testType
        End Function
        '''<summary>
        '''There are no comments for Property SingleType in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("singleType")>  _
        Public Property SingleType() As [Namespace].Foo.SingleType
            Get
                Return Me._SingleType
            End Get
            Set
                Me.OnSingleTypeChanging(value)
                Me._SingleType = value
                Me.OnSingleTypeChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _SingleType As [Namespace].Foo.SingleType
        Partial Private Sub OnSingleTypeChanging(ByVal value As [Namespace].Foo.SingleType)
        End Sub
        Partial Private Sub OnSingleTypeChanged()
        End Sub
    End Class
    '''<summary>
    '''There are no comments for SingleTypeSingle in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("singleTypeSingle")>  _
    Partial Public Class SingleTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of SingleType)
        ''' <summary>
        ''' Initialize a new SingleTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new SingleTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new SingleTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of SingleType))
            MyBase.New(query)
        End Sub
        '''<summary>
        '''There are no comments for BaseSet in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("baseSet")>  _
        Public ReadOnly Property BaseSet() As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.TestType)
            Get
                If Not Me.IsComposable Then
                    Throw New Global.System.NotSupportedException("The previous function is not composable.")
                End If
                If (Me._BaseSet Is Nothing) Then
                    Me._BaseSet = Context.CreateQuery(Of [Namespace].Foo.TestType)(GetPath("baseSet"))
                End If
                Return Me._BaseSet
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _BaseSet As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.TestType)
    End Class
    '''<summary>
    '''There are no comments for SingleType in the schema.
    '''</summary>
    '''<KeyProperties>
    '''KeyProp
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("keyProp")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("singleType")>  _
    Partial Public Class SingleType
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        '''<summary>
        '''Create a new SingleType object.
        '''</summary>
        '''<param name="keyProp">Initial value of KeyProp.</param>
        '''<param name="colorProp">Initial value of ColorProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Public Shared Function CreateSingleType(ByVal keyProp As Integer, ByVal colorProp As [Namespace].Foo.Color) As SingleType
            Dim singleType As SingleType = New SingleType()
            singleType.KeyProp = keyProp
            singleType.ColorProp = colorProp
            Return singleType
        End Function
        '''<summary>
        '''There are no comments for Property KeyProp in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("keyProp")>  _
        Public Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ColorProp in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("colorProp")>  _
        Public Property ColorProp() As [Namespace].Foo.Color
            Get
                Return Me._ColorProp
            End Get
            Set
                Me.OnColorPropChanging(value)
                Me._ColorProp = value
                Me.OnColorPropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _ColorProp As [Namespace].Foo.Color
        Partial Private Sub OnColorPropChanging(ByVal value As [Namespace].Foo.Color)
        End Sub
        Partial Private Sub OnColorPropChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property BaseSet in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("baseSet")>  _
        Public Property BaseSet() As Global.System.Collections.ObjectModel.Collection(Of [Namespace].Foo.TestType)
            Get
                Return Me._BaseSet
            End Get
            Set
                Me.OnBaseSetChanging(value)
                Me._BaseSet = value
                Me.OnBaseSetChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _BaseSet As Global.System.Collections.ObjectModel.Collection(Of [Namespace].Foo.TestType) = New Global.System.Collections.ObjectModel.Collection(Of [Namespace].Foo.TestType)()
        Partial Private Sub OnBaseSetChanging(ByVal value As Global.System.Collections.ObjectModel.Collection(Of [Namespace].Foo.TestType))
        End Sub
        Partial Private Sub OnBaseSetChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Foo7 in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("foo7")>  _
        Public Function Foo7() As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))
            Dim resource As Global.Microsoft.OData.Client.EntityDescriptor = Context.EntityTracker.TryGetEntityDescriptor(Me)
            If resource Is Nothing Then
                Throw New Global.System.Exception("cannot find entity")
            End If

            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))(Me.Context, resource.EditLink.OriginalString.Trim("/"C) + "/namespace.foo.foo7")
        End Function
    End Class
    '''<summary>
    '''There are no comments for Color in the schema.
    '''</summary>
    <Global.System.Flags()>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("color")>  _
    Public Enum Color
        <Global.Microsoft.OData.Client.OriginalNameAttribute("red")>  _
        Red = 0
        <Global.Microsoft.OData.Client.OriginalNameAttribute("white")>  _
        White = 1
        <Global.Microsoft.OData.Client.OriginalNameAttribute("blue")>  _
        Blue = 2
    End Enum
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.BaseType as [Namespace].Foo.BaseTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.BaseType), ByVal keys As Global.System.Collections.Generic.Dictionary(Of String, Object)) As [Namespace].Foo.BaseTypeSingle
            Return New [Namespace].Foo.BaseTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.BaseType as [Namespace].Foo.BaseTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.BaseType),
            keyProp As Integer) As [Namespace].Foo.BaseTypeSingle
            Dim keys As Global.System.Collections.Generic.Dictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "keyProp", keyProp }
            }
            Return New [Namespace].Foo.BaseTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.TestType as [Namespace].Foo.TestTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.TestType), ByVal keys As Global.System.Collections.Generic.Dictionary(Of String, Object)) As [Namespace].Foo.TestTypeSingle
            Return New [Namespace].Foo.TestTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.TestType as [Namespace].Foo.TestTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.TestType),
            keyProp As Integer) As [Namespace].Foo.TestTypeSingle
            Dim keys As Global.System.Collections.Generic.Dictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "keyProp", keyProp }
            }
            Return New [Namespace].Foo.TestTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Cast an entity of type [Namespace].Foo.BaseType to its derived type [Namespace].Foo.TestType
        ''' </summary>
        ''' <param name="source">source entity</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function CastToTestType(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [Namespace].Foo.BaseType)) As [Namespace].Foo.TestTypeSingle
            Dim query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [Namespace].Foo.TestType) = source.CastTo(Of [Namespace].Foo.TestType)()
            Return New [Namespace].Foo.TestTypeSingle(source.Context, query.GetPath(Nothing))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.SingleType as [Namespace].Foo.SingleTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.SingleType), ByVal keys As Global.System.Collections.Generic.Dictionary(Of String, Object)) As [Namespace].Foo.SingleTypeSingle
            Return New [Namespace].Foo.SingleTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.SingleType as [Namespace].Foo.SingleTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.SingleType),
            keyProp As Integer) As [Namespace].Foo.SingleTypeSingle
            Dim keys As Global.System.Collections.Generic.Dictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "keyProp", keyProp }
            }
            Return New [Namespace].Foo.SingleTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' There are no comments for Foo7 in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("foo7")>  _
        Public Function Foo7(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [Namespace].Foo.SingleType)) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))(source.Context, source.AppendRequestUri("namespace.foo.foo7"))
        End Function
    End Module
End Namespace
Namespace [Namespace].Bar
    '''<summary>
    '''There are no comments for SingletonContainer in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("singletonContainer")>  _
    Partial Public Class SingletonContainer
        Inherits Global.Microsoft.OData.Client.DataServiceContext
        '''<summary>
        '''Initialize a new SingletonContainer object.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Public Sub New(ByVal serviceRoot As Global.System.Uri)
            MyBase.New(serviceRoot, Global.Microsoft.OData.Client.ODataProtocolVersion.V4)
            Me.ResolveName = AddressOf Me.ResolveNameFromType
            Me.ResolveType = AddressOf Me.ResolveTypeFromName
            Me.OnContextCreated
            Me.Format.LoadServiceModel = AddressOf GeneratedEdmModel.GetInstance
            Me.Format.UseJson()
        End Sub
        Partial Private Sub OnContextCreated()
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private Shared ROOTNAMESPACE As String = GetType(SingletonContainer).Namespace.Remove(GetType(SingletonContainer).Namespace.LastIndexOf("Namespace.Bar"))
        '''<summary>
        '''Since the namespace configured for this service reference
        '''in Visual Studio is different from the one indicated in the
        '''server schema, use type-mappers to map between the two.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Protected Function ResolveTypeFromName(ByVal typeName As String) As Global.System.Type
            Dim resolvedType As Global.System.Type = Me.DefaultResolveType(typeName, "namespace.bar", String.Concat(ROOTNAMESPACE, "Namespace.Bar"))
            If (Not (resolvedType) Is Nothing) Then
                Return resolvedType
            End If
            resolvedType = Me.DefaultResolveType(typeName, "namespace.foo", String.Concat(ROOTNAMESPACE, "Namespace.Foo"))
            If (Not (resolvedType) Is Nothing) Then
                Return resolvedType
            End If
            Return Nothing
        End Function
        '''<summary>
        '''Since the namespace configured for this service reference
        '''in Visual Studio is different from the one indicated in the
        '''server schema, use type-mappers to map between the two.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Protected Function ResolveNameFromType(ByVal clientType As Global.System.Type) As String
            Dim originalNameAttribute As Global.Microsoft.OData.Client.OriginalNameAttribute =
                CType(Global.System.Linq.Enumerable.SingleOrDefault(Global.Microsoft.OData.Client.Utility.GetCustomAttributes(clientType, GetType(Global.Microsoft.OData.Client.OriginalNameAttribute), true)), Global.Microsoft.OData.Client.OriginalNameAttribute)
            If clientType.Namespace.Equals(String.Concat(ROOTNAMESPACE, "Namespace.Bar"), Global.System.StringComparison.OrdinalIgnoreCase) Then
                If (Not (originalNameAttribute) Is Nothing) Then
                    Return String.Concat("namespace.bar.", originalNameAttribute.OriginalName)
                End If
                Return String.Concat("namespace.bar.", clientType.Name)
            End If
            If clientType.Namespace.Equals(String.Concat(ROOTNAMESPACE, "Namespace.Foo"), Global.System.StringComparison.OrdinalIgnoreCase) Then
                If (Not (originalNameAttribute) Is Nothing) Then
                    Return String.Concat("namespace.foo.", originalNameAttribute.OriginalName)
                End If
                Return String.Concat("namespace.foo.", clientType.Name)
            End If
            If (Not (originalNameAttribute) Is Nothing) Then
                Dim fullName As String = clientType.FullName.Substring(ROOTNAMESPACE.Length)
                Return fullName.Remove(fullName.LastIndexOf(clientType.Name)) + originalNameAttribute.OriginalName
            End If
            Return clientType.FullName.Substring(ROOTNAMESPACE.Length)
        End Function
        '''<summary>
        '''There are no comments for TestTypeSet in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("testTypeSet")>  _
        Public ReadOnly Property TestTypeSet() As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.TestType)
            Get
                If (Me._TestTypeSet Is Nothing) Then
                    Me._TestTypeSet = MyBase.CreateQuery(Of [Namespace].Foo.TestType)("testTypeSet")
                End If
                Return Me._TestTypeSet
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _TestTypeSet As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.TestType)
        '''<summary>
        '''There are no comments for BaseTypeSet in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("baseTypeSet")>  _
        Public ReadOnly Property BaseTypeSet() As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.BaseType)
            Get
                If (Me._BaseTypeSet Is Nothing) Then
                    Me._BaseTypeSet = MyBase.CreateQuery(Of [Namespace].Foo.BaseType)("baseTypeSet")
                End If
                Return Me._BaseTypeSet
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _BaseTypeSet As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.BaseType)
        '''<summary>
        '''There are no comments for TestTypeSet in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Public Sub AddToTestTypeSet(ByVal testType As [Namespace].Foo.TestType)
            MyBase.AddObject("testTypeSet", testType)
        End Sub
        '''<summary>
        '''There are no comments for BaseTypeSet in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Public Sub AddToBaseTypeSet(ByVal baseType As [Namespace].Foo.BaseType)
            MyBase.AddObject("baseTypeSet", baseType)
        End Sub
        '''<summary>
        '''There are no comments for SuperType in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("superType")>  _
        Public ReadOnly Property SuperType() As [Namespace].Foo.TestTypeSingle
            Get
                If (Me._SuperType Is Nothing) Then
                    Me._SuperType = New [Namespace].Foo.TestTypeSingle(Me, "superType")
                End If
                Return Me._SuperType
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _SuperType As [Namespace].Foo.TestTypeSingle
        '''<summary>
        '''There are no comments for Single in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("single")>  _
        Public ReadOnly Property [Single]() As [Namespace].Foo.SingleTypeSingle
            Get
                If (Me._Single Is Nothing) Then
                    Me._Single = New [Namespace].Foo.SingleTypeSingle(Me, "single")
                End If
                Return Me._Single
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private _Single As [Namespace].Foo.SingleTypeSingle
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
        Private MustInherit Class GeneratedEdmModel
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
            Private Shared ParsedModel As Global.Microsoft.OData.Edm.IEdmModel = LoadModelFromString
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
            Private Const Edmx As String = "<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">" & _
 "  <edmx:DataServices>" & _
 "    <Schema Namespace=""namespace.foo"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityType Name=""baseType"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""keyProp"" />" & _
 "        </Key>" & _
 "        <Property Name=""keyProp"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <EntityType Name=""testType"" BaseType=""namespace.foo.baseType"">" & _
 "        <NavigationProperty Name=""singleType"" Type=""namespace.foo.singleType"" />" & _
 "      </EntityType>" & _
 "      <EntityType Name=""singleType"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""keyProp"" />" & _
 "        </Key>" & _
 "        <Property Name=""keyProp"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "        <Property Name=""colorProp"" Type=""namespace.foo.color"" Nullable=""false"" />" & _
 "        <NavigationProperty Name=""baseSet"" Type=""Collection(namespace.foo.testType)"" />" & _
 "      </EntityType>" & _
 "      <EnumType Name=""color"" UnderlyingType=""Edm.Int32"" IsFlags=""true"">" & _
 "        <Member Name=""red"" />" & _
 "        <Member Name=""white"" />" & _
 "        <Member Name=""blue"" />" & _
 "      </EnumType>" & _
 "      <Function Name=""foo6"">" & _
 "        <Parameter Name=""p1"" Type=""Collection(namespace.foo.testType)"" />" & _
 "        <ReturnType Type=""Edm.String"" />" & _
 "      </Function>" & _
 "      <Action Name=""foo7"" IsBound=""True"">" & _
 "        <Parameter Name=""p1"" Type=""namespace.foo.singleType"" />" & _
 "        <ReturnType Type=""Edm.Int32"" />" & _
 "      </Action>" & _
 "    </Schema>" & _
 "    <Schema Namespace=""namespace.bar"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityContainer Name=""singletonContainer"">" & _
 "        <EntitySet Name=""testTypeSet"" EntityType=""namespace.foo.testType"" />" & _
 "        <EntitySet Name=""baseTypeSet"" EntityType=""namespace.foo.baseType"" />" & _
 "        <Singleton Name=""superType"" Type=""namespace.foo.testType"" />" & _
 "        <Singleton Name=""single"" Type=""namespace.foo.singleType"" />" & _
 "        <FunctionImport Name=""foo6"" Function=""namespace.foo.foo6"" />" & _
 "      </EntityContainer>" & _
 "    </Schema>" & _
 "  </edmx:DataServices>" & _
 "</edmx:Edmx>"
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
            Public Shared Function GetInstance() As Global.Microsoft.OData.Edm.IEdmModel
                Return ParsedModel
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
            Private Shared Function LoadModelFromString() As Global.Microsoft.OData.Edm.IEdmModel
                Dim reader As Global.System.Xml.XmlReader = CreateXmlReader(Edmx)
                Try
                    Return Global.Microsoft.OData.Edm.Csdl.EdmxReader.Parse(reader)
                Finally
                    CType(reader,Global.System.IDisposable).Dispose
                End Try
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.2.0")>  _
            Private Shared Function CreateXmlReader(ByVal edmxToParse As String) As Global.System.Xml.XmlReader
                Return Global.System.Xml.XmlReader.Create(New Global.System.IO.StringReader(edmxToParse))
            End Function
        End Class
        ''' <summary>
        ''' There are no comments for Foo6 in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("foo6")>  _
        Public Function Foo6(p1 As Global.System.Collections.Generic.ICollection(Of [Namespace].Foo.TestType), Optional ByVal useEntityReference As Boolean = False) As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of String)
            Return Me.CreateFunctionQuerySingle(Of String)("", "/foo6", False, New Global.Microsoft.OData.Client.UriEntityOperationParameter("p1", p1, useEntityReference))
        End Function
    End Class
End Namespace
