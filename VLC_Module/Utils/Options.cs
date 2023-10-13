using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLC_Module.Utils
{
    public static class Options
    {
        public class CommandLineOptions
        {
            [Option(shortName: 'p', longName: "path", Required = true, HelpText = "Source Path")]
            public string Path { get; set; }

            [Option(shortName: 'r', longName: "repeat", Required = false, HelpText = "Loop", Default = false)]
            public bool? Loop { get; set; }

            [Option(shortName: 'l', longName: "left", Required = false, HelpText = "Window Left", Default = 0)]
            public int Left { get; set; }

            [Option(shortName: 't', longName: "top", Required = false, HelpText = "Window Top", Default = 0)]
            public int Top { get; set; }

            [Option(shortName: 'w', longName: "width", Required = false, HelpText = "Window Witdh", Default = 960)]
            public int Width { get; set; }

            [Option(shortName: 'h', longName: "height", Required = false, HelpText = "Window Height", Default = 540)]
            public int Height { get; set; }

            [Option(shortName: 'v', longName: "volume", Required = false, HelpText = "Volume", Default = 100)]
            public int Volume { get; set; }

            [Option(shortName: 'c', longName: "crop", Required = false, HelpText = "Crop", Default = null)]
            public string Crop { get; set; }

            [Option(shortName: 'e', longName: "extra", Required = false, HelpText = "Extra Paramaters", Default = "")]
            public string ExtraParameters { get; set; }

            [Option(longName: "rotate", Required = false, HelpText = "Rotate Degree", Default = null)]
            public string Rotate { get; set; }

            [Option(longName: "angle", Required = false, HelpText = "Angle", Default = null)]
            public string Angle { get; set; }
        }
    }
}
