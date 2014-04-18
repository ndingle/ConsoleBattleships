Module Module1

    Dim test As New BattleshipConsole(ConsoleColor.Yellow, ConsoleColor.Gray, ConsoleColor.Red)

    Sub Main()

        'Setup the objects
        test.DrawSquare(0, 0, 30, 25, ConsoleColor.White, , True, 1, ConsoleColor.Black)
        test.StartOverlay()
        test.WriteOverlay(10, 2, "Something happened...", ConsoleColor.Black, ConsoleColor.Red)
        test.FinishOverlay()
        test.Refresh()
        Console.ReadLine()
        test.RemoveOverlay()
        test.Refresh()
        Console.ReadLine()
        
    End Sub

End Module
