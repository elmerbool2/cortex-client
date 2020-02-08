﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace org.neurul.Cortex.Client.In
{
    public interface INeuronClient
    {
        Task CreateNeuron(string avatarUrl, string id, string authorId, CancellationToken token = default(CancellationToken));

        Task DeactivateNeuron(string avatarUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken));
    }
}
