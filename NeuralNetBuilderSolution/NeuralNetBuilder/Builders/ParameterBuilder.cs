using ImpEx;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
                //if (netParameters == null)
                //    _onInitializerStatusChanged("NetParameters are null");
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
                //if (trainerParameters == null)
                //    _onInitializerStatusChanged("TrainerParameters are null");
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
                //if (NetParameters.LayerParametersCollection == null)
                //    _onInitializerStatusChanged("LayerParametersCollection is null");
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

        #region methods

        public void ChangeParameter(string parameter, int layerId)
        {
            ParameterName name = parameter.Split(':').First().ToEnum<ParameterName>();
            string value = parameter.Split(':').Last();

            try
            {
                // Trainer Parameters

                switch (name)
                {
                    case ParameterName.Eta:
                        SetLearningRate(float.Parse(value));
                        return;
                    case ParameterName.dEta:
                        SetLearningRateChange(float.Parse(value));
                        return;
                    case ParameterName.cost:
                        SetCostType(int.Parse(value));
                        return;
                    case ParameterName.epochs:
                        SetEpochs(int.Parse(value));
                        return;
                }

                // Net parameters

                switch (name)
                {
                    case ParameterName.wInit:
                        SetWeightInitType(int.Parse(value));
                        return;
                    // Or glob as layerId?
                    //case ParameterName.wMinGlob:
                    //    SetWeightMin_Globally(float.Parse(parameterValue));
                    //    break;
                    //case ParameterName.wMaxGlobally:
                    //    SetWeightMax_Globally(float.Parse(parameterValue));
                    //    break;
                    //case ParameterName.bMinGlob:
                    //    SetBiasMin_Globally(float.Parse(parameterValue));
                    //    break;
                    //case ParameterName.bMaxGlob:
                    //    SetBiasMax_Globally(float.Parse(parameterValue));
                    //    break;
                }

                // Layer Parameters
                
                if (layerId < 0 || layerId > LayerParametersCollection.Count - 1)
                    throw new ArgumentException("Missing an existing layer id!");
                
                switch (name)
                {
                    case ParameterName.act:
                        SetActivationTypeAtLayer(layerId, int.Parse(value));
                        return;
                    case ParameterName.wMax:
                        SetWeightMaxAtLayer(layerId, float.Parse(value));
                        return;
                    case ParameterName.wMin:
                        SetWeightMinAtLayer(layerId, float.Parse(value));
                        return;
                    case ParameterName.bMax:
                        SetBiasMaxAtLayer(layerId, float.Parse(value));
                        return;
                    case ParameterName.bMin:
                        SetBiasMinAtLayer(layerId, float.Parse(value));
                        return;
                };
                    
                
                throw new ArgumentException($"Parameter {name} unknown.");
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); }

            //PropertyInfo pi = NetParameters.GetType().GetProperty(parameterName);
            //pi.SetValue(NetParameters, parameterValue);
        }

        #region methods: Change NetParameters

        public bool SetWeightInitType(int weightInitType)
        {
            try
            {
                NetParameters.WeightInitType = (WeightInitType)weightInitType;
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

        public bool AddLayerAfter(int precedingLayerId)
        {
            try
            {
                if (precedingLayerId < 0 || precedingLayerId > LayerParametersCollection.Count - 1)
                    throw new ArgumentException("Missing an existing layer id!");

                ILayerParameters precedingLayerParameters;
                precedingLayerParameters = LayerParametersCollection[precedingLayerId];
                ILayerParameters newLayerParameters = new LayerParameters();

                newLayerParameters.NeuronsPerLayer = precedingLayerParameters.NeuronsPerLayer;
                newLayerParameters.ActivationType = precedingLayerParameters.ActivationType;
                newLayerParameters.BiasMin = precedingLayerParameters.BiasMin;
                newLayerParameters.BiasMax = precedingLayerParameters.BiasMax;
                newLayerParameters.WeightMin = precedingLayerParameters.WeightMin;
                newLayerParameters.WeightMax = precedingLayerParameters.WeightMax;

                LayerParametersCollection.Insert(precedingLayerId + 1, newLayerParameters);
                ResetLayersIndeces();
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"New layer added. (Id = {precedingLayerId + 1}).");
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
        public bool SetActivationTypeAtLayer(int layerId, int activationType)
        {
            try
            {
                LayerParametersCollection[layerId].ActivationType = (ActivationType)activationType;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }

            _onInitializerStatusChanged($"Activation type of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
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

        public bool SetCostType(int costType)
        {
            try
            {
                TrainerParameters.CostType = (CostType)costType;
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
            try
            {
                _onInitializerStatusChanged("Loading net parameters from file, please wait...");

                // ar jasonParams = File.ReadAllText(_paths.NetParameters);
                // ar sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);

                NetParameters = await Import.LoadAsJsonAsync<NetParameters>(_paths.NetParameters);
                _onInitializerStatusChanged("Successfully loaded net parameters.");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged($"{e.Message}"); return false; }
        }
        public async Task<bool> LoadTrainerParametersAsync()
        {
            try
            {
                _onInitializerStatusChanged("Loading trainer parameters from file, please wait...");
                // var jasonParams = File.ReadAllText(_paths.TrainerParameters);
                // var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    
                TrainerParameters = await Import.LoadAsJsonAsync<TrainerParameters>(_paths.TrainerParameters);
                _onInitializerStatusChanged("Successfully loaded trainer parameters.");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged($"{e.Message}"); return false; }
        }

        public async Task<bool> SaveNetParametersAsync(string formatting_String = default)
        {
            Formatting formatting = Formatting.None;

            if (string.Equals(formatting_String, "indented"))
                formatting = Formatting.Indented;
            else if (formatting_String != default)
                throw new ArgumentException($"{formatting_String} is not a valid parameter for {MainCommand.save} {LoadAndSaveCommand.netpar}.\n" +
                    $"When saving net parameters you can add parameter 'indented' or no parameter at all.");

            try
            {
                _onInitializerStatusChanged("Saving net parameters, please wait...");
                await Export.SaveAsJsonAsync(NetParameters, _paths.NetParameters, formatting, true);
                _onInitializerStatusChanged("Successfully saved net parameters.");
                return true;
            }
            catch (Exception e) { _onInitializerStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainerParametersAsync(string formatting_String = default)
        {
            Formatting formatting = Formatting.None;

            if (string.Equals(formatting_String, "indented"))
                formatting = Formatting.Indented;
            else if (formatting_String != default)
                throw new ArgumentException($"{formatting_String} is not a valid parameter for {MainCommand.save} {LoadAndSaveCommand.netpar}.\n" +
                    $"When saving trainer parameters you can add parameter 'indented' or no parameter at all.");

            try
            {
                _onInitializerStatusChanged("Saving trainer parameters, please wait...");
                await Export.SaveAsJsonAsync(TrainerParameters, _paths.TrainerParameters, formatting, true);
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