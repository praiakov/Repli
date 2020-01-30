using LiteDB;
using Repli.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Repli
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            fbdFromFile.Description = "Selecione pasta de origem";
            fbdFromFile.RootFolder = Environment.SpecialFolder.MyComputer;
            fbdFromFile.ShowNewFolderButton = true;
            if (fbdFromFile.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = fbdFromFile.SelectedPath;
            }
        }

        private void Frm(object sender, EventArgs e)
        {
            LoadListFolder();                        
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (!"".Equals(txtPath.Text))
            {
                for (int i = 0; i < clbServer.Items.Count; i++)
                {
                    if (clbServer.GetItemChecked(i))
                    {
                        string pathFrom = (string)txtPath.Text;
                        string pathDest = (string)clbServer.Items[i];

                        var subDirectories = GetSubDirectoriesAndFiles(pathFrom);

                        foreach (var item in subDirectories)
                        {
                            if (File.Exists(pathDest + @"\" + item))
                            {
                                File.Copy(pathFrom + @"\" + item, pathDest + item, true);
                            }
                        }
                    }
                }

                MessageBox.Show("Transferência realizada com sucesso");

            }
            else
            {
                MessageBox.Show("Selecione um caminho de origem");
            }
        }

        // Return all subdirectories and files
        private List<string> GetSubDirectoriesAndFiles(string pathFrom)
        {
            string[] subdirectoryEntries = Directory.GetFiles(pathFrom, "*.*", SearchOption.AllDirectories);
            List<string> LoadSubDirs = new List<string>();

            foreach (string subdirectory in subdirectoryEntries)
            {
                LoadSubDirs.Add(subdirectory.Replace(pathFrom, ""));
            }

            return LoadSubDirs;
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            {
                if (!Directory.Exists(PathUtil.Items.Path))
                {
                    var dir = Directory.CreateDirectory(PathUtil.Items.Path);
                    dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {

                    using (var db = new LiteDatabase(@"C:\repli\database.db"))
                    {
                        var col = db.GetCollection<Server>("servers");

                        var server = new Server()
                        {
                            Path = dialog.SelectedPath
                        };

                        col.Insert(server);

                        MessageBox.Show("Adicionado com sucesso");

                        clbServer.Items.Clear();

                        LoadListFolder();
                    }                                       
                }
            }
        }

        private void LoadListFolder()
        {
            if (Directory.Exists(PathUtil.Items.Path))
            {
                using (var db = new LiteDatabase(@"C:\repli\database.db"))
                {
                    var col = db.GetCollection<Server>("servers");
                    var serves = col.Find(Query.All());
                                        
                    for (int i = 0; i < serves.Count(); i++)
                    {
                        clbServer.Items.Add(serves.ElementAt(i).Path);
                        clbServer.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            using (var db = new LiteDatabase(@"C:\repli\database.db"))
            {
                string itemSelected = clbServer.Text;
                
                var col = db.GetCollection<Server>("servers");

                var row = col.FindOne(Query.Contains("Path", itemSelected));
                
                col.Delete(row.Id);

                MessageBox.Show("Deletado com sucesso!");

                clbServer.Items.Clear();

                LoadListFolder();
            }
        }
    }
}

