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
    public class ContainerController : ApiController
    {

        //---------------- AUX -----------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        public int FetchParentId(string resName, string resType)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            SqlCommand cmd = null;
            int parentID = -1;
            try
            {
                sqlConnection.Open();

                if ("Applications" == resType)
                    cmd = new SqlCommand("SELECT * FROM Applications WHERE Name=@resName", sqlConnection);
                if ("Containers" == resType)
                    cmd = new SqlCommand("SELECT * FROM Containers WHERE Name=@resName", sqlConnection);
                cmd.Parameters.AddWithValue("resName", resName);
                cmd.CommandType = System.Data.CommandType.Text;
                sqlDataReader = cmd.ExecuteReader();

                if (sqlDataReader.Read()) parentID = (int)sqlDataReader["Id"];

                sqlDataReader.Close();
                sqlConnection.Close();
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
            }

            return parentID;
        }

        public IHttpActionResult DiscoverData(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            List<Data> dataList = new List<Data>();
            try
            {
                int parentID = FetchParentId(contName, "Containers");
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Data WHERE Parent = @parentID", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                sqlDataReader = cmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Data data = new Data()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Content = (string)sqlDataReader["Content"],
                        Creation_dt = (string)sqlDataReader["Creation_dt"],
                        Parent = (int)sqlDataReader["Parent"],
                    };
                    dataList.Add(data);
                }
                sqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }

            return Ok(dataList);
        }

        public IHttpActionResult DiscoverSubscriptions(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            List<Subscription> subscriptions = new List<Subscription>();
            try
            {
                int parentID = FetchParentId(contName, "Containers");
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Subscriptions WHERE Parent = @parentID", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                sqlDataReader = cmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Subscription subscription = new Subscription()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
                        Creation_dt = (string)sqlDataReader["Creation_dt"],
                        Parent = (int)sqlDataReader["Parent"],
                        Event = (string)sqlDataReader["Event"],
                        Endpoint = (string)sqlDataReader["Endpoint"],
                    };
                    subscriptions.Add(subscription);
                }
                sqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }

            return Ok(subscriptions);
        }
        //---------------- --- -----------------

        //---------------- HTTP -----------------
        [Route("api/somiod/{appName}/{contName}")]
        public IHttpActionResult Get(string appName, string contName)
        {
            if (Request.Headers.Contains("somiod-discover"))
            {
                if ("data" == Request.Headers.GetValues("somiod-discover").FirstOrDefault())
                    return DiscoverData(contName); 
                if ("subscription" == Request.Headers.GetValues("somiod-discover").FirstOrDefault())
                    return DiscoverSubscriptions(contName); 

                return BadRequest("Invalid somiod-discover header value. " +
                    "Did you mean 'data' or 'subscription'?");
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            try
            {
                Container returnCont = null;
                int parentID = FetchParentId(appName, "Applications");
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Containers WHERE Parent = @parentID AND Name = @contName", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                cmd.Parameters.AddWithValue("contName", contName);
                sqlDataReader = cmd.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    returnCont = new Container()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
                        Creation_dt = (string)sqlDataReader["Creation_dt"],
                        Parent = (int)sqlDataReader["Parent"],
                    };
                }
                sqlDataReader.Close();
                sqlConnection.Close();

                if (returnCont == null) return NotFound();
                return Ok(returnCont);
            }
            catch (Exception ex)
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }

        [Route("api/somiod/{appName}/containers")]
        public IHttpActionResult Post(string appName, [FromBody]Container container)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(appName, "Applications");
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Containers VALUES (@name, @creation_dt, @parent)", sqlConnection);
                cmd.Parameters.AddWithValue("name", container.Name);
                cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                cmd.Parameters.AddWithValue("parent", parentID);
                int nrows = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                if (nrows <= 0) return BadRequest("Could not create container resource");
                return Ok(nrows);
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }

        [Route("api/somiod/{appName}/{contName}")]
        public IHttpActionResult Put(string appName, string contName, [FromBody]Container container)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(appName, "Applications");
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Containers SET name = @name, parent = @parent WHERE name = @nameOld", sqlConnection);
                cmd.Parameters.AddWithValue("name", container.Name);
                cmd.Parameters.AddWithValue("parent", parentID);
                //here we might want to change the parent so routeParent != requestParent and this makes sense 
                //cmd.Parameters.AddWithValue("parent", container.Parent);
                cmd.Parameters.AddWithValue("nameOld", contName);
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

        [Route("api/somiod/{appName}/{contName}")]
        public IHttpActionResult Delete(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Containers WHERE name=@contName", sqlConnection);
                cmd.Parameters.AddWithValue("contName", contName);
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
