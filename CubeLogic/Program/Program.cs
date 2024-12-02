﻿﻿﻿using CubeLogic.Models;
using CubeLogic.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CubeLogic.FileAccessor;

namespace CubeLogic
{
    public class Program : IProgram
    {
        private readonly IConfig _config;
        private readonly IFileAccessor _fileAccessor;

        // Program constructor
        public Program(IConfig config, IFileAccessor fileAccessor)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fileAccessor = fileAccessor ?? throw new ArgumentNullException(nameof(fileAccessor));
        }

        public static void Main(string[] args)
        {
            try
            {
                // Setup the program
                string configJson = File.ReadAllText(@"../config.json");
                Config config = JsonConvert.DeserializeObject<Config>(configJson) ?? throw new JsonException("Failed to deserialize config.json"); 
                IFileAccessor fileAccessor = new CubeLogic.FileAccessor.FileAccessor(); 
                IProgram program = new Program(config, fileAccessor);
                // Get the path of the input and read its data
                List<string> lines = fileAccessor.Read(@"../InputTransactions.csv");
                List<Transaction> transactions = program.ReadTransactions(lines);

                // Manipulate the data
                List<OutputTransaction> outputTransactions = program.TransformTransactions(transactions, config);

                // Output the data
                program.WriteOutputTransactions(outputTransactions, @"../output.csv");

                Console.WriteLine("Processing complete. Output written to output.csv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Please check the syntax of the config and InputTransactions files");
            }
        }

        // Puts the data into Transactions
        public List<Transaction> ReadTransactions(List<string> lines)
        {
            try
            {
                return lines
                    .Skip(1)
                    .Select((line, index) =>
                    {
                        var parts = line.Split(',');
                        return new Transaction
                        {
                            Type = parts[0],
                            OrderId = int.Parse(parts[1]),
                            InstrumentId = int.Parse(parts[2]),
                            DateTime = DateTime.ParseExact(parts[3], "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture),
                            Price = decimal.Parse(parts[4], CultureInfo.InvariantCulture)
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading transactions: {ex.Message}");
                return new List<Transaction>();
            }
        }

        // Manipulates the transactions
        public List<OutputTransaction> TransformTransactions(List<Transaction> transactions, IConfig config)
        {
            if (config == null || config.Instruments == null || config.Instruments.Count == 0)
            {
                Console.WriteLine("Error: Invalid config file. Instruments list is null or empty.");
                return new List<OutputTransaction>();
            }

            var outputTransactions = new List<OutputTransaction>();
            var orderTransactionCounts = new Dictionary<int, int>();

            foreach (var transaction in transactions)
            {
                try
                {
                    //Get it in the correct Syntax
                    string type = transaction.Type.Replace("Order", "").ToUpper();
                    var instrument = config.Instruments.FirstOrDefault(i => i.InstrumentId == transaction.InstrumentId);
                    string country = instrument?.Country ?? "Error";
                    string instrumentName = instrument?.InstrumentName ?? "Error"; 

                    //Update Revisions
                    if (!orderTransactionCounts.ContainsKey(transaction.OrderId))
                    {
                        orderTransactionCounts[transaction.OrderId] = 1;
                    }

                    // Adjust the DateTime
                    TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config.Timezone);
                    DateTime transactionDate = transaction.DateTime;
                    DateTime adjustedDateTime = TimeZoneInfo.ConvertTimeToUtc(transactionDate, targetTimeZone);


                    if (!outputTransactions.Any(t =>
                        t.OrderId == transaction.OrderId &&
                        t.Type == type &&
                        t.Price == transaction.Price &&
                        t.Country == country &&
                        t.InstrumentName == instrumentName))
                    {
                        // If the transaction does not already exist
                        if (type == "UPDATE")
                        {
                            // If its Update then increment the Revision
                            orderTransactionCounts[transaction.OrderId]++;
                        }
                        // Add to the output list
                        outputTransactions.Add(new OutputTransaction
                        {
                            OrderId = transaction.OrderId,
                            Type = type,
                            Revision = orderTransactionCounts[transaction.OrderId],
                            DateTimeUtc = adjustedDateTime.ToUniversalTime(),
                            Price = transaction.Price,
                            Country = country,
                            InstrumentName = instrumentName
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing transaction: {ex.Message}");
                }
            }
            return outputTransactions;
        }

        // Write the output data to the output csv file
        public void WriteOutputTransactions(List<OutputTransaction> transactions, string filePath)
        {
            try
            {
                // Get data back in lines and in the correct Syntax
                string header = "OrderId,Type,Revision,DateTimeUtc,Price,Country,InstrumentName";
                string[] lines = transactions.Select(t => $"{t.OrderId},{t.Type},{t.Revision},{t.DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture)},{t.Price},{t.Country},{t.InstrumentName}").ToArray();
                // Write lines to the csv file
                File.WriteAllLines(filePath, new[] { header }.Concat(lines));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
    }
}
