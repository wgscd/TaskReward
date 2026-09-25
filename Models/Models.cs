using System;
using System.Collections.Generic;

namespace TaskReward.Models
{
    /// <summary>任务项</summary>
    public class TaskItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Points { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime Created { get; set; }
        public bool FromTemplate { get; set; }
    }

    /// <summary>任务模板（预置任务项模板）</summary>
    public class TemplateItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Points { get; set; }
    }

    /// <summary>每日任务项（单日）</summary>
    public class DailyTask
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Points { get; set; }
        public string Date { get; set; }          // yyyy-MM-dd
        public bool Completed { get; set; }
    }

    /// <summary>商城商品</summary>
    public class ShopProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Cost { get; set; }
        public int Stock { get; set; }            // -1 表示不限量
    }

    /// <summary>兑换记录</summary>
    public class Redemption
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string Icon { get; set; }
        public int Cost { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>成就定义</summary>
    public class AchievementDef
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public long Threshold { get; set; }       // 历史累计积分阈值
        public string Tier { get; set; }          // 成就等级名称（用于徽章配色）
        public DateTime? UnlockedDate { get; set; }
        public bool IsUnlocked => UnlockedDate != null;
    }

    /// <summary>应用数据根</summary>
    public class AppData
    {
        public long Points { get; set; }            // 当前可用积分
        public long LifetimeEarned { get; set; }    // 历史累计获得积分（成就依据）
        public long LifetimeSpent { get; set; }     // 历史累计消费积分
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public List<TemplateItem> Templates { get; set; } = new List<TemplateItem>();
        public List<DailyTask> DailyTasks { get; set; } = new List<DailyTask>();
        public List<ShopProduct> Products { get; set; } = new List<ShopProduct>();
        public List<Redemption> Redemptions { get; set; } = new List<Redemption>();
        public List<AchievementDef> Achievements { get; set; } = new List<AchievementDef>();
    }
}
