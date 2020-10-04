using EmployeePortal.API.Interface;
using EmployeePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace EmployeePortal.API.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public void Add(User user)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                _context.Users.Add(user);
            }
        }

        public void Delete(User user)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                _context.Users.Remove(user);
            }
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                return await _context.Photos.Where(u => u.UserId == userId)
                          .FirstOrDefaultAsync(p => (bool)p.IsMain);
            }
        }

        public async Task<Photo> GetPhoto(int id)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

                return photo;
            }
        }

        public async Task<User> GetUser(int id)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

                return user;
            }
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var users = await _context.Users.Include(p => p.Photos).ToListAsync();

                return users;
            }
        }

        public async Task<bool> SaveAll()
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                return await _context.SaveChangesAsync() > 0;
            }
        }
    }
}