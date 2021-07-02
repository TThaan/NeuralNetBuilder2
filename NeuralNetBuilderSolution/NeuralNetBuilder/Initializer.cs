using DeepLearningDataProvider;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    // wa: Just test/run a given net?

    public class Initializer
    {
        #region public

        public string NetAndTrainerParametersPath { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\ConsoleApi_NetAndTrainerParameters_test.txt";
        public string LogPath { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\ConsoleApi_Log.txt";
        public string SampleSetParametersPath { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\ConsoleApi_SampleSetParameters.txt";
        public string SampleSetPath { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\ConsoleApi_SampleSet.txt";
        public string InitializedNetPath { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\ConsoleApi_InitializedNet_test.txt";
        public string TrainedNetPath { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\ConsoleApi_TrainedNet.txt";

        public ISampleSetParameters SampleSetParameters { get; set; }
        public INetParameters NetParameters { get; set; }
        public ITrainerParameters TrainerParameters { get; set; }
        public ISampleSet SampleSet { get; set; }
        public INet Net { get; set; }
        public INet TrainedNet { get; set; }
        public ITrainer Trainer { get; set; }

        public async Task<bool> TrainAsync()
        {
            if (Trainer == null)
            {
                OnInitializerStatusChanged("\nYou need a trainer to start training!");
                return false;
            }

            try
            {
                OnInitializerStatusChanged($"\n            Training, please wait...\n");
                await Trainer.Train(LogPath, TrainerParameters.Epochs);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
                TrainedNet = Trainer.TrainedNet?.GetCopy();
                OnInitializerStatusChanged($"\n            Finished training.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> InitializeNetAsync()  // Task<bool> if method succeeded?
        {
            if (NetParameters == null)
            {
                OnInitializerStatusChanged("\nYou need net parameters to initialize the net!");
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("\nInitializing net, please wait...");
                    INet rawNet = NetFactory.CreateRawNet();                // as async method?
                    Net = NetFactory.InitializeNet(rawNet, NetParameters);  // as async method?
                    OnInitializerStatusChanged("Successfully initialized net.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> InitializeTrainerAsync()
        {
            if (TrainerParameters == null)
            {
                OnInitializerStatusChanged("\nYou need trainer parameters to initialize the trainer!");
                return false;
            }

            // Attach net & sampleset to trainer after initializing?
            if (Net == null)
            {
                OnInitializerStatusChanged("\nYou need to initialize the net to initialize the trainer!");
                return false;
            }
            if (SampleSet == null)
            {
                OnInitializerStatusChanged("\nYou need a sample set to initialize the trainer!");
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("\nInitializing trainer, please wait...");
                    ITrainer rawTrainer = TrainerFactory.GetRawTrainer();                                       // as async method?
                    Trainer = TrainerFactory.InitializeTrainer(rawTrainer, Net, TrainerParameters, SampleSet);  // as async method?
                    OnInitializerStatusChanged("Successfully initialized trainer.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> LoadNetAndTrainerParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("\nLoading net & trainer parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(NetAndTrainerParametersPath);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    NetParameters = sp.NetParameters;
                    TrainerParameters = sp.TrainerParameters;
                    OnInitializerStatusChanged("Successfully loaded net & trainer parameters.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged($"That didn't work.\n({e.Message})"); return false; }
            });
        }
        public async Task<bool> LoadSampleSetParametersAsync()
        {
            if (SampleSetParametersPath == default)
            {
                OnInitializerStatusChanged("No path to sample set parameters is set.");
                return false;
            }

            try
            {
                OnInitializerStatusChanged("\nLoading sample set parameters from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(SampleSetParametersPath);
                SampleSetParameters = JsonConvert.DeserializeObject<SampleSetParameters>(jsonString);
                OnInitializerStatusChanged("Successfully loaded sample set parameters.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> CreateSampleSetAsync()
        {
            if (SampleSetParameters == null)
            {
                OnInitializerStatusChanged("No sample set parameters are set.");
                return false;
            }

            try
            {
                var sampleSetSteward = new SampleSetSteward();

                OnInitializerStatusChanged("\nCreating samples, please wait...");
                SampleSet = await sampleSetSteward.CreateSampleSetAsync(SampleSetParameters);
                SampleSet.Parameters = (SampleSetParameters)SampleSetParameters;    // Use interface in data provider?
                OnInitializerStatusChanged("Successfully created samples.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveInitializedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("\nSaving initialized net, please wait...");
                var jsonString = JsonConvert.SerializeObject(Net, Formatting.Indented);
                await File.WriteAllTextAsync(InitializedNetPath, jsonString);
                OnInitializerStatusChanged("Successfully saved initialized net.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("\nSaving trained net, please wait...");
                var jsonString = JsonConvert.SerializeObject(TrainedNet, Formatting.Indented);
                await File.WriteAllTextAsync(TrainedNetPath, jsonString);
                OnInitializerStatusChanged("Successfully saved trained net.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveSampleSetAsync()
        {
            try
            {
                OnInitializerStatusChanged("\nSaving sample set, please wait...");
                var jsonString = JsonConvert.SerializeObject(SampleSet, Formatting.Indented);
                await File.WriteAllTextAsync(SampleSetPath, jsonString);
                OnInitializerStatusChanged("Successfully saved sample set.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadInitializedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("\nLoading initialized net from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(InitializedNetPath);

                dynamic dynamicNet = JObject.Parse(jsonString);
                ILayer[] layers = ((JArray)dynamicNet.Layers).ToObject<Layer[]>();

                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i].Id > 0)
                        layers[i].ReceptiveField = layers[i - 1];
                    if (layers[i].Id < layers.Length - 1)
                        layers[i].ProjectiveField = layers[i + 1];
                }

                Net = JsonConvert.DeserializeObject<Net>(jsonString);
                Net.Layers = layers;
                OnInitializerStatusChanged("Successfully loaded initialized net.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadTrainedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("\nLoading trained net from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(TrainedNetPath);

                dynamic dynamicNet = JObject.Parse(jsonString);
                ILayer[] layers = ((JArray)dynamicNet.Layers).ToObject<Layer[]>();

                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i].Id > 0)
                        layers[i].ReceptiveField = layers[i - 1];
                    if (layers[i].Id < layers.Length - 1)
                        layers[i].ProjectiveField = layers[i + 1];
                }

                TrainedNet = JsonConvert.DeserializeObject<Net>(jsonString);
                Net.Layers = layers;
                OnInitializerStatusChanged("Successfully loaded trained net.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadSampleSetAsync()
        {
            try
            {
                OnInitializerStatusChanged("\nLoading samples from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(SampleSetPath);

                dynamic dynamicSampleSet = JObject.Parse(jsonString);
                SampleSetParameters parameters = ((JObject)dynamicSampleSet.Parameters).ToObject<SampleSetParameters>();
                Sample[] testingSamples = ((JArray)dynamicSampleSet.TestingSamples).ToObject<Sample[]>();
                Sample[] trainingSamples = ((JArray)dynamicSampleSet.TrainingSamples).ToObject<Sample[]>();
                SampleSet = new SampleSet
                {
                    Parameters = parameters,
                    TestingSamples = testingSamples,
                    TrainingSamples = trainingSamples
                };
                Sample.Tolerance = parameters.TargetTolerance;
                OnInitializerStatusChanged("Successfully loaded samples.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }

        #endregion

        #region InitializerEventHandler

        public event InitializerStatusChangedEventHandler InitializerStatusChanged;

        void OnInitializerStatusChanged(string info)
        {
            InitializerStatusChanged?.Invoke(this, new InitializerStatusChangedEventArgs(info));
        }

        #endregion
    }
}
