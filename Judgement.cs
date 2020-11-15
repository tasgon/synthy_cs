using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace synthy_cs
{
    public class Judgement
    {
        public int HitPerfect { get; private set; } = 0;
        public int HitOkay { get; private set; } = 0;
        public int HitBad { get; private set; } = 0;
        public int HitMiss { get; private set; } = 0;
        private Song _song;
        public Queue<Tuple<MidiEventType, long>>[] KeyEvents = new Queue<Tuple<MidiEventType, long>>[128];

        public double Accuracy
        {
            get
            {
                var denom = (HitPerfect + HitOkay + HitBad + HitMiss);
                if (denom == 0) return 0d;
                return (double) (3 * HitPerfect + 2 * HitOkay + 1 * HitBad)
                       / (double) denom;
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

            var ev = queue.Peek();
            var window = Math.Abs(ev.Item2 - _song.CurrentTime);
            Console.WriteLine(window);
            if (window < Settings.HitPerfectMicros) HitPerfect++;
            else if (window < Settings.HitOkayMicros) HitOkay++;
            else if (window < Settings.HitBadMicros) HitBad++;
            else if (ev.Item1 == MidiEventType.NoteOn)
            {
                HitMiss++;
            }

            Console.WriteLine($"PF: {HitPerfect}; OK: {HitOkay}; BD: {HitBad}; MS: {HitMiss}");
        }

        public void Update()
        {
            foreach (var queue in KeyEvents)
            {
                if (queue.Count == 0) continue;
                var item = queue.Peek();
                if (item.Item2 < _song.CurrentTime + Settings.HitBadMicros)
                {
                    if (item.Item1 == MidiEventType.NoteOn)
                    {
                        HitMiss += 2;
                        queue.Dequeue();
                        queue.Dequeue();
                    }
                    else
                    {
                        HitBad += 1;
                        queue.Dequeue();
                    }
                }
            }
        }
    }
}