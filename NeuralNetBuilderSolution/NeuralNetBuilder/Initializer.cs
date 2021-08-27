using DeepLearningDataProvider;
using DeepLearningDataProvider.SampleSetHelpers;
using NeuralNetBuilder.Builders;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    // wa: Just test/run a given net?
    // wa: global parameters? Or in NetParameters?

    public class Initializer : NotificationChangedBase
    {
        #region fields & ctor

        public Initializer()
        {
            ParameterBuilder = new ParameterBuilder();                      // DI?
            ParameterBuilder.NetParameters = new NetParameters();           // DI?
            ParameterBuilder.TrainerParameters = new TrainerParameters();   // DI?
            PathBuilder = new PathBuilder();                                // DI?

            Trainer = new Trainer();                                        // DI?
            Net = new Net() { Layers = new ILayer[0] };                     // DI?
            // SampleSet = new SampleSet();        // DI?
            // -> Inject factories for Net, Trainer and SampleSet? And ImpExport instance?

            RegisterPropertyChangedHandlers();

            Notification = "Initializer created.";
        }

        #region helpers

        private void RegisterPropertyChangedHandlers()
        {
            PathBuilder.PropertyChanged += Builder_PropertyChanged;
            ParameterBuilder.PropertyChanged += Builder_PropertyChanged;
            ParameterBuilder.NetParameters.PropertyChanged += Builder_PropertyChanged;
            ParameterBuilder.TrainerParameters.PropertyChanged += Builder_PropertyChanged;
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

            Notification = $"\n            Training, please wait...\n";
            await Trainer.TrainAsync(shuffle, logFileName);   // Pass in the net here?  // Should epochs (all trainerparameters) already be in the trainer?
            TrainedNet = Trainer.TrainedNet?.GetCopy();
            Notification = $"\n            Finished training.\n";
        }
        //public void AppendLabelsLayerToNetParameters()
        //{
        //    if (SampleSet == null || SampleSet.TrainSet == null || SampleSet.TestSet == null)
        //        Notification = "You need a sample set (incl a train set and a test set) to append a default labels layer!";

        //    var labelsLayer = new LayerParameters
        //    {
        //        Id = ParameterBuilder.NetParameters.LayerParametersCollection.Count,
        //        NeuronsPerLayer = SampleSet.Targets.Count,
        //        ActivationType = ActivationType.Tanh,
        //        WeightMin = -1,
        //        WeightMax = 1,
        //        BiasMin = 0,
        //        BiasMax = 0,
        //    };

        //    ParameterBuilder.NetParameters.LayerParametersCollection.Add(labelsLayer);
        //}
        public async Task SaveInitializedNetAsync(string fileName)
        {
            Notification = "Saving initialized net, please wait...";

            var jsonString = JsonConvert.SerializeObject(Net, Formatting.Indented);
            await File.AppendAllTextAsync(fileName, jsonString);

            Notification = "Successfully saved initialized net."; // Not awaiting nested tasks result!
        }
        public async Task SaveTrainedNetAsync(string fileName)
        {
            Notification = "Saving trained net, please wait...";

            var jsonString = JsonConvert.SerializeObject(TrainedNet, Formatting.Indented);
            await File.AppendAllTextAsync(fileName, jsonString);

            Notification = "Successfully saved trained net.";     // Not awaiting nested tasks result!
        }
        public async Task LoadNetAsync(string fileName)
        {
            Notification = "Loading initialized net from file, please wait...";
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
            Notification = "Successfully loaded initialized net.";
        }
        public async Task LoadTrainedNetAsync(string fileName)
        {
            Notification = "Loading trained net from file, please wait...";
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
            Notification = "Successfully loaded trained net.";
        }
        public async Task LoadSampleSetAsync(string fileName, decimal split, int labelColumn, int[] ignoredColumns)
        {
            if (split <= 0 || split >= 1)
                throw new ArgumentException("The fraction of test samples must be (exclusively) between 0 and 1");

            Notification = "Loading sample set from file, please wait...";
            SampleSet = await SaveAndLoad.LoadSampleSetAsync(fileName, split, labelColumn, ignoredColumns); // Factory..
            Notification = "Successfully loaded sample set.";
        }
        public async Task UnloadSampleSetAsync()
        {
            Notification = "Unloading sample set, please wait...";
            await SaveAndLoad.UnloadSampleSetAsync(SampleSet);
            OnPropertyChanged(nameof(SampleSet));

            Notification = "Successfully unloaded sample set.";

        }
        public async Task SaveSampleSetAsync(string fileName, bool overWriteExistingFile = true)
        {
            if (SampleSet == null)
                throw new ArgumentException("You have no sample set to be saved!");

            Notification = "Loading sample set from file, please wait...";
            await SaveAndLoad.SaveSampleSetAsync(SampleSet, fileName, overWriteExistingFile);
            Notification = "Successfully loaded sample set.";
        }

        // Set IsLogged and Logname ?

        #endregion

        #region event handling methods

        public void Builder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Notification))
            {
                Notification = ((INotificationChanged)sender).Notification;
                return;
            }
            else 
            if (e.PropertyName == nameof(ParameterBuilder.NetParameters) ||
                e.PropertyName == nameof(ParameterBuilder.TrainerParameters))
                RegisterPropertyChangedHandlers();

            OnPropertyChanged(e.PropertyName);
        }

        #endregion
    }
}
