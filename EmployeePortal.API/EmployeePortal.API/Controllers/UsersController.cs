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
using System.Web.Http.Cors;

namespace EmployeePortal.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
   // [LogUser]
    public class UsersController : ApiController
    {
        private readonly IEmployeeRepository _repo = new EmployeeRepository();

        private EmployeePortalEntities db = new EmployeePortalEntities();
        

        [HttpGet]
        //[Route("api/user")]
        public async Task<IHttpActionResult> GetUsers([FromUri]UserParams userParams)
        {
            var currentUserId = int.Parse(ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userFromRepo.LastActive = DateTime.Now;
            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database
            await db.SaveChangesAsync();

            userParams.UserId = currentUserId;
           // changes is commiteed in data base


            var users = await _repo.GetUsers(userParams);

            var usersToReturn = Mapper.Map<IEnumerable<UserForListDto>>(users);

            //HttpResponse Response=null;
            HttpContext.Current.Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet]
        [Route("api/users/{id}",Name ="GetUser")]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var currentUserId = int.Parse(ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            userFromRepo.LastActive = DateTime.Now;
            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database
            await db.SaveChangesAsync();

            var user = await _repo.GetUser(id);

            var userToReturn = Mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateUser(int id,[FromBody] UserForUpdateDto userForUpdateDto)
        {
            var currentUserId = int.Parse(ClaimsPrincipal.Current.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var userFromRepoClaim = await _repo.GetUser(currentUserId);
            userFromRepoClaim.LastActive = DateTime.Now;
            db.Entry(userFromRepoClaim).State = EntityState.Modified;           // data is updated in database
            await db.SaveChangesAsync();

            var userFromRepo = await _repo.GetUser(id);

            Mapper.Map(userForUpdateDto, userFromRepo);

            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database

          
                await db.SaveChangesAsync();                 // changes is commiteed in data base
           

          
                return StatusCode(HttpStatusCode.NoContent);

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost]
        [Route("api/users/{id}/like/{recipientId}")]
        public async Task<IHttpActionResult> LikeUser(int id, int recipientId)
        {
            //if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);

            if (like != null)
                return BadRequest("You already like this user");

            if (await _repo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            db.Likes.Add(like);

            db.SaveChanges();
            return Ok();

           // return BadRequest("Failed to like user");
        }

    }
}
