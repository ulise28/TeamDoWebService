using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TR = System.Diagnostics.Trace;
using System.Text;

using log4net;


namespace TeamDoWebService.Code.Controllers
{
    public sealed class Trace
    {
        public static string ERROR = "[Service]Error -> ";
        public static string WARNING = "[Service]Warning -> ";
        public static string INFO = "[Service]Info -> ";
        public static string MESSAGE = "[Service]Message -> ";
        public static string ERRINFO = "[Service]ErrInfo -> ";

        private static string indentation = String.Empty;

        #region Logger Setup
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        private static void TraceObject(object value, string category)
        {
            if (value is Exception)
            {
                log.Error(ERRINFO + FormatException((Exception)value));
                TR.TraceError(typeof(Trace).ToString());
                TR.WriteLine(ERRINFO + FormatException((Exception)value));
            }
            else
            {
                string strValue = (new StringBuilder().Append(indentation).Append(value.ToString())).ToString();
                TR.WriteLine(category + " " + strValue);
                log.Info(category + " " + strValue);

            }
        }

        private static string FormatException(Exception exception)
        {
            StringBuilder result = new StringBuilder();

            while (exception != null)
            {
                result.Append(string.Format("{0}", exception.Message)).Append(Environment.NewLine);
                if ((exception.StackTrace != null) && (exception.StackTrace != String.Empty))
                    result.Append(string.Format("  - {0}", exception.StackTrace.Trim())).Append(Environment.NewLine);

                exception = exception.InnerException;
            }

            return result.ToString();
        }

        public static void TraceIndent()
        {
            indentation = indentation.Insert(0, " ");
        }

        public static void TraceUnindent()
        {
            if (indentation != String.Empty)
                indentation = indentation.Remove(0, 1);
        }

        #region ERROR

        public static void TraceError(string format, params object[] args)
        {
            TraceError((object)string.Format(format, args));
        }

        public static void TraceError(object value)
        {
            TraceObject(value, ERROR);
        }

        #endregion

        #region WARNING

        public static void TraceWarning(string format, params object[] args)
        {
            TraceWarning((object)string.Format(format, args));
        }

        public static void TraceWarning(object value)
        {
            TraceObject(value, WARNING);
        }

        #endregion

        #region INFO

        public static void TraceInfo(string format, params object[] args)
        {
            TraceInfo((object)string.Format(format, args));
        }

        public static void TraceInfo(object value)
        {
            TraceObject(value, INFO);
        }

        #endregion

        #region MESSAGE

        public static void TraceMessage(string format, params object[] args)
        {
            TraceMessage((object)string.Format(format, args));
        }

        public static void TraceMessage(object value)
        {
            TraceObject(value, MESSAGE);
        }

        #endregion
    }
}