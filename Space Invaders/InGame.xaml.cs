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
using System.Windows.Media; //necessaria per l'utilizzo di contenuti multimediali
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading; //utilizzo della libreria Threading per gestire il timer

namespace Space_Invaders
{
    public partial class InGame : Window
    {
        bool destra, sinistra, gameOver = false;

        List<Rectangle> oggettiDaRimuovere = new List<Rectangle>();

        int immagineNemico, timerProiettile, limiteTimerProiettile, nemiciTotali, velocitaNemici;
        double altezzaProiettiliNemici;

        DispatcherTimer timerGioco = new DispatcherTimer();
        ImageBrush skinGiocatore = new ImageBrush();
        ImageBrush gameOverImage = new ImageBrush();

        System.Media.SoundPlayer soundEffects = new System.Media.SoundPlayer();

        public InGame(int nemTot, int velNem)
        {
            //inizializzazione di tutte le variabili necessarie per settare i vari valori per la partita

            this.nemiciTotali = nemTot;
            this.velocitaNemici = velNem;
            InitializeComponent();

            timerProiettile = 0;
            limiteTimerProiettile = 80;

            altezzaProiettiliNemici = 10;

            timerGioco.Tick += GameLoop; //il metodo gameLoop si ripete ad ogni ripetizione del controllo timer (ad ogni "Tick")
            timerGioco.Interval = TimeSpan.FromMilliseconds(10);
            timerGioco.Start(); //avvio del timer

            skinGiocatore.ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/player.png"));
            giocatore.Fill = skinGiocatore;

            canvas1.Focus(); //imposta lo stato attivo per questo elemento
            GeneraNemici(nemiciTotali);
            lblNemiciRimasti.Content = $"Nemici rimanenti: {nemiciTotali}";
        }

        //METODO CHE SI RIPETE AD OGNI RIPETIZIONE DEL TIMER (è il "cuore" del funzionamento del gioco)
        private void GameLoop(object sender, EventArgs e)
        {
            //creazione della HitBox del giocatore, che è un oggetto di tipo Rect nel quale vengono salvate delle coordinate che delimitano il box
            Rect hitBoxGiocatore = new Rect(Canvas.GetLeft(giocatore), Canvas.GetTop(giocatore), giocatore.Width, giocatore.Height);
            
            //dal momento in cui in un dato istante la variabile sinistra o destra sono true, la posizione del giocatore viene cambiata (viene modificato il valore della sua coordinata sul piano orizzontale)
            if(sinistra == true && Canvas.GetLeft(giocatore)>0)
                Canvas.SetLeft(giocatore, Canvas.GetLeft(giocatore) - 10);

            if (destra == true && Canvas.GetLeft(giocatore) + 75 < this.Width)
                Canvas.SetLeft(giocatore, Canvas.GetLeft(giocatore) + 10);

            //i proiettili nemici vengono lanciati ad una frequenza costante
            timerProiettile -= 2;

            if(timerProiettile < 0)
            {
                GeneratoreProiettiliNemici(Canvas.GetLeft(giocatore) + 20, altezzaProiettiliNemici); //i proiettili vengono generati alla stessa altezza degli alieni
                timerProiettile = limiteTimerProiettile; //si resetta il timer dei proiettili
            }

            //CICLO CHE SCORRE TUTTI GLI OGGETTI DI TIPO Rectangle, CHE PERMETTE ALL'ALGORITMO DI COMPIERE VARIE AZIONI IN BASE AL LORO TAG
            foreach (var x in canvas1.Children.OfType<Rectangle>()) 
            {
                //comportamento dei PROIETTILI DEL GIOCATORE ad ogni ripetizione del ciclo
                if(x is Rectangle && (string)x.Tag == "proiettile")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20); //il proiettile viene fatto salire verso l'alto di 20 px ogni volta

                    if (Canvas.GetTop(x) < x.Height) //nel caso in cui il proiettile non serva più (è già andato in alto) viene rimosso per liberare la memoria ed "ottimizzare" il gioco
                        oggettiDaRimuovere.Add(x);

                    //si crea il "box" del proiettile delimitandone le coordinate
                    Rect proiettile = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    //ciclo che scorre nuovamente tutti i rettangoli, ma che si "concentra" solo con quelli contrassegnati con il tag "nemico"
                    foreach (var j in canvas1.Children.OfType<Rectangle>())
                    {
                        if(j is Rectangle && (string)j.Tag == "nemico")
                        {
                            //creazione della hitBox dei nemici
                            Rect hitBoxNemico = new Rect(Canvas.GetLeft(j), Canvas.GetTop(j), j.Width, j.Height);

                            //in caso di intersezioni tra le coordinate del box del proiettile e le coordinate del box del nemico, sia il proiettile sia il nemico vengono rimossi
                            if (proiettile.IntersectsWith(hitBoxNemico))
                            {
                                oggettiDaRimuovere.Add(j);
                                oggettiDaRimuovere.Add(x);

                                nemiciTotali -= 1;
                                lblNemiciRimasti.Content = $"Nemici rimanenti: {nemiciTotali}"; //modifica del testo della label
                            }
                                
                        }
                    }
                }

                //comportamento dei NEMICI ad ogni ripetizione del ciclo
                if (x is Rectangle && (string)x.Tag == "nemico")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + velocitaNemici);

                    if(Canvas.GetLeft(x) > canvas1.ActualWidth)
                    {
                        Canvas.SetLeft(x, -x.Width);
                        Canvas.SetTop(x, Canvas.GetTop(x) + x.Height+10);
                        altezzaProiettiliNemici = Canvas.GetTop(x);
                    }

                    Rect hitBoxNemico  = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (hitBoxGiocatore.IntersectsWith(hitBoxNemico))
                        VisualizzaGameOver("SEI STATO UCCISO DAGLI INVASORI!!");
                }

                //comportamento dei PROIETTILI NEMICI ad ogni ripetizioned el ciclo
                if (x is Rectangle && (string)x.Tag == "proiettileNemico")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 5);

                    if (Canvas.GetTop(x) > canvas1.ActualHeight-x.Height)
                    {
                        oggettiDaRimuovere.Add(x);
                    }

                    Rect hitBoxProiettileNemico = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (hitBoxGiocatore.IntersectsWith(hitBoxProiettileNemico))
                        VisualizzaGameOver("SEI STATO UCCISO DA UN PROIETTILE!!");
                }
            }

            //pulizia dal canvas1 di tutti gli oggetti da rimuovere (immagazzinati nell'apposita lista)
            foreach (Rectangle daTogliere in oggettiDaRimuovere)
            {
                canvas1.Children.Remove(daTogliere);
            }

            if (nemiciTotali < 1)
                VisualizzaGameOver("HAI SCONFITTO TUTTI GLI INVASORI");
        }



        //METODI ASSOCIATI ALL'OGGETTO canvas1 CHE PERCEPISCONO LA PRESSIONE DEI TASTI DELLA TASTIERA
        private void TastoGiu(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left || e.Key == Key.A)
                sinistra = true;

            if(e.Key == Key.Right || e.Key == Key.D)
                destra = true;
        }

        private void TastoSu(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
                sinistra = false;

            if (e.Key == Key.Right || e.Key == Key.D)
                destra = false;

            if(e.Key == Key.Space)
            {
                //generazione dell'oggetto di tipo Rectangle corrispondente al proiettile sparato
                Rectangle nuovoProiettile = new Rectangle
                {
                    Tag = "proiettile", //questi oggetti vengono contrassegnati con il tag "proiettile"
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.Orange
                };

                //vengono generati sopra alla skin del giocatore
                Canvas.SetTop(nuovoProiettile, Canvas.GetTop(giocatore) - nuovoProiettile.Height);
                Canvas.SetLeft(nuovoProiettile, Canvas.GetLeft(giocatore) + giocatore.Width/2);

                canvas1.Children.Add(nuovoProiettile); //aggiunta dell'oggetto al canvas
            }


            //nel caso in cui il gioco fosse finito, la pressione del tasto ENTER permette il ritorno al menu principale del gioco
            if(e.Key == Key.Enter && gameOver==true)
            {
                this.Close();
            }
        }


        //metodi per la GENERAZIONE DEI COMPONENTI DI GIOCO sullo schermo

        //generatore dei proiettili nemici
        private void GeneratoreProiettiliNemici(double x, double y)
        {
            Rectangle proiettileNemico = new Rectangle
            {
                Tag = "proiettileNemico", //per identificare gli elementi di tipo rectangle che costituiscono i proiettili nemici li si contrassegna con il tag apposito
                Height = 40,
                Width = 10,
                Fill = Brushes.Red,
                Stroke = Brushes.Purple,
                StrokeThickness = 1
            };

            Canvas.SetTop(proiettileNemico, y);
            Canvas.SetLeft(proiettileNemico, x);
            canvas1.Children.Add(proiettileNemico);
        }


        //generatore dei nemici
        private void GeneraNemici(int max)
        {
            int sinistra = 0;
            Random generatore = new Random();

            for (int i = 0; i < max; i++)
            {
                ImageBrush skinNemici = new ImageBrush();

                Rectangle nuovoNemico = new Rectangle
                {
                    Tag = "nemico", //l'oggetto generato viene contrassegnato con il tag "nemico"
                    Height = 45,
                    Width = 45,
                    Fill = skinNemici
                };

                //posizionamento dei nuovi nemici
                Canvas.SetTop(nuovoNemico, 10);
                Canvas.SetLeft(nuovoNemico, sinistra);
                canvas1.Children.Add(nuovoNemico);
                sinistra -= (int)nuovoNemico.Width+15;

                //ad ogni nemico viene assegnata un'immagine random tra quelle presenti nella cartella media
                immagineNemico = generatore.Next(1, 9);
                skinNemici.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/media/invader{immagineNemico}.gif"));

            }
        }

        //visualizzazione dei controlli per la fine della partita (sia in caso di vincita, sia in caso di perdita)
        private void VisualizzaGameOver(string msg)
        {
            //impostazione dell'immagine da applicare al rettangolo rectGameOver e del suono da associare a soundEffects in base all'esito della partita
            if(nemiciTotali>=1)
            {
                gameOverImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/game_over.jpg"));
                soundEffects.SoundLocation = "../../media/game_over.wav";
            }
                
            else
            {
                gameOverImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/you_win.jpg"));
                soundEffects.SoundLocation = "../../media/you_win.wav";

            }

            soundEffects.Play();
            gameOver = true;
            timerGioco.Stop();

            rectGameOver.Fill = gameOverImage;
            rectGameOver.Visibility = Visibility.Visible;
            
            lblGameOver.Content = msg + "\nPremi ENTER per tornare al menù";
        }
    }
}
