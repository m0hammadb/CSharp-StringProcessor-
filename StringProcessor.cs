using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace myStringProcessor
{
    public enum Protocols
    {
        HTTP, HTTPS, FTP, MMS, Unknown
    }
    public enum Methods
    {
        GET, POST
    }

    public class ResponseAnalyzer
    {
        private string _fullres;
        public ResponseAnalyzer(string res)
        {
            _fullres = res;
        }
    }
    public class HeaderGenerator
    {
        public static string Generate(string url, Methods method, Dictionary<string, string> dic = null, string data = "")
        {
            string ret = "";
            if (method == Methods.GET)
            {
                ret = GenerateGET(url, dic);
            }
            return ret;
        }

        private static string GenerateGET(string url, Dictionary<string, string> dic = null)
        {
            string ret = "";
            URLSplitter uspl = new URLSplitter(url);
            ret = string.Format("GET {0} HTTP/1.1\r\n", uspl.Folder);
            ret += "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.9 Safari/537.36\r\n";
            ret += string.Format("Host: {0}\r\n", uspl.Host);
            ret += string.Format("Connection: Close\r\n");

            if (dic != null)
            {
                foreach (KeyValuePair<string, string> k in dic)
                {
                    ret += string.Format("{0}: {1}\r\n", k.Key, k.Value);
                }
            }
            ret += "\r\n";
            return ret;
        }
    }
    public class URLSplitter
    {

        private string _fullurl;
        private string _noprourl;


        public string Host
        {
            get
            {
                string ret = "";
                if (!_noprourl.Contains("/"))
                {
                    //if (!_noprourl.Contains(":"))
                    //{
                    //    ret = _noprourl;
                    //}
                    //else
                    //{
                    ret = _noprourl.Split(':')[0];
                    //}
                }
                else
                {
                    ret = _noprourl.Split('/')[0].Split(':')[0];
                }

                return ret;
            }
        }
        public int Port
        {
            get
            {
                int ret = 80;
                string nofolderurl = _noprourl.Split('/')[0];
                if (nofolderurl.Contains(":"))
                {
                    string[] spl = nofolderurl.Split(':');
                    if (spl.Length > 1)
                    {
                        ret = Convert.ToInt32(spl[1]);
                    }
                }
                return ret;
            }
        }
        public string Folder
        {
            get
            {
                string ret = "/";
                if (_noprourl.Contains("/"))
                {
                    string[] spl = _noprourl.Split(new char[] { '/' }, 2);
                    if (spl.Length > 1)
                        ret = "/" + spl[1];
                }
                return ret;
            }
        }
        public Protocols Protocol
        {
            get
            {
                Protocols ret = Protocols.Unknown;
                string p = StringProcessor.FindEverythingPriorTo(_fullurl, "://").ToLower();
                switch (p)
                {
                    case "http":
                        ret = Protocols.HTTP;
                        break;
                    case "https":
                        ret = Protocols.HTTPS;
                        break;
                    case "ftp":
                        ret = Protocols.FTP;
                        break;
                    case "mms":
                        ret = Protocols.MMS;
                        break;
                }
                return ret;
            }
        }

        public URLSplitter(string url)
        {
            this._fullurl = url;
            if (_fullurl.Contains("://") && _fullurl.Length > 3)
            {
                string[] spl = _fullurl.Split(new string[] { "://" }, StringSplitOptions.RemoveEmptyEntries);
                this._noprourl = spl[1];

            }
            else
            {
                this._noprourl = this._fullurl;
            }
        }


    }
    class StringProcessor
    {
        public static string FindBetween(string sSource, string S1, string S2,int start=0)
        {
            string returnValue = "";
            int startIndex = sSource.IndexOf(S1,start);
            int endIndex = -1;
            if (startIndex >= 0)
            {
                endIndex = sSource.IndexOf(S2, startIndex + S1.Length);
                if (endIndex >= 0)
                {
                    returnValue = sSource.Substring(startIndex + S1.Length, endIndex - (startIndex + S1.Length));
                }
            }
            return returnValue;
        }
        public static string FindEverythingPriorTo(string sSource,string SearchTerm)
        {
            //salam chetori che khabar
            string tmp = "&!@RANDOM_THING&!@" + sSource;
            return FindBetween(tmp, "&!@RANDOM_THING&!@", SearchTerm);
        }
        public static string FindEverthingFrom(string sSource,string SearchTerm)
        {
            string tmp = sSource + "&!@RANDOM_THING&!@";
            return FindBetween(tmp, SearchTerm, "&!@RANDOM_THING&!@");
        }
        public static string[] FindBetweenArray(string sSource, string S1, string S2)
        {
            List<string> myStringList=new List<string>();
            string[] returnValue = null;
            string tmp = sSource;
            string s=StringProcessor.FindBetween(tmp,S1,S2);
            while (s != "")
            {
                myStringList.Add(s);
                tmp = tmp.Replace(S1 + s + S2, "");
                s = StringProcessor.FindBetween(tmp, S1, S2);
            }
            returnValue = myStringList.ToArray();
            return returnValue;
        }
    }
}
