using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_SQL_App.Models;

namespace WPF_SQL_App
{
    class AppContext
    {
        private static AppContext _appContext;
        public static AppContext Current
        {
            get
            {
                if (_appContext == null)
                {
                    _appContext = new AppContext();
                }
                return _appContext;
            }
        }


        public UserModel CurrentUser { get; set; }


        private AppContext()
        {

        }

    }
}
