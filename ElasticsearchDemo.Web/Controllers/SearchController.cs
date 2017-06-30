namespace ElasticsearchDemo.Web.Controllers
{
    using ElasticsearchDemo.Web.Models;
    using ElasticsearchDemo.Web.Requests;
    using Microsoft.AspNetCore.Mvc;
    using Nest;
    using System.Threading.Tasks;
    using System.Linq;

    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly IElasticClient client;

        private const int PAGE_SIZE = 100;

        public SearchController(IElasticClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Returns all employees
        /// </summary>
        /// <returns>All employee documents</returns>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> FetchAll()
        {
            // Using FluentAPI
            // var searchResponse = await client.SearchAsync<Employee>(q => q
            //   .Index("employee")
            //   .Type("employee_information")
            //   .MatchAll());

            // Using Object Initializer Syntax
            var searchRequest = new SearchRequest("employees", "employee_information")
            {
                Query = new MatchAllQuery(),
                Size = PAGE_SIZE
            };

            var results = await client.SearchAsync<Employee>(searchRequest);

            return Ok(results.Documents);
        }

        /// <summary>
        /// Searches employees by fullname with paging
        /// </summary>
        /// <param name="query">Text to search</param>
        /// <returns>Employees that match the criteria</returns>
        [HttpGet]
        public async Task<IActionResult> SearchEmployees(int page, string query)
        {
            // Using FluentAPI
            // var searchResponse = await client.SearchAsync<Employee>(q => q
            //    .Index("employee")
            //    .Type("employee_information")
            //    .Query(ma => ma
            //        .Match(e => e.Field("full_name").Query(query))));

            // Using Object Initializer Syntax
            var searchRequest = new SearchRequest("employees", "employee_information")
            {
                Query = new MatchQuery()
                {
                    Field = "full_name",
                    Query = query,
                },
                From = (page - 1) * PAGE_SIZE,
                Size = PAGE_SIZE
            };

            var results = await client.SearchAsync<Employee>(searchRequest);

            return Ok(results.Documents);
        }

        /// <summary>
        /// Complex searches with aggregation
        /// </summary>
        /// <param name="request">The search request</param>
        /// <returns>Search results</returns>
        [HttpPost]
        public async Task<IActionResult> ComplexSearch([FromBody] EmployeeSearchRequest request)
        {
            var query = new QueryContainer();

            if (request.Positions != null && request.Positions.Any())
            {
                foreach (var p in request.Positions)
                {
                    query |= +new TermQuery() { Field = "position", Value = p };
                }
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                query &= new MatchQuery() { Field = "full_name", Query = request.Search, Fuzziness = Fuzziness.EditDistance(1) };
            }

            var searchRequest = new SearchRequest<Employee>("employees", "employee_information")
            {
                From = (request.Page - 1) * PAGE_SIZE,
                Size = PAGE_SIZE,
                Query = query,
                Aggregations = new AverageAggregation("ave_age", "age")
            };

            var result = await client.SearchAsync<Employee>(searchRequest);

            var response = new EmployeeResponseModel
            {
                Employees = result.Documents,
                AverageAge = result.Aggs.Average("ave_age").Value
            };

            return Ok(response);
        }
    }
}