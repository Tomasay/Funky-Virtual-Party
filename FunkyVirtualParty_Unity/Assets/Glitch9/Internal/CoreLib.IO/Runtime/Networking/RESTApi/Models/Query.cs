namespace Glitch9.IO.Networking.RESTApi
{
    public class TokenQuery : Query
    {
        /// <summary> 
        /// Optional. A page token, received from a previous corpora.list call.
        /// Provide the nextPageToken returned in the response as an argument to the next request to retrieve the next page.
        /// When paginating, all other parameters provided to corpora.list must match the call that provided the page token.
        /// </summary>
        public string PageToken { get; set; }

        public TokenQuery(int? size = null, string pageToken = null)
        {
            Size = size;
            PageToken = pageToken;
        }
    }

    public class CursorQuery : Query
    {
        /// <summary>
        /// [Optional]
        /// Sort order by the created_at timestamp of the objects. 
        /// asc for ascending order and desc for descending order.
        /// </summary>
        public QueryOrder? Order { get; set; }

        /// <summary>
        /// Optional. A cursor for use in pagination. 
        /// after is an object ID that defines your place in the list. 
        /// For instance, if you make a list request and receive 100 objects, 
        /// ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list.
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// Optional. A cursor for use in pagination. 
        /// Before is an object ID that defines your place in the list. 
        /// For instance, if you make a list request and receive 100 objects, 
        /// ending with obj_foo, your subsequent call can include Before=obj_foo in order to fetch the previous page of the list.
        /// </summary>
        public string Before { get; set; }

        public CursorQuery(int? size = null, QueryOrder? order = null, string after = null, string before = null)
        {
            Size = size;
            Order = order;
            After = after;
            Before = before;
        }
    }

    public abstract class Query
    {
        /// <summary>
        /// Optional. A limit on the number of objects to be returned. 
        /// Limit can range between 1 and 100, and the default is 20.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// If true returns all datas including those that have been archived. 
        /// Archived datas are not included by default.
        /// </summary>
        public bool? IncludeArchived { get; set; }
    }

    /// <summary>
    /// A cursor for use in pagination.
    /// </summary>
    public class QueryCursor
    {
        /// <summary>
        /// An object ID that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects,
        /// ending with obj_foo, your subsequent call can include after=obj_foo
        /// in order to fetch the next page of the list.
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// An object ID that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects,
        /// ending with obj_foo, your subsequent call can include before=obj_foo
        /// in order to fetch the previous page of the list.
        /// </summary>
        public string Before { get; set; }

        public QueryCursor(string after, string before)
        {
            After = after;
            Before = before;
        }
    }

    public enum QueryOrder
    {
        Unset,
        [ApiEnum("desc")] Descending,
        [ApiEnum("asc")] Ascending,
        [ApiEnum("created_at")] CreatedAt,
    }
}
