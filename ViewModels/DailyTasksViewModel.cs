using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TaskReward.Models;
using TaskReward.Services;
using TaskReward.Views;

namespace TaskReward.ViewModels
{
    public class DailyTasksViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private DateTime _selectedDate;

        public ObservableCollection<DailyTask> Items { get; } = new ObservableCollection<DailyTask>();

        public DailyTasksViewModel(MainViewModel main)
        {
            _main = main;
            _selectedDate = DateTime.Today;
            main.DataChanged += Refresh;
            Refresh();
        }

        public string DateText => _selectedDate.ToString("yyyy年M月d日") + (_selectedDate == DateTime.Today ? "  ·  今天" : "");
        public bool IsToday => _selectedDate == DateTime.Today;
        public string DoneText
        {
            get
            {
                int done = Items.Count(i => i.Completed);
                int total = Items.Count;
                return total == 0 ? "今天还没有安排任务" : "已完成 " + done + " / " + total;
            }
        }
        public string PointsText => _main.PointsText;

        public ICommand PrevCommand => new RelayCommand(p => Move(-1));
        public ICommand NextCommand => new RelayCommand(p => Move(1));
        public ICommand TodayCommand => new RelayCommand(p => { _selectedDate = DateTime.Today; Refresh(); });
        public ICommand AddCommand => new RelayCommand(p => Add());
        public ICommand ToggleCommand => new RelayCommand(p => Toggle((DailyTask)p));
        public ICommand EditCommand => new RelayCommand(p => Edit((DailyTask)p));
        public ICommand DeleteCommand => new RelayCommand(p => Delete((DailyTask)p));

        private void Move(int delta)
        {
            _selectedDate = _selectedDate.AddDays(delta);
            Refresh();
        }

        private void Refresh()
        {
            Items.Clear();
            string key = _selectedDate.ToString("yyyy-MM-dd");
            foreach (var t in _main.Data.DailyTasks.Where(t => t.Date == key))
                Items.Add(t);

            OnPropertyChanged(nameof(DateText));
            OnPropertyChanged(nameof(IsToday));
            OnPropertyChanged(nameof(DoneText));
            OnPropertyChanged(nameof(PointsText));
        }

        private void Add()
        {
            var w = OpenEditor("新建每日任务", "", "", "⭐", 5);
            if (w != null)
            {
                _main.Data.DailyTasks.Add(new DailyTask
                {
                    Id = DataService.NewId(),
                    Name = w.ItemName,
                    Description = w.Description,
                    Icon = w.SelectedIcon,
                    Points = w.Points,
                    Date = _selectedDate.ToString("yyyy-MM-dd"),
                    Completed = false
                });
                _main.NotifyChanged();
            }
        }

        private void Edit(DailyTask t)
        {
            if (t == null) return;
            var w = OpenEditor("编辑每日任务", t.Name, t.Description, t.Icon, t.Points);
            if (w != null)
            {
                t.Name = w.ItemName;
                t.Description = w.Description;
                t.Icon = w.SelectedIcon;
                t.Points = w.Points;
                _main.NotifyChanged();
            }
        }

        private void Delete(DailyTask t)
        {
            if (t == null) return;
            if (MessageBox.Show("确定删除「" + t.Name + "」吗？", "删除确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (t.Completed)
                {
                    _main.Data.Points -= t.Points;
                    if (_main.Data.Points < 0) _main.Data.Points = 0;
                }
                _main.Data.DailyTasks.Remove(t);
                _main.NotifyChanged();
            }
        }

        private void Toggle(DailyTask t)
        {
            if (t == null) return;
            _main.ToggleDaily(t);
        }

        private ItemEditorWindow OpenEditor(string title, string name, string desc, string icon, int points)
        {
            var w = new ItemEditorWindow { Owner = Application.Current.MainWindow };
            w.Prepare(title, name, desc, icon, points);
            w.ShowDialog();
            return w.ResultOk ? w : null;
        }
    }
}
