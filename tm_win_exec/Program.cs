using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Args;
using System.Diagnostics;
using System.IO;

namespace Turing
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
           
            
                    Form x = new mtur();
                    Application.EnableVisualStyles();
                    Application.Run(x);
                
            
        }
    }
}
