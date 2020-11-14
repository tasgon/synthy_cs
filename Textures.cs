using System;
using System.IO;
using System.Net.Mime;
using Microsoft.Xna.Framework.Graphics;

namespace synthy_cs
{
    public class Textures
    {
        public static Texture2D NoteBlack, NoteWhite, NoteBlackOn, NoteWhiteOn;

        public static void InitTextures(Game1 game)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            NoteBlack = game.Content.Load<Texture2D>("note_black");
            NoteWhite = game.Content.Load<Texture2D>("note_white");
            NoteBlackOn = game.Content.Load<Texture2D>("note_black_on");
            NoteWhiteOn = game.Content.Load<Texture2D>("note_white_on");
        }
    }
}