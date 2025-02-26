﻿namespace NeuralNet_Core
{
    public enum LayerType
    {
        Layer, LearningLayer
    }
    public enum ActivationType
    {
        None,
        LeakyReLU, NullActivator,
        ReLU, Sigmoid, SoftMax, SoftMaxWithCrossEntropyLoss, Tanh
    }
    public enum CostType
    {
        SquaredMeanError
    }
    public enum WeightInitType
    {
        None, Xavier,
    }

    // Remove
    //public enum NetStatus
    //{
    //    Undefined, Initialized  // Raw instead of Undefined?
    //}
    public enum TrainerStatus
    {
        Undefined, Initialized, Running, Paused, Finished   // Raw instead of Undefined?
    }
}