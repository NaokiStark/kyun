using kyun.GameModes;

//This is a test of platformer

namespace kyun.game.GameModes.Test
{
    public class TestScreen : GameModeScreenBase
    {
        private TestPlayer pl;

        private static TestScreen instance;

        public static new TestScreen GetInstance()
        {
            if (instance == null)
                instance = new TestScreen();

            return instance;
        }

        public TestScreen() : base("Test Game")
        {
            pl = new TestPlayer();
            Controls.Add(pl);
        }
    }
}
