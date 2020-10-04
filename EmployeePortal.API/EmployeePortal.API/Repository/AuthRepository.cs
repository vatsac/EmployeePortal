using EmployeePortal.API.Interface;
using EmployeePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EmployeePortal.API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public async Task<User> Login(string username, string password)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var user = await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x => x.Username == username);

                if (user == null)
                    return null;

                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;

                return user;

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public async Task<bool> UserExists(string username)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                if (await _context.Users.AnyAsync(x => x.Username == username))
                    return true;

                return false;
            }
        }
    }
}