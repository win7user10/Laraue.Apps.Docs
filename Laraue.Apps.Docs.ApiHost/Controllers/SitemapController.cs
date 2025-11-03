using Laraue.CmsBackend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laraue.Apps.Docs.ApiHost.Controllers;

public class SitemapController(ISitemapGenerator sitemapGenerator, IOptions<SiteOptions> options) : ControllerBase
{
    [HttpGet("sitemap.xml")]
    public ContentResult Get()
    {
        var items = sitemapGenerator.GetItems();
        var sitemap = sitemapGenerator.GenerateSitemap(
            new GenerateSitemapRequest { BaseAddress = options.Value.SitemapBaseAddress },
            items);

        return Content(sitemap, "text/xml");
    }
}