using Nop.Data;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Services;

public class RelatedBlogPostService : IRelatedBlogPostService
{
    private readonly IRepository<RelatedBlogPost> _relatedBlogPostRepository;

    public RelatedBlogPostService(IRepository<RelatedBlogPost> relatedBlogPostRepository)
    {
        _relatedBlogPostRepository = relatedBlogPostRepository;
    }

    public async Task<IList<RelatedBlogPost>> GetRelatedBlogPostsAsync(int blogPostId)
    {
        return await _relatedBlogPostRepository.GetAllAsync(query => query
            .Where(mapping => mapping.BlogPostId == blogPostId)
            .OrderBy(mapping => mapping.DisplayOrder)
            .ThenBy(mapping => mapping.Id));
    }

    public async Task ReplaceRelatedBlogPostsAsync(int blogPostId, IList<int> relatedBlogPostIds)
    {
        await _relatedBlogPostRepository.DeleteAsync(mapping => mapping.BlogPostId == blogPostId);

        var mappings = relatedBlogPostIds
            .Where(relatedBlogPostId => relatedBlogPostId > 0 && relatedBlogPostId != blogPostId)
            .Distinct()
            .Select((relatedBlogPostId, displayOrder) => new RelatedBlogPost
            {
                BlogPostId = blogPostId,
                RelatedBlogPostId = relatedBlogPostId,
                DisplayOrder = displayOrder
            })
            .ToList();

        if (mappings.Count > 0)
            await _relatedBlogPostRepository.InsertAsync(mappings);
    }

    public async Task DeleteRelatedBlogPostsAsync(int blogPostId)
    {
        await _relatedBlogPostRepository.DeleteAsync(mapping =>
            mapping.BlogPostId == blogPostId || mapping.RelatedBlogPostId == blogPostId);
    }
}
