using DeepLearningDataProvider;
using NeuralNetBuilder.FactoriesAndParameters;

namespace NeuralNetBuilder
{
    public class Initializer
    {
        #region public

        public static INet GetRawNet()
        {
            return NetFactory.CreateRawNet();
        }
        public static INet InitializeNet(INet rawNet, INetParameters netParameters)
        {
            return NetFactory.InitializeNet(rawNet, netParameters);
        }
        public static ITrainer GetRawTrainer()
        {
            return TrainerFactory.GetRawTrainer();
        }
        public static ITrainer InitializeTrainer(ITrainer trainer, INet net, ITrainerParameters trainerParameters, ISampleSet sampleSet)
        {
            return TrainerFactory.InitializeTrainer(trainer, net, trainerParameters, sampleSet);
        }

        #endregion
    }
}
