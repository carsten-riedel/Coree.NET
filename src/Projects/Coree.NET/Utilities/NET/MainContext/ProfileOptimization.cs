using System.Reflection;

namespace Coree.NET.Utilities
{
    /// <summary>
    /// Provides utility methods for managing the application's execution context, typically used within the Program.Main entry point.
    /// </summary>
    public static partial class MainContext
    {
        /// <summary>
        /// Enhances startup performance by enabling profile optimization.
        /// </summary>
        /// <remarks>
        /// Call this method early in the Program.Main to optimize startup on subsequent runs.
        /// </remarks>
        public static bool ProfileOptimization()
        {
            // Define the potential profile locations
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string tempPath = System.IO.Path.GetTempPath();


            var directory = Coree.NETStandard.Utilities.MainContext.TryGetOrCreateAppNameDirectory();
            var appName = Coree.NETStandard.Utilities.MainContext.TryGetAppName();
            if (appName != null && directory != null)
            {
                var filename = $"{appName}.profile";
                try
                {
                    System.Runtime.ProfileOptimization.SetProfileRoot(directory.FullName);
                    System.Runtime.ProfileOptimization.StartProfile(filename);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

    }
}