using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    public interface ICreationInfo
    {
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

    }
}
