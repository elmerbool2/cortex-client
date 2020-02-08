using org.neurul.Cortex.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.EventSourcing.Client.Out
{
    public interface INeuronQueryClient
    {
        Task<NeuronData> GetNeuronById(string avatarId, Guid id, CancellationToken token = default(CancellationToken)); 
    }
}
