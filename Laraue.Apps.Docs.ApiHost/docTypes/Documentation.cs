using Laraue.CmsBackend;

namespace Laraue.Apps.Docs.ApiHost.docTypes;

public class Documentation : BaseContentType
{
    public required string Project { get; set; }
}