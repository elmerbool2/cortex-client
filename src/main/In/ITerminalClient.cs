using neurUL.Cortex.Common;
using System.Threading;
using System.Threading.Tasks;

namespace neurUL.Cortex.Client.In
{
    public interface ITerminalClient
    {
        Task CreateTerminal(string cortexInBaseUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string authorId, CancellationToken token = default(CancellationToken));
        Task DeactivateTerminal(string cortexInBaseUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken));
    }
}
