using System;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;
using Accord.Statistics.Kernels;

namespace TBG.Synapse.Services
{
    public class NeuralNetworkServices
    {
        public double CalculateAccuracy(Network network, double[][] input, double[][] output)
        {
            // Initialize a counter for the number of correct predictions
            int correct = 0;

            // Iterate through each data point in the validation set
            for (int i = 0; i < input.Length; i++)
            {
                // Make a prediction with the neural network
                double[] prediction = network.Compute(input[i]);

                // Calculate the error between the prediction and the output
                double error = output[i].Subtract(prediction).Abs().Sum();

                // If the error is below a certain threshold, consider the prediction correct
                if (error < 0.1)
                {
                    correct++;
                }
            }

            // Return the approximate accuracy as a percentage
            return (double)correct / input.Length * 100;
        }

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

        public Network TrainNetwork(Network network, double[,] input, double[,] output, double[,] validation)
        {
            double[][] jaggedInput = Matrix.Create(input).ToJagged();
            double[][] jaggedOutput = Matrix.Create(output).ToJagged();
            double[][] jaggedValidation = Matrix.Create(validation).ToJagged();

            // Create a new backpropagation learning algorithm
            var teacher = new BackPropagationLearning((ActivationNetwork)network);

            // Set the learning rate and momentum
            teacher.LearningRate = 0.1;
            teacher.Momentum = 0.1;

            // Set the learning rate to stop when the error is less than 0.01
            teacher.LearningRate = 0.01;

            double accuracy = 0;
            while (accuracy < 80)
            {
                teacher.RunEpoch(jaggedInput, jaggedOutput);
                accuracy = CalculateAccuracy(network, jaggedInput, jaggedValidation);
            }

            // Return the trained network
            return network;
        }
    }
}
