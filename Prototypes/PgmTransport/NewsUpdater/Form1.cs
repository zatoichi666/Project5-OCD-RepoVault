using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LatestNews;

namespace NewsUpdater
{
   public partial class Form1 : Form
   {
      NewsUpdateClient m_updater;

      public Form1()
      {
         InitializeComponent();
         m_updater = new NewsUpdateClient();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         Scoop update = new Scoop();
         update.Description = textBox1.Text;
         update.Originated = DateTime.Now;
         update.Source = textBox2.Text;

         m_updater.Update(update);
      }
   }
}
