using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    public class ApplicationController : ApiController
    {

        //------------- JUST FOR TESTING -- TODO: USE DB ------------- 
        List<Application> applications = new List<Application>
        {
            new Application {Id = 1, Name = "FirstApp", Creation_Dt = DateTime.Now.ToString("yyyy-M-dd H:m:ss")},
            new Application {Id = 2, Name = "SecondApp", Creation_Dt = DateTime.Now.ToString("yyyy-M-dd H:m:ss")},
        };
        //------------------------------------------------------------


        //TODO: Change methods to use DB

        [Route("api/applications")]
        public IEnumerable<Application> Get()
        {
            return applications;
        }


        [Route("api/applications/{name}")]
        public IHttpActionResult Get(string name)
        {
            Application application = applications.FirstOrDefault((p) => p.Name == name);
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }

        [Route("api/applications")]
        public IHttpActionResult Post([FromBody]Application app)
        {
            int initial_size = applications.Count();

            Application newApp = new Application();
            newApp.Id = app.Id;
            newApp.Name = app.Name;
            newApp.Creation_Dt = DateTime.Now.ToString("yyyy-M-dd H:m:ss");

            applications.Add(newApp);

            if (applications.Count == initial_size)
            {
                return BadRequest();
            } 
            return Ok(applications);
        }

        [Route("api/applications/{name}")]
        public IHttpActionResult Put(String name, [FromBody]Application app)
        {
            Application application = applications.FirstOrDefault(p => p.Name == name);
            Application originalApp = application;

            application = app;

            if (application == originalApp)
            {
                return BadRequest();
            }
            return Ok(applications);
        }

        [Route("api/applications/{name}")]
        public IHttpActionResult Delete(String name)
        {
            int initial_size = applications.Count();
            Application app_to_delete = applications.FirstOrDefault(p => p.Name == name); 

            applications.Remove(app_to_delete);

            if (applications.Count() == initial_size)
            {
                return BadRequest();
            }
            return Ok(applications);
        }
    }
}
