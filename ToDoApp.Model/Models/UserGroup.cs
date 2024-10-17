using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Model
{
    public class UserGroup : ICreationInfo, IBaseModel
    {
        private DateTime _createdDate {  get; set; }
        private string _createdBy { get; set; }

        [Required]
        
        public int UserId { get; set; }

        [Required]
        [Column]
        public string GroupId { get; set; }

        [Required]
        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        [Required]
        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }

        public string GetTableName()
        {
            return "`usergroup`";
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
