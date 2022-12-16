using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Neuro;
using Keras.Layers;
using Keras.Models;
using NUnit.Framework;
using TBG.Synapse.Services;

namespace TBG.Synapse.Test
{
    [TestFixture]
    public static class NeuralNetworkTest
    {
        [Test]
        public static void CreateNeuralNetwork_ReturnsCorrectStructure()
        {
            // Arrange
            int x = 2;
            int y = 3;
            int z = 1;
            bool isRegression = false;

            NeuralNetworkServices neuralNetworkServices = new NeuralNetworkServices();

            // Act
            Network network = neuralNetworkServices.CreateNeuralNetwork(x, y, z);

            // Assert
            Assert.AreEqual(x, network.InputsCount);
            Assert.AreEqual(y, network.Layers[0].Neurons.Count());
            Assert.AreEqual(z, network.Layers[1].Neurons.Count());
        }

        [Test]
        public static void LoadNetwork_LoadsNetworkFromFile()
        {
            NeuralNetworkServices neuralNetworkServices = new NeuralNetworkServices();

            // Arrange
            Network network = neuralNetworkServices.CreateNeuralNetwork(2, 3, 1);  // create a sample network
            string directory = "TestDirectory";
            string networkName = "TestNetwork";
            neuralNetworkServices.SaveNeuralNetwork(network, directory, networkName);  // save the network to a file

            // Act
            Network loadedNetwork = neuralNetworkServices.LoadNetwork(directory, networkName);

            // Assert
            Assert.AreEqual(network.InputsCount, loadedNetwork.InputsCount);
            Assert.AreEqual(network.Layers[0].Neurons.Count(), loadedNetwork.Layers[0].Neurons.Count());
            Assert.AreEqual(network.Layers[1].Neurons.Count(), loadedNetwork.Layers[1].Neurons.Count());
            for (int i = 0; i < loadedNetwork.Layers.Count(); i++)
            {
                Layer iLayer = loadedNetwork.Layers[i];
                for (int j = 0; j < iLayer.Neurons.Count(); j++)
                {
                    double[] weight = network.Layers[i].Neurons[j].Weights;
                    double[] loadedWeight = loadedNetwork.Layers[i].Neurons[j].Weights;
                    Assert.AreEqual(weight, loadedWeight);
                }
            }

            // delete the test directory and its contents
            Directory.Delete(directory, true);
        }

        [Test]
        public static void SaveNeuralNetwork_SavesNetworkToFile()
        {
            // Arrange
            NeuralNetworkServices neuralNetworkServices = new NeuralNetworkServices();

            Network network = neuralNetworkServices.CreateNeuralNetwork(2, 3, 1);  // create a sample network
            string directory = "TestDirectory";
            string networkName = "TestNetwork";

            // Act
            neuralNetworkServices.SaveNeuralNetwork(network, directory, networkName);

            // Assert
            string expectedFilePath = $"{directory}\\{networkName}.{Helper.SynapseFileExtension}";
            Assert.IsTrue(File.Exists(expectedFilePath));

            // delete the test directory and its contents
            Directory.Delete(directory, true);
        }
    }
}
