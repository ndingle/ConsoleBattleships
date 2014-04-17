Public Class BattleshipConsole

    Private Class OverlayObject

        Private _x As Integer
        Private _y As Integer
        Private _width As Integer
        Private _height As Integer
        Private _text As String

        Private _lifespan As Double
        Private _timer As Timers.Timer
        Private _destroyDelegate As Timers.ElapsedEventHandler

        Sub New(x As Integer, y As Integer, lifespan As Double, Optional width As Integer = 0, Optional height As Integer = 0, Optional text As String = "")

            'Create the objects we need
            Me.X = x
            Me.Y = y
            Me.Width = width
            Me.Height = height
            Me.Text = text

            'Create the timer object
            _destroyDelegate = New Timers.ElapsedEventHandler(AddressOf Me.Destroy)
            _timer = New Timers.Timer(lifespan)
            AddHandler _timer.Elapsed, _destroyDelegate
            _timer.Enabled = True

        End Sub

        Private Sub Destroy()

            'Shutdown the object
            _timer.Enabled = False
            RemoveHandler _timer.Elapsed, _destroyDelegate
            _timer.Dispose()

            'Destroy the object
            MsgBox("hello")

        End Sub

        Public Property Text As String
            Set(value As String)
                If Not Text Is Nothing Then
                    _text = value
                End If
            End Set
            Get
                Return _text
            End Get
        End Property

        Public Property X As Integer
            Set(value As Integer)
                If value >= 0 And value < CONSOLE_WIDTH Then
                    _x = value
                End If
            End Set
            Get
                Return _x
            End Get
        End Property

        Public Property Y As Integer
            Set(value As Integer)
                If value >= 0 And value < CONSOLE_HEIGHT Then
                    _y = value
                End If
            End Set
            Get
                Return _y
            End Get
        End Property

        Public Property Width As Integer
            Set(value As Integer)
                If value >= 0 And value < CONSOLE_WIDTH Then
                    _width = value
                End If
            End Set
            Get
                Return _width
            End Get
        End Property

        Public Property Height As Integer
            Set(value As Integer)
                If value >= 0 And value < CONSOLE_HEIGHT Then
                    _height = value
                End If
            End Set
            Get
                Return _height
            End Get
        End Property
    End Class

    Private Class ConsoleCharacter

        Private _foregroundColour As ConsoleColor = -1
        Private _backgroundColour As ConsoleColor = -1
        Private _character As Char = ""
        Private _update As Boolean = True

        Public Property BackgroundColour As ConsoleColor
            Set(value As ConsoleColor)
                If value <> _backgroundColour Then
                    _backgroundColour = value
                    _update = True
                End If
            End Set
            Get
                Return _backgroundColour
            End Get
        End Property

        Public Property ForegroundColour As ConsoleColor
            Set(value As ConsoleColor)
                If value <> _foregroundColour Then
                    _foregroundColour = value
                    _update = True
                End If
            End Set
            Get
                Return _foregroundColour
            End Get
        End Property

        Public Property Character As Char
            Set(value As Char)
                If _character <> value Then
                    _character = value
                    _update = True
                End If
            End Set
            Get
                Return _character
            End Get
        End Property

        Public Property Update As Boolean
            Set(value As Boolean)
                _update = value
            End Set
            Get
                Return _update
            End Get
        End Property

        Sub New(background As ConsoleColor, foreground As ConsoleColor)
            BackgroundColour = background
            ForegroundColour = foreground
        End Sub

        Public Sub Draw(x As Integer, y As Integer)

            If Update Then

                'Ensure the coords are in the bounds
                If x >= 0 And x <= Console.BufferWidth Then

                    If y >= 0 And y <= Console.BufferHeight Then
                        'Setup the colours ay
                        If ForegroundColour <> Console.ForegroundColor And ForegroundColour > -1 Then
                            Console.ForegroundColor = ForegroundColour
                        End If

                        If BackgroundColour <> Console.BackgroundColor And BackgroundColour > -1 Then
                            Console.BackgroundColor = BackgroundColour
                        End If

                        Console.SetCursorPosition(x, y)
                        Console.Write(_character)
                        _update = False
                    End If

                End If

            End If

        End Sub

    End Class

    Private Class ConsolePosition

        Public X As Integer = 0
        Public Y As Integer = 0

        Sub New(Optional x As Integer = 0, Optional y As Integer = 0)
            Me.X = x
            Me.Y = y
        End Sub

    End Class

    Private Const CONSOLE_WIDTH As Integer = 80
    Private Const CONSOLE_HEIGHT As Integer = 26

    'Contents of the console display
    Private _contents(CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1) As ConsoleCharacter
    Private _overlay(CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1) As ConsoleCharacter

    Private _overlayObjects As List(Of OverlayObject)

    Private _backgroundColour As ConsoleColor
    Private _foregroundColour As ConsoleColor
    Private _cursorColour As ConsoleColor

    Private _cursor As ConsolePosition
    Private _pauseRefresh As Boolean = False

    Public Event OnEnterPressed()

    Sub New(Optional background As ConsoleColor = ConsoleColor.Black, Optional foreground As ConsoleColor = ConsoleColor.Gray, Optional cursor As ConsoleColor = ConsoleColor.Red)

        'Set the colours
        _backgroundColour = background
        _foregroundColour = foreground
        _cursorColour = cursor

        'Don't create new _overlay objects as we want them empty if not used
        'Create the new objects
        For i = 0 To CONSOLE_WIDTH - 1
            For j = 0 To CONSOLE_HEIGHT - 1
                Content(i, j) = New ConsoleCharacter(_backgroundColour, _foregroundColour)
            Next
        Next

        _cursor = New ConsolePosition()
        _overlayObjects = New List(Of OverlayObject)

        'Other default settings
        Console.SetBufferSize(CONSOLE_WIDTH, CONSOLE_HEIGHT)
        Console.CursorVisible = False

        'Refresh the screen and set the cursor
        Me.Refresh()

    End Sub

    Public Sub Refresh()

        'do they want to stifle the refreshing
        If Not _pauseRefresh Then

            'Full
            For i = 0 To CONSOLE_WIDTH - 1
                For j = 0 To CONSOLE_HEIGHT - 1

                    'For some reason I can't draw the last character
                    If i = CONSOLE_WIDTH - 1 And j = CONSOLE_HEIGHT - 1 Then Exit For

                    'Check if we have an overlay to do
                    If Overlay(i, j) Is Nothing Then
                        Content(i, j).Draw(i, j)
                    Else
                        'Overlay's always update
                        Overlay(i, j).Draw(i, j)
                        Overlay(i, j).Update = True
                    End If

                Next
            Next

            'Draw the cursor
            Console.SetCursorPosition(_cursor.X, _cursor.Y)
            Console.BackgroundColor = CursorColour
            Console.ForegroundColor = Content(_cursor).ForegroundColour

            'Where is the char coming from
            If Overlay(_cursor) Is Nothing Then
                Console.Write(Content(_cursor).Character)
            Else
                Console.Write(Overlay(_cursor).Character)
            End If

            Console.SetCursorPosition(_cursor.X, _cursor.Y)

        End If

    End Sub

    Private Function CutStrings(text As String, delimiter As String) As String()

        If Not text Is Nothing Then

            'If we have a background and foreground then do something
            Dim lines() As String = text.Split("\n")

            'Remove the extra characters
            For i = 1 To lines.Length - 1
                lines(i) = lines(i).Remove(0, "\n".Length - 1)
            Next

            'Return our result
            Return lines

        Else

            Return Nothing

        End If

    End Function

    Public Sub Write(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

        'Call our write function with the content
        Write(_contents, x, y, text, background, foreground)

    End Sub

    Public Sub WriteOverlay(x As Integer, y As Integer, text As String, lifespan As Double, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

        'Call our write function with the content
        Write(_overlay, x, y, text, background, foreground)

        'Add in a new overlay object
        _overlayObjects.Add(New OverlayObject(x, y, lifespan, 0, 0, text))

    End Sub

    Private Sub Write(chars(,) As ConsoleCharacter, x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

        Dim lines() As String = CutStrings(text, "\n")

        'Add the text in
        For j = y To y + lines.Length - 1

            If j >= 0 And j < CONSOLE_HEIGHT Then

                For i = x To x + lines(j - y).Length - 1

                    'Ensure we don't go over the buffer
                    If i < CONSOLE_WIDTH Then

                        'Check that we have an object
                        If chars(i, j) Is Nothing Then chars(i, j) = New ConsoleCharacter(-1, -1)

                        'Copy the data into the cell
                        If foreground > -1 Then
                            chars(i, j).ForegroundColour = foreground
                        End If
                        If background > -1 Then
                            chars(i, j).BackgroundColour = background
                        End If
                        chars(i, j).Character = lines(j - y)(i - x)

                    End If

                Next

            End If

        Next

        Me.Refresh()

    End Sub

    Public Sub CursorMovement()

        'Wait for the keypress
        Dim key As ConsoleKeyInfo = Console.ReadKey

        'Ensure it's an arrow key
        If key.Key >= 37 And key.Key <= 40 Then

            'Force the update
            Content(_cursor).Update = True

            'Change it's position
            If key.Key = ConsoleKey.UpArrow Then
                If _cursor.Y > 0 Then
                    _cursor.Y -= 1
                End If
            ElseIf key.Key = ConsoleKey.DownArrow Then
                If _cursor.Y < CONSOLE_HEIGHT - 1 Then
                    _cursor.Y += 1
                End If
            ElseIf key.Key = ConsoleKey.LeftArrow Then
                If _cursor.X > 0 Then
                    _cursor.X -= 1
                End If
            ElseIf key.Key = ConsoleKey.RightArrow Then
                If _cursor.X < CONSOLE_WIDTH - 1 Then
                    _cursor.X += 1
                End If
            End If

        End If

        'If they pressed enter, tell the developer
        If key.Key = ConsoleKey.Enter Then
            RaiseEvent OnEnterPressed()
        End If

        Me.Refresh()

    End Sub

    Private Sub DrawBorder(chars(,) As ConsoleCharacter, x As Integer, y As Integer, width As Integer, height As Integer, colour As ConsoleColor, Optional border As Integer = 1)

        'Check we have good values
        If width > 0 And height > 0 Then

            'Top border
            For i = x To x + width

                For j = y To y + border - 1

                    'Check that we have an object
                    If chars(i, j) Is Nothing Then chars(i, j) = New ConsoleCharacter(-1, -1)

                    'Draw the top line of the border
                    If i >= 0 And i < CONSOLE_WIDTH And j >= 0 And j < CONSOLE_HEIGHT Then
                        chars(i, j).BackgroundColour = colour
                    End If

                Next

            Next

            'Walls
            For j = y + 1 To y + height - 1

                'Left wall
                For i = x To x + border - 1

                    'Check that we have an object
                    If chars(i, j) Is Nothing Then chars(i, j) = New ConsoleCharacter(-1, -1)

                    If j >= 0 And j < CONSOLE_HEIGHT And i >= 0 And i < CONSOLE_WIDTH Then
                        chars(i, j).BackgroundColour = colour
                    End If

                Next

                'Right wall
                For i = x - (border - 1) To x

                    'Check that we have an object
                    If chars(i + width, j) Is Nothing Then chars(i + width, j) = New ConsoleCharacter(-1, -1)

                    If j >= 0 And j < CONSOLE_HEIGHT And i + width >= 0 And i + width < CONSOLE_WIDTH Then
                        chars(i + width, j).BackgroundColour = colour
                    End If

                Next

            Next

            'If the border is larger, why bother
            If width > border Then

                'Bottom border
                For i = x To x + width

                    For j = y - (border - 1) To y

                        'Check that we have an object
                        If chars(i, j + height) Is Nothing Then chars(i, j + height) = New ConsoleCharacter(-1, -1)

                        'Draw the top line of the border
                        If i >= 0 And i < CONSOLE_WIDTH And j + height >= 0 And j + height < CONSOLE_HEIGHT Then
                            chars(i, j + height).BackgroundColour = colour
                        End If

                    Next

                Next

            End If

        End If

        Me.Refresh()

    End Sub

    Private Sub DrawSquare(chars(,) As ConsoleCharacter, x As Integer, y As Integer, width As Integer, height As Integer, fillColour As ConsoleColor, Optional drawBorder As Boolean = True, Optional borderSize As Integer = 0, Optional borderColour As ConsoleColor = ConsoleColor.Black)

        'Draw the square first
        For j = y To y + height

            For i = x To x + width

                If j >= 0 And j < CONSOLE_HEIGHT And
                    i >= 0 And i < CONSOLE_WIDTH Then

                    'Check that we have an object
                    If chars(i, j) Is Nothing Then chars(i, j) = New ConsoleCharacter(-1, -1)

                    'Colour in the object
                    chars(i, j).BackgroundColour = fillColour

                End If

            Next

        Next

        'Draw the border if they want one
        If drawBorder Then
            Me.DrawBorder(chars, x, y, width, height, borderColour, borderSize)
        End If

        Me.Refresh()

    End Sub

    Private Property Content(p As ConsolePosition) As ConsoleCharacter
        Get
            If Not p Is Nothing Then
                If p.X >= 0 And p.X < CONSOLE_WIDTH And p.Y >= 0 And p.Y < CONSOLE_HEIGHT Then
                    Return _contents(p.X, p.Y)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Get
        Set(value As ConsoleCharacter)
            If Not p Is Nothing Then
                If p.X >= 0 And p.X < CONSOLE_WIDTH And p.Y >= 0 And p.Y < CONSOLE_HEIGHT Then
                    _contents(p.X, p.Y) = value
                End If
            End If
        End Set
    End Property

    Private Property Content(x As Integer, y As Integer) As ConsoleCharacter
        Get
            Return Content(New ConsolePosition(x, y))
        End Get
        Set(value As ConsoleCharacter)
            Content(New ConsolePosition(x, y)) = value
        End Set
    End Property

    Private Property Overlay(p As ConsolePosition) As ConsoleCharacter
        Get
            If Not p Is Nothing Then
                If p.X >= 0 And p.X < CONSOLE_WIDTH And p.Y >= 0 And p.Y < CONSOLE_HEIGHT Then
                    Return _overlay(p.X, p.Y)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Get
        Set(value As ConsoleCharacter)
            If Not p Is Nothing Then
                If p.X >= 0 And p.X < CONSOLE_WIDTH And p.Y >= 0 And p.Y < CONSOLE_HEIGHT Then
                    _overlay(p.X, p.Y) = value
                End If
            End If
        End Set
    End Property

    Private Property Overlay(x As Integer, y As Integer) As ConsoleCharacter
        Get
            Return Overlay(New ConsolePosition(x, y))
        End Get
        Set(value As ConsoleCharacter)
            Overlay(New ConsolePosition(x, y)) = value
        End Set
    End Property
    Public Property CursorColour As ConsoleColor
        Set(value As ConsoleColor)
            If value >= 0 And value <= ConsoleColor.White Then
                _cursorColour = value
            End If
        End Set
        Get
            Return _cursorColour
        End Get
    End Property

    Public Property BackgroundColour As ConsoleColor
        Set(value As ConsoleColor)
            If value >= 0 And value <= ConsoleColor.White Then
                _backgroundColour = value
            End If
        End Set
        Get
            Return _backgroundColour
        End Get
    End Property

    Public Property ForegroundColour As ConsoleColor
        Set(value As ConsoleColor)
            If value >= 0 And value <= ConsoleColor.White Then
                _foregroundColour = value
            End If
        End Set
        Get
            Return _foregroundColour
        End Get
    End Property

    Public Property PauseRefresh() As Boolean
        Set(value As Boolean)
            _pauseRefresh = value
        End Set
        Get
            Return _pauseRefresh
        End Get
    End Property

End Class
