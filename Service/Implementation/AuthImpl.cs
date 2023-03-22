using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace User_Email_Verification.Service.Implementation
{
    public class AuthImpl : AuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthImpl(DataContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
            
        }
        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();

            try{
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

                if(user is null) {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Success = false;
                    response.Message = "Invalid Credentials";
                    return response;
                }else if(!ComparePassword(password, user.PasswordHash, user.PasswordSalt)){
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Success = false;
                    response.Message = "Invalid Credentials";
                    return response;
                }else{
                    response.Data = CreateToken(user);
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "User Login";
                    return response;
                }
            }catch(Exception ex) {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            
        }

        public async Task<ServiceResponse<string>> Register(User user, string password)
        {
            var response = new ServiceResponse<string>();
           try{
                 // Cheack if user exist
            if(await UserExist(user.Email)) {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Success = false;
                response.Message = "User already exist";
                return response;
            }

            // Hash password
            createPasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // add user table
            _context.Users.Add(user);
            // save database
            await _context.SaveChangesAsync();

            // User Responsne
            response.Data = CreateToken(user);
            response.StatusCode = HttpStatusCode.Created;
            response.Message = "User Created";
            return response;
           }catch(Exception ex) {
            response.Success = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = ex.Message;
            return response;
           }
        }

        public async Task<bool> UserExist(string email)
        {
            if(await _context.Users.AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()))){
                return true;
            }

            return false;
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool ComparePassword(string password, byte[] passwordHash, byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user) {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var AppSetting = _config.GetSection("AppSettings:Token").Value;

            if(AppSetting is null) {
                throw new Exception("AppSetting Token not found");
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(AppSetting));
            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}