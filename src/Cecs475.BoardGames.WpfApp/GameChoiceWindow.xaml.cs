using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Cecs475.BoardGames.WpfApp {
    /// <summary>
    /// Interaction logic for GameChoiceWindow.xaml
    /// </summary>
    public partial class GameChoiceWindow : Window {
        public GameChoiceWindow() {


            //Assembly ChessModelassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games");
            GameTypes = new List<IWpfGameFactory>();
            Type iGameFactory = typeof(IWpfGameFactory);
            var files = Directory.GetFiles("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games", "*.dll");
            foreach (var dll in files)
            {
                Assembly.LoadFrom(dll);
            }

            var boardTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => iGameFactory.IsAssignableFrom(t) && t.IsClass);


            foreach (var type in boardTypes)
            {
                var boardConstr = type.GetConstructor(Type.EmptyTypes);
                GameTypes.Add((IWpfGameFactory)boardConstr.Invoke(new object[0]));
                //GameTypes.Add((IWpfGameFactory)Activator.CreateInstance(type));
            }

            this.Resources.Add("GameTypes", GameTypes);

            InitializeComponent();
        }

        private List<IWpfGameFactory> GameTypes { get; set; }

		private void Button_Click(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			// Retrieve the game type bound to the button
			IWpfGameFactory gameType = b.DataContext as IWpfGameFactory;
			// Construct a GameWindow to play the game.
			var gameWindow = new GameWindow(gameType) {
				Title = gameType.GameName
			};
			// When the GameWindow closes, we want to show this window again.
			gameWindow.Closed += GameWindow_Closed;

			// Show the GameWindow, hide the Choice window.
			gameWindow.Show();
			this.Hide();
		}

		private void GameWindow_Closed(object sender, EventArgs e) {
			this.Show();
		}
	}
}
