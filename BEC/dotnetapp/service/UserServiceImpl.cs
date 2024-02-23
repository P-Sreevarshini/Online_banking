
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Repository;

namespace dotnetapp.Service
{
    public class UserServiceImpl : UserService
    {
        private readonly UserRepo _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager; 


        public UserServiceImpl(UserRepo userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole),
                new Claim(ClaimTypes.Name, user.Username),

            };

            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                expires: DateTime.Now.AddDays(365), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
         public async Task<User> GetUserByIdAsync(long userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }
        public async Task<(int, string)> Login(Login model)
        {
            var user = await userManager.GetUserByEmailAsync(model.Email);
            if (user == null)
                return (0, "Invalid Email");
            if (!await userManager.CheckPasswordAsync(user, model.Password))
                return (0, "Invalid password");

            // var userRoles = await userManager.GetRolesAsync(user);
            // var authClaims = new List<Claim>
            // {
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // };

            // foreach (var userRole in userRoles)
            // {
            //     authClaims.Add(new Claim(ClaimTypes.UserRole, userRole));
            // }
            // string token = GenerateToken(authClaims);
            // return (1, token);
        }
    }
}