using System;
using System.Linq;
using TaskReward.Models;

namespace TaskReward.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        public DashboardViewModel(MainViewModel main)
        {
            _main = main;
            main.DataChanged += Refresh;
            Refresh();
        }

        public string PointsText => _main.PointsText;
        public string LifetimeText => _main.LifetimeText;

        public int TaskCount => _main.Data.Tasks.Count;
        public int TaskDone => _main.Data.Tasks.Count(t => t.Completed);
        public string TaskDoneText => TaskDone + " / " + TaskCount;
        public int ProductCount => _main.Data.Products.Count;
        public int RedemptionCount => _main.Data.Redemptions.Count;
        public int AchUnlocked => _main.Data.Achievements.Count(a => a.UnlockedDate != null);
        public int AchTotal => _main.Data.Achievements.Count;

        public int TodayDone => _main.Data.DailyTasks.Count(t => t.Completed);

        /// <summary>下一个待解锁成就的进度 0-100</summary>
        public double NextAchProgress
        {
            get
            {
                var next = _main.Data.Achievements.FirstOrDefault(a => a.UnlockedDate == null);
                if (next == null) return 100;
                double earned = _main.Data.LifetimeEarned;
                // 计算相对上一已解锁阈值
                double prev = 0;
                foreach (var a in _main.Data.Achievements)
                {
                    if (a.UnlockedDate != null && a.Threshold < next.Threshold) prev = a.Threshold;
                }
                double span = next.Threshold - prev;
                if (span <= 0) return 100;
                return Math.Min(100, Math.Max(0, (earned - prev) / span * 100));
            }
        }

        public string NextAchName
        {
            get
            {
                var next = _main.Data.Achievements.FirstOrDefault(a => a.UnlockedDate == null);
                return next == null ? "全部成就已解锁" : "距离成就「" + next.Name + "」";
            }
        }

        public string NextAchText
        {
            get
            {
                var next = _main.Data.Achievements.FirstOrDefault(a => a.UnlockedDate == null);
                if (next == null) return "太棒了！";
                return "还差 " + Math.Max(0, next.Threshold - _main.Data.LifetimeEarned).ToString("N0") + " 分";
            }
        }

        /// <summary>当前成就等级（最高的已解锁）</summary>
        public string CurrentTier
        {
            get
            {
                var unlocked = _main.Data.Achievements.Where(a => a.UnlockedDate != null).OrderByDescending(a => a.Threshold).FirstOrDefault();
                return unlocked == null ? "bronze" : unlocked.Tier;
            }
        }

        public string CurrentRankName
        {
            get
            {
                var unlocked = _main.Data.Achievements.Where(a => a.UnlockedDate != null).OrderByDescending(a => a.Threshold).FirstOrDefault();
                return unlocked == null ? "初出茅庐" : unlocked.Name;
            }
        }

        private void Refresh()
        {
            OnPropertyChanged(nameof(PointsText));
            OnPropertyChanged(nameof(LifetimeText));
            OnPropertyChanged(nameof(TaskCount));
            OnPropertyChanged(nameof(TaskDone));
            OnPropertyChanged(nameof(TaskDoneText));
            OnPropertyChanged(nameof(ProductCount));
            OnPropertyChanged(nameof(RedemptionCount));
            OnPropertyChanged(nameof(AchUnlocked));
            OnPropertyChanged(nameof(AchTotal));
            OnPropertyChanged(nameof(TodayDone));
            OnPropertyChanged(nameof(NextAchProgress));
            OnPropertyChanged(nameof(NextAchName));
            OnPropertyChanged(nameof(NextAchText));
            OnPropertyChanged(nameof(CurrentTier));
            OnPropertyChanged(nameof(CurrentRankName));
        }
    }
}
