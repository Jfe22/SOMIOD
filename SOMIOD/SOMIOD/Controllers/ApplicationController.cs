using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    public class ApplicationController : ApiController
    {

        /*
        //------------- JUST FOR TESTING -- TODO: USE DB ------------- 
        List<Application> applications = new List<Application>
        {
            new Application {Id = 1, Name = "FirstApp", Creation_dt = DateTime.Now.ToString("yyyy-M-dd H:m:ss")},
            new Application {Id = 2, Name = "SecondApp", Creation_dt = DateTime.Now.ToString("yyyy-M-dd H:m:ss")},
        };
        //------------------------------------------------------------
        */

        //---------------- SETTING UP THE DB --------------------
        List<Application> applications = new List<Application>();
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        //-------------------------------------------------------


        //TODO: Change POST, PUT, DELETE methods to use DB

        [Route("api/applications")]
        public IEnumerable<Application> Get()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Applications", sqlConnection);

            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Application application = new Application()
                {
                    Id = (int)sqlDataReader["Id"],
                    Name = (string)sqlDataReader["Name"],
                    Creation_dt = (string)sqlDataReader["Creation_dt"]
                };
                applications.Add(application);
            }
            sqlDataReader.Close();
            sqlConnection.Close();

            return applications;
        }


        [Route("api/applications/{name}")]
        public IHttpActionResult Get(string name)
        {
            Application returnApp = null;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Applications WHERE Name=@name");
            cmd.Parameters.AddWithValue("name", name);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = sqlConnection;

            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.Read())
            {
                Application application = new Application()
                {
                    Id = (int)sqlDataReader["Id"],
                    Name = (string)sqlDataReader["Name"],
                    Creation_dt = (string)sqlDataReader["Creation_dt"]
                };
                returnApp = application;
            }
            sqlDataReader.Close();
            sqlConnection.Close();

            if (returnApp == null)
            {
                return NotFound();
            }
            return Ok(returnApp);
        }

        [Route("api/applications")]
        public IHttpActionResult Post([FromBody]Application app)
        {
            int initial_size = applications.Count();

            Application newApp = new Application();
            newApp.Id = app.Id;
            newApp.Name = app.Name;
            newApp.Creation_dt = DateTime.Now.ToString("yyyy-M-dd H:m:ss");

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
