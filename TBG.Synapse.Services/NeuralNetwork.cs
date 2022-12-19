using System;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;
using Accord.Statistics.Kernels;
using Newtonsoft.Json.Linq;

namespace TBG.Synapse.Services
{
    public class NeuralNetworkServices
    {
        public Network CreateNeuralNetwork(int x, int y, int z)
        {

            // Create a new neural network with X input, Y hidden, and Z output neurons
            ActivationNetwork network = new ActivationNetwork(
                new SigmoidFunction(), // activation function
                x, // number of input neurons
                y, // number of hidden neurons
                z // number of output neurons
            );

            network.Randomize();

            return network;
        }

        public void SaveNeuralNetwork(Network network, string directory, string networkName)
        {
            Directory.CreateDirectory(directory);
            network.Save($"{directory}\\{networkName}.{Helper.SynapseFileExtension}");
        }

        public Network LoadNetwork(string directory, string networkName)
        {
            // check if the directory exists
            if (!File.Exists($"{directory}\\{networkName}.{Helper.SynapseFileExtension}"))
            {
                throw new DirectoryNotFoundException("The specified Model does not exist");
            }

            // load the network from the specified directory
            Network network = Network.Load($"{directory}\\{networkName}.{Helper.SynapseFileExtension}");

            return network;
        }

        public Network TrainNetwork(Network network, double[,] input, double[,] target, double[,] validationInput, double[,] validationOutput, double targetAccuracy)
        {
            double[][] jaggedInput = Matrix.Create(input).ToJagged();
            double[][] jaggedTarget = Matrix.Create(target).ToJagged();
            double[][] jaggedValidationInput = Matrix.Create(validationInput).ToJagged();
            double[][] jaggedValidationOutput = Matrix.Create(validationOutput).ToJagged();

            // Create a new backpropagation learning algorithm
            var teacher = new BackPropagationLearning((ActivationNetwork)network);

            // Set the learning rate and momentum
            teacher.LearningRate = 0.1;
            teacher.Momentum = 0.1;

            // Set the learning rate to stop when the error is less than 0.01
            teacher.LearningRate = 0.01;

            double accuracy = 0;
            while (accuracy < targetAccuracy)
            {
                teacher.RunEpoch(jaggedInput, jaggedTarget);

                double[][] jaggedOutput = new double[validationOutput.GetLength(0)][];
                for (int i = 0; i < validationInput.GetLength(0); i++)
                {
                    double[] row = Enumerable.Range(0, validationInput.GetLength(1))
                        .Select(colIndex => validationInput[i, colIndex])
                        .ToArray();
                    double[] rowOutput = network.Compute(row);
                    jaggedOutput[i] = rowOutput;
                }

                accuracy = Helper.CalculateAccuracy(jaggedOutput, jaggedValidationOutput);
            }

            // Return the trained network
            return network;
        }
    }
}
