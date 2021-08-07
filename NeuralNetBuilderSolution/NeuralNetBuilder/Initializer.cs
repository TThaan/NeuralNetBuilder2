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
        private ParameterBuilder parameterBuilder;
        private ISampleSet sampleSet;
        private INet net, trainedNet;
        private ITrainer trainer;

        public Initializer()
        {
            paths = new PathBuilder(OnInitializerStatusChanged);                        // via DC?
            parameterBuilder = new ParameterBuilder(paths, OnInitializerStatusChanged); // via DC?
        }

        #endregion

        #region properties

        public PathBuilder Paths
        {
            get
            {
                //if (paths == null)
                //    OnInitializerStatusChanged("Paths are null");
                return paths;
            }
        }
        public ParameterBuilder ParameterBuilder
        {
            get
            {
                //if (parameterBuilder == null)
                //    OnInitializerStatusChanged("ParameterBuilder is null");
                return parameterBuilder;
            }
        }
        public ISampleSet SampleSet
        {
            get
            {
                //if (sampleSet == null)
                //    OnInitializerStatusChanged("SampleSet is null");
                return sampleSet;
            }
            set { sampleSet = value; }
        }
        public INet Net
        {
            get
            {
                //if (net == null)
                //    OnInitializerStatusChanged("Net is null");
                return net;
            }
            set { net = value; }
        }
        public INet TrainedNet
        {
            get
            {
                //if (trainedNet == null)
                //    OnInitializerStatusChanged("TrainedNet is null");
                return trainedNet;
            }
            set { trainedNet = value; }
        }
        public ITrainer Trainer
        {
            get
            {
                //if (trainer == null)
                //    OnInitializerStatusChanged("Trainer is null");
                return trainer;
            }
            set { trainer = value; }
        }
        public bool IsLogged { get; set; }

        #endregion

        #region methods

        public async Task<bool> TrainAsync(ISampleSet sampleSet, bool shuffle = false)
        {
            if (Trainer == null)
                throw new ArgumentException("\nYou need a trainer to start training!");
            if (Net == null)
                throw new ArgumentException("\nYou need a net to start training!");
            if (sampleSet == null)
                throw new ArgumentException("\nYou need a sample set to start training!");

            if (shuffle)
                await sampleSet.TrainSet.ShuffleAsync();

            try
            {
                OnInitializerStatusChanged($"\n            Training, please wait...\n");
                await Trainer.Train(shuffle, IsLogged ? Paths.Log : default);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
                TrainedNet = Trainer.TrainedNet?.GetCopy();
                OnInitializerStatusChanged($"\n            Finished training.\n");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        /// <summary>
        /// Valid parameters: Undefined, AppendLabelsLayer
        /// </summary>
        public async Task<bool> CreateNetAsync(bool appendLabelsLayer = false)
        {
            if (ParameterBuilder.NetParameters == null)
                throw new ArgumentException("You need net parameters to create the net!");

            if (appendLabelsLayer)
            {
                if (SampleSet == null || SampleSet.TrainSet == null || SampleSet.TestSet == null)
                {
                    OnInitializerStatusChanged("You need a sample set (incl a train set and a test set) to append a default labels layer!");
                    return false;
                }

                // to NetParametersFactory? Do I want labels layer parameters (in net parameters) or only the layer in the net?
                var labelsLayer = new LayerParameters
                {
                    Id = ParameterBuilder.NetParameters.LayerParametersCollection.Count,
                    NeuronsPerLayer = SampleSet.Targets.Count,
                    ActivationType = ActivationType.Tanh,
                    WeightMin = -1,
                    WeightMax = 1,
                    BiasMin = 0,
                    BiasMax = 0,
                };

                ParameterBuilder.NetParameters.LayerParametersCollection.Add(labelsLayer);
            }
            
            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("Creating net, please wait...");
                    Net = NetFactory.CreateNet(ParameterBuilder.NetParameters);  // as async method?
                    OnInitializerStatusChanged("Successfully created net.");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> CreateTrainerAsync()
        {
            if (ParameterBuilder.TrainerParameters == null)
            {
                OnInitializerStatusChanged("You need trainer parameters to create the trainer!");
                return false;
            }

            // Attach net & sampleset to trainer after initializing?
            if (Net == null)
            {
                OnInitializerStatusChanged("You need to create the net to create the trainer!");
                return false;
            }
            if (sampleSet == null)
            {
                OnInitializerStatusChanged("You need a sample set to create the trainer!");
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    OnInitializerStatusChanged("Createing trainer, please wait...");
                    Trainer = new Trainer(ParameterBuilder.TrainerParameters);
                    Trainer.OriginalNet = Net.GetCopy();
                    Trainer.SampleSet = sampleSet;
                    OnInitializerStatusChanged("Successfully created trainer.");
                    return true;
                }
                catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> SaveInitializedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("Saving initialized net, please wait...");

                var jsonString = JsonConvert.SerializeObject(Net, Formatting.Indented);
                await File.AppendAllTextAsync(Paths.InitializedNet, jsonString);

                OnInitializerStatusChanged("Successfully saved initialized net.");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("Saving trained net, please wait...");

                var jsonString = JsonConvert.SerializeObject(TrainedNet, Formatting.Indented);
                await File.AppendAllTextAsync(Paths.TrainedNet, jsonString);

                OnInitializerStatusChanged("Successfully saved trained net.");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("Loading initialized net from file, please wait...");
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
                OnInitializerStatusChanged("Successfully loaded initialized net.");
                return true;
            }
            catch (Exception e) { OnInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadTrainedNetAsync()
        {
            try
            {
                OnInitializerStatusChanged("Loading trained net from file, please wait...");
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
                OnInitializerStatusChanged("Successfully loaded trained net.");
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
