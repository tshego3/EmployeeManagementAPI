﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagementAPI.Models
{
    public class Employee
    {
        public int employeeID { get; set; }
        public string tbFirstName { get; set; }
        public string tbSurname { get; set; }
        public int tbTellNo { get; set; }
        public string tbEmail { get; set; }
    }
}