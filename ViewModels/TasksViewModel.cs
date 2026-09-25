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
    public class TasksViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;

        public ObservableCollection<TaskItem> Tasks { get; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TemplateItem> Templates { get; } = new ObservableCollection<TemplateItem>();

        public TasksViewModel(MainViewModel main)
        {
            _main = main;
            main.DataChanged += Refresh;
            Refresh();
        }

        public string PointsText => _main.PointsText;
        public string TaskDoneText => _main.Data.Tasks.Count(t => t.Completed) + " / " + _main.Data.Tasks.Count;

        public ICommand AddTaskCommand => new RelayCommand(p => AddTask());
        public ICommand ToggleCommand => new RelayCommand(p => Toggle((TaskItem)p));
        public ICommand EditCommand => new RelayCommand(p => Edit((TaskItem)p));
        public ICommand DeleteCommand => new RelayCommand(p => Delete((TaskItem)p));
        public ICommand AddFromTemplateCommand => new RelayCommand(p => AddFromTemplate((TemplateItem)p));

        public ICommand AddTemplateCommand => new RelayCommand(p => EditTemplate(null));
        public ICommand EditTemplateCommand => new RelayCommand(p => EditTemplate((TemplateItem)p));
        public ICommand DeleteTemplateCommand => new RelayCommand(p => DeleteTemplate((TemplateItem)p));

        private void Refresh()
        {
            Tasks.Clear();
            foreach (var t in _main.Data.Tasks.OrderBy(t => t.Created)) Tasks.Add(t);

            Templates.Clear();
            foreach (var t in _main.Data.Templates) Templates.Add(t);

            OnPropertyChanged(nameof(PointsText));
            OnPropertyChanged(nameof(TaskDoneText));
        }

        private void Toggle(TaskItem t)
        {
            if (t == null) return;
            _main.ToggleTask(t);
        }

        private void AddTask()
        {
            var w = OpenEditor("新建任务", "", "", "", 5);
            if (w != null)
            {
                var task = new TaskItem
                {
                    Id = DataService.NewId(),
                    Name = w.ItemName,
                    Description = w.Description,
                    Icon = w.SelectedIcon,
                    Points = w.Points,
                    Created = DateTime.Now,
                    Completed = false
                };
                _main.Data.Tasks.Add(task);
                _main.NotifyChanged();
            }
        }

        private void Edit(TaskItem t)
        {
            if (t == null) return;
            var w = OpenEditor("编辑任务", t.Name, t.Description, t.Icon, t.Points);
            if (w != null)
            {
                t.Name = w.ItemName;
                t.Description = w.Description;
                t.Icon = w.SelectedIcon;
                t.Points = w.Points;
                _main.NotifyChanged();
            }
        }

        private void Delete(TaskItem t)
        {
            if (t == null) return;
            if (MessageBox.Show("确定删除任务「" + t.Name + "」吗？", "删除确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (t.Completed)
                {
                    _main.Data.Points -= t.Points;
                    if (_main.Data.Points < 0) _main.Data.Points = 0;
                }
                _main.Data.Tasks.Remove(t);
                _main.NotifyChanged();
            }
        }

        private void AddFromTemplate(TemplateItem t)
        {
            if (t == null) return;
            var task = new TaskItem
            {
                Id = DataService.NewId(),
                Name = t.Name,
                Description = t.Description,
                Icon = t.Icon,
                Points = t.Points,
                Created = DateTime.Now,
                FromTemplate = true
            };
            _main.Data.Tasks.Add(task);
            _main.NotifyChanged();
        }

        private void EditTemplate(TemplateItem t)
        {
            var w = OpenEditor(t == null ? "新建模板" : "编辑模板",
                t?.Name ?? "", t?.Description ?? "", t?.Icon ?? "⭐", t?.Points ?? 5);
            if (w != null)
            {
                if (t == null)
                {
                    _main.Data.Templates.Add(new TemplateItem
                    {
                        Id = DataService.NewId(),
                        Name = w.ItemName,
                        Description = w.Description,
                        Icon = w.SelectedIcon,
                        Points = w.Points
                    });
                }
                else
                {
                    t.Name = w.ItemName;
                    t.Description = w.Description;
                    t.Icon = w.SelectedIcon;
                    t.Points = w.Points;
                }
                _main.NotifyChanged();
            }
        }

        private void DeleteTemplate(TemplateItem t)
        {
            if (t == null) return;
            if (MessageBox.Show("确定删除模板「" + t.Name + "」吗？", "删除确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _main.Data.Templates.Remove(t);
                _main.NotifyChanged();
            }
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
