using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper.MVVM.Model
{
    public class UserRecord:UserInfo
    {
        public TimeSpan BestTime { get; set; }
        public int Streak { get; set; }

        public UserRecord(UserInfo user)
        {
            Name = user.Name;
            BestTime = TimeSpan.Zero;
            Streak = 0;
            Level = user.Level;

        }
        public UserRecord()
        {

        }
    }
}
