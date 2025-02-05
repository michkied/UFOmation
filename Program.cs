namespace UFOmation;

public static class Program
{
    public static void Main()
    {
        var game = new Animation(800, 600, "Hello World");
        game.Run();
    }
}