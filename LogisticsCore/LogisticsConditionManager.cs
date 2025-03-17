using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOD.BllMod;
using static Settings.Setings;

namespace LogisticsCore
{
    public class LogisticsConditionManager
    {
        // 添加条件
        public List<ConditionGroup> ConditionGroups = new List<ConditionGroup>();

        // 添加条件组
        public void AddConditionGroup(ConditionGroupType groupType, EnumLogicType logicType, params SerializableCondition[] conditions)
        {
            ConditionGroups.Add(new ConditionGroup
            {
                GroupType = groupType,
                Conditions = conditions.ToList(),
                LogicType = logicType
            });
        }
        // 应用条件组逻辑
        public EnumLogicType ApplyConditions(Order order)
        {
            foreach (var group in ConditionGroups)
            {
                if (EvaluateConditionGroup(order, group))
                {
                    return group.LogicType;
                }
            }

            // 默认逻辑类型
            return EnumLogicType.京东生鲜医药快递;
        }

        // 评估条件组
        private bool EvaluateConditionGroup(Order order, ConditionGroup group)
        {
            bool result = group.GroupType == ConditionGroupType.AND;

            foreach (var condition in group.Conditions)
            {
                bool conditionResult = EvaluateCondition(order, condition);

                if (group.GroupType == ConditionGroupType.AND)
                {
                    result &= conditionResult;
                    if (!result) break;  // 如果是AND，且有条件不满足，则直接返回false
                }
                else if (group.GroupType == ConditionGroupType.OR)
                {
                    result |= conditionResult;
                    if (result) break;  // 如果是OR，且有条件满足，则直接返回true
                }
            }

            return result;
        }

        // 解析并评估条件
        private bool EvaluateCondition(Order order, SerializableCondition condition)
        {
            if (condition == null) throw new System.ArgumentNullException(nameof(condition), @"解析评估条件方法中的评估条件参数不可以为空!");

            var propertyValue = order?.GetType()?.GetProperty(condition.PropertyName)?.GetValue(order);

            switch (condition.ComparisonOperator)
            {
                case ">":
                    return Convert.ToDouble(propertyValue) > Convert.ToDouble(condition.Value);
                case "<":
                    return Convert.ToDouble(propertyValue) < Convert.ToDouble(condition.Value);
                case "==":
                    return propertyValue.ToString() == condition.Value.ToString();
                case "InList":
                    return ((List<string>)condition.Value).Contains(propertyValue?.ToString());
                // 根据需要添加其他操作符
                default:
                    return false;
            }
        }
        // 从JSON加载条件组
        public void LoadConditionGroupsFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                ConditionGroups = JsonConvert.DeserializeObject<List<ConditionGroup>>(json);
            }
        }

        // 保存条件为JSON
        // 保存条件组为JSON
        public void SaveConditionGroupsToJson(string filePath)
        {
            string json = JsonConvert.SerializeObject(ConditionGroups, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

    }

}

