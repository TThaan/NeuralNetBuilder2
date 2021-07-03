using DeepLearningDataProvider;
using NeuralNetBuilder.FactoriesAndParameters;
using System;

namespace NeuralNetBuilder.Builders
{
    public class ParameterBuilder
    {
        #region fields & ctor

        private readonly Action<string> _onInitializerStatusChanged;

        public ParameterBuilder(Action<string> onInitializerStatusChanged)
        {
            _onInitializerStatusChanged = onInitializerStatusChanged;
        }

        #endregion

        #region properties

        public ISampleSetParameters SampleSetParameters { get; set; }
        public INetParameters NetParameters { get; set; }
        public ITrainerParameters TrainerParameters { get; set; }

        #endregion

        #region methods



        #endregion
    }
}
