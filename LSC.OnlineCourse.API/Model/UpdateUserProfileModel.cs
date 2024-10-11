namespace LSC.OnlineCourse.API.Model
{
    public class UpdateUserProfileModel
    {
        public required int UserId { get; set; }
        public string? Bio { get; set; }
        public IFormFile? Picture { get; set; }
    }

}
