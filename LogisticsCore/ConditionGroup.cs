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
    public class ConditionGroup
    {
        public ConditionGroupType GroupType { get; set; } = ConditionGroupType.AND;  // 组合条件类型
        public List<SerializableCondition> Conditions { get; set; } = new List<SerializableCondition>();  // 条件列表
        public EnumLogicType LogicType { get; set; }  // 满足条件组时的逻辑类型
    }

    public enum ConditionGroupType
    {
        AND,  // 所有条件都满足
        OR    // 只要有一个条件满足
    }
}
