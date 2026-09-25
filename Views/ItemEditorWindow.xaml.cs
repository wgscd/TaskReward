using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskReward.ViewModels;

namespace TaskReward.Views
{
    /// <summary>通用条目编辑器（任务/模板/每日任务共用），通过 Result 返回数据</summary>
    public partial class ItemEditorWindow : Window
    {
        public IReadOnlyList<string> Icons => IconPalette.Icons;

        public string ItemTitle { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string SelectedIcon { get; set; }
        public string PointsText
        {
            get => Points.ToString();
            set { int.TryParse(value, out int v); Points = v; }
        }

        public bool ResultOk { get; private set; }

        public ItemEditorWindow()
        {
            InitializeComponent();
            IconGrid.ItemsSource = Icons;
        }

        public void Prepare(string title, string name, string desc, string icon, int points)
        {
            ItemTitle = title;
            ItemName = name;
            Description = desc;
            SelectedIcon = icon;
            Points = points;
            DataContext = this;
            // 默认选中当前图标
            if (!string.IsNullOrEmpty(icon))
            {
                foreach (var rb in FindVisual<IEnumerable<RadioButton>>())
                {
                    if (object.Equals(rb.Tag?.ToString(), icon)) { rb.IsChecked = true; break; }
                }
            }
        }

        private void Icon_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null) SelectedIcon = rb.Tag?.ToString();
        }

        private void Points_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemName))
            {
                MessageBox.Show("请填写名称", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Points < 0) Points = 0;
            if (string.IsNullOrEmpty(SelectedIcon)) SelectedIcon = "⭐";
            ResultOk = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ResultOk = false;
            DialogResult = false;
            Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Cancel_Click(sender, e);

        private System.Collections.Generic.IEnumerable<RadioButton> FindVisual<T>() where T : class
        {
            var list = new System.Collections.Generic.List<RadioButton>();
            FindVisual(IconGrid, list);
            return list;
        }

        private void FindVisual(DependencyObject parent, System.Collections.Generic.List<RadioButton> list)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is RadioButton rb) list.Add(rb);
                FindVisual(child, list);
            }
        }
    }
}
