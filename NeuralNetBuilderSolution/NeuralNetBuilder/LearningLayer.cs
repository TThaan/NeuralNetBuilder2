using MatrixHelper;
using NeuralNetBuilder.ActivatorFunctions;
using NeuralNetBuilder.CostFunctions;
using System;

namespace NeuralNetBuilder
{
    public interface ILearningLayer : ILayer
    {
        IMatrix DCDA { get; set; }
        IMatrix DADZ { get; set; }
        IMatrix Delta { get; set; }
        IMatrix WeightsChange { get; set; }
        IMatrix BiasesChange { get; set; }
        void ProcessDelta(IMatrix target, ICostFunction costFunction);
        void AdaptWeightsAndBiases(float learningRate);
    }

    public class LearningLayer : Layer, ILearningLayer
    {
        #region fields & ctor

        IMatrix dCDA, dACZ, delta, weightsChange, biasesChange;

        internal LearningLayer() { }

        #endregion

        #region public

        public IMatrix DCDA
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
        public IMatrix DADZ
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
        public IMatrix Delta
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
        public IMatrix WeightsChange
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
        public IMatrix BiasesChange
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

        public void ProcessDelta(IMatrix target, ICostFunction costFunction)
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
                PerformantOperations.SetScalarProduct(Delta, ReceptiveField.Output.GetTranspose(), WeightsChange);
                PerformantOperations.Multiplicate(WeightsChange, -learningRate, WeightsChange);

                // Add Change to Weights
                PerformantOperations.Add(Weights, WeightsChange, Weights);

                if (Biases != null)
                {
                    #region slow but tested

                    IMatrix etaTimesDelta = new Matrix(Biases.m);   // perf!

                    PerformantOperations.Multiplicate(Delta, learningRate, etaTimesDelta);  // check!
                    PerformantOperations.Subtract(Biases, etaTimesDelta, Biases);   // check!

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

        private void SetDCDA(IMatrix target, ICostFunction costFunction)
        {
            if (ProjectiveField == null)
            {
                DCDA.ForEach(x => 0);
                costFunction.Derivation(Output, target, DCDA);
            }
            else
            {
                PerformantOperations.SetScalarProduct(ProjectiveField.Weights.GetTranspose(), (ProjectiveField as ILearningLayer).Delta, DCDA);   // ta cast vs perf -> LearningNet incl LearningLayers?
            }
        }
        private void SetDADZ()
        {
            DADZ.ForEach(x => 0);
            ActivationFunction.Derivation(Input, DADZ);
        }
        private void SetDelta()
        {
            // Delta.ForEach(x => 0);
            PerformantOperations.SetHadamardProduct(DCDA, DADZ, Delta);
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
            result += $"{Input?.ToLog()}";
            result += $"{Output?.ToLog()}";
            result += $"{Weights?.ToLog()}";
            result += $"{Biases?.ToLog()}";
            result += $"{DCDA?.ToLog()}";
            result += $"{DADZ?.ToLog()}";
            result += $"{Delta?.ToLog()}";

            return result;
        }

        #endregion
    }
}
