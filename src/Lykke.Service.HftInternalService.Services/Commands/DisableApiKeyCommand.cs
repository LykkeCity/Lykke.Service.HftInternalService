using MessagePack;

namespace Lykke.Service.HftInternalService.Services.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class DisableApiKeyCommand
    {
        public string ApiKey { get; set; }
    }
}
