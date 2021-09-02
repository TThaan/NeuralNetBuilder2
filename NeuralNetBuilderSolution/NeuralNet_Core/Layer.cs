using CustomLogger;
using MatrixExtensions;
using NeuralNet_Core.ActivatorFunctions;
using NeuralNet_Core.FactoriesAndParameters.JsonConverters;
using Newtonsoft.Json;
using System;

namespace NeuralNet_Core
{
    // wa: IBaseLayer?
    public interface ILayer : ILoggable
    {
        int Id { get; set; }
        int N { get; set; }
        float[] Input { get; set; }
        float[] Output { get; set; }
        float[,] Weights { get; set; }
        float[,] WeightsTransposed { get; set; }
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
        float[,] weights, weightsTransposed;
        ILayer receptiveField, projectiveField;
        ActivationFunction activationFunction;

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
        [JsonConverter(typeof(JsonConverter_ActivatorFunctions))]
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
        public float[,] WeightsTransposed
        {
            get { return weightsTransposed == null ? weightsTransposed = weights?.Transpose() : weightsTransposed ; }
            set
            {
                if (weightsTransposed != value)
                {
                    weightsTransposed = value;
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
        [JsonIgnore]
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
        [JsonIgnore]
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
                originalInput.ForEach(x => x, Input);
            }
            else
            {
                // Input.ForEach(x => 0); // check!

                Weights.Multiply_MatrixWithColumnVector(ReceptiveField.Output, Input);
                //PerformantOperations.SetScalarProduct(Weights, ReceptiveField.Output, Input);
            }

            if (Biases != null)
                Input.Add(Biases, Input);
                //PerformantOperations.Add(Input, Biases, Input); // check!
        }
        void SetOutput()
        {
            Output?.ForEach(x => 0, Output); // check! redundant?

            ActivationFunction.Activation(Input, Output);
        }

        #endregion

        #region ILogger

        public string LoggableName => $"{GetType().Name} {Id}";
        public virtual string ToLog(Details details = Details.All)
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

            if (details == Details.Little)
                return result;
            
            result += $"{Input.ToLog(nameof(Input))}";
            result += $"{Output.ToLog(nameof(Output))}";

            if (details == Details.Medium)
                return result;

            result += $"{Weights.ToLog(nameof(Weights))}";
            result += $"{Biases.ToLog(nameof(Biases))}";

            return result;
        }

        #endregion
    }
}
