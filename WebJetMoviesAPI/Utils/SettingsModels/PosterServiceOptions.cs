// ReSharper disable InconsistentNaming
namespace WebJetMoviesAPI.Utils.SettingsModels
{
    public class PosterServiceOptions
    {
        public string BaseImageUrl { get; set; }
        public string BaseApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string language { get; set; }
        public string page { get; set; }
        public string include_adult { get; set; }
    }
}