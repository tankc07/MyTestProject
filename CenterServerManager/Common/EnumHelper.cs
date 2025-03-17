using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterServerManager.Common
{
    public class EnumHelper<T>
    {
        public static List<T> ToList()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
