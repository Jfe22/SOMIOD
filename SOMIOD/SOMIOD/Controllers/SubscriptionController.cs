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
    public class SubscriptionController : ApiController
    {
        //---------------- SET UP --------------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        public int FetchParentId(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            int parentID = -1;
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Containers WHERE Name=@contName", sqlConnection);
                cmd.Parameters.AddWithValue("contName", contName);
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
        //--------------------------------------------


        // GET: api/Subscription
        [Route("api/somiod/{appName}/{contName}/subscriptions")]
        public IEnumerable<Subscription> Get(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            List<Subscription> subscriptions = new List<Subscription>();
            try
            {
                int parentID = FetchParentId(contName);
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
            }

            return subscriptions;

        }

        // GET: api/Subscription/5
        [Route("api/somiod/{appName}/{contName}/subscriptions/{subName}")]
        public IHttpActionResult Get(string contName, string subName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null; 
            try
            {
                Subscription returnSub = null;
                int parentID = FetchParentId(contName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Subscriptions WHERE Parent = @parentID AND Name = @subName", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                cmd.Parameters.AddWithValue("subName", subName);
                sqlDataReader = cmd.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    returnSub = new Subscription()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
                        Creation_dt = (string)sqlDataReader["Creation_dt"],
                        Parent = (int)sqlDataReader["Parent"],
                        Event = (string)sqlDataReader["Event"],
                        Endpoint = (string)sqlDataReader["Endpoint"],
                    };
                }
                sqlDataReader.Close();
                sqlConnection.Close();

                if (returnSub == null) return NotFound();
                return Ok(returnSub);
            } 
            catch (Exception ex) 
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Subscription
        [Route("api/somiod/{appName}/{contName}/subscriptions")]
        public IHttpActionResult Post(string contName, [FromBody]Subscription subscription)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(contName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Subscriptions VALUES (@name, @creation_dt, @parent, @event, @endpoint)", sqlConnection);
                cmd.Parameters.AddWithValue("name", subscription.Name);
                cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                cmd.Parameters.AddWithValue("parent", parentID);
                cmd.Parameters.AddWithValue("event", subscription.Event);
                cmd.Parameters.AddWithValue("endpoint", subscription.Endpoint);
                //cmd.Parameters.AddWithValue("name", container.Parent);
                //does the container even need to be passed on the request? appName is already in query so its duplicate on request
                int nrows = cmd.ExecuteNonQuery();
                sqlConnection.Close();

                if (nrows <= 0) return BadRequest("Could not create subscription resource");
                return Ok(nrows);
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Subscription/5
        [Route("api/somiod/{appName}/{contName}/subscriptions/{subName}")]
        public IHttpActionResult Put(string contName, string subName, [FromBody]Subscription subscription)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(contName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Subscriptions SET name=@name, parent=@parent, event=@event, endpoint=@endpoint WHERE name=@subName", sqlConnection);
                cmd.Parameters.AddWithValue("name", subscription.Name);
                //cmd.Parameters.AddWithValue("parent", parentID);
                //here we might want to change the parent so routeParent =! requestParent and this makes sense 
                cmd.Parameters.AddWithValue("parent", subscription.Parent);
                cmd.Parameters.AddWithValue("event", subscription.Event);
                cmd.Parameters.AddWithValue("endpoint", subscription.Endpoint);
                cmd.Parameters.AddWithValue("subName", subName);
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

        // DELETE: api/Subscription/5
        [Route("api/somiod/{appName}/{contName}/subscriptions/{subName}")]
        public IHttpActionResult Delete(string subName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Subscriptions WHERE name=@subName", sqlConnection);
                cmd.Parameters.AddWithValue("subName", subName);
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
    }
}
