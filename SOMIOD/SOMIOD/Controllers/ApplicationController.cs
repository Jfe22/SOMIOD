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

        //---------------- SETTING UP THE DB --------------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        //-------------------------------------------------------


        //TODO: Use try-catch to catch user errors while requesting 

        //[Route("api/applications")]
        [Route("api/somiod/applications")]

        public IEnumerable<Application> Get()
        {
            List<Application> applications = new List<Application>();
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


        [Route("api/somiod/{name}")]
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
                returnApp = new Application()
                {
                    Id = (int)sqlDataReader["Id"],
                    Name = (string)sqlDataReader["Name"],
                    Creation_dt = (string)sqlDataReader["Creation_dt"]
                };
            }
            sqlDataReader.Close();
            sqlConnection.Close();

            if (returnApp == null) return NotFound();
            return Ok(returnApp);
        }


        [Route("api/somiod/applications")]
        public IHttpActionResult Post([FromBody]Application app)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO Applications VALUES (@name, @creation_dt)", sqlConnection);
            cmd.Parameters.AddWithValue("name", app.Name);
            cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
            int nrows = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nrows <= 0) return BadRequest(); 
            return Ok(nrows);
        }


        [Route("api/somiod/{name}")]
        public IHttpActionResult Put(String name, [FromBody]Application app)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("UPDATE Applications SET name=@nameNew WHERE name=@nameOld", sqlConnection);
            cmd.Parameters.AddWithValue("nameNew", app.Name);
            cmd.Parameters.AddWithValue("nameOld", name);
            //should time also change when updated???
            //cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
            int nrows = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nrows <= 0) return BadRequest();
            return Ok(nrows);
        }


        [Route("api/somiod/{name}")]
        public IHttpActionResult Delete(String name)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM Applications WHERE name=@name", sqlConnection);
            cmd.Parameters.AddWithValue("name", name);
            int nrows = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nrows <= 0) return BadRequest();
            return Ok(nrows);
        }
    }
}
