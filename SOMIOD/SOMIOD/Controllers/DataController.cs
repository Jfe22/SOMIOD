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
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return parentID;
        }
        //---------------- --- -----------------


        //---------------- HTTP -----------------
        //considering data has no name field, which attribute should we use in route? ---> curr using ID
        [Route("api/somiod/{appName}/{contName}/data/{dataId}")]
        public IHttpActionResult Get(string contName, string dataId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            try
            {
                Data returnData = null;
                int parentID = FetchParentId(contName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Data WHERE Parent = @parentID AND Id = @dataId", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                cmd.Parameters.AddWithValue("dataId", dataId);
                sqlDataReader = cmd.ExecuteReader();
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

                if (returnData == null) return NotFound();
                return Ok(returnData);
            } 
            catch (Exception ex) 
            {
                if (!sqlDataReader.IsClosed) sqlDataReader.Close();
                if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                return BadRequest(ex.Message);
            }
        }




        [Route("api/somiod/{appName}/{contName}/data")]
        public IHttpActionResult Post(string contName, [FromBody] Data data)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            //apenas descomentar se comecarmos a usar Name no model da data -- MANDAR MAIL A PROF
            //int tries = 0;
            //string uniqueNameGen = "";
            //while (true)
            //{
                try
                {
                    int parentID = FetchParentId(contName);
                    sqlConnection.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO Data VALUES (@content, @creation_dt, @parent)", sqlConnection);
                    cmd.Parameters.AddWithValue("content", data.Content);
                    cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
                    cmd.Parameters.AddWithValue("parent", parentID);
                    int nrows = cmd.ExecuteNonQuery();
                    sqlConnection.Close();

                    //publish mensage here??
                    //MttqClient teste = new MttqClient("IPAddress.Parse("...
                    //----------------------

                    if (nrows <= 0) return BadRequest("Could not create data resource");
                    return Ok(nrows);
                }
                catch (Exception ex)
                {
                    if (sqlConnection.State == System.Data.ConnectionState.Open) sqlConnection.Close();
                    return BadRequest(ex.Message);
                }
            //}
        }


        [Route("api/somiod/{appName}/{contName}/data/{dataId}")]
        public IHttpActionResult Put(string contName, int dataId, [FromBody] Data data)
        {
            return BadRequest("UPDATE OPERATION NOT ALLOWED FOR DATA AND SUBSCRIPTIONS");

            /* ----------- UNCOMMENT TO ENABLE UPDATE OPERATIONS FOR DATA ------------ 

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                int parentID = FetchParentId(contName);
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Data SET content = @content, parent = @parent WHERE id = @dataId", sqlConnection);
                cmd.Parameters.AddWithValue("content", data.Content);
                cmd.Parameters.AddWithValue("parent", parentID);
                cmd.Parameters.AddWithValue("dataId", dataId);
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

        [Route("api/somiod/{appName}/{contName}/data/{dataId}")]
        public IHttpActionResult Delete(int dataId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM Data WHERE id=@dataId", sqlConnection);
                cmd.Parameters.AddWithValue("dataId", dataId);
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
