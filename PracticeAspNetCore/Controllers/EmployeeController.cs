using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using PracticeAspNetCore.Models;

namespace PracticeAspNetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<EmployeeController> logger;
        //private EmployeeContext db;

        public EmployeeController(/*EmployeeContext context, */IConfiguration configuration, ILogger<EmployeeController> logger)
        {
            //db = context;
            this.configuration = configuration;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ulong>> Create([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest();

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            int companyId = (int)(employee.CompanyId != null ? employee.CompanyId : 0);
            Passport passport = employee.Passport != null ? employee.Passport : new Passport();

            MySqlCommand command = new MySqlCommand("INSERT INTO Employee(Name, Surname, Phone, CompanyId, PassportType, PassportNumber) VALUES " +
                $"('{employee.Name}'," +
                $"'{employee.Surname}'," +
                $"'{employee.Phone}'," +
                $"{companyId}," +
                $"'{passport.Type}'," +
                $"'{passport.Number}'); SELECT LAST_INSERT_ID();", connection);

            return (ulong)await command.ExecuteScalarAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Read([FromBody] int? companyId)
        {
            if (companyId == null)
                return BadRequest();

            #region MySqlConnection
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            MySqlCommand command = new MySqlCommand($"Select * from Employee WHERE CompanyId = {companyId}", connection);
            List<Employee> employees = new List<Employee>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee()
                    {
                        Id = Convert.ToInt32(reader["idEmployee"]),
                        Name = reader["Name"].ToString(),
                        Surname = reader["Surname"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        CompanyId = Convert.ToInt32(reader["CompanyId"]),
                        Passport = new Passport()
                        {
                            Type = reader["PassportType"].ToString(),
                            Number = reader["PassportNumber"].ToString()
                        }
                    });
                }

            }
            await connection.CloseAsync();

            return employees;
            #endregion

            #region DbContext
            //List<Employee> employees = await db.Employees.Where(employee => employee.CompanyId == companyId).ToListAsync();
            //return employees;
            #endregion
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Employee updatedEmployee)
        {
            if (updatedEmployee == null)
                return BadRequest();

            #region MySqlConnection
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            MySqlCommand command = new MySqlCommand("UPDATE Employee SET @changeParameters WHERE idEmployee=@id", connection);

            List<string> changeFields = new List<string>();
            if (updatedEmployee.Name != null) changeFields.Add($"Name='{updatedEmployee.Name}'");
            if (updatedEmployee.Surname != null) changeFields.Add($"Surname='{updatedEmployee.Surname}'");
            if (updatedEmployee.Phone != null) changeFields.Add($"Phone='{updatedEmployee.Phone}'");
            if (updatedEmployee.CompanyId != 0) changeFields.Add($"CompanyId='{updatedEmployee.CompanyId}'");
            if (updatedEmployee.Passport != null)
            {
                if (updatedEmployee.Passport.Type != null) changeFields.Add($"PassportType='{updatedEmployee.Passport.Type}'");
                if (updatedEmployee.Passport.Number != null) changeFields.Add($"PassportNumber='{updatedEmployee.Passport.Number}'");
            }

            string changeParameters = changeFields.Join(", ");
            command.CommandText = command.CommandText.Replace("@changeParameters", changeParameters);
            command.CommandText = command.CommandText.Replace("@id", updatedEmployee.Id.ToString());

            await command.ExecuteScalarAsync();

            return Ok();
            #endregion

            #region DbContext
            //Employee oldEmployee = await db.Employees.FirstOrDefaultAsync(employee => employee.Id == updatedEmployee.Id);
            //updatedEmployee.Name = updatedEmployee.Name != null ? updatedEmployee.Name : oldEmployee.Name;
            //updatedEmployee.Surname = updatedEmployee.Surname != null ? updatedEmployee.Surname : oldEmployee.Surname;
            //updatedEmployee.Phone = updatedEmployee.Phone != null ? updatedEmployee.Phone : oldEmployee.Phone;
            //updatedEmployee.CompanyId = updatedEmployee.CompanyId != null ? updatedEmployee.CompanyId : oldEmployee.CompanyId;
            //Passport passport = new Passport() {
            //    Type = updatedEmployee.Passport.Type != null ? updatedEmployee.Passport.Type : oldEmployee.Passport.Type,
            //    Number = updatedEmployee.Passport.Number != null ? updatedEmployee.Passport.Number : oldEmployee.Passport.Number
            //};
            //updatedEmployee.Passport = passport;

            //db.Employees.Update(updatedEmployee);

            //return Ok();
            #endregion
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody] int? id)
        {
            if (id == null)
                return BadRequest();

            #region MySqlConnection
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            MySqlCommand command = new MySqlCommand($"DELETE FROM Employee WHERE idEmployee = {id}", connection);
            await command.ExecuteNonQueryAsync();
            return Ok();
            #endregion

            #region DbContext
            //Employee employee = await db.Employees.FirstOrDefaultAsync(employee => employee.Id == id);
            //if (employee != null) {
            //    db.Employees.Remove(employee);
            //}

            //return Ok();
            #endregion
        }

    }
}
