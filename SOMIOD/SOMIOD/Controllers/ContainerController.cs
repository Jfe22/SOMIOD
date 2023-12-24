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
        [Route("api/applications/{appName}/containers")]
        public IHttpActionResult Post(string appName, [FromBody]Container container)
        {
            int parentID = fetchParentId(appName);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO Containers VALUES (@name, @creation_dt, @parent)", sqlConnection);
            cmd.Parameters.AddWithValue("name", container.Name);
            cmd.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-M-dd H:m:ss"));
            cmd.Parameters.AddWithValue("parent", parentID);
            //cmd.Parameters.AddWithValue("name", container.Parent);
            //does the container even need to be passed on the request? appName is already in query so its duplicate on request
            int nrows = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nrows <= 0) return BadRequest();
            return Ok(nrows);
        }

        // PUT: api/Container/5
        [Route("api/applications/{appName}/containers/{contName}")]
        public IHttpActionResult Put(string appName, string contName, [FromBody]Container container)
        {
            int parentID = fetchParentId(appName);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("UPDATE Containers SET name = @name, parent = @parent WHERE name = @nameOld", sqlConnection);
            cmd.Parameters.AddWithValue("name", container.Name);
            //cmd.Parameters.AddWithValue("parent", parentID);
            //here we might want to change the parent so routeParent =! requestParent and this makes sense 
            cmd.Parameters.AddWithValue("parent", container.Parent);
            cmd.Parameters.AddWithValue("nameOld", contName);
            int nrows = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nrows <= 0) return BadRequest();
            return Ok(nrows);


        }

        [Route("api/applications/{appName}/containers/{contName}")]
        public IHttpActionResult Delete(string contName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM Containers WHERE name=@contName", sqlConnection);
            cmd.Parameters.AddWithValue("contName", contName);
            int nrows = cmd.ExecuteNonQuery();
            sqlConnection.Close();

            if (nrows <= 0) return BadRequest();
            return Ok(nrows);
        }
    }
}
