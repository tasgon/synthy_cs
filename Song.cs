using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace synthy_cs
{
    public class Song
    {
        public readonly MidiFile File;
        public List<Note> AllNotes { get; private set; }
        public Queue<Note> AllNotesQueue { get; private set; }
        public Queue<Note> OnScreenNotes { get; private set; } = new Queue<Note>();
        public long CurrentTime = 0;
        public DateTime StartTime { get; private set; }
        public readonly TempoMap SongTempoMap;
        public Judgement SongJudgement = null;

        public Song(string path, Game1 game)
        {
            File = MidiFile.Read(path);
            AllNotes = File.GetNotes().OrderBy(i => i.Time).ToList();
            SongTempoMap = File.GetTempoMap();
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            CurrentTime = -Settings.TimeWindowMillis * 1000;
            AllNotesQueue = new Queue<Note>(AllNotes);
            OnScreenNotes.Clear();
            SongJudgement = new Judgement(this);
            Piano.CurrentSong = this;
        }

        public void Update(GameTime gameTime)
        {
            CurrentTime += (int)(gameTime.ElapsedGameTime.TotalMilliseconds * 1000);
            if (AllNotesQueue.Count > 0)
            {
                while (AllNotesQueue.Peek().TimeAs<MetricTimeSpan>(SongTempoMap).TotalMicroseconds
                       < (CurrentTime + Settings.TimeWindowMillis * 1000))
                {
                    var note = AllNotesQueue.Dequeue();
                    OnScreenNotes.Enqueue(note);
                }
            }
            
            if (OnScreenNotes.Count == 0) return;
            while (OnScreenNotes.Peek().EndTimeAs<MetricTimeSpan>(SongTempoMap).TotalMicroseconds < CurrentTime)
            {
                var note = OnScreenNotes.Dequeue();
            }
        }

        public void Draw(Game1 game, SpriteBatch sb)
        {
            float scaleX = game.GraphicsDevice.Viewport.Width 
                           / (game.OnScreenPiano.KeyPositions.Last().Item2.X + Textures.NoteWhite.Width);
            foreach (var note in OnScreenNotes)
            {
                var idx = note.NoteNumber - 21;
                var tex = game.OnScreenPiano.KeyPositions[idx].Item1;
                var rect = new Rectangle((int) (game.OnScreenPiano.KeyPositions[idx].Item2.X * scaleX),
                    note.VerticalPos(this, game), (int) (tex.Width * scaleX), note.Height(this, game));
                Console.WriteLine(rect);
                sb.Draw(tex, rect, Color.White);
            }
        }
    }

    static class NoteExt
    {
        public static int VerticalPos(this Note note, Song song, Game1 game)
        {
            var timePerPixel = Settings.TimeWindowMillis * 1000
                               / (game.GraphicsDevice.Viewport.Height - game.OnScreenPiano.Height);
            var time = - (int)(note.EndTimeAs<MetricTimeSpan>(song.SongTempoMap).TotalMicroseconds
                             - song.CurrentTime);
            return time / timePerPixel + (game.GraphicsDevice.Viewport.Height - game.OnScreenPiano.Height);
        }

        public static int Height(this Note note, Song song, Game1 game)
        {
            var timePerPixel = Settings.TimeWindowMillis * 1000
                               / (game.GraphicsDevice.Viewport.Height - game.OnScreenPiano.Height);
            var time = (int)(note.LengthAs<MetricTimeSpan>(song.SongTempoMap).TotalMicroseconds);
            return time / timePerPixel;
        }
    }
}
