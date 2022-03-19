using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CollectionSynchronization
{
    
    public partial class MainWindow : Window
    {
        private ObservableCollection<TreeItem> _items = new ObservableCollection<TreeItem>();

        bool _isStop = false;

        public MainWindow()
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(this._items, new object());
           
            CTreeView.ItemsSource = _items;
        }

        private void StartAddItemCommand(object sender, RoutedEventArgs e)
        { 
            // 別スレッドからコレクションにアイテムを追加する
            Task.Run(async () =>
            {
                var cnt = 0;//追加速度を制御するためのカウンタ
                while (!_isStop)
                {
                    cnt++;
                    var nowStr = DateTime.Now.ToString();
                    var treeItem = new TreeItem(nowStr);
                    treeItem.Items.Add(new TreeItem(nowStr + ": child1"));
                    treeItem.Items.Add(new TreeItem(nowStr + ": child2"));

                    if (cnt % 50000 == 0) _items.Add(treeItem);
                }

                _isStop = false;
            });
        }

        private void StopAddItemCommand(object sender, RoutedEventArgs e)
        {
            _isStop = true;
        }
    }

    public sealed class TreeItem
    {
        public TreeItem(string name)
        {
            Name = name;
            BindingOperations.EnableCollectionSynchronization(this.Items, new object());
        }


        public string Name { get; set; }
        public ObservableCollection<TreeItem> Items { get; set; } = new ObservableCollection<TreeItem>();
    }

}
