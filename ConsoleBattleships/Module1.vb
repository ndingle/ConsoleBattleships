﻿Module Module1

    Dim test As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)

    Sub Main()

        'Setup the objects
        test.DrawSquare(0, 0, 30, 25, ConsoleColor.White, , True, 1, ConsoleColor.Black)
        test.SetCursorMinimum(1, 1)
        test.SetCursorMaximum(28, 23)
        test.Refresh()
        MsgBox(test.ReadLine())
        test.Refresh()
        Console.ReadLine()
        
    End Sub

End Module