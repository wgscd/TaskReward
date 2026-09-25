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
    public class ShopViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;

        public ObservableCollection<ShopProduct> Products { get; } = new ObservableCollection<ShopProduct>();
        public ObservableCollection<Redemption> History { get; } = new ObservableCollection<Redemption>();

        public ShopViewModel(MainViewModel main)
        {
            _main = main;
            main.DataChanged += Refresh;
            Refresh();
        }

        public string PointsText => _main.PointsText;
        public string SpentText => _main.LifetimeSpent.ToString("N0");

        public ICommand RedeemCommand => new RelayCommand(p => Redeem((ShopProduct)p));
        public ICommand AddCommand => new RelayCommand(p => Edit(null));
        public ICommand EditCommand => new RelayCommand(p => Edit((ShopProduct)p));
        public ICommand DeleteCommand => new RelayCommand(p => Delete((ShopProduct)p));

        private void Refresh()
        {
            Products.Clear();
            foreach (var p in _main.Data.Products) Products.Add(p);

            History.Clear();
            foreach (var r in _main.Data.Redemptions.Take(30)) History.Add(r);

            OnPropertyChanged(nameof(PointsText));
            OnPropertyChanged(nameof(SpentText));
        }

        private void Redeem(ShopProduct p)
        {
            if (p == null) return;
            if (_main.Data.Points < p.Cost)
            {
                MessageBox.Show("积分不足，还差 " + (p.Cost - _main.Data.Points) + " 分", "无法兑换",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show("确定用 " + p.Cost + " 积分兑换「" + p.Name + "」吗？", "兑换确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (!_main.Redeem(p))
                    MessageBox.Show("兑换失败，可能积分不足或库存为 0", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Edit(ShopProduct p)
        {
            var w = new ProductEditorWindow { Owner = Application.Current.MainWindow };
            w.Prepare(p);
            w.ShowDialog();
            if (!w.ResultOk) return;

            if (p == null)
            {
                _main.Data.Products.Add(new ShopProduct
                {
                    Id = DataService.NewId(),
                    Name = w.ItemName,
                    Description = w.Description,
                    Icon = w.SelectedIcon,
                    Cost = w.Cost,
                    Stock = w.Stock
                });
            }
            else
            {
                p.Name = w.ItemName;
                p.Description = w.Description;
                p.Icon = w.SelectedIcon;
                p.Cost = w.Cost;
                p.Stock = w.Stock;
            }
            _main.NotifyChanged();
        }

        private void Delete(ShopProduct p)
        {
            if (p == null) return;
            if (MessageBox.Show("确定删除商品「" + p.Name + "」吗？", "删除确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _main.Data.Products.Remove(p);
                _main.NotifyChanged();
            }
        }
    }
}
