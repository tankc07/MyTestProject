using LogisticsCore;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using static Settings.Setings;
using System.Windows.Input;
using PropertyChanged;

namespace CenterServerManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SetConditionViewModel : BindableBase
    {
        public ObservableCollection<ConditionGroup> ConditionGroups { get; set; }
        public ConditionGroup SelectedConditionGroup { get; set; }

        public ObservableCollection<ConditionGroupType> GroupTypeList { get; set; }
        public ObservableCollection<EnumLogicType> LogicTypeList { get; set; }

        public DelegateCommand<object> AddConditionCommand { get; }
        public DelegateCommand<object> DeleteConditionCommand { get; }
        public DelegateCommand<object> SaveConditionGroupsCommand { get; }
        public DelegateCommand<object> LoadConditionGroupsCommand { get; }

        // 构造函数
        public SetConditionViewModel()
        {
            // 初始化数据和命令
            ConditionGroups = new ObservableCollection<ConditionGroup>();
            GroupTypeList = new ObservableCollection<ConditionGroupType>(Enum.GetValues(typeof(ConditionGroupType)).Cast<ConditionGroupType>());
            LogicTypeList = new ObservableCollection<EnumLogicType>(Enum.GetValues(typeof(EnumLogicType)).Cast<EnumLogicType>().Where(e => e != EnumLogicType.Default && e != EnumLogicType.极兔百事));

            AddConditionCommand = new DelegateCommand<object>(AddCondition);
            DeleteConditionCommand = new DelegateCommand<object>(DeleteCondition);
            SaveConditionGroupsCommand = new DelegateCommand<object>(SaveConditionGroups);
            LoadConditionGroupsCommand = new DelegateCommand<object>(LoadConditionGroups);
        }

        private void AddCondition(object parameter)
        {
            if (SelectedConditionGroup != null)
            {
                SelectedConditionGroup.Conditions.Add(new SerializableCondition());
            }
        }

        private void DeleteCondition(object parameter)
        {
            if (SelectedConditionGroup != null && parameter is SerializableCondition condition)
            {
                SelectedConditionGroup.Conditions.Remove(condition);
            }
        }

        private void SaveConditionGroups(object parameter)
        {
            var manager = new LogisticsConditionManager { ConditionGroups = ConditionGroups.ToList() };
            manager.SaveConditionGroupsToJson("conditionGroups.json");
        }

        private void LoadConditionGroups(object parameter)
        {
            var manager = new LogisticsConditionManager();
            manager.LoadConditionGroupsFromJson("conditionGroups.json");
            ConditionGroups.Clear();
            foreach (var group in manager.ConditionGroups)
            {
                ConditionGroups.Add(group);
            }
        }
    }
}