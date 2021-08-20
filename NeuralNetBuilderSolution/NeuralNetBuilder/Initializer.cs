using DeepLearningDataProvider;
using DeepLearningDataProvider.SampleSetExtensionMethods;
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
using static NeuralNetBuilder.Helpers;

namespace NeuralNetBuilder
{
    // wa: Just test/run a given net?
    // wa: global parameters? Or in NetParameters?

    // Remove INSC and implement property 'Status' like in ParameterBuilder?
    public class Initializer : INotifyPropertyChanged//, INotifyStatusChanged
    {
        #region fields & ctor

        private string status;

        public Initializer()
        {
            ParameterBuilder = new ParameterBuilder();                      // DI?
            ParameterBuilder.NetParameters = new NetParameters();           // DI?
            ParameterBuilder.TrainerParameters = new TrainerParameters();   // DI?
            PathBuilder = new PathBuilder();                                // DI?

            Trainer = new Trainer();            // DI?
            Net = new Net();                    // DI?
            SampleSet = new SampleSet();        // DI?

            RegisterPropertyChanged();

            Status = "Initializer created.";
        }

        #region helpers

        private void RegisterPropertyChanged()
        {
            ParameterBuilder.NetParameters.PropertyChanged += InitializerAssistant_PropertyChanged;
            ParameterBuilder.TrainerParameters.PropertyChanged += InitializerAssistant_PropertyChanged;
            ParameterBuilder.PropertyChanged += InitializerAssistant_PropertyChanged;
            PathBuilder.PropertyChanged += InitializerAssistant_PropertyChanged;
        }

        #endregion

        #endregion

        #region properties

        public PathBuilder PathBuilder { get; }
        public ParameterBuilder ParameterBuilder { get; }
        public ISampleSet SampleSet { get; set; }
        public INet Net { get; set; }
        public INet TrainedNet { get; set; }
        public ITrainer Trainer { get; set; }
        // public bool IsLogged { get; set; }  // INPC
        // public string LogName { get; set; } //?

        public string Status
        {
            get { return status; }
            set
            {
                // No equality check due to potentially reapeated statuses.
                status = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region methods

        public async Task TrainAsync(ISampleSet sampleSet, bool shuffle, string logFileName = "")
        {
            if (Trainer == null)
                ThrowFormattedArgumentException("\nYou need a trainer to start training!");
            if (Net == null)
                ThrowFormattedArgumentException("\nYou need a net to start training!");
            if (sampleSet == null)
                ThrowFormattedArgumentException("\nYou need a sample set to start training!");

            if (shuffle)
                await sampleSet.TrainSet.ShuffleAsync();

            Status = $"\n            Training, please wait...\n";
            await Trainer.TrainAsync(shuffle, logFileName);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
            TrainedNet = Trainer.TrainedNet?.GetCopy();
            Status = $"\n            Finished training.\n";
        }
        public async Task CreateNetAsync(bool appendLabelsLayer = false)
        {
            if (ParameterBuilder.NetParameters == null)
                ThrowFormattedArgumentException("You need net parameters to create the net!");

            if (appendLabelsLayer)
            {
                if (SampleSet == null || SampleSet.TrainSet == null || SampleSet.TestSet == null)
                    Status = "You need a sample set (incl a train set and a test set) to append a default labels layer!";

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
            
            await Task.Run(() =>
            {
                Status = "Creating net, please wait...";
                Net = NetFactory.CreateNet(ParameterBuilder.NetParameters);  // as async method?
                Status = "Successfully created net.";
            });
        }
        public async Task CreateTrainerAsync()
        {
            if (ParameterBuilder.TrainerParameters == null)
                ThrowFormattedArgumentException("You need trainer parameters to create the trainer!");
            if (Net == null)
                ThrowFormattedArgumentException("You need to create the net to create the trainer!");
            if (SampleSet == null)
                ThrowFormattedArgumentException("You need a sample set to create the trainer!");

            await Task.Run(() =>
            {
                Status = "Createing trainer, please wait...";
                Trainer.Epochs = ParameterBuilder.TrainerParameters.Epochs;
                Trainer.LearningRate = ParameterBuilder.TrainerParameters.LearningRate;
                Trainer.LearningRateChange = ParameterBuilder.TrainerParameters.LearningRateChange;
                Trainer.CostType = ParameterBuilder.TrainerParameters.CostType;
                Trainer.OriginalNet = Net.GetCopy();
                Trainer.SampleSet = SampleSet;
                Status = "Successfully created trainer.";
            });
        }
        public async Task SaveInitializedNetAsync(string fileName)
        {
            Status = "Saving initialized net, please wait...";

            var jsonString = JsonConvert.SerializeObject(Net, Formatting.Indented);
            await File.AppendAllTextAsync(fileName, jsonString);

            Status = "Successfully saved initialized net.";
        }
        public async Task SaveTrainedNetAsync(string fileName)
        {
            Status = "Saving trained net, please wait...";

            var jsonString = JsonConvert.SerializeObject(TrainedNet, Formatting.Indented);
            await File.AppendAllTextAsync(fileName, jsonString);

            Status = "Successfully saved trained net.";
        }
        public async Task LoadNetAsync(string fileName)
        {
            Status = "Loading initialized net from file, please wait...";
            var jsonString = await File.ReadAllTextAsync(fileName);

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
            Status = "Successfully loaded initialized net.";
        }
        public async Task LoadTrainedNetAsync(string fileName)
        {
            Status = "Loading trained net from file, please wait...";
            var jsonString = await File.ReadAllTextAsync(fileName);

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
            Status = "Successfully loaded trained net.";
        }
        /// <summary>
        /// This method provides a notification after the sample set is loaded completely.
        /// </summary>
        //public async Task LoadSampleSetAsync(string fileName, decimal split, int labelColumn, int[] ignoredColumns)
        //{
        //    await SampleSet.LoadAsync(fileName, split, labelColumn, ignoredColumns);
        //    OnPropertyChanged(nameof(SampleSet));
        //}

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
            if (e.PropertyName == nameof(Status))
            {
                // Better use interface including prop 'Status' for Status holders? Or bring back separate StatusChangedEvent.
                Status = ((dynamic)sender).Status;
                return;
            }
            else if (e.PropertyName == nameof(ParameterBuilder.NetParameters) ||
                e.PropertyName == nameof(ParameterBuilder.TrainerParameters))
                RegisterPropertyChanged();
            
            OnPropertyChanged(e.PropertyName);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        //#region INotifyStatusChanged

        //private event StatusChangedEventHandler statusChanged;
        //public event StatusChangedEventHandler StatusChanged
        //{
        //    add
        //    {
        //        if (statusChanged == null || !statusChanged.GetInvocationList().Contains(value))
        //            statusChanged += value;
        //        // else Log when debugging.

        //    }
        //    remove { statusChanged -= value; }
        //}
        //public bool IsStatusChangedNull => statusChanged == null;
        //public void InitializerAssistant_StatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //        OnStatusChanged(e.Info);
        //}
        //protected virtual void OnStatusChanged([CallerMemberName] string propertyName = null)
        //{
        //    statusChanged?.Invoke(this, new StatusChangedEventArgs(propertyName));
        //}

        //#endregion
    }
}
