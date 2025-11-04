using Laraue.Apps.Docs.ApiHost;
using Laraue.Apps.Docs.ApiHost.docTypes;
using Laraue.CmsBackend;
using Laraue.CmsBackend.Extensions;
using Laraue.CmsBackend.MarkdownTransformation;
using Laraue.Core.Exceptions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ExceptionHandleMiddleware>();

var cmsBackend = new CmsBackendBuilder(new MarkdownParser(new MarkdownToHtmlTransformer(), new ArticleInnerLinksGenerator()), new MarkdownProcessor())
    .AddContentType<Project>()
    .AddContentType<Article>()
    .AddContentType<Documentation>()
    .AddContentFolder("blog")
    .Build();

builder.Services.AddSingleton(cmsBackend);
builder.Services.AddSingleton<ISitemapGenerator, SitemapGenerator>();

builder.Services.AddOptions<SiteOptions>();
builder.Services.Configure<SiteOptions>(builder.Configuration.GetSection("SiteOptions"));

var app = builder.Build();

var origins = builder
    .Configuration
    .GetRequiredSection("Cors:Hosts")
    .Get<string[]>() ?? throw new InvalidOperationException();

app.UseCors(corsPolicyBuilder =>
    corsPolicyBuilder.WithOrigins(origins)
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseMiddleware<ExceptionHandleMiddleware>();
app.MapControllers();
app.Run();