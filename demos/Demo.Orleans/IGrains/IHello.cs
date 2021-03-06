using Orleans;
using System.Threading.Tasks;

namespace Demo.Orleans
{
    internal interface IHello : IGrainWithIntegerKey
    {
        Task Say(string name);
    }
}
