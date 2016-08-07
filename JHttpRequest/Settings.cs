using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHttpRequest
{
    class Settings
    {
        public static TimeSpan ConnectTimeout = TimeSpan.FromSeconds(15);
        public static double dConnectTimeout = TimeSpan.FromSeconds(15).TotalMilliseconds;
        public static int iConnectTimeout = Convert.ToInt32(TimeSpan.FromSeconds(15).TotalMilliseconds);

        public static TimeSpan ReadTimeout = TimeSpan.FromSeconds(15);
        public static double dReadTimeout = TimeSpan.FromSeconds(15).TotalMilliseconds;
        public static int iReadTimeout = Convert.ToInt32(TimeSpan.FromSeconds(15).TotalMilliseconds);

        public static TimeSpan WriteTimeout = TimeSpan.FromSeconds(15);
        public static double dWriteTimeout = TimeSpan.FromSeconds(15).TotalMilliseconds;
        public static int iWriteTimeout = Convert.ToInt32(TimeSpan.FromSeconds(15).TotalMilliseconds);

        public static void setTimeout(int ConnTimeoutSec=-1, int RdTimeoutSec=-1, int WrTimeoutSec=-1)
        {
            if (ConnTimeoutSec > 0)
            {
                ConnectTimeout = TimeSpan.FromSeconds(ConnTimeoutSec);
                dConnectTimeout = TimeSpan.FromSeconds(ConnTimeoutSec).TotalMilliseconds;
                iConnectTimeout = Convert.ToInt32(TimeSpan.FromSeconds(ConnTimeoutSec).TotalMilliseconds);
            }
            if (RdTimeoutSec > 0)
            {
                ReadTimeout = TimeSpan.FromSeconds(RdTimeoutSec);
                dReadTimeout = TimeSpan.FromSeconds(RdTimeoutSec).TotalMilliseconds;
                iReadTimeout = Convert.ToInt32(TimeSpan.FromSeconds(RdTimeoutSec).TotalMilliseconds);
            }
            if (WrTimeoutSec > 0)
            {
                WriteTimeout = TimeSpan.FromSeconds(WrTimeoutSec);
                dWriteTimeout = TimeSpan.FromSeconds(WrTimeoutSec).TotalMilliseconds;
                iWriteTimeout = Convert.ToInt32(TimeSpan.FromSeconds(WrTimeoutSec).TotalMilliseconds);
            }

        }

    }
}
