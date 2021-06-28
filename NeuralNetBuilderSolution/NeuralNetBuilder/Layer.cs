using CustomLogger;
using MatrixExtensions;
using NeuralNetBuilder.ActivatorFunctions;
using System;

namespace NeuralNetBuilder
{
    public interface ILayer : ILoggable
    {
        int Id { get; set; }
        int N { get; set; }
        float[] Input { get; set; }
        float[] Output { get; set; }
        float[,] Weights { get; set; }
        float[] Biases { get; set; }
        ActivationFunction ActivationFunction { get; set; }
        ILayer ReceptiveField { get; set; }
        ILayer ProjectiveField { get; set; }
        void ProcessInput(float[] originalInput = null);
    }

    [Serializable]
    public class Layer : ILayer
    {
        #region fields & ctor

        int id, n;
        float[] input, output, biases;
        float[,] weights;
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
        public float[] Input
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
        public float[] Output
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
        public float[,] Weights
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
        public float[] Biases
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

        public void ProcessInput(float[] originalInput = null)
        {
            SetInput(originalInput);
            SetOutput();
            ProjectiveField?.ProcessInput();
        }

        #endregion

        #region helpers

        void SetInput(float[] originalInput)
        {
            if (originalInput != null)
            {
                Input = originalInput.ForEach(x => x);
            }
            else
            {
                // Input.ForEach(x => 0); // check!

                Input = Weights.Multiply_MatrixWithColumnVector(ReceptiveField.Output);
                //PerformantOperations.SetScalarProduct(Weights, ReceptiveField.Output, Input);
            }

            if (Biases != null)
                Input = Input.Add(Biases);
                //PerformantOperations.Add(Input, Biases, Input); // check!
        }
        void SetOutput()
        {
            Output?.ForEach(x => 0); // check!

            Output = ActivationFunction.Activation(Input);
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
            result += $"{Input?.ToLog(nameof(Input))}";
            result += $"{Output?.ToLog(nameof(Output))}";
            result += $"{Weights?.ToLog(nameof(Weights))}";
            result += $"{Biases?.ToLog(nameof(Biases))}";

            return result;
        }

        #endregion
    }
}
