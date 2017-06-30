using System.Collections.Generic;
using ElasticsearchDemo.Web.Models;

namespace ElasticsearchDemo.Web.Controllers
{
    internal class EmployeeResponseModel
    {
        public IReadOnlyCollection<Employee> Employees { get; set; }

        public double? AverageAge { get; set; }
    }
}