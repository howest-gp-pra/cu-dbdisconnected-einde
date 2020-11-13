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
        DataSet dsBookList = new DataSet("Bibliotheek");
        string XMLDirectory = Directory.GetCurrentDirectory() + "/XMLBestanden";
        string XMLFile = Directory.GetCurrentDirectory() + "/XMLBestanden/boeken.xml";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ReadXML())
            {
                CreateTables();
                FillTables();
            }
            dgAuthors.ItemsSource = dsBookList.Tables[0].DefaultView;
            dgBooks.ItemsSource = dsBookList.Tables[2].DefaultView;
            SourceDataChange();
        }
        private bool ReadXML()
        {
            bool read = false;
            if (Directory.Exists(XMLDirectory))
            {
                if (File.Exists(XMLFile))
                {
                    dsBookList.ReadXml(XMLFile, XmlReadMode.ReadSchema);
                    read = true;
                }
            }
            return read;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Directory.Exists(XMLDirectory))
                Directory.CreateDirectory(XMLDirectory);
            if (File.Exists(XMLFile))
                File.Delete(XMLFile);
            dsBookList.WriteXml(XMLFile, XmlWriteMode.WriteSchema);
        }
        private void CreateTables()
        {
            // creatie dtAuteur
            DataTable dtAuthor;
            dtAuthor = new DataTable();
            dsBookList.Tables.Add(dtAuthor);

            dtAuthor.TableName = "Auteur";

            DataColumn dcAuthorId = new DataColumn();
            dcAuthorId.ColumnName = "auteurID";
            dcAuthorId.DataType = typeof(int);
            dcAuthorId.AutoIncrement = true;
            dcAuthorId.AutoIncrementSeed = 1;
            dcAuthorId.AutoIncrementStep = 1;
            dcAuthorId.Unique = true;
            dtAuthor.Columns.Add(dcAuthorId);
            dtAuthor.PrimaryKey = new DataColumn[] { dcAuthorId };

            DataColumn dcAuthorName = new DataColumn();
            dcAuthorName.ColumnName = "auteurNaam";
            dcAuthorName.DataType = typeof(string);
            dcAuthorName.MaxLength = 50;
            dtAuthor.Columns.Add(dcAuthorName);

            DataColumn dcDisplayAuthor = new DataColumn("DisplayAuteur");
            dcDisplayAuthor.Expression = "auteurNaam + ' (' + auteurID + ')'";
            dtAuthor.Columns.Add(dcDisplayAuthor);

            // creatie dtUitgever
            DataTable dtPublisher = new DataTable();
            dtPublisher.TableName = "Uitgever";
            dsBookList.Tables.Add(dtPublisher);

            DataColumn dcPublisherId = new DataColumn();
            dcPublisherId.ColumnName = "uitgeverID";
            dcPublisherId.DataType = typeof(int);
            dcPublisherId.AutoIncrement = true;
            dcPublisherId.AutoIncrementSeed = 1;
            dcPublisherId.AutoIncrementStep = 1;
            dcPublisherId.Unique = true;

            DataColumn dcPUblisherName = new DataColumn();
            dcPUblisherName.ColumnName = "uitgeverNaam";
            dcPUblisherName.DataType = typeof(string);
            dcPUblisherName.MaxLength = 50;

            dtPublisher.Columns.Add(dcPublisherId);
            dtPublisher.Columns.Add(dcPUblisherName);
            dtPublisher.PrimaryKey = new DataColumn[] { dcPublisherId };

            // creatie boeken
            DataTable dtBooks = new DataTable();
            dtBooks.TableName = "Boeken";
            dsBookList.Tables.Add(dtBooks);

            DataColumn dcBookId = new DataColumn();
            dcBookId.ColumnName = "boekID";
            dcBookId.DataType = typeof(int);
            dcBookId.AutoIncrement = true;
            dcBookId.AutoIncrementSeed = 1;
            dcBookId.AutoIncrementStep = 1;
            dcBookId.Unique = true;
            dtBooks.Columns.Add(dcBookId);
            dtBooks.PrimaryKey = new DataColumn[] { dcBookId };

            // de overige velden voegen we op een kortere manier toe
            dtBooks.Columns.Add("Titel", typeof(string));
            dtBooks.Columns.Add("AuteurID", typeof(int));
            dtBooks.Columns.Add("UitgeverID", typeof(int));
            dtBooks.Columns.Add("Jaartal", typeof(int));

            dsBookList.Relations.Add(dsBookList.Tables[0].Columns["AuteurID"], dsBookList.Tables[2].Columns["AuteurID"]);
        }
        private void FillTables()
        {
            AddDataAuthor("Boon Louis");
            AddDataAuthor("Tuchman Barbara");
            AddDataAuthor("Cook Robin");
            AddDataPublisher("AW Bruna");
            AddDataPublisher("Luttingh");
        }
        private void AddDataAuthor(string name)
        {
            DataRow newAuthor = dsBookList.Tables["Auteur"].NewRow();
            newAuthor["auteurNaam"] = name;
            dsBookList.Tables["Auteur"].Rows.Add(newAuthor);
        }
        private void AddDataPublisher(string name)
        {
            DataRow newPublisher = dsBookList.Tables["Uitgever"].NewRow();
            newPublisher["UitgeverNaam"] = name;
            dsBookList.Tables["Uitgever"].Rows.Add(newPublisher);
        }
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            DataView sortedTable = new DataView();
            sortedTable.Table = dsBookList.Tables["Auteur"];
            sortedTable.Sort = "AuteurNaam desc, AuteurID desc";
            dgAuthors.ItemsSource = sortedTable;
        }
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            DataView filteredTable = new DataView(dsBookList.Tables["Auteur"]);
            filteredTable.RowFilter = "AuteurNaam like 'T%'";
            dgAuthors.ItemsSource = filteredTable;
        }
        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            string author = txtAuthor.Text.Trim();
            AddDataAuthor(author);
            dgAuthors.ItemsSource = dsBookList.Tables["Auteur"].DefaultView;
            SourceDataChange();
        }
        private void SourceDataChange()
        {
            cmbAuthors.Items.Clear();
            cmbPublishers.Items.Clear();
            ComboBoxItem itm;
            for (int counter = 0; counter < dsBookList.Tables[0].Rows.Count; counter++)
            {
                itm = new ComboBoxItem();
                itm.Content = (dsBookList.Tables[0].Rows[counter][1]);
                itm.Tag = (dsBookList.Tables[0].Rows[counter][0]);
                cmbAuthors.Items.Add(itm);
            }
            cmbAuthors.SelectedIndex = 0;

            for (int counter = 0; counter < dsBookList.Tables[1].Rows.Count; counter++)
            {
                itm = new ComboBoxItem();
                itm.Content = dsBookList.Tables[1].Rows[counter][1];
                itm.Tag = dsBookList.Tables[1].Rows[counter][0];
                cmbPublishers.Items.Add(itm);
            }
            cmbPublishers.SelectedIndex = 0;
        }
        private void AddBook(string title, int authorId, int publisherId, int year)
        {
            DataRow newBook = dsBookList.Tables[2].NewRow(); //Tables[2] is Tables["Boeken"]
            newBook["Titel"] = title;
            newBook["AuteurID"] = authorId;
            newBook["UitgeverID"] = publisherId;
            newBook["Jaartal"] = year;
            dsBookList.Tables[2].Rows.Add(newBook);
        }
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title = txtTitle.Text;
                int year = int.Parse(txtYear.Text);
                ComboBoxItem itm;
                itm = (ComboBoxItem)cmbAuthors.SelectedItem;
                int IDauthor = int.Parse(itm.Tag.ToString());
                itm = (ComboBoxItem)cmbPublishers.SelectedItem;
                int IDpublisher = int.Parse(itm.Tag.ToString());
                AddBook(title, IDauthor, IDpublisher, year);
                dgBooks.ItemsSource = dsBookList.Tables[2].DefaultView;
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
                string searchAuthorId = dgAuthors.SelectedValue.ToString();
                foreach (DataRow row in dsBookList.Tables[0].Rows)
                {
                    if (row["auteurID"].ToString() == searchAuthorId)
                    {
                        dsBookList.Tables[0].Rows.Remove(row);
                        break;
                    }
                }
                SourceDataChange();
                dgAuthors.ItemsSource = dsBookList.Tables[0].DefaultView;
                dgBooks.ItemsSource = dsBookList.Tables[2].DefaultView;
            }
        }





    }
}
