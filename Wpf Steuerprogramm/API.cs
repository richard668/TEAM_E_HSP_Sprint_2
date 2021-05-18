using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using INFITF;
using MECMOD;
using PARTITF;

namespace Wpf_Steuerprogramm
{
    class CAD
    {
        INFITF.Application hsp_catiaApp;
        MECMOD.PartDocument hsp_catiaPart;
        MECMOD.Sketch hsp_catiaProfil;
        MECMOD.Sketch hsp_catiaProfil2;
       
        public CAD(object [] ParameterListe)
        {
          
            //Hier wird das gesamte Modell erstellt, wenn nichts schief geht
            try
            {

                //Abfrage ob Catia geöffnet ist
                if(CatiaLaeuft())
                {

                    //Erstellt ein neues Part
                    INFITF.Documents catDocuments1 = hsp_catiaApp.Documents;
                    hsp_catiaPart = catDocuments1.Add("Part") as MECMOD.PartDocument;

                    //Erstellt eine Skizze für den Kopf
                    ErstelleLeereSkizze();

                    //Erstellt ein Profil vom Kopf
                    ProfilKopf(ParameterListe);

                    //Erstellt einen Block für den Kopf
                    BlockErstellen(ParameterListe);

                    //Erstellt eine Skizze für den Schaft
                    //ErstelleLeereSkizze();

                    //Erstellt ein Profil für den Schaft
                    //ProfilSchaft(ParameterListe);

                    //Erstellt ein Block für den Schaft


                    //Erstellt Gewinde am Schaft



                }

                else
                {
                    MessageBox.Show("Laufende Catia Application nicht gefunden");        
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception aufgetreten");
            }


        }


        private bool CatiaLaeuft()
        {
            try
            {
                object catiaObject = System.Runtime.InteropServices.Marshal.GetActiveObject(
                    "CATIA.Application");
                hsp_catiaApp = (INFITF.Application)catiaObject;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public void ErstelleLeereSkizze()
        {
            // geometrisches Set auswaehlen und umbenennen
            HybridBodies catHybridBodies1 = hsp_catiaPart.Part.HybridBodies;
            HybridBody catHybridBody1;
            try
            {
                catHybridBody1 = catHybridBodies1.Item("Geometrisches Set.1");
            }
            catch (Exception)
            {
                MessageBox.Show("Kein geometrisches Set gefunden! " + Environment.NewLine +
                    "Ein PART manuell erzeugen und ein darauf achten, dass 'Geometisches Set' aktiviert ist.",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            catHybridBody1.set_Name("Profile");
            // neue Skizze im ausgewaehlten geometrischen Set anlegen
            Sketches catSketches1 = catHybridBody1.HybridSketches;
            OriginElements catOriginElements = hsp_catiaPart.Part.OriginElements;
            Reference catReference1 = (Reference)catOriginElements.PlaneYZ;
            hsp_catiaProfil =  catSketches1.Add(catReference1);

            // Achsensystem in Skizze erstellen 
            ErzeugeAchsensystem();

            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }


        private void ErzeugeAchsensystem()
        {
            object[] arr = new object[] {0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0,
                                         0.0, 0.0, 1.0 };
            hsp_catiaProfil.SetAbsoluteAxisData(arr);
        }

        private void ProfilKopf(object[] ParameterListe)
        {
            // Skizze umbenennen
            hsp_catiaProfil.set_Name("Kopf");

           
            // Skizze oeffnen
            Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();

            // Listen Werte wieder in richtige Datentypen umwandeln
            int Kopf = Convert.ToInt32(ParameterListe[0]);
            double Durchmesser = Convert.ToDouble(ParameterListe[1]);
            double Gewindelänge = Convert.ToDouble(ParameterListe[2]);
            double Schaftlänge = Convert.ToDouble(ParameterListe[3]);
            double Steigung = Convert.ToDouble(ParameterListe[4]);
            int Gewindeart = Convert.ToInt32(ParameterListe[5]);
            double Schlüsselweite = Convert.ToDouble(ParameterListe[6]);
            double Kopfhöhe = Convert.ToDouble(ParameterListe[7]);
            double Kopfdurchmesser = Convert.ToDouble(ParameterListe[8]);
            string Schraubenrichtung = Convert.ToString(ParameterListe[9]);
            

            //Sechskantkopf
            if (Kopf == 1)
            {
                //Eckkoordinaten berechnen
                double Grad = 30;
                double Halbkreis = 180;
                double Winkel = Grad / Halbkreis * Math.PI;
                double swr = Schlüsselweite / 2;
                double x = Math.Tan(Winkel) *swr;
                double h = Schlüsselweite / (2 * Math.Cos(Winkel));

                
                //Punkte zeichnen
                Point2D catPoint2D1 = catFactory2D1.CreatePoint(-x, swr);
                Point2D catPoint2D2 = catFactory2D1.CreatePoint(x, swr);
                Point2D catPoint2D3 = catFactory2D1.CreatePoint(h, 0);
                Point2D catPoint2D4 = catFactory2D1.CreatePoint(x, -swr);
                Point2D catPoint2D5 = catFactory2D1.CreatePoint(-x, -swr);
                Point2D catPoint2D6 = catFactory2D1.CreatePoint(-h, 0);

                //Linien zeichnen
                Line2D catLine2D1 = catFactory2D1.CreateLine(-x, swr, x, swr);
                catLine2D1.StartPoint = catPoint2D1;
                catLine2D1.EndPoint = catPoint2D2;

                Line2D catLine2D2 = catFactory2D1.CreateLine(x, swr, h, 0);
                catLine2D2.StartPoint = catPoint2D2;
                catLine2D2.EndPoint = catPoint2D3;

                Line2D catLine2D3 = catFactory2D1.CreateLine(h, 0, x, -swr);
                catLine2D3.StartPoint = catPoint2D3;
                catLine2D3.EndPoint = catPoint2D4;

                Line2D catLine2D4 = catFactory2D1.CreateLine(x, -swr, -x, -swr);
                catLine2D4.StartPoint = catPoint2D4;
                catLine2D4.EndPoint = catPoint2D5;

                Line2D catLine2D5 = catFactory2D1.CreateLine(-x, -swr, -h, 0);
                catLine2D5.StartPoint = catPoint2D5;
                catLine2D5.EndPoint = catPoint2D6;

                Line2D catLine2D6 = catFactory2D1.CreateLine(-h, 0, -x, swr);
                catLine2D6.StartPoint = catPoint2D6;
                catLine2D6.EndPoint = catPoint2D1;
            }

            //Zylinderkopf
            if(Kopf == 2)
            {

            }

            //Senkkopf
            if (Kopf == 3)
            {

            }


            // Skizzierer verlassen
            hsp_catiaProfil.CloseEdition();
            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }


        private void BlockErstellen(object[] ParameterListe)
        {
            double Kopfhöhe = Convert.ToDouble(ParameterListe[7]);

            // Hauptkoerper in Bearbeitung definieren
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;

            // Block(Balken) erzeugen
            ShapeFactory catShapeFactory1 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;
            Pad catPad1 = catShapeFactory1.AddNewPad(hsp_catiaProfil, Kopfhöhe);

            // Block umbenennen
            catPad1.set_Name("Kopf-Block");

            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }


        


        private void ProfilSchaft(object[]ParameterListe)
        {
            double Durchmesser = Convert.ToDouble(ParameterListe[1]);
            double Gewindelänge = Convert.ToDouble(ParameterListe[2]);
            double Schaftlänge = Convert.ToDouble(ParameterListe[3]);

            // Skizze oeffnen
            Factory2D catFactory2D2 = hsp_catiaProfil2.OpenEdition();

            Circle2D catCircle2D1 = catFactory2D2.CreateCircle(0, 0, Durchmesser/2, 0, 0);           

            // Skizzierer verlassen
            hsp_catiaProfil2.CloseEdition();
            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }

    }

}
