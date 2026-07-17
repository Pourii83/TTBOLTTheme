using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Services;

public interface IRelatedBlogPostService
{
    Task<IList<RelatedBlogPost>> GetRelatedBlogPostsAsync(int blogPostId);

    Task ReplaceRelatedBlogPostsAsync(int blogPostId, IList<int> relatedBlogPostIds);

    Task DeleteRelatedBlogPostsAsync(int blogPostId);
}
