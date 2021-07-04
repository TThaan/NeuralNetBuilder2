using DeepLearningDataProvider;
using NeuralNetBuilder.Builders;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    // wa: Just test/run a given net?
    // wa: global parameters? Or in NetParameters?
    public class Initializer
    {
        #region fields & ctor

        private PathBuilder paths;
        private ParameterBuilder parameters;
        private ISampleSet sampleSet;
        private INet net, trainedNet;
        private ITrainer trainer;

        public Initializer()
        {
            paths = new PathBuilder(OnInitializerStatusChanged);            // via DC?
            parameters = new ParameterBuilder(OnInitializerStatusChanged);  // via DC?
        }

        #endregion

        #region properties

        public PathBuilder Paths
        {
            get
            {
                if (paths == null)
                    OnInitializerStatusChanged("Paths are null");
                return paths;
            }
        }
        public ParameterBuilder Parameters
        {
            get
            {
                if (parameters == null)
                    OnInitializerStatusChanged("Parameters are null");
                return parameters;
            }
        }

        public ISampleSet SampleSet
        {
            get
            {
                if (sampleSet == null)
                    OnInitializerStatusChanged("SampleSet is null");
                return sampleSet;
            }
            set { sampleSet = value; }
        }
        public INet Net
        {
            get
            {
                if (net == null)
                    OnInitializerStatusChanged("Net is null");
                return net;
            }
            set { net = value; }
        }
        public INet TrainedNet
        {
            get
            {
                if (trainedNet == null)
                    OnInitializerStatusChanged("TrainedNet is null");
                return trainedNet;
            }
            set { trainedNet = value; }
        }
        public ITrainer Trainer
        {
            get
            {
                if (trainer == null)
                    OnInitializerStatusChanged("Trainer is null");
                return trainer;
            }
            set { trainer = value; }
        }
        public bool IsLogged { get; set; }

        #endregion

        #region methods

        public async Task<bool> TrainAsync()
        {
            if (Trainer == null)
            {
                OnInitializerStatusChanged("\nYou need a trainer to start training!");
                return false;
            }
            if (Net == null)
            {
                OnInitializerStatusChanged("\nYou need a net to start training!");
                return false;
            }
            if (SampleSet == null)
            {
                OnInitializerStatusChanged("\nYou need a sample set to start training!");
                return false;
            }

            try
            {
                OnInitializerStatusChanged($"\n            Training, please wait...\n");
                await Trainer.Train(Net, SampleSet, IsLogged ? Paths.Log : default);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
                TrainedNet = Trainer.TrainedNet?.GetCopy();
                OnInitializerStatusChanged($"\n            Finished training.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> CreateNetAsync()  // Task<bool> if method succeeded?
        {
            if (Parameters.NetParameters == null)
            {
                OnInitializerStatusChanged("\nYou need net parameters to initialize the net!");
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("\nInitializing net, please wait...");
                    Net = NetFactory.CreateNet(Parameters.NetParameters);  // as async method?
                    OnInitializerStatusChanged("Successfully initialized net.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> CreateTrainerAsync()
        {
            if (Parameters.TrainerParameters == null)
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
                    Trainer = new Trainer(Parameters.TrainerParameters);
                    OnInitializerStatusChanged("Successfully initialized trainer.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> LoadNetParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("\nLoading net parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(Paths.NetParameters);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    Parameters.NetParameters = sp.NetParameters;
                    OnInitializerStatusChanged("Successfully loaded net parameters.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged($"{e.Message}"); return false; }
            });
        }
        public async Task<bool> LoadTrainerParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("\nLoading trainer parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(Paths.TrainerParameters);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    Parameters.TrainerParameters = sp.TrainerParameters;
                    OnInitializerStatusChanged("Successfully loaded trainer parameters.\n");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged($"{e.Message}"); return false; }
            });
        }
        public async Task<bool> LoadSampleSetParametersAsync()
        {
            if (Paths.SampleSetParameters == default)
            {
                OnInitializerStatusChanged("No path to sample set parameters is set.");
                return false;
            }

            try
            {
                OnInitializerStatusChanged("\nLoading sample set parameters from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(Paths.SampleSetParameters);
                Parameters.SampleSetParameters = JsonConvert.DeserializeObject<SampleSetParameters>(jsonString);
                OnInitializerStatusChanged("Successfully loaded sample set parameters.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> CreateSampleSetAsync()
        {
            if (Parameters.SampleSetParameters == null)
            {
                OnInitializerStatusChanged("No sample set parameters are set.");
                return false;
            }

            try
            {
                var sampleSetSteward = new SampleSetSteward();

                OnInitializerStatusChanged("\nCreating samples, please wait...");
                SampleSet = await sampleSetSteward.CreateSampleSetAsync(Parameters.SampleSetParameters);
                SampleSet.Parameters = (SampleSetParameters)Parameters.SampleSetParameters;    // Use interface in data provider?
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
                await File.WriteAllTextAsync(Paths.InitializedNet, jsonString);
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
                await File.WriteAllTextAsync(Paths.TrainedNet, jsonString);
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
                await File.WriteAllTextAsync(Paths.SampleSet, jsonString);
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
                var jsonString = await File.ReadAllTextAsync(Paths.InitializedNet);

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
                var jsonString = await File.ReadAllTextAsync(Paths.TrainedNet);

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
                var jsonString = await File.ReadAllTextAsync(Paths.SampleSet);

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
