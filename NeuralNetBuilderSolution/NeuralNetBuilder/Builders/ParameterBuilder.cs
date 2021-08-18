using ImpEx;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NeuralNetBuilder.Builders
{
    // Builders provide methods to create/load and interact with the data classes (all pocos?).
    // You can access them from the ConsoleApi, AIDemoUI or use them as Wpf's 'Command-Executes'.
    // They already do or will (soon) provide an event to notify about the (succeeded) data changes.

    // Task: Add specific exception messages or remove them at all!

    public class ParameterBuilder : ParametersBase
    {
        #region fields & ctor

        private IEnumerable<ActivationType> activationTypes;
        private IEnumerable<CostType> costTypes;
        private IEnumerable<WeightInitType> weightInitTypes;

        private INetParameters netParameters;
        private ITrainerParameters trainerParameters;

        private string status;

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

        public string Status 
        {
            get { return status; }
            set
            {
                // No equality check due to potentially reapeated statuses.
                status = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region methods

        #region methods: Change NetParameters
        
        public void SetWeightInitType(int weightInitType)
        {
            try
            {
                NetParameters.WeightInitType = (WeightInitType)weightInitType;
                Status = $"WeightInitType = {NetParameters.WeightInitType}.";
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
                Status = $"Global WeightMax = {weightMax}.";
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
                Status = $"Global WeightMin = {weightMin}.";
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
                Status = $"Global BiasMax = {biasMax}.";
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
                Status = $"Global BiasMin = {biasMin}.";
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

                Status = $"New layer added. (Id = {precedingLayerId + 1}).";
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
                    Status = $"Layer deleted. (Id = {layerId}).";
                }
                else
                {
                    Status = $"You must not delete the last standing layer.";
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
                Status = $"Layer moved left. (OldId = {layerId}).";
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
                Status = $"Layer moved right. (OldId = {layerId}).";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }

        // ta: Changing N or just add/remove layers?
        public void SetNeuronsAtLayer(int layerId, int neurons)
        {
            try
            {
                LayerParametersCollection[layerId].NeuronsPerLayer = neurons;
                Status = $"Amount of neurons in layer {layerId} = {LayerParametersCollection[layerId].NeuronsPerLayer}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetActivationTypeAtLayer(int layerId, int activationType)
        {
            try
            {
                LayerParametersCollection[layerId].ActivationType = (ActivationType)activationType;
                Status = $"Activation type of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetWeightMaxAtLayer(int layerId, float weightMax)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMax = weightMax;
                Status = $"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetWeightMinAtLayer(int layerId, float weightMin)
        {
            try
            {
                LayerParametersCollection[layerId].WeightMin = weightMin;
                Status = $"WeightMin of layer {layerId} = {LayerParametersCollection[layerId].WeightMin}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetBiasMaxAtLayer(int layerId, float biasMax)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMax = biasMax;
                Status = $"BiasMax of layer {layerId} = {LayerParametersCollection[layerId].BiasMax}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetBiasMinAtLayer(int layerId, float biasMin)
        {
            try
            {
                LayerParametersCollection[layerId].BiasMin = biasMin;
                Status = $"BiasMin of layer {layerId} = {LayerParametersCollection[layerId].BiasMin}.";
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
                Status = $"{TrainerParameters}.{TrainerParameters.CostType} has been set to {TrainerParameters.CostType}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetLearningRateChange(float learningRateChange)
        {
            try
            {
                TrainerParameters.LearningRateChange = learningRateChange;
                Status = $"{TrainerParameters}.{TrainerParameters.LearningRateChange} has been set to {TrainerParameters.LearningRateChange}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetLearningRate(float learningRate)
        {
            try
            {
                TrainerParameters.LearningRate = learningRate;
                Status = $"{TrainerParameters}.{TrainerParameters.LearningRate} has been set to {TrainerParameters.LearningRate}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public void SetEpochs(int epochs)
        {
            try
            {
                TrainerParameters.Epochs = epochs;
                Status = $"{TrainerParameters}.{TrainerParameters.Epochs} has been set to {TrainerParameters.Epochs}.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }

        #endregion

        #region methods: Load & Save

        public async Task LoadNetParametersAsync(string fileName)
        {
            try
            {
                Status = "Loading net parameters from file, please wait...";
                NetParameters = await Import.LoadAsJsonAsync<NetParameters>(fileName);
                Status = "Successfully loaded net parameters.";
            }
            catch (JsonReaderException e) { throw new ArgumentException($"Couldn't read the Json file.\nDetails: {e.Message}"); }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public async Task LoadTrainerParametersAsync(string path)
        {
            try
            {
                Status = "Loading trainer parameters from file, please wait...";                    
                TrainerParameters = await Import.LoadAsJsonAsync<TrainerParameters>(path);
                Status = "Successfully loaded trainer parameters.";
            }
            catch (JsonReaderException e) { throw new ArgumentException($"Couldn't read the Json file.\nDetails: {e.Message}"); }
            catch (Exception e) { ThrowFormattedException(e); }
        }

        public async Task SaveNetParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            try
            {
                Status = "Saving net parameters, please wait...";
                await Export.SaveAsJsonAsync(NetParameters, path, formatting, true);
                Status = "Successfully saved net parameters.";
            }
            catch (Exception e) { ThrowFormattedException(e); }
        }
        public async Task SaveTrainerParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            try
            {
                Status = "Saving trainer parameters, please wait...";
                await Export.SaveAsJsonAsync(TrainerParameters, path, formatting, true);
                Status = "Successfully saved trainer parameters.";
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