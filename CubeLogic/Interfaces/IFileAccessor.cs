using System.Collections.Generic;

namespace CubeLogic.Interfaces
{
    public interface IFileAccessor
    {
        List<string> Read(string filePath);
    }
}
