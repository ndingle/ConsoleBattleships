Public Enum BattelshipShipFacing
    Up
    Down
    Left
    Right
End Enum

Public Class BattleShip

    Private _dead As Boolean
    Private _length As Integer
    Private _coord As BattleshipConsole.ConsolePosition
    Private _facing As BattelshipShipFacing = BattelshipShipFacing.Right
    Private _hits As List(Of BattleshipConsole.ConsolePosition)


    Sub New(ByVal facing As BattelshipShipFacing, Optional ByVal length As Integer = 1, Optional ByVal coord As BattleshipConsole.ConsolePosition = Nothing)

        'Setup the ship's resources
        _coord = New BattleshipConsole.ConsolePosition()
        _hits = New List(Of BattleshipConsole.ConsolePosition)
        Me.Facing = facing
        Me.Length = length
        Me.Coord = New BattleshipConsole.ConsolePosition(coord)


    End Sub


    Private Function CheckPosition(ByVal pos As BattleshipConsole.ConsolePosition, ByVal pos2 As BattleshipConsole.ConsolePosition, Optional shooting As Boolean = True) As Boolean

        'Is it is the same coord?
        If pos.X = pos2.X And
            pos.Y = pos2.Y Then

            If shooting Then

                'Ensure we have not hit this before
                Dim contains As Boolean = False
                For Each h As BattleshipConsole.ConsolePosition In _hits
                    If h = pos Then
                        contains = True
                        Exit For
                    End If
                Next

                'If not, then add it and check if the ship died
                If Not contains Then

                    _hits.Add(New BattleshipConsole.ConsolePosition(pos))

                    'Check if we are dead
                    If _hits.Count = Length Then
                        Dead = True
                    End If

                    Return True

                End If
            Else
                Return True
            End If

        End If

        Return False

    End Function


    Public Function CheckHit(ByVal pos As BattleshipConsole.ConsolePosition, Optional shooting As Boolean = True) As Boolean

        'Check if the position would be in the ship's placement
        If Facing = BattelshipShipFacing.Right Then

            'Move to the right
            For i = Coord.X To Coord.X + (Length - 1)

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(i, Coord.Y), shooting) Then
                    Return True
                End If

            Next

        ElseIf Facing = BattelshipShipFacing.Left Then

            'Move to the left
            For i = Coord.X - (Length - 1) To Coord.X

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(i, Coord.Y), shooting) Then
                    Return True
                End If

            Next

        ElseIf Facing = BattelshipShipFacing.Up Then

            'Move to the up
            For j = Coord.Y - (Length - 1) To Coord.Y

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(Coord.X, j), shooting) Then
                    Return True
                End If

            Next

        ElseIf _facing = BattelshipShipFacing.Down Then

            'Move to the down
            For j = Coord.Y To Coord.Y + (Length - 1)

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(Coord.X, j), shooting) Then
                    Return True
                End If

            Next

        End If

        'Not found, end this search
        Return False

    End Function


    Public Property Length As Integer
        Set(ByVal value As Integer)
            'Ensure we have a positive length
            If value > 0 Then
                _length = value
            End If
        End Set
        Get
            Return _length
        End Get
    End Property


    Public Property Coord() As BattleshipConsole.ConsolePosition
        Set(ByVal value As BattleshipConsole.ConsolePosition)
            'Ensure we have a value
            If Not value Is Nothing Then
                _coord = value
            End If
        End Set
        Get
            Return _coord
        End Get
    End Property


    Public Property Facing As BattelshipShipFacing
        Set(ByVal value As BattelshipShipFacing)
            If value >= 0 And value <= 3 Then
                _facing = value
            End If
        End Set
        Get
            Return _facing
        End Get
    End Property


    Public Property Dead As Boolean
        Set(ByVal value As Boolean)
            _dead = value
        End Set
        Get
            Return _dead
        End Get
    End Property


End Class


Public Class BattleshipPlayer

    Private _cursor As BattleshipConsole.ConsolePosition
    Private _ships As List(Of BattleShip)
    Private _shots As List(Of BattleshipConsole.ConsolePosition)

    Sub New(x As Integer, y As Integer)

        'Setup objects
        _cursor = New BattleshipConsole.ConsolePosition(x, y)
        _ships = New List(Of BattleShip)
        _shots = New List(Of BattleshipConsole.ConsolePosition)

    End Sub


    Function AddShip(ByVal facing As BattelshipShipFacing, ByVal length As Integer, ByVal pos As BattleshipConsole.ConsolePosition) As Integer

        'Create a new ship and add it into the list
        _ships.Add(New BattleShip(facing, length, pos))
        Return _ships.Count

    End Function


    Function CheckHit(ByVal pos As BattleshipConsole.ConsolePosition, Optional ByVal shooting As Boolean = True) As Integer

        'Loop through the ships
        Dim result As Integer = -1
        For i = 0 To _ships.Count - 1

            If _ships(i).CheckHit(pos, shooting) Then
                'Remember where we are up to and then exit early
                result = i
                Exit For
            End If

        Next

        Return result

    End Function


    Public Function CheckHit(x As Integer, y As Integer, Optional shooting As Boolean = True) As Integer

        'Overload this awesome function!
        Return CheckHit(New BattleshipConsole.ConsolePosition(x, y), shooting)

    End Function


    Public Function AddShot(pos As BattleshipConsole.ConsolePosition) As Boolean

        'Check if the position already exists 
        For Each p As BattleshipConsole.ConsolePosition In _shots
            If p = pos Then
                Return False
            End If
        Next

        'Add the shot
        _shots.Add(New BattleshipConsole.ConsolePosition(pos))
        Return True

    End Function


    Public Function AddShot(x As Integer, y As Integer) As Boolean

        'Check we have a the shot first
        Return AddShot(New BattleshipConsole.ConsolePosition(x, y))

    End Function


    Public Function GetDeadShips() As Integer

        'Go through and count the dead ships
        Dim count As Integer = 0

        For Each ship As BattleShip In _ships
            If ship.Dead Then count += 1
        Next

        Return count

    End Function


    Public Function IsDefeated() As Boolean

        'All of the ships dead?
        If GetDeadShips() = _ships.Count Then
            Return True
        Else
            Return False
        End If

    End Function


    Public Property Cursor As BattleshipConsole.ConsolePosition
        Set(value As BattleshipConsole.ConsolePosition)
            If value IsNot Nothing Then
                _cursor = value
            End If
        End Set
        Get
            Return _cursor
        End Get
    End Property


End Class

Public Class BattleshipPlayers

    Private _players(1) As BattleshipPlayer


    Public Sub SetupPlayer(playerIndex As Integer, x As Integer, y As Integer)

        _players(playerIndex) = New BattleshipPlayer(x, y)

    End Sub


    Public ReadOnly Property Player(index As Integer) As BattleshipPlayer
        Get
            If index >= 0 And index <= 1 Then
                Return _players(index)
            Else
                Return Nothing
            End If
        End Get
    End Property

End Class
