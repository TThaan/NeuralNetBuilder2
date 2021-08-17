using DeepLearningDataProvider;
using NeuralNetBuilder.Builders;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    // wa: Just test/run a given net?
    // wa: global parameters? Or in NetParameters?
    public class Initializer : INotifyPropertyChanged, INotifyStatusChanged// : NotifierBase
    {
        #region fields & ctor

        private PathBuilder paths;
        private ParameterBuilder parameterBuilder;
        private ISampleSet sampleSet;
        private INet net, trainedNet;
        private ITrainer trainer;

        public Initializer()
        {
            paths = new PathBuilder();                                      // DI?
            parameterBuilder = new ParameterBuilder();                      // DI?
            parameterBuilder.NetParameters = new NetParameters();           // DI?
            parameterBuilder.TrainerParameters = new TrainerParameters();   // DI?

            // Do I need INotifyStatusChanged only in Builders?
            RegisterStatusChanged();

            // Do I need IPropertyStatusChanged only in Builders?
            RegisterPropertyChanged();

            // wa: INCC?

            // Define an uninitialized trainer to enable the client to register events when defining the initializer?
            // Actually: 'OnInitializerStatusChanged(..)' is passed to al builders.
            // So only initializer's event need be registered.
            // But the UI shall have access to yet to be defined properties instead of throwing an exception due to a non existing class.
            trainer = new Trainer();        // DI?
            net = new Net();                // DI?
            sampleSet = new SampleSet();    // DI?
        }

        #region helpers

        private void RegisterStatusChanged()
        {
            parameterBuilder.StatusChanged += InitializerAssistant_StatusChanged;
            parameterBuilder.TrainerParameters.StatusChanged += InitializerAssistant_StatusChanged;
            parameterBuilder.NetParameters.StatusChanged += InitializerAssistant_StatusChanged;
        }
        private void RegisterPropertyChanged()
        {
            parameterBuilder.NetParameters.PropertyChanged += InitializerAssistant_PropertyChanged;
            parameterBuilder.TrainerParameters.PropertyChanged += InitializerAssistant_PropertyChanged;
            parameterBuilder.PropertyChanged += InitializerAssistant_PropertyChanged;
        }

        #endregion

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
        public string LogName { get; set; } //?

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
                OnStatusChanged($"\n            Training, please wait...\n");
                await Trainer.TrainAsync(shuffle, IsLogged ? Paths.Log : default);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
                TrainedNet = Trainer.TrainedNet?.GetCopy();
                OnStatusChanged($"\n            Finished training.\n");
                return true;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }
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
                    OnStatusChanged("You need a sample set (incl a train set and a test set) to append a default labels layer!");
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
                    OnStatusChanged("Creating net, please wait...");
                    Net = NetFactory.CreateNet(ParameterBuilder.NetParameters);  // as async method?
                    OnStatusChanged("Successfully created net.");
                    return true;
                }
                catch (Exception e) { OnStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> CreateTrainerAsync()
        {
            if (ParameterBuilder.TrainerParameters == null)
            {
                OnStatusChanged("You need trainer parameters to create the trainer!");
                return false;
            }

            // Attach net & sampleset to trainer after initializing?
            if (Net == null)
            {
                OnStatusChanged("You need to create the net to create the trainer!");
                return false;
            }
            if (sampleSet == null)
            {
                OnStatusChanged("You need a sample set to create the trainer!");
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    OnStatusChanged("Createing trainer, please wait...");
                    Trainer.Epochs = ParameterBuilder.TrainerParameters.Epochs;
                    Trainer.LearningRate = ParameterBuilder.TrainerParameters.LearningRate;
                    Trainer.LearningRateChange = ParameterBuilder.TrainerParameters.LearningRateChange;
                    Trainer.CostType = ParameterBuilder.TrainerParameters.CostType;
                    Trainer.OriginalNet = Net.GetCopy();
                    Trainer.SampleSet = sampleSet;
                    OnStatusChanged("Successfully created trainer.");
                    return true;
                }
                catch (Exception e) { OnStatusChanged(e.Message); return false; }
            });
        }
        public async Task<bool> SaveInitializedNetAsync()
        {
            try
            {
                OnStatusChanged("Saving initialized net, please wait...");

                var jsonString = JsonConvert.SerializeObject(Net, Formatting.Indented);
                await File.AppendAllTextAsync(Paths.InitializedNet, jsonString);

                OnStatusChanged("Successfully saved initialized net.");
                return true;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainedNetAsync()
        {
            try
            {
                OnStatusChanged("Saving trained net, please wait...");

                var jsonString = JsonConvert.SerializeObject(TrainedNet, Formatting.Indented);
                await File.AppendAllTextAsync(Paths.TrainedNet, jsonString);

                OnStatusChanged("Successfully saved trained net.");
                return true;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadNetAsync()
        {
            try
            {
                OnStatusChanged("Loading initialized net from file, please wait...");
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
                OnStatusChanged("Successfully loaded initialized net.");
                return true;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadTrainedNetAsync()
        {
            try
            {
                OnStatusChanged("Loading trained net from file, please wait...");
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
                OnStatusChanged("Successfully loaded trained net.");
                return true;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }
        }

        // Set IsLogged and Logname ?

        #endregion

        #region INotifyPropertyChanged

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (propertyChanged == null || !propertyChanged.GetInvocationList().Contains(value))
                    propertyChanged += value;
                // else Log when debugging.

            }
            remove { propertyChanged -= value; }
        }
        public bool IsPropertyChangedNull => propertyChanged == null;
        public void InitializerAssistant_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParameterBuilder.NetParameters))
                RegisterPropertyChanged();
            // else if (e.PropertyName == nameof(ParameterBuilder.NetParameters.LayerParametersCollection))

                OnPropertyChanged(e.PropertyName);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyStatusChanged

        private event StatusChangedEventHandler statusChanged;
        public event StatusChangedEventHandler StatusChanged
        {
            add
            {
                if (statusChanged == null || !statusChanged.GetInvocationList().Contains(value))
                    statusChanged += value;
                // else Log when debugging.

            }
            remove { statusChanged -= value; }
        }
        public bool IsStatusChangedNull => statusChanged == null;
        public void InitializerAssistant_StatusChanged(object sender, StatusChangedEventArgs e)
        {
                OnStatusChanged(e.Info);
        }
        protected virtual void OnStatusChanged([CallerMemberName] string propertyName = null)
        {
            statusChanged?.Invoke(this, new StatusChangedEventArgs(propertyName));
        }

        #endregion
    }
}
