using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace LatestNews
{
   public partial class Form1 : Form
   {
      List<Scoop> list = new List<Scoop>();

      public Form1()
      {
         InitializeComponent();
         //dataGridView1.DataSource = list;
         InitService();
      }

      private void InitService()
      {
         NewsUpdater.Updated += new EventHandler<UpdateEventArgs>(NewsUpdater_Updated);
         ServiceHost host = new ServiceHost(typeof(NewsUpdater));
         host.Open();
      }

      void NewsUpdater_Updated(object sender, UpdateEventArgs e)
      {
         //list.Add(e.Update);
         dataGridView1.Rows.Add(new object[] { e.Update.Originated, e.Update.Source, e.Update.Description});
         dataGridView1.Refresh();
      }
   }
}
