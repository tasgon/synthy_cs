using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace synthy_cs
{
    /*public enum KeyColor
    {
        White,
        Black
    }*/
    
    public class DrawnPiano
    {
        public static readonly string KEYS =
            "WBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWW"; 
        
        //private SpriteBatch BasePiano;
        public RenderTarget2D WhiteKeys, BlackKeys;
        public List<Tuple<Texture2D, Vector2>> KeyPositions = new List<Tuple<Texture2D, Vector2>>();
        public List<int> WhiteIndices = new List<int>(), BlackIndices = new List<int>();
        public int Height = 100;

        public DrawnPiano(Game1 game)
        {
            var g = game.GraphicsDevice;
            var batch = new SpriteBatch(g);
            WhiteKeys = new RenderTarget2D(g, Textures.NoteWhite.Width*52, Textures.NoteWhite.Height);
            var dh = Textures.NoteBlack.Width / 2;
            var pos = new Vector2(0f, 0f);
            var blackKeyPositions = new List<Vector2>();
            g.SetRenderTarget(WhiteKeys);
            batch.Begin();
            for (int i = 0; i < 88; i++)
            {
                var k = KEYS[i];
                if (k == 'W')
                {
                    KeyPositions.Add(new Tuple<Texture2D, Vector2>(Textures.NoteWhiteOn, pos));
                    batch.Draw(Textures.NoteWhite, pos, Color.White);
                    pos.X += Textures.NoteWhite.Width;
                    WhiteIndices.Add(i);
                }
                else
                {
                    pos.X -= dh;
                    blackKeyPositions.Add(pos);
                    KeyPositions.Add(new Tuple<Texture2D, Vector2>(Textures.NoteBlackOn, pos));
                    pos.X += dh;
                    BlackIndices.Add(i);
                }
            }
            batch.End();
            BlackKeys = new RenderTarget2D(g, Textures.NoteWhite.Width * 52, Textures.NoteWhite.Height);
            g.SetRenderTarget(BlackKeys);
            batch.Begin();
            foreach (var p in blackKeyPositions) batch.Draw(Textures.NoteBlack, p, Color.White);
            batch.End();
            g.SetRenderTarget(null);
        }

        public void Draw(Game1 game, SpriteBatch sb)
        {
            float scaleX = game.GraphicsDevice.Viewport.Width / (KeyPositions.Last().Item2.X + Textures.NoteWhite.Width);
            float scaleY = (float)Height / Textures.NoteWhite.Height;
            foreach (int i in WhiteIndices)
            {
                var x = (int)(KeyPositions[i].Item2.X * scaleX);
                Texture2D tex;
                if (Piano.PressedKeys[i]) tex = Textures.NoteWhiteOn;
                else tex = Textures.NoteWhite;
                var rect = new Rectangle(x, game.GraphicsDevice.Viewport.Height - Height,
                    (int) (tex.Width * scaleX), (int) (tex.Height * scaleY));
                sb.Draw(tex, rect, Color.White);
            }
            foreach (int i in BlackIndices)
            {
                var x = (int)(KeyPositions[i].Item2.X * scaleX);
                Texture2D tex;
                if (Piano.PressedKeys[i]) tex = Textures.NoteBlackOn;
                else tex = Textures.NoteBlack;
                var rect = new Rectangle(x, game.GraphicsDevice.Viewport.Height - Height,
                    (int) (tex.Width * scaleX), (int) (tex.Height * scaleY));
                sb.Draw(tex, rect, Color.White);
            }
        }
    }
}