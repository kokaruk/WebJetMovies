namespace WebJetMoviesAPI.Utils
{
    public class CheapestMovieResponse<T> where T : class
    {
        public string ParentName { get; set; }
        public T Member { get; set; }
    }
}