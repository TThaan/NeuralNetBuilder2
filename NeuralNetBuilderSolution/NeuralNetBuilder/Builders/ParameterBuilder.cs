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

        private readonly PathBuilder _paths;
        private readonly Action<string> _onInitializerStatusChanged;

        private IEnumerable<ActivationType> activationTypes;
        private IEnumerable<CostType> costTypes;
        private IEnumerable<WeightInitType> weightInitTypes;

        private INetParameters netParameters;
        private ITrainerParameters trainerParameters;

        public ParameterBuilder(PathBuilder paths, Action<string> onInitializerStatusChanged)
        {
            _paths = paths;
            _onInitializerStatusChanged = onInitializerStatusChanged;
        }

        #endregion

        #region properties

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
            // [nameof(SetWeightInitType)] = "wInit",  // This or the latter redundant?
            // [nameof(SetWeightMin_Globally)] = "wMin", 
            // [nameof(SetWeightMax_Globally)] = "wMax", 
            // [nameof(SetBiasMin_Globally)] = "bMin",
            // [nameof(SetBiasMax_Globally)] = "bMax",
            [nameof(SetWeightMaxAtLayer)] = "wMax",
            [nameof(SetWeightMinAtLayer)] = "wMin",
            [nameof(SetBiasMaxAtLayer)] = "bMax",
            [nameof(SetBiasMinAtLayer)] = "bMin",
            [nameof(AddNewLayerAfter)] = "add",
            [nameof(DeleteLayer)] = "del",
            [nameof(MoveLayerLeft)] = "left",
            [nameof(MoveLayerRight)] = "right",
            [nameof(SetNeuronsAtLayer)] = "neurons",
            [nameof(SetActivationTypeAtLayer)] = "act"
        };

        public IEnumerable<CostType> CostTypes => costTypes ??
            (costTypes = Enum.GetValues(typeof(CostType)).ToList<CostType>());
        public IEnumerable<WeightInitType> WeightInitTypes => weightInitTypes ??
            (weightInitTypes = Enum.GetValues(typeof(WeightInitType)).ToList<WeightInitType>());
        public IEnumerable<ActivationType> ActivationTypes => activationTypes ??
            (activationTypes = Enum.GetValues(typeof(ActivationType)).ToList<ActivationType>());

        #endregion

        #region methods

        public void ChangeParameter(string parameterName, string parameterValue, string id)
        {
            try
            {
                // Net parameters
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

                // Trainer Parameters
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
                        SetEpochs(int.Parse(parameterValue));
                        break;
                }

                // Layer Parameters
                int.TryParse(id, out int layerId);

                if (parameterName == ParameterNames[nameof(AddNewLayerAfter)])
                    AddNewLayerAfter(layerId);
                else if (parameterName == ParameterNames[nameof(DeleteLayer)])
                    DeleteLayer(layerId);
                else if (parameterName == ParameterNames[nameof(MoveLayerLeft)])
                    MoveLayerLeft(layerId);
                else if (parameterName == ParameterNames[nameof(MoveLayerRight)])
                    MoveLayerRight(layerId);

                else if (parameterName == ParameterNames[nameof(SetNeuronsAtLayer)])
                    SetNeuronsAtLayer(layerId, int.Parse(parameterValue));
                else if (parameterName == ParameterNames[nameof(SetActivationTypeAtLayer)])
                    SetActivationTypeAtLayer(layerId, parameterValue.ToEnum<ActivationType>());
                else if (parameterName == ParameterNames[nameof(SetWeightMaxAtLayer)])
                    SetWeightMaxAtLayer(layerId, float.Parse(parameterValue));
                else if (parameterName == ParameterNames[nameof(SetWeightMinAtLayer)])
                    SetWeightMinAtLayer(layerId, float.Parse(parameterValue));
                else if (parameterName == ParameterNames[nameof(SetBiasMaxAtLayer)])
                    SetBiasMaxAtLayer(layerId, float.Parse(parameterValue));
                else if (parameterName == ParameterNames[nameof(SetBiasMinAtLayer)])
                    SetBiasMinAtLayer(layerId, float.Parse(parameterValue));
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); }

            //PropertyInfo pi = NetParameters.GetType().GetProperty(parameterName);
            //pi.SetValue(NetParameters, parameterValue);
        }

        #region methods: Change NetParameters

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

            _onInitializerStatusChanged($"New layer added. (Id = {precedingLayerIndex + 1}).");
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
            
            _onInitializerStatusChanged($"Layer deleted. (Id = {layerId}).");
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

            _onInitializerStatusChanged($"Layer moved left. (OldId = {layerId}).");
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

            _onInitializerStatusChanged($"Layer moved right. (OldId = {layerId}).");
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

        #region methods: Change TrainerParameters

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

        public void CreateNetParameters()
        {
            NetParameters = new NetParameters();
            NetParameters.LayerParametersCollection.Add(new LayerParameters() 
            { Id = 0, NeuronsPerLayer = 4, WeightMin = -1, WeightMax = 1, ActivationType = ActivationType.NullActivator});
            _onInitializerStatusChanged("Net parameters created.");
        }
        public void CreateTrainerParameters()
        {
            TrainerParameters = new TrainerParameters();
            _onInitializerStatusChanged("Trainer parameters created.");
        }

        public async Task<bool> LoadNetParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    _onInitializerStatusChanged("Loading net parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(_paths.NetParameters);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    NetParameters = sp.NetParameters;
                    _onInitializerStatusChanged("Successfully loaded net parameters.");
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
                    _onInitializerStatusChanged("Loading trainer parameters from file, please wait...");
                    var jasonParams = File.ReadAllText(_paths.TrainerParameters);
                    var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    TrainerParameters = sp.TrainerParameters;
                    _onInitializerStatusChanged("Successfully loaded trainer parameters.");
                    return true;
                }
                catch (Exception e) { _onInitializerStatusChanged($"{e.Message}"); return false; }
            });
        }

        public async Task<bool> SaveNetParametersAsync()
        {
            try
            {
                _onInitializerStatusChanged("Saving net parameters, please wait...");

                var jsonString = JsonConvert.SerializeObject(NetParameters, Formatting.Indented);
                await File.AppendAllTextAsync(_paths.NetParameters, jsonString);

                _onInitializerStatusChanged("Successfully saved net parameters.");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainerParametersAsync()
        {
            try
            {
                _onInitializerStatusChanged("Saving trainer parameters, please wait...");

                var jsonString = JsonConvert.SerializeObject(TrainerParameters, Formatting.Indented);
                await File.AppendAllTextAsync(_paths.TrainerParameters, jsonString);

                _onInitializerStatusChanged("Successfully saved trainer parameters.");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }

        #endregion

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
