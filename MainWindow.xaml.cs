
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace showdow
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructeur qui cree la form  
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            load();
            param.strAAfficher = "Bonjour!";
            w1 = new Window1(param);
            w1.Show();
            param.Timer = false;
            w1.update();
        }
        /// <summary>
        /// Form qui contient le label pour l'affichage du label des stats
        /// </summary>

        Window1 w1;
        Param param;
        public static string fileName = "param.json";


        /// <summary>
        /// Charge les parametres du fichier .json
        /// </summary>
        public void load()
        {
            if (File.Exists(fileName))
            {
                var jsonString = File.ReadAllText(fileName);
                param = JsonSerializer.Deserialize<Param>(jsonString);
                _eloBool.IsChecked = param._Elo;
                _GxeBool.IsChecked = param._Gxe;
                _LadderBool.IsChecked = param._Place;
            }
            else
            {
                param = new Param();
            }
        }

        /// <summary>
        /// Sauvegarde des parametres dans un fichier json
        /// </summary>
        public void save()
        {
            var jsonString = JsonSerializer.Serialize<Param>(param);
            File.WriteAllText(fileName, jsonString);

        }

        /// <summary>
        /// Gere le bool pour l'elo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _eloBool_Click(object sender, RoutedEventArgs e)
        {
            param._Elo = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            save();
        }
        /// <summary>
        /// Gere le bool pour le GXE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _GxeBool_Checked(object sender, RoutedEventArgs e)
        {
            param._Gxe = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            save();
        }

        /// <summary>
        /// Gere le bool pour le ladder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _LadderBool_Checked(object sender, RoutedEventArgs e)
        {

            param._Place = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            save();
        }



        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            maj();
            //    w1.updateText();

        }

        System.Timers.Timer timer;
        /// <summary>
        /// gere le timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            param._name = _compte.Text;
            param._tier = _Tier.Text;
            if (!param.Timer)
            {
                maj();
                timer = new System.Timers.Timer(30000);
                //             
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
                timer.Enabled = true;
                ti.Content = "Stop timer";

            }
            else
            {
                timer.Stop();
                timer.Dispose();
                ti.Content = "lancer timer";
            }
            param.Timer = !param.Timer;

        }
        /// <summary>
        /// sauvegarde des parametres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            save();
        }


        /// <summary>
        /// Pour changer la couleur, et la police
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var fontDialog1 = new System.Windows.Forms.FontDialog();
            fontDialog1.ShowColor = true;
            if (fontDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                param._fontName = fontDialog1.Font.Name;
                param._fontSize = fontDialog1.Font.Size;
                param.R = fontDialog1.Color.R;
                param.G = fontDialog1.Color.G;
                param.B = fontDialog1.Color.B;
            }
            save();
            w1.update();
        }

        /// <summary>
        /// Demande a mettre a jour les stats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            param._name = _compte.Text;
            param._tier = _Tier.Text;
            maj();
            // w1.updateText();
        }


        /// <summary>
        /// Recherche les stat de l'user puis sa place sur le ladder (si coché)
        /// </summary>
        private void maj()
        {
            try
            {
                param.strAAfficher = "";
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString("https://pokemonshowdown.com/users/" + param._name + ".json");
                    var rootobject4 = JsonSerializer.Deserialize<Rootobject2>(json);

                    if (param._Elo)
                    {
                        string str = rootobject4.ratings[param._tier].elo.Split('.')[0];
                        param.strAAfficher += str + "   ";
                    }

                    if (param._Gxe)
                    {
                        param.strAAfficher += rootobject4.ratings[param._tier].gxe + "%   ";
                    }

                }
                if (param._Place)
                {
                    using (WebClient wc = new WebClient())
                    {
                        var json = wc.DownloadString("https://pokemonshowdown.com/ladder/" + param._tier + ".json");
                        var rootobject = JsonSerializer.Deserialize<Rootobject>(json);
                        int i = 1;
                        foreach (Toplist top in rootobject.toplist)
                        {

                            if (top.userid == param._name || top.username == param._name)
                            {
                                param.strAAfficher += " N°" + i;

                            }
                            i++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Une erreur est survenue veilley veritier l'id du joueur et le tier");


            }

        }
        /// <summary>
        /// Ferme l'application completment en cliquand sur la croix 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            w1.Close();

            Application.Exit();
        }
    }

    /// <summary>
    /// Classe pour sauvegarder les parametres
    /// </summary>
    public class Param : INotifyPropertyChanged
    {
        /// <summary>
        /// pour le data binding
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string StrAAfficher;
        public string strAAfficher
        {
            get { return StrAAfficher; }
            set
            {
                StrAAfficher = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }


        public string _fontName { get; set; }
        public double _fontSize { get; set; }
        // public System.Drawing.Color _color { get; set; }
        public string _name { get; set; }
        public string _tier { get; set; }
        public List<string> _Listtier { get; set; }
        public bool _Elo { get; set; }
        public bool _Place { get; set; }
        public bool _Gxe { get; set; }
        public bool Timer { get; set; }
    }


    /// <summary>
    /// classes pour lire la reponses pour le top ladder
    /// </summary>
    public class Rootobject
    {
        public string formatid { get; set; }
        public string format { get; set; }
        public Toplist[] toplist { get; set; }
    }

    public class Toplist
    {
        public string userid { get; set; }
        public string username { get; set; }
        public int w { get; set; }
        public int l { get; set; }
        public int t { get; set; }
        public float gxe { get; set; }
        public float r { get; set; }
        public float rd { get; set; }
        public int sigma { get; set; }
        public string rptime { get; set; }
        public float rpr { get; set; }
        public float rprd { get; set; }
        public int rpsigma { get; set; }
        public float elo { get; set; }
    }


    /// <summary>
    /// Classes pour les classement d'un joueur
    /// </summary>
    public class Rootobject2
    {
        public string username { get; set; }
        public string userid { get; set; }
        public int registertime { get; set; }
        public int group { get; set; }
        public Dictionary<string, test> ratings { get; set; }
    }

    public class Ratings
    {
        public Dictionary<string, test> rating { get; set; }
    }

    public class test
    {
        public string elo { get; set; }
        public string gxe { get; set; }
        public string rpr { get; set; }
        public string rprd { get; set; }
    }
}
