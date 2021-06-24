namespace NeuralNetBuilder
{
    public interface INotifyStatusChanged
    {
        event TrainerStatusChangedEventHandler StatusChanged;
    }
}
