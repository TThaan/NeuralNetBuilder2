using MatrixExtensions;
using NeuralNetBuilder.ActivatorFunctions;
using NeuralNetBuilder.CostFunctions;
using System;

namespace NeuralNetBuilder
{
    public interface ILearningLayer : ILayer
    {
        float[] DCDA { get; set; }
        float[] DADZ { get; set; }
        float[] Delta { get; set; }
        float[,] WeightsChange { get; set; }
        float[] BiasesChange { get; set; }
        void ProcessDelta(float[] target, ICostFunction costFunction);
        void AdaptWeightsAndBiases(float learningRate);
    }

    public class LearningLayer : Layer, ILearningLayer
    {
        #region fields & ctor

        float[] dCDA, dACZ, delta, biasesChange;
        float[,] weightsChange;

        internal LearningLayer() { }

        #endregion

        #region public

        public float[] DCDA
        {
            get { return dCDA; }
            set
            {
                if (dCDA != value)
                {
                    dCDA = value;
                    // OnPropertyChanged();
                }
            }
        }
        public float[] DADZ
        {
            get { return dACZ; }
            set
            {
                if (dACZ != value)
                {
                    dACZ = value;
                    // OnPropertyChanged();
                }
            }
        }
        public float[] Delta
        {
            get { return delta; }
            set
            {
                if (delta != value)
                {
                    delta = value;
                    // OnPropertyChanged();
                }
            }
        }
        public float[,] WeightsChange
        {
            get { return weightsChange; }
            set
            {
                if (weightsChange != value)
                {
                    weightsChange = value;
                    // OnPropertyChanged();
                }
            }
        }
        public float[] BiasesChange
        {
            get { return biasesChange; }
            set
            {
                if (biasesChange != value)
                {
                    biasesChange = value;
                    // OnPropertyChanged();
                }
            }
        }

        public void ProcessDelta(float[] target, ICostFunction costFunction)
        {
            SetDCDA(target, costFunction);
            SetDADZ();
            SetDelta();
            (ReceptiveField as ILearningLayer)?.ProcessDelta(target, costFunction);   // ta cast vs perf -> LearningNet incl LearningLayers?
        }
        public void AdaptWeightsAndBiases(float learningRate)
        {
            if (ReceptiveField != null)
            {
                // Get Change of Weights

                weightsChange = Delta.Multiply_ScalarProduct_ColumnWithRow(ReceptiveField.Output);
                //PerformantOperations.SetScalarProduct(Delta, ReceptiveField.Output.GetTranspose(), WeightsChange);

                weightsChange = weightsChange.Multiply(-learningRate);
                //PerformantOperations.Multiplicate(WeightsChange, -learningRate, WeightsChange);


                // Add Change to Weights

                Weights = Weights.Add(WeightsChange);
                //PerformantOperations.Add(Weights, WeightsChange, Weights);

                if (Biases != null)
                {
                    #region slow but tested

                    float[] etaTimesDelta = Delta.Multiply(learningRate);
                    //PerformantOperations.Multiplicate(Delta, learningRate, etaTimesDelta);  // check!

                    Biases = Biases.Subtract(etaTimesDelta);
                    //PerformantOperations.Subtract(Biases, etaTimesDelta, Biases);   // check!

                    #endregion

                    #region fast yet untested

                    throw new NotImplementedException();

                    #endregion
                }
            }

            (ProjectiveField as ILearningLayer)?.AdaptWeightsAndBiases(learningRate);
        }

        #endregion

        #region helpers

        private void SetDCDA(float[] target, ICostFunction costFunction)
        {
            if (ProjectiveField == null)
            {
                DCDA.ForEach(x => 0);   // redundant?
                DCDA = costFunction.Derivation(Output, target);
            }
            else
            {
                DCDA = ProjectiveField.Weights.Transpose().Multiply_MatrixWithColumnVector((ProjectiveField as ILearningLayer).Delta);
                //PerformantOperations.SetScalarProduct(ProjectiveField.Weights.GetTranspose(), (ProjectiveField as ILearningLayer).Delta, DCDA);   // ta cast vs perf -> LearningNet incl LearningLayers?
            }
        }
        private void SetDADZ()
        {
            DADZ.ForEach(x => 0);   // redundant?
            DADZ = ActivationFunction.Derivation(Input);
        }
        private void SetDelta()
        {
            // Delta.ForEach(x => 0);
            Delta = DCDA.Multiply_Elementwise(DADZ);
            // PerformantOperations.SetHadamardProduct(DCDA, DADZ, Delta);

        }

        #endregion

        #region ILogger

        public override string ToLog()
        {
            string result = LoggableName;

            result += $" ({nameof(Id)}: {Id}";
            result += $", Neurons: {N}";
            result += $", {nameof(ActivationFunction)}: {ActivationFunction.GetType().Name}";
            result += ReceptiveField == null
                ? $", {nameof(ReceptiveField)}: None"
                : $", {nameof(ReceptiveField)}: {ReceptiveField.Id}";
            result += ProjectiveField == null
                ? $", {nameof(ProjectiveField)}: None"
                : $", {nameof(ProjectiveField)}: {ProjectiveField.Id}";
            result += ")\n";
            result += $"{Input?.ToLog(nameof(Input))}";
            result += $"{Output?.ToLog(nameof(Output))}";
            result += $"{Weights?.ToLog(nameof(Weights))}";
            result += $"{Biases?.ToLog(nameof(Biases))}";
            result += $"{DCDA?.ToLog(nameof(DCDA))}";
            result += $"{DADZ?.ToLog(nameof(DADZ))}";
            result += $"{Delta?.ToLog(nameof(Delta))}";

            return result;
        }

        #endregion
    }
}
