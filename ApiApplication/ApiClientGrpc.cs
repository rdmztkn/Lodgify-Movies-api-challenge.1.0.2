using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoDefinitions;
using static ProtoDefinitions.MoviesApi;

namespace ApiApplication
{
    public class ApiClientGrpc
    {
        private readonly MoviesApiClient client;

        private static readonly HttpClientHandler HttpHandler = new()
        {
            ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        private static Metadata MetadataHeaders = new Metadata()
    {
        new Metadata.Entry("X-Apikey", "68e5fbda-9ec9-4858-97b2-4a8349764c63")
    };

        private static GrpcChannel channel =
                GrpcChannel.ForAddress("https://localhost:7443/", new GrpcChannelOptions()
                {
                    HttpHandler = HttpHandler
                });

        public ApiClientGrpc()
        {
            client = new MoviesApiClient(channel);
        }

        public async Task<showListResponse> GetAll()
        {
            var all = await client.GetAllAsync(new Empty(), headers: MetadataHeaders);
            all.Data.TryUnpack<showListResponse>(out var data);
            return data;
        }

        public async Task<showResponse> GetById(string id)
        {
            var movie = await client.GetByIdAsync(new IdRequest() { Id = id }, headers: MetadataHeaders);

            movie.Data.TryUnpack<showResponse>(out var data);

            return data;
        }
    }
}