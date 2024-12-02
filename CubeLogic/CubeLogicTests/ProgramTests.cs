using CubeLogic;
using CubeLogic.Interfaces;
using CubeLogic.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace CubeLogicTests
{
    public class ProgramTests
    {
        [Fact]
        public void Test_ReadTransactions_GivenExample()
        {
            
            var mockConfig = new Mock<IConfig>();
            mockConfig.Setup(c => c.Timezone).Returns("Europe/Malta");
            mockConfig.Setup(c => c.Instruments).Returns(new List<Instrument>
            {
                new Instrument { InstrumentId = 123, InstrumentName = "Coal", Country = "Germany" },
                new Instrument { InstrumentId = 124, InstrumentName = "Oil", Country = "USA" }
            });
            // Mock the data from example csv
            List<string> inputData = new List<string>() {
                "Type,OrderId,InstrumentId,DateTime,Price",
                "AddOrder,1,123,2023-11-10T01:02:03.6671230,55.01",
                "AddOrder,2,124,2023-11-10T01:03:03.6671230,55.01",
                "UpdateOrder,1,123,2023-11-10T02:05:03.6671230,55.08",
                "UpdateOrder,1,123,2023-11-10T02:05:04.6671230,55.09",
                "UpdateOrder,1,123,2023-11-10T02:06:03.6671230,55.09",
                "DeleteOrder,1,123,2023-11-10T03:04:03.6671230,55.09",
                "AddOrder,3,999,2023-11-10T04:02:08.6671230,55.00"
            };

            var mockFileAccessor = new Mock<IFileAccessor>();
            
            mockFileAccessor.Setup(x => x.Read(It.IsAny<string>())).Returns(inputData.ToList());

            IProgram program = new Program(mockConfig.Object, mockFileAccessor.Object); 
            List<Transaction> transactions = program.ReadTransactions(inputData.ToList());

            Assert.Equal(7, transactions.Count);

            Assert.Equal(1, transactions[0].OrderId);
            Assert.Equal("AddOrder", transactions[0].Type);
            Assert.Equal(123, transactions[0].InstrumentId);
            Assert.Equal("2023-11-10T01:02:03.6671230", transactions[0].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.01m, transactions[0].Price);

            Assert.Equal(2, transactions[1].OrderId);
            Assert.Equal("AddOrder", transactions[1].Type);
            Assert.Equal(124, transactions[1].InstrumentId);
            Assert.Equal("2023-11-10T01:03:03.6671230", transactions[1].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.01m, transactions[1].Price);

            Assert.Equal(1, transactions[2].OrderId);
            Assert.Equal("UpdateOrder", transactions[2].Type);
            Assert.Equal(123, transactions[2].InstrumentId);
            Assert.Equal("2023-11-10T02:05:03.6671230", transactions[2].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.08m, transactions[2].Price);

            Assert.Equal(1, transactions[3].OrderId);
            Assert.Equal("UpdateOrder", transactions[3].Type);
            Assert.Equal(123, transactions[3].InstrumentId);
            Assert.Equal("2023-11-10T02:05:04.6671230", transactions[3].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.09m, transactions[3].Price);

            Assert.Equal(1, transactions[4].OrderId);
            Assert.Equal("UpdateOrder", transactions[4].Type);
            Assert.Equal(123, transactions[4].InstrumentId);
            Assert.Equal("2023-11-10T02:06:03.6671230", transactions[4].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.09m, transactions[4].Price);

            Assert.Equal(1, transactions[5].OrderId);
            Assert.Equal("DeleteOrder", transactions[5].Type);
            Assert.Equal(123, transactions[5].InstrumentId);
            Assert.Equal("2023-11-10T03:04:03.6671230", transactions[5].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.09m, transactions[5].Price);

            Assert.Equal(3, transactions[6].OrderId);
            Assert.Equal("AddOrder", transactions[6].Type);
            Assert.Equal(999, transactions[6].InstrumentId);
            Assert.Equal("2023-11-10T04:02:08.6671230", transactions[6].DateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            Assert.Equal(55.00m, transactions[6].Price);
        }

        [Fact]
        public void Test_ReadTransactions_EmptyData()
        {
            
            var mockConfig = new Mock<IConfig>();
            mockConfig.Setup(c => c.Timezone).Returns("Europe/Malta");
            mockConfig.Setup(c => c.Instruments).Returns(new List<Instrument>
            {
                new Instrument { InstrumentId = 123, InstrumentName = "Coal", Country = "Germany" },
                new Instrument { InstrumentId = 124, InstrumentName = "Oil", Country = "USA" }
            });
            // Empty data mock
            List<string> inputData = new List<string>() {
                "Type,OrderId,InstrumentId,DateTime,Price"
            };

            var mockFileAccessor = new Mock<IFileAccessor>();
            
            mockFileAccessor.Setup(x => x.Read(It.IsAny<string>())).Returns(inputData.ToList());

            IProgram program = new Program(mockConfig.Object, mockFileAccessor.Object); 
            List<Transaction> transactions = program.ReadTransactions(inputData.ToList());

            Assert.Empty(transactions);
        }

        [Fact]
        public void Test_TransformTransactions_GivenExample()
        {
            
            var mockConfig = new Mock<IConfig>();
            mockConfig.Setup(c => c.Timezone).Returns("Europe/Malta");
            mockConfig.Setup(c => c.Instruments).Returns(new List<Instrument>
            {
                new Instrument { InstrumentId = 123, InstrumentName = "Coal", Country = "Germany" },
                new Instrument { InstrumentId = 124, InstrumentName = "Oil", Country = "USA" }
            });
           IProgram program = new Program(mockConfig.Object, new Mock<IFileAccessor>().Object);
            // Manually create transactions list for this test
            List<Transaction> transactions = new List<Transaction>()
            {
                new Transaction { Type = "AddOrder", OrderId = 1, InstrumentId = 123, DateTime = DateTime.ParseExact("2023-11-10T01:02:03.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.01m },
                new Transaction { Type = "AddOrder", OrderId = 2, InstrumentId = 124, DateTime = DateTime.ParseExact("2023-11-10T01:03:03.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.01m },
                new Transaction { Type = "UpdateOrder", OrderId = 1, InstrumentId = 123, DateTime = DateTime.ParseExact("2023-11-10T02:05:03.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.08m },
                new Transaction { Type = "UpdateOrder", OrderId = 1, InstrumentId = 123, DateTime = DateTime.ParseExact("2023-11-10T02:05:04.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.09m },
                new Transaction { Type = "UpdateOrder", OrderId = 1, InstrumentId = 123, DateTime = DateTime.ParseExact("2023-11-10T02:06:03.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.09m },
                new Transaction { Type = "DeleteOrder", OrderId = 1, InstrumentId = 123, DateTime = DateTime.ParseExact("2023-11-10T03:04:03.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.09m },
                new Transaction { Type = "AddOrder", OrderId = 3, InstrumentId = 999, DateTime = DateTime.ParseExact("2023-11-10T04:02:08.6671230", "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture), Price = 55.00m }
            };

            List<OutputTransaction> outputTransactions = program.TransformTransactions(transactions, mockConfig.Object);

            Assert.Equal(6, outputTransactions.Count);

            Assert.Equal(1, outputTransactions[0].OrderId);
            Assert.Equal("ADD", outputTransactions[0].Type);
            Assert.Equal(1, outputTransactions[0].Revision);
            Assert.Equal("2023-11-10T00:02:03.6671230Z", outputTransactions[0].DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.Equal(55.01m, outputTransactions[0].Price);
            Assert.Equal("Germany", outputTransactions[0].Country);
            Assert.Equal("Coal", outputTransactions[0].InstrumentName);

            Assert.Equal("ADD", outputTransactions[1].Type);
            Assert.Equal(1, outputTransactions[1].Revision);
            Assert.Equal("2023-11-10T00:03:03.6671230Z", outputTransactions[1].DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.Equal(55.01m, outputTransactions[1].Price);
            Assert.Equal("USA", outputTransactions[1].Country);
            Assert.Equal("Oil", outputTransactions[1].InstrumentName);

            Assert.Equal("UPDATE", outputTransactions[2].Type);
            Assert.Equal(2, outputTransactions[2].Revision);
            Assert.Equal("2023-11-10T01:05:03.6671230Z", outputTransactions[2].DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.Equal(55.08m, outputTransactions[2].Price);
            Assert.Equal("Germany", outputTransactions[2].Country);
            Assert.Equal("Coal", outputTransactions[2].InstrumentName);

            Assert.Equal("UPDATE", outputTransactions[3].Type);
            Assert.Equal(3, outputTransactions[3].Revision);
            Assert.Equal("2023-11-10T01:05:04.6671230Z", outputTransactions[3].DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.Equal(55.09m, outputTransactions[3].Price);
            Assert.Equal("Germany", outputTransactions[3].Country);
            Assert.Equal("Coal", outputTransactions[3].InstrumentName);

            Assert.Equal("DELETE", outputTransactions[4].Type);
            Assert.Equal(3, outputTransactions[4].Revision);
            Assert.Equal("2023-11-10T02:04:03.6671230Z", outputTransactions[4].DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.Equal(55.09m, outputTransactions[4].Price);
            Assert.Equal("Germany", outputTransactions[4].Country);
            Assert.Equal("Coal", outputTransactions[4].InstrumentName);

            // Checks Error is caught if InstrumentId is invalid
            Assert.Equal("ADD", outputTransactions[5].Type);
            Assert.Equal(1, outputTransactions[5].Revision);
            Assert.Equal("2023-11-10T03:02:08.6671230Z", outputTransactions[5].DateTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.Equal(55.00m, outputTransactions[5].Price);
            Assert.Equal("Error", outputTransactions[5].Country);
            Assert.Equal("Error", outputTransactions[5].InstrumentName);
        }
        [Fact]
        public void Test_TransformTransactions_EmptyData()
        {
            
            var mockConfig = new Mock<IConfig>();
            mockConfig.Setup(c => c.Timezone).Returns("Europe/Malta");
            mockConfig.Setup(c => c.Instruments).Returns(new List<Instrument>
            {
                new Instrument { InstrumentId = 123, InstrumentName = "Coal", Country = "Germany" },
                new Instrument { InstrumentId = 124, InstrumentName = "Oil", Country = "USA" }
            });
           IProgram program = new Program(mockConfig.Object, new Mock<IFileAccessor>().Object);
            // Empty transactions list
            List<Transaction> transactions = new List<Transaction>()
            {};

            List<OutputTransaction> outputTransactions = program.TransformTransactions(transactions, mockConfig.Object);

            Assert.Empty(outputTransactions);
        }
    }
}
