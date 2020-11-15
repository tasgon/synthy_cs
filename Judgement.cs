using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace synthy_cs
{
    public struct HitMarker
    {
        public int Error;
        public long Time;
    }
    public class Judgement
    {
        public int HitPerfect { get; private set; } = 0;
        public int HitOkay { get; private set; } = 0;
        public int HitBad { get; private set; } = 0;
        public int HitMiss { get; private set; } = 0;
        private Song _song;
        public Queue<Tuple<MidiEventType, long>>[] KeyEvents = new Queue<Tuple<MidiEventType, long>>[128];
        private bool[] PreviousNotHit = new bool[128];
        public Queue<HitMarker> HitMarkers = new Queue<HitMarker>();

        public double Accuracy
        {
            get
            {
                var denominator = (HitPerfect + HitOkay + HitBad + HitMiss);
                if (denominator == 0) return 0d;
                return (double) (3 * HitPerfect + 2 * HitOkay + 1 * HitBad)
                       / (double) denominator;
            }
        }

        public Judgement(Song song)
        {
            _song = song;
            for (int i = 0; i < 128; i++) KeyEvents[i] = new Queue<Tuple<MidiEventType, long>>();
            foreach (var note in song.AllNotes)
            {
                var list = KeyEvents[note.NoteNumber];
                var startTime = note.TimeAs<MetricTimeSpan>(song.SongTempoMap).TotalMicroseconds;
                var endTime = note.EndTimeAs<MetricTimeSpan>(song.SongTempoMap).TotalMicroseconds;
                list.Enqueue(new Tuple<MidiEventType, long>(MidiEventType.NoteOn, startTime));
                list.Enqueue(new Tuple<MidiEventType, long>(MidiEventType.NoteOff, endTime));
            }
        }

        public void RecordNoteEvent(NoteEvent e)
        {
            var queue = KeyEvents[e.NoteNumber];
            if (queue.Count == 0)
            {
                HitMiss += 1;
                return;
            }

            var (eventType, eventTime) = queue.Peek();
            if (eventType == e.EventType)
            {
                var error = _song.CurrentTime - eventTime;
                // Ignore way early events
                if (error < -HitBad * 2) return;
                var window = Math.Abs(error);
                if (window < Settings.HitBadMicros)
                {
                    var marker = new HitMarker { Error = (int) error, Time = _song.CurrentTime };
                    HitMarkers.Enqueue(marker);
                    if (window < Settings.HitPerfectMicros) HitPerfect++;
                    else if (window < Settings.HitOkayMicros) HitOkay++;
                    else HitBad++;
                }
                else HitMiss++;
            }
        }

        public void Update()
        {
            //foreach (var queue in KeyEvents)
            for (int i = 0; i < 128; i++)
            {
                var queue = KeyEvents[i];
                if (queue.Count == 0) continue;
                var (eventType, eventTime) = queue.Peek();
                if (eventTime + Settings.HitBadMicros >= _song.CurrentTime) continue;
                if (eventType == MidiEventType.NoteOn)
                {
                    HitMiss++;
                    queue.Dequeue();
                    PreviousNotHit[i] = true;
                    //queue.Dequeue();
                }
                else
                {
                    if (PreviousNotHit[i]) HitMiss++;
                    else HitBad++;
                    queue.Dequeue();
                }
            }

            if (HitMarkers.Count == 0) return;
            // Remove old hitmarkers
            while (HitMarkers.Peek().Time < _song.CurrentTime - 1e6) HitMarkers.Dequeue();
        }

        public void Draw(Game1 game, SpriteBatch sb)
        {
            var startX = (game.GraphicsDevice.Viewport.Width - Settings.HitMarkerBaseWidth) / 2;
            var pixelsPerTime = Settings.HitMarkerBaseWidth / (Settings.HitBadMicros * 2);
            var baseRect = new Rectangle(startX, game.GraphicsDevice.Viewport.Height / 2, 
                Settings.HitMarkerBaseWidth, 2);
            sb.Draw(Textures.HitMarkerBase, baseRect, Color.White);
            var markerY = (game.GraphicsDevice.Viewport.Height - Textures.HitMarker.Height) / 2; 
            var hitMarkerRect = new Rectangle(0, markerY, 3, 20);
            foreach (var hitMarker in HitMarkers)
            {
                hitMarkerRect.X = startX + (hitMarker.Error - Settings.HitBadMicros) * pixelsPerTime;
                sb.Draw(Textures.HitMarker, hitMarkerRect, Color.White);
            }
        }
    }
}