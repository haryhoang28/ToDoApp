﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    public interface IModificationInfo
    {
        DateTime ModifiedDate { get; set; }
        string ModifiedBy { get; set; }
    }
}
