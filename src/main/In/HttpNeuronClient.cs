using NLog;
using org.neurul.Common.Http;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace org.neurul.Cortex.Client.In
{
    public class HttpNeuronClient : INeuronClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpNeuronClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static readonly string neuronsPath = "cortex/neurons/";
        private static readonly string neuronsPathTemplate = neuronsPath + "{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpNeuronClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task CreateNeuron(string avatarUrl, string id, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpNeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateNeuronInternal(avatarUrl, id, authorId, token).ConfigureAwait(false));

        private async Task CreateNeuronInternal(string avatarUrl, string id, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Id = id,
                AuthorId = authorId
            };

            await this.requestProvider.PostAsync(
               $"{avatarUrl}{HttpNeuronClient.neuronsPath}",
               data
               );
        }

        public async Task DeactivateNeuron(string avatarUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpNeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.DeactivateNeuronInternal(avatarUrl, id, expectedVersion, authorId, token));

        private async Task DeactivateNeuronInternal(string avatarUrl, string id, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken))
        {
            await this.requestProvider.DeleteAsync<object>(
               $"{avatarUrl}{string.Format(HttpNeuronClient.neuronsPathTemplate, id)}",
               null,               
               token: token,
               headers: new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }
    }
}
