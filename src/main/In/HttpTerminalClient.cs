using NLog;
using neurUL.Common.Http;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using neurUL.Cortex.Common;

namespace neurUL.Cortex.Client.In
{
    public class HttpTerminalClient : ITerminalClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpTerminalClient.logger.Error(ex, "Error occurred while communicating with neurUL Cortex. " + ex.InnerException?.Message)
            );

        private static readonly string terminalsPath = "cortex/terminals/";
        private static readonly string terminalsPathTemplate = terminalsPath + "{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpTerminalClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task CreateTerminal(string cortexInBaseUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpTerminalClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateTerminalInternal(cortexInBaseUrl, id, presynapticNeuronId, postsynapticNeuronId, effect, strength, authorId, token).ConfigureAwait(false));

        public async Task CreateTerminalInternal(string cortexInBaseUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Id = id,
                PresynapticNeuronId = presynapticNeuronId,
                PostsynapticNeuronId = postsynapticNeuronId,
                Effect = effect.ToString(),
                Strength = strength.ToString(),
                AuthorId = authorId
            };

            await this.requestProvider.PostAsync(
               $"{cortexInBaseUrl}{HttpTerminalClient.terminalsPath}",
               data
               );
        }

        public async Task DeactivateTerminal(string cortexInBaseUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpTerminalClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.DeactivateTerminalInternal(cortexInBaseUrl, id, expectedVersion, authorId, token).ConfigureAwait(false));

        public async Task DeactivateTerminalInternal(string cortexInBaseUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                AuthorId = authorId
            };

            await this.requestProvider.DeleteAsync<object>(
               $"{cortexInBaseUrl}{string.Format(HttpTerminalClient.terminalsPathTemplate, id)}",
               null,
               token: token,
               headers: new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }
    }
}
