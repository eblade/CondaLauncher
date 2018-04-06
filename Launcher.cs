using System;
using System.Diagnostics;
using System.IO;
using static System.Environment;
using System.Runtime.InteropServices;


namespace launcher
{
    public class Launcher {
        private string _condaEnv;
        private string _module;
        private string _condaPackages;
        private string _pipPackages;
        private bool _upgrade;
        private bool _debug;

        private string _appData;
        private string _condaExePath;
        private string _condaPath;
        private string _condaEnvPath;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public Launcher(string envName, string module, string condaPackages, string pipPackages, bool upgrade=false, bool debug=false) {
            _condaEnv = envName;
            _module = module;
            _condaPackages = condaPackages;
            _pipPackages = pipPackages;
            _appData = Environment.GetFolderPath(SpecialFolder.LocalApplicationData);
            _upgrade = upgrade;
            _debug = debug;
        }

        public bool Setup() {
            if (!FindConda(out _condaExePath)) {
                System.Console.Error.WriteLine("Missing Anaconda3 or Miniconda3. Please install!");
                return false;
            }
            _condaPath = Directory.GetParent(Path.GetDirectoryName(_condaExePath)).FullName;

            bool needPip = false || _upgrade;
            if (!FindEnv(_condaEnv, out _condaEnvPath)) {
                System.Console.WriteLine($"Missing Environment {_condaEnv}!");
                InstallEnv();
                needPip = true;
            }

            if (!FindEnv(_condaEnv, out _condaEnvPath)) {
                System.Console.Error.WriteLine("Failed to install the environment!");
                return false;
            }

            if (needPip) {
                InstallPip();
            }

            return true;
        }

        private bool FindConda(out string path) =>
            BuildCondaPath("miniconda3", out path) || BuildCondaPath("anaconda3", out path);

        private bool BuildCondaPath(string variant, out string path) {
            path = Path.Combine(_appData, "Continuum", variant, "Scripts", "conda.exe");
            return File.Exists(path);
        }

        private bool FindEnv(string name, out string path) {
            path = Path.Combine(_condaPath, "envs", name);
            return Directory.Exists(path);
        }

        private bool BuildEnvPath(string variant, out string path) {
            path = Path.Combine(_appData, "Continuum", variant, "Scripts", "conda.exe");
            return File.Exists(path);
        }


        public void InstallEnv() {
            var process = new Process();
            process.StartInfo.FileName = _condaExePath;
            Console.WriteLine(process.StartInfo.FileName);
            process.StartInfo.Arguments = $"create -n {_condaEnv} -y python=3.6 " + (_condaPackages ?? "");
            process.Start();
            process.WaitForExit();
        }

        public void InstallPip() {
            if (string.IsNullOrWhiteSpace(_pipPackages)) return;
            var process = new Process();
            process.StartInfo.FileName = Path.Combine(_condaEnvPath, "python.exe");
            Console.WriteLine(process.StartInfo.FileName);
            process.StartInfo.Arguments = $"-m pip install {(_upgrade ? "--upgrade " : "")}" + _pipPackages;
            process.Start();
            process.WaitForExit();
        }

        public void Launch() {
            var process = new Process();
            process.StartInfo.FileName = Path.Combine(_condaEnvPath, _debug ? "python.exe" : "pythonw.exe");
            Console.WriteLine(process.StartInfo.FileName);
            process.StartInfo.Arguments = $"-m {_module}";

            if (!_debug) {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
            }

            process.Start();
        }
    }
}

