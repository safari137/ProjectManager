using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Models.Xero;

namespace ProjectManager.Services.XeroService.Payroll
{
    public static class EmployeeService
    {
        private static readonly Dictionary<Guid, string> _employeedDictionary = new Dictionary<Guid, string>();
        private static bool _hasBeenInitialized = false;

        public static string GetNameFromId(Guid id)
        {
            if (!_hasBeenInitialized)
                Initialize();

            string result;

            var found = _employeedDictionary.TryGetValue(id, out result);
            if (!found)
            {
                Initialize();
                found = _employeedDictionary.TryGetValue(id, out result);
                if (!found)
                {
                    _employeedDictionary.Add(id, "DoesNotExist");
                    result = "DoesnotExist";
                }
            }
            return result;
        }

        public static List<Employee> GetAllEmployees()
        {
            if (!_hasBeenInitialized)
                Initialize();

            var employeeList = _employeedDictionary.Select(employee => new Employee
            {
                XeroEmployeeId = employee.Key, FullName = employee.Value
            }).ToList();

            return employeeList;
        } 

        private static void Initialize()
        {
            _hasBeenInitialized = true;

            var employees = XeroApiService
                .PayrollApi
                .Employees
                .Find()
                .ToList();

            foreach (var employee in employees.Where(employee => !_employeedDictionary.ContainsKey(employee.Id)))
            {
                _employeedDictionary.Add(employee.Id, employee.FirstName + " " + employee.LastName);
            }
        }
    }
}
