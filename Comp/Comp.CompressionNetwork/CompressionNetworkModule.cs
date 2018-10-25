using Comp.CompressionNetwork.Network;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Comp.CompressionNetwork
{
    public class CompressionNetworkModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<INeuralNetworkFactory>().ToFactory();
        }
    }
}
