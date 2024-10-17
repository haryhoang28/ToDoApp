using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Model;

namespace ToDoApp.BL
{
    public interface IBaseBL
    {
        // Only use for INSERT, UPDATE, DELETE
        // using Task which is "async" to have better performance
        Task<T> InsertOne<T>(T baseModel) where T : IBaseModel, ICreationInfo;

    }
}
