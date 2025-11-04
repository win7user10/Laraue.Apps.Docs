using Laraue.CmsBackend;

namespace Laraue.Apps.Blog.ApiHost.docTypes;

public class Documentation : BaseContentType
{
    public required string Project { get; set; }
}