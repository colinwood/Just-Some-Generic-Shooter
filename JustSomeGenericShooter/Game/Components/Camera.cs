using System.Linq;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace JustSomeGenericShooter.Components
{
    public class Camera : DrawableGameComponent
    {
        public Player player;
        private const float thirdPersonOffset = 0.1F;

        public Pose Pose
        {
            get
            {
                // Get pose of the player. (This is the ground position, not the head position.)
                var pose = player.Pose;

                // Create offset vector from player to the camera.
                var orientation = pose.Orientation;
                var thirdPersonDistance = orientation * new Vector3F(0, 0, thirdPersonOffset);

                // Compute camera position. 
                var eyeHeight = new Vector3F(0, player.CharacterController.Height - 0.12f, 0);
                var position = pose.Position + eyeHeight + thirdPersonDistance;

                return new Pose(position, orientation);
            }
        }

        public Matrix Projection { get; private set; }


        public Camera(Game game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            // Store reference to Player.
            player = Game.Components.OfType<Player>().First();

            // Create the projection matrix.
            Projection = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(60),
              GraphicsDevice.Viewport.AspectRatio,
              0.1f,
              1000f);

            base.LoadContent();
        }
    }
}
