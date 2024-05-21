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
        public static void ProfileOptimization()
        {
            var primaryOrCallingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var assemblyName = primaryOrCallingAssembly.GetName().Name;
            if (string.IsNullOrEmpty(assemblyName))
            {
                return;
            }
            assemblyName = $"{assemblyName}.profile";

            // Define the potential profile locations
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string tempPath = System.IO.Path.GetTempPath();

            if (string.IsNullOrEmpty(baseDirectory) || string.IsNullOrEmpty(localAppData))
            {
                return;
            }

            string[] possibleLocations = {
                    Path.Combine(baseDirectory, assemblyName),
                    Path.Combine(localAppData, assemblyName),
                    Path.Combine(tempPath, assemblyName),
                };

            foreach (var profileLocation in possibleLocations)
            {
                try
                {
                    string? directory = Path.GetDirectoryName(profileLocation);
                    if (string.IsNullOrEmpty(directory))
                    {
                        continue;
                    }

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllBytes($"{profileLocation}.tmp", Array.Empty<byte>());
                    File.Delete($"{profileLocation}.tmp");

                    System.IO.FileInfo fileInfo = new FileInfo(profileLocation);
                    if (fileInfo.Exists)
                    {
                        if (DateTime.UtcNow - fileInfo.LastWriteTimeUtc > TimeSpan.FromDays(7))
                        {
                            System.Runtime.ProfileOptimization.SetProfileRoot(directory);
                            System.Runtime.ProfileOptimization.StartProfile(Path.GetFileName(profileLocation));
                        }
                    }
                    else
                    {
                        System.Runtime.ProfileOptimization.SetProfileRoot(directory);
                        System.Runtime.ProfileOptimization.StartProfile(Path.GetFileName(profileLocation));
                    }

                    return;
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}