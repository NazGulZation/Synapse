using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBG.Synapse.Services;

namespace TBG.Synapse.Test
{
    [TestFixture]
    internal class HelperTest
    {

        [Test]
        public void CalculateAccuracy()
        {
            double[][] output = new double[][]
            {
                new double[] { 0.1, 0.9 },
                new double[] { 0.9, 0.1 },
                new double[] { 0.1, 0.9 }
            };

            double[][] expected = new double[][]
            {
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 1 }
            };

            double accuracy = Helper.CalculateAccuracy(output, expected);

            Assert.That(accuracy, Is.EqualTo(100).Within(1.0));
        }

        [Test]
        public void ConvertToMatrix()
        {
            List<JObject> data = new List<JObject>
            {
                new JObject
                {
                    { "col1", 1 },
                    { "col2", 2.5 },
                    { "col3", "hello" }
                },
                new JObject
                {
                    { "col1", 3 },
                    { "col2", 4.5 },
                    { "col3", "world" }
                }
            };

            object[,] expected =
            {
                { 1, 2.5, "hello" },
                { 3, 4.5, "world" }
            };

            object[,] result = Helper.ConvertJSONListToMatrix(data);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestGetRandomRows()
        {
            // Create a 2D array with 3 rows and 2 columns
            double[][] val = new double[][]
            {
            new double[] { 1, 2 },
            new double[] { 3, 4 },
            new double[] { 5, 6 }
            };

            // Get 2 random rows from the array
            double[][] randomRows = Helper.GetRandomRows(val, 2);

            // Assert that the length of the returned array is equal to the number of rows requested
            Assert.AreEqual(2, randomRows.Length);

            // Assert that each row in the returned array is contained in the original array
            foreach (double[] row in randomRows)
            {
                Assert.Contains(row, val);
            }
        }

        [Test]
        public void JsonToList_ValidInput_ReturnsExpectedResult()
        {
            // Arrange
            var json = new List<JObject>
            {
                new JObject
                {
                    { "name", "cat" },
                    { "age", 3 },
                    { "weight", 5.5 }
                },
                new JObject
                {
                    { "name", "dog" },
                    { "age", 2 },
                    { "weight", 7.5 }
                }
            };
            var expected = new double[2, 4]
            {
                { 1, 0, 3, 5.5 },
                { 0, 1, 2, 7.5 }
            };

            // Act
            var result = Helper.JsonToMatrixDouble(json);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void LoadCsvToJson_ShouldReturnListOfJsonObjects_FromCsvFile()
        {
            // Arrange
            string directory = Path.GetTempPath();
            string filePath = "test.csv";
            string csvData = "Name,Age\r\nJohn,25\r\nJane,30";
            File.WriteAllText($"{directory}\\{filePath}", csvData);
            List<string> columns = new List<string> { "Name" };

            // Act
            List<object> jsonObjects = Helper.LoadCsvToJson(directory, filePath, columns);

            // Assert
            Assert.IsInstanceOf<List<object>>(jsonObjects);
            Assert.AreEqual(2, jsonObjects.Count);
            Assert.IsInstanceOf<Dictionary<string, string>>(jsonObjects[0]);
            Assert.AreEqual(1, (jsonObjects[0] as Dictionary<string, string>).Count);
            Assert.AreEqual("John", (jsonObjects[0] as Dictionary<string, string>)["Name"]);

            // Cleanup
            File.Delete($"{directory}\\{filePath}");
        }

        [Test]
        public void OneHotEncode()
        {
            // Arrange
            var input = new List<string> { "cat", "dog", "bird", "cat", "bird" };
            var expected = new float[][]
            {
                new float[] { 1, 0, 0 },
                new float[] { 0, 1, 0 },
                new float[] { 0, 0, 1 },
                new float[] { 1, 0, 0 },
                new float[] { 0, 0, 1 }
            };

            // Act
            var result = Helper.OneHotEncode(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void OneHotDecode()
        {
            // Define the input data
            var input = new List<string> { "cat", "dog", "cat", "bird", "dog" };

            // Encode the input data
            var oneHotEncoded = Helper.OneHotEncode(input);

            // Decode the encoded data
            var decoded = Helper.OneHotDecode(oneHotEncoded, input);

            // Assert that the decoded data is equal to the original input data
            CollectionAssert.AreEqual(input, decoded);
        }

        [Test]
        public void SelectColumns()
        {
            // Define the input array
            object[,] array = new object[3, 5] {
                { 1, 2, 3, 4, 5 },
                { 6, 7, 8, 9, 10 },
                { 11, 12, 13, 14, 15 }
            };

            // Define the column indices to select
            int[] columnIndices = { 0, 2, 4 };

            // Call the SelectColumns function
            object[,] selectedColumns = Helper.SelectColumns(array, columnIndices);

            // Assert that the returned array has the expected number of rows and columns
            Assert.AreEqual(3, selectedColumns.GetLength(0));
            Assert.AreEqual(3, selectedColumns.GetLength(1));

            // Assert that the returned array has the expected values
            Assert.AreEqual(1, selectedColumns[0, 0]);
            Assert.AreEqual(3, selectedColumns[0, 1]);
            Assert.AreEqual(5, selectedColumns[0, 2]);
            Assert.AreEqual(6, selectedColumns[1, 0]);
            Assert.AreEqual(8, selectedColumns[1, 1]);
            Assert.AreEqual(10, selectedColumns[1, 2]);
            Assert.AreEqual(11, selectedColumns[2, 0]);
            Assert.AreEqual(13, selectedColumns[2, 1]);
            Assert.AreEqual(15, selectedColumns[2, 2]);
        }

        [Test]
        public void SelectJsonColumns()
        {
            // Arrange
            var objects = new List<JObject>
            {
                new JObject
                {
                    { "ID", 1 },
                    { "Name", "John" },
                    { "Age", 30 }
                },
                new JObject
                {
                    { "ID", 2 },
                    { "Name", "Jane" },
                    { "Age", 25 }
                }
            };

            var expected = new List<JObject>
            {
                new JObject
                {
                    { "ID", 1 },
                    { "Name", "John" }
                },
                new JObject
                {
                    { "ID", 2 },
                    { "Name", "Jane" }
                }
            };

            // Act
            var selectedColumns = Helper.SelectJsonColumns(objects, new string[] { "ID", "Name" });

            // Assert
            Assert.AreEqual(expected.Count, selectedColumns.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], selectedColumns[i]);
            }
        }

    }
}
