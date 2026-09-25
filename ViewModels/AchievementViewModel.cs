using System;
using TaskReward.Models;

namespace TaskReward.ViewModels
{
    /// <summary>成就卡片包装：提供只读进度等展示属性</summary>
    public class AchievementViewModel : ViewModelBase
    {
        private readonly Func<long> _lifetime;
        public AchievementDef Def { get; }

        public AchievementViewModel(AchievementDef def, Func<long> lifetime)
        {
            Def = def;
            _lifetime = lifetime;
        }

        public string Icon => Def.Icon;
        public string Name => Def.Name;
        public string Description => Def.Description;
        public string Tier => Def.Tier;
        public bool IsUnlocked => Def.UnlockedDate != null;

        public double Progress
        {
            get
            {
                if (Def.Threshold <= 0) return 100;
                return Math.Min(100, Math.Max(0, _lifetime() * 100.0 / Def.Threshold));
            }
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(IsUnlocked));
        }
    }
}
