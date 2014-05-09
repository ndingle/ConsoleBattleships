Module Module1

    Dim gfx As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)
    Dim players As New BattleshipPlayers
    Dim shipCount As Integer

    Sub DrawSea()

        gfx.DrawSquare(0, 0, 12, 12, ConsoleColor.DarkBlue, ConsoleColor.White, True, 1, ConsoleColor.Gray)

    End Sub


    Sub ShipSetup(playerIndex As Integer, shipLength As Integer)

        'Message the user
        gfx.StartOverlay()
        gfx.Write(1, 15, "Set ship location", , ConsoleColor.Red)
        gfx.FinishOverlay("location")

        Dim hit As Boolean = False
        Dim facing As BattelshipShipFacing
        Dim pos As BattleshipConsole.ConsolePosition

        'Loop until we have no extra hits
        Do

            'Incase we repeat
            gfx.RemoveOverlay("x marks the spot")
            gfx.RemoveOverlay("ship facing")

            'Wait for their selection and draw an x at the position
            pos = gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
            gfx.StartOverlay()
            gfx.Write(pos.X, pos.Y, "X", , ConsoleColor.Black)
            gfx.FinishOverlay("x marks the spot")

            'Write the message to the ser

            gfx.StartOverlay()
            gfx.Write(1, 15, "Ship faces: Left, right, up, down", , ConsoleColor.Red)
            gfx.FinishOverlay("ship facing")

            'Wait for the correct key
            Dim key As ConsoleKeyInfo
            key = New ConsoleKeyInfo
            hit = False

            'Loop until we hit the correct key
            Do

                key = gfx.ReadKey(False, False)

            Loop Until key.Key = ConsoleKey.UpArrow Or
                    key.Key = ConsoleKey.DownArrow Or
                    key.Key = ConsoleKey.LeftArrow Or
                    key.Key = ConsoleKey.RightArrow

            'Remove the X
            gfx.RemoveOverlay("x marks the spot")

            Select Case key.Key
                Case ConsoleKey.UpArrow

                    For j = pos.Y - (shipLength - 1) To pos.Y

                        'Ensure this coord is in the boundaries
                        If gfx.IsCoordInMouseBounds(pos.X, j) Then

                            If players.players(playerIndex - 1).CheckHit(pos.X, j, False) > -1 Then
                                hit = True
                                Exit For
                            End If

                        Else
                            hit = True
                            Exit For
                        End If

                    Next

                Case ConsoleKey.DownArrow

                    For j = pos.Y To pos.Y + (shipLength - 1)

                        'Ensure this coord is in the boundaries
                        If gfx.IsCoordInMouseBounds(pos.X, j) Then

                            If players.players(playerIndex - 1).CheckHit(pos.X, j, False) > -1 Then
                                hit = True
                                Exit For
                            End If

                        Else
                            hit = True
                            Exit For
                        End If

                    Next

                Case ConsoleKey.LeftArrow

                    For i = pos.X - (shipLength - 1) To pos.X

                        If gfx.IsCoordInMouseBounds(i, pos.Y) Then

                            If players.players(playerIndex - 1).CheckHit(i, pos.Y, False) > -1 Then
                                hit = True
                                Exit For
                            End If

                        Else
                            hit = True
                            Exit For
                        End If

                    Next

                Case ConsoleKey.RightArrow

                    For i = pos.X To pos.X + (shipLength - 1)

                        If gfx.IsCoordInMouseBounds(i, pos.Y) Then

                            If players.players(playerIndex - 1).CheckHit(i, pos.Y, False) > -1 Then
                                hit = True
                                Exit For
                            End If

                        Else
                            hit = True
                            Exit For
                        End If

                    Next

            End Select

            'Draw the ship graphic
            If Not hit Then

                gfx.StartOverlay()
                Select Case key.Key
                    Case ConsoleKey.UpArrow
                        gfx.DrawSquare(pos.X, pos.Y - (shipLength - 1), 1, shipLength, 10 + shipCount)
                        facing = BattelshipShipFacing.Up
                    Case ConsoleKey.DownArrow
                        gfx.DrawSquare(pos.X, pos.Y, 1, shipLength, 10 + shipCount)
                        facing = BattelshipShipFacing.Down
                    Case ConsoleKey.LeftArrow
                        gfx.DrawSquare(pos.X - (shipLength - 1), pos.Y, shipLength, 1, 10 + shipCount)
                        facing = BattelshipShipFacing.Left
                    Case ConsoleKey.RightArrow
                        gfx.DrawSquare(pos.X, pos.Y, shipLength, 1, 10 + shipCount)
                        facing = BattelshipShipFacing.Right
                End Select
                gfx.FinishOverlay("ship" & shipCount)

            End If

        Loop Until hit = False

        'Add in the new ship
        players.players(playerIndex - 1).AddShip(facing, shipLength, pos)

        'Increase the ship count
        shipCount += 1

    End Sub


    Sub PlayerSetup(index As Integer)

        'Setup the ships for the first character
        gfx.StartOverlay()
        gfx.Write(1, 13, "Player " & index & " Ship Setup", , ConsoleColor.Black)
        gfx.FinishOverlay("Player" & index)

        gfx.SetCursorMinimum(1, 1)
        gfx.SetCursorMaximum(10, 10)
        gfx.SetCursorPosition(1, 1)

        ShipSetup(index, 2)
        ShipSetup(index, 3)
        ShipSetup(index, 3)
        ShipSetup(index, 4)
        ShipSetup(index, 5)

        'Reset the ship counts
        shipCount = 0

        gfx.RemoveOverlay("Player" & index)
        gfx.RemoveOverlay("location")
        gfx.StartOverlay()
        gfx.RemoveOverlay("ship facing")
        gfx.Write(1, 13, "Player " & index & " setup complete! Press enter to continue.")
        gfx.FinishOverlay("PlayerComplete")
        gfx.WaitForKey(New ConsoleKey() {ConsoleKey.Enter})

    End Sub


    Sub PlayersSetup()

        'Setup the players
        PlayerSetup(1)
        gfx.RemoveAllOverlays()
        PlayerSetup(2)
        gfx.RemoveAllOverlays()

        'Tell them we're done!
        gfx.StartOverlay()
        gfx.Write(1, 13, "All players setup!\nPress enter to start the game", , ConsoleColor.Red)
        gfx.FinishOverlay("complete")

        gfx.WaitForKey(New ConsoleKey() {ConsoleKey.Enter})

        gfx.RemoveAllOverlays()

    End Sub


    Sub GameSetup()

        'Set cursor
        gfx.SetCursorPosition(1, 1)

        'Ships underneath all
        For i = 0 To 1
            For j = 0 To 4
                'Create the base overlays and disable them
                gfx.StartOverlay(10 + j)
                gfx.FinishOverlay("player" & i & "ship" & j)
                gfx.HideOverlay("player" & i & "ship" & j)
            Next

            'Player shot layer
            gfx.StartOverlay()
            gfx.FinishOverlay("player" & i & "shots")
            gfx.HideOverlay("player" & i & "shots")

        Next

        'Setup the announcement label
        gfx.StartOverlay()
        gfx.FinishOverlay("announcement")

    End Sub


    Sub PlayerShot(ByVal index As Integer)

        'Activate their overlays and disable the other players
        For i = 0 To 4
            gfx.ShowOverlay("player" & index & "ship" & i)
            gfx.HideOverlay("player" & (1 - i) & "ship" & i)
        Next

        'Setup the shows overlays
        gfx.ShowOverlay("player" & index & "shots")
        gfx.HideOverlay("player" & (1 - index) & "shots")

        'Tell them what they have to do
        gfx.Write(1, 13, "Player " & index + 1 & " take your shot!")

        'Let them select a position
        Dim pos As BattleshipConsole.ConsolePosition

        'Loop until they choose a new location
        Do
            pos = New BattleshipConsole.ConsolePosition
            pos = gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
        Loop Until Not players.players(index).IsAlreadyShot(pos)

        'Add to the shot overlay
        gfx.Write(pos.X, pos.Y, "X", , ConsoleColor.Black, "player" & index & "shots")

        'Check if they actually hit the other player's ship
        Dim shipNum As Integer = players.players(1 - index).CheckHit(pos)

        If shipNum > -1 Then

            'Hit!
            'Draw on the ship's overlay
            'TODO: Change the colour of the background
            gfx.DrawSquare(pos.X, pos.Y, 1, 1, ConsoleColor.Yellow, , , , , "player" & index & "ship" & shipNum)
            gfx.Write(1, 13, "Hit!! Player " & (1 - index) + 1 & "'s turn now.")

        Else

            gfx.Write(1, 13, "Miss. Player " & (1 - index) + 1 & "'s turn now.")

        End If

        gfx.WaitForKey(New ConsoleKey() {ConsoleKey.Enter})

    End Sub


    Sub Main()

        'Setup the objects
        DrawSea()
        PlayersSetup()
        GameSetup()
        PlayerShot(0)
        PlayerShot(1)
        
    End Sub

End Module
