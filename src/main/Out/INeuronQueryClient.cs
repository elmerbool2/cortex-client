using neurUL.Cortex.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace neurUL.Cortex.Client.Out
{
    public interface INeuronQueryClient
    {
        Task<NeuronData> GetNeuronById(string avatarId, Guid id, CancellationToken token = default(CancellationToken)); 
    }
}
