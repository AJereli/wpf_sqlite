using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_SQL_App.Models;

namespace WPF_SQL_App
{
    interface IDBProvider
    {
        Task<UserModel> Login(string username, string password);
        IEnumerable<BoxModel> GetAllBoxes(long userId);
    }
}
