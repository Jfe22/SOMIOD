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
        //---------------- AUX -----------------
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
        //---------------- --- -----------------


        //---------------- HTTP -----------------
        [Route("api/somiod/{appName}/{contName}/sub/{subName}")]
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

        [Route("api/somiod/{appName}/{contName}/sub")]
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

        [Route("api/somiod/{appName}/{contName}/sub/{subName}")]
        public IHttpActionResult Put(string contName, string subName, [FromBody]Subscription subscription)
        {
            return BadRequest("UPDATE OPERATION NOT ALLOWED FOR DATA AND SUBSCRIPTIONS");

            /* ------- UNCOMMENT TO ENABLE UPDATE OPERATIONS FOR SUBSCRIPTIONS ------- 

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(contName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Subscriptions SET name=@name, parent=@parent, event=@event, endpoint=@endpoint WHERE name=@subName", sqlConnection);
                cmd.Parameters.AddWithValue("name", subscription.Name);
                cmd.Parameters.AddWithValue("parent", parentID);
                //here we might want to change the parent so routeParent != requestParent and this makes sense 
                //cmd.Parameters.AddWithValue("parent", subscription.Parent);
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
            ------- UPDATE OPERATION NOT ALLOWED FOR DATA AND SUBSCRIPTIONS ------- */ 
        }

        [Route("api/somiod/{appName}/{contName}/sub/{subName}")]
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
        //---------------- ---- -----------------
    }
}
