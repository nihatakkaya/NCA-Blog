using BlogApi.DTOs.Comments;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.Controllers
{
    [ApiController] //bunun sayesinde gelen json verilerini dto ya otomatik bağlama ve validation yapma işlemleri gerçekleşiyor.
    [Route("api/posts/{postId}/comments")]  //controllerın temel adresini belirler
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment(
            int postId,
            CreateCommentDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var comment = await _commentService.CreateCommentAsync(
                dto,
                userId,
                postId
            );

            return Ok(comment);
        }
        [HttpGet]   //burada authorizea gerek yok yorumları herkes görebilir. sadece yorum eklemek için authorize gerekiyor.
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);

            return Ok(comments);
        }
        [Authorize]
        [HttpPut("{commentId}")]//adres oluşturuyorum. api/posts/1/comments/1 gibi. 1.postId 1.commentId
        public async Task<IActionResult> UpdateComment(
            int postId,
            int commentId,
            UpdateCommentDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            bool updated = await _commentService.UpdateCommentAsync(
                commentId,
                userId,
                dto
            );

            if (!updated)
            {
                return NotFound("Yorum bulunamadı veya bu yorumu düzenleme yetkiniz yok.");
            }

            return Ok("Yorum güncellendi.");
        }
        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(
            int postId,
            int commentId)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            bool deleted = await _commentService.DeleteCommentAsync(
                commentId,
                userId
            );

            if (!deleted)
            {
                return NotFound("Yorum bulunamadı veya bu yorumu silme yetkiniz yok.");
            }

            return Ok("Yorum silindi.");
        }
    }

}