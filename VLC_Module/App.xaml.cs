using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VLC_Module.Utils;

namespace VLC_Module
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var args = e.Args;
            Parser.Default
                .ParseArguments<Options.CommandLineOptions>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options.CommandLineOptions opts)
        {
            Console.WriteLine();
            MainWindow wnd = new MainWindow(opts);
            wnd.Title = "VLC";
            wnd.Show();
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (Error e in errs)
            {
                Console.WriteLine($"Error: {e}");
            }

        }
    }
}
