using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Lock
    {
        private bool isLocked = false;

        public async Task EnterLock()
        {
            await WaitForUnlock();
            isLocked = true;
        }

        public void LeaveLock()
        {
            isLocked = false;
        }

        private static int TimeOutInMiliseconds = 500;

        private async Task WaitForUnlock()
        {
            while (isLocked)
            {
                await Task.Delay(TimeOutInMiliseconds);
            }
        }
    }
}
