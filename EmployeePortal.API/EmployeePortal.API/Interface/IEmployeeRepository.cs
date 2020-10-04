using EmployeePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.API.Interface
{
    public interface IEmployeeRepository
    {
        void Add(User user);

        void Delete(User user);

        Task<bool> SaveAll();

        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUser(int id);

        Task<Photo> GetPhoto(int id);

        Task<Photo> GetMainPhotoForUser(int userId);

        //Task<Likes> GetLike(int userId, int recipientId);

        //Task<Message> GetMessage(int id);

        //Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);

        //Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}
