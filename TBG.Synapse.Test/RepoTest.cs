using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using TBG.Synapse.Repository;

namespace TBG.Synapse.Test
{
    [TestFixture]
    public static class RepoTest
    {
        readonly static string connectionString;
        static RepoTest()
        {
            connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=Synapse;Integrated Security=True;";
        }

        [Test]
        public static void TestCreate()
        {
            var rep = new SynapseRepository<EmployeeTest>(connectionString);

            rep.CreateTableIdentity();

            var employee = new EmployeeTest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Department = "IT",
                DateOfBirth = DateTime.Now,
                Salary = 10000.00M
            };

            employee.Id = rep.InsertIdentity(employee);

            var insertedEmployee = rep.Get($"ID = {employee.Id}");

            Assert.IsNotNull(insertedEmployee);
        }

        [Test]
        public static void TestInsert()
        {
            var rep = new SynapseRepository<EmployeeTest>(connectionString);

            var employee = new EmployeeTest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Department = "IT",
                DateOfBirth = DateTime.Now,
                Salary = 10000.00M
            };

            employee.Id = rep.InsertIdentity(employee);

            var insertedEmployee = rep.Get($"ID = {employee.Id}").FirstOrDefault();

            Assert.IsNotNull(insertedEmployee);
        }

        [Test]
        public static void TestUpdate()
        {
            var rep = new SynapseRepository<EmployeeTest>(connectionString);

            var employee = new EmployeeTest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Department = "IT",
                DateOfBirth = DateTime.Now,
                Salary = 10000.00M
            };

            employee.Id = rep.InsertIdentity(employee);

            employee.Name = "John Smith";
            employee.Salary = 20000.00M;
            rep.Update(employee);

            var updatedEmployee = rep.Get($"ID = {employee.Id}").FirstOrDefault();

            Assert.AreEqual(employee.Name, updatedEmployee.Name);
            Assert.AreEqual(employee.Salary, updatedEmployee.Salary);
        }

        [Test]
        public static void TestDelete()
        {
            var rep = new SynapseRepository<EmployeeTest>(connectionString);

            var employee = new EmployeeTest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Department = "IT",
                DateOfBirth = DateTime.Now,
                Salary = 10000.00M
            };

            employee.Id = rep.InsertIdentity(employee);

            rep.Delete(employee);

            var deletedEmployee = rep.Get($"ID = {employee.Id}").FirstOrDefault();

            Assert.IsNull(deletedEmployee);
        }
    }
}
