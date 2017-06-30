using ElasticsearchDemo.Web.Models;
using System.Collections.Generic;

namespace ElasticsearchDemo.Web.Responses
{
    public class EmployeeSearchResponse
    {
        public IEnumerable<Employee> Employees { get; set; }

        public double? AverageAge { get; set; }
    }
}
