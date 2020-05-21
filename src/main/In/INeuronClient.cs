using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace neurUL.Cortex.Client.In
{
    public interface INeuronClient
    {
        Task CreateNeuron(string cortexInBaseUrl, string id, string authorId, CancellationToken token = default(CancellationToken));

        Task DeactivateNeuron(string cortexInBaseUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken));
    }
}
