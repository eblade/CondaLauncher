using System;
using System.Diagnostics;
using System.IO;
using static System.Environment;
using System.Linq;
using System.Collections.Generic;

namespace launcher
{
    class Program {
        static void Main(string[] args) {
            var config = new Configuration("launcher.ini");
            if (!config.IsOk) Exit(-1);

            var upgrade = args.Contains("--upgrade");
            var debug = args.Contains("--debug");
            var envName = config["env"];
            var module = config["module"];
            var condaDeps = config["dependencies:conda"];
            var pipDeps = config["dependencies:pip"];

            Check("env", envName);
            Check("module", module);

            var launcher = new Launcher(envName, module, condaDeps, pipDeps, upgrade, debug);
            if (!launcher.Setup()) {
                Exit(-1);
            }
            launcher.Launch();
        }

        static void Check(string param, string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                Console.Error.WriteLine($"Required in launcher.ini: {param}");
                Exit(-1);
            }
        }
    }

    class Dependencies {
        public string[] Conda { get; set; }
        public string[] Pip { get; set; }
    }

    class Configuration {
        public bool IsOk { get; } 
        IDictionary<string, string> _values = new Dictionary<string, string>();

        public Configuration(string path = "launcher.ini") {
            try {
                using (var f = File.OpenText(path)) {
                    string line;
                    while ((line = f.ReadLine()) != null) {
                        var parts = line.Split(new char[] {'='}, 2);
                        if (parts.Count() != 2) {
                            Console.Error.WriteLine("Bad config line: " + line);
                            continue;
                        }
                        _values[parts[0].Trim()] = parts[1].Trim();
                    }
                }
                IsOk = true;
            } catch (IOException e) {
                Console.Error.WriteLine($"Missing launcher.ini ({e.Message})");
                IsOk = false;
            }
        }

        public string this[string key] {
            get {
                if (_values.TryGetValue(key, out var value)) {
                    return value;
                } else {
                    return null;
                }
            }
        }
    }
}
