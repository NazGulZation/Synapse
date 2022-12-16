using System;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;

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
    }
}
