using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKS_Klient
{
    public class Watcher
    {
        ListManager listManager;
        Thread thread;
        bool isWorking;

        public Watcher(ListManager listManager)
        {
            this.listManager = listManager;
            isWorking = false;
            thread = new Thread(DoWork);
        }

        public void Start()
        {
            if (isWorking)
                return;
            isWorking = true;
            thread.Start();
        }

        private void DoWork()
        {
            while (true)
            {

            }
        }

        public void Stop()
        {
            if (!isWorking)
                return;
            isWorking = false;
            if (thread.IsAlive)
                thread.Abort();
        }
    }
}
