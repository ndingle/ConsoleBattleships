﻿Module Module1

    Dim gfx As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)


    Sub DrawSea()

        gfx.DrawSquare(0, 0, 12, 12, ConsoleColor.DarkBlue, ConsoleColor.White, True, 1, ConsoleColor.Gray)

    End Sub


    Sub ShipSetup(playerIndex As Integer, shipLength As Integer)

        'Message the user
        gfx.StartOverlay()
        gfx.Write(1, 15, "Set first ship location", , ConsoleColor.Red)
        gfx.FinishOverlay()

        'Wait for their selection and draw an x at the position
        Dim pos As BattleshipConsole.ConsolePosition = gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
        gfx.StartOverlay()
        gfx.Write(pos.X, pos.Y, "X", , ConsoleColor.Black)
        gfx.FinishOverlay()

        'Write the message to the ser
        gfx.RemoveLastOverlay()
        gfx.StartOverlay()
        gfx.Write(1, 15, "Ship faces: Left, right, up, down")
        gfx.FinishOverlay()

        'Wait for the correct key
        Dim key As ConsoleKeyInfo
        While key.Key <> ConsoleKey.UpArrow And
             key.Key <> ConsoleKey.DownArrow And
             key.Key <> ConsoleKey.LeftArrow And
             key.Key <> ConsoleKey.RightArrow

            key = gfx.ReadKey(False)

        End While

        'Draw the ship graphic
        gfx.RemoveLastOverlay()
        gfx.RemoveLastOverlay()
        gfx.StartOverlay()
        Select Case key.Key
            Case ConsoleKey.UpArrow
                gfx.DrawSquare(pos.X, pos.Y - shipLength, 1, shipLength, ConsoleColor.Red)
            Case ConsoleKey.DownArrow
                gfx.DrawSquare(pos.X, pos.Y, 1, shipLength, ConsoleColor.Red)
            Case ConsoleKey.LeftArrow
                gfx.DrawSquare(pos.X - shipLength, pos.Y, shipLength, 1, ConsoleColor.Red)
            Case ConsoleKey.RightArrow
                gfx.DrawSquare(pos.X, pos.Y, shipLength, 1, ConsoleColor.Red)
        End Select
        gfx.FinishOverlay()


    End Sub


    Sub PlayerSetup()

        'Setup the ships for the first character
        gfx.Write(1, 13, "Player 1 Ship Setup", , ConsoleColor.Black)

        gfx.SetCursorMinimum(1, 1)
        gfx.SetCursorMaximum(10, 10)
        gfx.SetCursorPosition(1, 1)

        ShipSetup(1, 2)
        ShipSetup(1, 3)
        ShipSetup(1, 3)
        ShipSetup(1, 4)
        ShipSetup(1, 5)

        gfx.RemoveAllOverlays()

        '
        gfx.Write(1, 13, "Player 2 Ship Setup", , ConsoleColor.Black)

        gfx.SetCursorMinimum(1, 1)
        gfx.SetCursorMaximum(10, 10)
        gfx.SetCursorPosition(1, 1)

        ShipSetup(2, 2)
        ShipSetup(2, 3)
        ShipSetup(2, 3)
        ShipSetup(2, 4)
        ShipSetup(2, 5)

        gfx.RemoveAllOverlays()

    End Sub


    Sub Main()

        'Setup the objects
        DrawSea()
        PlayerSetup()

        gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
        
    End Sub

End Module
