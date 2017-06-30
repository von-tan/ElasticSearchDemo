namespace ElasticsearchDemo.Web.Requests
{
    public class EmployeeSearchRequest
    {
        public string Search { get; set; }

        public string[] Positions { get; set; }

        public int Page { get; set; } = 1;
    }
}
