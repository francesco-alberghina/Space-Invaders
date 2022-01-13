using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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



namespace Space_Invaders
{
    /// <summary>
    /// Logica di interazione per Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {

        System.Media.SoundPlayer backgroundTrackPlayer = new System.Media.SoundPlayer();

        //costruttore del menù principale
        public Menu()
        {
            InitializeComponent();

            //quando il nemù viene creato, viene riprodotta anche la colonna sonora
            backgroundTrackPlayer.SoundLocation = "../../media/soundtrack.wav";
            backgroundTrackPlayer.PlayLooping();
        }

        //metodi associati al click di ogni bottone del menù, 
        private void btnFacile_Click(object sender, RoutedEventArgs e)
        {
            InGame f2 = new InGame(5, 5);
            this.Visibility = Visibility.Hidden;
            f2.ShowDialog();
            this.Visibility = Visibility.Visible;
            backgroundTrackPlayer.PlayLooping();
        }

        private void btnMedio_Click(object sender, RoutedEventArgs e)
        {
            InGame f2 = new InGame(7, 10);
            this.Visibility = Visibility.Hidden;
            f2.ShowDialog();
            this.Visibility = Visibility.Visible;
            backgroundTrackPlayer.PlayLooping();
        }

        private void btnDifficile_Click(object sender, RoutedEventArgs e)
        {
            InGame f2 = new InGame(10, 15);
            this.Visibility = Visibility.Hidden;
            f2.ShowDialog();
            this.Visibility = Visibility.Visible;
            backgroundTrackPlayer.PlayLooping();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnInformazioni_Click(object sender, RoutedEventArgs e)
        {
            FrmInformazioni fInfo = new FrmInformazioni();
            fInfo.ShowDialog();
        }
    }
}
