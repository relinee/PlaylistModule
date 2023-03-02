using Grpc.Core;
using Grpc.Net.Client;
using PlaylistModuleClientApp;

using var channel = GrpcChannel.ForAddress("https://localhost:7298");
var client = new PlaylistService.PlaylistServiceClient(channel);

var emptyReq = new Google.Protobuf.WellKnownTypes.Empty();

try
{
    var reques = client.GetAllSongs(emptyReq);

    foreach (var song in reques.Songs)
    {
        Console.WriteLine($"{song.Id}. {song.Title} - {song.Duration}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}