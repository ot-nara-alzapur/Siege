using Siege.ServiceLocation.Bindings;
using Siege.ServiceLocation.UseCases;

namespace Siege.ServiceLocation.Extensions.FactorySupport
{
    public class ConditionalFactoryUseCase<TService> : FactoryUseCase<TService>, IConditionalUseCase<TService>
    {
        public override System.Type GetUseCaseBindingType()
        {
            return typeof (IConditionalUseCaseBinding<>);
        }
        
    }
}