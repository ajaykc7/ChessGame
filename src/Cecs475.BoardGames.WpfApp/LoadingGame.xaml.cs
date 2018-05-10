using Cecs475.BoardGames.WpfView;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class GameType
    {
        public GameFiles[] gameFiles { get; set; }
    }

    public class GameFiles
    {
        public string Name { get; set; }
        public File[] Files { get; set; }
    }

    public class File
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public string PublicKey { get; set; }
        public string Version { get; set; }
    }

    public partial class LoadingGame : Window
    {
        public LoadingGame()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var client = new RestClient("https://cecs475-boardamges.herokuapp.com");
            var request = new RestRequest("api/games", Method.GET);

            var response = await client.ExecuteTaskAsync<GameType>(request);

            var gameList = response.Data;
            //var response = await client.ExecuteTaskAsync(request);

            //JObject obj = JObject.Parse(response.Content);

            //var gameList = JsonConvert.DeserializeObject<List<IWpfGameFactory>>(response.Content);

            foreach (var game in gameList.gameFiles)
            {
               
                Debug.WriteLine(game.Name);
                string remoteUri = "";
                string fileName = "";
                string webResource = remoteUri + fileName;

                WebClient mWebClient = new WebClient();
                var downloadTask = mWebClient.DownloadFileTaskAsync(webResource, fileName);

                await downloadTask;
            }

            //var task = client.ExecuteTaskAsync(request);
            //var response = await task;

            /*if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                MessageBox.Show("Game not found");
            }
            else
            {
                JObject obj = JObject.Parse(response.Content);
            }*/

            //needs to be inside a foor loop
            



            Window.GetWindow(new GameChoiceWindow()).Show();
            this.Close();
        }
    }
}
