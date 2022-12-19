using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

using NUnit.Framework;
using TBG.Synapse.Repository;

namespace TBG.Synapse.Test
{
    [TestFixture]
    public static class RepoTest
    {
        readonly private static string _connectionString;
        static RepoTest()
        {
            _connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=Synapse;Integrated Security=True;";
        }

        [Test]
        public static void TestCreate()
        {
            var rep = new SynapseRepository<EmployeeTest>(_connectionString);

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
            var rep = new SynapseRepository<EmployeeTest>(_connectionString);

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
            var rep = new SynapseRepository<EmployeeTest>(_connectionString);

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
            var rep = new SynapseRepository<EmployeeTest>(_connectionString);

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

        [Test]
        public static void ExecuteSqlQuery()
        {
            string createTableSql = @"
                CREATE TABLE TempCustomers (
                    CustomerId INT PRIMARY KEY,
                    CustomerName NVARCHAR(50)
                )
            ";
            string dropTableSql = "DROP TABLE TempCustomers";

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                SynapseRepository.ExecuteSqlQuery(createTableSql, connection);
                SynapseRepository.ExecuteSqlQuery(dropTableSql, connection);
            }
        }

    }
}
