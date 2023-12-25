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


        [Route("api/somiod/{appName}/{contName}/data")]
        public IEnumerable<Data> Get(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            List<Data> dataList = new List<Data>();
            try
            {
                int parentID = FetchParentId(contName);
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
            }
            catch (Exception ex)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
            }

            return dataList;
        }

        //considering data has no name field, which attribute should we use in route? ---> curr using ID
        [Route("api/somiod/{appName}/{contName}/data/{dataId}")]
        public IHttpActionResult Get(string contName, string dataId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(contName);
                Data returnData = null;
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
            catch (Exception ex) 
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
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
