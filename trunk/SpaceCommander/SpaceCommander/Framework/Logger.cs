namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;

    public class Logger
    {
        #region Private
        static private Logger instance;
        static private bool logFullException;
        #endregion

        #region Public 
        public LogType LogType { get; set; }
        public string LogPath { get; set; }
        #endregion

        /// <summary>
        /// Returns the singleton instance of the Logger
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }

        private Logger()
        {
            this.LogType = LogType.Error;
            logFullException = true;
        }

        private void Write(string sAssembly, string sObjName, string Message, LogType MessageType)
        {
            if (MessageType >= LogType)
            {
                string sOut = string.Format("{4}\r\n{0}: {1}/{2}\r\n{3}", DateTime.Now.ToString(), sAssembly, sObjName, Message, MessageType.ToString());
#if !XBOX360
                System.IO.StreamWriter sw = new System.IO.StreamWriter(LogPath, true, Encoding.ASCII);
                sw.WriteLine(sOut);
                sw.WriteLine();
                sw.Close();
#endif
            }
        }

        #region öffentliche Write Methoden
        public void Write(string Message, LogType MessageType)
        {
            string sAssembly = Assembly.GetCallingAssembly().FullName;
            string sMethod = qualifiedObjName();

            this.Write(sAssembly, sMethod, Message, MessageType);
        }

        public void Write(Exception ex, LogType MessageType)
        {
            string _Exmessage = string.Empty;
            string sAssembly = Assembly.GetCallingAssembly().FullName;
            string sMethod = qualifiedObjName();

            if (logFullException)
                _Exmessage = ex.ToString();
            else
                _Exmessage = ex.Message;

            this.Write(sAssembly, sMethod, _Exmessage, MessageType);
        }

        public void Write(Exception ex, string Message, LogType MessageType)
        {
            string _Exmessage = string.Empty;
            string sAssembly = Assembly.GetCallingAssembly().FullName;
            string sMethod = qualifiedObjName();

            if (logFullException)
                _Exmessage = ex.ToString();
            else
                _Exmessage = ex.Message;

            this.Write(sAssembly, sMethod, string.Format("{0}\r\n{1}", Message, _Exmessage), MessageType);
        }

        #endregion

        private string qualifiedObjName()
        {
            System.Diagnostics.StackFrame sf = new System.Diagnostics.StackTrace(true).GetFrame(2);

            string sMethodName = sf.GetMethod().Name;
            string sClassname = sf.GetMethod().DeclaringType.Name;

            return string.Format("{0}.{1} Line {2}", sClassname, sMethodName, sf.GetFileLineNumber().ToString());
        }
    }

    public enum LogType
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
