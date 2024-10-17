using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DL;


namespace ToDoApp.BL
{
    public class UserBL : BaseBL, IUserBL
    {
        public UserBL(IUserDL dl) : base(dl)
        {
        }
    }
}

