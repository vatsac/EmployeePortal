using AutoMapper;
using EmployeePortal.API.Dtos;
using EmployeePortal.API.Helper;
using EmployeePortal.API.Interface;
using EmployeePortal.API.Models;
using EmployeePortal.API.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EmployeePortal.API.Controllers
{
    [Authorize]
    public class MessagesController : ApiController
    {
        private readonly IEmployeeRepository _repo = new EmployeeRepository();

        private EmployeePortalEntities db = new EmployeePortalEntities();

        [HttpGet]
        [Route("api/users/{userId}/messages/id",Name ="GetMessage")]
        public async Task<IHttpActionResult> GetMessage(int userId, int id)
        {
            var currentUserId = int.Parse(ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userFromRepo.LastActive = DateTime.Now;
            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database
            await db.SaveChangesAsync();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        [Route("api/users/{userId}/messages")]
        public async Task<IHttpActionResult> GetMessagesForUser(int userId, [FromUri]MessageParams messageParams)
        {
            var currentUserId = int.Parse(ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userFromRepo.LastActive = DateTime.Now;
            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database
            await db.SaveChangesAsync();


            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = Mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            foreach (var item in messages)
            {
                var sender = await _repo.GetUser(item.SenderId);
                item.SenderKnownAs = sender.KnownAs;
                item.SenderPhotoUrl = sender.Photos.FirstOrDefault(p => (bool)p.IsMain).Url;
                var recepient = await _repo.GetUser(item.RecipientId);
                item.RecipientKnownAs = recepient.KnownAs;
                item.RecipientPhotoUrl = recepient.Photos.FirstOrDefault(p => (bool)p.IsMain).Url;
            }

            HttpContext.Current.Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            

            return Ok(messages);
        }

        [HttpGet]
        [Route("api/users/{userId}/messages/thread/{recipientId}")]
        public async Task<IHttpActionResult> GetMessageThread(int userId, int recipientId)
        {
            //if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();

            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);

            var messageThread = Mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            foreach (var item in messageThread)
            {
                var sender = await _repo.GetUser(item.SenderId);
                item.SenderKnownAs = sender.KnownAs;
                item.SenderPhotoUrl = sender.Photos.FirstOrDefault(p => (bool)p.IsMain).Url;
                var recepient = await _repo.GetUser(item.RecipientId);
                item.RecipientKnownAs = recepient.KnownAs;
                item.RecipientPhotoUrl = recepient.Photos.FirstOrDefault(p => (bool)p.IsMain).Url;
            }


            return Ok(messageThread);
        }




        [HttpPost]
        [Route("api/users/{userId}/messages")]
        public async Task<IHttpActionResult> CreateMessage(int userId,[FromBody] MessageForCreationDto messageForCreationDto)
        {
            var currentUserId = int.Parse(ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userFromRepo.LastActive = DateTime.Now;
            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database
            await db.SaveChangesAsync();

            messageForCreationDto.SenderId = userId;
            var sender = await _repo.GetUser(userId);

            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find user");

            var message = Mapper.Map<Message>(messageForCreationDto);

            db.Messages.Add(message);

            db.SaveChanges();
            
               var messageToReturn = Mapper.Map<MessageForCreationDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            

        }



    }
}
