using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace RekenApplicatie.Utilities
{
    public class ResourceManager
    {
        private static MediaPlayer mediaPlayer = new MediaPlayer();
        private string imagesFolder;
        private string soundsFolder;

        public ResourceManager()
        {
            // Set up the folders path - use execution directory for built app
            // This fixes the path to look in the output directory where files are copied
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            imagesFolder = Path.Combine(baseDir, "Images");
            soundsFolder = Path.Combine(baseDir, "Sounds");

            Debug.WriteLine($"Images folder path: {imagesFolder}");
            Debug.WriteLine($"Sounds folder path: {soundsFolder}");

            // Create the directories if they don't exist
            EnsureDirectoryExists(imagesFolder);
            EnsureDirectoryExists(soundsFolder);
        }

        // Ensure a directory exists
        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                    Debug.WriteLine($"Created directory: {path}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to create directory {path}: {ex.Message}");
                }
            }
        }

        // Load an image from file
        public BitmapImage LoadImage(string imageName)
        {
            try
            {
                // First, try to load from the output directory
                string imagePath = Path.Combine(imagesFolder, imageName);
                Debug.WriteLine($"Trying to load image from: {imagePath}");
                Debug.WriteLine($"File exists: {File.Exists(imagePath)}");

                if (File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }

                // If not found, try to load from development path
                string devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images", imageName);
                Debug.WriteLine($"Trying development path: {devPath}");
                Debug.WriteLine($"File exists: {File.Exists(devPath)}");

                if (File.Exists(devPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(devPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }

                Debug.WriteLine($"Image {imageName} not found in any location");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

        // Play a sound file
        public void PlaySound(string soundFileName)
        {
            try
            {
                // First, try to load from the output directory
                string soundFilePath = Path.Combine(soundsFolder, soundFileName);
                Debug.WriteLine($"Trying to play sound from: {soundFilePath}");
                Debug.WriteLine($"File exists: {File.Exists(soundFilePath)}");

                if (File.Exists(soundFilePath))
                {
                    mediaPlayer.Open(new Uri(soundFilePath, UriKind.Absolute));
                    mediaPlayer.Play();
                    Debug.WriteLine("Sound playback initiated");
                    return;
                }

                // If not found, try to load from development path
                string devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Sounds", soundFileName);
                Debug.WriteLine($"Trying development path: {devPath}");
                Debug.WriteLine($"File exists: {File.Exists(devPath)}");

                if (File.Exists(devPath))
                {
                    mediaPlayer.Open(new Uri(devPath, UriKind.Absolute));
                    mediaPlayer.Play();
                    Debug.WriteLine("Sound playback initiated from dev path");
                    return;
                }

                Debug.WriteLine($"Sound {soundFileName} not found in any location");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
}