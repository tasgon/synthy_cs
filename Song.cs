using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Xna.Framework;

namespace synthy_cs
{
    class Song
    {
        public readonly MidiFile File;
        public List<Note> AllNotes { get; private set; } = new List<Note>();
        public Queue<Note> AllNotesQueue { get; private set; };
        public Queue<Note> ActiveNotes { get; private set; } = new Queue<Note>();
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
                while (AllNotesQueue.Peek().TimeAs<MetricTimeSpan>(File.GetTempoMap()).TotalMicroseconds < CurrentTime)
                {
                    ActiveNotes.Enqueue(AllNotesQueue.Dequeue());
                }
            }
            
            if (ActiveNotes.Count == 0) return;
            while (ActiveNotes.Peek().EndTimeAs<MetricTimeSpan>(File.GetTempoMap()).TotalMicroseconds < CurrentTime)
            {
                ActiveNotes.Dequeue();
            }
        }
    }
}
