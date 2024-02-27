using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Employee
    {
        public Employee()
        {
            Accounts = new HashSet<Account>();
            InverseEmployees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public int? SupervisorId { get; set; }
        public int? EmployeesId { get; set; }

        public virtual Employee Employees { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Employee> InverseEmployees { get; set; }
    }
}
