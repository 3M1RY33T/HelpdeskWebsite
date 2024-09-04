using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HelpdeskDAL;
using Xunit;
using Xunit.Abstractions;

namespace HelpdeskViewModels
{
    public class ViewModelTests
    {
        private readonly ITestOutputHelper output;
        public ViewModelTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Employee_GetByLastnameTest()
        {
            EmployeeViewModel vm = new() { Lastname = "Pincher" };
            await vm.GetByLastname();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            EmployeeViewModel vm = new() { Id = 1 };
            await vm.GetById();
            Assert.NotNull(vm.Id);
        }

        [Fact]
        public async Task Employee_GetByPhonenoTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(444)444-4444" };
            await vm.GetByPhoneno();
            Assert.NotNull(vm.Phoneno);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            List<EmployeeViewModel> allStudentVms = null;
            EmployeeViewModel vm = new();
            allStudentVms = await vm.GetAll();
            Assert.True(allStudentVms.Count > 0);
        }


        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeViewModel vm;
            vm = new()
            {
                Title = "Mr.",
                Firstname = "Yigit",
                Lastname = "Yildiz",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure division id is in Division table
            };
            await vm.Add();
            Assert.True(vm.Id > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(555)555-1234" };
            await vm.GetByPhoneno(); // Student just added in Add test
            vm.Email = vm.Email == "some@abc.com" ? "any@abc.com" : "some@abc.com";
            // will be -1 if failed 0 if no data changed, 1 if succcessful
            Assert.True(await vm.Update() == 1);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777)777-7777" };
            await vm.GetByPhoneno();
            Assert.True(await vm.Delete() == 1); // 1 student deleted

        }

        [Fact]
        public async Task Employee_ComprehensiveVMTest()
        {
            EmployeeViewModel evm = new()
            {
                Title = "Mr.",
                Firstname = "Yigit",
                Lastname = "Yildiz",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure department id is in Departments table
            };
            await evm.Add();
            output.WriteLine("New Employee Added - Id = " + evm.Id);
            int? id = evm.Id; // need id for delete later
            await evm.GetById();
            output.WriteLine("New Employee " + id + " Retrieved");
            evm.Phoneno = "(555)555-1233";
            if (await evm.Update() == 1)
            {
                output.WriteLine("Employee " + id + " phone# was updated to - " +
               evm.Phoneno);
            }
            else
            {
                output.WriteLine("Employee " + id + " phone# was not updated!");
            }
            evm.Phoneno = "Another change that should not work";
            if (await evm.Update() == -2)
            {
                output.WriteLine("Employee " + id + " was not updated due to stale data");
            }
            evm = new EmployeeViewModel
            {
                Id = id
            };
            // need to reset because of concurrency error
            await evm.GetById();
            if (await evm.Delete() == 1)
            {
                output.WriteLine("Employee " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
           => await evm.GetById());
        }

        [Fact]
        public async Task Call_ComprehensiveVMTest()
        {
            CallViewModel cvm = new();
            EmployeeViewModel evm = new();
            ProblemViewModel pvm = new()
            {
                Description = "Memory Update"
            };
            await pvm.Add();
            cvm.DateOpened = DateTime.Now;
            cvm.DateClosed = null;
            cvm.OpenStatus = true;
            evm.Email = "y_y@someschool.com";
            await evm.GetByEmail();
            cvm.EmployeeId = Convert.ToInt16(evm.Id);
            evm.Email = "bb@abc.com";
            await evm.GetByEmail();
            cvm.TechId = Convert.ToInt16(evm.Id);
            pvm.Description = "Memory Update";
            await pvm.GetByDescription();
            cvm.ProblemId = pvm.Id.Value;
            cvm.Notes = "Yigit has bad ram, Burner to fix it";
            await cvm.Add();
            output.WriteLine("New Call Generated - Id = " + cvm.Id);
            int id = cvm.Id;
            cvm = new();
            cvm.Id = id;
            await cvm.GetById();
            cvm.Notes += "\nOrdered New RAM!";
            if (await cvm.Update() == UpdateStatus.Stale)
            {
                output.WriteLine("Call #" + Convert.ToInt16(cvm.Id) + " was not updated due to stale data");
            }
            int deleteCall = await cvm.Delete();
            Assert.True(deleteCall == 1);
        }

    }
}
