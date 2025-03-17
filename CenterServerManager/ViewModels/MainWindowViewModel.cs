using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CenterServerManager.Common;
using LogisticsCore;
using Prism.Commands;
using Prism.Mvvm;
using PropertyChanged;
using Settings;

namespace CenterServerManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        public List<Setings.EnumLogicType> LogicTypes { get; set; } = EnumHelper<Setings.EnumLogicType>.ToList()
            .Where(e => e != Setings.EnumLogicType.Default && e != Setings.EnumLogicType.极兔百事).ToList();
        public Setings.EnumLogicType SelectedLogic { get; set; } = Setings.EnumLogicType.京东生鲜医药快递;
        public string DefaultWeight { get; set; } = 4.ToString();

        public ObservableCollection<ConditionGroup> ConditionGroups { get; set; }
        public ConditionGroup SelectedConditionGroup { get; set; }

        public ObservableCollection<ConditionGroupType> GroupTypeList { get; set; }
        public ObservableCollection<Setings.EnumLogicType> LogicTypeList { get; set; }

        public DelegateCommand<object> AddConditionCommand { get; }
        public DelegateCommand<object> DeleteConditionCommand { get; }
        public DelegateCommand<object> SaveConditionGroupsCommand { get; }
        public DelegateCommand<object> LoadConditionGroupsCommand { get; }

        // 构造函数 GroupTypeList GroupType LogicTypeList
        public MainWindowViewModel()
        {
            // 初始化数据和命令
            ConditionGroups = new ObservableCollection<ConditionGroup>();
            GroupTypeList = new ObservableCollection<ConditionGroupType>(Enum.GetValues(typeof(ConditionGroupType)).Cast<ConditionGroupType>().ToList());
            LogicTypeList = new ObservableCollection<Setings.EnumLogicType>(Enum.GetValues(typeof(Setings.EnumLogicType)).Cast<Setings.EnumLogicType>()
                .Where(e => e != Setings.EnumLogicType.Default && e != Setings.EnumLogicType.极兔百事).ToList());

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
