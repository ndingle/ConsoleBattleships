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


    Public Class ConsolePosition

        Private _x As Integer
        Private _y As Integer


        Sub New(Optional ByVal x As Integer = 0, Optional ByVal y As Integer = 0)

            'Setup the default values
            Me.X = x
            Me.Y = y

        End Sub


        Sub New(pos As ConsolePosition)

            Me.X = pos.X
            Me.Y = pos.Y

        End Sub


        Public Shared Operator =(A As ConsolePosition, B As ConsolePosition) As Boolean

            If A.X = B.X And
                A.Y = B.Y Then
                Return True
            Else
                Return False
            End If

        End Operator


        Public Shared Operator <>(A As ConsolePosition, B As ConsolePosition) As Boolean

            Return Not (A = B)

        End Operator


        Public Property X As Integer
            Set(ByVal value As Integer)
                If value >= 0 Then
                    _x = value
                End If
            End Set
            Get
                Return _x
            End Get
        End Property


        Public Property Y As Integer
            Set(ByVal value As Integer)
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
        Private _defaultBackgroundColour As ConsoleColor
        Private _defaultForegroundColour As ConsoleColor


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
                            Content(i, j).Character = lines(j - y)(i - x)
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


        Public Sub EraseContent(x As Integer, y As Integer,
                                width As Integer, height As Integer)

            'We remove all of the content that is in that section (don't allow regular buffers to be null)
            If width > 0 And height > 0 Then

                For i = x To x + (width - 1)

                    For j = y To y + (height - 1)

                        If _isOverlay Then
                            Content(i, j) = Nothing
                        Else
                            Content(i, j).Background = _defaultbackgroundColour
                            Content(i, j).Foreground = _defaultforegroundColour
                            Content(i, j).Character = " "

                        End If

                    Next

                Next

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
        Private _overlayNames As List(Of String)
        Private _newOverlay As ConsoleBuffer

        Private _cursorOverlay As ConsoleBuffer
        Private _cursorPosition As ConsolePosition
        Private _cursorMinimum As ConsolePosition
        Private _cursorMaximum As ConsolePosition

        Public Const CURSOR_NAME As String = "##CURSOR"

        Sub New()

            'Setup the object
            _overlays = New List(Of ConsoleBuffer)
            _overlayNames = New List(Of String)

            'Add in the cursor object at the position 
            _cursorPosition = New ConsolePosition
            _cursorMinimum = New ConsolePosition(0, 0)
            _cursorMaximum = New ConsolePosition(CONSOLE_WIDTH - 1, CONSOLE_HEIGHT - 1)

            'Create the cursor overlay, manually add it and update it
            _cursorOverlay = New ConsoleBuffer(, , True)
            _overlays.Add(_cursorOverlay)
            _overlayNames.Add(CURSOR_NAME)
            UpdateCursor(New ConsolePosition(0, 0), True)

        End Sub


        Private Sub UpdateCursor(oldCursorPosition As ConsolePosition, Optional force As Boolean = False)

            'Remove the old square and place the new one
            If oldCursorPosition <> _cursorPosition Or force Then
                _cursorOverlay.EraseContent(oldCursorPosition.X, oldCursorPosition.Y, 1, 1)
                _cursorOverlay.DrawSquare(_cursorPosition.X, _cursorPosition.Y, 1, 1, ConsoleColor.Red)
            End If

        End Sub


        Public Sub StartOverlay()

            'Create a blank overlay
            _newOverlay = New ConsoleBuffer(, , True)

        End Sub


        Public Function FinishOverlay(name As String) As Boolean

            'OK, now add it on the list, but underneath the cursor overlay, if they provided a bad name or a duplicate, ignore it
            If name.ToUpper <> CURSOR_NAME And _overlayNames.IndexOf(name.ToUpper) = -1 Then
                _overlays.Insert(_overlays.Count - 1, _newOverlay)
                _overlayNames.Insert(_overlayNames.Count - 1, name.ToUpper)
                _newOverlay = Nothing
                Return True
            Else
                Return False
            End If

        End Function


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
                                If result(i, j).Character = vbNullChar Then
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


        Public Function AddToTopOverlay(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1) As Boolean

            'Add some text to the top overlay only
            If _overlays.Count > 1 Then

                'Add to the overlay on the top
                _overlays(_overlays.Count - 2).Write(x, y, text, background, foreground)
                Return True

            Else
                'No overlays to work with
                Return False
            End If

        End Function


        Public Sub RemoveOverlay(name As String)

            'Find the name
            If name.ToUpper <> CURSOR_NAME Then
                Dim index As Integer = _overlayNames.IndexOf(name.ToUpper)
                Me.RemoveOverlayAt(index)
            End If

        End Sub


        Public Sub RemoveOverlayAt(index As Integer)

            'Remove the bloody overlay ay
            If index >= 0 And index < _overlays.Count - 1 Then
                _overlays.RemoveAt(index)
                _overlayNames.RemoveAt(index)
            End If

        End Sub


        Public Sub RemoveAllOverlays()

            'Rmove all of the overlays and add the cursor back
            _overlays.Clear()
            _overlays.Add(_cursorOverlay)
            _overlayNames.Add(CURSOR_NAME)

        End Sub


        Public Function MoveCursor(key As ConsoleKey) As Boolean

            Dim oldCursorPosition As New ConsolePosition(_cursorPosition)

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

            'Update the cursor if we need it
            If oldCursorPosition <> _cursorPosition Then
                UpdateCursor(oldCursorPosition)
                Return True
            End If

            Return False

        End Function


        Public Function SetCursorMinimum(x As Integer, y As Integer) As Boolean

            Dim oldCursorPosition As New ConsolePosition(_cursorPosition)

            'Set the mimium coords of the cursor
            If x >= 0 And x < CONSOLE_WIDTH Then
                _cursorMinimum.X = x
                If _cursorPosition.X < x Then _cursorPosition.X = x
            End If

            If y >= 0 And y < CONSOLE_HEIGHT Then
                _cursorMinimum.Y = y
                If _cursorPosition.Y < y Then _cursorPosition.Y = y
            End If

            If oldCursorPosition <> _cursorPosition Then
                UpdateCursor(oldCursorPosition)
                Return True
            End If

            Return False

        End Function


        Public Function SetCursorMaximum(x As Integer, y As Integer) As Boolean

            Dim oldCursorPosition As New ConsolePosition(_cursorPosition)

            'Set the mimium coords of the cursor
            If x >= 0 And x < CONSOLE_WIDTH Then
                _cursorMaximum.X = x
                If _cursorPosition.X > x Then _cursorPosition.X = x
            End If

            If y >= 0 And y < CONSOLE_HEIGHT Then
                _cursorMaximum.Y = y
                If _cursorPosition.Y > y Then _cursorPosition.Y = y
            End If

            If oldCursorPosition <> _cursorPosition Then
                UpdateCursor(oldCursorPosition)
                Return True
            End If

            Return False

        End Function


        Public Function SetCursorPosition(x As Integer, y As Integer) As Boolean

            Dim oldCursorPosition As New ConsolePosition(_cursorPosition)

            'Manually set the location of the cursor
            If x >= _cursorMinimum.X And x <= _cursorMaximum.X And
                y >= _cursorMinimum.Y And y <= _cursorMaximum.Y Then
                _cursorPosition.X = x
                _cursorPosition.Y = y
                UpdateCursor(oldCursorPosition)
                Return True
            End If

            Return False

        End Function


        Public Function Count() As Integer
            Return _overlays.Count
        End Function


        Public ReadOnly Property CursorPosition As ConsolePosition
            Get
                Return _cursorPosition
            End Get
        End Property


        Public ReadOnly Property CursorMinimum As ConsolePosition
            Get
                Return _cursorMinimum
            End Get
        End Property


        Public ReadOnly Property CursorMaximum As ConsolePosition
            Get
                Return _cursorMaximum
            End Get
        End Property


    End Class


    'Start of BattleshipConsole
    Public Const CONSOLE_WIDTH As Integer = 80
    Public Const CONSOLE_HEIGHT As Integer = 26

    Private _mainBuffer As ConsoleBuffer
    Private _buffers(1) As ConsoleBuffer
    Private _autoRefresh As Boolean
    Private _drawCursor As Boolean
    Private _drawingOverlay As Boolean
    Private _currentBuffer As Integer = 0

    Private _overlayManager As ConsoleOverlayManager
    Private _lastCursorPosition As ConsolePosition

    Private _backgroundColour As ConsoleColor
    Private _foregroundColour As ConsoleColor
    Private _cursorColour As ConsoleColor


    Sub New(background As ConsoleColor, foreground As ConsoleColor, cursor As ConsoleColor)

        'Setup the variable details
        _backgroundColour = background
        _foregroundColour = foreground
        _cursorColour = cursor
        _autoRefresh = True
        _drawCursor = True
        _drawingOverlay = False

        'Create new buffers
        _mainBuffer = New ConsoleBuffer(_backgroundColour, _foregroundColour)
        _buffers(0) = New ConsoleBuffer(_backgroundColour, _foregroundColour)
        _buffers(1) = New ConsoleBuffer(_backgroundColour, _foregroundColour)

        'Setup other objects
        _overlayManager = New ConsoleOverlayManager()
        _lastCursorPosition = New ConsolePosition(_overlayManager.CursorPosition)

        'Setup the console object
        Console.CursorVisible = False
        Console.SetBufferSize(CONSOLE_WIDTH, CONSOLE_HEIGHT)
        Console.BackgroundColor = _backgroundColour
        Console.ForegroundColor = _foregroundColour
        Console.Clear()

    End Sub


    Public Sub Write(x As Integer, y As Integer, text As String, Optional background As ConsoleColor = -1, Optional foreground As ConsoleColor = -1)

        'Write to the main buffer
        If Not _drawingOverlay Then
            _mainBuffer.Write(x, y, text, background, foreground)
            If AutoRefresh Then Me.Refresh()
        Else
            _overlayManager.Write(x, y, text, background, foreground)
        End If

    End Sub


    Public Sub RemoveLastOverlay()

        _overlayManager.RemoveOverlayAt(_overlayManager.Count - 2)
        If AutoRefresh Then Me.Refresh()

    End Sub


    Public Sub RemoveOverlay(name As String)

        _overlayManager.RemoveOverlay(name)
        If AutoRefresh Then Me.Refresh()

    End Sub


    Public Sub RemoveOverlayAt(index As Integer)

        If index >= 0 And index < _overlayManager.Count - 1 Then
            _overlayManager.RemoveOverlayAt(index)
            If AutoRefresh Then Me.Refresh()
        End If

    End Sub


    Public Sub RemoveAllOverlays()

        _overlayManager.RemoveAllOverlays()

    End Sub


    Public Sub StartOverlay()

        'Start the new overlay
        _drawingOverlay = True
        _overlayManager.StartOverlay()

    End Sub


    Public Function FinishOverlay(name As String) As Boolean

        'Finalise the overlay
        _drawingOverlay = False
        Dim result As Boolean = _overlayManager.FinishOverlay(name)
        'Only refresh if something was added
        If AutoRefresh And result Then Me.Refresh()
        Return result

    End Function


    Public Sub DrawSquare(x As Integer, y As Integer,
                              width As Integer, height As Integer,
                              background As ConsoleColor,
                              Optional foreground As ConsoleColor = -1,
                              Optional drawBorderAround As Boolean = False,
                              Optional borderSize As Integer = 0,
                              Optional borderColour As ConsoleColor = -1)

        'Just draw the square!!!!
        If Not _drawingOverlay Then
            _mainBuffer.DrawSquare(x, y, width, height, background, foreground, drawBorderAround, borderSize, borderColour)
            If AutoRefresh Then Me.Refresh()
        Else
            _overlayManager.DrawSquare(x, y, width, height, background, foreground, drawBorderAround, borderSize, borderColour)
        End If

    End Sub


    Public Sub DrawBorder(x As Integer, y As Integer,
                             width As Integer, height As Integer,
                             borderSize As Integer,
                             borderColour As ConsoleColor)

        'Draw the border 
        If Not _drawingOverlay Then
            _mainBuffer.DrawBorder(x, y, width, height, borderSize, borderColour)
            If AutoRefresh Then Me.Refresh()
        Else
            _overlayManager.DrawBorder(x, y, width, height, borderSize, borderColour)
        End If

    End Sub


    Public Function MoveCursorUntilKeyPress(acceptedKeys() As ConsoleKey) As BattleshipConsole.ConsolePosition

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

        Return _overlayManager.CursorPosition

    End Function


    Public Function MoveCursor(Optional autoKey As ConsoleKey = -1) As ConsoleKey

        'Allow the user to move the cursor one space
        Dim key As ConsoleKey = autoKey
        If autoKey = -1 Then key = Console.ReadKey.Key

        Dim oldCursorPosition As New ConsolePosition(_overlayManager.CursorPosition)

        'Do we move the cursor and refresh
        If _overlayManager.MoveCursor(key) Then
            _lastCursorPosition = oldCursorPosition
        End If

        Me.Refresh()

        Return key

    End Function


    Public Sub SetCursorMinimum(x As Integer, y As Integer)

        Dim oldCursorPosition As New ConsolePosition(_overlayManager.CursorPosition)

        'Set the mimium coords of the cursor
        If _overlayManager.SetCursorMinimum(x, y) And AutoRefresh Then
            _lastCursorPosition = oldCursorPosition
            Me.Refresh()
        End If


    End Sub


    Public Sub SetCursorMaximum(x As Integer, y As Integer)

        Dim oldCursorPosition As New ConsolePosition(_overlayManager.CursorPosition)

        'Set the mimium coords of the cursor
        If _overlayManager.SetCursorMaximum(x, y) And AutoRefresh Then
            _lastCursorPosition = oldCursorPosition
            Me.Refresh()
        End If

    End Sub


    Public Sub SetCursorPosition(x As Integer, y As Integer)

        'Manually set the location of the cursor
        If _overlayManager.SetCursorPosition(x, y) And AutoRefresh Then Me.Refresh()

    End Sub


    Public Function ReadKey(Optional moveCursor As Boolean = False) As ConsoleKeyInfo

        'Read a single key and return it's value
        Dim key As ConsoleKeyInfo = Console.ReadKey(True)

        'Add to the top buffer or just the main one
        If Not _overlayManager.AddToTopOverlay(_overlayManager.CursorPosition.X, _overlayManager.CursorPosition.Y, key.KeyChar) Then
            _mainBuffer.Write(_overlayManager.CursorPosition.X, _overlayManager.CursorPosition.Y, key.KeyChar)
        End If

        'Move the cursor if they want
        If moveCursor And key.Key <> ConsoleKey.Enter Then
            Me.MoveCursor(ConsoleKey.RightArrow)
        End If

        'Refresh the screen
        Me.Refresh()

        Return key

    End Function


    Public Function ReadLine() As String

        'Use the readkey and wait until the enter key
        Dim key As ConsoleKeyInfo
        Dim text As String = ""

        'Loop until we get the enter key
        Do

            'Get a key and move onward
            key = ReadKey(True)
            text &= key.KeyChar

        Loop Until key.Key = ConsoleKey.Enter

        Return text

    End Function


    Public Sub Refresh()

        'TODO: Fix issue with cursor being the same colour as another overlay

        'Bring all the buffers together into the current buffer
        ConsolidateBuffers()

        'Refresh the screen using the current buffer
        For i = 0 To CONSOLE_WIDTH - 1
            For j = 0 To CONSOLE_HEIGHT - 1

                If _buffers(_currentBuffer)(i, j) <> _buffers(1 - _currentBuffer)(i, j) Or
                    New ConsolePosition(i, j) = _overlayManager.CursorPosition Or
                    New ConsolePosition(i, j) = _lastCursorPosition Then

                    Console.SetCursorPosition(i, j)

                    'Draw the character
                    Console.ForegroundColor = _buffers(_currentBuffer)(i, j).Foreground
                    Console.BackgroundColor = _buffers(_currentBuffer)(i, j).Background
                    Console.Write(_buffers(_currentBuffer)(i, j).Character)

                End If

            Next
        Next

        'Set the new position and move on, ay
        Console.SetCursorPosition(_overlayManager.CursorPosition.X, _overlayManager.CursorPosition.Y)

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
                If overlayBuffer(i, j) IsNot Nothing Then

                    'Only take what has a value
                    If overlayBuffer(i, j).Background <> -1 Then
                        _buffers(_currentBuffer)(i, j).Background = overlayBuffer(i, j).Background
                    End If
                    If overlayBuffer(i, j).Foreground <> -1 Then
                        _buffers(_currentBuffer)(i, j).Foreground = overlayBuffer(i, j).Foreground
                    End If
                    If overlayBuffer(i, j).Character <> vbNullChar Then
                        _buffers(_currentBuffer)(i, j).Character = overlayBuffer(i, j).Character
                    End If

                End If

            Next

        Next

    End Sub


    Public Function GetCursorPosition() As BattleshipConsole.ConsolePosition
        'Give them the cursor of the position
        Return _overlayManager.CursorPosition
    End Function


    Public Property AutoRefresh As Boolean
        Set(value As Boolean)
            _autoRefresh = value
        End Set
        Get
            Return _autoRefresh
        End Get
    End Property


    Public Property DrawCursor As Boolean
        Set(value As Boolean)
            _drawCursor = value
        End Set
        Get
            Return _drawCursor
        End Get
    End Property

End Class
