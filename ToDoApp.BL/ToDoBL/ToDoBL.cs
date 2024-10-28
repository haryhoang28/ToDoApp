using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DL;

namespace ToDoApp.BL
{
    public class ToDoBL : BaseBL, IToDoBL
    {
        public ToDoBL(IToDoDL dl) : base(dl)
        {
        }
    }
}
