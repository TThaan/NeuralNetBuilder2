using ImpEx;
using NeuralNetBuilder.FactoriesAndParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NeuralNetBuilder.Builders
{
    // Builders provide methods to create/load and interact with the data classes.
    // (I.e. they're a bit more than a factory.)
    // You can access them from the ConsoleApi, AIDemoUI or use them as Wpf's 'Command-Executes'.
    // They provide a Status property incl an event to notify about the (succeeded) data changes.
    // Failed attempts throwing an exception can be caught in the client.

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
            NetParameters.WeightInitType = (WeightInitType)weightInitType;
            Status = $"WeightInitType = {NetParameters.WeightInitType}.";
        }
        public void SetWeightMax_Globally(float weightMax)
        {
            // WeightMax_Global = weightMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = weightMax;
            Status = $"Global WeightMax = {weightMax}.";
        }
        public void SetWeightMin_Globally(float weightMin)
        {
            //WeightMin_Global = weightMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = weightMin;
            Status = $"Global WeightMin = {weightMin}.";
        }
        public void SetBiasMax_Globally(float biasMax)
        {
            //BiasMax_Global = biasMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = biasMax;
            Status = $"Global BiasMax = {biasMax}.";
        }
        public void SetBiasMin_Globally(float biasMin)
        {
            //BiasMin_Global = biasMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = biasMin;
            Status = $"Global BiasMin = {biasMin}.";
        }

        public void AddLayerAfter(int precedingLayerId)
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
        public void DeleteLayer(int layerId)
        {
            LayerParametersCollection.Remove(LayerParametersCollection[layerId]);
            
            if (LayerParametersCollection.Count > 2)
                Status = $"Layer deleted. (Id = {layerId}).";
            else
                Status = $"You must not delete the last standing layer.";
            
            ResetLayersIndeces();
        }
        public void MoveLayerLeft(int layerId)
        {
            LayerParametersCollection.Move(
            layerId, layerId > 0 ? layerId - 1 : 0);
            ResetLayersIndeces();
            Status = $"Layer moved left. (OldId = {layerId}).";
        }
        public void MoveLayerRight(int layerId)
        {
            LayerParametersCollection.Move(
            layerId, layerId < NetParameters.LayerParametersCollection.Count - 1 ? layerId + 1 : 0);
            ResetLayersIndeces();
            Status = $"Layer moved right. (OldId = {layerId}).";
        }

        // ta: Changing N or just add/remove layers?
        public void SetNeuronsAtLayer(int layerId, int neurons)
        {
            LayerParametersCollection[layerId].NeuronsPerLayer = neurons;
            Status = $"Amount of neurons in layer {layerId} = {LayerParametersCollection[layerId].NeuronsPerLayer}.";
        }
        public void SetActivationTypeAtLayer(int layerId, int activationType)
        {
            LayerParametersCollection[layerId].ActivationType = (ActivationType)activationType;
            Status = $"Activation type of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.";
        }
        public void SetWeightMaxAtLayer(int layerId, float weightMax)
        {
            LayerParametersCollection[layerId].WeightMax = weightMax;
            Status = $"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.";
        }
        public void SetWeightMinAtLayer(int layerId, float weightMin)
        {
            LayerParametersCollection[layerId].WeightMin = weightMin;
            Status = $"WeightMin of layer {layerId} = {LayerParametersCollection[layerId].WeightMin}.";
        }
        public void SetBiasMaxAtLayer(int layerId, float biasMax)
        {
            LayerParametersCollection[layerId].BiasMax = biasMax;
            Status = $"BiasMax of layer {layerId} = {LayerParametersCollection[layerId].BiasMax}.";
        }
        public void SetBiasMinAtLayer(int layerId, float biasMin)
        {
            LayerParametersCollection[layerId].BiasMin = biasMin;
            Status = $"BiasMin of layer {layerId} = {LayerParametersCollection[layerId].BiasMin}.";
        }

        #endregion

        #region methods: Change TrainerParameters

        public void SetCostType(int costType)
        {
            TrainerParameters.CostType = (CostType)costType;
            Status = $"{TrainerParameters}.{TrainerParameters.CostType} has been set to {TrainerParameters.CostType}.";
        }
        public void SetLearningRateChange(float learningRateChange)
        {
            TrainerParameters.LearningRateChange = learningRateChange;
            Status = $"{TrainerParameters}.{TrainerParameters.LearningRateChange} has been set to {TrainerParameters.LearningRateChange}.";
        }
        public void SetLearningRate(float learningRate)
        {
            TrainerParameters.LearningRate = learningRate;
            Status = $"{TrainerParameters}.{TrainerParameters.LearningRate} has been set to {TrainerParameters.LearningRate}.";
        }
        public void SetEpochs(int epochs)
        {
            TrainerParameters.Epochs = epochs;
            Status = $"{TrainerParameters}.{TrainerParameters.Epochs} has been set to {TrainerParameters.Epochs}.";
        }

        #endregion

        #region methods: Load & Save

        public async Task LoadNetParametersAsync(string fileName)
        {
            Status = "Loading net parameters from file, please wait...";
            NetParameters = await Import.LoadAsJsonAsync<NetParameters>(fileName);
            Status = "Successfully loaded net parameters.";
        }
        public async Task LoadTrainerParametersAsync(string path)
        {
            Status = "Loading trainer parameters from file, please wait...";                    
            TrainerParameters = await Import.LoadAsJsonAsync<TrainerParameters>(path);
            Status = "Successfully loaded trainer parameters.";
        }

        public async Task SaveNetParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            Status = "Saving net parameters, please wait...";
            await Export.SaveAsJsonAsync(NetParameters, path, formatting, true);
            Status = "Successfully saved net parameters.";
        }
        public async Task SaveTrainerParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            Status = "Saving trainer parameters, please wait...";
            await Export.SaveAsJsonAsync(TrainerParameters, path, formatting, true);
            Status = "Successfully saved trainer parameters.";
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