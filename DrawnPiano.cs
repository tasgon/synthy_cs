using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace synthy_cs
{
    public class DrawnPiano
    {
        public static readonly string KEYS =
            "WBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWWBWBWWBWBWBWW"; 
        
        //private SpriteBatch BasePiano;
        public RenderTarget2D BasePiano;

        public DrawnPiano(Game1 game)
        {
            var g = game.GraphicsDevice;
            var batch = new SpriteBatch(g);
            BasePiano = new RenderTarget2D(g, Textures.NoteWhite.Width*52, Textures.NoteWhite.Height);
            var dh = Textures.NoteBlack.Width / 2;
            var pos = new Vector2(0f, 0f);
            g.SetRenderTarget(BasePiano);
            batch.Begin();
            foreach (char k in KEYS)
            {
                if (k == 'W')
                {
                    batch.Draw(Textures.NoteWhite, pos, Color.White);
                    pos.X += Textures.NoteWhite.Width;
                }
                else
                {
                    pos.X -= dh;
                    batch.Draw(Textures.NoteBlack, pos, Color.White);
                    pos.X += dh;
                }
            }
            batch.End();
            g.SetRenderTarget(null);
        }
    }
}