using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogisticsCore;
using Settings;

namespace CenterServerManager.Model
{
    public class CenterServerManagerConfig
    {
        public class LogicConfig
        {
            /// <summary>
            /// 默认物流
            /// </summary>
            public Setings.EnumLogicType DefaultLogic { get; set; }
            /// <summary>
            /// 不称重时或Weight=0时, 默认重量值.
            /// </summary>
            public double DefaultWeightValue { get; set; }
            /// <summary>
            /// 物流分配评估条件组
            /// </summary>
            public List<ConditionGroup> ConditionGroups { get; set; }
        }
    }
}
