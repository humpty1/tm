using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Args;
using System.IO;
using System.Diagnostics;

namespace Turing
{
    class mtur : Form
    {
        PictureBox cat = new PictureBox();
        Bitmap jpg;
        Timer time = new Timer();
        DataGridView data_grid = new DataGridView();
        List<char> position = new List<char>();
        List<char> registers = new List<char>();
        List<string> reg = new List<string>();
        List<string> pos = new List<string>();
        Label result_lab;
        ProcessStartInfo mp;
        MenuStrip cms = new MenuStrip();
       // ToolStripMenu tsmi = new ToolStripMenu(); 
        
        Label m_t = new Label();
        TextBox txt_1 = new TextBox();
        TextBox txt_2 = new TextBox();
        Button but_1 = new Button();
        int i = 0;
        int ps = 1;
        string[] read;
        string[] spl;
        string reg_key;
        string FILE;



        public mtur()
        {
            
           
            but_1.Click += new EventHandler(but_1_Click);

           
            this.Size = new Size(800, 600);
            this.BackColor = Color.Bisque;
            m_t.Location = new Point(2 * this.Width / 5, 20);
            m_t.Size = new Size(180, 40);
            m_t.Font = new Font("Segoe Print", 12, FontStyle.Bold);
            m_t.Text = "Машина Тьюринга";
            Controls.Add(m_t);
            jpg = new Bitmap("2.jpg");
            cat.Size = new Size(30, 30);
            txt_1.Location = new Point(10, 20);
            txt_1.Width = 80;
            txt_2.Location = new Point(10, 50);
            txt_2.Width = 80;
            Controls.Add(txt_1);
            Controls.Add(txt_2);
            Controls.Add(but_1);
            but_1.Location = new Point(100, 20);
            but_1.Text = "OK";
            but_1.BackColor = Color.Transparent;
           
            //tsmi.Text = "Good";
            cms.BackColor = Color.Bisque;
            ToolStripMenuItem tsi = new ToolStripMenuItem("Вибрати файл");
            this.MainMenuStrip = cms;                                                                                                         
            cms.Items.Add(tsi);

            string[] file = {"mul.tm (Множення двох чисел)", "compare.tm(Порівняння двох чисел)", "int_dev2(Число ділиться націло на 2?)"};
            for (int i = 0; i < file.Length; i++)
			{
                tsi.DropDownItems.Add(file[i]);
            }
            Controls.Add(cms);

            tsi.DropDownItems[0].Click += new EventHandler(mtur_Click);
            tsi.DropDownItems[1].Click += new EventHandler(mtur1_Click);
            tsi.DropDownItems[2].Click += new EventHandler(mtur2_Click);


        }

        void mtur_Click(object sender, EventArgs e)
        {
            FILE = "mul.tm";
            txt_2.Visible = true;

        }
        void mtur1_Click(object sender, EventArgs e)
        {
            FILE = "compare.tm";
            txt_2.Visible = true;

        }
        void mtur2_Click(object sender, EventArgs e)
        {
            FILE = "int_dev2.tm";
            txt_2.Visible = false;

        }

        void cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        void but_1_Click(object sender, EventArgs e)
        {
            
                
                try
                {
                    int registr_1 = 0;
                    int registr_2 = 0;
                    reg_key = "L";

                    if (FILE == "mul.tm" || FILE == "compare.tm")
                    {
                        if ((registr_1 = Convert.ToInt32(txt_1.Text)) >= 0 && (registr_2 = Convert.ToInt32(txt_2.Text)) >= 0)
                        {

                            for (int i = 0; i < registr_1; i++)
                                reg_key += "|";
                            reg_key += "#";
                            for (int i = 0; i < registr_2; i++)
                                reg_key += "|";
                            reg_key += "L";


                        }
                    }
                    if (FILE == "int_dev2.tm") 
                    {
                        reg_key = "L";
                        if ((registr_1 = Convert.ToInt32(txt_1.Text)) >= 0 )
                        {

                            for (int i = 0; i < registr_1; i++)
                                reg_key += "|";
                            reg_key += "L";


                        }
                    }

                    mp = new ProcessStartInfo("tm1.exe");
                    mp.RedirectStandardError = true;
                    mp.UseShellExecute = false;
                    mp.Arguments = "-v " + "-b L -r " + reg_key + " -f " + FILE + "";

                    Console.Error.WriteLine(" arguments line '{0}'",  mp.Arguments);
                    string wrt = "t2.txt";
                    StreamWriter sw = new StreamWriter(wrt);
                    Process proc = Process.Start(mp);
                    var error = proc.StandardError.ReadToEnd();
                    if (error.Length > 0)
                    {
                        // MessageBox.Show(error);
                        sw.WriteLine(error);

                    }
                    sw.Close();
                    read = File.ReadAllLines(wrt);

                   // time.Interval = 100;
                    time.Start();
                    for (int i = 0; i < read.Length; i++)
                    {
                        spl = read[i].Split(new Char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (spl.Length > 1)
                        {
                            if (spl[0][0] == 'H')
                                pos.Add(spl[1]);

                            if (spl[0][0] == 'R')
                                reg.Add(spl[1]);
                        }
                    }

                    time.Tick -= new EventHandler(time_Tick);
                    time.Tick += new EventHandler(time_Tick);

                }
                catch 
                {
                    MessageBox.Show("Введіть додатне число!");

                }

        }
        int tim = 0;
        void time_Tick(object sender, EventArgs e)
        {
            try
            {
                AutoScroll = true;

                Console.Error.WriteLine(" pos.Count/req.Count/i: {0}/{1}/{2} ", pos.Count, reg.Count, i);

                tim++;
                Console.WriteLine(tim);
                if (i < reg.Count)
                {
                    Controls.Add(cat);
                    Controls.Add(data_grid);

                    List<string> str = new List<string>();
                    registers = new List<char>();
                    position = new List<char>();
                    cat.Image = (Image)new Bitmap(jpg, cat.Size);
                    data_grid.RowHeadersVisible = false;
                    data_grid.AllowUserToAddRows = false;
                    data_grid.AllowUserToDeleteRows = false;
                    data_grid.ScrollBars = ScrollBars.None;
                    data_grid.Rows.Clear();

                    Console.Error.WriteLine(" pos.Count/req.Count: {0}/{1} ", pos.Count, reg.Count);

                    Console.WriteLine();
                    Console.WriteLine(pos[i] + " ");
                    Console.WriteLine(reg[i] + " ");

                    for (int j = 0; j < pos[i].Length; j++)
                        position.Add(pos[i][j]);

                    for (int j = 0; j < reg[i].Length; j++)
                        registers.Add(reg[i][j]);

                    for (int r = 0; r < registers.Count; r++)
                        if (registers[r] != ' ')
                            str.Add(Convert.ToString(registers[r]));

                    data_grid.ColumnCount = str.Count;

                    for (ps = 0; ps < position.Count; ps++)
                        if (position[ps] != ' ')
                            break;

                    for (int col = 0; col < data_grid.ColumnCount; col++)
                        data_grid.Columns[col].Width = 40;

                    data_grid.Size = new Size(data_grid.Columns[0].Width * data_grid.Columns.Count, 45);
                    data_grid.Location = new Point(this.Width / 2 - data_grid.Width / 2, this.Height / 3);
                    cat.Location = new Point(data_grid.Columns[0].Width * (ps - 1) + this.Width / 2 - data_grid.Width / 2 + 10, this.Height / 3 + 50);

                    data_grid.Rows.Add(str.ToArray());

                    i++;
                }

                else
                {

                    Console.Error.WriteLine(" else/ reg.count {0}:", reg.Count);

                    time.Stop();
                    result_lab = new Label();
                    result_lab.Location = new Point(200,400);
                    result_lab.Width = 150;
                    result_lab.Font = new Font("Segoe Print", 12);
                    int result = 0;
                    if (i> 0 ){
                      string end = reg[i - 1];
                      for (int j = 0; j < reg[i - 1].Length; j++)
                      {
                          if (reg[i - 1][j] == '|')
                              result++;
                      }
                      result_lab.Text = "Результат = " + result;
                    }
                    else {
                      result_lab.Text = "Результат отсутствует ";
		    }
                    Controls.Add(result_lab);
                    Console.WriteLine("Result " + result);
                }

                position.Clear();
                registers.Clear();
            }
            catch (Exception ex){
                Console.WriteLine("Error:{0}/\n{1}/\n{2}/\n {3}"
                                       , ex.Message
                                         , ex.StackTrace
                                           , ex.TargetSite 
                                             , ex.Data    );

                MessageBox.Show("Ви не обрали файл або не ввели число(числа)!");
            }
        }
    }
}
