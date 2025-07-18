using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using WebApi3.Models; // User modeli için
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace WebApi3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var userList = new List<User>();
            string connStr = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "SELECT Id, Name FROM [User]"; // Tablo adı köşeli parantezle
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    userList.Add(new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString()
                    });
                }
            }

            return Ok(userList);
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                   new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
    );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            // Basit kontrol, sadece örnek için (veritabanıyla eşleştirme önerilir)
            if (user.Name == "admin" && user.Password == "1234")
            {
                var token = GenerateJwtToken(user.Name);
                return Ok(new { token });
            }

            return Unauthorized("Geçersiz kullanıcı");
        }

        [HttpPost]
        [Route("AddUser")]
        public IActionResult AddUser([FromBody] User newUser)
        {
            if (newUser == null || string.IsNullOrEmpty(newUser.Name))
                return BadRequest("User data is invalid.");

            string connStr = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "INSERT INTO [User] (Name, Email, Password) VALUES (@Name, @Email, @Password)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", newUser.Name);
                    cmd.Parameters.AddWithValue("@Email", newUser.Email);
                    cmd.Parameters.AddWithValue("@Password", newUser.Password);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return Ok("User added successfully");
                    else
                        return StatusCode(500, "Error inserting user");
                }
            }
        }

        [HttpPut]
        [Route("UserChanged/{id}")]
        public IActionResult UserChanged(int id, [FromBody] User user)
        {

            if (User == null || string.IsNullOrEmpty(user.Name))
                return BadRequest("User data is invalid.");

            string connStr = configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"UPDATE [User] 
                         SET Name = @Name, Email = @Email, Password = @Password
                         WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", user.Password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return Ok("User updated successfully");
                    else
                        return NotFound("User not found or no changes made");
                }
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "DELETE FROM [User] WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return Ok($"User with Id={id} deleted successfully.");
                    else
                        return NotFound($"User with Id={id} not found.");
                }
            }
        }


    }
}
