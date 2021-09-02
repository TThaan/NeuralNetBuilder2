using MatrixExtensions;

namespace NeuralNet_Core.CostFunctions
{
    public class SquaredMeanError : ICostFunction
    {
        public void Cost(float[] output, float[] target, float[] result) 
        {
            // result = (target - output) * (target - output); //.5f * 
            target.Subtract(output, result);
            result.Multiply_Elementwise(result, result);
        }
        public void Derivation(float[] output, float[] target, float[] result)
        {
            output.Subtract(target, result);
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
