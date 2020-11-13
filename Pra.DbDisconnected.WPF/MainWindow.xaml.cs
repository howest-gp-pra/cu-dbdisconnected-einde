using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.IO;

namespace Pra.DbDisconnected.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        DataSet dsBoekenLijst = new DataSet("Bibliotheek");
        string XMLMap = Directory.GetCurrentDirectory() + "/XMLBestanden";
        string XMLBestand = Directory.GetCurrentDirectory() + "/XMLBestanden/boeken.xml";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!LeesXML())
            {
                MaakTabellen();
                VulTabellen();
            }
            dgAuthors.ItemsSource = dsBoekenLijst.Tables[0].DefaultView;
            dgBooks.ItemsSource = dsBoekenLijst.Tables[2].DefaultView;
            BronGegevensAanpassen();
        }
        private bool LeesXML()
        {
            bool gelezen = false;
            if (Directory.Exists(XMLMap))
            {
                if (File.Exists(XMLBestand))
                {
                    dsBoekenLijst.ReadXml(XMLBestand, XmlReadMode.ReadSchema);
                    gelezen = true;
                }
            }
            return gelezen;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Directory.Exists(XMLMap))
                Directory.CreateDirectory(XMLMap);
            if (File.Exists(XMLBestand))
                File.Delete(XMLBestand);
            dsBoekenLijst.WriteXml(XMLBestand, XmlWriteMode.WriteSchema);
        }
        private void MaakTabellen()
        {
            // creatie dtAuteur
            DataTable dtAuteur;
            dtAuteur = new DataTable();
            dsBoekenLijst.Tables.Add(dtAuteur);

            dtAuteur.TableName = "Auteur";

            DataColumn dcAuteurID = new DataColumn();
            dcAuteurID.ColumnName = "auteurID";
            dcAuteurID.DataType = typeof(int);
            dcAuteurID.AutoIncrement = true;
            dcAuteurID.AutoIncrementSeed = 1;
            dcAuteurID.AutoIncrementStep = 1;
            dcAuteurID.Unique = true;
            dtAuteur.Columns.Add(dcAuteurID);
            dtAuteur.PrimaryKey = new DataColumn[] { dcAuteurID };

            DataColumn dcAuteurNaam = new DataColumn();
            dcAuteurNaam.ColumnName = "auteurNaam";
            dcAuteurNaam.DataType = typeof(string);
            dcAuteurNaam.MaxLength = 50;
            dtAuteur.Columns.Add(dcAuteurNaam);

            DataColumn dcDdisplayAuteur = new DataColumn("DisplayAuteur");
            dcDdisplayAuteur.Expression = "auteurNaam + ' (' + auteurID + ')'";
            dtAuteur.Columns.Add(dcDdisplayAuteur);

            // creatie dtUitgever
            DataTable dtUitgever = new DataTable();
            dtUitgever.TableName = "Uitgever";
            dsBoekenLijst.Tables.Add(dtUitgever);

            DataColumn dcUitgeverID = new DataColumn();
            dcUitgeverID.ColumnName = "uitgeverID";
            dcUitgeverID.DataType = typeof(int);
            dcUitgeverID.AutoIncrement = true;
            dcUitgeverID.AutoIncrementSeed = 1;
            dcUitgeverID.AutoIncrementStep = 1;
            dcUitgeverID.Unique = true;

            DataColumn dcUitgeverNaam = new DataColumn();
            dcUitgeverNaam.ColumnName = "uitgeverNaam";
            dcUitgeverNaam.DataType = typeof(string);
            dcUitgeverNaam.MaxLength = 50;

            dtUitgever.Columns.Add(dcUitgeverID);
            dtUitgever.Columns.Add(dcUitgeverNaam);
            dtUitgever.PrimaryKey = new DataColumn[] { dcUitgeverID };

            // creatie boeken
            DataTable dtBoeken = new DataTable();
            dtBoeken.TableName = "Boeken";
            dsBoekenLijst.Tables.Add(dtBoeken);

            DataColumn dcboekID = new DataColumn();
            dcboekID.ColumnName = "boekID";
            dcboekID.DataType = typeof(int);
            dcboekID.AutoIncrement = true;
            dcboekID.AutoIncrementSeed = 1;
            dcboekID.AutoIncrementStep = 1;
            dcboekID.Unique = true;
            dtBoeken.Columns.Add(dcboekID);
            dtBoeken.PrimaryKey = new DataColumn[] { dcboekID };

            // de overige velden voegen we op een kortere manier toe
            dtBoeken.Columns.Add("Titel", typeof(string));
            dtBoeken.Columns.Add("AuteurID", typeof(int));
            dtBoeken.Columns.Add("UitgeverID", typeof(int));
            dtBoeken.Columns.Add("Jaartal", typeof(int));

            dsBoekenLijst.Relations.Add(dsBoekenLijst.Tables[0].Columns["AuteurID"], dsBoekenLijst.Tables[2].Columns["AuteurID"]);
        }
        private void VulTabellen()
        {
            ToevoegenDataAuteur("Boon Louis");
            ToevoegenDataAuteur("Tuchman Barbara");
            ToevoegenDataAuteur("Cook Robin");
            ToevoegenDataUitgever("AW Bruna");
            ToevoegenDataUitgever("Luttingh");
        }
        private void ToevoegenDataAuteur(string naam)
        {
            DataRow nieuweAuteur = dsBoekenLijst.Tables["Auteur"].NewRow();
            nieuweAuteur["auteurNaam"] = naam;
            dsBoekenLijst.Tables["Auteur"].Rows.Add(nieuweAuteur);
        }
        private void ToevoegenDataUitgever(string naam)
        {
            DataRow nieuweUitgever = dsBoekenLijst.Tables["Uitgever"].NewRow();
            nieuweUitgever["UitgeverNaam"] = naam;
            dsBoekenLijst.Tables["Uitgever"].Rows.Add(nieuweUitgever);
        }
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            DataView gesorteerdeTabel = new DataView();
            gesorteerdeTabel.Table = dsBoekenLijst.Tables["Auteur"];
            gesorteerdeTabel.Sort = "AuteurNaam desc, AuteurID desc";
            dgAuthors.ItemsSource = gesorteerdeTabel;
        }
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            DataView gefilterdeTabel = new DataView(dsBoekenLijst.Tables["Auteur"]);
            gefilterdeTabel.RowFilter = "AuteurNaam like 'T%'";
            dgAuthors.ItemsSource = gefilterdeTabel;
        }
        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            string auteur = txtAuthor.Text.Trim();
            ToevoegenDataAuteur(auteur);
            dgAuthors.ItemsSource = dsBoekenLijst.Tables["Auteur"].DefaultView;
            BronGegevensAanpassen();
        }
        private void BronGegevensAanpassen()
        {
            cmbAuthors.Items.Clear();
            cmbPublishers.Items.Clear();
            ComboBoxItem itm;
            for (int teller = 0; teller < dsBoekenLijst.Tables[0].Rows.Count; teller++)
            {
                itm = new ComboBoxItem();
                itm.Content = (dsBoekenLijst.Tables[0].Rows[teller][1]);
                itm.Tag = (dsBoekenLijst.Tables[0].Rows[teller][0]);
                cmbAuthors.Items.Add(itm);
            }
            cmbAuthors.SelectedIndex = 0;

            for (int teller = 0; teller < dsBoekenLijst.Tables[1].Rows.Count; teller++)
            {
                itm = new ComboBoxItem();
                itm.Content = dsBoekenLijst.Tables[1].Rows[teller][1];
                itm.Tag = dsBoekenLijst.Tables[1].Rows[teller][0];
                cmbPublishers.Items.Add(itm);
            }
            cmbPublishers.SelectedIndex = 0;
        }
        private void ToevoegenBoek(string titel, int auteurID, int uitgeverID, int jaartal)
        {
            DataRow nieuwBoek = dsBoekenLijst.Tables[2].NewRow(); //Tables[2] is Tables["Boeken"]
            nieuwBoek["Titel"] = titel;
            nieuwBoek["AuteurID"] = auteurID;
            nieuwBoek["UitgeverID"] = uitgeverID;
            nieuwBoek["Jaartal"] = jaartal;
            dsBoekenLijst.Tables[2].Rows.Add(nieuwBoek);
        }
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string titel = txtTitle.Text;
                int jaartal = int.Parse(txtYear.Text);
                ComboBoxItem itm;
                itm = (ComboBoxItem)cmbAuthors.SelectedItem;
                int IDauteur = int.Parse(itm.Tag.ToString());
                itm = (ComboBoxItem)cmbPublishers.SelectedItem;
                int IDuitgever = int.Parse(itm.Tag.ToString());
                ToevoegenBoek(titel, IDauteur, IDuitgever, jaartal);
                dgBooks.ItemsSource = dsBoekenLijst.Tables[2].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Foute ingave \nReden :" + ex.Message);
            }
        }
        private void RemoveAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (dgAuthors.SelectedIndex > -1)
            {
                string zoekAuteurID = dgAuthors.SelectedValue.ToString();
                foreach (DataRow rij in dsBoekenLijst.Tables[0].Rows)
                {
                    if (rij["auteurID"].ToString() == zoekAuteurID)
                    {
                        dsBoekenLijst.Tables[0].Rows.Remove(rij);
                        break;
                    }
                }
                BronGegevensAanpassen();
                dgAuthors.ItemsSource = dsBoekenLijst.Tables[0].DefaultView;
                dgBooks.ItemsSource = dsBoekenLijst.Tables[2].DefaultView;
            }
        }





    }
}
