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



        [Route("api/somiod/applications")]

        public IEnumerable<Application> Get()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null; 
            List<Application> applications = new List<Application>();
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Applications", sqlConnection);
                sqlDataReader = cmd.ExecuteReader();
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
            }
            catch (Exception ex)
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
            }

            return applications;
        }


        [Route("api/somiod/{appName}")]
        public IHttpActionResult Get(string appName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null; 
            Application returnApp = null;
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Applications WHERE Name=@appName");
                cmd.Parameters.AddWithValue("appName", appName);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = sqlConnection;
                sqlDataReader = cmd.ExecuteReader();
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
            catch (Exception ex)
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }


        [Route("api/somiod/applications")]
        public IHttpActionResult Post([FromBody]Application app)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Applications VALUES (@name, @creation_dt)", sqlConnection);
                cmd.Parameters.AddWithValue("name", app.Name);
                cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                int nrows = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                if (nrows <= 0) return BadRequest();
                return Ok(nrows);
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }


        [Route("api/somiod/{appName}")]
        public IHttpActionResult Put(String appName, [FromBody]Application app)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Applications SET name=@nameNew WHERE name=@nameOld", sqlConnection);
                cmd.Parameters.AddWithValue("nameNew", app.Name);
                cmd.Parameters.AddWithValue("nameOld", appName);
                //should time also change when updated???
                //cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                int nrows = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                if (nrows <= 0) return BadRequest();
                return Ok(nrows);
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }


        [Route("api/somiod/{appName}")]
        public IHttpActionResult Delete(String appName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Applications WHERE name=@appName", sqlConnection);
                cmd.Parameters.AddWithValue("appName", appName);
                int nrows = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                if (nrows <= 0) return BadRequest();
                return Ok(nrows);
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }
    }
}
