Public Class BattleshipConsole

    Private Class ConsoleTimer

        Public Timer As Timers.Timer
        Public Index As Integer
        Private _delegate As EventHandler

        Event TimerTick(index As Integer)

        Sub New(interval As Double)

            'Create the new timer with the correct details
            Timer = New Timers.Timer(interval)
            AddHandler Timer.Elapsed, AddressOf Tick
            Timer.Enabled = True

        End Sub

        Private Sub Tick()

            'Raise the event and return the index for them
            RaiseEvent TimerTick(Index)

        End Sub

    End Class

    Private Class ConsoleOverlayManager

        'Here is a list of our overlays
        'TODO: Fix terrible memory usage
        Private _overlays As List(Of ConsoleCharacter(,))
        Private _timers As List(Of ConsoleTimer)

        Private _pause As Boolean = False

        Sub New()

            'Create our objects
            _overlays = New List(Of ConsoleCharacter(,))
            _timers = New List(Of ConsoleTimer)

        End Sub

        Private Sub TimerTick(index As Integer)

            'Disable this timer first
            _timers(index).Timer.Enabled = False

            While _pause
                'Waste some time here, it shouldn't ever cause a lock up
                'TODO: Perhaps look at WaitOne functionality
                Threading.Thread.Sleep(10)
            End While

            'Stop certain operations
            _pause = True

            'First reduce the index size
            For i As Integer = index + 1 To _timers.Count - 1
                _timers(i).Index -= 1
            Next

            'Remove the data
            _overlays.RemoveAt(index)
            _timers.RemoveAt(index)

            'Restart those certain operations
            _pause = False

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

        Private Sub CreateTimer(interval As Double)

            'Create the timer object
            Dim timer As New ConsoleTimer(interval)
            AddHandler timer.TimerTick, AddressOf TimerTick
            _timers.Add(timer)

        End Sub

        Public Sub AddText(x As Integer, y As Integer, interval As Double, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

            'Add into our array
            Dim overlay(CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1) As ConsoleCharacter
            Dim lines() As String = CutStrings(text, "\n")

            For j = y To y + lines.Length - 1

                For i = x To x + lines(j).Length - 1

                    'Create a new object
                    overlay(i, j) = New ConsoleCharacter(background, foreground)
                    overlay(i, j).Character = text(i - x)

                Next

            Next

            'Add in the new overlay
            _overlays.Add(overlay)

            'New timer
            CreateTimer(interval)

        End Sub

        Public Sub AddOverlay(x As Integer, y As Integer, interval As Double, width As Integer, height As Integer, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

            'Add into our array


        End Sub

        Public ReadOnly Property Overlay(index As Integer) As ConsoleCharacter(,)
            Get
                Return _overlays(index)
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return _overlays.Count
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

    Private Class ConsoleBuffer

        Private _content As ConsoleCharacter(,)

        Sub New(width As Integer, height As Integer, backgroundColour As ConsoleColor, foregroundColour As ConsoleColor)

            'Create the new objects
            For i = 0 To CONSOLE_WIDTH - 1
                For j = 0 To CONSOLE_HEIGHT - 1
                    _content(i, j) = New ConsoleCharacter(backgroundColour, foregroundColour)
                Next
            Next

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

            Dim lines() As String = CutStrings(text, "\n")

            'Add the text in
            For j = y To y + lines.Length - 1

                If j >= 0 And j < CONSOLE_HEIGHT Then

                    For i = x To x + lines(j - y).Length - 1

                        'Ensure we don't go over the buffer
                        If i < CONSOLE_WIDTH Then

                            'Check that we have an object
                            If Content(i, j) Is Nothing Then Content(i, j) = New ConsoleCharacter(-1, -1)

                            'Copy the data into the cell
                            If foreground > -1 Then
                                Content(i, j).ForegroundColour = foreground
                            End If
                            If background > -1 Then
                                Content(i, j).BackgroundColour = background
                            End If
                            Content(i, j).Character = lines(j - y)(i - x)

                        End If

                    Next

                End If

            Next

        End Sub


        Public Sub DrawBorder(x As Integer, y As Integer, width As Integer, height As Integer, colour As ConsoleColor, Optional border As Integer = 1)

            'Check we have good values
            If width > 0 And height > 0 Then

                'Top border
                For i = x To x + width

                    For j = y To y + border - 1

                        'Draw the top line of the border
                        If i >= 0 And i < CONSOLE_WIDTH And j >= 0 And j < CONSOLE_HEIGHT Then
                            Content(i, j).BackgroundColour = colour
                        End If

                    Next

                Next

                'Walls
                For j = y + 1 To y + height - 1

                    'Left wall
                    For i = x To x + border - 1

                        If j >= 0 And j < CONSOLE_HEIGHT And i >= 0 And i < CONSOLE_WIDTH Then
                            Content(i, j).BackgroundColour = colour
                        End If

                    Next

                    'Right wall
                    For i = x - (border - 1) To x

                        If j >= 0 And j < CONSOLE_HEIGHT And i + width >= 0 And i + width < CONSOLE_WIDTH Then
                            Content(i + width, j).BackgroundColour = colour
                        End If

                    Next

                Next

                'If the border is larger, why bother
                If width > border Then

                    'Bottom border
                    For i = x To x + width

                        For j = y - (border - 1) To y

                            'Draw the top line of the border
                            If i >= 0 And i < CONSOLE_WIDTH And j + height >= 0 And j + height < CONSOLE_HEIGHT Then
                                Content(i, j + height).BackgroundColour = colour
                            End If

                        Next

                    Next

                End If

            End If

        End Sub


        Public Sub DrawSquare(x As Integer, y As Integer, width As Integer, height As Integer, fillColour As ConsoleColor, Optional drawBorder As Boolean = True, Optional borderSize As Integer = 0, Optional borderColour As ConsoleColor = ConsoleColor.Black)

            'Draw the square first
            For j = y To y + height

                For i = x To x + width

                    If j >= 0 And j < CONSOLE_HEIGHT And
                        i >= 0 And i < CONSOLE_WIDTH Then

                        'Colour in the object
                        Content(i, j).BackgroundColour = fillColour

                    End If

                Next

            Next

            'Draw the border if they want one
            If drawBorder Then
                Me.DrawBorder(x, y, width, height, borderColour, borderSize)
            End If

        End Sub


        Public Sub Draw()

            'Full draw
            For i = 0 To _content.GetUpperBound(0) - 1
                For j = 0 To _content.GetUpperBound(1) - 1

                    'For some reason I can't draw the last character
                    If i = _content.GetUpperBound(0) - 1 And j = _content.GetUpperBound(1) - 1 Then Exit For

                    'Ensure we should still draw the character cell
                    Content(i, j).Draw(i, j)

                Next
            Next

        End Sub


        Default Public Property Content(i As Integer, j As Integer) As ConsoleCharacter
            Get
                Return Content(New ConsolePosition(i, j))
            End Get
            Set(value As ConsoleCharacter)
                Content(New ConsolePosition(i, j)) = value
            End Set
        End Property


        Default Public Property Content(pos As ConsolePosition) As ConsoleCharacter
            Get
                If pos.X >= _content.GetLowerBound(0) And pos.X <= _content.GetUpperBound(0) And
                    pos.Y >= _content.GetLowerBound(1) And pos.Y <= _content.GetUpperBound(1) Then
                    Return _content(pos.X, pos.Y)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As ConsoleCharacter)
                If pos.X >= _content.GetLowerBound(0) And pos.X <= _content.GetUpperBound(0) And
                    pos.Y >= _content.GetLowerBound(1) And pos.Y <= _content.GetUpperBound(1) And
                    Not value Is Nothing Then
                    _content(pos.X, pos.Y) = value
                End If
            End Set
        End Property

    End Class

    Private Class ConsolePosition

        Public X As Integer = 0
        Public Y As Integer = 0

        Sub New(Optional x As Integer = 0, Optional y As Integer = 0)
            Me.X = x
            Me.Y = y
        End Sub

    End Class

    Private Class ConsoleRect

        Public X As Integer = 0
        Public Y As Integer = 0
        Public Width As Integer = 0
        Public Height As Integer = 0

        Sub New(Optional x As Integer = 0, Optional y As Integer = 0, Optional width As Integer = 0, Optional height As Integer = 0)
            Me.X = x
            Me.Y = y
            Me.Width = width
            Me.Height = height
        End Sub

    End Class


    Private Const CONSOLE_WIDTH As Integer = 80
    Private Const CONSOLE_HEIGHT As Integer = 26

    'Contents of the console display
    Private _primaryBuffer As ConsoleBuffer

    Private _overlayObjects As ConsoleOverlayManager

    Private _backgroundColour As ConsoleColor
    Private _foregroundColour As ConsoleColor
    Private _cursorColour As ConsoleColor

    Private _cursor As ConsolePosition
    Private _cursorBounds As ConsoleRect
    Private _pauseRefresh As Boolean = False

    Public Event OnEnterPressed()

    Sub New(Optional background As ConsoleColor = ConsoleColor.Black, Optional foreground As ConsoleColor = ConsoleColor.Gray, Optional cursor As ConsoleColor = ConsoleColor.Red)

        'Set the colours
        _backgroundColour = background
        _foregroundColour = foreground
        _cursorColour = cursor

        'Create the new objects
        _primaryBuffer = New ConsoleBuffer(CONSOLE_WIDTH, CONSOLE_HEIGHT, background, foreground)

        _cursor = New ConsolePosition()
        _cursorBounds = New ConsoleRect(0, 0, CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1)
        _overlayObjects = New ConsoleOverlayManager

        'Other default settings
        Console.SetBufferSize(CONSOLE_WIDTH, CONSOLE_HEIGHT)
        Console.CursorVisible = False

        'Refresh the screen and set the cursor
        Me.Refresh()

    End Sub

    Public Sub Refresh()

        'do they want to stifle the refreshing
        If Not _pauseRefresh Then

            'Draw the primary buffer
            _primaryBuffer.Draw()

            'TODO: Increase efficiency by checking if top overlay has character before drawing again
            'TODO: Perhaps an array of boolean and walk through overlay's backwards
            Dim skip As Boolean = False

            'Go through our overlays to see if we have anything
            For overlayIndex = 0 To _overlayObjects.Count - 1
                'Check that the character is actually an object, and set the char update as well
                'If Not _overlayObjects.Overlay(overlayIndex)(i, j) Is Nothing Then
                '    _overlayObjects.Overlay(overlayIndex)(i, j).Draw(i, j)
                '    _primaryBuffer(i, j).Update = True
                '    skip = True
                'End If
            Next

            'Draw the cursor
            Console.SetCursorPosition(_cursor.X, _cursor.Y)
            Console.BackgroundColor = CursorColour
            Console.ForegroundColor = _primaryBuffer(_cursor).ForegroundColour
            Console.Write(_primaryBuffer(_cursor).Character)
            Console.SetCursorPosition(_cursor.X, _cursor.Y)

        End If

    End Sub

    Public Sub WriteOverlay()

        _overlayObjects.AddText(0, 0, 500, "text")

        Me.Refresh()

    End Sub

    Public Sub CursorMovement()

        'Wait for the keypress
        Dim key As ConsoleKeyInfo = Console.ReadKey

        'Ensure it's an arrow key
        If key.Key >= 37 And key.Key <= 40 Then

            'Force the update
            _primaryBuffer(_cursor).Update = True

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
