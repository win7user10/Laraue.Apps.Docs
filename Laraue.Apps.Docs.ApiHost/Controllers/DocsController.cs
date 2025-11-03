using Laraue.CmsBackend;
using Laraue.CmsBackend.Contracts;
using Laraue.Core.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Laraue.Apps.Docs.ApiHost.Controllers;

[ApiController]
[Route("api/docs")]
public class DocsController(ICmsBackend cmsBackend) : ControllerBase
{
    [HttpPost("single")]
    public Dictionary<string, object> Get([FromBody] GetEntityRequest request)
    {
        return cmsBackend.GetEntity(request);
    }
    
    [HttpPost("list")]
    public IShortPaginatedResult<Dictionary<string, object>> Get([FromBody] GetEntitiesRequest request)
    {
        return cmsBackend.GetEntities(request);
    }
    
    [HttpPost("property-values-count")]
    public List<CountPropertyRow> CountValues([FromBody] CountPropertyValuesRequest request)
    {
        return cmsBackend.CountPropertyValues(request);
    }
    
    [HttpPost("sections")]
    public List<SectionItem> GetSections([FromBody] GetSectionsRequest request)
    {
        return cmsBackend.GetSections(request);
    }
}