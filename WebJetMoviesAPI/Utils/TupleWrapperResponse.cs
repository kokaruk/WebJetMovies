namespace WebJetMoviesAPI.Utils
{
    public class TupleWrapperResponse<T> where T : class
    {
        /// <summary>
        /// Generic tuple response wrapper. 
        /// </summary>
        // represent the key value
        public string ParentName { get; set; }
        public T Member { get; set; }
    }
}