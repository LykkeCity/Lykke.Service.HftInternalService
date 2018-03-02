using MessagePack;

namespace Lykke.Service.HftInternalService.Services.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class DeleteApiKeyCommand
    {
        public string ApiKey { get; set; }
    }
}
