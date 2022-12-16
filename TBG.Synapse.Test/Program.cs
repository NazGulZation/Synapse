using Accord.Neuro;
using TBG.Synapse.Repository;
using TBG.Synapse.Services;
using TBG.Synapse.Test;

var connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=Synapse;Integrated Security=True;";

#region Test Repo
//RepoTest.TestCreate();
//RepoTest.TestInsert();
//RepoTest.TestUpdate();
//RepoTest.TestDelete();
#endregion

#region Test LogError
//try
//{
//    throwError();
//}
//catch (Exception ex)
//{
//    Helper.LogError(ex);
//}

//void throwError()
//{
//    throwErrorError();
//}

//void throwErrorError()
//{
//    throw new Exception("AAA");
//}
#endregion

#region Test Neural Network
//NeuralNetworkServices neuralNetworkServices = new NeuralNetworkServices();
//Network network = neuralNetworkServices.CreateNeuralNetwork(2, 2, 1);
//neuralNetworkServices.SaveNeuralNetwork(network, "Models", "TestModel");
//Network loadedNetwork = neuralNetworkServices.LoadNetwork("Models", "TestModel");
#endregion

var obj = Helper.LoadCsvToJson("Dataset", "data_revenue.csv");
var foo = "bar";