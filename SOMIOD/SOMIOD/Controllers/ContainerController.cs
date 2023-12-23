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
        //---------------- SETTING UP THE DB --------------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        public int fetchParentId(string appName)
        {
            int parentID = -1;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Applications WHERE Name=@appName", sqlConnection);
            cmd.Parameters.AddWithValue("appName", appName);
            cmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sqlDataReader = cmd.ExecuteReader();

            if (sqlDataReader.Read()) parentID = (int)sqlDataReader["Id"];
            return parentID;
        }
        //-------------------------------------------------------


        [Route("api/applications/{appName}/containers")]
        public IEnumerable<Container> Get(string appName)
        {
            int parentID = fetchParentId(appName);
            List<Container> containers = new List<Container>();
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Containers WHERE Parent = @parentID", sqlConnection);
            cmd.Parameters.AddWithValue("parentID", parentID);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
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

            return containers;
        }

        [Route("api/applications/{appName}/containers/{contName}")]
        public IHttpActionResult Get(string appName, string contName)
        {
            int parentID = fetchParentId(appName);
            Container returnCont = null;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Containers WHERE Parent = @parentID AND Name = @contName", sqlConnection);
            cmd.Parameters.AddWithValue("parentID", parentID);
            cmd.Parameters.AddWithValue("contName", contName);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
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

            if (returnCont == null) return BadRequest();
            return Ok(returnCont);
        }

        // POST: api/Container
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Container/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Container/5
        public void Delete(int id)
        {
        }
    }
}
