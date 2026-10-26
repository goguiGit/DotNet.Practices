# Translation

Componente real portado de Woffu.Services.BackOffice: traducción de textos con un LLM a través de `IChatClient` (Microsoft.Extensions.AI).

- `Interfaces/ITranslationEngine`: `TranslateAsync(from, to, text, ct)`. Es lo que inyectan los handlers; nadie depende de la clase concreta.
- `TranslationEngine`: compone system + user message, llama a `IChatClient.GetResponseAsync`, limpia la respuesta. Texto vacío devuelve vacío sin llamar al modelo. Cualquier excepción se loguea y devuelve vacío: la traducción es best effort.
- `Language`: enum propio del ejemplo (en Woffu viene de `Woffu.Library.Common`).

## Cómo se instala en Woffu (replicar en cualquier servicio)

- `services.AddScoped<ITranslationEngine, TranslationEngine>()` en la capa Application.
- `IChatClient` como **singleton** en la capa API: `new AzureOpenAIClient(endpoint, credential).GetChatClient(deployment).AsIChatClient()`.
- Config en `OpenAI:Endpoint`, `OpenAI:ApiKey`, `OpenAI:DeploymentName`. Nunca en `appsettings.json`: user-secrets en local, Key Vault en entornos. Sin `ApiKey` se usa `DefaultAzureCredential`.
- Si falta `OpenAI:Endpoint` se registra un `NoOpChatClient` que lanza al usarse: la app arranca sin secrets y el mal uso se ve al momento, no en silencio.
- Dependencias: la librería solo referencia `Microsoft.Extensions.AI.Abstractions`; `Azure.AI.OpenAI` y `Microsoft.Extensions.AI.OpenAI` solo donde se construye el cliente (API o tests de integración).

## Tests

- Unitarios con `FakeChatClient : IChatClient` en el proyecto de tests (sin Moq): respuesta fija, excepción opcional, captura los mensajes enviados. `NullLogger<T>.Instance` como logger.
- Integración (`[Category("Integration")]`) contra Azure OpenAI real. Lee user-secrets con `AddUserSecrets<T>()` y hace `Assert.Ignore` si no están: pasa en CI y en máquinas sin credenciales.
- `UserSecretsId` del proyecto de tests: `DotNet.Practices.Secrets`. Para rellenarlo:

```
dotnet user-secrets set "OpenAI:Endpoint" "https://<recurso>.openai.azure.com/" --project tests/DotNet.Practices.Abstractions.Tests
dotnet user-secrets set "OpenAI:ApiKey" "<clave>" --project tests/DotNet.Practices.Abstractions.Tests
dotnet user-secrets set "OpenAI:DeploymentName" "gpt-4o" --project tests/DotNet.Practices.Abstractions.Tests
```

Tests en `tests/DotNet.Practices.Abstractions.Tests/Translation/`.
