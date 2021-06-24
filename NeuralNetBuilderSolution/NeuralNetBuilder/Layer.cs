using CustomLogger;
using MatrixHelper;
using NeuralNetBuilder.ActivatorFunctions;
using System;

namespace NeuralNetBuilder
{
    public interface ILayer : ILoggable
    {
        int Id { get; set; }
        int N { get; set; }
        IMatrix Input { get; set; }
        IMatrix Output { get; set; }
        IMatrix Weights { get; set; }
        IMatrix Biases { get; set; }
        ActivationFunction ActivationFunction { get; set; }
        ILayer ReceptiveField { get; set; }
        ILayer ProjectiveField { get; set; }
        void ProcessInput(IMatrix originalInput = null);
    }

    [Serializable]
    public class Layer : ILayer
    {
        #region fields & ctor

        int id, n;
        IMatrix input, output, weights, biases;
        ILayer receptiveField, projectiveField;
        ActivationFunction activationFunction;

        public Layer() { }// internal?

        #endregion

        #region public

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    // OnPropertyChanged();
                }
            }
        }
        public int N
        {
            get { return n; }
            set
            {
                if (n != value)
                {
                    n = value;
                    // OnPropertyChanged();
                }
            }
        }
        public IMatrix Input
        {
            get { return input; }
            set
            {
                if (input != value)
                {
                    input = value;
                    // OnPropertyChanged();
                }
            }
        }        
        public IMatrix Output
        {
            get { return output; }
            set
            {
                if (output != value)
                {
                    output = value;
                    // OnPropertyChanged();
                }
            }
        }
        public ActivationFunction ActivationFunction
        {
            get { return activationFunction; }
            set
            {
                if (activationFunction != value)
                {
                    activationFunction = value;
                    // OnPropertyChanged();
                }
            }
        }
        public IMatrix Weights
        {
            get { return weights; }
            set
            {
                if (weights != value)
                {
                    weights = value;
                    // OnPropertyChanged();
                }
            }
        }
        public IMatrix Biases
        {
            get { return biases; }
            set
            {
                if (biases != value)
                {
                    biases = value;
                    // OnPropertyChanged();
                }
            }
        }
        public ILayer ReceptiveField
        {
            get { return receptiveField; }
            set
            {
                if (receptiveField != value)
                {
                    receptiveField = value;
                    // OnPropertyChanged();
                }
            }
        }
        public ILayer ProjectiveField
        {
            get { return projectiveField; }
            set
            {
                if (projectiveField != value)
                {
                    projectiveField = value;
                    // OnPropertyChanged();
                }
            }
        }

        public void ProcessInput(IMatrix originalInput = null)
        {
            SetInput(originalInput);
            SetOutput();
            ProjectiveField?.ProcessInput();
        }

        #endregion

        #region helpers

        void SetInput(IMatrix originalInput)
        {
            if (originalInput != null)
            {
                Input.ForEach(originalInput, x => x);
            }
            else
            {
                // Input.ForEach(x => 0); // check!
                PerformantOperations.SetScalarProduct(Weights, ReceptiveField.Output, Input);
            }

            if (Biases != null)
                PerformantOperations.Add(Input, Biases, Input); // check!
        }
        void SetOutput()
        {
            Output?.ForEach(x => 0); // check!
            ActivationFunction.Activation(Input, Output);
        }

        #endregion

        #region ILogger

        public string LoggableName => $"{GetType().Name} {Id}";
        public virtual string ToLog()
        {
            string result = LoggableName;

            result += $" ({nameof(Id)}: {Id}";
            result += $", Neurons: {N}";
            result += $", {nameof(ActivationFunction)}: {ActivationFunction?.GetType().Name}";
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

            return result;
        }

        #endregion
    }
}
