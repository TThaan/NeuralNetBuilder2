using DeepLearningDataProvider;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace NeuralNetBuilder.Builders
{
    public class ParameterBuilder
    {
        #region fields & ctor

        private IEnumerable<ActivationType> activationTypes;
        private IEnumerable<CostType> costTypes;
        private IEnumerable<WeightInitType> weightInitTypes;
        private readonly PathBuilder _paths;
        private readonly Action<string> _onInitializerStatusChanged;

        private ISampleSetParameters sampleSetParameters;
        private INetParameters netParameters;
        private ITrainerParameters trainerParameters;

        public ParameterBuilder(PathBuilder paths, Action<string> onInitializerStatusChanged)
        {
            _paths = paths;
            _onInitializerStatusChanged = onInitializerStatusChanged;
        }

        #endregion

        #region properties

        public IEnumerable<SetName> SampleSetTemplates => new SampleSetSteward().DefaultSampleSetParameters.Keys;
        public ISampleSetParameters SampleSetParameters
        {
            get
            {
                if (sampleSetParameters == null)
                    _onInitializerStatusChanged("SampleSetParameters are null");
                return sampleSetParameters;
            }
            set
            {
                sampleSetParameters = value;
            }
        }
        public INetParameters NetParameters
        {
            get
            {
                if (netParameters == null)
                    _onInitializerStatusChanged("NetParameters are null");
                return netParameters;
            }
            set
            {
                netParameters = value;
            }
        }
        public ITrainerParameters TrainerParameters
        {
            get
            {
                if (trainerParameters == null)
                    _onInitializerStatusChanged("TrainerParameters are null");
                return trainerParameters;
            }
            set
            {
                trainerParameters = value;
            }
        }
        public ObservableCollection<ILayerParameters> LayerParametersCollection
        {
            get
            {
                if (NetParameters.LayerParametersCollection == null)
                    _onInitializerStatusChanged("LayerParametersCollection is null");
                return NetParameters.LayerParametersCollection;
            }
        }

        public IEnumerable<CostType> CostTypes => costTypes ??
            (costTypes = Enum.GetValues(typeof(CostType)).ToList<CostType>());
        public IEnumerable<WeightInitType> WeightInitTypes => weightInitTypes ??
            (weightInitTypes = Enum.GetValues(typeof(WeightInitType)).ToList<WeightInitType>());
        public IEnumerable<ActivationType> ActivationTypes => activationTypes ??
            (activationTypes = Enum.GetValues(typeof(ActivationType)).ToList<ActivationType>());

        #endregion

        #region methods: Change NetParameters

        public bool SetWeightMax_Globally(float weightMax)
        {
            // WeightMax_Global = weightMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = weightMax;
            
            _onInitializerStatusChanged($"Global WeightMax = {weightMax}.");
            return true;
        }
        public bool SetWeightMin_Globally(float weightMin)
        {
            //WeightMin_Global = weightMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = weightMin;

            _onInitializerStatusChanged($"Global WeightMin = {weightMin}.");
            return true;
        }
        public bool SetBiasMax_Globally(float biasMax)
        {
            //BiasMax_Global = biasMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = biasMax;

            _onInitializerStatusChanged($"Global BiasMax = {biasMax}.");
            return true;
        }
        public bool SetBiasMin_Globally(float biasMin)
        {
            //BiasMin_Global = biasMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = biasMin;

            _onInitializerStatusChanged($"Global BiasMin = {biasMin}.");
            return true;
        }
        public bool AddNewLayerAfter(ILayerParameters precedingLayerParameters)
        {
            int precedingLayerIndex;
            if (precedingLayerParameters == null) precedingLayerIndex = -1;
            else precedingLayerIndex = precedingLayerParameters.Id;

            try
            {
                ILayerParameters newLayerParameters = new LayerParameters();

                if (precedingLayerIndex > -1)
                {
                    newLayerParameters.NeuronsPerLayer = precedingLayerParameters.NeuronsPerLayer;
                    newLayerParameters.ActivationType = precedingLayerParameters.ActivationType;
                    newLayerParameters.BiasMin = precedingLayerParameters.BiasMin;
                    newLayerParameters.BiasMax = precedingLayerParameters.BiasMax;
                    newLayerParameters.WeightMin = precedingLayerParameters.WeightMin;
                    newLayerParameters.WeightMax = precedingLayerParameters.WeightMax;
                }

                LayerParametersCollection.Insert(precedingLayerIndex + 1, newLayerParameters);
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"New layer added. (Id = {precedingLayerIndex + 1}.");
            return true;
        }
        public bool DeleteLayer(ILayerParameters layerParameters)
        {
            try
            {
                if (LayerParametersCollection.Count > 2)
                    LayerParametersCollection.Remove(layerParameters);
                else
                {
                    _onInitializerStatusChanged($"You must not delete the last standing layer.");
                    return false;
                }
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
            
            _onInitializerStatusChanged($"Layer deleted. (Id = {layerParameters.Id}.");
            return true;
        }
        public bool MoveLayerLeft(ILayerParameters layerParameters)
        {
            try
            {
                LayerParametersCollection.Move(
                layerParameters.Id, layerParameters.Id > 0 ? layerParameters.Id - 1 : 0);
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Layer moved left. (OldId = {layerParameters.Id}.");
            return true;
        }
        public bool MoveLayerRight(ILayerParameters layerParameters)
        {
            try
            {
                LayerParametersCollection.Move(
                layerParameters.Id, layerParameters.Id < NetParameters.LayerParametersCollection.Count - 1 ? layerParameters.Id + 1 : 0);
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Layer moved right. (OldId = {layerParameters.Id}.");
            return true;
        }

        public bool SetNeuronsAtLayer(int index, int neurons)
        {
            try
            {
                LayerParametersCollection[index].NeuronsPerLayer = neurons;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Amount of neurons in layer {index} = {LayerParametersCollection[index].NeuronsPerLayer}.");
            return true;
        }
        public bool SetActivationTypeAtLayer(int index, ActivationType activationType)
        {
            try
            {
                LayerParametersCollection[index].ActivationType = activationType;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightMax of layer {index} = {LayerParametersCollection[index].WeightMax}.");
            return true;
        }
        public bool SetWeightMaxAtLayer(int index, float weightMax)
        {
            try
            {
                LayerParametersCollection[index].WeightMax = weightMax;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightMax of layer {index} = {LayerParametersCollection[index].WeightMax}.");
            return true;
        }
        public bool SetWeightMinAtLayer(int index, float weightMin)
        {
            try
            {
                LayerParametersCollection[index].WeightMin = weightMin;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightMin of layer {index} = {LayerParametersCollection[index].WeightMin}.");
            return true;
        }
        public bool SetBiasMaxAtLayer(int index, float biasMax)
        {
            try
            {
                LayerParametersCollection[index].BiasMax = biasMax;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"BiasMax of layer {index} = {LayerParametersCollection[index].BiasMax}.");
            return true;
        }
        public bool SetBiasMinAtLayer(int index, float biasMin)
        {
            try
            {
                LayerParametersCollection[index].BiasMin = biasMin;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"BiasMin of layer {index} = {LayerParametersCollection[index].BiasMin}.");
            return true;
        }
        public bool SetWeightInitType(WeightInitType weightInitType)
        {
            try
            {
                NetParameters.WeightInitType = weightInitType;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightInitType = {NetParameters.WeightInitType}.");
            return true;
        }


        #endregion

        #region methods: Change SampleSetParameters

        public bool SetSampleSetName(SetName name)
        {
            try
            {
                SampleSetParameters.Name = name;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Sample Set Name has been set to {SampleSetParameters.Name}.");
            return true;
        }
        public bool SetAmountOfTestingSamples(int testingSamples)
        {
            try
            {
                SampleSetParameters.TestingSamples = testingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Amount of testing samples has been set to {SampleSetParameters.TestingSamples}.");
            return true;
        }
        public bool SetAmountOfTrainingSamples(int trainingSamples)
        {
            try
            {
                SampleSetParameters.TrainingSamples = trainingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Amount of training samples has been set to {SampleSetParameters.TrainingSamples}.");
            return true;
        }
        public bool UseAllAvailableTestingSamples()
        {
            try
            {
                SampleSetParameters.TestingSamples = SampleSetParameters.AllTestingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Amount of testing samples has been set to {SampleSetParameters.AllTestingSamples}.");
            return true;
        }
        public bool UseAllAvailableTrainingSamples()
        {
            try
            {
                SampleSetParameters.TestingSamples = SampleSetParameters.AllTestingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Amount of training samples has been set to {SampleSetParameters.AllTrainingSamples}.");
            return true;
        }

        #endregion

        #region methods: Create, Load & Save

        public void CreateSampleSetParameters()
        {
            SampleSetParameters = new SampleSetParameters();
            _onInitializerStatusChanged("Sample set parameters created.");
        }
        public void CreateNetParameters()
        {
            NetParameters = new NetParameters();
            _onInitializerStatusChanged("Net parameters created.");
        }
        public void CreateTrainerParameters()
        {
            TrainerParameters = new TrainerParameters();
            _onInitializerStatusChanged("Trainer parameters created.");
        }

        public async Task<bool> LoadSampleSetParametersAsync()
        {
            if (_paths.SampleSetParameters == default)
            {
                _onInitializerStatusChanged("No path to sample set parameters is set.");
                return false;
            }

            try
            {
                _onInitializerStatusChanged("\nLoading sample set parameters from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(_paths.SampleSetParameters);
                SampleSetParameters = JsonConvert.DeserializeObject<SampleSetParameters>(jsonString);
                _onInitializerStatusChanged("Successfully loaded sample set parameters.\n");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> LoadNetParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    _onInitializerStatusChanged("\nLoading net parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(_paths.NetParameters);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    NetParameters = sp.NetParameters;
                    _onInitializerStatusChanged("Successfully loaded net parameters.\n");
                    return true;
                }
                catch (Exception e) { _onInitializerStatusChanged($"{e.Message}"); return false; }
            });
        }
        public async Task<bool> LoadTrainerParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    _onInitializerStatusChanged("\nLoading trainer parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(_paths.TrainerParameters);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    TrainerParameters = sp.TrainerParameters;
                    _onInitializerStatusChanged("Successfully loaded trainer parameters.\n");
                    return true;
                }
                catch (Exception e) { _onInitializerStatusChanged($"{e.Message}"); return false; }
            });
        }

        public async Task<bool> SaveSampleSetParametersAsync()
        {
            try
            {
                _onInitializerStatusChanged("\nSaving sample set parameters, please wait...");
                var jsonString = JsonConvert.SerializeObject(SampleSetParameters, Formatting.Indented);
                await File.WriteAllTextAsync(_paths.SampleSetParameters, jsonString);
                _onInitializerStatusChanged("Successfully saved sample set parameters.\n");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveNetParametersAsync()
        {
            try
            {
                _onInitializerStatusChanged("\nSaving net parameters, please wait...");
                var jsonString = JsonConvert.SerializeObject(NetParameters, Formatting.Indented);
                await File.WriteAllTextAsync(_paths.NetParameters, jsonString);
                _onInitializerStatusChanged("Successfully saved net parameters.\n");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainerParametersAsync()
        {
            try
            {
                _onInitializerStatusChanged("\nSaving trainer parameters, please wait...");
                var jsonString = JsonConvert.SerializeObject(TrainerParameters, Formatting.Indented);
                await File.WriteAllTextAsync(_paths.TrainerParameters, jsonString);
                _onInitializerStatusChanged("Successfully saved trainer parameters.\n");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }

        #endregion

        #region helpers

        private void ResetLayersIndeces()
        {
            int i = 0;
            foreach (var layerParam in NetParameters.LayerParametersCollection)
                layerParam.Id = i++;
        }

        #endregion
    }
}
