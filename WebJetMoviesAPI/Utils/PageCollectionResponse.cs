using System;
using System.Collections.Generic;

namespace WebJetMoviesAPI.Utils
{
    public class PageCollectionResponse<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
    }
}