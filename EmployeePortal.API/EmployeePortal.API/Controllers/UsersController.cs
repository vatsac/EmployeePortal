using AutoMapper;
using EmployeePortal.API.Dtos;
using EmployeePortal.API.Interface;
using EmployeePortal.API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EmployeePortal.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class UsersController : ApiController
    {
        private readonly IEmployeeRepository _repo = new EmployeeRepository();

        [HttpGet]
        //[Route("api/user")]
        public async Task<IHttpActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var usersToReturn = Mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet]

        public async Task<IHttpActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = Mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }
    }
}
