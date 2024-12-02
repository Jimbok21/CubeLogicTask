using CubeLogic.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace CubeLogic.FileAccessor
{
    public class FileAccessor : IFileAccessor
    {
        // Take a file path and put its data into lines
        public List<string> Read(string filePath)
        {
            try
            {
                return File.ReadAllLines(filePath).ToList();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
