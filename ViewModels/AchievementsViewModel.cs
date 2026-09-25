using System.Collections.ObjectModel;
using System.Linq;
using TaskReward.Models;

namespace TaskReward.ViewModels
{
    public class AchievementsViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        public ObservableCollection<AchievementViewModel> Achievements { get; } = new ObservableCollection<AchievementViewModel>();

        public AchievementsViewModel(MainViewModel main)
        {
            _main = main;
            main.DataChanged += Refresh;
            Refresh();
        }

        public string LifetimeText => _main.LifetimeText;
        public long Lifetime => _main.Data.LifetimeEarned;
        public string UnlockedText => _main.Data.Achievements.Count(a => a.UnlockedDate != null) + " / " + _main.Data.Achievements.Count;
        public int UnlockedCount => _main.Data.Achievements.Count(a => a.UnlockedDate != null);

        public double OverallProgress
        {
            get
            {
                if (_main.Data.Achievements.Count == 0) return 0;
                return _main.Data.Achievements.Count(a => a.UnlockedDate != null) * 100.0 / _main.Data.Achievements.Count;
            }
        }

        private void Refresh()
        {
            Achievements.Clear();
            foreach (var a in _main.Data.Achievements.OrderBy(x => x.Threshold))
                Achievements.Add(new AchievementViewModel(a, () => _main.Data.LifetimeEarned));

            OnPropertyChanged(nameof(LifetimeText));
            OnPropertyChanged(nameof(Lifetime));
            OnPropertyChanged(nameof(UnlockedText));
            OnPropertyChanged(nameof(UnlockedCount));
            OnPropertyChanged(nameof(OverallProgress));
        }
    }
}
