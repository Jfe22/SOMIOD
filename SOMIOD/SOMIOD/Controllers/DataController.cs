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
    public class DataController : ApiController
    {

        //---------------- SETTING UP THE DB --------------------
        string connectionString = SOMIOD.Properties.Settings.Default.ConnStr;
        public int fetchParentId(string contName)
        {
            int parentID = -1;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Containers WHERE Name=@contName", sqlConnection);
            cmd.Parameters.AddWithValue("contName", contName);
            cmd.CommandType = System.Data.CommandType.Text;
            SqlDataReader sqlDataReader = cmd.ExecuteReader();

            if (sqlDataReader.Read()) parentID = (int)sqlDataReader["Id"];
            return parentID;
        }
        //-------------------------------------------------------


        [Route("api/somiod/{appName}/{contName}/data")]
        public IEnumerable<Data> Get(string contName)
        {
            int parentID = fetchParentId(contName);
            List<Data> dataList = new List<Data>();
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Data WHERE Parent = @parentID", sqlConnection);
            cmd.Parameters.AddWithValue("parentID", parentID);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
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

            return dataList;
        }

        //considering data has no name field, which attribute should we use in route? ---> curr using ID
        [Route("api/somiod/{appName}/{contName}/data/{dataId}")]
        public IHttpActionResult Get(string contName, string dataId)
        {
            int parentID = fetchParentId(contName);
            Data returnData = null;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Data WHERE Parent = @parentID AND Id = @dataId", sqlConnection);
            cmd.Parameters.AddWithValue("parentID", parentID);
            cmd.Parameters.AddWithValue("dataId", dataId);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.Read())
            {
                returnData = new Data()
                {
                    Id = (int)sqlDataReader["Id"],
                    Content = (string)sqlDataReader["Content"],
                    Creation_dt = (string)sqlDataReader["Creation_dt"],
                    Parent = (int)sqlDataReader["Parent"],
                };
            }
            sqlDataReader.Close();
            sqlConnection.Close();

            if (returnData == null) return BadRequest();
            return Ok(returnData);
        }




        // POST: api/Data
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Data/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Data/5
        public void Delete(int id)
        {
        }

    }
}
