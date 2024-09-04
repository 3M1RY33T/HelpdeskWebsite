using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExercisesDAL;
using HelpdeskDAL;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HelpdeskViewModels;
using System.Threading.Tasks.Dataflow;
using System.ComponentModel.Design;

namespace CasestudyTests
{
    public class DAOTests
    {
        private readonly ITestOutputHelper output;
        public DAOTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByEmail("sj@abc.com");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetById(8);
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            EmployeeDAO dao = new();
            List<Employee> allEmployees = await dao.GetAll();
            Assert.True(allEmployees.Count > 0);
        }

        [Fact]
        public async Task Employee_Add()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Yigit",
                LastName = "Yildiz",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "y_y@someschool.com"
            };
            Assert.True(await dao.Add(newEmployee) > 0);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForDelete = await dao.GetByLastname("Yildiz");
            if (employeeForDelete != null)
            {
                Assert.True(await dao.Delete(employeeForDelete.Id) == 1);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForUpdate = await dao.GetByLastname("Joe");
            if (employeeForUpdate != null)
            {
                string oldPhoneNo = employeeForUpdate.PhoneNo!;
                //string newPhoneNo = oldPhoneNo == "(555)555-5555" ? "(555)555-1234" : "(444)444-4446";
                string newPhoneNo = oldPhoneNo.Substring(0, oldPhoneNo.Length - 1) + ((Convert.ToInt32(oldPhoneNo.Substring(oldPhoneNo.Length - 1, 1)) + 1) % 10).ToString();
                employeeForUpdate!.PhoneNo = newPhoneNo;
                Assert.True(await dao.Update(employeeForUpdate!) == UpdateStatus.Ok);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByPhoneNumber("(555)555-5555");
            if (selectedEmployee != null)
            {
                int PhoneNoInt = int.Parse(selectedEmployee.PhoneNo);
                Assert.NotNull(selectedEmployee);
            }
        }

        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            EmployeeDAO dao1 = new();
            EmployeeDAO dao2 = new();
            Employee employeeForUpdate1 = await dao1.GetByLastname("Dish");
            Employee employeeForUpdate2 = await dao2.GetByLastname("Dish");
            if (employeeForUpdate1 != null)
            {
                string? oldPhoneNo = employeeForUpdate1.PhoneNo;
                string? newPhoneNo = oldPhoneNo == "(555) 555-5553" ? "(555)-555-5555" : "(555) 555-5553";
                employeeForUpdate1.PhoneNo = newPhoneNo;
                if (await dao1.Update(employeeForUpdate1) == UpdateStatus.Ok)
                {
                    // need to change the phone # to something else
                    employeeForUpdate2.PhoneNo = "(666)-666-6663";
                    Assert.True(await dao2.Update(employeeForUpdate2) == UpdateStatus.Stale);
                }
                else
                    Assert.True(false); // first update failed
            }
            else
                Assert.True(false); // didn't find student 1
        }

        [Fact]
        public async Task Student_LoadPicsTest()
        {
            {
                PicsUtility util = new();
                Assert.True(await util.AddEmployeePicsToDb());
            }
        }

        [Fact]
        public async Task Employee_ComprehensiveTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Joe",
                LastName = "Smith",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com"
            };
            int newEmployeeId = await dao.Add(newEmployee);
            output.WriteLine("New Employee Generated - Id = " + newEmployeeId);
            newEmployee = await dao.GetById(newEmployeeId);
            byte[] oldtimer = newEmployee.Timer!;
            output.WriteLine("New Employee " + newEmployee.Id + " Retrieved");
            newEmployee.PhoneNo = "(555)555-1233";
            if (await dao.Update(newEmployee) == UpdateStatus.Ok)
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was updated to - " + newEmployee.PhoneNo);
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was not updated!");
            }
            newEmployee.Timer = oldtimer; // to simulate another user
            newEmployee.PhoneNo = "doesn't matter data is stale now";
            if (await dao.Update(newEmployee) == UpdateStatus.Stale)
            {
                output.WriteLine("Employee " + newEmployeeId + " was not updated due to stale data");
            }

            dao = new();
            await dao.GetById(newEmployeeId);
            if (await dao.Delete(newEmployeeId) == 1)
            {
                output.WriteLine("Employee " + newEmployeeId + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newEmployeeId));
        }

        [Fact]
        public async Task Call_ComprehensiveTest()
        {
            EmployeeDAO empDao = new();
            Employee emp = await empDao.GetByEmail("y_y@someschool.com");

            ProblemDAO prbDao = new();
            Problem prb = await prbDao.GetByDescription("Memory Update");
            
            CallDAO dao = new();
            Call newCall = new()
            {
                DateOpened = DateTime.Now,
                DateClosed = null,
                OpenStatus = true,
            };
            dao.Add(newCall);
            
            newCall.EmployeeId = Convert.ToInt16(emp.Id);
            
            emp = await empDao.GetById(7);
            newCall.TechId = Convert.ToInt16(emp.Id);

            newCall.ProblemId = Convert.ToInt16(prb.Id);

            newCall.Notes = "Yigit has bad ram, Burner to fix it";
            
            await dao.Add(newCall);

            output.WriteLine("New Call Generated - Id = " + newCall.Id);
            int id = newCall.Id;

            newCall = await dao.GetById(id);
            newCall.Notes += "\nOrdered New RAM!";
            var updateStatus = await dao.Update(newCall);
            if (updateStatus == UpdateStatus.Stale)
            {
                output.WriteLine("Call #" + Convert.ToInt16(newCall.Id) + " was not updated due to stale data");
            }
            int deleteCall = await dao.Delete(newCall.Id);
            Assert.True(deleteCall == 1);
        }

    }
}