using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Extensions
{
    internal static class ExceptionExtensions
    {
        public static void SetInnerException(this Exception exception, Exception innerException)
        {
            Type type = typeof(Exception);
            FieldInfo fieldInfo = type.GetField("_innerException", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo == null) return;

            fieldInfo.SetValue(exception, innerException);
        }
    }
}
