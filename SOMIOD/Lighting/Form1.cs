using RestSharp;
using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lighting
{
    public partial class Form1 : Form
    {


        //can we use the models like this??? had to add a reference to SOMIOD and I think I shouldn't

        string baseURI = @"http://localhost:49760/api/somiod/";
        RestClient restClient = null;

        public Form1()
        {
            InitializeComponent();
            restClient = new RestClient(baseURI);
        }

        private void buttonConnectApp_Click(object sender, EventArgs e)
        {
            RestRequest requestApp = new RestRequest("lighting", Method.Get);
            RestResponse responseApp = restClient.Execute(requestApp);

            if (responseApp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //create app resource
                SOMIOD.Models.Application application = new SOMIOD.Models.Application
                {
                    Name = "Lighting"
                };
                RestRequest requestCreateApp = new RestRequest("applications", Method.Post);
                requestCreateApp.AddObject(application);
                RestResponse responseCreateApp = restClient.Execute(requestCreateApp);
                if (responseCreateApp.StatusCode != System.Net.HttpStatusCode.OK) return;

                //create container resource
                SOMIOD.Models.Container container = new SOMIOD.Models.Container
                {
                    Name = "light_bulb"
                };
                RestRequest requestCreateCont = new RestRequest("lighting/containers", Method.Post);
                requestCreateCont.AddObject(container);
                RestResponse responseCreateCont = restClient.Execute(requestCreateCont);
                if (responseCreateApp.StatusCode != System.Net.HttpStatusCode.OK) return;

                //create subscription resource
                Subscription subscription = new Subscription
                {
                    Name = "sub1",
                    Event = "pdahahaha",
                    Endpoint = "poragorasla",
                };
                RestRequest requestCreateSub = new RestRequest("lighting/lighting_bulb/subscriptions", Method.Post);
                requestCreateSub.AddObject(subscription);
                RestResponse responseCreateSub = restClient.Execute(requestCreateCont);
            }

            RestRequest requestData = new RestRequest("lighting/light_bulb/data/8", Method.Get);
            requestData.RequestFormat = DataFormat.Xml;
            //var responseData = restClient.Execute<Data>(requestData).Data;
            Data responseData = restClient.Execute<Data>(requestData).Data;
            if (responseData == null) textBoxLightValue.Text = "No data yet";
            else textBoxLightValue.Text = responseData.Content;

            //turn the websocket on here as a listener???

        }
    }

}
