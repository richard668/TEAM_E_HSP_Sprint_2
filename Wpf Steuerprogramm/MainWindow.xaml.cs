using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Wpf_Steuerprogramm
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtBox_Gewindelänge.Text = _numValue.ToString();
        }


        Schraube Bolt = new Schraube();

        bool GewindeFeature = true;
        bool GewindeHelix = false;

        #region Berechnen Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double dichte = 0.00785;

            Bolt.Gewindeart = rb_Gewindeart();
            (Bolt.Durchmesser, Bolt.DurchmesserWW_Zoll) = Durchmesserauswahl(Bolt.MetrischeTabelle(), Bolt.WhitworthTabelle(), Bolt.Gewindeart);
            Bolt.Gewindelänge = Convert.ToDouble(txtBox_Gewindelänge.Text);
            Bolt.Schaftlänge = Convert.ToDouble(txtBox_Schaftlänge.Text);
            Bolt.Gesamtlänge = Bolt.Gewindelänge + Bolt.Schaftlänge;
            Bolt.Kopf = rb_Kopf();
            Bolt.Material = rb_Material();
            (Bolt.Streckgrenze, Bolt.Zugfestigkeit) = Festigkeiten(Bolt.Material);
            Bolt.Festigkeitsklasse = Festigkeitsklasse();


            if (Bolt.Gewindeart == 1)
            {
                txtBox_Steigung.Text = Convert.ToString(Konsolenprogramm.BerechnungSteigung(Bolt.MetrischeTabelle(), Bolt.Durchmesser));
                Bolt.Steigung = Convert.ToDouble(txtBox_Steigung.Text);
            }
            if (Bolt.Gewindeart == 2)
            {
                Bolt.Steigung = Convert.ToDouble(txtBox_Steigung.Text);
            }

            if (Bolt.Gewindeart == 3)
            {
                (Bolt.Gangzahl, Bolt.Steigung) = Konsolenprogramm.BerechnungWitworthSteigung(Bolt.WhitworthTabelle(), Bolt.Durchmesser);
                txtBox_Steigung.Text = Convert.ToString(Bolt.Steigung);
                Bolt.Steigung = Convert.ToDouble(txtBox_Steigung.Text);

            }
            Bolt.Flankendurchmesser = Konsolenprogramm.Flankendurchmesser(Bolt.Gewindeart, Bolt.Durchmesser, Bolt.Steigung);

            if (Bolt.Gewindeart != 3)
            {
                Bolt.Schlüsselweite = Konsolenprogramm.AusgabeSchlüsselweite(Bolt.Kopf, Bolt.Durchmesser, Bolt.MetrischeTabelle());

            }
            if (Bolt.Gewindeart == 3)
            {

                Bolt.Schlüsselweite = Konsolenprogramm.AusgabeWW_SechskantSchlüsselweite(Bolt.WhitworthTabelle(), Bolt.Durchmesser, Bolt.Kopf);
                Bolt.DurchmesserWW_Zoll_mm = Konsolenprogramm.AusgabeWitworthdurchmesser_mm(Bolt.WhitworthTabelle(), Bolt.Durchmesser);

            }


            Bolt.Kopfhöhe = Konsolenprogramm.AusgabeKopfhöhe(Bolt.Kopf, Bolt.Durchmesser, Bolt.MetrischeTabelle(), Bolt.WhitworthTabelle(), Bolt.Gewindeart, Bolt.DurchmesserWW_Zoll_mm);
            Bolt.Kopfdurchmesser = Konsolenprogramm.AusgabeKopfdurchmesser(Bolt.Kopf, Bolt.Durchmesser, Bolt.MetrischeTabelle(), Bolt.Gewindeart, Bolt.Schlüsselweite);
            Bolt.Gewindevolumen = Konsolenprogramm.Gewindevolumen(Bolt.Gewindeart, Bolt.Gewindelänge, Bolt.Flankendurchmesser);
            Bolt.Gewindemasse = Konsolenprogramm.Gewindemasse(dichte, Bolt.Gewindevolumen);
            Bolt.Schaftvolumen = Konsolenprogramm.Schaftvolumen(Bolt.Durchmesser, Bolt.Schaftlänge);
            Bolt.Schaftmasse = Konsolenprogramm.Schaftmasse(dichte, Bolt.Schaftvolumen);
            Bolt.Kopfvolumen = Konsolenprogramm.Kopfvolumen(Bolt.Kopf, Bolt.Schlüsselweite, Bolt.Kopfhöhe, Bolt.Kopfdurchmesser, Bolt.Durchmesser);
            Bolt.Kopfmasse = Konsolenprogramm.Kopfmasse(dichte, Bolt.Kopfvolumen);
            Bolt.Gesamtmasse = Konsolenprogramm.Gesamtmasse((double)Bolt.Gewindemasse, Bolt.Schaftmasse, Bolt.Kopfmasse);
            Bolt.Kernlochdurchmesser = Konsolenprogramm.BerechnungKernlochbohrung(Bolt.Durchmesser, Bolt.Steigung, Bolt.MetrischeTabelle(), Bolt.Gewindeart, Bolt.WhitworthTabelle()); ;
            Bolt.Durchgangsbohrung = Konsolenprogramm.BerechnungDurchgangsbohrung(Bolt.MetrischeTabelle(), Bolt.Durchmesser);
            Bolt.Senkdurchmesser = Konsolenprogramm.BerechnungSenkdurchmesser(Bolt.MetrischeTabelle(), Bolt.Durchmesser);
            Bolt.DurchmesserKegelsenkung = Konsolenprogramm.BerechnungDurchmesserKegelsenkung(Bolt.MetrischeTabelle(), Bolt.Durchmesser);
            Bolt.MaxBelastung = Konsolenprogramm.BerechnungMaxBelastung(Bolt.Durchmesser, Bolt.Steigung, Bolt.MetrischeTabelle(), Bolt.Streckgrenze, Bolt.Gewindeart, Bolt.WhitworthTabelle());
            Bolt.WhitworthDurchmesser = Konsolenprogramm.AusgabeWitworthdurchmesser(Bolt.WhitworthTabelle(), Bolt.Durchmesser);
            Bolt.WhitworthFlankendurchmesser = Konsolenprogramm.AusgabeWitworthflankendurchmesser(Bolt.WhitworthTabelle(), Bolt.Durchmesser);
            Bolt.Schraubenrichtung = Gewinderichtung();
            Bolt.SchraubenbezeichnungMX = Konsolenprogramm.SchraubenbezeichnungMX(Bolt.Gewindeart, Bolt.Kopf, Bolt.Durchmesser, Bolt.Gesamtlänge, Bolt.Festigkeitsklasse, Bolt.Schraubenrichtung);
            Bolt.SchraubenbezeichnungMF = Konsolenprogramm.SchraubenbezeichnungMF(Bolt.Gewindeart, Bolt.Kopf, Bolt.Durchmesser, Bolt.Steigung, Bolt.Gesamtlänge, Bolt.Festigkeitsklasse, Bolt.Schraubenrichtung);
            Bolt.Gesamtpreis = Konsolenprogramm.Preisberechnung(Bolt.Gewindeart, Bolt.Kopf, (double)Bolt.Gewindemasse, Bolt.Schaftmasse, Bolt.Kopfmasse);
            Bolt.Flankenwinkel = Konsolenprogramm.Flankenwinkel(Bolt.Gewindeart);
            Bolt.Senktiefe = Konsolenprogramm.BerechnungSenktiefe(Bolt.MetrischeTabelle(), Bolt.Durchmesser);
            Bolt.Schraubenkopfname = Konsolenprogramm.Schraubenkopfname(Bolt.Gewindeart, Bolt.Kopf);
            Bolt.SchraubenbezeichnungWW = Konsolenprogramm.SchraubenbezeichnungWW((string)Bolt.DurchmesserWW_Zoll, Bolt.Gesamtlänge, Bolt.Festigkeitsklasse, Bolt.Schraubenrichtung);


            //Ausgabe Norm
            string Norm = "";
            if (Bolt.Gewindeart == 1)
            {
                Norm = Bolt.SchraubenbezeichnungMX;
            }
            if (Bolt.Gewindeart == 2)
            {
                Norm = Bolt.SchraubenbezeichnungMF;
            }
            if (Bolt.Gewindeart == 3)
            {
                Norm = Bolt.SchraubenbezeichnungWW;
            }

            lbl_Ausgaben.Content = "Ausgaben für " + Norm;


            // Ausgaben abhängig von der Gewindeart
            string Flankendurchmesser = "";
            string Durchgangsbohrung = "";
            string Gangzahl = "";
            string Schlüsselweite = "";
            string Kopfhöhe = "";

            if (Bolt.Gewindeart == 1)
            {
                Flankendurchmesser = "Flankendurchmesser: " + Bolt.Flankendurchmesser + " mm\n";
                Durchgangsbohrung = "Durchgangsbohrung: " + Bolt.Durchgangsbohrung + " mm\n";
                Schlüsselweite = "Schlüsselweite: " + Bolt.Schlüsselweite + "mm\n";
                Kopfhöhe = "Kopfhöhe: " + Bolt.Kopfhöhe + "mm\n";
                lbl_Masse.Visibility = Visibility.Visible;
                lbl_Beschreibung2.Visibility = Visibility.Visible;
            }
            if (Bolt.Gewindeart == 2)
            {
                Flankendurchmesser = "Flankendurchmesser: " + Bolt.Flankendurchmesser + " mm\n";
                Durchgangsbohrung = "Durchgangsbohrung: " + Bolt.Durchgangsbohrung + " mm";
                Schlüsselweite = "Schlüsselweite: " + Bolt.Schlüsselweite + "mm\n";
                Kopfhöhe = "Kopfhöhe: " + Bolt.Kopfhöhe + "mm\n";
                lbl_Masse.Visibility = Visibility.Visible;
                lbl_Beschreibung2.Visibility = Visibility.Visible;
            }
            if (Bolt.Gewindeart == 3)
            {
                Schlüsselweite = "Schlüsselweite: " + Bolt.Schlüsselweite + "mm\n";
                Kopfhöhe = "Kopfhöhe: " + Bolt.Kopfhöhe + "mm\n";
                Flankendurchmesser = "Flankendurchmesser: " + Bolt.WhitworthFlankendurchmesser + " mm\n";
                Gangzahl = "Gangzahl: " + Bolt.Gangzahl + "\n";
                lbl_Masse.Visibility = Visibility.Visible;
                lbl_Beschreibung2.Visibility = Visibility.Visible;
            }


            //Ausgaben abhänging vom Kopf
            string Kopfdurchmesser = "";
            string Senkdurchmesser = "";
            string Senktiefe = "";

            if (Bolt.Kopf == 2) //Zylinder
            {
                Kopfdurchmesser = "Kopfdurchmesser: " + Bolt.Kopfdurchmesser + " mm\n";
                Senkdurchmesser = "Senkdurchmesser: " + Bolt.Senkdurchmesser + " mm\n";
                Senktiefe = "Senktiefe: " + Bolt.Senktiefe + " mm";
            }
            if (Bolt.Kopf == 3) //Senkkopf
            {
                Kopfdurchmesser = "Kopfdurchmesser: " + Bolt.Kopfdurchmesser + " mm\n";
                Senkdurchmesser = "Senkdurchmesser: " + Bolt.DurchmesserKegelsenkung + " mm\n";
            }



            // Ausgabe im Label geometrische Informationen
            lbl_geoInfo_links.Content = Schlüsselweite + Kopfhöhe + Kopfdurchmesser + Gangzahl;
            lbl_geoInfo_rechts.Content = Flankendurchmesser + "Flankenwinkel: " + Bolt.Flankenwinkel + "°\n" + "Steigung: " + Bolt.Steigung;

            // Ausgabe im Label Masse
            lbl_Masse.Content = Bolt.Gesamtmasse + " g\n";

            // Ausgabe im Label mechanische Eigenschaften
            lbl_mechanischeEigenschaften_links.Content = "Re: " + Bolt.Streckgrenze + " MPa\n" + "Rm: " + Bolt.Zugfestigkeit + " MPa\n";
            lbl_mechanischeEigenschaften_rechts.Content = "Maximale Belastung: " + Bolt.MaxBelastung + " kN\n";

            // Ausgabe im Label Senkungen und Bohrungen
            lbl_SenkungUndBohrung_links.Content = "Kernlochdurchmesser: " + Bolt.Kernlochdurchmesser + " mm\n" + Durchgangsbohrung;
            lbl_SenkungUndBohrung_rechts.Content = Senkdurchmesser + Senktiefe;

            //Ausgabe im Label Preis
            lbl_Preis.Content = "Preis: " + Bolt.Gesamtpreis + " € ";


            //CAD Button freischalten
            btn_CAD.IsEnabled = true;

        }

        #endregion Berechnen Button

        #region Eingabemethoden
        //Eingabemethoden

        public string Gewinderichtung()
        {
            string gewinderichtung = "";
            if ((bool)rb_Linksgewinde.IsChecked)
            {
                gewinderichtung = " LH ";
            }

            if ((bool)rb_Rechtsgewinde.IsChecked)
            {
                gewinderichtung = " ";
            }

            return gewinderichtung;
        }

        public int rb_Gewindeart()
        {
            int Gewindeart = 0;
            int de = 0;

            de = cmbBox_Gewindeart.SelectedIndex;
            Gewindeart = de + 1;
            return Gewindeart;
        }

        public int rb_Kopf()
        {
            int Kopf = 0;
            int de = 0;

            de = cmbBox_Kopf.SelectedIndex;
            Kopf = de + 1;
            return Kopf;
        }

        public int rb_Material()
        {
            int Material = 0;
            int de = 0;

            de = cmbBox_Material.SelectedIndex;
            Material = de + 1;
            return Material;
        }
        public string Festigkeitsklasse()
        {
            string Festigkeitsklasse = "";

            if (cmbBox_8_8.IsSelected == true)
            {
                Festigkeitsklasse = "8.8";
            }

            if (cmbBox_10_9.IsSelected == true)
            {
                Festigkeitsklasse = "10.9";
            }

            if (cmbBox_12_9.IsSelected == true)
            {
                Festigkeitsklasse = "12.9";
            }

            if (cmbBox_A4_50.IsSelected == true)
            {
                Festigkeitsklasse = "A4-50";
            }

            return Festigkeitsklasse;
        }

        public (double, double) Festigkeiten(int Material)
        {
            double Streckgrenze = 0;
            double Zugfestigkeit = 0;

            if (Material == 1)
            {
                Streckgrenze = 640;
                Zugfestigkeit = 800;
            }

            if (Material == 2)
            {
                Streckgrenze = 900;
                Zugfestigkeit = 1000;
            }

            if (Material == 3)
            {
                Streckgrenze = 1080;
                Zugfestigkeit = 1200;
            }

            if (Material == 4)
            {
                Streckgrenze = 210;
                Zugfestigkeit = 500;
            }

            return (Streckgrenze, Zugfestigkeit);

        }



        public (double, string) Durchmesserauswahl(double[,] MetrischeTabelle, string[,] WhitworthTabelle, int Gewindeart)
        {
            double Durchmesser = 0;

            int de = 0;
            string Durchmesser_WW_Zoll = "";
            if (Gewindeart == 1 | Gewindeart == 2)
            {
                de = cmbBox_Metrisch.SelectedIndex;
                Durchmesser = MetrischeTabelle[de, 0];
            }

            else
            {
                de = cmbBox_Whitworth.SelectedIndex;
                Durchmesser_WW_Zoll = WhitworthTabelle[de, 0];
                Durchmesser = Double.Parse(WhitworthTabelle[de, 1]);
            }
            return (Durchmesser, Durchmesser_WW_Zoll);
        }
        #endregion Eingabemethoden



        #region Steuerbefehle WPF
        private void cmbBox_MF_Selected(object sender, RoutedEventArgs e)
        {
            txtBox_Steigung.IsEnabled = true;

        }
        private void cmbBox_MF_Unselected(object sender, RoutedEventArgs e)
        {
            txtBox_Steigung.IsEnabled = false;
        }

        private void cmbBox_WW_Selected(object sender, RoutedEventArgs e)
        {
            cmbBox_Whitworth.Visibility = Visibility.Visible;
        }

        private void cmbBox_WW_Unselected(object sender, RoutedEventArgs e)
        {

            cmbBox_Whitworth.Visibility = Visibility.Hidden;

        }

        // numeric_up_down Code startet hier  
        // Für Gewindelänge

        private double _numValue;

        public double NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtBox_Gewindelänge.Text = value.ToString();
            }
        }



        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue += 5; // Wert um 5 erhöht
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue -= 5;  // Wert um 5 erhöht
        }

        private void txtBox_Gewindelänge_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBox_Gewindelänge == null)
            {
                return;
            }

            if (!double.TryParse(txtBox_Gewindelänge.Text, out _numValue))
                txtBox_Gewindelänge.Text = _numValue.ToString();
        }


        // für Schaftlänge 

        private double _numValue_1;

        public double NumValue_1
        {
            get { return _numValue_1; }
            set
            {
                _numValue_1 = value;
                txtBox_Schaftlänge.Text = value.ToString();
            }
        }



        private void cmdUp_Click_1(object sender_1, RoutedEventArgs e)
        {
            NumValue_1 += 5; // Wert um 5 erhöht
        }

        private void cmdDown_Click_1(object sender_1, RoutedEventArgs e)
        {
            NumValue_1 -= 5; // Wert um 5 erhöht
        }

        private void txtBox_Schaftlänge_TextChanged(object sender_1, TextChangedEventArgs e)
        {
            if (txtBox_Schaftlänge == null)
            {
                return;
            }

            if (!double.TryParse(txtBox_Schaftlänge.Text, out _numValue_1))
                txtBox_Schaftlänge.Text = _numValue_1.ToString();
        }
        //numeric_up_down Code endet hier



        private double _numValue_Steigung;

        public double NumValue_Steigung
        {
            get { return _numValue; }
            set
            {
                _numValue_Steigung = value;
                txtBox_Steigung.Text = value.ToString();
            }
        }
        private void txtBox_Steigung_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBox_Steigung == null)
            {
                return;
            }

            if (!double.TryParse(txtBox_Steigung.Text, out _numValue_Steigung))
                txtBox_Steigung.Text = _numValue_Steigung.ToString();
        }



        private void cmbBox_Zylinderkopf_Selected(object sender, RoutedEventArgs e)
        {
            lbl_BildSechskant.Visibility = Visibility.Hidden;
            lbl_BildSenkkopf.Visibility = Visibility.Hidden;
            lbl_BildZylinderkopf.Visibility = Visibility.Visible;
        }

        private void cmbBox_Senkkopf_Selected(object sender, RoutedEventArgs e)
        {
            lbl_BildSechskant.Visibility = Visibility.Hidden;
            lbl_BildSenkkopf.Visibility = Visibility.Visible;
            lbl_BildZylinderkopf.Visibility = Visibility.Hidden;
        }



        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();

        }

        private void chk1_Checked(object sender, RoutedEventArgs e) //Darkmode
        {
            Properties.Settings.Default.ColorMode = "Black";
            // Properties.Settings.Default.Save();
        }

        private void chk1_Unchecked(object sender, RoutedEventArgs e) //Whitemode
        {
            Properties.Settings.Default.ColorMode = "White";
            // Properties.Settings.Default.Save();
        }

        private void cmbBox_Zylinderkopf_Unselected(object sender, RoutedEventArgs e)
        {
            lbl_BildZylinderkopf.Visibility = Visibility.Hidden;
            lbl_BildSenkkopf.Visibility = Visibility.Hidden;
            lbl_BildSechskant.Visibility = Visibility.Visible;
        }

        private void cmbBox_Senkkopf_Unselected(object sender, RoutedEventArgs e)
        {
            lbl_BildZylinderkopf.Visibility = Visibility.Hidden;
            lbl_BildSenkkopf.Visibility = Visibility.Hidden;
            lbl_BildSechskant.Visibility = Visibility.Visible;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            double a = Convert.ToDouble(txtBox_Gewindelänge.Text);


            if (a <= 9)
            {
                txtBox_Gewindelänge.Background = Brushes.Red;

                MessageBox.Show("Gewindelänge muss mindestens 10 mm\nund eine Zahl sein!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBox_Gewindelänge.Text = "10";
            }
            if (a > 9)
            {
                txtBox_Gewindelänge.Background = Brushes.Transparent;
            }
        }

        private void Grid_MouseMove_1(object sender, MouseEventArgs e)
        {
            double a = Convert.ToDouble(txtBox_Schaftlänge.Text);


            if (a < 0)
            {
                txtBox_Schaftlänge.Background = Brushes.Red;

                MessageBox.Show("Schaftlänge darf nicht negativ sein", "Warnung", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBox_Schaftlänge.Text = "0";
            }
            if (a >= 0)
            {
                txtBox_Schaftlänge.Background = Brushes.Transparent;
            }
        }


        private void txtBox_Steigung_MouseMove(object sender, MouseEventArgs e)
        {
            double a = Convert.ToDouble(txtBox_Steigung.Text);

            if (a <= 0)
            {
                txtBox_Steigung.Background = Brushes.Red;

                MessageBox.Show("Steigung muss größer als 0 sein!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBox_Steigung.Text = "1";
            }
            if (a > 0)
            {
                txtBox_Steigung.Background = Brushes.Transparent;
            }
        }

        public void GewindefeatureIsChecked(object sender, RoutedEventArgs e)
        {
            GewindeFeature = true;
            GewindeHelix = false;
        }

        public void GewindeHelixIsChecked(object sender, RoutedEventArgs e)
        {
            GewindeFeature = false;
            GewindeHelix = true;
        }
        #endregion Steuerbefehle WPF

        private void btn_CAD_Click(object sender, RoutedEventArgs e)
        {

            object[] Parameter = new object[12];
            Parameter[0] = Bolt.Kopf; //int
            Parameter[1] = Bolt.Durchmesser; //double
            Parameter[2] = Bolt.Gewindelänge; //double
            Parameter[3] = Bolt.Schaftlänge; //double
            Parameter[4] = Bolt.Steigung; //double
            Parameter[5] = Bolt.Gewindeart; //int
            Parameter[6] = Bolt.Schlüsselweite; //double
            Parameter[7] = Bolt.Kopfhöhe; //double
            Parameter[8] = Bolt.Kopfdurchmesser; //double
            Parameter[9] = Bolt.Schraubenrichtung; //string
            Parameter[10] = GewindeFeature;
            Parameter[11] = GewindeHelix;

            new CAD(Parameter);
        }

       
    }






    #region Konsolenprogramm

    //Berechnungmethoden

    class Konsolenprogramm
    {

        static public string SchraubenbezeichnungMX(int Gewindeart, int Kopf, double Durchmesser, double Gesamtlänge, string Festigkeitsklasse, string Schraubenrichtung)
        {
            string schraubenbezeichnung = "";
            string gewindetyp = "";
            string bezugsnorm = "";
            string schraubenname = "";


            if (Kopf == 1 & Gewindeart == 1)//Sechskant Regelgewinde
            {
                schraubenname = "Sechskantschraube ";
                bezugsnorm = "DIN EN ISO 4014 ";
                gewindetyp = "M";
            }

            if (Kopf == 2 & Gewindeart == 1)//Zylinder Regelgewinde
            {
                schraubenname = "Zylinderschraube ";
                bezugsnorm = "DIN EN ISO 4762 ";
                gewindetyp = "M";
            }

            if (Kopf == 3 & Gewindeart == 1)//Senkschraube Regelgewinde
            {
                schraubenname = "Senkschraube ";
                bezugsnorm = "DIN EN ISO 10642 ";
                gewindetyp = "M";

            }

            schraubenbezeichnung = schraubenname + bezugsnorm + "- " + gewindetyp + Durchmesser + Schraubenrichtung + "x " + Gesamtlänge + " - " + Festigkeitsklasse;
            return schraubenbezeichnung;
        }
        static public string SchraubenbezeichnungMF(int Gewindeart, int Kopf, double Durchmesser, double Steigung, double Gesamtlänge, string Festigkeitsklasse, string Schraubenrichtung)
        {
            string schraubenbezeichnung = "";
            string gewindetyp = "";
            string bezugsnorm = "";
            string schraubenname = "";


            if (Kopf == 1 & Gewindeart == 2)//Sechskant Feingewinde
            {
                schraubenname = "Sechskantschraube ";
                bezugsnorm = "DIN EN ISO 8765 ";
                gewindetyp = "M";

            }


            if (Kopf == 2 & Gewindeart == 2)//Zylinder Feingewinde
            {
                schraubenname = "Zylinderschraube ";
                bezugsnorm = "DIN 34821 ";
                gewindetyp = "M";
            }

            if (Kopf == 3 & Gewindeart == 2)//Senkschraube Feingewinde
            {
                schraubenname = "Senkschraube ";
                gewindetyp = "M";

            }

            schraubenbezeichnung = schraubenname + bezugsnorm + "- " + gewindetyp + Durchmesser + Schraubenrichtung + "x " + Steigung + " x " + Gesamtlänge + " - " + Festigkeitsklasse;
            return schraubenbezeichnung;
        }

        static public string SchraubenbezeichnungWW(string DurchmesserWW_Zoll, double Gesamtlänge, string Festigkeitsklasse, string Schraubenrichtung)
        {
            string schraubenbezeichnung = "";


            string schraubenname = "Whitworth-Gewinde ";



            schraubenbezeichnung = schraubenname + "G " + DurchmesserWW_Zoll + "''" + Schraubenrichtung + "x " + Gesamtlänge + " - " + Festigkeitsklasse;
            return schraubenbezeichnung;
        }

        static public string Schraubenkopfname(int Gewindeart, int Kopf)
        {
            string schraubenkopfname = "";

            if (Kopf == 1 && Gewindeart != 3)
            {
                schraubenkopfname = "Sechskant";
            }

            if (Kopf == 2 && Gewindeart != 3)
            {
                schraubenkopfname = "Zylinderkopf";
            }

            if (Kopf == 3 && Gewindeart != 3)
            {
                schraubenkopfname = "Senkkopf";
            }

            if (Gewindeart == 3)
            {
                schraubenkopfname = "-/-";
            }

            return schraubenkopfname;
        }

        static public double Flankendurchmesser(int Gewindeart, double Durchmesser, double Steigung)
        {
            double flankendurchmesser = 0;

            if (Gewindeart == 1)
            {
                flankendurchmesser = Durchmesser - (0.6495 * (BerechnungSteigung(Tabellen(), Durchmesser)));
            }

            if (Gewindeart == 2)
            {
                flankendurchmesser = Durchmesser - (0.6495 * Steigung);
            }

            if (Gewindeart == 3)
            {
                string flankendurchmessre = AusgabeWitworthflankendurchmesser(WitworthTabelle(), Durchmesser);
                flankendurchmesser = Convert.ToDouble(flankendurchmessre);
            }

            return Math.Round(flankendurchmesser, 2);
        }
        static public double Flankenwinkel(int Gewindeart)
        {
            double flankenwinkel = 0;

            if (Gewindeart == 1)
            {
                flankenwinkel = 60;
            }
            if (Gewindeart == 2)
            {
                flankenwinkel = 60;
            }
            if (Gewindeart == 3)
            {
                flankenwinkel = 55;
            }

            return flankenwinkel;
        }

        static public double Gewindevolumen(int Gewindeart, double laengenausgabegewinde, double flankendurchmesser)
        {
            double gewindevolumen = 0;

            if (Gewindeart == 1)
            {
                gewindevolumen = (Math.PI / 4) * (Math.Pow(flankendurchmesser, 2)) * laengenausgabegewinde;
                //Console.WriteLine("Flankendurchmesser: " + Math.Round(flankendurchmessre, 2) + " mm");
            }

            if (Gewindeart == 2)
            {
                gewindevolumen = (Math.PI / 4) * (Math.Pow(flankendurchmesser, 2)) * laengenausgabegewinde;
                //Console.WriteLine("Flankendurchmesser: " + Math.Round(flankendurchmessre, 2) + " mm");
            }

            if (Gewindeart == 3)
            {
                //double flankendurchmessre = durchmessereingabe - (0.64 * WitworthSteigung);
                gewindevolumen = (Math.PI / 4) * (Math.Pow(flankendurchmesser, 2)) * laengenausgabegewinde;
                //Console.WriteLine("Flankendurchmesser: "+Math.Round(flankendurchemesser,2)+" mm");
            }

            return Math.Round(gewindevolumen, 2);
        }
        static public double Gewindemasse(double dichte, double gewindevolumen)
        {
            double gewindemasse = gewindevolumen * dichte;
            return Math.Round(gewindemasse, 2);
        }


        static public double Schaftvolumen(double Durchmesser, double laengenausgabeschaft)
        {
            double schaftvolumen = (Math.PI / 4) * (Math.Pow(Durchmesser, 2)) * laengenausgabeschaft;
            return Math.Round(schaftvolumen, 2);
        }
        static public double Schaftmasse(double dichte, double schaftvolumen)
        {
            double schaftmasse = schaftvolumen * dichte;
            return Math.Round(schaftmasse, 2);
        }


        static public double Kopfvolumen(int Kopf, double Schlüsselweite, double Kopfhöhe, double kopfdurchmesser, double Durchmesser)
        {
            double kopfvolumen = 0;

            if (Kopf == 1) //Sechskant
            {
                kopfvolumen = Kopfhöhe * 3 / 2 * Schlüsselweite / 2 / 0.8660254038 * Schlüsselweite;
            }

            if (Kopf == 2) //Zylinder
            {
                kopfvolumen = Kopfhöhe * Math.PI / 4 * kopfdurchmesser * kopfdurchmesser;
            }

            if (Kopf == 3) //Senkkopf
            {
                kopfvolumen = Math.PI * Kopfhöhe / 12 * (kopfdurchmesser * kopfdurchmesser + Durchmesser * Durchmesser + kopfdurchmesser * Durchmesser);
            }

            return Math.Round(kopfvolumen, 2);
        }
        static public double Kopfmasse(double dichte, double kopfvolumen)
        {
            double kopfmasse = kopfvolumen * dichte;
            return Math.Round(kopfmasse, 2);
        }

        static public double Gesamtmasse(double gewindemasse, double schaftmasse, double kopfmasse)
        {
            double gesamtmasse = gewindemasse + schaftmasse + kopfmasse;
            return Math.Round(gesamtmasse, 2);
        }


        static public double Preisberechnung(int Gewindeart, int Kopf, double gewindemasse, double schaftmasse, double kopfmasse)
        {
            double gesamtpreis = 0;
            double preisMX = 0.08; // z.B. 0,08 €/g
            double preisMF = 0.12; // z.B. 0,12€/g
            double preisWW = 0.4; // z.B. 0,1€/g
            double gewindepreis = 0;
            double schaftpreis = 0;
            double kopfpreis = 0;
            double preisSechskant = 0.5;
            double preisZylinder = 0.2;
            double preisSenkkopf = 0.6;

            if (Gewindeart == 1) //Regelgewinde
            {
                gewindepreis = preisMX * gewindemasse;
                schaftpreis = preisMX * schaftmasse;
            }

            if (Gewindeart == 2) //Feingewinde
            {
                gewindepreis = preisMF * gewindemasse;
                schaftpreis = preisMF * schaftmasse;
            }


            if (Gewindeart == 3) //Whitworth
            {
                gewindepreis = preisWW * gewindemasse;
                schaftpreis = preisWW * schaftmasse;
            }

            if (Kopf == 1) //Sechskant
            {
                kopfpreis = kopfmasse * preisSechskant;
            }

            if (Kopf == 2) //Zylinder
            {
                kopfpreis = kopfmasse * preisZylinder;
            }

            if (Kopf == 3) //Senkkopf
            {
                kopfpreis = kopfmasse * preisSenkkopf;
            }

            gesamtpreis = gewindepreis + schaftpreis + kopfpreis;

            return Math.Round(gesamtpreis, 2);
        }






















        static public double AusgabeSchlüsselweite(int Kopf, double Durchmesser, double[,] Tabelle)
        {
            double Schlüsselweite = 0;
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln
            if (Kopf == 1)
            {
                for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        Schlüsselweite = Tabelle[jj, 7]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                    }
                }
            }

            if (Kopf == 2)
            {
                for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        Schlüsselweite = Tabelle[jj, 9]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                    }
                }
            }

            if (Kopf == 3)
            {
                for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        Schlüsselweite = Tabelle[jj, 11]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                    }
                }
            }

            return Schlüsselweite;
        }

        static public double AusgabeKopfdurchmesser(int Kopf, double Durchmesser, double[,] Tabelle, int Gewindeart, double Schlüsselweite)
        {
            double kopfdurchmesser = 0;
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln


            if (Gewindeart != 3)
            {
                if (Kopf == 2)//Zylinder
                {
                    for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                    {
                        M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                        if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                        {
                            kopfdurchmesser = Tabelle[jj, 8]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                        }
                    }
                }

                if (Kopf == 3) //Senkkopf
                {
                    for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                    {
                        M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                        if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                        {
                            kopfdurchmesser = Tabelle[jj, 12]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                        }
                    }
                }
            }

            if (Gewindeart == 3)
            {
                if (Kopf == 2)
                {
                    kopfdurchmesser = 2 * Schlüsselweite;
                }

                if (Kopf == 3)
                {
                    kopfdurchmesser = 1.92 * Durchmesser;
                }
            }

            return kopfdurchmesser;
        }

        static public double AusgabeKopfhöhe(int Kopf, double Durchmesser, double[,] Tabelle, string[,] Witworth, int Gewindeart, double DurchmesserWW_Zoll_Double)
        {
            double Kopfhöhe = 0;
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln

            if (Gewindeart != 3)
            {
                if (Kopf == 1) //Sechskantkopf
                {
                    for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                    {
                        M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                        if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                        {
                            Kopfhöhe = Tabelle[jj, 6]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                        }
                    }
                }

                if (Kopf == 2) //Zylinderkopf
                {
                    for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                    {
                        M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                        if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                        {
                            Kopfhöhe = Tabelle[jj, 0]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                        }
                    }
                }

                if (Kopf == 3) //Senkkopf
                {
                    for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                    {
                        M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                        if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                        {
                            Kopfhöhe = Tabelle[jj, 10]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                        }
                    }
                }
            }

            if (Gewindeart == 3)
            {
                if (Kopf == 1)
                {
                    Kopfhöhe = DurchmesserWW_Zoll_Double * 2 / 3;
                }

                if (Kopf == 2)
                {
                    Kopfhöhe = DurchmesserWW_Zoll_Double;
                }

                if (Kopf == 3)
                {
                    Kopfhöhe = DurchmesserWW_Zoll_Double * 0.62;
                }
            }

            return Math.Round(Kopfhöhe, 2);
        }



        static public double BerechnungDurchgangsbohrung(double[,] Tabelle, double Durchmesser)
        {

            //Duchgangbohrung Durchmesser
            double Durchgangsbohrung = 0; // Wert der am Ende ausgegeben werden soll
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln
            for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    Durchgangsbohrung = Tabelle[jj, 1]; // Wert aus der Tabelle wird Durchgangsbohrung übergeben     
                }
            }
            return Durchgangsbohrung;

            // Wenn keine Übereinstimmung gefunden wurde sollte noch eine Meldung ausgegeben werden  
        }

        static public double BerechnungSteigung(double[,] Tabelle, double Durchmesser) // 
        {

            //Duchgangbohrung Durchmesser
            double Steigung = 0; // Wert der am Ende ausgegeben werden soll
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln
            for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    Steigung = Tabelle[jj, 5]; // Wert aus der Tabelle wird übergeben     
                }
            }

            return Steigung;

            // Wenn keine Übereinstimmung gefunden wurde sollte noch eine Meldung ausgegeben werden  
        }

        static public double BerechnungSenkdurchmesser(double[,] Tabelle, double Durchmesser) // 
        {

            //Duchgangbohrung Durchmesser
            double Senkdurchmesser = 0; // Wert der am Ende ausgegeben werden soll
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln
            for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    Senkdurchmesser = Tabelle[jj, 2]; // Wert aus der Tabelle wird übergeben
                }
            }
            return Senkdurchmesser;

            // Wenn keine Übereinstimmung gefunden wurde sollte noch eine Meldung ausgegeben werden  
        }

        static public double BerechnungSenktiefe(double[,] Tabelle, double Durchmesser) // 
        {

            //Duchgangbohrung Durchmesser
            double Senktiefe = 0; // Wert der am Ende ausgegeben werden soll
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln
            for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    Senktiefe = Tabelle[jj, 3]; // Wert aus der Tabelle wird übergeben
                }
            }
            return Senktiefe;

            // Wenn keine Übereinstimmung gefunden wurde sollte noch eine Meldung ausgegeben werden  
        }

        static public double BerechnungDurchmesserKegelsenkung(double[,] Tabelle, double Durchmesser) // 
        {

            //Duchgangbohrung Durchmesser
            double DurchmesserKegelsenkung = 0; // Wert der am Ende ausgegeben werden soll
            int jj = 0; // Variable die zum hochzählen verwendet werden soll
            int M = 0; // double der in der Tabelle steht in einen int umwandeln
            for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                M = Convert.ToInt32(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    DurchmesserKegelsenkung = Tabelle[jj, 4]; // Wert aus der Tabelle wird übergeben
                }
            }
            return DurchmesserKegelsenkung;

            // Wenn keine Übereinstimmung gefunden wurde sollte noch eine Meldung ausgegeben werden  
        }


        static public double BerechnungKernlochbohrung(double Durchmesser, double Steigung, double[,] Tabelle, int Gewindeauswahl, string[,] Witworth)
        {
            int jj = 0;
            double M = 0;
            double Kerndurchmesser = 0;

            //Für metrische Gewinde
            if (Gewindeauswahl == 1 | Gewindeauswahl == 2)
            {
                if (Steigung == 0)
                {
                    for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                    {
                        M = Convert.ToDouble(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                        if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                        {
                            Steigung = Tabelle[jj, 5]; // Wert aus der Tabelle wird übergeben
                        }
                    }
                }

                Kerndurchmesser = (Durchmesser - Steigung);
            }

            //Für Whitworth Gewinde
            else
            {
                for (jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in int
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        Kerndurchmesser = Convert.ToDouble(Witworth[jj, 4]); // Wert aus der Tabelle wird übergeben
                    }
                }
            }
            return Math.Round(Kerndurchmesser, 2);
        }


        static public double BerechnungMaxBelastung(double Durchmesser, double Steigung, double[,] Tabelle, double Streckgrenze, int Gewindeart, string[,] Witworth)
        {
            int jj = 0;
            double M = 0;
            double MaxBelastung = 0;
            if (Gewindeart == 1)
            {

                for (jj = 0; jj <= 8; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    M = Convert.ToDouble(Tabelle[jj, 0]); //umwandeln der Strings in der Tabelle in int
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        Steigung = Tabelle[jj, 5]; // Wert aus der Tabelle wird übergeben
                    }
                }
                MaxBelastung = (Math.Pow((((Durchmesser - 0.6495 * Steigung) + (Durchmesser - 1.2269 * Steigung)) / 2), 2)) * Math.PI * 0.25 * Streckgrenze;

            }



            if (Gewindeart == 2)
            {
                MaxBelastung = (Math.Pow((((Durchmesser - 0.6495 * Steigung) + (Durchmesser - 1.2269 * Steigung)) / 2), 2)) * Math.PI * 0.25 * Streckgrenze;
            }

            if (Gewindeart == 3)
            {
                double Spannungsquerschnitt = 0;

                for (jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in int
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        Spannungsquerschnitt = Convert.ToDouble(Witworth[jj, 3]);
                    }
                }

                MaxBelastung = Streckgrenze * Spannungsquerschnitt;
            }

            return Math.Round(MaxBelastung / 1000, 2);
        }




        //Steigung des Witworth Gewindes als Gangzahl und in mm
        static public (double, double) BerechnungWitworthSteigung(string[,] Witworth, double Durchmesser)
        {
            double Gangzahl = 0;

            for (int jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                double M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in double
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    Gangzahl = Convert.ToDouble(Witworth[jj, 2]); // Wert aus der Tabelle wird Gangzahl übergeben
                }
            }

            double Steigung = Math.Round(25.4 / Gangzahl, 2);

            return (Gangzahl, Steigung);
        }

        static public string AusgabeWitworthdurchmesser(string[,] Witworth, double Durchmesser)
        {
            string durchmesserWW = "0";

            for (int jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                double M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in double
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    durchmesserWW = Witworth[jj, 0]; // Wert aus der Tabelle wird Gangzahl übergeben
                }
            }


            return durchmesserWW;
        }

        static public double AusgabeWitworthdurchmesser_mm(string[,] Witworth, double Durchmesser)
        {
            string durchmesserWW_mm = "0";

            for (int jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                double M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in double
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    durchmesserWW_mm = Witworth[jj, 1]; // Wert aus der Tabelle wird Gangzahl übergeben
                }
            }

            double durchmesserWW_mm_Doubble = Convert.ToDouble(durchmesserWW_mm);

            return Math.Round(durchmesserWW_mm_Doubble, 2);
        }

        static public string AusgabeWitworthflankendurchmesser(string[,] Witworth, double Durchmesser)
        {
            string durchmesserWW = "0";

            for (int jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
            {
                double M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in double
                if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                {
                    durchmesserWW = Witworth[jj, 5]; // Wert aus der Tabelle wird Gangzahl übergeben
                }
            }


            return durchmesserWW;
        }

        static public double AusgabeWW_SechskantSchlüsselweite(string[,] Witworth, double Durchmesser, int Kopf)
        {
            string WW_SW = "0";


            if (Kopf == 1)
            {
                for (int jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    double M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in double
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        WW_SW = Witworth[jj, 6]; // Wert aus der Tabelle wird Gangzahl übergeben
                    }
                }
            }

            if (Kopf == 2)
            {
                for (int jj = 0; jj <= 7; jj++) // durchsuchen der Tabelle nach dem richtigen Durchmesser
                {
                    double M = Convert.ToDouble(Witworth[jj, 1]); //umwandeln der Strings in der Tabelle in double
                    if (Durchmesser == M) // Vergleich ob in dem Tabellenfeld der gleiche Wert steht wie in der Eingabe
                    {
                        WW_SW = Witworth[jj, 7]; // Wert aus der Tabelle wird Gangzahl übergeben
                    }
                }
            }

            double WW_SW_Double = Convert.ToDouble(WW_SW);

            if (Kopf == 3)
            {
                WW_SW_Double = Durchmesser * 2 / 3;
            }



            return Math.Round(WW_SW_Double, 2);
        }


        static public double[,] Tabellen() // Funktion die ein Array zurückgibt
        {
            // die Werte können nicht mit Formeln errechnet werden, sondern sind auf diese Tabellenwerte genormt
            // deswegen haben wir die als Tabelle hinterlegt um sie bei den Berechnungen bzw. Ausgaben zu verwenden

            double[,] Tabelle = new double[9, 13];

            //Gewinde Nenndurchmesser
            Tabelle[0, 0] = 3;
            Tabelle[1, 0] = 4;
            Tabelle[2, 0] = 5;
            Tabelle[3, 0] = 6;
            Tabelle[4, 0] = 8;
            Tabelle[5, 0] = 10;
            Tabelle[6, 0] = 12;
            Tabelle[7, 0] = 16;
            Tabelle[8, 0] = 20;

            //Durchgangsloch Durchmesser
            Tabelle[0, 1] = 3.4;
            Tabelle[1, 1] = 4.5;
            Tabelle[2, 1] = 5.5;
            Tabelle[3, 1] = 6.6;
            Tabelle[4, 1] = 9;
            Tabelle[5, 1] = 11;
            Tabelle[6, 1] = 13.5;
            Tabelle[7, 1] = 17.5;
            Tabelle[8, 1] = 22;

            //ISO 4762 Senkdurchmesser Zylinderkopfschraube
            Tabelle[0, 2] = 6.5;
            Tabelle[1, 2] = 8;
            Tabelle[2, 2] = 10;
            Tabelle[3, 2] = 11;
            Tabelle[4, 2] = 15;
            Tabelle[5, 2] = 18;
            Tabelle[6, 2] = 20;
            Tabelle[7, 2] = 26;
            Tabelle[8, 2] = 33;

            //ISO 4762 Senktiefe Zylinderkopfschraube
            Tabelle[0, 3] = 3.4;
            Tabelle[1, 3] = 4.4;
            Tabelle[2, 3] = 5.4;
            Tabelle[3, 3] = 6.4;
            Tabelle[4, 3] = 8.6;
            Tabelle[5, 3] = 10.6;
            Tabelle[6, 3] = 12.6;
            Tabelle[7, 3] = 16.6;
            Tabelle[8, 3] = 20.6;

            // Durchmesser Kegelsenkung
            Tabelle[0, 4] = 6.9;
            Tabelle[1, 4] = 9.2;
            Tabelle[2, 4] = 11.5;
            Tabelle[3, 4] = 13.7;
            Tabelle[4, 4] = 18.3;
            Tabelle[5, 4] = 22.7;
            Tabelle[6, 4] = 27.2;
            Tabelle[7, 4] = 34;
            Tabelle[8, 4] = 40.7;

            //Steigung metrisches Regelgewinde
            Tabelle[0, 5] = 0.5;
            Tabelle[1, 5] = 0.7;
            Tabelle[2, 5] = 0.8;
            Tabelle[3, 5] = 1;
            Tabelle[4, 5] = 1.25;
            Tabelle[5, 5] = 1.5;
            Tabelle[6, 5] = 1.75;
            Tabelle[7, 5] = 2;
            Tabelle[8, 5] = 2.5;

            //Kopfhöhe Sechskantschraube
            Tabelle[0, 6] = 2;
            Tabelle[1, 6] = 2.8;
            Tabelle[2, 6] = 3.5;
            Tabelle[3, 6] = 4;
            Tabelle[4, 6] = 5.3;
            Tabelle[5, 6] = 6.4;
            Tabelle[6, 6] = 7.5;
            Tabelle[7, 6] = 10;
            Tabelle[8, 6] = 12.5;

            //Schlüsselweite Sechskantschraube
            Tabelle[0, 7] = 5.5;
            Tabelle[1, 7] = 7;
            Tabelle[2, 7] = 8;
            Tabelle[3, 7] = 10;
            Tabelle[4, 7] = 13;
            Tabelle[5, 7] = 16;
            Tabelle[6, 7] = 18;
            Tabelle[7, 7] = 24;
            Tabelle[8, 7] = 30;

            //Kopfdurchmesser Zylinderkopfschraube
            Tabelle[0, 8] = 5.5;
            Tabelle[1, 8] = 7;
            Tabelle[2, 8] = 8.5;
            Tabelle[3, 8] = 10;
            Tabelle[4, 8] = 13;
            Tabelle[5, 8] = 16;
            Tabelle[6, 8] = 18;
            Tabelle[7, 8] = 24;
            Tabelle[8, 8] = 30;

            //Kopfhöhe Zylinderkopfschraube = Nenndurchmesser

            //Schlüsselweite des Innensechskants bei Zylinderkopfschrauben
            Tabelle[0, 9] = 2.5;
            Tabelle[1, 9] = 3;
            Tabelle[2, 9] = 4;
            Tabelle[3, 9] = 5;
            Tabelle[4, 9] = 6;
            Tabelle[5, 9] = 8;
            Tabelle[6, 9] = 10;
            Tabelle[7, 9] = 14;
            Tabelle[8, 9] = 17;

            //Kopfhöhe Senkschrauben
            Tabelle[0, 10] = 1.9;
            Tabelle[1, 10] = 2.5;
            Tabelle[2, 10] = 3.1;
            Tabelle[3, 10] = 3.7;
            Tabelle[4, 10] = 5;
            Tabelle[5, 10] = 6.2;
            Tabelle[6, 10] = 7.4;
            Tabelle[7, 10] = 8.8;
            Tabelle[8, 10] = 10.2;

            //Schlüsselweite des Innensechskants bei Senkschrauben
            Tabelle[0, 11] = 2;
            Tabelle[1, 11] = 2.5;
            Tabelle[2, 11] = 3;
            Tabelle[3, 11] = 4;
            Tabelle[4, 11] = 5;
            Tabelle[5, 11] = 6;
            Tabelle[6, 11] = 8;
            Tabelle[7, 11] = 10;
            Tabelle[8, 11] = 12;

            //Kopfdurchmesser bei Senkschrauben
            Tabelle[0, 12] = 5.5;
            Tabelle[1, 12] = 7.5;
            Tabelle[2, 12] = 9.4;
            Tabelle[3, 12] = 11.3;
            Tabelle[4, 12] = 15.2;
            Tabelle[5, 12] = 19.2;
            Tabelle[6, 12] = 23.1;
            Tabelle[7, 12] = 29;
            Tabelle[8, 12] = 36;

            return Tabelle;
        }


        static public string[,] WitworthTabelle() // Funktion die ein Array zurückgibt
        {
            // die Werte können nicht mit Formeln errechnet werden, sondern sind auf diese Tabellenwerte genormt
            // deswegen haben wir die als Tabelle hinterlegt um sie bei den Berechnungen bzw. Ausgaben zu verwenden

            string[,] Witworth = new string[8, 8];

            //Gewinde Nenndurchmesser
            Witworth[0, 0] = "1/4";
            Witworth[1, 0] = "3/8";
            Witworth[2, 0] = "1/2";
            Witworth[3, 0] = "3/4";
            Witworth[4, 0] = "1";
            Witworth[5, 0] = "1 1/4";
            Witworth[6, 0] = "1 1/2";
            Witworth[7, 0] = "2";

            //Außendurchmesser
            Witworth[0, 1] = "6,35";
            Witworth[1, 1] = "9,53";
            Witworth[2, 1] = "12,7";
            Witworth[3, 1] = "19,05";
            Witworth[4, 1] = "25,4";
            Witworth[5, 1] = "31,75";
            Witworth[6, 1] = "38,1";
            Witworth[7, 1] = "50,8";

            //Gangzahl
            Witworth[0, 2] = "20";
            Witworth[1, 2] = "16";
            Witworth[2, 2] = "12";
            Witworth[3, 2] = "10";
            Witworth[4, 2] = "8";
            Witworth[5, 2] = "7";
            Witworth[6, 2] = "6";
            Witworth[7, 2] = "4,5";

            //Spannungsquerschnitt
            Witworth[0, 3] = "17,5";
            Witworth[1, 3] = "44,1";
            Witworth[2, 3] = "78,4";
            Witworth[3, 3] = "196";
            Witworth[4, 3] = "358";
            Witworth[5, 3] = "577";
            Witworth[6, 3] = "839";
            Witworth[7, 3] = "1491";

            //Kernlochdurchmesser
            Witworth[0, 4] = "4,72";
            Witworth[1, 4] = "7,49";
            Witworth[2, 4] = "9,99";
            Witworth[3, 4] = "15,8";
            Witworth[4, 4] = "21,34";
            Witworth[5, 4] = "27,10";
            Witworth[6, 4] = "32,68";
            Witworth[7, 4] = "43,57";

            //Flankendurchmesser
            Witworth[0, 5] = "5,54";
            Witworth[1, 5] = "8,51";
            Witworth[2, 5] = "11,35";
            Witworth[3, 5] = "17,42";
            Witworth[4, 5] = "23,37";
            Witworth[5, 5] = "29,43";
            Witworth[6, 5] = "35,39";
            Witworth[7, 5] = "47,19";

            //Sechskant Schlüsselweite
            Witworth[0, 6] = "11,113";
            Witworth[1, 6] = "14,288";
            Witworth[2, 6] = "19,050";
            Witworth[3, 6] = "28,575";
            Witworth[4, 6] = "38,100";
            Witworth[5, 6] = "47,625";
            Witworth[6, 6] = "57,150";
            Witworth[7, 6] = "76,200";

            //Zylinderkopf Schlüsselweite
            Witworth[0, 7] = "4,775";
            Witworth[1, 7] = "7,938";
            Witworth[2, 7] = "9,525";
            Witworth[3, 7] = "15,875";
            Witworth[4, 7] = "19,050";
            Witworth[5, 7] = "22,225";
            Witworth[6, 7] = "25,400";
            Witworth[7, 7] = "38,100";

            return Witworth;

        }
    }


    #endregion

    #region SchraubenKlasse
    class Schraube
    {
        public double Durchmesser { get; set; }
        public double Gewindelänge { get; set; }
        public double Schaftlänge { get; set; }
        public double Steigung { get; set; }
        public int Kopf { get; set; }
        public int Gewindeart { get; set; }
        public int Material { get; set; }
        public double Streckgrenze { get; set; }
        public double Zugfestigkeit { get; set; }
        public double Gangzahl { get; set; }
        public double Gesamtlänge { get; set; }
        public string Festigkeitsklasse { get; set; }
        public object Gewindemasse { get; set; }
        public double Flankendurchmesser { get; set; }
        public double Gewindevolumen { get; set; }
        public double Schaftvolumen { get; set; }
        public double Schaftmasse { get; set; }
        public double Schlüsselweite { get; set; }
        public double Kopfhöhe { get; set; }
        public double Kopfdurchmesser { get; set; }
        public double Kopfvolumen { get; set; }
        public double Kopfmasse { get; set; }
        public double Gesamtmasse { get; set; }
        public double Kernlochdurchmesser { get; set; }
        public double Durchgangsbohrung { get; set; }
        public double Senkdurchmesser { get; set; }
        public double DurchmesserKegelsenkung { get; set; }
        public double MaxBelastung { get; set; }
        public string WhitworthDurchmesser { get; set; }
        public string WhitworthFlankendurchmesser { get; set; }
        public string SchraubenbezeichnungMX { get; set; }
        public string SchraubenbezeichnungMF { get; set; }
        public double Gesamtpreis { get; set; }
        public double Flankenwinkel { get; set; }
        public double Senktiefe { get; set; }
        public string Schraubenkopfname { get; set; }
        public string Schraubenrichtung { get; set; }
        public string SchraubenbezeichnungWW { get; set; }
        public object DurchmesserWW_Zoll { get; set; }
        public double DurchmesserWW_Zoll_mm { get; internal set; }

        public double[,] MetrischeTabelle()
        {
            return Konsolenprogramm.Tabellen();
        }

        public string[,] WhitworthTabelle()
        {
            return Konsolenprogramm.WitworthTabelle();
        }


    }
    #endregion

}
