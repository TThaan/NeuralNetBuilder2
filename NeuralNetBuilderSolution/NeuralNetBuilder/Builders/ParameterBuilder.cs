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

        public Dictionary<string, string> ParameterNames { get; set; } = new Dictionary<string, string>
        {
            // NetParameters
            [nameof(INetParameters.WeightInitType)] = "wInit", 
            [nameof(SetWeightMin_Globally)] = "wMin global", 
            [nameof(SetWeightMax_Globally)] = "wMax global", 
            [nameof(SetBiasMin_Globally)] = "bMin global",
            [nameof(SetBiasMax_Globally)] = "bMax global",
            [nameof(AddNewLayerAfter)] = "layer add",
            [nameof(DeleteLayer)] = "layer del",
            [nameof(MoveLayerLeft)] = "layer left",
            [nameof(MoveLayerRight)] = "layer right",
            [nameof(SetNeuronsAtLayer)] = "layer neurons",
            [nameof(SetActivationTypeAtLayer)] = "layer act",
            [nameof(SetWeightMaxAtLayer)] = "layer wMax",
            [nameof(SetWeightMinAtLayer)] = "layer wMin",
            [nameof(SetBiasMaxAtLayer)] = "layer bMax",
            [nameof(SetBiasMinAtLayer)] = "layer bMin",
            [nameof(SetWeightInitType)] = "layer wInit"
        };

        public IEnumerable<CostType> CostTypes => costTypes ??
            (costTypes = Enum.GetValues(typeof(CostType)).ToList<CostType>());
        public IEnumerable<WeightInitType> WeightInitTypes => weightInitTypes ??
            (weightInitTypes = Enum.GetValues(typeof(WeightInitType)).ToList<WeightInitType>());
        public IEnumerable<ActivationType> ActivationTypes => activationTypes ??
            (activationTypes = Enum.GetValues(typeof(ActivationType)).ToList<ActivationType>());

        #endregion

        #region methods: Change NetParameters

        public void ChangeANetParameter(string parameterName, string parameterValue)
        {
            try
            {
                switch (parameterName)
                {
                    case nameof(NetParameters.WeightInitType):
                        SetWeightInitType(parameterValue.ToEnum<WeightInitType>());
                        break;
                    case nameof(SetWeightMin_Globally):
                        SetWeightMin_Globally(float.Parse(parameterValue));
                        break;
                    case nameof(SetWeightMax_Globally):
                        SetWeightMax_Globally(float.Parse(parameterValue));
                        break;
                    case nameof(SetBiasMin_Globally):
                        SetBiasMin_Globally(float.Parse(parameterValue));
                        break;
                    case nameof(SetBiasMax_Globally):
                        SetBiasMax_Globally(float.Parse(parameterValue));
                        break;
                }
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); }

            //PropertyInfo pi = NetParameters.GetType().GetProperty(parameterName);
            //pi.SetValue(NetParameters, parameterValue);
        }
        public void ChangeALayerParameter(string id, string parameterName, string parameterValue)
        {
            try
            {
                int.TryParse(id, out int layerId);

                if (parameterName == ParameterNames[nameof(AddNewLayerAfter)])
                    AddNewLayerAfter(layerId);
                if (parameterName == ParameterNames[nameof(DeleteLayer)])
                    DeleteLayer(layerId);
                if (parameterName == ParameterNames[nameof(MoveLayerLeft)])
                    MoveLayerLeft(layerId);
                if (parameterName == ParameterNames[nameof(MoveLayerRight)])
                    MoveLayerRight(layerId);

                if (parameterName == ParameterNames[nameof(SetNeuronsAtLayer)])
                    SetNeuronsAtLayer(layerId, int.Parse(parameterValue));
                if (parameterName == ParameterNames[nameof(SetActivationTypeAtLayer)])
                    SetActivationTypeAtLayer(layerId, parameterValue.ToEnum<ActivationType>());
                if (parameterName == ParameterNames[nameof(SetWeightMaxAtLayer)])
                    SetWeightMaxAtLayer(layerId, float.Parse(parameterValue));
                if (parameterName == ParameterNames[nameof(SetWeightMinAtLayer)])
                    SetWeightMinAtLayer(layerId, float.Parse(parameterValue));
                if (parameterName == ParameterNames[nameof(SetBiasMaxAtLayer)])
                    SetBiasMaxAtLayer(layerId, float.Parse(parameterValue));
                if (parameterName == ParameterNames[nameof(SetBiasMinAtLayer)])
                    SetBiasMinAtLayer(layerId, float.Parse(parameterValue));

            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); }

            //PropertyInfo pi = NetParameters.GetType().GetProperty(parameterName);
            //pi.SetValue(NetParameters, parameterValue);
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

        public bool AddNewLayerAfter(int precedingLayerIndex)
        {
            //if (precedingLayerParameters == null) precedingLayerIndex = -1;
            //else precedingLayerIndex = precedingLayerParameters.Id;

            ILayerParameters precedingLayerParameters;

            try
            {
                precedingLayerParameters = LayerParametersCollection[precedingLayerIndex];
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
        public bool DeleteLayer(int layerId)
        {
            try
            {
                if (LayerParametersCollection.Count > 2)
                    LayerParametersCollection.Remove(LayerParametersCollection[layerId]);
                else
                {
                    _onInitializerStatusChanged($"You must not delete the last standing layer.");
                    return false;
                }
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
            
            _onInitializerStatusChanged($"Layer deleted. (Id = {layerId}.");
            return true;
        }
        public bool MoveLayerLeft(int layerId)
        {
            try
            {
                LayerParametersCollection.Move(
                layerId, layerId > 0 ? layerId - 1 : 0);
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Layer moved left. (OldId = {layerId}.");
            return true;
        }
        public bool MoveLayerRight(int layerId)
        {
            try
            {
                LayerParametersCollection.Move(
                layerId, layerId < NetParameters.LayerParametersCollection.Count - 1 ? layerId + 1 : 0);
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Layer moved right. (OldId = {layerId}.");
            return true;
        }

        public bool SetNeuronsAtLayer(int layerId, int neurons)
        {
            try
            {
                LayerParametersCollection[layerId].NeuronsPerLayer = neurons;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Amount of neurons in layer {layerId} = {LayerParametersCollection[layerId].NeuronsPerLayer}.");
            return true;
        }
        public bool SetActivationTypeAtLayer(int layerId, ActivationType activationType)
        {
            try
            {
                LayerParametersCollection[layerId].ActivationType = activationType;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
            return true;
        }
        public bool SetWeightMaxAtLayer(int layerId, float weightMax)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMax = weightMax;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
            return true;
        }
        public bool SetWeightMinAtLayer(int layerId, float weightMin)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMin = weightMin;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"WeightMin of layer {layerId} = {LayerParametersCollection[layerId].WeightMin}.");
            return true;
        }
        public bool SetBiasMaxAtLayer(int layerId, float biasMax)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMax = biasMax;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"BiasMax of layer {layerId} = {LayerParametersCollection[layerId].BiasMax}.");
            return true;
        }
        public bool SetBiasMinAtLayer(int layerId, float biasMin)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMin = biasMin;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"BiasMin of layer {layerId} = {LayerParametersCollection[layerId].BiasMin}.");
            return true;
        }

        #endregion

        #region methods: Change SampleSetParameters

        public void ChangeASampleSetParameter(string parameterName, string parameterValue)
        {
            try
            {
                switch (parameterName)
                {
                    case nameof(SampleSetParameters.Name):
                        SetSampleSetName(parameterValue.ToEnum<SetName>());
                        break;
                    case nameof(SampleSetParameters.TestingSamples):
                        SetAmountOfTestingSamples(int.Parse(parameterValue));
                        break;
                    case nameof(SampleSetParameters.TrainingSamples):
                        SetAmountOfTrainingSamples(int.Parse(parameterValue));
                        break;
                    case nameof(SampleSetParameters.InputDistortion):
                        SetInputDistortion(int.Parse(parameterValue));
                        break;
                    case nameof(SampleSetParameters.TargetTolerance):
                        SetTargetTolerance(float.Parse(parameterValue));
                        break;
                }
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); }
        }

        public bool SetSampleSetName(SetName name)
        {
            try
            {
                SampleSetParameters.Name = name;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.Name} has been set to {SampleSetParameters.Name}.");
            return true;
        }
        public bool SetAmountOfTestingSamples(int testingSamples)
        {
            try
            {
                SampleSetParameters.TestingSamples = testingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.TestingSamples} has been set to {SampleSetParameters.TestingSamples}.");
            return true;
        }
        public bool SetAmountOfTrainingSamples(int trainingSamples)
        {
            try
            {
                SampleSetParameters.TrainingSamples = trainingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.TrainingSamples} has been set to {SampleSetParameters.TrainingSamples}.");
            return true;
        }
        public bool SetInputDistortion(int targetTolerance)
        {
            try
            {
                SampleSetParameters.InputDistortion = targetTolerance;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.InputDistortion} of samples has been set to {SampleSetParameters.InputDistortion}.");
            return true;
        }
        public bool SetTargetTolerance(float targetTolerance)
        {
            try
            {
                SampleSetParameters.TargetTolerance = targetTolerance;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.TargetTolerance} of samples has been set to {SampleSetParameters.TargetTolerance}.");
            return true;
        }
        public bool UseAllAvailableTestingSamples()
        {
            try
            {
                SampleSetParameters.TestingSamples = SampleSetParameters.AllTestingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.TestingSamples} has been set to {SampleSetParameters.AllTestingSamples}.");
            return true;
        }
        public bool UseAllAvailableTrainingSamples()
        {
            try
            {
                SampleSetParameters.TestingSamples = SampleSetParameters.AllTestingSamples;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{SampleSetParameters}.{SampleSetParameters.TestingSamples} has been set to {SampleSetParameters.AllTrainingSamples}.");
            return true;
        }
        public bool SetSamplePaths()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region methods: Change SampleSetParameters

        public void ChangeATrainerParameter(string parameterName, string parameterValue)
        {
            try
            {
                switch (parameterName)
                {
                    case nameof(TrainerParameters.LearningRate):
                        SetLearningRate(float.Parse(parameterValue));
                        break;
                    case nameof(TrainerParameters.LearningRateChange):
                        SetLearningRateChange(float.Parse(parameterValue));
                        break;
                    case nameof(TrainerParameters.CostType):
                        SetCostType(parameterValue.ToEnum<CostType>());
                        break;
                    case nameof(TrainerParameters.Epochs):
                        SetInputDistortion(int.Parse(parameterValue));
                        break;
                }
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); }
        }

        public bool SetCostType(CostType costType)
        {
            try
            {
                TrainerParameters.CostType = costType;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{TrainerParameters}.{TrainerParameters.CostType} has been set to {TrainerParameters.CostType}.");
            return true;
        }
        public bool SetLearningRateChange(float learningRateChange)
        {
            try
            {
                TrainerParameters.LearningRateChange = learningRateChange;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{TrainerParameters}.{TrainerParameters.LearningRateChange} has been set to {TrainerParameters.LearningRateChange}.");
            return true;
        }
        public bool SetLearningRate(float learningRate)
        {
            try
            {
                TrainerParameters.LearningRate = learningRate;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{TrainerParameters}.{TrainerParameters.LearningRate} has been set to {TrainerParameters.LearningRate}.");
            return true;
        }
        public bool SetEpochs(int epochs)
        {
            try
            {
                TrainerParameters.Epochs = epochs;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"{TrainerParameters}.{TrainerParameters.Epochs} has been set to {TrainerParameters.Epochs}.");
            return true;
        }

        #endregion

        #region methods: Create, Load & Save

        public bool CreateSampleSetParameters(string templateName = "")
        {
            SetName name = SetName.Custom;
            switch (templateName)
            {
                case "FourPixelCamera":
                    name = SetName.FourPixelCamera;
                    break;
                case "MNIST":
                    name = SetName.MNIST;
                    break;
            }
            if (name == SetName.Custom)
            {
                _onInitializerStatusChanged($"Template name {templateName} is unavailable.");
                return false;
            }

            SampleSetParameters = new SampleSetParameters { Name = name };
            _onInitializerStatusChanged("Sample set parameters created.");
            return true;
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
