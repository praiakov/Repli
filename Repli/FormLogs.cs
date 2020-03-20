using LiteDB;
using Repli.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repli
{
    public partial class FormLogs : Form
    {
        public FormLogs()
        {
            InitializeComponent();
        }

        private void FormLogs_Load(object sender, EventArgs e)
        {
            using var db = new LiteDatabase(PathUtil.Items.DataBase);
            
                List<string> listLogs = new List<string>();

                var col = db.GetCollection<Logs>("logs");

                var logs = col.Find(Query.All());

                foreach (var item in logs)
                {
                    listLogs.Add(item.Hostname + " " + item.Data + " " + item.Path);
                }

                for (int i = 0; i < listLogs.Count; i++)
                {
                    txtLogs.Text = string.Join(",", listLogs[i] + Environment.NewLine + txtLogs.Text);
                }
        }
    }
}
