﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    public class Group : IBaseModel, ICreationInfo
    {
        private DateTime _createdDate { get; set; } // cái dấu _ chỉ dành cho private
        private string _createdBy { get; set; }
        [Key]
        public int GroupId { get; set; }

        [Required]
        [Column]
        public string GroupName {  get; set; }

        [Required]
        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        [Required]
        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; } // lỗi ở đây nhé

        public string GetTableName()
        {
            return "`group`";
        }

        public void SetPrimaryKey(dynamic key)
        {
            if (key is int)
            {
                GroupId = (int)key;
            }
            else
            {
                GroupId = 0;
            }
        }
    }
}
