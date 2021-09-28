using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmployeeData
{
    public class Employees
    {
        public List<Employee> employees = new List<Employee>();
        public Employees(string filename)
        {
            employees.AddRange((from row in File.ReadLines(filename).Where(t => !string.IsNullOrWhiteSpace(t)).AsEnumerable()
                                let columns = row.Split(',')
                                select new Employee
                                {
                                    Id = columns[0],
                                    ManagerId = columns[1],
                                    Salary = ValidateSalary(columns[2]) ? Convert.ToInt32(columns[2]) : 0
                                }).OrderBy(x => x.ManagerId).ToList());

            if (ValidateManager()) throw new Exception("Employee with more than  one manager found");
            if (!ValidateOneCEO()) throw new Exception("More than CEO found");
            if (ValidateManagerNotEmployee()) throw new Exception("Some Managers are not employees");
        }

        public bool ValidateSalary(string salary)
        {
            int result;
            return int.TryParse(salary, out result);
        }

        public bool ValidateManager()
        {
            var result = employees.GroupBy(
                  (item => new { item.Id, item.ManagerId }),
                  (key, elements) => new {
                      Id = key,
                      count = elements.Distinct().Count()
                  }).Where(x => x.count > 1).Count();

            return result > 0;
        }

        public bool ValidateOneCEO()
        {
            return employees.Where(c => c.ManagerId == null || c.ManagerId == string.Empty).Count() == 1;
        }


        //NB: To be implemented later
        public bool ValidateEmployeeCircularReference()
        {
            return false;
        }

        public bool ValidateManagerNotEmployee()
        {
            var employeeIds = new HashSet<string>(employees.Select(x => x.Id));

            return employees.Where(e => !employeeIds.Contains(e.ManagerId) && e.ManagerId != string.Empty).Count() > 0;
        }
    }
}
