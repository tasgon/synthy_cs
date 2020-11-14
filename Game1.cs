using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.IO;

namespace synthy_cs
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Desktop _desktop;
        private DrawnPiano _drawnPiano;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content 
            MyraEnvironment.Game = this;
            var vstack = new VerticalStackPanel();
            vstack.AddChild(new Label
            {
                Id = "hello",
                Text = "This is synthy."
            });
            var kbdBtn = new TextButton
            {
                Text = $"Keyboard ({Piano.Devices.Count}): {Piano.SelectedDevice?.Name}"
            };
            kbdBtn.Click += (s, a) =>
            {
                Piano.Rotate();
                kbdBtn.Text = $"Keyboard: {Piano.SelectedDevice?.Name}";
            };
            vstack.AddChild(kbdBtn);
            vstack.AddChild(new Label
            {
                Id = "Songs",
                Text = "Songs:"
            });
            foreach (var file in Directory.GetFiles(Program.SynthyRoot))
            {
                var btn = new TextButton
                {
                    Text = file
                };
                vstack.AddChild(btn);
            }
            _desktop = new Desktop();
            _desktop.Root = vstack;
            
            Textures.InitTextures(this);
            _drawnPiano = new DrawnPiano(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _desktop.Render();
            _spriteBatch.Begin();
            //_spriteBatch.Draw(_drawnPiano.BasePiano, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            _drawnPiano.Draw(this, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
