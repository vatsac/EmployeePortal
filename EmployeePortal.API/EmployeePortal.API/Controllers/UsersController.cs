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

            userParams.UserId = currentUserId;
            
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
            var user = await _repo.GetUser(id);

            var userToReturn = Mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateUser(int id,[FromBody] UserForUpdateDto userForUpdateDto)
        {
            //if (id != int.Parse(User.Identity.Name))
              //  return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            Mapper.Map(userForUpdateDto, userFromRepo);

            db.Entry(userFromRepo).State = EntityState.Modified;           // data is updated in database

          
                await db.SaveChangesAsync();                 // changes is commiteed in data base
           

          
                return StatusCode(HttpStatusCode.NoContent);

            throw new Exception($"Updating user {id} failed on save");
        }

    }
}
