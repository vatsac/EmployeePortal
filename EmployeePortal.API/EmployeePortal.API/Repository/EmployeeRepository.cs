using EmployeePortal.API.Interface;
using EmployeePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using EmployeePortal.API.Helper;

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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                return await _context.Likes.FirstOrDefaultAsync(u =>
            u.LikerId == userId && u.LikeeId == recipientId);
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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

                users = users.Where(u => u.Id != userParams.UserId);

                if (userParams.Likers)
                {
                    var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                    users = users.Where(u => userLikers.Contains(u.Id));
                }

                if (userParams.Likees)
                {
                    var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                    users = users.Where(u => userLikees.Contains(u.Id));
                }

                if (userParams.MinAge != 0 || userParams.MaxAge != 99)
                {
                    var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                    var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                    users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
                }

                if (!string.IsNullOrEmpty(userParams.OrderBy))
                {
                    switch (userParams.OrderBy)
                    {
                        case "created":
                            users = users.OrderByDescending(u => u.Created);
                            break;
                        default:
                            users = users.OrderByDescending(u => u.LastActive);
                            break;
                    }
                }
                return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
            }
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var user = await _context.Users
            .Include(x => x.Likes)
            .Include(x => x.Likes1)
            .FirstOrDefaultAsync(u => u.Id == id);

                if (likers)
                {
                    return user.Likes.Where(u => u.LikeeId == id).Select(i => i.LikerId);
                }
                else
                {
                    return user.Likes1.Where(u => u.LikerId == id).Select(i => i.LikeeId);
                }
            }
        }


        public async Task<bool> SaveAll()
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                return await _context.SaveChangesAsync() > 0;
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            }
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                //.Include(p => p.PostAuthor.Select(pa => pa.Author).Select(a => a.Interests))
                var messages = _context.Messages
            .Include(u => u.User).Include(u => u.User.Photos)
            .Include(u => u.User1).Include(u => u.User.Photos)
            .AsQueryable();

                switch (messageParams.MessageContainer)
                {
                    case "Inbox":
                        messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                        break;
                    case "Outbox":
                        messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                        break;
                    default:
                        messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                        break;
                }

                messages = messages.OrderByDescending(d => d.MessageSent);
                return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
            }
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            using (EmployeePortalEntities _context = new EmployeePortalEntities())
            {
                var messages = await _context.Messages
                     .Include(u => u.User).Include(p => p.User.Photos)
                     .Include(u => u.User1).Include(p => p.User1.Photos)
                     .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId ||
                      m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
                      .OrderByDescending(m => m.MessageSent)
                      .ToListAsync();

                return messages;
            }

        }
    }
}