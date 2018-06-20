# Lykke.Service.HftInternalService

Lykke internal service for creation and management of high-frequency trading wallets and it's api-keys.

Nuget: https://www.nuget.org/packages/Lykke.Service.HftInternalService.Client/

[Refit](https://github.com/reactiveui/refit) client can be generated using the [Lykke HttpClientGenerator](https://github.com/LykkeCity/Lykke.HttpClientGenerator) or the default Refit ```RestService```.

### Service endpoints

#### IsAlive

```csharp
public bool IsHftInterserviceAlive() {
    var generator = HttpClientGenerator
        .BuildForUrl(serviceUrl)
        .Create();
    var client = generator.Generate<IHftInternalServiceApi>();

    try {
        var isAlive = await client.IsAlive();
        return true;
    }
    catch (Execption) {
        return false;
    }
}
```

#### Key management 

```csharp
var generator = HttpClientGenerator
    .BuildForUrl(serviceUrl)
    .Create();
var client = generator.Generate<IKeysApi>();

// Get information for a specific key
var key = await client.GetKey("MY-API-KEY");

// Get all keys of a client.
var keys = await keysClient.GetKeys(clientId);

// Create a new api wallet.
var created = await keysClient.CreateKey(new CreateApiKeyModel
{
    ClientId = clientId,
    Description = "My api wallet",
    Name = "ApiWallet"
});

// Update existing api wallet.
var updated = await keysClient.UpdateKey(new UpdateApiKeyModel
{
    ClientId = clientId,
    WalletId = created.WalletId
});

// Remove a api wallet.
wallet = await keysClient.GetKey(created.ApiKey);

```

