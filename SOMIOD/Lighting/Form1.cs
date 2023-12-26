using RestSharp;
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

        string baseURI = @"http://localhost:49760/api/somiod/";
        RestClient restClient = null;

        public Form1()
        {
            InitializeComponent();
            restClient = new RestClient(baseURI);
        }

        private void buttonConnectApp_Click(object sender, EventArgs e)
        {

            //fazer pedido get a application lightbulb
            
            //se nao existir fazer post e criar

            //no final fazer a ligacao como receiver nos web sockets?

            //por motivos de teste, aproveitar ja este butao para apanhar o valor do light bulb e meter na
            //box???



        }
    }

}
