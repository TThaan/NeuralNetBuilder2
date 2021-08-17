using ImpEx;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NeuralNetBuilder.Builders
{
    // Builders provide methods to interact with the data classes (all pocos?).
    // You can access them from the ConsoleApi, AIDemoUI or use them as Wpf's 'Command-Executes'.
    // They already do or will (soon) provide an event to notify about the (succeeded) data changes.

    public class ParameterBuilder : InitializerAssistant
    {
        #region fields & ctor

        private IEnumerable<ActivationType> activationTypes;
        private IEnumerable<CostType> costTypes;
        private IEnumerable<WeightInitType> weightInitTypes;

        private INetParameters netParameters;
        private ITrainerParameters trainerParameters;

        #endregion

        #region properties

        public INetParameters NetParameters
        {
            get { return netParameters; }
            set
            {
                // No equality check due to performance. (wa ref equal?)
                netParameters = value;
                OnPropertyChanged();
            }
        }
        public ITrainerParameters TrainerParameters
        {
            get { return trainerParameters; }
            set
            {
                // No equality check due to performance. 
                trainerParameters = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ILayerParameters> LayerParametersCollection => NetParameters.LayerParametersCollection;

        public IEnumerable<CostType> CostTypes => costTypes ??
            (costTypes = Enum.GetValues(typeof(CostType)).ToList<CostType>());
        public IEnumerable<WeightInitType> WeightInitTypes => weightInitTypes ??
            (weightInitTypes = Enum.GetValues(typeof(WeightInitType)).ToList<WeightInitType>());
        public IEnumerable<ActivationType> ActivationTypes => activationTypes ??
            (activationTypes = Enum.GetValues(typeof(ActivationType)).ToList<ActivationType>());

        #endregion

        #region methods
        
        #region methods: Change NetParameters

        public void SetWeightInitType(int weightInitType)
        {
            try
            {
                NetParameters.WeightInitType = (WeightInitType)weightInitType;
                OnStatusChanged($"WeightInitType = {NetParameters.WeightInitType}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void SetWeightMax_Globally(float weightMax)
        {
            // WeightMax_Global = weightMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = weightMax;            
            OnStatusChanged($"Global WeightMax = {weightMax}.");
        }
        public void SetWeightMin_Globally(float weightMin)
        {
            //WeightMin_Global = weightMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = weightMin;
            OnStatusChanged($"Global WeightMin = {weightMin}.");
        }
        public void SetBiasMax_Globally(float biasMax)
        {
            //BiasMax_Global = biasMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = biasMax;
            OnStatusChanged($"Global BiasMax = {biasMax}.");
        }
        public void SetBiasMin_Globally(float biasMin)
        {
            //BiasMin_Global = biasMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = biasMin;
            OnStatusChanged($"Global BiasMin = {biasMin}.");
        }

        public void AddLayerAfter(int precedingLayerId)
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

                OnStatusChanged($"New layer added. (Id = {precedingLayerId + 1}).");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void DeleteLayer(int layerId)
        {
            LayerParametersCollection.Remove(LayerParametersCollection[layerId]);
            try
            {
                if (LayerParametersCollection.Count > 2)
                {
                    OnStatusChanged($"Layer deleted. (Id = {layerId}).");
                }
                else
                {
                    OnStatusChanged($"You must not delete the last standing layer.");
                }
                ResetLayersIndeces();
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void MoveLayerLeft(int layerId)
        {
            try
            {
                LayerParametersCollection.Move(
                layerId, layerId > 0 ? layerId - 1 : 0);
                ResetLayersIndeces();
                OnStatusChanged($"Layer moved left. (OldId = {layerId}).");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void MoveLayerRight(int layerId)
        {
            try
            {
                LayerParametersCollection.Move(
                layerId, layerId < NetParameters.LayerParametersCollection.Count - 1 ? layerId + 1 : 0);
                ResetLayersIndeces();
                OnStatusChanged($"Layer moved right. (OldId = {layerId}).");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }

        // ta: Changing N or just add/remove layers?
        public void SetNeuronsAtLayer(int layerId, int neurons)
        {
            try
            {
                LayerParametersCollection[layerId].NeuronsPerLayer = neurons;
                OnStatusChanged($"Amount of neurons in layer {layerId} = {LayerParametersCollection[layerId].NeuronsPerLayer}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void SetActivationTypeAtLayer(int layerId, int activationType)
        {
            try
            {
                LayerParametersCollection[layerId].ActivationType = (ActivationType)activationType;
                OnStatusChanged($"Activation type of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void SetWeightMaxAtLayer(int layerId, float weightMax)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMax = weightMax;
                OnStatusChanged($"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void SetWeightMinAtLayer(int layerId, float weightMin)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMin = weightMin;
                OnStatusChanged($"WeightMin of layer {layerId} = {LayerParametersCollection[layerId].WeightMin}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void SetBiasMaxAtLayer(int layerId, float biasMax)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMax = biasMax;
                OnStatusChanged($"BiasMax of layer {layerId} = {LayerParametersCollection[layerId].BiasMax}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }
        public void SetBiasMinAtLayer(int layerId, float biasMin)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMin = biasMin;
                OnStatusChanged($"BiasMin of layer {layerId} = {LayerParametersCollection[layerId].BiasMin}.");
            }
            catch (Exception e) { OnStatusChanged(e.Message); }
        }

        #endregion

        #region methods: Change TrainerParameters

        public bool SetCostType(int costType)
        {
            try
            {
                TrainerParameters.CostType = (CostType)costType;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }

            OnStatusChanged($"{TrainerParameters}.{TrainerParameters.CostType} has been set to {TrainerParameters.CostType}.");
            return true;
        }
        public bool SetLearningRateChange(float learningRateChange)
        {
            try
            {
                TrainerParameters.LearningRateChange = learningRateChange;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }

            OnStatusChanged($"{TrainerParameters}.{TrainerParameters.LearningRateChange} has been set to {TrainerParameters.LearningRateChange}.");
            return true;
        }
        public bool SetLearningRate(float learningRate)
        {
            try
            {
                TrainerParameters.LearningRate = learningRate;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }

            OnStatusChanged($"{TrainerParameters}.{TrainerParameters.LearningRate} has been set to {TrainerParameters.LearningRate}.");
            return true;
        }
        public bool SetEpochs(int epochs)
        {
            try
            {
                TrainerParameters.Epochs = epochs;
            }
            catch (Exception e) { OnStatusChanged(e.Message); return false; }

            OnStatusChanged($"{TrainerParameters}.{TrainerParameters.Epochs} has been set to {TrainerParameters.Epochs}.");
            return true;
        }

        #endregion

        #region methods: Load & Save
        public async Task<bool> LoadNetParametersAsync(string path)
        {
            try
            {
                OnStatusChanged("Loading net parameters from file, please wait...");

                // ar jasonParams = File.ReadAllText(_paths.NetParameters);
                // ar sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);

                NetParameters = await Import.LoadAsJsonAsync<NetParameters>(path);
                //NetParameters.AdoptValuesOfOtherNetParameters(loadedNetParameters);

                OnStatusChanged("Successfully loaded net parameters.");
                return true;
            }
            catch (Exception e) { base.OnStatusChanged($"{e.Message}"); return false; }
        }
        public async Task<bool> LoadTrainerParametersAsync(string path)
        {
            try
            {
                OnStatusChanged("Loading trainer parameters from file, please wait...");
                // var jasonParams = File.ReadAllText(_paths.TrainerParameters);
                // var sp = JsonConvert.DeserializeObject<SerializedParameters>(jasonParams);
                    
                TrainerParameters = await Import.LoadAsJsonAsync<TrainerParameters>(path);  // Adopt!
                OnStatusChanged("Successfully loaded trainer parameters.");
                return true;
            }
            catch (Exception e) { base.OnStatusChanged($"{e.Message}"); return false; }
        }

        public async Task<bool> SaveNetParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            try
            {
                OnStatusChanged("Saving net parameters, please wait...");
                await Export.SaveAsJsonAsync(NetParameters, path, formatting, true);
                base.OnStatusChanged("Successfully saved net parameters.");
                return true;
            }
            catch (Exception e) { base.OnStatusChanged(e.Message); return false; }
        }
        public async Task<bool> SaveTrainerParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            try
            {
                OnStatusChanged("Saving trainer parameters, please wait...");
                await Export.SaveAsJsonAsync(TrainerParameters, path, formatting, true);
                OnStatusChanged("Successfully saved trainer parameters.");
                return true;
            }
            catch (Exception e) { base.OnStatusChanged(e.Message); return false; }
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