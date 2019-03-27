using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiceForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Random rnd = new Random();

        private const int THROWS_PER_UPDATE = THROWS_LIMIT/(2*10);
        private const int THROWS_LIMIT = 20000000;

        private List<int> dice = new List<int>();
        private int discardCount = 0;

        // How many times each value was rolled
        private List<Int32> counts = new List<int>();
        private Int64 numThrows = 0;

        private CultureInfo cultureInfo = new CultureInfo("en-GB");

        private int totalMaxDieValue
        {
            get { return dice.Sum(); }
        }

        private int diesToCount
        {
            get { return dice.Count - discardCount; }
        }

        public void updateListView()
        {
            resultsList.Clear();
            resultsList.FullRowSelect = true;

            resultsList.Columns.Add("", 80);
            for (int i = 1; i <= totalMaxDieValue; i++)
            {
                resultsList.Columns.Add(i.ToString(), 50);
            }

            ListViewItem item = new ListViewItem("Probabilities");
            for (int i = 1; i < counts.Count; i++)
            {
                double count = counts[i];
                item.SubItems.Add((count*100 / Convert.ToDouble(numThrows)).ToString("0.00", cultureInfo));
            }

            resultsList.Items.Add(item);

        }

        public void updateData()
        {
            if (numThrows > THROWS_LIMIT)
                return;
            
            for (int i = 0; i < THROWS_PER_UPDATE; i++)
            {
                List<int> throwResults = new List<int>();
                foreach (int sides in dice)
                {
                    throwResults.Add(rnd.Next(1, sides + 1));               
                }

                throwResults.Sort();

                int sum = 0;
                // Take the best results and store them
                for (int j = discardCount; j < throwResults.Count; j++)
                {
                    sum += throwResults[j];
                }

                counts[sum]++;
                numThrows++;
            }

            double progressRatio = Convert.ToDouble(numThrows) / THROWS_LIMIT;
            progressBar.Value = Math.Min(Convert.ToInt32(Math.Ceiling(progressRatio*progressBar.Maximum)), progressBar.Maximum);
        }

        private void ResetStats()
        {
            counts.Clear();
            numThrows = 0;
            for (int i = 0; i <= totalMaxDieValue; i++)
            {
                counts.Add(0);
            }

            progressBar.Value = 0;
        }

        public void OnDiceChange()
        {
            dice.Clear();
            dice.Add(Convert.ToInt32(dieSides_0.Value));
            dice.Add(Convert.ToInt32(dieSides_1.Value));
            dice.Add(Convert.ToInt32(dieSides_2.Value));
            dice.Add(Convert.ToInt32(dieSides_3.Value));

            dice.RemoveAll(e => e == 0);

            // Update discard counter
            int maxDiscardCount = Math.Max(dice.Count - 1, 0);
            discardUpDown.Minimum = 0;
            discardUpDown.Maximum = maxDiscardCount;
            discardUpDown.Value = Math.Min(Math.Max(discardUpDown.Minimum, discardUpDown.Value), discardUpDown.Maximum);

            ResetStats();
        }

        public void onDiscardChange()
        {
            discardCount = Convert.ToInt32(discardUpDown.Value);
            ResetStats();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cb_adv2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void diceBox_Enter(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void dieSides_0_ValueChanged(object sender, EventArgs e)
        {
            OnDiceChange();
        }

        private void dieSides_1_ValueChanged(object sender, EventArgs e)
        {
            OnDiceChange();
        }

        private void dieSides_2_ValueChanged(object sender, EventArgs e)
        {
            OnDiceChange();
        }

        private void dieSides_3_ValueChanged(object sender, EventArgs e)
        {
            OnDiceChange();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Timer timer = new Timer();
            timer.Interval = (100); // 100ms
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            OnDiceChange();
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            if (counts.Count > 0 && dice.Count > 0 && numThrows<THROWS_LIMIT)
            {
                updateData();      
                updateListView();
            }
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            onDiscardChange();
        }

        private void resultsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender != resultsList) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in resultsList.SelectedItems)
            {
                foreach (ListViewItem.ListViewSubItem subitem in item.SubItems)
                {
                    builder.AppendFormat("{0} \t", subitem.Text);
                }
                
            }
                

            Clipboard.SetText(builder.ToString());
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
