using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    public interface IModificationInfo
    {
        DateTime ModifyDate { get; set; }
        string ModifyBy { get; set; }
    }
}
