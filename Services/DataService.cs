using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using TaskReward.Models;

namespace TaskReward.Services
{
    /// <summary>JSON 持久化服务</summary>
    public static class DataService
    {
        private static string DataPath
        {
            get
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(dir, "data.json");
            }
        }

        private static readonly DataContractJsonSerializer Serializer =
            new DataContractJsonSerializer(typeof(AppData));

        public static AppData Load()
        {
            try
            {
                if (File.Exists(DataPath))
                {
                    string json = File.ReadAllText(DataPath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                        {
                            var data = Serializer.ReadObject(ms) as AppData;
                            if (data != null)
                            {
                                if (data.Tasks == null) data.Tasks = new List<TaskItem>();
                                if (data.Templates == null) data.Templates = new List<TemplateItem>();
                                if (data.DailyTasks == null) data.DailyTasks = new List<DailyTask>();
                                if (data.Products == null) data.Products = new List<ShopProduct>();
                                if (data.Redemptions == null) data.Redemptions = new List<Redemption>();
                                data.Achievements = EnsureAchievements(data);
                                return data;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 损坏则备份并重建
                try
                {
                    if (File.Exists(DataPath))
                        File.Copy(DataPath, DataPath + ".bak", true);
                }
                catch { }
            }
            var seed = CreateSeed();
            Save(seed);
            return seed;
        }

        public static void Save(AppData data)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.WriteObject(ms, data);
                    File.WriteAllText(DataPath, Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
            catch { }
        }

        /// <summary>生成初始化示例数据（模板 + 商城商品 + 成就定义）</summary>
        private static AppData CreateSeed()
        {
            var data = new AppData();
            data.Points = 0;
            data.LifetimeEarned = 0;

            // 预置任务模板
            data.Templates = new List<TemplateItem>
            {
                new TemplateItem { Id = NewId(), Name = "完成作业",  Description = "按时、认真完成当天全部作业", Icon = "📚", Points = 10 },
                new TemplateItem { Id = NewId(), Name = "阅读半小时", Description = "课外阅读至少 30 分钟并记录心得", Icon = "📖", Points = 8 },
                new TemplateItem { Id = NewId(), Name = "整理书桌",   Description = "把书桌整理整齐、物品归位", Icon = "🧹", Points = 5 },
                new TemplateItem { Id = NewId(), Name = "帮忙做家务", Description = "洗碗、倒垃圾、扫地等任选一项", Icon = "🏠", Points = 8 },
                new TemplateItem { Id = NewId(), Name = "锻炼身体",   Description = "户外运动或跳绳 20 分钟以上", Icon = "🏃", Points = 10 },
                new TemplateItem { Id = NewId(), Name = "早睡早起",   Description = "晚上 22:30 前睡觉，早晨按时起床", Icon = "🌙", Points = 6 },
                new TemplateItem { Id = NewId(), Name = "练习才艺",   Description = "练琴 / 画画 / 唱歌等才艺练习 20 分钟", Icon = "🎨", Points = 8 },
                new TemplateItem { Id = NewId(), Name = "复习功课",   Description = "主动复习当天所学内容", Icon = "✏️", Points = 6 },
                new TemplateItem { Id = NewId(), Name = "孝顺暖心",   Description = "主动为家人做一件暖心小事", Icon = "💝", Points = 8 },
                new TemplateItem { Id = NewId(), Name = "节约用眼",   Description = "今天电子屏幕使用时间控制在合理范围", Icon = "👀", Points = 5 },
            };

            // 预置商城商品
            data.Products = new List<ShopProduct>
            {
                new ShopProduct { Id = NewId(), Name = "周末观影券", Description = "兑换一次家庭影院 / 电影院观影机会", Icon = "🎬", Cost = 50, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "零食大礼包", Description = "挑选自己喜欢的健康零食一包", Icon = "🍬", Cost = 30, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "游戏时间 +30 分钟", Description = "额外获得 30 分钟游戏 / 娱乐时间", Icon = "🎮", Cost = 20, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "新文具一件", Description = "兑换一支喜欢的笔或笔记本", Icon = "✏️", Cost = 40, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "自由支配金 10 元", Description = "兑换 10 元自由支配零花钱", Icon = "💰", Cost = 80, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "小小心愿", Description = "向爸爸妈妈兑换一个小心愿（可协商）", Icon = "🎁", Cost = 100, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "周末自然醒", Description = "周末早晨可以睡到自然醒，不被打扰", Icon = "🛏️", Cost = 60, Stock = -1 },
                new ShopProduct { Id = NewId(), Name = "星光奖章", Description = "获得一枚实体星光奖章，挂上荣誉墙", Icon = "🏅", Cost = 150, Stock = -1 },
            };

            data.Achievements = EnsureAchievements(data);
            return data;
        }

        /// <summary>保证成就定义存在（按历史积分阈值）</summary>
        private static List<AchievementDef> EnsureAchievements(AppData data)
        {
            var defs = new List<AchievementDef>
            {
                new AchievementDef { Id = "a1", Name = "初出茅庐",  Description = "历史累计积分达到 10 分",  Icon = "🌱", Threshold = 10,  Tier = "bronze" },
                new AchievementDef { Id = "a2", Name = "小有成就",  Description = "历史累计积分达到 50 分",  Icon = "⭐", Threshold = 50,  Tier = "bronze" },
                new AchievementDef { Id = "a3", Name = "明日之星",  Description = "历史累计积分达到 100 分", Icon = "🌟", Threshold = 100, Tier = "silver" },
                new AchievementDef { Id = "a4", Name = "勤学达人",  Description = "历史累计积分达到 200 分", Icon = "🎓", Threshold = 200, Tier = "silver" },
                new AchievementDef { Id = "a5", Name = "铜牌少年",  Description = "历史累计积分达到 500 分", Icon = "🥉", Threshold = 500, Tier = "bronze" },
                new AchievementDef { Id = "a6", Name = "银牌少年",  Description = "历史累计积分达到 1000 分", Icon = "🥈", Threshold = 1000, Tier = "silver" },
                new AchievementDef { Id = "a7", Name = "金牌少年",  Description = "历史累计积分达到 2000 分", Icon = "🥇", Threshold = 2000, Tier = "gold" },
                new AchievementDef { Id = "a8", Name = "钻石少年",  Description = "历史累计积分达到 5000 分", Icon = "💎", Threshold = 5000, Tier = "diamond" },
                new AchievementDef { Id = "a9", Name = "传奇之星",  Description = "历史累计积分达到 10000 分", Icon = "🏆", Threshold = 10000, Tier = "legend" },
            };

            var current = data.Achievements ?? new List<AchievementDef>();
            foreach (var def in defs)
            {
                if (!current.Exists(a => a.Id == def.Id))
                {
                    current.Add(def);
                }
            }
            current.Sort((x, y) => x.Threshold.CompareTo(y.Threshold));
            return current;
        }

        public static string NewId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
