using org.neurul.Cortex.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace org.neurul.Cortex.Client.Out
{
    public interface INeuronQueryClient
    {
        Task<NeuronData> GetNeuronById(string avatarId, Guid id, CancellationToken token = default(CancellationToken)); 
    }
}
