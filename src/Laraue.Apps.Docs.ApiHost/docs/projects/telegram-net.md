---
title: Telegram.NET
type: project
tags: [Telegram,.NET,C#]
description: Allows to write ASP NET like telegram controllers with middlewares and auth.
---
## Key features
| Feature      | Value                                                                       |
|--------------|-----------------------------------------------------------------------------|
| Language     | C#                                                                          |
| Framework    | NET9                                                                        |
| Project type | Library                                                                     |
| Status       | Active Development                                                          |
| License      | MIT                                                                         |
| Nuget        | ![latest version](https://img.shields.io/nuget/v/Laraue.Telegram.NET.Core)  |
| Downloads    | ![latest version](https://img.shields.io/nuget/dt/Laraue.Telegram.NET.Core) |
| Github       | [Laraue.Telegram.NET](https://github.com/win7user10/Laraue.Telegram.NET)    |

## A few more words about Telegram bots
Telegram offers a versatile platform, allowing to build bots for automating tasks, create interactive tools. It provides a
low-barrier entry point to learn bot development, automate project workflows, and create engaging interfaces for the applications,
eliminating from the requirement to invent interface blocks, design etc.

## The main problems tried to be solved
The Telegram API is designed to provide an easy start of bots creating. It doesn't impose to think about the app architecture,
and sometimes it leads to the apps with a big if-else entrypoint that is hard to maintain. Something like that
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
The library should help the engineer to write the applications without spaghetti code.

## The library vision
There is no requirement to invent something new in architecture when all is already invented. Controllers from an MVC pattern
sounds nice for the case. The library should allow writing ASP Net like controllers for Telegram and mark methods
with attributes, like `[TelegramMessage("/new")]` when the message was taken or `[TelegramCallbackRoute("/answer")]` when 
the callback response received. The engineer always sees all defined commands the bot can react that make the development
and support processes easier.

## How to use the library

### Controller define
An engineer should define telegram controllers in the code
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
The services requested in constructor will be resolved from the Microsoft DI container. The attribute
`TelegramMessageRoute("/start")` means the user message `"/start"` will be processed by the method above. 
The parameter `TelegramRequestContext` will contain the object of the request and allows to directly get the `Message` object. 

### The library registration
The engineer should decide how to handle telegram requests. There are two ways

#### Webhooks
The way means the engineer will set the host address in Telegram, calling the endpoint 
`https://api.telegram.org/bot(token)/setWebhook?url=https://site/address-no-one-knows`.
At the client side can be set the url for handling telegram requests.
The request usually starts to handle immediately after the user wrote something.
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelegramCore(new TelegramBotClientOptions("51182636:AAHiPDQ8kVcbs2WZWG4Z..."));
var app = builder.Build();
app.MapTelegramRequests("address-no-one-knows");
app.Run();
```

#### Long pooling
The way means the application will call Telegram to get new updates for the bot. It is suitable for local environment for 
test purposes (there is no easy way to provide a public host for local environment). Also, can be used in the cases when
the application load is too much to make horizontal scaling at the application level.
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelegramCore(new TelegramBotClientOptions("51182636:AAHiPDQ8kVcbs2WZWG4Z..."));
builder.Services.AddTelegramLongPoolingService();
var app = builder.Build();
app.Run();
```

### Authentication
To work with authentication, the developer should define the `User<UserKey>` object and register the new functionality
in the container
```csharp
services.AddTelegramCore()
    .AddTelegramAuthentication<User, Guid, TelegramUserQueryService, RequestContext>();
```
The first generic arguments are known. 
The `TelegramUserQueryService` implements [ITelegramUserQueryService](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Authentication/Services/ITelegramUserQueryService.cs),
that allows a library to now how to get the system user by the telegram user id and how to store a new user object.
The `RequestContext` is just the little class that will be captured in controllers, to avoid taken `TelegramRequestContext<Guid>` as argument.
```csharp
public sealed class RequestContext : TelegramRequestContext<Guid> // Here Guid is the key of TUser
{
}
```

Now the user information can be taken in Telegram controllers
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
Some endpoints can be available only for users with extended permissions. To reach that define the roles constants.
```csharp
public static class Roles
{
    public const string Admin = "Admin";
}
```
Set the attribute for the protected endpoints.
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
And the developer should implement the class that will explain, what is the role related to the specified user 
implementing the [IUserRoleProvider](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Authentication/Services/IUserRoleProvider.cs).
After the registration in the container, the authorization should work.
```csharp
.AddScoped<IUserRoleProvider, UserRoleProvider>()
```
Another option is to use the already written [class](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Authentication/Services/StaticUserRoleProvider.cs)
that loads user roles from the application options.

#### Middlewares
The functionality to extend the request pipeline or interrupt request before the application layer as on ASP NET.
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

Register the middleware in the container
```csharp
services.AddTelegramMiddleware<LogExceptionsMiddleware>();
```

#### Localization
To enable the functionality, needs to implement the inheritor of [BaseCultureInfoProvider](https://github.com/win7user10/Laraue.Telegram.NET/blob/master/src/Laraue.Telegram.NET.Localization/BaseCultureInfoProvider.cs) that will return the user culture.
The base implementation could look like that
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
then set up the localization
```csharp
.AddTelegramRequestLocalization<LocalizationProvider>()
.Configure<TelegramRequestLocalizationOptions>(opt =>
{
    opt.AvailableLanguages = ["en", "fr"];
    opt.DefaultLanguage = ["en"];
})
```
Now the developer can use default Microsoft functionality with define language phrases in different `resx` files. As example, 
having hierarchy
`Resources/Buttons.resx`
`Resources/Buttons.fr.resx`
and call `Resources.Buttons.Menu` from the C# code. It will get the French phrase when the user auth has French language,
otherwise the default phrase will be returned.

## Challenges
The main solved problems will be described in the separated articles
- How to make architecture allows adding only functionality the user required 
- How to make architecture extendable, allows the end user to add new request handling features

## Timeline
- **Jan 2023** Base version with core functionality based on webhooks
- **Feb 2023** Authorization package added
- **Jan 2024** Localization package added
- **Aug 2025** Long pooling mode added

## Real use cases
The library is widely used in the [Learn Language](learn-language) project and is responsible for all communication with telegram.
Also, the project [SPB Real Estate](real-estate) use this library to provide the Telegram interface.