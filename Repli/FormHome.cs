using LiteDB;
using Repli.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
            FolderBrowserDialog fbdFromFile = new FolderBrowserDialog();

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

                                using var db = new LiteDatabase(PathUtil.Items.DataBase);
                                
                                    var col = db.GetCollection<Logs>("logs");

                                    var log = new Logs()
                                    {
                                        Path = pathFrom + item,
                                        Data = DateTime.Now,
                                        Hostname = Environment.MachineName
                                    };

                                    col.Insert(log);
                                
                            }

                            timerBar.Start();

                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um caminho de origem");
            }
        }

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

                    using var db = new LiteDatabase(PathUtil.Items.DataBase);

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

        private void LoadListFolder()
        {
            if (Directory.Exists(PathUtil.Items.Path))
            {
                using var db = new LiteDatabase(PathUtil.Items.DataBase);
                
                    var col = db.GetCollection<Server>("servers");
                    var serves = col.Find(Query.All());

                    for (int i = 0; i < serves.Count(); i++)
                    {
                        clbServer.Items.Add(serves.ElementAt(i).Path);
                        clbServer.SetItemChecked(i, true);
                    }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            using var db = new LiteDatabase(PathUtil.Items.DataBase);
            
                string itemSelected = clbServer.Text;

                var col = db.GetCollection<Server>("servers");

                var row = col.FindOne(Query.Contains("Path", itemSelected));

                col.Delete(row.Id);

                MessageBox.Show("Deletado com sucesso!");

                clbServer.Items.Clear();

                LoadListFolder();
        }

        private void timerBar_Tick(object sender, EventArgs e)
        {
            int i = 25;
            progressBar.Increment(10 + i);
            lbBar.Text = progressBar.Value.ToString() + "%";

            if (lbBar.Text.Contains("100"))
            {
                timerBar.Stop();
                Thread.Sleep(10);
                MessageBox.Show("Copiado com sucesso!");

                progressBar.Value = 0;
                lbBar.Text = "0%";
            }
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            Form frmLogs = new FormLogs();
            frmLogs.ShowDialog();
        }
    }
}

