using AuthWithStorage.Application.DTOs;
using AuthWithStorage.Application.Services;
using AuthWithStorage.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthWithStorage.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IValidator<FileRequest> _validator;

        public FileController(IFileService fileService, IValidator<FileRequest> validator)
        {
            _fileService = fileService;
            _validator = validator;
        }

        /// <returns>A response indicating the result of the operation.</returns>
        /// <remarks>
        /// Sample request using Postman:
        /// 
        /// POST /api/files
        /// Content-Type: multipart/form-data
        /// 
        /// Form-data:
        /// - name: example
        /// - description: sample file
        /// - file: [Select a file]
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateFile([FromForm] FileRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            int fileId;
            await using (var fileStream = request.FormFile.OpenReadStream())
            {
                fileId = await _fileService.AddFileAsync(new FileDto{Name = request.Name, Type = request.Type, UploadedByUserId = request.UploadedByUserId, ContentType = request.ContentType}, fileStream);
            }

            return CreatedAtAction(nameof(GetFileById), new { id = fileId }, request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            var file = await _fileService.GetFileByIdAsync(id);
            if (file == null)
                return NotFound();

            return Ok(file);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, [FromForm] FileRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            
            }

            using (var fileStream = request.FormFile.OpenReadStream())
            {
                await _fileService.UpdateFileAsync(new FileDto{Type = request.Type}, fileStream);
            }

            return NoContent();
        }
       

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            await _fileService.DeleteFileAsync(id);
            return NoContent();
        }
    }
}