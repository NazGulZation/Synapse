using Accord.Neuro;
using Newtonsoft.Json.Linq;
using System;
using TBG.Synapse.Repository;
using TBG.Synapse.Services;
using TBG.Synapse.Test;

// Ask the user for the training data CSV path
Console.WriteLine("Enter the path of the training data CSV file:");
string trainingCsvPath = Console.ReadLine();

// Split the CSV path into the directory and file name
string trainingDirectory = Path.GetDirectoryName(trainingCsvPath);
string trainingFileName = Path.GetFileName(trainingCsvPath);

// Load the training data CSV using LoadCsvToJson with all columns loaded
List<JObject> trainingJsonObjects = Helper.LoadCsvToJson(trainingDirectory, trainingFileName);

Console.WriteLine();
// Show to user the top 10 rows with column header of the training data CSV
Console.WriteLine("Top 10 rows of the training data CSV file:");

// Get the column headers from the first JSON object
string[] headers = trainingJsonObjects[0].Properties().Select(p => p.Name.Replace("\r", "")).ToArray();

// Print the column headers
Console.WriteLine(string.Join(",", headers));

// Print the first 10 rows of the training data
for (int i = 0; i < 10 && i < trainingJsonObjects.Count; i++)
{
    string[] row = trainingJsonObjects[i].Properties().Select(p => p.Value.ToString()).ToArray();
    Console.WriteLine(string.Join(",", row));
}


Console.WriteLine();
// Ask the user for the validation data CSV path
Console.WriteLine("Enter the path of the validation data CSV file:");
string validationCsvPath = Console.ReadLine();

// Split the CSV path into the directory and file name
string validationDirectory = Path.GetDirectoryName(validationCsvPath);
string validationFileName = Path.GetFileName(validationCsvPath);

// Load the validation data CSV using LoadCsvToJson with all columns loaded
List<JObject> validationJsonObjects = Helper.LoadCsvToJson(validationDirectory, validationFileName);

Console.WriteLine();
// Show to user the top 10 rows with column header of the training data CSV
Console.WriteLine("Top 10 rows of the training data CSV file:");

// Print the column headers
Console.WriteLine(string.Join(",", headers));

// Print the first 10 rows of the training data
for (int i = 0; i < 10 && i < validationJsonObjects.Count; i++)
{
    string[] row = validationJsonObjects[i].Properties().Select(p => p.Value.ToString()).ToArray();
    Console.WriteLine(string.Join(",", row));
}


Console.WriteLine("");
// Ask the user which columns to use as output
Console.WriteLine("Enter the names of the columns to use as output, separated by commas:");
string outputColumnsString = Console.ReadLine();

// Split the output column names into a list
List<string> outputColumns = outputColumnsString.Split(',').Select(s => s.Trim()).ToList();

// Get the input columns
List<string> inputcolumns = (trainingJsonObjects[0]).Properties()
    .Select(p => p.Name).Except(outputColumns).ToList();

var trainingDataInputJson = Helper.SelectJsonColumns(trainingJsonObjects, inputcolumns.ToArray());
var trainingDataInput = Helper.JsonToMatrixDouble(trainingDataInputJson);

var trainingDataOutputJson = Helper.SelectJsonColumns(trainingJsonObjects, outputColumns.ToArray());
var trainingDataOutput = Helper.JsonToMatrixDouble(trainingDataOutputJson);

var validationDataInputJson = Helper.SelectJsonColumns(validationJsonObjects, inputcolumns.ToArray());
var validationDataInput = Helper.JsonToMatrixDouble(validationDataInputJson);

var validationDataOutputJson = Helper.SelectJsonColumns(validationJsonObjects, outputColumns.ToArray());
var validationDataOutput = Helper.JsonToMatrixDouble(validationDataOutputJson);

int inputNeurons = trainingDataInput.GetLength(1);
int hiddenNeurons = Convert.ToInt32(Math.Ceiling(trainingDataInput.GetLength(1)/2d));
int outputNeurons = trainingDataOutput.GetLength(1);

NeuralNetworkServices neuralNetworkServices = new NeuralNetworkServices();
var network = neuralNetworkServices
    .CreateNeuralNetwork(inputNeurons, hiddenNeurons, outputNeurons);

Console.ReadLine();