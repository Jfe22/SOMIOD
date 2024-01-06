using RestSharp;
using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote
{
    public partial class Form1 : Form
    {

        string baseURI = @"http://localhost:49760/api/somiod/";
        RestClient restClient = null;

        public Form1()
        {
            InitializeComponent();
            restClient = new RestClient(baseURI);
            RestRequest request = new RestRequest("remote", Method.Get);
            RestResponse response = restClient.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                SOMIOD.Models.Application application = new SOMIOD.Models.Application
                {
                    Name = "Remote"
                };
                RestRequest requestCreateSwitch = new RestRequest("", Method.Post);
                requestCreateSwitch.AddObject(application);
                RestResponse responseCreateSwitch = restClient.Execute(requestCreateSwitch);
            }
        }

        private void buttonOn_Click(object sender, EventArgs e)
        {
            RequestCreateDataSubscription requestBody = new RequestCreateDataSubscription()
            {
                Res_type = "data",
                Name = "ON",
                Content = "ON"
            };
            RestRequest requestCreateData = new RestRequest("smarttv/session1", Method.Post);
            requestCreateData.AddObject(requestBody);
            RestResponse responseCreateData = restClient.Execute(requestCreateData);
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            RequestCreateDataSubscription requestBody = new RequestCreateDataSubscription()
            {
                Res_type = "data",
                Name = "OFF",
                Content = "OFF"
            };
            RestRequest requestCreateData = new RestRequest("smarttv/session1", Method.Post);
            requestCreateData.AddObject(requestBody);
            RestResponse responseCreateData = restClient.Execute(requestCreateData);
        }
    }
}
