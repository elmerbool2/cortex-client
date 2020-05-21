using NLog;
using neurUL.Common.Http;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace neurUL.Cortex.Client.In
{
    public class HttpNeuronClient : INeuronClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpNeuronClient.logger.Error(ex, "Error occurred while communicating with neurUL Cortex. " + ex.InnerException?.Message)
            );

        private static readonly string neuronsPath = "cortex/neurons/";
        private static readonly string neuronsPathTemplate = neuronsPath + "{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpNeuronClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task CreateNeuron(string cortexInBaseUrl, string id, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpNeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateNeuronInternal(cortexInBaseUrl, id, authorId, token).ConfigureAwait(false));

        private async Task CreateNeuronInternal(string cortexInBaseUrl, string id, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Id = id,
                AuthorId = authorId
            };

            await this.requestProvider.PostAsync(
               $"{cortexInBaseUrl}{HttpNeuronClient.neuronsPath}",
               data
               );
        }

        public async Task DeactivateNeuron(string cortexInBaseUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpNeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.DeactivateNeuronInternal(cortexInBaseUrl, id, expectedVersion, authorId, token));

        private async Task DeactivateNeuronInternal(string cortexInBaseUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                AuthorId = authorId
            };

            await this.requestProvider.DeleteAsync<object>(
               $"{cortexInBaseUrl}{string.Format(HttpNeuronClient.neuronsPathTemplate, id)}",
               data,
               token: token,
               headers: new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }
    }
}
