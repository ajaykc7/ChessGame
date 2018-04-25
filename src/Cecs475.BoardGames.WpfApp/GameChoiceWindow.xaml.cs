using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
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

            
            Type iGameFactory = typeof(IWpfGameFactory);

            Assembly tttModelassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games/Cecs475.BoardGames.TicTacToe.Model.dll");
            Assembly chessModelassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games/Cecs475.BoardGames.Chess.Model.dll");
            Assembly othelloModelassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games/Cecs475.BoardGames.Othello.Model.dll");
            Assembly othelloViewassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games/Cecs475.BoardGames.Othello.WpfView.dll");
            Assembly chessViewassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games/Cecs475.BoardGames.Chess.WpfView.dll");
            Assembly tttViewassembly = Assembly.LoadFrom("../../../../src/Cecs475.BoardGames.WpfApp/bin/Debug/games/Cecs475.BoardGames.TicTacToe.WpfView.dll");

            var gameTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => iGameFactory.IsAssignableFrom(t) && t.IsClass);

            var constructorList = gameTypes.Select(c => c.GetConstructor(Type.EmptyTypes));

            InitializeComponent();
		}

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
