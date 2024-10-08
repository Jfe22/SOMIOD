﻿using SOMIOD.Models;
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
        [Route("api/somiod/{appName}/{contName}/data/{dataName}")]
        public IHttpActionResult Get(string contName, string dataName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataReader sqlDataReader = null;
            Data returnData = null;
            int parentID = FetchParentId(contName);
            try
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Data WHERE Parent = @parentID AND Name = @dataName", sqlConnection);
                cmd.Parameters.AddWithValue("parentID", parentID);
                cmd.Parameters.AddWithValue("dataName", dataName);
                sqlDataReader = cmd.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    returnData = new Data()
                    {
                        Id = (int)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
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


        /* ---------- POST HANDLED BY CONTAINERCONTROLLER --------------
        [Route("api/somiod/{appName}/{contName}/data")]
        public IHttpActionResult Post(string contName, [FromBody] Data data)
        {
        }
        ---------- POST HANDLED BY CONTAINERCONTROLLER -------------- */ 


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

                SqlCommand cmd = new SqlCommand("UPDATE Data SET content = @content, parent = @parent, name = @name WHERE id = @dataId", sqlConnection);
                cmd.Parameters.AddWithValue("name", data.Name);
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
