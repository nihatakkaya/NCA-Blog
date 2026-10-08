using BlogApi.DTOs.Categories;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateCategoryAsync(dto);

            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();

            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            UpdateCategoryDto dto)
        {
            bool updated = await _categoryService.UpdateCategoryAsync(id, dto);

            if (!updated)
            {
                return NotFound("Kategori bulunamadı.");
            }

            return Ok("Kategori güncellendi.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id) //mevcut kategoriyi silmek için
        {
            bool deleted = await _categoryService.DeleteCategoryAsync(id);  //asıl silme işlemini service katmanında yapıyoruz. controller katmanı sadece service katmanını çağırıyor.

            if (!deleted)
            {
                return NotFound("Kategori bulunamadı.");
            }

            return Ok("Kategori silindi.");
        }
    }
}