using DeepLearningDataProvider;
using DeepLearningDataProvider.SampleSetHelpers;
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
using System.Windows;
using System.Threading;

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

            // Trainer = new Trainer();            // DI?
            // Net = new Net() { Layers = new ILayer[0] };                    // DI?
            // SampleSet = new SampleSet();        // DI?
            // -> Inject factories for Net, Trainer and SampleSet? And ImpExport instance?

            RegisterPropertyChanged();

            Status = "Initializer created.";
        }

        #region helpers

        private void RegisterPropertyChanged()
        {
            PathBuilder.PropertyChanged += InitializerAssistant_PropertyChanged;
            ParameterBuilder.PropertyChanged += InitializerAssistant_PropertyChanged;
            ParameterBuilder.NetParameters.PropertyChanged += InitializerAssistant_PropertyChanged;
            ParameterBuilder.TrainerParameters.PropertyChanged += InitializerAssistant_PropertyChanged;
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

        // Really some values from parameters and some from fields?
        public async Task TrainAsync(bool shuffle, string logFileName = "")
        {
            if (Trainer == null)
                throw new ArgumentException("\nYou need a trainer to start training!");
            if (Net == null)
                throw new ArgumentException("\nYou need a net to start training!");
            if (SampleSet == null)
                throw new ArgumentException("\nYou need a sample set to start training!");

            if (shuffle)
                await SampleSet.TrainSet.ShuffleAsync();

            Status = $"\n            Training, please wait...\n";
            await Trainer.TrainAsync(shuffle, logFileName);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
            TrainedNet = Trainer.TrainedNet?.GetCopy();
            Status = $"\n            Finished training.\n";
        }
        public async Task CreateNetAsync(bool appendLabelsLayer = false)
        {
            if (ParameterBuilder.NetParameters == null)
                throw new ArgumentException("You need net parameters to create the net!");

            if (appendLabelsLayer)
            {
                if (SampleSet == null || SampleSet.TrainSet == null || SampleSet.TestSet == null)
                    Status = "You need a sample set (incl a train set and a test set) to append a default labels layer!";

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

            Status = "Creating net, please wait...";
            Net = await NetFactory.CreateNetAsync(ParameterBuilder.NetParameters);  // Get Factory via DI?
            Status = "Successfully created net.";
        }
        public void CreateTrainer()
        {
            if (ParameterBuilder.TrainerParameters == null)
                throw new ArgumentException("You need trainer parameters to create the trainer!");
            if (Net == null)
                throw new ArgumentException("You need to create the net to create the trainer!");
            if (SampleSet == null)
                throw new ArgumentException("You need a sample set to create the trainer!");

            Status = "Createing trainer, please wait...";
            Trainer = new Trainer();    // Use Factory?
            Trainer.Epochs = ParameterBuilder.TrainerParameters.Epochs;
            Trainer.LearningRate = ParameterBuilder.TrainerParameters.LearningRate;
            Trainer.LearningRateChange = ParameterBuilder.TrainerParameters.LearningRateChange;
            Trainer.CostType = ParameterBuilder.TrainerParameters.CostType;
            Trainer.OriginalNet = Net.GetCopy();
            Trainer.SampleSet = SampleSet;
            Status = "Successfully created trainer.";
        }
        public async Task SaveInitializedNetAsync(string fileName)
        {
            Status = "Saving initialized net, please wait...";

            var jsonString = JsonConvert.SerializeObject(Net, Formatting.Indented);
            await File.AppendAllTextAsync(fileName, jsonString);

            Status = "Successfully saved initialized net."; // Not awaiting nested tasks result!
        }
        public async Task SaveTrainedNetAsync(string fileName)
        {
            Status = "Saving trained net, please wait...";

            var jsonString = JsonConvert.SerializeObject(TrainedNet, Formatting.Indented);
            await File.AppendAllTextAsync(fileName, jsonString);

            Status = "Successfully saved trained net.";     // Not awaiting nested tasks result!
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
        public async Task LoadSampleSetAsync(string fileName, decimal split, int labelColumn, int[] ignoredColumns)
        {
            if (split <= 0 || split >= 1)
                throw new ArgumentException("The fraction of test samples must be (exclusively) between 0 and 1 (betwwen 0 and 100 in percent)");

            Status = "Loading sample set from file, please wait...";
            SampleSet = await SaveAndLoad.LoadSampleSetAsync(fileName, split, labelColumn, ignoredColumns); // Factory..
            Status = "Successfully loaded sample set.";
        }
        public async Task UnloadSampleSetAsync()
        {
            Status = "Unloading sample set, please wait...";
            await SaveAndLoad.UnloadSampleSetAsync(SampleSet);
            OnPropertyChanged(nameof(SampleSet));

            Status = "Successfully unloaded sample set.";

        }
        public async Task SaveSampleSetAsync(string fileName, bool overWriteExistingFile = true)
        {
            if (SampleSet == null)
                throw new ArgumentException("You have no sample set to be saved!");

            Status = "Loading sample set from file, please wait...";
            await SaveAndLoad.SaveSampleSetAsync(SampleSet, fileName, overWriteExistingFile);
            Status = "Successfully loaded sample set.";
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
