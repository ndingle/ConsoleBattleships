Module Module1

    Dim gfx As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Black, ConsoleColor.Red)
    Dim players As New BattleshipPlayers
    Dim shipCount As Integer

    Const X_START As Integer = 15
    Const Y_START As Integer = 6
    Const X_WIDTH As Integer = 12
    Const Y_HEIGHT As Integer = 12
    Const X_PLAYER_INC As Integer = 36

    Sub DrawSea(playerIndex As Integer)

        gfx.Write(X_START + (X_PLAYER_INC * playerIndex + 2), Y_START - 1, "Player " & playerIndex + 1)
        gfx.DrawSquare(X_START + (X_PLAYER_INC * playerIndex), Y_START, X_WIDTH, Y_HEIGHT, ConsoleColor.DarkBlue, , True, 1, ConsoleColor.Black)

    End Sub


    Sub ShipSetup(playerIndex As Integer, shipLength As Integer)

        'Message the user
        gfx.StartOverlay()
        gfx.FinishOverlay("location")

        Dim hit As Boolean = False
        Dim facing As BattelshipShipFacing
        Dim pos As BattleshipConsole.ConsolePosition

        'Loop until we have no extra hits
        Do

            gfx.EraseAllContent("location")
            gfx.Write(X_START + 14, Y_START + Y_HEIGHT - 2, "Choose ship location", ConsoleColor.Black, ConsoleColor.Red, "location")

            'Incase we repeat
            gfx.RemoveOverlay("x marks the spot")
            gfx.RemoveOverlay("ship facing")

            'Wait for their selection and draw an x at the position
            Do
                pos = gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter})
            Loop Until players.Player(playerIndex).CheckHit(pos, False) = -1

            gfx.StartOverlay()
            gfx.Write(pos.X, pos.Y, "X", , ConsoleColor.Black)
            gfx.FinishOverlay("x marks the spot")

            'Write the message to the ser
            gfx.Write(X_START + 14, Y_START + Y_HEIGHT - 2, "Left right up or down", ConsoleColor.Black, ConsoleColor.Red, "location")

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

                            If players.Player(playerIndex).CheckHit(pos.X, j, False) > -1 Then
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

                            If players.Player(playerIndex).CheckHit(pos.X, j, False) > -1 Then
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

                            If players.Player(playerIndex).CheckHit(i, pos.Y, False) > -1 Then
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

                            If players.Player(playerIndex).CheckHit(i, pos.Y, False) > -1 Then
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
        players.Player(playerIndex).AddShip(facing, shipLength, pos)

        'Increase the ship count
        shipCount += 1

    End Sub


    Sub SetupPlayerCursor(playerIndex As Integer)

        'Based on the player's index set the cursor up
        gfx.SetCursorMaximum(X_START + X_WIDTH - 2 + (playerIndex * X_PLAYER_INC), Y_START + Y_HEIGHT - 2)
        gfx.SetCursorMinimum(X_START + 1 + (playerIndex * X_PLAYER_INC), Y_START + 1)
        gfx.SetCursorPosition(players.Player(playerIndex).Cursor)

    End Sub


    Sub PlayerSetup(playerIndex As Integer)

        'Ensure the player is setup
        players.SetupPlayer(playerIndex, X_START + 1 + (playerIndex * X_PLAYER_INC), Y_START + 1)

        'Setup the ships for the first character
        gfx.StartOverlay()
        gfx.Write(X_START + 16, Y_START + 1, "Player " & playerIndex + 1 & "'s turn.", , ConsoleColor.Red)
        gfx.FinishOverlay("Player" & playerIndex)

        'Setup the cursor
        SetupPlayerCursor(playerIndex)

        ShipSetup(playerIndex, 2)
        ShipSetup(playerIndex, 3)
        ShipSetup(playerIndex, 3)
        ShipSetup(playerIndex, 4)
        ShipSetup(playerIndex, 5)

        'Reset the ship counts
        shipCount = 0

        gfx.RemoveOverlay("Player" & playerIndex)
        gfx.RemoveOverlay("location")
        gfx.RemoveOverlay("ship facing")
        gfx.StartOverlay()
        gfx.FinishOverlay("PlayerComplete")
        gfx.Write(X_START + 12, Y_START + 1, "Player " & playerIndex + 1 & " setup complete!\nPress enter to continue.", , ConsoleColor.Red, "PlayerComplete")
        gfx.WaitForKey(New ConsoleKey() {ConsoleKey.Enter})

    End Sub


    Sub PlayersSetup()

        'Setup the players
        For i = 0 To 1
            PlayerSetup(i)
            gfx.RemoveAllOverlays()
        Next

    End Sub


    Sub GameSetup()

        'Ships underneath all
        For i = 0 To 1
            For j = 0 To 4
                'Create the base overlays and disable them
                gfx.StartOverlay()
                gfx.FinishOverlay("player" & i & "ship" & j)
            Next

            'Player shot layer
            gfx.StartOverlay()
            gfx.FinishOverlay("player" & i & "shots")

            gfx.StartOverlay()
            gfx.FinishOverlay("player" & i & "hitOrMiss")

        Next

        'Setup the announcement label
        gfx.StartOverlay()
        gfx.FinishOverlay("announcement")

    End Sub


    Sub PlayerShot(ByVal playerIndex As Integer)

        'Tell them what they have to do
        gfx.EraseAllContent("announcement")
        gfx.EraseAllContent("player" & playerIndex & "hitOrMiss")
        gfx.Write(X_START + 16, Y_START + 1, "Player " & playerIndex + 1 & "'s turn.", , ConsoleColor.Red, "announcement")

        'Setup the cursor
        SetupPlayerCursor(playerIndex)
        gfx.Refresh()

        'Let them select a position
        Dim pos As BattleshipConsole.ConsolePosition
        Dim oldPos As BattleshipConsole.ConsolePosition

        'Loop until they choose a new location and auto add the shot if needs be
        Do
            pos = New BattleshipConsole.ConsolePosition(gfx.MoveCursorUntilKeyPress(New ConsoleKey() {ConsoleKey.Enter}))
            oldPos = New BattleshipConsole.ConsolePosition(pos)
            'Move the shot to the opposite side
            'TODO: Figure out better maths here
            If playerIndex = 0 Then
                pos.X = pos.X + X_PLAYER_INC
            Else
                pos.X = pos.X - X_PLAYER_INC
            End If
        Loop Until players.Player(playerIndex).AddShot(pos)

        'Add to the shot overlay
        gfx.Write(oldPos.X, pos.Y, "X", , ConsoleColor.Black, "player" & playerIndex & "shots")

        'Check if they actually hit the other player's ship
        Dim shipNum As Integer = players.Player(1 - playerIndex).CheckHit(pos, True)

        If shipNum > -1 Then

            'Hit!
            'Draw on the ship's overlay
            gfx.DrawSquare(oldPos.X, pos.Y, 1, 1, ConsoleColor.White, , , , , "player" & playerIndex & "ship" & shipNum)
            gfx.Write(X_START + 1 + (playerIndex * X_PLAYER_INC), Y_START + Y_HEIGHT / 2 - 1, "   Hit!   ", ConsoleColor.Black, ConsoleColor.Green, "player" & playerIndex & "hitOrMiss")

        Else

            'Miss
            gfx.Write(X_START + 1 + (playerIndex * X_PLAYER_INC), Y_START + Y_HEIGHT / 2 - 1, "   Miss   ", ConsoleColor.Black, ConsoleColor.Red, "player" & playerIndex & "hitOrMiss")

        End If

        'Setup the player position
        players.Player(playerIndex).Cursor = oldPos

        gfx.Refresh()

    End Sub


    Sub Main()

        'Setup the objects
        For i = 0 To 1
            DrawSea(i)
        Next
        PlayersSetup()
        GameSetup()

        'Loop until one of them dies!
        While True

            For i = 0 To 1

                'Shoot!
                PlayerShot(i)

                'Just see if anyone won yet
                If players.Player(0).IsDefeated Or
                   players.Player(1).IsDefeated Then
                    Exit While
                End If

            Next

        End While

        'Clear the screen
        gfx.RemoveAllOverlays()
        gfx.EraseAllContent()
        gfx.SetCursorMinimum(0, 0)
        gfx.SetCursorMaximum(BattleshipConsole.CONSOLE_WIDTH - 1, BattleshipConsole.CONSOLE_HEIGHT - 1)
        gfx.SetCursorPosition(0, 0)
        gfx.DrawCursor = False

        'Determine who won
        Dim winningPlayer As Integer = 0
        If players.Player(0).IsDefeated Then
            winningPlayer = 1
        End If

        'Message them!
        gfx.Write(30, BattleshipConsole.CONSOLE_HEIGHT / 2 - 1, "Player " & winningPlayer + 1 & " wins!!!!", ConsoleColor.Red, ConsoleColor.White)
        gfx.WaitForKey(New ConsoleKey() {ConsoleKey.Enter})

    End Sub

End Module
