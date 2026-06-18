using Nop.Core;
using Nop.Data;
using Nop.Plugin.Widgets.TTBOLTCarousel.Domain;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.TTBOLTCarousel.Areas.Admin.Services;

public class SliderItemService : ISliderItemService
{
    private readonly IRepository<SliderItem> _repository;
    private readonly IWorkContext _workContext;

    public SliderItemService(IRepository<SliderItem> repository,
        IPictureService pictureService,
         IWorkContext workContext)
    {

        _repository = repository;
        _workContext = workContext;
    }

    public virtual async Task<SliderItem> GetSlideByIdAsync(int sliderItemId)
    {
        return await _repository.GetByIdAsync(sliderItemId);
    }

    public virtual async Task<IPagedList<SliderItem>> GetAllSlides(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var rez = _repository.GetAll(query => query.OrderBy(slide => slide.Order)).ToList();

        return new PagedList<SliderItem>(rez, pageIndex, pageSize);
    }

    public virtual async Task InsertSlideAsync(SliderItem sliderItem)
    {
        await _repository.InsertAsync(sliderItem, false);
    }
    public virtual async Task UpdateSlideAsync(SliderItem sliderItem)
    {
        await _repository.UpdateAsync(sliderItem, false);
    }

    public virtual async Task DeleteSlideAsync(SliderItem sliderItem)
    {
        await _repository.DeleteAsync(sliderItem, false);
    }
    public virtual void Log(SliderItem sliderItem)
    {
        if (sliderItem == null)
            throw new ArgumentNullException(nameof(sliderItem));
        _repository.Insert(sliderItem);
    }

    public async Task<List<SliderItem>> GetSlideList()
    {
        var langId = _workContext.GetWorkingLanguageAsync().Result.Id;
        return _repository.GetAll().Where(c => c.LanguageId == langId).OrderBy(c => c.Order).ToList();
    }


}