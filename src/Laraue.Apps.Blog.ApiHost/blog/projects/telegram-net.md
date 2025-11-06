---
title: Telegram.NET
type: project
tags: [Telegram,.NET,C#]
description: Allows to write ASP NET like telegram controllers  middlewares and authentication.
createdAt: 2025-11-01
updatedAt: 2025-11-04
---
## Key Features
|              |                                                                             |
|--------------|-----------------------------------------------------------------------------|
| Language     | C#                                                                          |
| Framework    | NET9                                                                        |
| Project type | Library                                                                     |
| Status       | Completed                                                                   |
| License      | MIT                                                                         |
| Nuget        | ![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Core)  |
| Downloads    | ![latest version](https://img.shields.io/nuget/dt/Laraue.Telegram.NET.Core) |
| Github       | [Laraue.Telegram.NET](https://github.com/win7user10/Laraue.Telegram.NET)    |

## A Few More Words About Telegram Bots
Telegram offers a versatile platform, allowing to build bots for automating tasks, create interactive tools. It provides a
low-barrier entry point to learn bot development, automate project workflows, and create engaging interfaces for the applications,
eliminating from the requirement to invent interface blocks, design etc.

## The Main Problems This Library Tries to Solve
The Telegram API is designed for easy bot creation but doesn't enforce good architectural patterns.
This often leads to monolithic entry points filled with `if-else` chains, which are hard to maintain.
```
if (request.Message?.Text == "/Start")
{
    Start();
}
else if (request.Message?.Text == "/Settings")
{
    OpenSettings()
}
else if (request.Callback?.Data.StartsWith("/ChangeSettings"))
{
    ChangeSettings();
}
// And other commands
```
This library aims to help engineers write clean, maintainable Telegram bots by avoiding spaghetti code.

## The Library Vision
There's no need to reinvent the wheel when well-established patterns like MVC already exist.
The library allows engineers to write ASP.NET-like controllers for Telegram bots using attributes such as:
- `[TelegramMessageRoute("/new")]` for handling messages
- `[TelegramCallbackRoute("/answer")]` for handling callbacks

This makes it easy to see all available bot commands, improving development and maintenance.

## How to Use the Library

### Controller Definition
Define Telegram controllers in code:
```csharp
public class MenuController : TelegramController
{
    private readonly IMenuService _service;

    public SettingsController(IMenuService service)
    {
        _service = service;
    }
    
    [TelegramMessageRoute("/start")]
    public Task ShowMenuAsync(TelegramRequestContext requestContext)
    {
        return _service.HandleStartAsync(requestContext.Update.Message!);
    }
}
```
Services requested in the constructor are resolved from the Microsoft DI container. The attribute
`TelegramMessageRoute("/start")` means the user message `"/start"` will be processed by the method above. 
The parameter `TelegramRequestContext` will contain the object of the request and allows to directly get the `Message` object. 

### Library Registration
The engineer should decide how to handle telegram requests. There are two ways

#### Webhooks
Set the webhook URL in Telegram: `https://api.telegram.org/bot(token)/setWebhook?url=https://site/address-no-one-knows`.
Then map the endpoint in your application:
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelegramCore(new TelegramBotClientOptions("51182636:AAHiPDQ8kVcbs2WZWG4Z..."));
var app = builder.Build();
app.MapTelegramRequests("address-no-one-knows");
app.Run();
```

#### Long Polling
The way means the application will call Telegram to get new updates for the bot.
Useful for local environments or when horizontal scaling isn't feasible:
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelegramCore(new TelegramBotClientOptions("51182636:AAHiPDQ8kVcbs2WZWG4Z..."));
builder.Services.AddTelegramLongPoolingService();
var app = builder.Build();
app.Run();
```

### Authentication
To enable authentication, define a `User<UserKey>` object and register the functionality:
```csharp
services.AddTelegramCore()
    .AddTelegramAuthentication<User, Guid, TelegramUserQueryService, RequestContext>();
```
- `TelegramUserQueryService` implements [ITelegramUserQueryService](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Authentication/Services/ITelegramUserQueryService.cs),
and handles user lookup/store logic.
- `RequestContext` is a wrapper around `TelegramRequestContext<Guid>` to simplify usage.

```csharp
public sealed class RequestContext : TelegramRequestContext<Guid> // Here Guid is the key of TUser
{
}
```
Now, in controllers, you can access user information:
```csharp
public class MenuController : TelegramController
{
    [TelegramMessageRoute("/me")]
    public Task MeAsync(RequestContext request, CancellationToken ct)
    {
        long telegramUserId = request.Update.GetUserId();
        Guid systemUserId = request.UserId;
        
        // other code
    }
}
```
### Autorization
Protect endpoints using roles:
```csharp
public static class Roles
{
    public const string Admin = "Admin";
}
```
Apply the attribute to protected endpoints:
```csharp
public class AdminController : TelegramController
{
    [RequiresUserRole(Roles.Admin)]
    [TelegramMessageRoute("/stat")]
    public Task SendStatAsync(RequestContext request, CancellationToken ct)
    {
        // write a logic
    }
}
```
Implement [IUserRoleProvider](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Authentication/Services/IUserRoleProvider.cs)
to define how roles are assigned to users:
```csharp
.AddScoped<IUserRoleProvider, UserRoleProvider>()
```
Alternatively, use the built-in [StaticUserRoleProvider](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Authentication/Services/StaticUserRoleProvider.cs)
that loads roles from app options.

#### Middlewares
Extend request handling or intercept requests before they reach the app layer, similar to ASP.NET:
```csharp
public class LogExceptionsMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly TelegramRequestContext _telegramRequestContext;

    public LogExceptionsMiddleware(
        ITelegramMiddleware next,
        TelegramRequestContext telegramRequestContext)
    {
        _next = next;
        _telegramRequestContext = telegramRequestContext;
    }
    
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        try
        {
            return await _next.InvokeAsync(ct);
        }
        catch (BadTelegramRequestException ex)
        {
            _logger.LogError(ex, "Error occured");
        }

        return null;
    }
}
```
Register the middleware:
```csharp
services.AddTelegramMiddleware<LogExceptionsMiddleware>();
```
#### Localization
Enable localization by implementing [BaseCultureInfoProvider](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Localization/BaseCultureInfoProvider.cs):
```csharp
public class LocalizationProvider : BaseCultureInfoProvider
{
    private readonly RequestContext _context;
    private readonly IUserRepository _userRepository;

    public LocalizationProvider(
        RequestContext context,
        IOptions<TelegramRequestLocalizationOptions> options,
        ILogger<BaseCultureInfoProvider> logger,
        IUserRepository userRepository)
        : base(context, options, logger)
    {
        _context = context;
        _userRepository = userRepository;
    }

    protected override async Task<TelegramProviderCultureResult> DetermineProviderCultureResultAsync(
        CultureInfo userInterfaceCulture,
        CancellationToken cancellationToken = default)
    {
        var settings = await _userRepository
            .GetSettingsAsync(_context.UserId, cancellationToken);

        return new TelegramProviderCultureResult(
            new CultureInfo(settings.Code),
            new CultureInfo(settings.Code));
    }
}
```
Set up localization:
```csharp
.AddTelegramRequestLocalization<LocalizationProvider>()
.Configure<TelegramRequestLocalizationOptions>(opt =>
{
    opt.AvailableLanguages = ["en", "fr"];
    opt.DefaultLanguage = ["en"];
})
```
Now use standard Microsoft localization features with `resx` files:
```
Resources/Buttons.resx
Resources/Buttons.fr.resx
```
Access localized strings like `Resources.Buttons.Menu` — the correct language is selected automatically based
on the user's settings.

## Challenges
The main solved problems will be described in the separated articles
- How to make the architecture modular so users only include what they need
- How to design an extendable system that allows adding new request handling features

## Timeline
- **Jan 2023** Base version with core functionality based on webhooks
- **Feb 2023** Authorization package added
- **Jan 2024** Localization package added
- **Aug 2025** Long pooling mode added

## Real Use Cases
The library is widely used in the [Learn Language](learn-language) project and is responsible for all communication with telegram.
Also, the project [SPB Real Estate](real-estate) use this library to provide the Telegram interface.