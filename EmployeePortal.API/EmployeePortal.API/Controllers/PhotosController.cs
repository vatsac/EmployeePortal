using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EmployeePortal.API.Dtos;
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
    public class PhotosController : ApiController
    {
        private readonly IEmployeeRepository _repo = new EmployeeRepository();

        private EmployeePortalEntities db = new EmployeePortalEntities();

        [HttpGet]
        [Route("api/users/{userId}/photos", Name = "GetPhoto")]
        public async Task<IHttpActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = Mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        [Route("api/users/{userId}/photos")]
        public async Task<IHttpActionResult> AddPhotoForUser(int userId
            /*[FromBody]PhotoForCreationDto photoForCreationDto*/)
        {
            Account acc = new Account(
                "vatsac",
                "936419744999822",
                "w39XjwviejNAug2X74Hfg2I7_qg"
            );

            var _cloudinary = new Cloudinary(acc);

            var userFromRepo = await _repo.GetUser(userId);

            var file  = System.Web.HttpContext.Current.Request.Files.Count > 0 ?
        HttpContext.Current.Request.Files[0] : null; 

            var uploadResult = new ImageUploadResult();

            if (file.ContentLength > 0)
            {
                using (var stream = file.InputStream)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation()
                       .Width(500).Height(500).Crop("fill").Gravity("face")

                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                };
            }

            var photoForCreationDto = new PhotoForCreationDto
            {
                File = file,
                Url = uploadResult.Uri.ToString(),
                PublicId = uploadResult.PublicId,
                UserId = userId
            };

            var photo = Mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => (bool)u.IsMain))
                photo.IsMain = true;
            else
                photo.IsMain = false;

            db.Photos.Add(photo);
            await db.SaveChangesAsync();
            var photoToReturn = Mapper.Map<PhotoForReturnDto>(photo);

            return CreatedAtRoute("GetPhoto", new { userId,id = photo.Id }, photoToReturn);

        }

        [HttpPost]
        [Route("api/users/{userId}/photos/{id}/setMain")]
        public async Task<IHttpActionResult> SetMainPhoto(int userId, int id)
        {
            //if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();

            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if ((bool)photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            db.Entry(photoFromRepo).State = EntityState.Modified;           // data is updated in database
            db.Entry(currentMainPhoto).State = EntityState.Modified;           // data is updated in database


            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);

            //return BadRequest("Could not set photo to main");
        }

        [HttpDelete]
        [Route("api/users/{userId}/photos/{id}")]
        public async Task<IHttpActionResult> DeletePhoto(int userId, int id)
        {
            //if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //    return Unauthorized();
            Account acc = new Account(
                "vatsac",
                "936419744999822",
                "w39XjwviejNAug2X74Hfg2I7_qg"
            );

            var _cloudinary = new Cloudinary(acc);

            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if ((bool)photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");

            if (photoFromRepo.PublicID != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicID);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    db.Entry(photoFromRepo).State = EntityState.Deleted;

                }
            }

            if (photoFromRepo.PublicID == null)
            {
                db.Entry(photoFromRepo).State = EntityState.Deleted;
            }

            await db.SaveChangesAsync();
                return Ok();

          //  return BadRequest("Failed to delete the photo");

        }
    }
}
