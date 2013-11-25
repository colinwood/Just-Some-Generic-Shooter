namespace JustSomeGenericShooter
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main()
        {
            using (var game = new GenericGame())
            {
                game.Run();
            }
        }
    }
#endif
}

