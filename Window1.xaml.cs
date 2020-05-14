using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Media;

namespace showdow
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Param param;
        public Window1(Param p)
        {
            InitializeComponent();
          
            //fenetre toujours active
            this.Topmost = true;
            param = p;
            //databinding relou
            this.DataContext = param;
        }

        /// <summary>
        /// update des preferences sur la police
        /// </summary>
        public void update()
        {

            labe.FontFamily = new System.Windows.Media.FontFamily(param._fontName);
            labe.FontSize = param._fontSize;
            labe.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(param.R, param.G, param.B));

        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        /// <summary>
        /// Pour permettre de deplacer la fenetre en glissant sur le texte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}


