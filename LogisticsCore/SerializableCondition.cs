using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using static Settings.Setings;

namespace LogisticsCore
{
    [AddINotifyPropertyChangedInterface]
    public class SerializableCondition
    {
        public int Priority { get; set; }  // 优先级
        public string PropertyName { get; set; }  // 订单属性名 (例如 "Weight", "PROVINCENAME")
        public string ComparisonOperator { get; set; }  // 比较操作符 (例如 ">", "<", "==", "InList")
        public object Value { get; set; }  // 比较的值 (例如 "2000", "海南", "青海")
        public EnumLogicType LogicType { get; set; }  // 满足条件时的逻辑类型
    }
}
