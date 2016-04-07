using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKS_Klient
{
    class Worker
    {
        Settings settings;


        public Worker(Settings settings)
        {
            this.settings = settings;
            Thread.Sleep(5000);
            MessageBox.Show("OK!");
        }
    }
}
