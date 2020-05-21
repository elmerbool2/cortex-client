using neurUL.Cortex.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace neurUL.Cortex.Client.Out
{
    public interface INeuronQueryClient
    {
        Task<NeuronData> GetNeuronById(string cortexOutBaseUrl, Guid id, CancellationToken token = default(CancellationToken)); 
    }
}
