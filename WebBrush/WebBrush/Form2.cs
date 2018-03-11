using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using motorStruct;

namespace WebBrush
{
    public partial class Form2 : Form
    {
        motorReader mr = new motorReader();
        recommendReader rr = new recommendReader();
        public Form2()
        {
            InitializeComponent();
            initDgv1();
            init();
        }
        private void initDgv1()
        {
            DataTable dtable = new DataTable("Rock");
            //set columns names
            dtable.Columns.Add("Band", typeof(System.String));
            dtable.Columns.Add("Song", typeof(System.String));
            dtable.Columns.Add("Album", typeof(System.String));
            dtable.Columns.Add("Album2", typeof(System.String));


            //Add Rows
            DataRow drow = dtable.NewRow();
            drow["Band"] = "Iron Maiden";
            drow["Song"] = "Wasted Years";
            drow["Album"] = "Ed Hunter";
            drow["Album2"] = "kG";
            dtable.Rows.Add(drow);

            drow = dtable.NewRow();
            drow["Band"] = "Metallica";
            drow["Song"] = "Enter Sandman";
            drow["Album"] = "Metallica";
            drow["Album2"] = "Tracy";
            dtable.Rows.Add(drow);

            drow = dtable.NewRow();
            drow["Band"] = "Jethro Tull";
            drow["Song"] = "Locomotive Breath";
            drow["Album"] = "Aqualung";
            drow["Album2"] = "Md";
            dtable.Rows.Add(drow);

            drow = dtable.NewRow();
            drow["Band"] = "Mr. Big";
            drow["Song"] = "Seven Impossible Days";
            drow["Album"] = "Japandemonium";
            drow["Album2"] = "Hunter";
            dtable.Rows.Add(drow);

            dataGridView1.DataSource = dtable;
        }
        private void init()
        {
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dataGridView1.DataSource = mr.myMotorList;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.DataSource = rr.recommendList;
            //label1.Text=mr.myMotorList[7].NickName.GetElement(1);
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
        }
    }
}
