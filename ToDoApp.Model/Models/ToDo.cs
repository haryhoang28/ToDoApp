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
    public class ToDo : ICreationInfo, IModificationInfo, IBaseModel
    {
        private DateTime _createdDate {  get; set; }
        private DateTime _modifiedDate { get; set; }
        private string _createdBy { get; set; }
        private string _modifiedBy { get; set; }
        [Key]
        [Column]
        public int ToDoId { get; set; }

        [Required]
        [Column]
        public string ToDoName { get; set; }

        [Required]
        [Column]
        public DateTime StartDate { get; set; }

        [Required]
        [Column]
        public DateTime EndDate { get; set; }

        [Required]
        [Column]
        public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }
        
        [Required]
        [Column]
        public string ModifiedBy { get => _modifiedBy; set => _modifiedBy = value; }

        [Required]
        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        
        [Required]
        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }

        public string GetTableName()
        {
            return "`todo`";
        }

        public void SetPrimaryKey(dynamic key)
        {
            if (key is int)
            {
                ToDoId = (int)key;
            }
            else
            {
                ToDoId = 0;
            }
        }


    }
}
