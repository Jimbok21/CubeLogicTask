using CubeLogic.Models;
using System.Collections.Generic;

namespace CubeLogic.Interfaces
{
    public interface IProgram
    {
        List<Transaction> ReadTransactions(List<string> lines);
        List<OutputTransaction> TransformTransactions(List<Transaction> transactions, IConfig config);
        void WriteOutputTransactions(List<OutputTransaction> transactions, string filePath);
    }
}
