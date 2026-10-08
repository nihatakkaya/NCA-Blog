using BlogApi.DTOs.Tags;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //route hangi controllerdan erişileceğini belirlemek için
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTag(CreateTagDto dto)
        {
            var tag = await _tagService.CreateTagAsync(dto);

            return Ok(tag);
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagService.GetTagsAsync();

            return Ok(tags);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, UpdateTagDto dto)
        {
            bool updated = await _tagService.UpdateTagAsync(id, dto);

            if (!updated)
                return NotFound("Tag bulunamadı.");

            return Ok("Tag güncellendi.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            bool deleted = await _tagService.DeleteTagAsync(id);

            if (!deleted)
                return NotFound("Tag bulunamadı.");

            return Ok("Tag silindi.");
        }

    }
}