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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartTV
{
    public partial class Form1 : Form
    {
        string baseURI = @"http://localhost:49760/api/somiod/";
        RestClient restClient = null;
        MqttClient mqttClient = null;

        public Form1()
        {
            InitializeComponent();

            // ----------- set up http ------------------------
            restClient = new RestClient(baseURI);
            RestRequest requestApp = new RestRequest("smarttv", Method.Get);
            RestResponse responseApp = restClient.Execute(requestApp);

            if (responseApp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //create app resource
                SOMIOD.Models.Application application = new SOMIOD.Models.Application
                {
                    Name = "SmartTV"
                };
                RestRequest requestCreateApp = new RestRequest("", Method.Post);
                requestCreateApp.AddObject(application);
                RestResponse responseCreateApp = restClient.Execute(requestCreateApp);
                System.Diagnostics.Debug.WriteLine(responseCreateApp.Content);
                MessageBox.Show(responseCreateApp.Content);
                if (responseCreateApp.StatusCode != System.Net.HttpStatusCode.OK) return;

                //create container resource
                SOMIOD.Models.Container container = new SOMIOD.Models.Container
                {
                    Name = "session1"
                };
                RestRequest requestCreateCont = new RestRequest("smarttv", Method.Post);
                requestCreateCont.AddObject(container);
                RestResponse responseCreateCont = restClient.Execute(requestCreateCont);
                System.Diagnostics.Debug.WriteLine(responseCreateCont.Content);
                if (responseCreateApp.StatusCode != System.Net.HttpStatusCode.OK) return;

                //create subscription resource
                RequestCreateDataSubscription subscription = new RequestCreateDataSubscription
                {
                    Res_type = "subscription",
                    Name = "sub1",
                    Event = 1,
                    Endpoint = "test.mosquitto.org",
                };
                RestRequest requestCreateSub = new RestRequest("smarttv/session1", Method.Post);
                requestCreateSub.AddObject(subscription);
                RestResponse responseCreateSub = restClient.Execute(requestCreateSub);
                System.Diagnostics.Debug.WriteLine(responseCreateApp.Content);
            }
            // ----------- set up http ------------------------


            // ----------- set up mqtt ------------------------
            mqttClient = new MqttClient("test.mosquitto.org");
            string[] mStrTopicsInfo = { "create", "destroy" };
            mqttClient.Connect(Guid.NewGuid().ToString());
            if (!mqttClient.IsConnected) MessageBox.Show("Erro ao ligar sockets");

            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };

            mqttClient.Subscribe(mStrTopicsInfo, qosLevels);
            // ----------- set up mqtt ------------------------
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            textBoxValue.Text = Encoding.UTF8.GetString(e.Message);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RestRequest requestData = new RestRequest("smarttv/session1", Method.Get);
            requestData.RequestFormat = DataFormat.Xml;
            requestData.AddHeader("somiod-discover", "data");
            List<Data> responseData = restClient.Execute<List<Data>>(requestData).Data;
            if (responseData == null || responseData.Count == 0) textBoxValue.Text = "No data yet";
            else textBoxValue.Text = responseData[responseData.Count - 1].Content;
        }
    }
}
