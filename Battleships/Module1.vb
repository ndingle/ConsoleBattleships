Module Module1

    Dim enterNotPressed As Boolean = True
    Dim test As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)

    Sub Enter()
        enterNotPressed = False
    End Sub

    Sub Main()

        'Setup the objects
        AddHandler test.OnEnterPressed, AddressOf Enter
        test.WriteOverlay()
        While enterNotPressed
            test.CursorMovement()
        End While

    End Sub

End Module
