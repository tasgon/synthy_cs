using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace synthy_cs
{
    class Song
    {
        public readonly MidiFile File;
        public Song(string path)
        {
            File = MidiFile.Read(path);
        }
    }
}
