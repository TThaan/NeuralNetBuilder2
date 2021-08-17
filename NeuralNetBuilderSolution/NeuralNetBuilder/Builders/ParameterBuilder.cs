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

    // Task: Add specific exception messages or remove them at all!

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
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetWeightMax_Globally(float weightMax)
        {
            try
            {
                // WeightMax_Global = weightMax;
                foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = weightMax;            
                OnStatusChanged($"Global WeightMax = {weightMax}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetWeightMin_Globally(float weightMin)
        {
            try
            {
                //WeightMin_Global = weightMin;
                foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = weightMin;
                OnStatusChanged($"Global WeightMin = {weightMin}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetBiasMax_Globally(float biasMax)
        {
            try
            {
                //BiasMax_Global = biasMax;
                foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = biasMax;
                OnStatusChanged($"Global BiasMax = {biasMax}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetBiasMin_Globally(float biasMin)
        {
            try
            {
                //BiasMin_Global = biasMin;
                foreach (var lp in NetParameters.LayerParametersCollection)
                lp.WeightMax = biasMin;
                OnStatusChanged($"Global BiasMin = {biasMin}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
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
            catch (Exception e) { ThrowFormattedException(e); }
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
            catch (Exception e) { ThrowFormattedException(e); }
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
            catch (Exception e) { ThrowFormattedException(e); }
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
            catch (Exception e) { ThrowFormattedException(e); }
        }

        // ta: Changing N or just add/remove layers?
        public void SetNeuronsAtLayer(int layerId, int neurons)
        {
            try
            {
                LayerParametersCollection[layerId].NeuronsPerLayer = neurons;
                OnStatusChanged($"Amount of neurons in layer {layerId} = {LayerParametersCollection[layerId].NeuronsPerLayer}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetActivationTypeAtLayer(int layerId, int activationType)
        {
            try
            {
                LayerParametersCollection[layerId].ActivationType = (ActivationType)activationType;
                OnStatusChanged($"Activation type of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetWeightMaxAtLayer(int layerId, float weightMax)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMax = weightMax;
                OnStatusChanged($"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetWeightMinAtLayer(int layerId, float weightMin)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMin = weightMin;
                OnStatusChanged($"WeightMin of layer {layerId} = {LayerParametersCollection[layerId].WeightMin}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetBiasMaxAtLayer(int layerId, float biasMax)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMax = biasMax;
                OnStatusChanged($"BiasMax of layer {layerId} = {LayerParametersCollection[layerId].BiasMax}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetBiasMinAtLayer(int layerId, float biasMin)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMin = biasMin;
                OnStatusChanged($"BiasMin of layer {layerId} = {LayerParametersCollection[layerId].BiasMin}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }

        #endregion

        #region methods: Change TrainerParameters

        public void SetCostType(int costType)
        {
            try
            {
                TrainerParameters.CostType = (CostType)costType;
                OnStatusChanged($"{TrainerParameters}.{TrainerParameters.CostType} has been set to {TrainerParameters.CostType}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetLearningRateChange(float learningRateChange)
        {
            try
            {
                TrainerParameters.LearningRateChange = learningRateChange;
                OnStatusChanged($"{TrainerParameters}.{TrainerParameters.LearningRateChange} has been set to {TrainerParameters.LearningRateChange}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetLearningRate(float learningRate)
        {
            try
            {
                TrainerParameters.LearningRate = learningRate;
                OnStatusChanged($"{TrainerParameters}.{TrainerParameters.LearningRate} has been set to {TrainerParameters.LearningRate}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetEpochs(int epochs)
        {
            try
            {
                TrainerParameters.Epochs = epochs;
                OnStatusChanged($"{TrainerParameters}.{TrainerParameters.Epochs} has been set to {TrainerParameters.Epochs}.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }

        #endregion

        #region methods: Load & Save

        public async Task LoadNetParametersAsync(string fileName)
        {
            try
            {
                OnStatusChanged("Loading net parameters from file, please wait...");
                NetParameters = await Import.LoadAsJsonAsync<NetParameters>(fileName);
                OnStatusChanged("Successfully loaded net parameters.");
            }
            catch (JsonReaderException e) { throw new ArgumentException($"Couldn't read the Json file.\nDetails: {e.Message}"); }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public async Task LoadTrainerParametersAsync(string path)
        {
            try
            {
                OnStatusChanged("Loading trainer parameters from file, please wait...");                    
                TrainerParameters = await Import.LoadAsJsonAsync<TrainerParameters>(path);
                OnStatusChanged("Successfully loaded trainer parameters.");
            }
            catch (JsonReaderException e) { throw new ArgumentException($"Couldn't read the Json file.\nDetails: {e.Message}"); }
            catch (Exception e) { ThrowFormattedException(e); }
        }

        public async Task SaveNetParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            try
            {
                OnStatusChanged("Saving net parameters, please wait...");
                await Export.SaveAsJsonAsync(NetParameters, path, formatting, true);
                OnStatusChanged("Successfully saved net parameters.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public async Task SaveTrainerParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            try
            {
                OnStatusChanged("Saving trainer parameters, please wait...");
                await Export.SaveAsJsonAsync(TrainerParameters, path, formatting, true);
                OnStatusChanged("Successfully saved trainer parameters.");
            }
            catch (Exception e) { ThrowFormattedException(e); }
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
        private void ThrowFormattedException(Exception e)
        {
            throw new ArgumentException($"{e.GetType().Name}:\nDetails: {e.Message}");
        }

        #endregion
    }
}