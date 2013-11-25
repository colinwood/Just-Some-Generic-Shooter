using System.Linq;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JustSomeGenericShooter.Components
{
    public class Map : DrawableGameComponent
    {
        private Simulation simulation;
        private Model model;

        public Map(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            simulation = ((GenericGame)Game).Simulation;

            //Floor
            AddBody("GroundPlane", Pose.Identity, new PlaneShape(Vector3F.UnitY, -2.5F), MotionType.Static);
            
            //Loaf Map01 model from content pipeline.
            model = Game.Content.Load<Model>("map01");

            //Collision map vertices (Map01 is concave, so automatic generation is not possible with this engine)
            CreateWall(0F, -7F, 16F);           CreateWall(0F, 7F, 16F);            CreateWall(-7F, 0F, 16F, true);     CreateWall(7F, 0F, 16F, true); 
            CreateWall(-5F, -6F, 4F);           CreateWall(1.5F, -6F, 3F);          CreateWall(5F, -6F, 2F);            CreateWall(-6F, -5F, 2F);   
            CreateWall(-1.5F, -5F, 1F);         CreateWall(6.5F, -5F, 1F);          CreateWall(2F, -4F, 2F);            CreateWall(-4.5F, -3F, 1F);
            CreateWall(-0.5F, -3F, 3F);         CreateWall(5F, -3F, 2F);            CreateWall(-5.5F, -2F, 1F);         CreateWall(-0.5F, -2F, 3F);
            CreateWall(5F, -2F, 2F);            CreateWall(-3.5F, -1F, 1F);         CreateWall(-1F, -1F, 2F);           CreateWall(3F, -1F, 2F);
            CreateWall(-4.5F, 0F, 1F);          CreateWall(-2F, 0F, 2F);            CreateWall(5F, 0F, 2F);             CreateWall(-0.5F, 1F, 1F);
            CreateWall(4.5F, 1F, 1F);           CreateWall(6.5F, 1F, 1F);           CreateWall(2.5F, 2F, 1F);           CreateWall(-5.5F, 3F, 1F);
            CreateWall(-3.5F, 3F, 1F);          CreateWall(-0.5F, 3F, 3F);          CreateWall(-6.5F, 4F, 1F);          CreateWall(0.5F, 4F, 3F);
            CreateWall(6.5F, 4F, 1F);           CreateWall(-5.5F, 5F, 1F);          CreateWall(-2.5F, 5F, 1F);          CreateWall(-0.5F, 5F, 1F);
            CreateWall(3.5F, 5F, 1F);           CreateWall(5.5F, 5F, 1F);           CreateWall(-5F, 6F, 4F);            CreateWall(-1.5F, 6F, 1F);
            CreateWall(0.5F, 6F, 1F);           CreateWall(5F, 6F, 2F);             CreateWall(-6F, 0.5F, 5F, true);    CreateWall(-6F, 4.5F, 1F, true);
            CreateWall(-5F, -4F, 2F, true);     CreateWall(-5F, -1F, 2F, true);     CreateWall(-5F, 4F, 2F, true);      CreateWall(-4F, -2F, 2F, true);
            CreateWall(-4F, 1.5F, 3F, true);    CreateWall(-3F, -3.5F, 5F, true);   CreateWall(-3F, 1.5F, 3F, true);    CreateWall(-3F, 5.5F, 1F, true);
            CreateWall(-2F, -4F, 2F, true);     CreateWall(-2F, -1.5F, 1F, true);   CreateWall(-2F, 4F, 2F, true);      CreateWall(-2F, 6.5F, 1F, true);
            CreateWall(-1F, -6F, 2F, true);     CreateWall(-1F, 0.5F, 1F, true);    CreateWall(-1F, 4.5F, 1F, true);    CreateWall(-1F, 6.5F, 1F, true);
            CreateWall(0F, -6.5F, 1F, true);    CreateWall(0F, 0F, 2F, true);       CreateWall(0F, 5.5F, 1F, true);     CreateWall(1F, -3.5F, 1F, true);
            CreateWall(1F, 0.5F, 5F, true);     CreateWall(1F, 6.5F, 1F, true);     CreateWall(2F, 0.5F, 3F, true);     CreateWall(2F, 5.5F, 3F, true);
            CreateWall(3F, -5F, 2F, true);      CreateWall(3F, 3.5F, 3F, true);     CreateWall(4F, -4.5F, 3F, true);    CreateWall(4F, -1.5F, 1F, true);
            CreateWall(4F, 0.5F, 1F, true);     CreateWall(4F, 5.5F, 1F, true);     CreateWall(5F, 3F, 4F, true);       CreateWall(6F, -6.5F, 1F, true);
            CreateWall(6F, -4F, 2F, true);      CreateWall(6F, -1F, 2F, true);      CreateWall(6F, 2.5F, 3F, true);     CreateWall(6F, 5.5F, 1F, true);

            base.Initialize();
        }

        // Creates a new rigid body and adds it to the simulation.
        private void AddBody(string name, Pose pose, Shape shape, MotionType motionType)
        {
            var rigidBody = new RigidBody(shape)
            {
                Name = name,
                Pose = pose,
                MotionType = motionType,
            };

            simulation.RigidBodies.Add(rigidBody);
        }

        public override void Draw(GameTime gameTime)
        {
            var camera = Game.Components.OfType<Camera>().First();

            //Actually draw the map and correct the roation.
            DrawModel(model, new Pose(new Vector3F(0, 0, 0), QuaternionF.CreateRotationX(MathHelper.ToRadians(-90))), camera.Pose.Inverse, camera.Projection);

            base.Draw(gameTime);
        }

        private void CreateWall(float x, float z, float w, bool isVertical = false, string name = "")
        {
            const float wallThickness = 0.01F;
            const float wallHeight = 2.5F;

            //Add collision map segment
            AddBody(name, new Pose(new Vector3F(x, -1.5F, z)),
                isVertical ? new BoxShape(wallThickness, wallHeight, w) : new BoxShape(w, wallHeight, wallThickness),
                MotionType.Static);
        }

        private void DrawModel(Model mdl, Pose pose, Matrix view, Matrix projection)
        {
            //Compute from local to world coordinates
            var transforms = new Matrix[mdl.Bones.Count];
            mdl.CopyAbsoluteBoneTransformsTo(transforms);

            //iterate thtrough the meshes and draw each one.
            foreach (var mesh in mdl.Meshes)
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
    }

}
