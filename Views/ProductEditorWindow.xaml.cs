using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskReward.Models;
using TaskReward.ViewModels;

namespace TaskReward.Views
{
    public partial class ProductEditorWindow : Window
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int Stock { get; set; }
        public string SelectedIcon { get; set; }

        public string CostText { get => Cost.ToString(); set { int.TryParse(value, out int v); Cost = v; } }
        public string StockText { get => Stock.ToString(); set { int.TryParse(value, out int v); Stock = v; } }
        public bool ResultOk { get; private set; }

        public ProductEditorWindow()
        {
            InitializeComponent();
            IconGrid.ItemsSource = IconPalette.Icons;
        }

        public void Prepare(ShopProduct p)
        {
            ItemName = p?.Name ?? "";
            Description = p?.Description ?? "";
            Cost = p?.Cost ?? 10;
            Stock = p?.Stock ?? -1;
            SelectedIcon = p?.Icon ?? "🎁";
            DataContext = this;
            if (!string.IsNullOrEmpty(SelectedIcon))
            {
                foreach (var rb in AllRadios())
                    if (rb.Tag?.ToString() == SelectedIcon) { rb.IsChecked = true; break; }
            }
        }

        private void Icon_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null) SelectedIcon = rb.Tag?.ToString();
        }

        private void Num_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^-?[0-9]+$");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemName))
            {
                MessageBox.Show("请填写商品名称", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Cost < 1) Cost = 1;
            if (Stock < -1) Stock = -1;
            if (string.IsNullOrEmpty(SelectedIcon)) SelectedIcon = "🎁";
            ResultOk = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ResultOk = false; DialogResult = false; Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Cancel_Click(sender, e);

        private IEnumerable<RadioButton> AllRadios()
        {
            var list = new List<RadioButton>();
            Walk(IconGrid, list);
            return list;
        }

        private void Walk(DependencyObject parent, List<RadioButton> list)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is RadioButton rb) list.Add(rb);
                Walk(child, list);
            }
        }
    }
}
