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
        private SpriteFont _font;
        private Desktop _desktop;
        public DrawnPiano OnScreenPiano;
        private Song _currentSong = null;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (sender, args) =>
            {
                OnScreenPiano.Height = GraphicsDevice.Viewport.Height / 5;
            };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("MainFont");

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
            foreach (var file in Directory.GetFiles(Program.SynthyRoot, "*.mid*"))
            {
                var btn = new TextButton
                {
                    Text = file
                };
                btn.Click += (sender, args) =>
                {
                    this._currentSong = new Song(btn.Text, this);
                    this._currentSong.Start();
                };
                vstack.AddChild(btn);
            }
            _desktop = new Desktop();
            _desktop.Root = vstack;
            
            Textures.InitTextures(this);
            OnScreenPiano = new DrawnPiano(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _currentSong?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _currentSong?.Draw(this, _spriteBatch);
            OnScreenPiano.Draw(this, _spriteBatch);
            if (_currentSong != null)
            {
                var pos = new Vector2(GraphicsDevice.Viewport.Width - 100, 0);
                _spriteBatch.DrawString(_font, $"{(_currentSong.SongJudgement.Accuracy*100):F2}%", pos, Color.White);
            }
            _spriteBatch.End();
            
            _desktop.Render();

            base.Draw(gameTime);
        }
    }
}
