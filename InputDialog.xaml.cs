using System.Windows;

namespace GameEditor
{
    public partial class InputDialog : Window
    {
        public int WidthValue { get; private set; }
        public int HeightValue { get; private set; }

        public InputDialog()
        {
            InitializeComponent();
        }

        // 确定按钮点击事件
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入是否为整数
            if (int.TryParse(WidthTextBox.Text, out int width) && int.TryParse(HeightTextBox.Text, out int height))
            {
                WidthValue = width;
                HeightValue = height;
                DialogResult = true; // 关闭窗口并返回 true
                Close();
            }
            else
            {
                MessageBox.Show("请输入有效的整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}