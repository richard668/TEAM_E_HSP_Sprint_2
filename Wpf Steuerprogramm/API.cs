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
        MECMOD.Sketch hsp_catiaProfil_Schlüssel;
        MECMOD.Sketch hsp_catiaProfil_Schaft;
        MECMOD.Sketch hsp_catiaProfil_Senkkopf;
        Part myPart;
        Pad Schaft;
        ShapeFactory catShapeFactorySchaft;


        public int Kopf { get; private set; }
        public int Schlüsselweite { get; private set; }
        public int Kopfdurchmesser { get; private set; }

        public CAD(object [] ParameterListe)
        {
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
            //Hier wird das gesamte Modell erstellt, wenn nichts schief geht
            try
            {

                //Abfrage ob Catia geöffnet ist
                if(CatiaLaeuft())
                {

                    //Erstellt ein neues Part
                    INFITF.Documents catDocuments1 = hsp_catiaApp.Documents;
                    hsp_catiaPart = catDocuments1.Add("Part") as MECMOD.PartDocument;

                    //Erstellt alle Skizzen
                    ErstelleLeereSkizze();

                    //Erstellt ein Profil vom Kopf
                    ProfilKopf(ParameterListe);

                    //Erstellt einen Block für den Kopf
                    if (Kopf == 1 | Kopf == 2)
                    { BlockKopfErstellen(ParameterListe); }
                    else
                    { RotationskörperErstellen(); }

                    //Erstellt ein Profil für den Schaft
                    ProfilSchaft(ParameterListe);

                    //Erstellt ein Block für den Schaft
                    BlockSchaftErstellen(ParameterListe);

                    if (Kopf==2 | Kopf == 3)
                    {
                        ErstelleTascheProfil(ParameterListe);
                        ErstelleTasche(ParameterListe);
                    }



                    //Erstellt Gewinde am Schaft
                    Gewindefeature(ParameterListe);


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
            Reference catReference2 = (Reference)catOriginElements.PlaneZX;
            hsp_catiaProfil =  catSketches1.Add(catReference1);            
            hsp_catiaProfil_Schaft = catSketches1.Add(catReference1);
            hsp_catiaProfil_Schlüssel = catSketches1.Add(catReference1);
            hsp_catiaProfil_Senkkopf = catSketches1.Add(catReference2);

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

            // Skizze umbenennen
            hsp_catiaProfil.set_Name("Kopf");
            hsp_catiaProfil_Senkkopf.set_Name("Senkkopf");

                              
            

            //Sechskantkopf
            if (Kopf == 1)
            {
                // Skizze oeffnen
                Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();


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

                // Skizzierer verlassen
                hsp_catiaProfil.CloseEdition();
            }

            //Zylinderkopf
            if(Kopf == 2)
            {
                // Skizze oeffnen
                Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
                Circle2D circle2DZylinderKopf = catFactory2D1.CreateClosedCircle(0, 0, Kopfdurchmesser/2);
                // Skizzierer verlassen
                hsp_catiaProfil.CloseEdition();

            }

            //Senkkopf
            if (Kopf == 3)
            {
                // Skizze oeffnen
                Factory2D catFactory2D1 = hsp_catiaProfil_Senkkopf.OpenEdition();

                //Punkte zeichnen
                Point2D catPoint2D1 = catFactory2D1.CreatePoint(0, 0);
                Point2D catPoint2D2 = catFactory2D1.CreatePoint(0, Kopfhöhe);
                Point2D catPoint2D3 = catFactory2D1.CreatePoint(Durchmesser / 2, Kopfhöhe);
                Point2D catPoint2D4 = catFactory2D1.CreatePoint(Kopfdurchmesser/2, 0);


                //Linien zeichnen
                Line2D catLine2D1 = catFactory2D1.CreateLine(0, 0, 0, Kopfhöhe);
                catLine2D1.StartPoint = catPoint2D1;
                catLine2D1.EndPoint = catPoint2D2;

                Line2D catLine2D2 = catFactory2D1.CreateLine(0, Kopfhöhe, Durchmesser / 2, Kopfhöhe);
                catLine2D2.StartPoint = catPoint2D2;
                catLine2D2.EndPoint = catPoint2D3;

                Line2D catLine2D3 = catFactory2D1.CreateLine(Durchmesser / 2, Kopfhöhe, Kopfdurchmesser/2, 0);
                catLine2D3.StartPoint = catPoint2D3;
                catLine2D3.EndPoint = catPoint2D4;

                Line2D catLine2D4 = catFactory2D1.CreateLine(Kopfdurchmesser/2, 0, 0, 0);
                catLine2D4.StartPoint = catPoint2D4;
                catLine2D4.EndPoint = catPoint2D1;

                // Skizzierer verlassen
                hsp_catiaProfil.CloseEdition();
            }


           
            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }


        private void BlockKopfErstellen(object[] ParameterListe)
        {
            double Kopfhöhe = Convert.ToDouble(ParameterListe[7]);
            double Durchmesser = Convert.ToDouble(ParameterListe[1]);
            double Schlüsseltiefe = 0 - Durchmesser / 2;

            // Listen Werte wieder in richtige Datentypen umwandeln
            int Kopf = Convert.ToInt32(ParameterListe[0]);
            double Gewindelänge = Convert.ToDouble(ParameterListe[2]);
            double Schaftlänge = Convert.ToDouble(ParameterListe[3]);
            double Steigung = Convert.ToDouble(ParameterListe[4]);
            int Gewindeart = Convert.ToInt32(ParameterListe[5]);
            double Schlüsselweite = Convert.ToDouble(ParameterListe[6]);
            double Kopfdurchmesser = Convert.ToDouble(ParameterListe[8]);
            string Schraubenrichtung = Convert.ToString(ParameterListe[9]);

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

        private void ErstelleTascheProfil(object[] ParameterListe)
        {
            double Kopfhöhe = Convert.ToDouble(ParameterListe[7]);
            double Durchmesser = Convert.ToDouble(ParameterListe[1]);
            double Schlüsseltiefe = 0 - Durchmesser / 2;

            // Listen Werte wieder in richtige Datentypen umwandeln
            int Kopf = Convert.ToInt32(ParameterListe[0]);
            double Gewindelänge = Convert.ToDouble(ParameterListe[2]);
            double Schaftlänge = Convert.ToDouble(ParameterListe[3]);
            double Steigung = Convert.ToDouble(ParameterListe[4]);
            int Gewindeart = Convert.ToInt32(ParameterListe[5]);
            double Schlüsselweite = Convert.ToDouble(ParameterListe[6]);
            double Kopfdurchmesser = Convert.ToDouble(ParameterListe[8]);
            string Schraubenrichtung = Convert.ToString(ParameterListe[9]);

            // Skizze umbenennen
            hsp_catiaProfil_Schlüssel.set_Name("Schlüssel");
            // Skizze oeffnen
            Factory2D catFactory2D3 = hsp_catiaProfil_Schlüssel.OpenEdition();

            //Eckkoordinaten berechnen
            double Grad = 30;
            double Halbkreis = 180;
            double Winkel = Grad / Halbkreis * Math.PI;
            double swr = Schlüsselweite / 2;
            double x = Math.Tan(Winkel) * swr;
            double h = Schlüsselweite / (2 * Math.Cos(Winkel));


            //Punkte zeichnen
            Point2D catPoint2D1 = catFactory2D3.CreatePoint(-x, swr);
            Point2D catPoint2D2 = catFactory2D3.CreatePoint(x, swr);
            Point2D catPoint2D3 = catFactory2D3.CreatePoint(h, 0);
            Point2D catPoint2D4 = catFactory2D3.CreatePoint(x, -swr);
            Point2D catPoint2D5 = catFactory2D3.CreatePoint(-x, -swr);
            Point2D catPoint2D6 = catFactory2D3.CreatePoint(-h, 0);

            //Linien zeichnen
            Line2D catLine2D1 = catFactory2D3.CreateLine(-x, swr, x, swr);
            catLine2D1.StartPoint = catPoint2D1;
            catLine2D1.EndPoint = catPoint2D2;

            Line2D catLine2D2 = catFactory2D3.CreateLine(x, swr, h, 0);
            catLine2D2.StartPoint = catPoint2D2;
            catLine2D2.EndPoint = catPoint2D3;

            Line2D catLine2D3 = catFactory2D3.CreateLine(h, 0, x, -swr);
            catLine2D3.StartPoint = catPoint2D3;
            catLine2D3.EndPoint = catPoint2D4;

            Line2D catLine2D4 = catFactory2D3.CreateLine(x, -swr, -x, -swr);
            catLine2D4.StartPoint = catPoint2D4;
            catLine2D4.EndPoint = catPoint2D5;

            Line2D catLine2D5 = catFactory2D3.CreateLine(-x, -swr, -h, 0);
            catLine2D5.StartPoint = catPoint2D5;
            catLine2D5.EndPoint = catPoint2D6;

            Line2D catLine2D6 = catFactory2D3.CreateLine(-h, 0, -x, swr);
            catLine2D6.StartPoint = catPoint2D6;
            catLine2D6.EndPoint = catPoint2D1;

            // Skizzierer verlassen
            hsp_catiaProfil_Schlüssel.CloseEdition();
            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }

        private void ErstelleTasche(object[] ParameterListe)
        {
            double Kopfhöhe = Convert.ToDouble(ParameterListe[7]);
            double Durchmesser = Convert.ToDouble(ParameterListe[1]);
            double Schlüsseltiefe = 0 - Durchmesser / 2;

            // Listen Werte wieder in richtige Datentypen umwandeln
            int Kopf = Convert.ToInt32(ParameterListe[0]);
            double Gewindelänge = Convert.ToDouble(ParameterListe[2]);
            double Schaftlänge = Convert.ToDouble(ParameterListe[3]);
            double Steigung = Convert.ToDouble(ParameterListe[4]);
            int Gewindeart = Convert.ToInt32(ParameterListe[5]);
            double Schlüsselweite = Convert.ToDouble(ParameterListe[6]);
            double Kopfdurchmesser = Convert.ToDouble(ParameterListe[8]);
            string Schraubenrichtung = Convert.ToString(ParameterListe[9]);

           
            // Tasche erzeugen
            ShapeFactory catShapeFactory11 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;

            Pocket catPad3 = catShapeFactory11.AddNewPocket(hsp_catiaProfil_Schlüssel, Schlüsseltiefe);

            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }


        private void ProfilSchaft(object[]ParameterListe)
        {
            double Durchmesser = Convert.ToDouble(ParameterListe[1]);
            
            // Skizze umbenennen
            hsp_catiaProfil_Schaft.set_Name("Schaft");

            // Skizze oeffnen
            Factory2D catFactory2D2 = hsp_catiaProfil_Schaft.OpenEdition();

            
            Circle2D circle2DSchaft = catFactory2D2.CreateClosedCircle(0, 0, Durchmesser / 2);

            // Skizzierer verlassen
            hsp_catiaProfil_Schaft.CloseEdition();
            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }

        private void BlockSchaftErstellen(object[] ParameterListe)
        {
            
            // Listen Werte wieder in richtige Datentypen umwandeln
            int Kopf = Convert.ToInt32(ParameterListe[0]);
            double Gewindelänge = Convert.ToDouble(ParameterListe[2]);
            double Schaftlänge = Convert.ToDouble(ParameterListe[3]);
            double Steigung = Convert.ToDouble(ParameterListe[4]);
            int Gewindeart = Convert.ToInt32(ParameterListe[5]);
            double Schlüsselweite = Convert.ToDouble(ParameterListe[6]);
            double Kopfhöhe = Convert.ToDouble(ParameterListe[7]);
            double Kopfdurchmesser = Convert.ToDouble(ParameterListe[8]);
            string Schraubenrichtung = Convert.ToString(ParameterListe[9]);

            // Hauptkoerper in Bearbeitung definieren
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;

            // Block(Balken) erzeugen
            catShapeFactorySchaft = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;



            Schaft = catShapeFactorySchaft.AddNewPad(hsp_catiaProfil_Schaft, Gewindelänge+Schaftlänge+Kopfhöhe);
            // Block umbenennen
            Schaft.set_Name("Schaft-Block");



            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }

        internal void RotationskörperErstellen()
        {
            // Hauptkoerper in Bearbeitung definieren
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;

            //Rotationskörper erzeugen
            ShapeFactory catShapeFactorySenkkopf = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;

            Rib senkKopf = catShapeFactorySenkkopf.AddNewRib(hsp_catiaProfil_Senkkopf, hsp_catiaProfil_Schaft);

        }


        internal void Gewindefeature(object[] ParameterListe)
        {

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
            myPart = hsp_catiaPart.Part;
            // Gewinde...
            // ... Referenzen lateral und limit erzeugen

            Reference RefMantelflaeche;
            Reference RefFrontflaeche;

            if (Kopf!=3&&Gewindeart==1)
            {
                RefMantelflaeche = myPart.CreateReferenceFromBRepName(
                "RSur:(Face:(Brp:(Pad.2;0:(Brp:(Sketch.2;1)));None:();Cf11:());WithTemporaryBody;WithoutBuildError;WithSelectingFeatureSupport;MFBRepVersion_CXR15)", Schaft);
                
                RefFrontflaeche = myPart.CreateReferenceFromBRepName(
                    "RSur:(Face:(Brp:(Pad.2;2);None:();Cf11:());WithTemporaryBody;WithoutBuildError;WithSelectingFeatureSupport;MFBRepVersion_CXR15)", Schaft);

                // ... Gewinde erzeugen, Parameter setzen
                PARTITF.Thread thread1 = catShapeFactorySchaft.AddNewThreadWithOutRef();
                thread1.Side = CatThreadSide.catRightSide;
                thread1.Diameter = Durchmesser;
                thread1.Depth = Gewindelänge;
                thread1.LateralFaceElement = RefMantelflaeche; // Referenz lateral
                thread1.LimitFaceElement = RefFrontflaeche; // Referenz limit

                // ... Standardgewinde gesteuert über eine Konstruktionstabelle
                thread1.CreateUserStandardDesignTable("Metric_Thick_Pitch", @"C:\Program Files\Dassault Systemes\B28\win_b64\resources\standard\thread\Metric_Thick_Pitch.xml");
                thread1.Diameter = Durchmesser;
                thread1.Pitch = Steigung;
            }

            if (Kopf==3&&Gewindeart==1)
            {
                RefMantelflaeche = myPart.CreateReferenceFromBRepName(
                "RSur:(Face:(Brp:(Pad.1;0:(Brp:(Sketch.2;1)));None:();Cf12:());WithTemporaryBody;WithoutBuildError;WithSelectingFeatureSupport;MFBRepVersion_CXR29)", Schaft);

                RefFrontflaeche = myPart.CreateReferenceFromBRepName(
                    "RSur:(Face:(Brp:(Pad.1;2);None:();Cf12:());WithTemporaryBody;WithoutBuildError;WithSelectingFeatureSupport;MFBRepVersion_CXR29)", Schaft);

                // ... Gewinde erzeugen, Parameter setzen
                PARTITF.Thread thread1 = catShapeFactorySchaft.AddNewThreadWithOutRef();
                thread1.Side = CatThreadSide.catRightSide;
                thread1.Diameter = Durchmesser;
                thread1.Depth = Gewindelänge;
                thread1.LateralFaceElement = RefMantelflaeche; // Referenz lateral
                thread1.LimitFaceElement = RefFrontflaeche; // Referenz limit

                // ... Standardgewinde gesteuert über eine Konstruktionstabelle
                thread1.CreateUserStandardDesignTable("Metric_Thick_Pitch", @"C:\Program Files\Dassault Systemes\B28\win_b64\resources\standard\thread\Metric_Thick_Pitch.xml");
                thread1.Diameter = Durchmesser;
                thread1.Pitch = Steigung;
            }

                        
            // Part update und fertig
            myPart.Update();
        }
    }

}
