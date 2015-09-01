using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace smpBayerRegCode
{
    public class userinfo
    {

        private string strUserName;

        private string strPassword;

        public string UserName
        {

            get { return strUserName; }

            set { strUserName = value; }

        }

        public string Password
        {

            get { return strPassword; }

            set { strPassword = value; }

        }

        public userinfo()
        {

            strUserName = "";

            strPassword = "";

        }

    }
}
