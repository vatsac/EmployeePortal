using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeePortal.API.Dtos
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }

        public HttpPostedFile File { get; set; } =  HttpContext.Current.Request.Files.Count > 0 ?
        HttpContext.Current.Request.Files[0] : null;

        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public string PublicId { get; set; }

        public int UserId { get; set; }

        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}