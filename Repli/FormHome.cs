using Repli.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var path = dialog.SelectedPath;

                    var pathFile = Path.Combine(PathUtil.Items.Path, Util.PathUtil.Items.File);

                    if (!Directory.Exists(PathUtil.Items.Path))
                    {
                        var dir = Directory.CreateDirectory(PathUtil.Items.Path);
                        dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                    }

                    if (!File.Exists(PathUtil.Items.Path))
                    {
                        using (StreamWriter sw = File.AppendText(pathFile))
                        {
                            sw.WriteLine(path);
                            MessageBox.Show("Adicionado com sucesso");
                        }
                    }

                    clbServer.Items.Clear();
                    LoadListFolder();
                }
            }
        }

        private void LoadListFolder()
        {
            if (Directory.Exists(PathUtil.Items.Path))
            {
                using (StreamReader stRead = new StreamReader(PathUtil.Items.Path + @"\" + PathUtil.Items.File))
                {
                    while (!stRead.EndOfStream)
                    {
                        clbServer.Items.Add(Convert.ToString(stRead.ReadLine()));
                    }

                    for (int i = 0; i < clbServer.Items.Count; i++)
                    {
                        clbServer.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string tempFile = Path.GetTempFileName();
            string removedPath = "";

            for (int i = 0; i < clbServer.Items.Count; i++)
            {
                if (clbServer.GetSelected(i))
                {
                    removedPath = clbServer.Text;
                }
            }

            using (var sr = new StreamReader(PathUtil.Items.Path + @"\" + PathUtil.Items.File))
            using (var sw = new StreamWriter(tempFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line != removedPath)
                        sw.WriteLine(line);
                }
            }

            var pathFile = Path.Combine(PathUtil.Items.Path, Util.PathUtil.Items.File);

            File.Delete(pathFile);
            File.Move(tempFile, pathFile);
            MessageBox.Show("Deletado com sucesso");

            clbServer.Items.Clear();
            LoadListFolder();
        }
    }
}

