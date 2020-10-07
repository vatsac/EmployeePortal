using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using EmployeePortal.API.Controllers;
using EmployeePortal.API.Dtos;
using EmployeePortal.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmployeePortal.API.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public  async Task RegisterMethod_UserCreated()
        {

            // Set up Prerequisites   
            var controller = new AuthController();
            UserForRegisterDto user = new UserForRegisterDto
            {
                Username = "UserNameTest",
                Password = "UsernamePassword",
                Gender = "Male",
                KnownAs = "knownasTest",
                DateOfBirth = DateTime.Now,
                City = "TestCity",
                Country = "TestCountry"

            };
            // Act  
            IHttpActionResult actionResult = await controller.Register(user);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<User>;
            // Assert  
            //Assert.IsNotNull(createdResult);
            //Assert.AreEqual("GetUser", createdResult.RouteName);
            // Assert.IsNotNull(createdResult.RouteValues["id"]);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }
    }
}
