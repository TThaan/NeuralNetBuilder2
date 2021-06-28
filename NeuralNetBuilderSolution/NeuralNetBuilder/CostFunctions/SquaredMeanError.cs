using MatrixExtensions;

namespace NeuralNetBuilder.CostFunctions
{
    public class SquaredMeanError : ICostFunction
    {
        public float[] Cost(float[] output, float[] target) 
        {
            // result = (target - output) * (target - output); //.5f * 
            var tmp = target.Subtract(output);
            return tmp.Multiply_Elementwise(tmp);
        }
        public float[] Derivation(float[] output, float[] target)
        {
            return output.Subtract(target);
        }
        public float GetTotalCost(float[] output, float[] target)
        {
            float result = 0;

            for (int j = 0; j < output.Length; j++)
            {
                result += (target[j] - output[j]) * (target[j] - output[j]);    // .5f * // PerfOps
            }

            return result;
        }
    }
}
