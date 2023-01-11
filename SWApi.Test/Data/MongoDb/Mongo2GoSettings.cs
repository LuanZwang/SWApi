namespace SWApi.Test.Data.MongoDb
{
    public static class Mongo2GoSettings
    {
        private static string _binariesSearchPath;

        private static string Mongo2GoFileVersion =>
            System.Diagnostics.FileVersionInfo
                .GetVersionInfo("Mongo2Go.dll")
                .FileVersion.Replace(".0", "");

        public static string GetBinariesSearchDirectory()
        {
            if (_binariesSearchPath != null) return _binariesSearchPath;

            _binariesSearchPath = FindBinariesSearchDirectory();
            return _binariesSearchPath;
        }

        private static string FindBinariesSearchDirectory()
        {
            var home = Environment.GetEnvironmentVariable("HOMEPATH");
            var version = Mongo2GoFileVersion;

            var libraryPath = $@"{home}\.nuget\packages\mongo2go\{version}";
            if (Directory.Exists(libraryPath)) return libraryPath;

            libraryPath = $@"{home}\NugetPackage\mongo2go\{version}";
            if (Directory.Exists(libraryPath)) return libraryPath;

            libraryPath = @$"D:\a\1\s\NugetPackage\mongo2go\{version}";
            if (!Directory.Exists(libraryPath))
                throw new DirectoryNotFoundException("Mongo2Go Binaries Search Directory");

            return libraryPath;
        }
    }
}