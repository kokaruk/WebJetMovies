using System;
using System.Collections.Generic;

namespace WebJetMoviesAPI.Utils
{
    /// <summary>
    ///     Generic response wrapper class to return paginated collection and url to next previous page
    /// </summary>
    public class PageCollectionResponse<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
    }
}