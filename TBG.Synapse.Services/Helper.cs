using System.Diagnostics;
using System.IO;
using System.Text;
using TBG.Synapse.Repository;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Accord.Math;
using System.Data.Common;
using Accord.Neuro;

namespace TBG.Synapse.Services
{
    public static class Helper
    {
        public readonly static string SynapseFileExtension = "tbg.synapse";

        private readonly static SynapseRepository<LogError> _logErrorRepo;

        static Helper()
        {
            _logErrorRepo = new SynapseRepository<LogError>("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=Synapse;Integrated Security=True;");
            _logErrorRepo.CreateTable();
        }

        public static double CalculateAccuracy(double[][] output, double[][] expected)
        {
            double mse = 0;

            for (int i = 0; i < output.Length; i++)
            {
                for (int j = 0; j < output[i].Length; j++)
                {
                    mse += Math.Pow(output[i][j] - expected[i][j], 2);
                }
            }

            mse /= output.Length;

            // The maximum MSE occurs when the output and expected values are completely different,
            // so we normalize the MSE by dividing it by the maximum MSE.
            return 100 * (1 - mse / (output[0].Length * Math.Pow(1 - (-1), 2)));
        }

        public static object[,] ConvertJSONListToMatrix(List<JObject> data)
        {
            int numRows = data.Count;
            int numCols = data[0].Count;
            string[] columnNames = data[0].Properties().Select(p => p.Name).ToArray();

            var matrix = Matrix.Create<object>(numRows, numCols, 0);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    JToken value = data[i][columnNames[j]];

                    if (value.Type == JTokenType.Integer)
                    {
                        matrix[i, j] = (int)value;
                    }
                    else if (value.Type == JTokenType.Float)
                    {
                        matrix[i, j] = (float)value;
                    }
                    else if (value.Type == JTokenType.String)
                    {
                        string strValue = (string)value;
                        if (double.TryParse(strValue, out double dValue))
                            matrix[i, j] = dValue;
                        else if (int.TryParse(strValue, out int intValue))
                            matrix[i, j] = intValue;
                        else
                            matrix[i, j] = strValue;
                    }
                    // Add additional cases for other data types as needed
                }
            }

            return matrix;
        }

        private static LogError GetLogError(Exception ex)
        {
            var logError = new LogError();
            logError.Message = ex.Message;
            StackTrace stackTrace = new StackTrace(ex, true);
            if (stackTrace.GetFrames().Count() > 0)
            {
                FileInfo fileInfo = new FileInfo(stackTrace.GetFrame(0).GetFileName());
                logError.FileName = stackTrace.GetFrame(0).GetFileName();
                logError.Line = stackTrace.GetFrame(0).GetFileLineNumber();
                logError.Column = stackTrace.GetFrame(0).GetFileColumnNumber();
                logError.Code = stackTrace.GetFrame(0).GetMethod().Name;
            }
            return logError;
        }

        public static double[][] GetRandomRows(double[][] val, int rows)
        {
            // Create a list to hold the selected rows
            var selectedRows = new List<double[]>();

            // Create a random number generator
            var rng = new Random();

            // Select the specified number of rows at random
            for (int i = 0; i < rows; i++)
            {
                // Get a random index within the range of the array
                int index = rng.Next(val.Length);

                // Add the row at the randomly chosen index to the list
                selectedRows.Add(val[index]);
            }

            // Return the list of selected rows as an array
            return selectedRows.ToArray();
        }

        public static List<JObject> LoadCsvToJson(string directory, string filePath, List<string> columns = null)
        {
            if (!File.Exists($"{directory}\\{filePath}"))
            {
                throw new DirectoryNotFoundException("The specified CSV file does not exist");
            }

            // Read the CSV data from a file
            string csvData = File.ReadAllText($"{directory}\\{filePath}");

            // Split the CSV data into lines
            string[] lines = csvData.Split('\n');

            // Get the field names from the first line
            string[] fieldNames = lines[0].Split(',');

            // Create a list to hold the JSON objects
            List<object> jsonObjects = new List<object>();

            // Loop through the remaining lines
            for (int i = 1; i < lines.Length; i++)
            {
                // Split the current line into fields
                string[] fields = lines[i].Split(',');

                // Create a new dictionary to hold the field values
                Dictionary<string, string> jsonObject = new Dictionary<string, string>();

                // Loop through the fields and add them to the dictionary
                for (int j = 0; j < fieldNames.Length; j++)
                {
                    // Check if the current field should be included in the JSON object
                    if (columns == null || columns.Contains(fieldNames[j]))
                    {
                        jsonObject.Add(fieldNames[j].Replace("\r", ""), fields[j].Replace("\r", ""));
                    }
                }

                // Add the JSON object to the list
                jsonObjects.Add(jsonObject);
            }

            // Return the list of JSON objects
            return jsonObjects.Select(o => JObject.FromObject(o)).ToList(); ;
        }


        public static double[,] JsonToMatrixDouble(List<JObject> input)
        {
            object[,] matrix = ConvertJSONListToMatrix(input);

            int rows = matrix.GetLength(0); // number of rows in the matrix
            int cols = matrix.GetLength(1); // number of columns in the matrix

            // Lists to store the indices of the non-string columns and string columns
            List<int> nonStringCols = new List<int>();
            List<int> stringCols = new List<int>();

            // Iterate through the columns
            for (int i = 0; i < cols; i++)
            {
                // Check if the current column is a string column
                bool isStringColumn = (matrix[1, i] is string);
                // Add the current column index to the appropriate list
                if (isStringColumn)
                    stringCols.Add(i);
                else
                    nonStringCols.Add(i);
            }

            // Create a new 2D matrix to store the converted values
            object[,] oneHotEncoded = new object[rows, cols];

            // Iterate through the columns
            for (int i = 0; i < cols; i++)
            {
                // Check if the current column is a string column
                if (stringCols.Contains(i))
                {
                    // Convert the string column to a float column using OneHotEncode function
                    List<string> inputString = new List<string>();
                    for (int j = 0; j < rows; j++)
                    {
                        inputString.Add((string)matrix[j, i]);
                    }
                    double[][] encoded = OneHotEncode(inputString);
                    for (int j = 0; j < rows; j++)
                    {
                        oneHotEncoded[j, i] = encoded[j];
                    }
                }
                else
                {
                    // Convert the non-string column to a float column
                    for (int j = 0; j < rows; j++)
                    {
                        oneHotEncoded[j, i] = Convert.ToDouble(matrix[j, i]);
                    }
                }
            }

            int newCols = 0;
            for (int i = 0; i < oneHotEncoded.GetLength(1); i++)
            {
                if (oneHotEncoded[0, i] is not double[])
                    newCols++;
                else
                    newCols += ((double[])oneHotEncoded[0, i]).GetLength(0);
            }

            double[,] returnData = new double[rows, newCols];
            // Iterate through the columns
            for (int i = 0; i < rows; i++)
            {
                int currentCol = 0;
                for (int j = 0; j < cols; j++)
                {
                    if (oneHotEncoded[i, j] is not double[])
                    {
                        returnData[i, currentCol] = (double)oneHotEncoded[i, j];
                        currentCol++;
                    }
                    else
                    {
                        foreach (double value in (double[])oneHotEncoded[i, j])
                        {
                            returnData[i, currentCol] = value;
                            currentCol++;
                        }
                    }
                }
            }

            return returnData;
        }

        public static string LogError(Exception ex)
        {
            LogError logError = GetLogError(ex);
            logError.ID = Guid.NewGuid().ToString();
            logError.TimeStamp = DateTime.Now;

            _logErrorRepo.Insert(logError);

            return $"Error on system with ID: {logError.ID}";
        }

        public static List<string> OneHotDecode(double[][] oneHotEncoded, List<string> input)
        {
            // Get the unique values in the input list
            var uniqueValues = input.Distinct().ToList();

            // Initialize a list to hold the decoded values
            var decoded = new List<string>();

            // Iterate over each row in the one-hot encoded array
            foreach (var row in oneHotEncoded)
            {
                // Find the index of the element with the value of 1
                int index = Array.IndexOf(row, 1);

                // Add the corresponding value from the unique values list to the decoded list
                decoded.Add(uniqueValues[index]);
            }

            return decoded;
        }

        public static double[][] OneHotEncode(List<string> input)
        {
            // Get the unique values in the input list
            var uniqueValues = input.Distinct().ToList();

            // Initialize a list to hold the one-hot encoded values
            var oneHotEncoded = new List<double[]>();

            // Iterate over each value in the input list
            foreach (var value in input)
            {
                // Initialize an array to hold the one-hot encoded values for this value
                var row = new double[uniqueValues.Count];

                // Set the value for the corresponding index to 1 and the rest to 0
                for (int i = 0; i < uniqueValues.Count; i++)
                {
                    if (value == uniqueValues[i])
                    {
                        row[i] = 1;
                    }
                    else
                    {
                        row[i] = 0;
                    }
                }

                // Add the one-hot encoded values for this value to the list
                oneHotEncoded.Add(row);
            }

            return oneHotEncoded.ToArray();
        }

        public static object[,] SelectColumns(object[,] array, int[] columnIndices)
        {
            // Determine the number of rows and columns in the array
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            // Create a new array to hold the selected columns
            object[,] selectedColumns = new object[rows, columnIndices.Length];

            // Loop through each row and copy the selected columns to the new array
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columnIndices.Length; j++)
                {
                    selectedColumns[i, j] = array[i, columnIndices[j]];
                }
            }

            return selectedColumns;
        }

        public static List<JObject> SelectJsonColumns(List<JObject> objects, string[] columns)
        {
            // Create a new list to hold the selected columns
            var selectedColumns = new List<JObject>();

            // Iterate through each object in the input list
            foreach (var obj in objects)
            {
                // Create a new JSON object to hold the selected columns
                var selectedObject = new JObject();

                // Iterate through each column in the list of column names
                foreach (var column in columns)
                {
                    // Add the specified column to the selected object, if it exists in the original object
                    if (obj.ContainsKey(column))
                    {
                        selectedObject[column] = obj[column];
                    }
                }

                // Add the selected object to the list of selected columns
                selectedColumns.Add(selectedObject);
            }

            // Return the list of selected columns
            return selectedColumns;
        }
    }
}