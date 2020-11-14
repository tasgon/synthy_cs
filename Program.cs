using System;
using System.IO;
using Melanchall.DryWetMidi.Devices;

namespace synthy_cs
{
    public static class Program
    {

        public static readonly string SynthyRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "synthy");

        [STAThread]
        static void Main()
        {
            Piano.Init();
            using (var game = new Game1())
                game.Run();
        }
    }
}
