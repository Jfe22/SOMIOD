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

        //---------------- AUX -----------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;

        public int FetchParentId(string appName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            int parentID = -1;
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Applications WHERE Name=@appName", sqlConnection);
                cmd.Parameters.AddWithValue("appName", appName);
                cmd.CommandType = System.Data.CommandType.Text;
                sqlDataReader = cmd.ExecuteReader();

                if (sqlDataReader.Read()) parentID = (int)sqlDataReader["Id"];

                sqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
            }

            return parentID;
        }

        public IHttpActionResult DiscoverApplications()
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

            return Ok(applications);
        }

        public IHttpActionResult DiscoverContainers(string appName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            List<Container> containers = new List<Container>();
            try
            {
                int parentID = FetchParentId(appName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Containers WHERE Parent = @parentID", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                sqlDataReader = cmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Container container = new Container()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
                        Creation_dt = (string)sqlDataReader["Creation_dt"],
                        Parent = (int)sqlDataReader["Parent"],
                    };
                    containers.Add(container);
                }
                sqlDataReader.Close();
                sqlConnection.Close();
            }
            catch
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
            }

            return Ok(containers);
        }
        //---------------- --- -----------------


        //---------------- HTTP -----------------
        [Route("api/somiod")]
        public IHttpActionResult Get()
        {
            if (Request.Headers.Contains("somiod-discover"))
            {
                if ("application" == Request.Headers.GetValues("somiod-discover").FirstOrDefault())
                    return DiscoverApplications(); 
                else 
                    return BadRequest("Invalid somiod-discover header value. " +
                        "Did you mean 'application'?");
            }

            return BadRequest("URL only available with somiod-discover header. " +
                "Did you mean to use 'somiod-discover: application'?");
        }
        

        [Route("api/somiod/{appName}")]
        public IHttpActionResult Get(string appName)
        {
            if (Request.Headers.Contains("somiod-discover"))
            {
                if ("container" == Request.Headers.GetValues("somiod-discover").FirstOrDefault())
                    return DiscoverContainers(appName); 
                else 
                    return BadRequest("Invalid somiod-discover header value. " +
                        "Did you mean 'container'?");
            }

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

                if (nrows <= 0) return BadRequest("Could not create application resource");
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

                if (nrows <= 0) return NotFound();
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

                if (nrows <= 0) return NotFound();
                return Ok(nrows);
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }
        //---------------- ---- -----------------
    }
}
