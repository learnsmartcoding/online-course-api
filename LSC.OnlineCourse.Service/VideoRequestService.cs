using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace LSC.OnlineCourse.Service
{
    public class VideoRequestService : IVideoRequestService
    {
        private readonly IVideoRequestRepository _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<VideoRequestService> logger;

        public VideoRequestService(IVideoRequestRepository repository, IMapper mapper,
            HttpClient httpClient, IConfiguration configuration, ILogger<VideoRequestService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<List<VideoRequestModel>> GetAllAsync()
        {
            var videoRequests = await _repository.GetAllAsync();
            return _mapper.Map<List<VideoRequestModel>>(videoRequests);
        }

        public async Task<VideoRequestModel?> GetByIdAsync(int id)
        {
            var videoRequest = await _repository.GetByIdAsync(id);
            return videoRequest == null ? null : _mapper.Map<VideoRequestModel>(videoRequest);
        }

        public async Task<IEnumerable<VideoRequestModel>> GetByUserIdAsync(int userId)
        {
            var videoRequests = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<VideoRequestModel>>(videoRequests);
        }

        public async Task<VideoRequestModel> CreateAsync(VideoRequestModel model)
        {
            var videoRequest = _mapper.Map<VideoRequest>(model);
            var createdVideoRequest = await _repository.AddAsync(videoRequest);
            return _mapper.Map<VideoRequestModel>(createdVideoRequest);
        }

        public async Task<VideoRequestModel> UpdateAsync(int id, VideoRequestModel model)
        {
            var existingVideoRequest = await _repository.GetByIdAsync(id);
            if (existingVideoRequest == null)
            {
                throw new KeyNotFoundException($"VideoRequest with ID {id} not found.");
            }

            model.UserId = existingVideoRequest.UserId; // set it to user's id itself if not it becomes admin's request
            _mapper.Map(model, existingVideoRequest);
            var updatedVideoRequest = await _repository.UpdateAsync(existingVideoRequest);
            return _mapper.Map<VideoRequestModel>(updatedVideoRequest);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<VideoModelRequest> SendVideoRequestAckEmail(VideoRequestModel model)
        {
            var videoModelRequest  = new VideoModelRequest() { VideoRequestId = model.VideoRequestId };
            var _functionUrl = configuration["AzureFunction:VideoRequestTriggerUrl"];
            logger.LogInformation("_functionUrl:" + _functionUrl);

            var response = await httpClient.PostAsJsonAsync(_functionUrl, videoModelRequest); 

            logger.LogInformation($"response code: {response.StatusCode}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<VideoModelRequest>();
        }
    }
}
