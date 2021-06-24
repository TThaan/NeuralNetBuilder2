using CustomLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    //public class InitializerFactory
    //{
    //    public static IInitializer GetInitializer(INetParameters netParameters, ITrainerParameters trainerParameters, ILogger logger)
    //    {
    //        IInitializer result = new Initializer();

    //        switch (netParameters.Mode)
    //        {
    //            case NetCreationMode.Create:
    //                result.Net = result.GetNet(netParameters);
    //                break;
    //            case NetCreationMode.CreateAndSave:
    //                result.Net = result.GetNet(netParameters, new FileInfo(netParameters.FileName));
    //                break;
    //            case NetCreationMode.Load:
    //                result.Net = result.GetNet(new FileInfo(netParameters.FileName));
    //                break;
    //        }

    //        result.Trainer = result.GetTrainer(result.Net, trainerParameters, logger);
    //        // result.Trainer.StatusChanged += _mainVM.StatusVM.Trainer_StatusChanged;

    //        return result;
    //    }
    //    public static async Task<IInitializer> GetInitializerAsync(INetParameters netParameters, ITrainerParameters trainerParameters, ILogger logger)
    //    {
    //        return await Task.Run(() =>
    //        {
    //            return GetInitializer(netParameters, trainerParameters, logger);
    //        });
    //    }
    //}
}
