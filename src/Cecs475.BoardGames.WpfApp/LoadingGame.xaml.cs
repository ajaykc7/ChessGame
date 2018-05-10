using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.WpfApp
{
    /// <summary>
    /// Interaction logic for LoadingGame.xaml
    /// </summary>
    public partial class LoadingGame : Window
    {
        public LoadingGame()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var client = new RestClient("https:///cecs475-boardgames.herokuapp.com");
            var request = new RestRequest("api/games", Method.GET);

            var task = client.ExecuteTaskAsync(request);
            var response = await task;

            /*if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                MessageBox.Show("Game not found");
            }
            else
            {
                JObject obj = JObject.Parse(response.Content);
            }*/

            //needs to be inside a foor loop
            string remoteUri = "";
            string fileName = "";
            string webResource = remoteUri + fileName;

            WebClient mWebClient = new WebClient();
            var downloadTask = mWebClient.DownloadFileTaskAsync(webResource, fileName);

            await downloadTask;
            
            

            Window.GetWindow(new GameChoiceWindow()).Show();
            this.Close();
        }
    }
}
