using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace _3DCameraV2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private Vector3 _camTarget;
        private Vector3 _camPosition;
        private float _camAngle;
        private Matrix _projectionMatrix;
        private Matrix _viewMatrix;

        private List<Matrix> _modelWorldMatrixes = new(10);

        private Model _model;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();

            //Setup Camera
            _camAngle = 0f;
            _camPosition = new Vector3(0f, 0f, -5);
            _camTarget = _camPosition + Matrix.CreateFromYawPitchRoll(_camAngle, 0, 0).Forward * 10;
            _viewMatrix = Matrix.CreateLookAt(_camPosition, _camTarget, Vector3.Up);

            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f), _graphics.
                    GraphicsDevice.Viewport.AspectRatio,
                1f, 1000f);
            
            // Position all the boxes in 3D space, with random rotations
            for (int i = 0; i < 10; i++)
            {
                var worldMatrix = Matrix.CreateWorld(_camTarget, Vector3.
                    Forward, Vector3.Up);

                worldMatrix *= Matrix.CreateTranslation(i * 5, 0, 0);
                //worldMatrix *= Matrix.CreateFromYawPitchRoll(i, i, i);

                _modelWorldMatrixes.Add(worldMatrix);
            }

            _model = Content.Load<Model>("MonoCube");

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                    Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _camAngle -= 0.01f;
                if (_camAngle < 0)
                    _camAngle = (float)MathHelper.TwoPi;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _camAngle += 0.01f;
                if (_camAngle > MathHelper.TwoPi)
                    _camAngle = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                var unit = Matrix.CreateFromYawPitchRoll(_camAngle, 0, 0).Forward;

                _camPosition += unit / 10;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                var unit = Matrix.CreateFromYawPitchRoll(_camAngle, 0, 0).Backward;

                _camPosition += unit / 10;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                _camPosition += _viewMatrix.Forward;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                _camPosition += _viewMatrix.Backward;
            }

            _camTarget = _camPosition + Matrix.CreateFromYawPitchRoll(_camAngle, 0, 0).Forward * 10;
            _viewMatrix = Matrix.CreateLookAt(_camPosition, _camTarget, Vector3.Up);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var modelWorldMatrix in _modelWorldMatrixes)
            {
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(1f, 0, 0);
                        effect.View = _viewMatrix;
                        effect.World = modelWorldMatrix;
                        effect.Projection = _projectionMatrix;
                    }

                    mesh.Draw();
                }
            }

            base.Draw(gameTime);
        }
    }
}