using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using uPLibrary.Networking.M2Mqtt;

namespace SOMIOD.Controllers
{
    public class ContainerController : ApiController
    {

        //---------------- AUX -----------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        MqttClient mqttClient = null;
        string[] mStrTopicsInfo = {"creation", "deletion"};

        //mqttClient = new MqttClient(mqttBrokerString); 

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
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return parentID;
        }

        public List<Data> DiscoverData(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            List<Data> dataList = new List<Data>();
            int parentID = FetchParentId(contName, "Containers");
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Data WHERE Parent = @parentID", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                sqlDataReader = cmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Data data = new Data()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return dataList;
        }

        public List<Subscription> DiscoverSubscriptions(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            List<Subscription> subscriptions = new List<Subscription>();
            int parentID = FetchParentId(contName, "Containers");
            try
            {
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
                        Event = (int)sqlDataReader["Event"],
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return subscriptions;
        }

        public IHttpActionResult CreateData(string contName, Data data)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            int parentID = FetchParentId(contName, "Containers");
            int tries = 0;
            string uniqueNameGen = "";
            while (true)
            {
                try
                {
                    sqlConnection.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO Data VALUES (@name, @content, @creation_dt, @parent)", sqlConnection);
                    cmd.Parameters.AddWithValue("name", data.Name + uniqueNameGen);
                    cmd.Parameters.AddWithValue("content", data.Content);
                    cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                    cmd.Parameters.AddWithValue("parent", parentID);
                    int nrows = cmd.ExecuteNonQuery();
                    sqlConnection.Close();

                    if (nrows <= 0) return BadRequest("Could not create data resource");
                    return Ok(nrows);
                }
                catch (Exception ex)
                {
                    if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                    uniqueNameGen = "(" + ++tries + ")";
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public IHttpActionResult CreateSubscription(string contName, Subscription subscription)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            int parentID = FetchParentId(contName, "Containers");
            int tries = 0;
            string uniqueNameGen = "";
            while (true)
            {
                try
                {
                    sqlConnection.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO Subscriptions VALUES (@name, @creation_dt, @parent, @event, @endpoint)", sqlConnection);
                    cmd.Parameters.AddWithValue("name", subscription.Name + uniqueNameGen);
                    cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                    System.Diagnostics.Debug.WriteLine(parentID);
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
                    uniqueNameGen = "(" + ++tries + ")";
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        //---------------- --- -----------------

        //---------------- HTTP -----------------
        [Route("api/somiod/{appName}/{contName}")]
        public IHttpActionResult Get(string appName, string contName)
        {
            if (Request.Headers.Contains("somiod-discover"))
            {
                if ("data" == Request.Headers.GetValues("somiod-discover").FirstOrDefault())
                    return Ok(DiscoverData(contName));
                if ("subscription" == Request.Headers.GetValues("somiod-discover").FirstOrDefault())
                    return Ok(DiscoverSubscriptions(contName));

                return BadRequest("Invalid somiod-discover header value. " +
                    "Did you mean 'data' or 'subscription'?");
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            Container returnCont = null;
            int parentID = FetchParentId(appName, "Applications");
            try
            {
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

        [Route("api/somiod/{appName}/{contName}")]
        public IHttpActionResult Post(string appName, string contName, [FromBody]RequestCreateDataSubscription resource)
        {
            if (resource.Res_type == "data")
            {

                if (resource.Content == null) return BadRequest("Content can't be empty");
                Data parcialData = new Data()
                {
                    Name = resource.Name,
                    Content = resource.Content
                };

                // WHEN DATA IS CREATED WE SEND A MQTT MESSAGE
                List<Subscription> subs = new List<Subscription>();
                subs = DiscoverSubscriptions(contName);
                foreach (Subscription sub in subs)
                {
                    if (sub.Event == 1) 
                    {
                        mqttClient = new MqttClient(sub.Endpoint);
                        mqttClient.Connect(Guid.NewGuid().ToString());
                        System.Diagnostics.Debug.WriteLine(sub.Endpoint);
                        if (mqttClient.IsConnected)
                        {
                            System.Diagnostics.Debug.WriteLine("WEBSOCKET CONNECTED");
                            mqttClient.Publish("creation", Encoding.UTF8.GetBytes(resource.Content));
                            System.Diagnostics.Debug.WriteLine("message published");
                        }
                    }
                } 
                // -------------------------------------------
                return CreateData(contName, parcialData);
            }

            if (resource.Res_type == "subscription")
            {
                if (resource.Event < 1 || resource.Event > 2) 
                    return BadRequest("Invalid Event value. Acepted values are 1 and 2. 1 is for creation, 2 for deletion");
                if (resource.Endpoint == null) return BadRequest("Endpoint can't be empty");
                Subscription partialSubscription = new Subscription()
                {
                    Name = resource.Name,
                    Event = resource.Event,
                    Endpoint = resource.Endpoint
                };
                return CreateSubscription(contName, partialSubscription);
            }

            return BadRequest("POSTING TO THIS ENDPOINT REQUIRES THE USE OF res_type BODY PARAM. " +
                "USAGE: 'res_type: data' OR 'res_type: subscription");
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
