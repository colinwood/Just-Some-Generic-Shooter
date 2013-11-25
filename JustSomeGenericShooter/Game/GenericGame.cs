using DigitalRune.Physics;
using DigitalRune.Physics.ForceEffects;
using JustSomeGenericShooter.Components;
using JustSomeGenericShooter.InputBindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JustSomeGenericShooter
{
    public class GenericGame : Game
    {
        public Simulation Simulation { get; private set; }

        GraphicsDeviceManager graphics;

        public GenericGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Create a new simulation with common settings.
            Simulation = new Simulation();
            Simulation.Settings.Timing.MaxNumberOfSteps = 1;
            Simulation.ForceEffects.Add(new Gravity());
            Simulation.ForceEffects.Add(new Damping());

            //Add game components.
            Components.Add(new Map(this));                  //Creates level with test obstacles.
            Components.Add(new Camera(this));               //Defines the camera position, orientation and projection.
            Components.Add(new Player(this));               //Controls the player vehicle.
            Components.Add(new KeyboardBinding(this));      //Creates a keyboard binding object
            Components.Add(new MouseBinding(this));         //Creates a mouse binding object

            base.Initialize();      
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AliceBlue);

            base.Draw(gameTime);
        }
    }
}
