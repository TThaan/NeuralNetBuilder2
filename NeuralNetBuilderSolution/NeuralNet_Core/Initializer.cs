using DeepLearningDataProvider;
using DeepLearningDataProvider.SampleSetHelpers;
using NeuralNet_Core.Builders;
using NeuralNet_Core.FactoriesAndParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace NeuralNet_Core
{
    // Task: Make redundant!
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
            SampleSet = new SampleSet();                                    // DI?

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
            // SampleSet?
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

        // as ext meth!

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
