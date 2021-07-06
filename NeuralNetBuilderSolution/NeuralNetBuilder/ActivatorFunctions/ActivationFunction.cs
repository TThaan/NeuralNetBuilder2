using MatrixExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;


namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public abstract class ActivationFunction    // Better as ext meths?
    {
        [JsonConverter(typeof(StringEnumConverter))]    // Or just as int..(The json/text file is better readable with string value!)?
        public ActivationType ActivationType { get; internal set; } // redundant?

        public abstract float Activation(float weightedInput);
        public abstract float Derivation(float weightedInput);
        public virtual void Activation(float[] weightedInput, float[] result)
        {
            weightedInput.ForEach(x => Activation(x), result);
        }
        public virtual void Derivation(float[] weightedInput, float[] result)
        {
            weightedInput.ForEach(x => Derivation(x), result);
        }
    }
}
