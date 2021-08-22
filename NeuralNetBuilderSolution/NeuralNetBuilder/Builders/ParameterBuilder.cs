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

    public class ParameterBuilder : NotificationChangedBase
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
            NetParameters.WeightInitType = (WeightInitType)weightInitType;
            Notification = $"WeightInitType = {NetParameters.WeightInitType}.";
        }
        public void SetWeightMax_Globally(float weightMax)
        {
            // WeightMax_Global = weightMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = weightMax;
            Notification = $"Global WeightMax = {weightMax}.";
        }
        public void SetWeightMin_Globally(float weightMin)
        {
            //WeightMin_Global = weightMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = weightMin;
            Notification = $"Global WeightMin = {weightMin}.";
        }
        public void SetBiasMax_Globally(float biasMax)
        {
            //BiasMax_Global = biasMax;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = biasMax;
            Notification = $"Global BiasMax = {biasMax}.";
        }
        public void SetBiasMin_Globally(float biasMin)
        {
            //BiasMin_Global = biasMin;
            foreach (var lp in NetParameters.LayerParametersCollection)
            lp.WeightMax = biasMin;
            Notification = $"Global BiasMin = {biasMin}.";
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

            Notification = $"New layer added. (Id = {precedingLayerId + 1}).";
        }
        public void DeleteLayer(int layerId)
        {
            LayerParametersCollection.Remove(LayerParametersCollection[layerId]);
            
            if (LayerParametersCollection.Count > 2)
                Notification = $"Layer {layerId} deleted.";
            else
                Notification = $"You must not delete the last standing layer.";
            
            ResetLayersIndeces();
        }
        public void MoveLayerLeft(int layerId)
        {
            LayerParametersCollection.Move(
            layerId, layerId > 0 ? layerId - 1 : 0);
            ResetLayersIndeces();
            Notification = $"Switched layers {layerId} and {layerId - 1}.";
        }
        public void MoveLayerRight(int layerId)
        {
            LayerParametersCollection.Move(
            layerId, layerId < NetParameters.LayerParametersCollection.Count - 1 ? layerId + 1 : 0);
            ResetLayersIndeces();
            Notification = $"Switched layers {layerId} and {layerId + 1}.";
        }

        public void SetNeuronsAtLayer(int layerId, int neurons)
        {
            LayerParametersCollection[layerId].NeuronsPerLayer = neurons;
            Notification = $"Amount of neurons in layer {layerId} = {LayerParametersCollection[layerId].NeuronsPerLayer}.";
        }
        public void SetActivationTypeAtLayer(int layerId, int activationType)
        {
            LayerParametersCollection[layerId].ActivationType = (ActivationType)activationType;
            Notification = $"Activation type of layer {layerId} = {(ActivationType)activationType}.";
        }
        public void SetWeightMaxAtLayer(int layerId, float weightMax)
        {
            LayerParametersCollection[layerId].WeightMax = weightMax;
            Notification = $"WeightMax of layer {layerId} = {LayerParametersCollection[layerId].WeightMax}.";
        }
        public void SetWeightMinAtLayer(int layerId, float weightMin)
        {
            LayerParametersCollection[layerId].WeightMin = weightMin;
            Notification = $"WeightMin of layer {layerId} = {LayerParametersCollection[layerId].WeightMin}.";
        }
        public void SetBiasMaxAtLayer(int layerId, float biasMax)
        {
            LayerParametersCollection[layerId].BiasMax = biasMax;
            Notification = $"BiasMax of layer {layerId} = {LayerParametersCollection[layerId].BiasMax}.";
        }
        public void SetBiasMinAtLayer(int layerId, float biasMin)
        {
            LayerParametersCollection[layerId].BiasMin = biasMin;
            Notification = $"BiasMin of layer {layerId} = {LayerParametersCollection[layerId].BiasMin}.";
        }

        #endregion

        #region methods: Change TrainerParameters

        public void SetCostType(int costType)
        {
            TrainerParameters.CostType = (CostType)costType;
            Notification = $"{nameof(TrainerParameters.CostType)} has been set to {TrainerParameters.CostType}.";
        }
        public void SetLearningRateChange(float learningRateChange)
        {
            TrainerParameters.LearningRateChange = learningRateChange;
            Notification = $"{nameof(TrainerParameters.LearningRateChange)} has been set to {TrainerParameters.LearningRateChange}.";
        }
        public void SetLearningRate(float learningRate)
        {
            TrainerParameters.LearningRate = learningRate;
            Notification = $"{nameof(TrainerParameters.LearningRate)} has been set to {TrainerParameters.LearningRate}.";
        }
        public void SetEpochs(int epochs)
        {
            TrainerParameters.Epochs = epochs;
            Notification = $"{nameof(TrainerParameters.Epochs)} has been set to {TrainerParameters.Epochs}.";
        }

        #endregion

        #region methods: Load & Save

        public async Task LoadNetParametersAsync(string fileName)
        {
            Notification = "Loading net parameters from file, please wait...";
            NetParameters = await Import.LoadAsJsonAsync<NetParameters>(fileName);
            Notification = "Successfully loaded net parameters.";
        }
        public async Task LoadTrainerParametersAsync(string path)
        {
            Notification = "Loading trainer parameters from file, please wait...";                    
            TrainerParameters = await Import.LoadAsJsonAsync<TrainerParameters>(path);
            Notification = "Successfully loaded trainer parameters.";
        }

        public async Task SaveNetParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            Notification = "Saving net parameters, please wait...";
            await Export.SaveAsJsonAsync(NetParameters, path, formatting, true);
            Notification = "Successfully saved net parameters.";
        }
        public async Task SaveTrainerParametersAsync(string path, Formatting formatting = Formatting.Indented)
        {
            Notification = "Saving trainer parameters, please wait...";
            await Export.SaveAsJsonAsync(TrainerParameters, path, formatting, true);
            Notification = "Successfully saved trainer parameters.";
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