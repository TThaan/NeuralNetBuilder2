using DeepLearningDataProvider;
using NeuralNetBuilder.FactoriesAndParameters;
using System;
using System.Linq;

namespace NeuralNetBuilder.Builders
{
    // Builders provide methods to interact with the data classes (all pocos?).
    // You can access them from the ConsoleApi, AIDemoUI or use them as Wpf's 'Command-Executes'.
    // They already do or will (soon) provide an event to notify about the (succeeded) data changes.

    // Unused class ?

    public class NetBuilder : NotifierBase
    {
        #region fields & ctor

        private readonly Action<string> _onInitializerStatusChanged;

        public NetBuilder(Action<string> onInitializerStatusChanged)
        {
            _onInitializerStatusChanged = onInitializerStatusChanged;
        }

        #endregion

        #region properties

        //public ISampleSetParameters SampleSetParameters { get; set; }
        public INetParameters NetParameters { get; set; }
        public ITrainerParameters TrainerParameters { get; set; }

        // SampleSetParameters

        // NetParameters
        public WeightInitType WeightInitType { get; set; }
        public float WeightMin_Global { get; set; }
        public float WeightMax_Global { get; set; }
        public float BiasMin_Global { get; set; }
        public float BiasMax_Global { get; set; }
        public bool AreParametersGlobal { get; set; }   // redundant?

        // TrainerParameters
        public CostType CostType { get; set; }
        public float LearningRate { get; set; }
        public float LearningRateChange { get; set; }
        public int EpochCount { get; set; }

        #endregion

        #region methods

        public bool SetWeightMaxAtLayer(int index, float weightMax)
        {
            if (!AreRelevantVariablesSet(index, nameof(NetParameters), nameof(NetParameters.LayerParametersCollection)))
                return false;

            NetParameters.LayerParametersCollection[index].WeightMax = weightMax;
            _onInitializerStatusChanged($"WeightMax of layer {index} = {weightMax}.");
            return true;
        }
        public bool SetWeightMinAt(int index, float weightMin)
        {
            if (!AreRelevantVariablesSet(index, nameof(NetParameters), nameof(NetParameters.LayerParametersCollection)))
                return false;

            NetParameters.LayerParametersCollection[index].WeightMin = weightMin;
            _onInitializerStatusChanged($"WeightMin of layer {index} = {weightMin}.");
            return true;
        }
        public bool SetBiasMaxAt(int index, float biasMax)
        {
            if (!AreRelevantVariablesSet(index, nameof(NetParameters), nameof(NetParameters.LayerParametersCollection)))
                return false;

            NetParameters.LayerParametersCollection[index].BiasMax = biasMax;
            _onInitializerStatusChanged($"BiasMax of layer {index} = {biasMax}.");
            return true;
        }
        public bool SetBiasMinAt(int index, float biasMin)
        {
            if (!AreRelevantVariablesSet(index, nameof(NetParameters), nameof(NetParameters.LayerParametersCollection)))
                return false;

            NetParameters.LayerParametersCollection[index].BiasMin = biasMin;
            _onInitializerStatusChanged($"BiasMin of layer {index} = {biasMin}.");
            return true;
        }
        public bool AddNewLayerAfter(ILayerParameters precedingLayerParameters)
        {
            int precedingLayerIndex;
            if (precedingLayerParameters == null) precedingLayerIndex = -1;
            else precedingLayerIndex = precedingLayerParameters.Id;

            if (!AreRelevantVariablesSet(precedingLayerIndex, nameof(NetParameters), nameof(NetParameters.LayerParametersCollection)))
                return false;

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

            NetParameters.LayerParametersCollection.Insert(precedingLayerIndex + 1, newLayerParameters);
            ResetLayersIndeces();

            _onInitializerStatusChanged($"New layer added. (Id = {precedingLayerIndex + 1}.");
            return true;
        }
        public bool DeleteLayer(ILayerParameters layerParameters)
        {
            if (NetParameters.LayerParametersCollection.Count > 2)
                NetParameters.LayerParametersCollection.Remove(layerParameters);
            else
            {
                _onInitializerStatusChanged($"You must not delete the last standing layer.");
                return false; 
            }
            ResetLayersIndeces();

            _onInitializerStatusChanged($"Layer deleted. (Id = {layerParameters.Id}.");
            return true;
        }
        public bool MoveLayerLeft(ILayerParameters layerParameters)
        {
            NetParameters.LayerParametersCollection.Move(
                layerParameters.Id, layerParameters.Id > 0 ? layerParameters.Id - 1 : 0);
            ResetLayersIndeces();

            _onInitializerStatusChanged($"Layer moved left. (OldId = {layerParameters.Id}.");
            return true;
        }
        public bool MoveLayerRight(ILayerParameters layerParameters)
        {
            NetParameters.LayerParametersCollection.Move(
                layerParameters.Id, layerParameters.Id < NetParameters.LayerParametersCollection.Count - 1 ? layerParameters.Id + 1 : 0);
            ResetLayersIndeces();

            _onInitializerStatusChanged($"Layer moved right. (OldId = {layerParameters.Id}.");
            return true;
        }

        #endregion

        #region helpers

        private bool AreRelevantVariablesSet(int layerIndex = -1, params string[] instances)
        {
            if (instances.Contains(nameof(NetParameters)) && NetParameters == null)
            {
                _onInitializerStatusChanged($"NetParameters do not exist.");
                return false;
            }
            else if (instances.Contains(nameof(NetParameters.LayerParametersCollection)) && NetParameters.LayerParametersCollection == null)
            {
                _onInitializerStatusChanged($"NetParameters.LayerParametersCollection does not exist.");
                return false;
            }
            else if (layerIndex > - 1 && NetParameters.LayerParametersCollection[layerIndex] == null)
            {
                _onInitializerStatusChanged($"Layer {layerIndex} does not exist.");
                return false;
            }

            return true;
        }
        private void ResetLayersIndeces()
        {
            int i = 0;
            foreach (var layerParam in NetParameters.LayerParametersCollection)
                layerParam.Id = i++;
        }

        #endregion
    }
}
