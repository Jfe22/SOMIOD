﻿using RestSharp;
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

namespace Switch
{
    public partial class Form1 : Form
    {
        
        string baseURI = @"http://localhost:49760/api/somiod/";
        RestClient restClient = null;

        public Form1()
        {
            InitializeComponent();
            restClient = new RestClient(baseURI);

            //verify here if the switch has already been created? should we do the same on the lighting app?
            RestRequest request = new RestRequest("switch", Method.Get);
            RestResponse response = restClient.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                SOMIOD.Models.Application application = new SOMIOD.Models.Application
                {
                    Name = "Switch"
                };
                RestRequest requestCreateSwitch = new RestRequest("applications", Method.Post);
                requestCreateSwitch.AddObject(application);
                RestResponse responseCreateSwitch = restClient.Execute(requestCreateSwitch);
            }
        }

        //dataId is hardcoded, will the app suport more then one light???
        private void buttonLightOn_Click(object sender, EventArgs e)
        {
            RestRequest request = new RestRequest("ligthing/light_bulb/data/8", Method.Get);
            RestResponse response = restClient.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                SOMIOD.Models.Data data = new SOMIOD.Models.Data
                {
                    Content = "ON"
                };
                RestRequest requestCreateData = new RestRequest("ligthing/light_bulb/data", Method.Post);
                requestCreateData.AddObject(data);
                RestResponse responseCreateData = restClient.Execute(requestCreateData);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SOMIOD.Models.Data data = new SOMIOD.Models.Data
                {
                    Content = "ON"
                };
                RestRequest requestUpdateData = new RestRequest("ligthing/light_bulb/data/8", Method.Put);
                requestUpdateData.AddObject(data);
                RestResponse responseUpdateData = restClient.Execute(requestUpdateData);
            }

        }

        private void buttonLightOff_Click(object sender, EventArgs e)
        {
            RestRequest request = new RestRequest("ligthing/light_bulb/data/8", Method.Get);
            RestResponse response = restClient.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                SOMIOD.Models.Data data = new SOMIOD.Models.Data
                {
                    Content = "OFF"
                };
                RestRequest requestCreateData = new RestRequest("ligthing/light_bulb/data", Method.Post);
                requestCreateData.AddObject(data);
                RestResponse responseCreateData = restClient.Execute(requestCreateData);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SOMIOD.Models.Data data = new SOMIOD.Models.Data
                {
                    Content = "OFF"
                };
                RestRequest requestUpdateData = new RestRequest("ligthing/light_bulb/data/8", Method.Put);
                requestUpdateData.AddObject(data);
                RestResponse responseUpdateData = restClient.Execute(requestUpdateData);
            }
        }
    }
}