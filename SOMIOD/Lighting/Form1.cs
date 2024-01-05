using RestSharp;
using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Lighting
{
    public partial class Form1 : Form
    {


        //can we use the models like this??? had to add a reference to SOMIOD and I think I shouldn't

        string baseURI = @"http://localhost:49760/api/somiod/";
        RestClient restClient = null;
        MqttClient mqttClient = null;

        public Form1()
        {
            InitializeComponent();
            restClient = new RestClient(baseURI);
            mqttClient = new MqttClient("127.0.0.1");
            string[] mStrTopicsInfo = { "create", "destroy" };
            mqttClient.Connect(Guid.NewGuid().ToString());
            if (!mqttClient.IsConnected) MessageBox.Show("Erro ao ligar sockets");

            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };

            mqttClient.Subscribe(mStrTopicsInfo, qosLevels);
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("GOT SOME");
            textBoxLightValue.Text = Encoding.UTF8.GetString(e.Message);
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
                RestRequest requestCreateApp = new RestRequest("", Method.Post);
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
                    Event = 1,
                    Endpoint = "localhost",
                };
                RestRequest requestCreateSub = new RestRequest("lighting/light_bulb/sub", Method.Post);
                requestCreateSub.AddObject(subscription);
                RestResponse responseCreateSub = restClient.Execute(requestCreateCont);
            }

            RestRequest requestData = new RestRequest("lighting/light_bulb", Method.Get);
            requestData.RequestFormat = DataFormat.Xml;
            requestData.AddHeader("somiod-discover", "data");
            List<Data> responseData = restClient.Execute<List<Data>>(requestData).Data;
            if (responseData == null || responseData.Count == 0) textBoxLightValue.Text = "No data yet";
            else textBoxLightValue.Text = responseData[responseData.Count - 1].Content;

        }
    }

}
