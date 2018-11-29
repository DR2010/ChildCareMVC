using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary
{
    public interface IEmployee
    {
        event EventHandler<EventArgs> EmployeeList;
        event EventHandler<EventArgs> EmployeeAdd;
        event EventHandler<EventArgs> EmployeeUpdate;
        event EventHandler<EventArgs> EmployeeDelete;

        void DisplayMessage(string message);
        void ResetScreen();

        List<FCMBusinessLibrary.Employee> employeeList { get; set; }
        ResponseStatus response { get; set; }
        Employee employee { get; set; }

    }
}
