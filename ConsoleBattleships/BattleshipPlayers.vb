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


    Sub New(Optional ByVal length As Integer = 1, Optional ByVal coord As BattleshipConsole.ConsolePosition = Nothing)

        'Setup the ship's resources
        _coord = New BattleshipConsole.ConsolePosition()
        _hits = New List(Of BattleshipConsole.ConsolePosition)
        Me.Length = length
        Me.Coord = coord


    End Sub


    Private Function CheckPosition(ByVal pos As BattleshipConsole.ConsolePosition, ByVal pos2 As BattleshipConsole.ConsolePosition) As Boolean

        'Is it is the same coord?
        If pos.X = pos2.X And
            pos.Y = pos2.Y Then

            'Ensure we have not hit this before
            If Not _hits.Contains(pos) Then

                _hits.Add(pos)

                'Check if we are dead
                If _hits.Count = Length Then
                    Dead = True
                End If

                Return True

            End If

        End If

        Return False

    End Function


    Public Function CheckHit(ByVal pos As BattleshipConsole.ConsolePosition) As Boolean

        'Check if the position would be in the ship's placement
        If Facing = BattelshipShipFacing.Right Then

            'Move to the right
            For i = Coord.X To Coord.X + Length - 1

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(i, Coord.Y)) Then
                    Return True
                End If

            Next

        ElseIf Facing = BattelshipShipFacing.Left Then

            'Move to the left
            For i = Coord.X To Coord.X - (Length - 1)

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(i, Coord.Y)) Then
                    Return True
                End If

            Next

        ElseIf Facing = BattelshipShipFacing.Up Then

            'Move to the up
            For i = Coord.Y To Coord.Y - (Length - 1)

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(Coord.X, i)) Then
                    Return True
                End If

            Next

        ElseIf _facing = BattelshipShipFacing.Down Then

            'Move to the down
            For i = Coord.Y To Coord.Y + (Length - 1)

                'Check just that position
                If CheckPosition(pos, New BattleshipConsole.ConsolePosition(Coord.X, i)) Then
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

    Sub New(Optional ByVal name As String = "")

        'Setup objects
        _name = name
        _ships = New List(Of BattleShip)

    End Sub

    Function AddShip(ByVal length As Integer, ByVal pos As BattleshipConsole.ConsolePosition) As Integer

        'Create a new ship and add it into the list
        _ships.Add(New BattleShip(length, pos))
        Return _ships.Count

    End Function

    Function CheckHit(ByVal pos As BattleshipConsole.ConsolePosition) As Boolean

        'Loop through the ships
        For Each s As BattleShip In _ships

            If s.CheckHit(pos) Then
                Return True
            End If

        Next

        Return False

    End Function

End Class

Public Class BattleshipPlayers

    Public player1 As BattleshipPlayer
    Public player2 As BattleshipPlayer

    Sub New()

        'TODO: Debug
        player1 = New BattleshipPlayer("Poodle")
        player2 = New BattleshipPlayer("Muncher")

    End Sub

End Class
