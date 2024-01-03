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
            SOMIOD.Models.Data data = new SOMIOD.Models.Data
            {
                Name = "ON",
                Content = "ON"
            };
            RestRequest requestCreateData = new RestRequest("ligthing/light_bulb/data", Method.Post);
            requestCreateData.AddObject(data);
            RestResponse responseCreateData = restClient.Execute(requestCreateData);
        }

        private void buttonLightOff_Click(object sender, EventArgs e)
        {
            SOMIOD.Models.Data data = new SOMIOD.Models.Data
            {
                Name = "OFF",
                Content = "OFF"
            };
            RestRequest requestCreateData = new RestRequest("ligthing/light_bulb/data", Method.Post);
            requestCreateData.AddObject(data);
            RestResponse responseCreateData = restClient.Execute(requestCreateData);
        }
    }
}
