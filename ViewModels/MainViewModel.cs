using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaskReward.Models;
using TaskReward.Services;

namespace TaskReward.ViewModels
{
    /// <summary>主视图模型：持有数据、统一积分记账、成就解锁、保存与全局刷新</summary>
    public class MainViewModel : ViewModelBase
    {
        public AppData Data { get; private set; }

        public DashboardViewModel Dashboard { get; }
        public TasksViewModel Tasks { get; }
        public DailyTasksViewModel Daily { get; }
        public ShopViewModel Shop { get; }
        public AchievementsViewModel Achievements { get; }

        public event Action DataChanged;

        public MainViewModel()
        {
            Data = DataService.Load();

            Dashboard = new DashboardViewModel(this);
            Tasks = new TasksViewModel(this);
            Daily = new DailyTasksViewModel(this);
            Shop = new ShopViewModel(this);
            Achievements = new AchievementsViewModel(this);

            NavCommand = new RelayCommand(p => Navigate((string)p));
            Navigate("home");   // 启动默认进入首页
        }

        public long Points => Data.Points;
        public long LifetimeEarned => Data.LifetimeEarned;
        public long LifetimeSpent => Data.LifetimeSpent;

        public string PointsText => Data.Points.ToString("N0");
        public string LifetimeText => Data.LifetimeEarned.ToString("N0");

        // ================= 导航 =================
        public ICommand NavCommand { get; }
        private object _currentPage;
        private int _selectedIndex;
        public object CurrentPage { get => _currentPage; private set => Set(ref _currentPage, value); }
        public int SelectedIndex { get => _selectedIndex; set => Set(ref _selectedIndex, value); }

        private void Navigate(string key)
        {
            switch (key)
            {
                case "home": CurrentPage = Dashboard; SelectedIndex = 0; break;
                case "tasks": CurrentPage = Tasks; SelectedIndex = 1; break;
                case "daily": CurrentPage = Daily; SelectedIndex = 2; break;
                case "shop": CurrentPage = Shop; SelectedIndex = 3; break;
                case "achieve": CurrentPage = Achievements; SelectedIndex = 4; break;
            }
        }

        // ================= 记账 =================
        /// <summary>完成/取消任务，增减积分</summary>
        public void ToggleTask(TaskItem t)
        {
            if (t.Completed)
            {
                t.Completed = false;
                t.CompletedDate = null;
                Data.Points -= t.Points;
                if (Data.Points < 0) Data.Points = 0;
            }
            else
            {
                t.Completed = true;
                t.CompletedDate = DateTime.Now;
                Data.Points += t.Points;
                Data.LifetimeEarned += t.Points;
            }
            Refresh();
        }

        public void ToggleDaily(DailyTask t)
        {
            if (t.Completed)
            {
                t.Completed = false;
                Data.Points -= t.Points;
                if (Data.Points < 0) Data.Points = 0;
            }
            else
            {
                t.Completed = true;
                Data.Points += t.Points;
                Data.LifetimeEarned += t.Points;
            }
            Refresh();
        }

        /// <summary>兑换：扣减积分并记录，返回是否成功</summary>
        public bool Redeem(ShopProduct p)
        {
            if (p == null) return false;
            if (p.Stock == 0) return false;
            if (Data.Points < p.Cost) return false;

            Data.Points -= p.Cost;
            Data.LifetimeSpent += p.Cost;
            if (p.Stock > 0) p.Stock -= 1;

            Data.Redemptions.Insert(0, new Redemption
            {
                Id = DataService.NewId(),
                ProductName = p.Name,
                Icon = p.Icon,
                Cost = p.Cost,
                Date = DateTime.Now
            });
            Refresh();
            return true;
        }

        /// <summary>校验成就并持久化</summary>
        public void Refresh()
        {
            bool changed = false;
            foreach (var a in Data.Achievements)
            {
                if (a.UnlockedDate == null && Data.LifetimeEarned >= a.Threshold)
                {
                    a.UnlockedDate = DateTime.Now;
                    changed = true;
                }
            }
            if (changed) Save();
            else DataService.Save(Data);
            OnPropertyChanged(nameof(PointsText));
            OnPropertyChanged(nameof(LifetimeText));
            DataChanged?.Invoke();
        }

        public void Save() => DataService.Save(Data);

        /// <summary>删除任务/模板/商品/每日任务后统一刷新</summary>
        public void NotifyChanged()
        {
            Save();
            OnPropertyChanged(nameof(PointsText));
            OnPropertyChanged(nameof(LifetimeText));
            DataChanged?.Invoke();
        }
    }
}
