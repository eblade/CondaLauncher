using System;
using System.Diagnostics;
using System.IO;
using static System.Environment;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace launcher
{
    class Program {
        static void Main(string[] args) {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("launcher.json")
                .Build();
            
            var upgrade = args.Contains("--upgrade");
            var envName = config["env"];
            var module = config["module"];
            var condaDeps = config.GetSection("dependencies:conda").GetChildren().Select(x => x.Value).ToArray();
            var pipDeps = config.GetSection("dependencies:pip").GetChildren().Select(x => x.Value).ToArray();

            var launcher = new Launcher(envName, module, condaDeps, pipDeps, upgrade);
            if (!launcher.Setup()) {
                Exit(-1);
            }
            launcher.Launch();
        }
    }

    class Dependencies {
        public string[] Conda { get; set; }
        public string[] Pip { get; set; }
    }
}
