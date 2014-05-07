Module Module1

    Dim gfx As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)
    Dim shipCount As Integer

    Sub DrawSea()

        gfx.DrawSquare(0, 0, 12, 12, ConsoleColor.DarkBlue, ConsoleColor.White, True, 1, ConsoleColor.Gray)

    End Sub


    Sub ShipSetup(playerIndex As Integer, shipLength As Integer)

        'Message the user
        gfx.StartOverlay()
        gfx.Write(1, 15, "Set first ship location", , ConsoleColor.Red)
        gfx.FinishOverlay("location")

        'Wait for their selection and draw an x at the position
        Dim pos As BattleshipConsole.ConsolePosition = gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
        gfx.StartOverlay()
        gfx.Write(pos.X, pos.Y, "X", , ConsoleColor.Black)
        gfx.FinishOverlay("x marks the spot")

        'Write the message to the ser

        gfx.StartOverlay()
        gfx.Write(1, 15, "Ship faces: Left, right, up, down", , ConsoleColor.Red)
        gfx.FinishOverlay("ship facing")

        'Wait for the correct key
        Dim key As ConsoleKeyInfo
        While key.Key <> ConsoleKey.UpArrow And
             key.Key <> ConsoleKey.DownArrow And
             key.Key <> ConsoleKey.LeftArrow And
             key.Key <> ConsoleKey.RightArrow

            key = gfx.ReadKey(False)

        End While

        'Remove the X
        gfx.RemoveOverlay("x marks the spot")

        'Draw the ship graphic
        gfx.StartOverlay()
        Select Case key.Key
            Case ConsoleKey.UpArrow
                gfx.DrawSquare(pos.X, pos.Y - (shipLength - 1), 1, shipLength, 10 + shipCount)
            Case ConsoleKey.DownArrow
                gfx.DrawSquare(pos.X, pos.Y, 1, shipLength, 10 + shipCount)
            Case ConsoleKey.LeftArrow
                gfx.DrawSquare(pos.X - (shipLength - 1), pos.Y, shipLength, 1, 10 + shipCount)
            Case ConsoleKey.RightArrow
                gfx.DrawSquare(pos.X, pos.Y, shipLength, 1, 10 + shipCount)
        End Select
        gfx.FinishOverlay("ship" & shipCount)
        shipCount += 1

    End Sub


    Sub PlayerSetup(index As Integer)

        'Setup the ships for the first character
        gfx.Write(1, 13, "Player " & index & " Ship Setup", , ConsoleColor.Black)

        gfx.SetCursorMinimum(1, 1)
        gfx.SetCursorMaximum(10, 10)
        gfx.SetCursorPosition(1, 1)

        ShipSetup(index, 2)
        ShipSetup(index, 3)
        ShipSetup(index, 3)
        ShipSetup(index, 4)
        ShipSetup(index, 5)

    End Sub


    Sub PlayersSetup()

        PlayerSetup(1)
        'PlayerSetup(2)

    End Sub


    Sub Main()

        'Setup the objects
        DrawSea()
        PlayersSetup()
        'gfx.Refresh()

        gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
        
    End Sub

End Module
