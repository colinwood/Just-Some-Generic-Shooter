using System.Linq;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Specialized;
using JustSomeGenericShooter.InputBindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MathHelper = Microsoft.Xna.Framework.MathHelper;

namespace JustSomeGenericShooter.Components
{
    public class Player : DrawableGameComponent
    {
        private Camera camera;
        private float yaw;
        private float pitch;
        private const float LinearVelocityMagnitude = 5f;
        private const int MouseOrigin = 300;
        private Model gun;
        private Simulation simulation;
        private IBinding keyboardBinding;
        private IBinding mouseBinding;
        private readonly Vector3F[] moveDirection = { Vector3F.Zero };
        private GameTime gTime = new GameTime();

        public Pose GunPosition { get; private set; }
        public KinematicCharacterController CharacterController { get; set; }

        public Pose Pose
        {
            get
            {
                Vector3F position = CharacterController.Position;
                QuaternionF orientation = QuaternionF.CreateRotationY(yaw) * QuaternionF.CreateRotationX(pitch);
                return new Pose(position, orientation);
            }
        }

        public Player(Game game) : base(game) { }

        public override void Initialize()
        {
            // Get a reference to all of the other dependent components
            camera = Game.Components.OfType<Camera>().First();
            simulation = ((GenericGame)Game).Simulation;

            //Need to differentiate between the IBinding objects as we need both of them
            //We COULD fetch them by actual type, but I'd rather get them in a generic fashion with and interface
            foreach (var binding in Game.Components.OfType<IBinding>())
            {
                if (binding is KeyboardBinding)
                    keyboardBinding = binding;
                else if (binding is MouseBinding)
                    mouseBinding = binding;
            }

            //Bind thw WASD keys to the keyboard controller with the appropriate movement actions
            //Also, lambdas are very SEXY -Nick
            keyboardBinding.Bind(Keys.W, () => { moveDirection[0].Z--; });
            keyboardBinding.Bind(Keys.S, () => { moveDirection[0].Z++; });
            keyboardBinding.Bind(Keys.A, () => { moveDirection[0].X--; });
            keyboardBinding.Bind(Keys.D, () => { moveDirection[0].X++; });

            //Same with the mouse x and y axes
            mouseBinding.Bind(MState.XAxis, () =>
            {
                var deltaTime = (float)gTime.ElapsedGameTime.TotalSeconds;

                //Compute new yaw from mouse delta x.
                float deltaYaw = 0;
                deltaYaw -= Mouse.GetState().X - MouseOrigin;
                yaw += deltaYaw * deltaTime * 0.1f;
            });

            mouseBinding.Bind(MState.YAxis, () =>
            {
                var deltaTime = (float)gTime.ElapsedGameTime.TotalSeconds;

                //Compute new pitch from mouse delta y.
                float deltaPitch = 0;
                deltaPitch -= Mouse.GetState().Y - MouseOrigin;
                pitch += deltaPitch * deltaTime * 0.1f;

                //Only reset on Y because we need BOTH x and y to evaluate before the reset happens
                Mouse.SetPosition(MouseOrigin, MouseOrigin);
            });


            CharacterController = new KinematicCharacterController(simulation)
            {
                Position = new Vector3F(-0.5F, 0, 0.5F),
                Gravity = 10
            };

            gun = Game.Content.Load<Model>("gun");

            Mouse.SetPosition(MouseOrigin, MouseOrigin);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            gTime = gameTime;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update the camera to point the same direction we are facing
            UpdateCamera();

            //Update the orientation (the direction we are facing)
            var orientation = QuaternionF.CreateRotationY(yaw) * QuaternionF.CreateRotationX(pitch);

            //Apply movement
            moveDirection[0] = orientation.Rotate(moveDirection[0]);
            moveDirection[0].TryNormalize();
            var moveVelocity = moveDirection[0] * LinearVelocityMagnitude;
            CharacterController.Move(moveVelocity, 0.0F, deltaTime);

            //Reset movement back to zero
            moveDirection[0] = Vector3F.Zero;
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            //return if the window is not in focus
            if (!Game.IsActive) return;

            //Limit the pitch angle to +/- 90°.
            //TODO: For some reason, this is not working.
            pitch = MathHelper.Clamp(pitch, -89, 89);
        }

        public override void Draw(GameTime gameTime)
        {
            //The only needed object that the player needs to draw is the gun.
            //We update here so the move happens in the same frame to avoid 'jumping'
            UpdateGunPosition();

            DrawModel(gun, GunPosition, camera.Pose.Inverse, camera.Projection);
            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Pose pose, Matrix view, Matrix projection)
        {
            //Compute bones transfomations relative to parent bone
            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //Iterate through all meshes, set up the lighting, and draw it.
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index] * pose;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        private void UpdateGunPosition()
        {
            //Init some variables
            var pose = camera.player.Pose;
            const float x = -89.5F;
            const float z = 89.6F;

            //Create offset for gun's orientation.
            var gunOrientation = pose.Orientation * Matrix33F.CreateRotationX(x) * Matrix33F.CreateRotationZ(z);
            var gunDistance = gunOrientation * new Vector3F(0.3F, -0.1F, -0.2F);

            //Create offset for gun's orientation.
            var eyeHeight = new Vector3F(0, camera.player.CharacterController.Height - 0.12f, 0);
            var position = pose.Position + eyeHeight + gunDistance;

            //Update pose
            GunPosition = new Pose(position, gunOrientation);
        }
        
    }
}
