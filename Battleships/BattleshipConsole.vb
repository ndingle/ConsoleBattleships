Public Class BattleshipConsole


    Private Class ConsoleCharacter

        Private _background As ConsoleColor
        Private _foreground As ConsoleColor
        Private _character As Char


        Sub New(Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1, Optional character As Char = "")

            'Set the defaults for this character
            Me.Background = background
            Me.Foreground = foreground
            Me.Character = character

        End Sub


        Sub New(c As ConsoleCharacter)

            'Copy the data from the new character
            Me.Background = c.Background
            Me.Foreground = c.Foreground
            Me.Character = c.Character

        End Sub


        Public Shared Operator =(A As ConsoleCharacter, B As ConsoleCharacter) As Boolean

            If A.Background = B.Background And
                A.Foreground = B.Foreground And
                A.Character = B.Character Then
                Return True
            Else
                Return False
            End If

        End Operator


        Public Shared Operator <>(A As ConsoleCharacter, B As ConsoleCharacter) As Boolean

            Return Not (A = B)

        End Operator


        Public Property Background As ConsoleColor
            Set(value As ConsoleColor)
                '-1 is used for a transparent character (useful for overlays)
                If value >= -1 And value <= 15 Then
                    _background = value
                End If
            End Set
            Get
                Return _background
            End Get
        End Property


        Public Property Foreground As ConsoleColor
            Set(value As ConsoleColor)
                '-1 is used for a unchanged character colour (useful for overlays)
                If value >= -1 And value <= 15 Then
                    _foreground = value
                End If
            End Set
            Get
                Return _foreground
            End Get
        End Property


        Public Property Character As Char
            Set(value As Char)
                'Null characters are allowed, as they might be a blank spot)
                _character = value
            End Set
            Get
                Return _character
            End Get
        End Property


    End Class


    Private Class ConsolePosition

        Private _x As Integer
        Private _y As Integer


        Sub New(Optional x As Integer = 0, Optional y As Integer = 0)

            'Setup the default values
            Me.X = x
            Me.Y = y

        End Sub


        Public Property X As Integer
            Set(value As Integer)
                If value >= 0 Then
                    _x = value
                End If
            End Set
            Get
                Return _x
            End Get
        End Property


        Public Property Y As Integer
            Set(value As Integer)
                If value >= 0 Then
                    _y = value
                End If
            End Set
            Get
                Return _y
            End Get
        End Property


    End Class


    Private Class ConsoleBuffer


        Private _content(CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1) As ConsoleCharacter
        Private _isOverlay As Boolean


        Sub New(Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1, Optional overlay As Boolean = False)

            'If the user wants to create a fresh buffer, otherwise leave it blank for better memory usage
            If Not overlay Then

                'Create new console characters
                For i As Integer = 0 To CONSOLE_WIDTH - 1
                    For j As Integer = 0 To CONSOLE_HEIGHT - 1
                        _content(i, j) = New ConsoleCharacter(background, foreground)
                    Next
                Next
            End If

            'Ensure we remember this
            _isOverlay = overlay

        End Sub


        Public Sub CopyBuffer(buffer As ConsoleBuffer)

            'Ensure we have a buffer object
            If Not buffer Is Nothing Then

                'OK Start the copy!
                For i = 0 To CONSOLE_WIDTH - 1
                    For j = 0 To CONSOLE_HEIGHT - 1

                        Dim overwrite As Boolean = False

                        'Check if we should replace the character
                        If Content(i, j) <> buffer.Content(i, j) Then
                            overwrite = True
                        End If

                        'Perform the overwrite
                        If overwrite Then Content(i, j) = New ConsoleCharacter(buffer.Content(i, j))

                    Next
                Next

            End If

        End Sub


        Private Function SplitString(text As String, Optional delimiter As String = "\n") As String()

            'Cut the strings up and return them
            If Not text Is Nothing Then

                'If we have a background and foreground then do something
                Dim lines() As String = text.Split(delimiter)

                'Remove the extra characters
                For i = 1 To lines.Length - 1
                    lines(i) = lines(i).Remove(0, delimiter.Length - 1)
                Next

                'Return our result
                Return lines

            Else

                Return Nothing

            End If

        End Function


        Public Sub Write(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

            'Split the strings first
            Dim lines() As String = SplitString(text, "\n")

            For j = y To y + lines.Length - 1

                For i = x To x + lines(j - y).Length - 1

                    If InBounds(i, j) Then

                        'Create a new object
                        If Not _isOverlay Then
                            If background = -1 Then
                                background = Content(i, j).Background
                            End If
                            If foreground Then
                                foreground = Content(i, j).Foreground
                            End If
                            Content(i, j).Background = background
                            Content(i, j).Foreground = foreground
                            Content(i, j).Character = lines(j)(i - x)
                        Else
                            'Check if the object exists first
                            If Content(i, j) Is Nothing Then
                                Content(i, j) = New ConsoleCharacter(background, foreground, lines(j - y)(i - x))
                            Else
                                Content(i, j).Background = background
                                Content(i, j).Foreground = foreground
                                Content(i, j).Character = lines(j - y)(i - x)
                            End If
                        End If

                    End If

                Next

            Next

        End Sub


        Public Sub DrawBorder(x As Integer, y As Integer,
                              width As Integer, height As Integer,
                              borderSize As Integer,
                              borderColour As ConsoleColor)

            '
            'Check we have good values
            If width > 0 And height > 0 Then

                'Top border
                For i = x To x + width - 1

                    For j = y To y + borderSize - 1

                        'Draw the top line of the border
                        If InBounds(i, j) Then
                            Content(i, j).Background = borderColour
                            If _isOverlay Then Content(i, j).Character = ""
                        End If

                    Next

                Next

                'Ensure the height is bigger than 1
                If height > 1 Then

                    'Walls
                    For j = y + 1 To y + height - 1

                        'Left wall
                        For i = x To x + borderSize - 1

                            If InBounds(i, j) Then
                                Content(i, j).Background = borderColour
                                If _isOverlay Then Content(i, j).Character = ""
                            End If

                        Next

                        'Right wall
                        For i = x - (borderSize - 1) To x

                            If InBounds(i + width - 1, j) Then
                                Content(i + width - 1, j).Background = borderColour
                                If _isOverlay Then Content(i, j).Character = ""
                            End If

                        Next

                    Next

                    'If the border is larger, why bother
                    If width > borderSize Then

                        'Bottom border
                        For i = x To x + width - 1

                            For j = y - (borderSize - 1) To y

                                'Draw the top line of the border
                                If InBounds(i, j + height - 1) Then
                                    Content(i, j + height - 1).Background = borderColour
                                    If _isOverlay Then Content(i, j).Character = ""
                                End If

                            Next

                        Next

                    End If

                End If

            End If

        End Sub


        Public Sub DrawSquare(x As Integer, y As Integer,
                              width As Integer, height As Integer,
                              background As ConsoleColor,
                              Optional foreground As ConsoleColor = -1,
                              Optional drawBorderAround As Boolean = False,
                              Optional borderSize As Integer = 0,
                              Optional borderColour As ConsoleColor = -1)

            'Draw the square first
            For j = y To y + height - 1

                For i = x To x + width - 1

                    If InBounds(i, j) Then

                        If Content(i, j) Is Nothing Then Content(i, j) = New ConsoleCharacter()

                        'Colour in the object
                        If background <> -1 Then
                            Content(i, j).Background = background
                        End If
                        If foreground <> -1 Then
                            Content(i, j).Foreground = foreground
                        End If

                        'If this is an overlay then null the character out (they can add text afterward)
                        If _isOverlay Then
                            Content(i, j).Character = ""
                        End If

                    End If

                Next

            Next

            'Draw the border if they want one
            If drawBorderAround Then
                Me.DrawBorder(x, y, width, height, borderSize, borderColour)
            End If

        End Sub


        Public Function InBounds(x As Integer, y As Integer) As Boolean

            If x >= 0 And x <= _content.GetUpperBound(0) And
                y >= 0 And y <= _content.GetUpperBound(1) Then
                Return True
            Else
                Return False
            End If

        End Function


        Default Public Property Content(pos As ConsolePosition) As ConsoleCharacter
            Get
                Return Content(pos.X, pos.Y)
            End Get
            Set(value As ConsoleCharacter)
                Content(pos.X, pos.Y) = value
            End Set
        End Property


        Default Public Property Content(i As Integer, j As Integer) As ConsoleCharacter
            Get
                'Ensure the coords in the bounds
                If InBounds(i, j) Then
                    Return _content(i, j)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As ConsoleCharacter)
                If InBounds(i, j) Then
                    _content(i, j) = value
                End If
            End Set
        End Property


    End Class


    Private Class ConsoleOverlayManager

        Private _overlays As List(Of ConsoleBuffer)
        Private _newOverlay As ConsoleBuffer

        Sub New()

            'Setup the object
            _overlays = New List(Of ConsoleBuffer)

        End Sub


        Public Sub StartOverlay()

            'Create a blank overlay
            _newOverlay = New ConsoleBuffer(, , True)

        End Sub


        Public Sub FinishOverlay()

            'OK, now add it on the list
            _overlays.Add(_newOverlay)
            _newOverlay = Nothing

        End Sub


        Public Sub Write(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

            'Write to the main buffer
            If Not _newOverlay Is Nothing Then
                _newOverlay.Write(x, y, text, background, foreground)
            End If

        End Sub


        Public Sub DrawSquare(x As Integer, y As Integer,
                              width As Integer, height As Integer,
                              background As ConsoleColor,
                              Optional foreground As ConsoleColor = -1,
                              Optional drawBorderAround As Boolean = False,
                              Optional borderSize As Integer = 0,
                              Optional borderColour As ConsoleColor = -1)

            'Draw the square to the overlay
            If Not _newOverlay Is Nothing Then
                _newOverlay.DrawSquare(x, y, width, height, background, foreground, drawBorderAround, borderSize, borderColour)
            End If

        End Sub


        Public Sub DrawBorder(x As Integer, y As Integer,
                             width As Integer, height As Integer,
                             borderSize As Integer,
                             borderColour As ConsoleColor)

            'Draw the border
            If Not _newOverlay Is Nothing Then
                _newOverlay.DrawBorder(x, y, width, height, borderSize, borderColour)
            End If

        End Sub


        Public Function GetOverlayBuffer() As ConsoleBuffer

            Dim result As New ConsoleBuffer(, , True)

            'Go backwards as the top is most important
            For overlay = _overlays.Count - 1 To 0 Step -1

                For i = 0 To CONSOLE_WIDTH - 1
                    For j = 0 To CONSOLE_HEIGHT - 1

                        'See if the cell is something
                        If Not _overlays(overlay)(i, j) Is Nothing Then

                            'Add it to our new return buffer
                            If result(i, j) Is Nothing Then
                                result(i, j) = New ConsoleCharacter(_overlays(overlay)(i, j))
                            Else
                                'Copy if any cells are empty
                                If result(i, j).Background = -1 Then
                                    result(i, j).Background = _overlays(overlay)(i, j).Background
                                End If
                                If result(i, j).Foreground = -1 Then
                                    result(i, j).Foreground = _overlays(overlay)(i, j).Foreground
                                End If
                                If result(i, j).Character = "" Then
                                    result(i, j).Character = _overlays(overlay)(i, j).Character
                                End If
                            End If

                        End If

                    Next
                Next

            Next

            'The overlay is finished, return it
            Return result

        End Function


        Public Sub RemoveOverlay(index As Integer)

            'Remove the bloody overlay ay
            If index >= 0 And index < _overlays.Count Then
                _overlays.RemoveAt(index)
            End If

        End Sub


    End Class


    'Start of BattleshipConsole
    Public Const CONSOLE_WIDTH As Integer = 80
    Public Const CONSOLE_HEIGHT As Integer = 26

    Private _mainBuffer As ConsoleBuffer
    Private _buffers(1) As ConsoleBuffer
    Private _currentBuffer As Integer = 0

    Private _overlayManager As ConsoleOverlayManager
    Private _cursorPosition As ConsolePosition
    Private _cursorMinimum As ConsolePosition
    Private _cursorMaximum As ConsolePosition

    Private _backgroundColour As ConsoleColor
    Private _foregroundColour As ConsoleColor
    Private _cursorColour As ConsoleColor


    Sub New(background As ConsoleColor, foreground As ConsoleColor, cursor As ConsoleColor)

        'Setup the variable details
        _backgroundColour = background
        _foregroundColour = foreground
        _cursorColour = cursor

        'Create new buffers
        _mainBuffer = New ConsoleBuffer(_backgroundColour, _foregroundColour)
        _buffers(0) = New ConsoleBuffer(_backgroundColour, _foregroundColour)
        _buffers(1) = New ConsoleBuffer(_backgroundColour, _foregroundColour)

        'Setup other objects
        _overlayManager = New ConsoleOverlayManager()
        _cursorPosition = New ConsolePosition()
        _cursorMinimum = New ConsolePosition(0, 0)
        _cursorMaximum = New ConsolePosition(CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1)

        'Setup the console object
        Console.CursorVisible = False
        Console.SetBufferSize(CONSOLE_WIDTH, CONSOLE_HEIGHT)
        Console.BackgroundColor = _backgroundColour
        Console.ForegroundColor = _foregroundColour
        Console.Clear()

    End Sub


    Public Sub Write(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

        'Write to the main buffer
        _mainBuffer.Write(x, y, text, background, foreground)
        Me.Refresh()

    End Sub


    Public Sub RemoveOverlay()

        _overlayManager.RemoveOverlay(0)
        Me.Refresh()

    End Sub


    Public Sub StartOverlay()

        'Start the new overlay
        _overlayManager.StartOverlay()

    End Sub


    Public Sub FinishOverlay()

        'Finalise the overlay
        _overlayManager.FinishOverlay()
        Me.Refresh()

    End Sub


    Public Sub WriteOverlay(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

        'Push a new overlay on
        _overlayManager.Write(x, y, text, background, foreground)

    End Sub


    Public Sub DrawSquareOverlay(x As Integer, y As Integer,
                              width As Integer, height As Integer,
                              background As ConsoleColor,
                              Optional foreground As ConsoleColor = -1,
                              Optional drawBorderAround As Boolean = False,
                              Optional borderSize As Integer = 0,
                              Optional borderColour As ConsoleColor = -1)

        'Draw the square
        _overlayManager.DrawSquare(x, y, width, height, background, foreground, drawBorderAround, borderSize, borderColour)

    End Sub


    Public Sub DrawSquare(x As Integer, y As Integer,
                              width As Integer, height As Integer,
                              background As ConsoleColor,
                              Optional foreground As ConsoleColor = -1,
                              Optional drawBorderAround As Boolean = False,
                              Optional borderSize As Integer = 0,
                              Optional borderColour As ConsoleColor = -1)

        'Just draw the square!!!!
        _mainBuffer.DrawSquare(x, y, width, height, background, foreground, drawBorderAround, borderSize, borderColour)

        'Ensure we don't do two refresh calls
        If Not drawBorderAround Then Me.Refresh()

    End Sub


    Public Sub DrawBorder(x As Integer, y As Integer,
                             width As Integer, height As Integer,
                             borderSize As Integer,
                             borderColour As ConsoleColor)

        'Draw the border 
        _mainBuffer.DrawBorder(x, y, width, height, borderSize, borderColour)
        Me.Refresh()

    End Sub


    Public Sub MoveCursorUntilKeyPress(acceptedKeys() As ConsoleKey)

        'Use the move cursor position until one of the approved keys is pressed
        Dim moving As Boolean = True

        If Not acceptedKeys Is Nothing Then

            If acceptedKeys.Count > 0 Then

                While moving

                    'Move one space
                    Dim key As ConsoleKey = MoveCursor()

                    'Check if they pressed a good key
                    For Each accepted As ConsoleKey In acceptedKeys
                        If key = accepted Then moving = False
                    Next

                End While

            End If

        End If

    End Sub


    Public Function MoveCursor() As ConsoleKey

        'Allow the user to move the cursor one space
        Dim key As ConsoleKey = Console.ReadKey.Key

        'Do we move the cursor?
        Select Case key
            Case ConsoleKey.UpArrow
                If _cursorPosition.Y > _cursorMinimum.Y Then _cursorPosition.Y -= 1
            Case ConsoleKey.DownArrow
                If _cursorPosition.Y < _cursorMaximum.Y Then _cursorPosition.Y += 1
            Case ConsoleKey.LeftArrow
                If _cursorPosition.X > _cursorMinimum.X Then _cursorPosition.X -= 1
            Case ConsoleKey.RightArrow
                If _cursorPosition.X < _cursorMaximum.X Then _cursorPosition.X += 1
        End Select

        Me.Refresh()

        Return key

    End Function


    Public Sub SetCursorMinimum(x As Integer, y As Integer)

        'Set the mimium coords of the cursor
        If x >= 0 And x < CONSOLE_WIDTH Then
            _cursorMinimum.X = x
            If _cursorPosition.X < x Then _cursorPosition.X = x
        End If

        If y >= 0 And y < CONSOLE_HEIGHT Then
            _cursorMinimum.Y = y
            If _cursorPosition.Y < y Then _cursorPosition.Y = y
        End If

    End Sub


    Public Sub SetCursorMaximum(x As Integer, y As Integer)

        'Set the mimium coords of the cursor
        If x >= 0 And x < CONSOLE_WIDTH Then
            _cursorMaximum.X = x
            If _cursorPosition.X > x Then _cursorPosition.X = x
        End If

        If y >= 0 And y < CONSOLE_HEIGHT Then
            _cursorMaximum.Y = y
            If _cursorPosition.Y > y Then _cursorPosition.Y = y
        End If

    End Sub


    Public Sub Refresh()

        'Bring all the buffers together into the current buffer
        ConsolidateBuffers()

        'Refresh the screen using the current buffer
        For i = 0 To CONSOLE_WIDTH - 1
            For j = 0 To CONSOLE_HEIGHT - 1

                If _buffers(_currentBuffer)(i, j) <> _buffers(1 - _currentBuffer)(i, j) Then

                    Console.SetCursorPosition(i, j)

                    'Draw the character
                    Console.ForegroundColor = _buffers(_currentBuffer)(i, j).Foreground
                    Console.BackgroundColor = _buffers(_currentBuffer)(i, j).Background
                    Console.Write(_buffers(_currentBuffer)(i, j).Character)

                End If

            Next
        Next

        'Set the new position and move on, ay
        Console.SetCursorPosition(_cursorPosition.X, _cursorPosition.Y)

        'Switch the buffers
        _currentBuffer = 1 - _currentBuffer

    End Sub


    Private Sub ConsolidateBuffers()

        'Add the mainbuffer into the current buffer
        _buffers(_currentBuffer).CopyBuffer(_mainBuffer)

        'Grab the overlay object and integrate that
        Dim overlayBuffer As ConsoleBuffer = _overlayManager.GetOverlayBuffer()

        'Now time to bring them together
        For i = 0 To CONSOLE_WIDTH - 1
            For j = 0 To CONSOLE_HEIGHT - 1

                'Ensure there is an overlay
                If Not overlayBuffer(i, j) Is Nothing Then
                    'Only take what has a value
                    If overlayBuffer(i, j).Background <> -1 Then
                        _buffers(_currentBuffer)(i, j).Background = overlayBuffer(i, j).Background
                    End If
                    If overlayBuffer(i, j).Foreground <> -1 Then
                        _buffers(_currentBuffer)(i, j).Foreground = overlayBuffer(i, j).Foreground
                    End If
                    If overlayBuffer(i, j).Character <> "" Then
                        _buffers(_currentBuffer)(i, j).Character = overlayBuffer(i, j).Character
                    End If
                End If

            Next
        Next

        'Set the cursor position and negate the other buffer so it always gets drawn
        _buffers(_currentBuffer)(_cursorPosition.X, _cursorPosition.Y).Background = _cursorColour
        _buffers(1 - _currentBuffer)(_cursorPosition.X, _cursorPosition.Y).Background = -1

    End Sub


End Class
