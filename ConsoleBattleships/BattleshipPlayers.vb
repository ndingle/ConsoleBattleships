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
                If Not _hits.Contains(pos) Then

                    _hits.Add(pos)

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

    Private _name As String
    Private _ships As List(Of BattleShip)
    Private _shots As List(Of BattleshipConsole.ConsolePosition)

    Sub New(Optional ByVal name As String = "")

        'Setup objects
        _name = name
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
        For i = 0 To _ships.Count - 1

            If _ships(i).CheckHit(pos, shooting) Then
                Return i
            End If

        Next

        'If we are shooting then record the shot
        If shooting Then
            _shots.Add(New BattleshipConsole.ConsolePosition(pos))
        End If

        Return -1

    End Function


    Public Function CheckHit(x As Integer, y As Integer, Optional shooting As Boolean = True) As Boolean

        'Overload this awesome function!
        Return CheckHit(New BattleshipConsole.ConsolePosition(x, y), shooting)

    End Function


    Public Function IsAlreadyShot(pos As BattleshipConsole.ConsolePosition) As Boolean

        'Check we have a the shot first
        If _shots.IndexOf(pos) >= 0 Then
            Return True
        Else
            Return False
        End If

    End Function


    Public Function IsAlreadyShot(x As Integer, y As Integer) As Boolean

        'Check we have a the shot first
        Return IsAlreadyShot(New BattleshipConsole.ConsolePosition(x, y))

    End Function


End Class

Public Class BattleshipPlayers

    Public players(1) As BattleshipPlayer

    Sub New()

        'TODO: Debug
        players(0) = New BattleshipPlayer("Poodle")
        players(1) = New BattleshipPlayer("Muncher")

    End Sub


End Class
