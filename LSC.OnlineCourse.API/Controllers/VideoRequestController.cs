using LSC.OnlineCourse.API.Common;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace LSC.OnlineCourse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VideoRequestController : ControllerBase
    {
        private readonly IVideoRequestService _videoRequestService;
        private readonly IUserClaims userClaims;

        public VideoRequestController(IVideoRequestService videoRequestService, IUserClaims userClaims)
        {
            _videoRequestService = videoRequestService;
            this.userClaims = userClaims;
        }

        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<IEnumerable<VideoRequestModel>>> GetAll()
        {
            List<VideoRequestModel> videoRequests;
            var userRoles = userClaims.GetUserRoles();
            if (userRoles.Contains("Admin"))
            {
                videoRequests =  await _videoRequestService.GetAllAsync();
            }
            else
            {
                var videoRequest = await _videoRequestService.GetByUserIdAsync(userClaims.GetUserId());
                videoRequests = videoRequest.ToList();
            }
          
            return Ok(videoRequests);
        }

        [HttpGet("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<VideoRequestModel>> GetById(int id)
        {
            var videoRequest = await _videoRequestService.GetByIdAsync(id);
            if (videoRequest == null)
            {
                return NotFound();
            }
            return Ok(videoRequest);
        }

        [HttpGet("user/{userId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<IEnumerable<VideoRequestModel>>> GetByUserId(int userId)
        {
            var videoRequests = await _videoRequestService.GetByUserIdAsync(userId);
            return Ok(videoRequests);
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<ActionResult<VideoRequestModel>> Create(VideoRequestModel model)
        {
            var createdVideoRequest = await _videoRequestService.CreateAsync(model);
            //await _videoRequestService.SendVideoRequestAckEmail(model); // we are using SQLTrigger to send email automatically
            return CreatedAtAction(nameof(GetById), new { id = createdVideoRequest.VideoRequestId }, createdVideoRequest);
        }

        [HttpPut("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update(int id, VideoRequestModel model)
        {
            try
            {
                //if a user who knows this end point and not having role as admin can directly hit. this is a 
                //security issue. we will see how we can fix this in security video in this series.
                // we have now restricted from UI app but backend is not protected.
                var updatedVideoRequest = await _videoRequestService.UpdateAsync(id, model);
                //await _videoRequestService.SendVideoRequestAckEmail(model); // we are using SQLTrigger to send email automatically
                return Ok(updatedVideoRequest);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            await _videoRequestService.DeleteAsync(id);
            return NoContent();
        }
    }

}
