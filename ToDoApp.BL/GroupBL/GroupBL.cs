using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DL;


namespace ToDoApp.BL
{
    public class GroupBL : BaseBL, IGroupBL
    {
        public GroupBL(IGroupDL dl) : base(dl)
        {
        }
    }
}
