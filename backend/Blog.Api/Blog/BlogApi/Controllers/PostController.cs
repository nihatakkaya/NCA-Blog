using BlogApi.DTOs.Posts;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)  //postservices i controller içine alıyoruz ki blog işlemlerini servise yaptırabilelim
        {
            _postService = postService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDto dto)  //yeni blog yazısı oluşturur
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!         //JWT’nin içinden giriş yapan kullanıcının Id bilgisini alıyor.
            );

            var post = await _postService.CreatePostAsync(dto, userId);

            return Ok(post);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(int page = 1, int pageSize = 10) //ilk sayfa ve 10 post gelir
        {
            if (page < 1)
            {
                return BadRequest("Sayfa numarası 1 veya daha büyük olmalıdır.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Sayfa boyutu 1 ile 100 arasında olmalıdır.");
            }

            var posts = await _postService.GetPostsAsync(page, pageSize);

            return Ok(posts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllPostsForAdmin()
        {
            var posts = await _postService.GetAllPostsForAdminAsync();

            return Ok(posts);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts(string query)
        {
            var posts = await _postService.SearchPostsAsync(query);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)    //Belirli bir blog yazısını ID'sine göre getirir.
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound("Blog yazısı bulunamadı.");
            }

            return Ok(post);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, UpdatePostDto dto)  //Var olan blog yazısını günceller.
        {
            bool updated = await _postService.UpdatePostAsync(id, dto);

            if (!updated)
            {
                return NotFound("Blog yazısı bulunamadı.");
            }

            return Ok("Blog yazısı güncellendi.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)     //Blog yazısını siler.
        {
            bool deleted = await _postService.DeletePostAsync(id);

            if (!deleted)
            {
                return NotFound("Blog yazısı bulunamadı.");
            }

            return Ok("Blog yazısı silindi.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/publish")]
        public async Task<IActionResult> ChangePublishStatus(
            int id,
            bool isPublished)
        {
            bool updated = await _postService.ChangePublishStatusAsync(
                id,
                isPublished
            );

            if (!updated)
            {
                return NotFound("Blog yazısı bulunamadı.");
            }

            return Ok("Yayın durumu güncellendi.");
        }
    }
}