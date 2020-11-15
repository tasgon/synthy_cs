using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace synthy_cs
{
    class Song
    {
        public readonly MidiFile File;
        public List<Note> AllNotes { get; private set; } = new List<Note>();
        public Queue<Note> AllNotesQueue { get; private set; }
        public Queue<Note> OnScreenNotes { get; private set; } = new Queue<Note>();
        private long CurrentTime = 0;
        public Song(string path, Game1 game)
        {
            File = MidiFile.Read(path);
            AllNotes = File.GetNotes().OrderBy(i => i.Time).ToList();
            AllNotesQueue = new Queue<Note>(AllNotes);
        }

        public void Update(GameTime gameTime)
        {
            CurrentTime += (int)(gameTime.ElapsedGameTime.TotalMilliseconds * 1000);
            if (AllNotesQueue.Count > 0)
            {
                while (AllNotesQueue.Peek().TimeAs<MetricTimeSpan>(File.GetTempoMap()).TotalMicroseconds
                       < (CurrentTime + Settings.TimeWindowMillis * 1000))
                {
                    OnScreenNotes.Enqueue(AllNotesQueue.Dequeue());
                }
            }
            
            if (OnScreenNotes.Count == 0) return;
            while (OnScreenNotes.Peek().EndTimeAs<MetricTimeSpan>(File.GetTempoMap()).TotalMicroseconds < CurrentTime)
            {
                OnScreenNotes.Dequeue();
            }
        }

        public void Draw(Game1 game, SpriteBatch sb)
        {
            
        }
    }

    static class NoteExt
    {
        public static int VerticalPos(this Note note)
        {
            return 0;
        }

        public static int Height(this Note note)
        {
            
        }
    }
}
