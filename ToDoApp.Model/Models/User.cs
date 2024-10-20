﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    
    public class User : IBaseModel, ICreationInfo, IModificationInfo
    {
        private string _createdBy;
        private DateTime _createdDate;
        private string _modifiedBy;
        private DateTime _modifiedDate;

        [Key]
        public int UserId { get; set; }

        [Column]
        [StringLength(250)]
        public string Username { get; set; }  
        
        [Column]
        [StringLength(250)]
        public string Password { get; set; }
        
        [Column]
        [StringLength(250)]
        public string Fullname { get; set; }

        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }
        
        [Column]
        public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }

        [Column]
        public string ModifiedBy { get => _modifiedBy; set => _modifiedBy = value; }

        public string GetTableName()
        {
            return "`user`";
        }
        public void SetPrimaryKey(dynamic key)
        {
            if (key is int)
            {
                UserId = (int)key;
            }
            else
            {
                UserId = 0;
            }
        }
    }
}
