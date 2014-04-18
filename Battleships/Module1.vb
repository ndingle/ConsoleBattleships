Module Module1

    Dim test As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)

    Sub Main()

        'Setup the objects
        test.DrawSquare(0, 0, 5, 5, ConsoleColor.White, , True, 1, ConsoleColor.Black)
        test.StartOverlay()
        test.DrawSquare(0, 0, 80, 25, ConsoleColor.Red, ConsoleColor.Black)
        test.WriteOverlay(10, 2, "Something happened...", ConsoleColor.Black, ConsoleColor.Red)
        test.FinishOverlay()
        test.Refresh()
        Console.ReadLine()
        
    End Sub

End Module
