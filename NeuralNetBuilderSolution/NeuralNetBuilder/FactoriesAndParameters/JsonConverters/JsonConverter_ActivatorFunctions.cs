using NeuralNetBuilder.ActivatorFunctions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace NeuralNetBuilder.FactoriesAndParameters.JsonConverters
{
    public class JsonConverter_ActivatorFunctions : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ActivationFunction);    // redundant?
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ActivationFunction result;

            JObject jObj = (JObject)serializer.Deserialize(reader);
            JToken activationTypeToken = jObj["ActivationType"];
            ActivationType activationType = activationTypeToken.ToObject<ActivationType>();

            switch (activationType)
            {
                case ActivationType.LeakyReLU:
                    result = new LeakyReLU();
                    break;
                case ActivationType.NullActivator:
                    result = new NullActivator();
                    break;
                case ActivationType.ReLU:
                    result = new ReLU();
                    break;
                case ActivationType.Sigmoid:
                    result = new Sigmoid();
                    break;
                case ActivationType.SoftMax:
                    result = new SoftMax();
                    break;
                case ActivationType.SoftMaxWithCrossEntropyLoss:
                    result = new SoftMaxWithCrossEntropyLoss();
                    break;
                case ActivationType.Tanh:
                    result = new Tanh();
                    break;
                case ActivationType.None:
                    result = null;
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Serialize(writer, value);
        }
    }
}
