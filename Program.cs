using System;
using Melanchall.DryWetMidi.Devices;

namespace synthy_cs
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("printing devices");
            foreach (var dev in InputDevice.GetAll())
            {
                Console.WriteLine(dev.Name);
            }
            using (var game = new Game1())
                game.Run();
        }
    }
}
