using Laraue.CmsBackend;

namespace Laraue.Apps.Docs.ApiHost.docTypes;

public class Article : BaseContentType
{
    public required string[] Projects { get; init; }
    public required string Description { get; init; }
}