using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RollerSurvivor.Scripts;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace GameEditor
{
    public partial class MainWindow : Window
    {
        private MapBlock _mapBlock = null;
        private bool _isDragging = false;
        private Point _startPoint;
        private double _cellSize = 30;

        public MainWindow()
        {
            InitializeComponent();


            LoadFileList();

            // 绘制 MapBlock
            DrawMapBlock();
        }

        private void DrawMapBlock()
        {
            if (_mapBlock is null)
            {
                return;
            }
            for (int x = 0; x < _mapBlock.Width; x++)
            {
                for (int y = 0; y < _mapBlock.Height; y++)
                {
                    // 创建矩形
                    Rectangle rect = new Rectangle
                    {
                        Width = _cellSize,
                        Height = _cellSize,
                        Fill = new SolidColorBrush(_mapBlock.Map[x, y] ? Colors.Gray : Colors.Black) { Opacity = 0.5 }, // 设置透明度
                        Stroke = Brushes.White,
                        StrokeThickness = 1
                    };

                    // 设置矩形的位置
                    Canvas.SetLeft(rect, x * _cellSize);
                    Canvas.SetTop(rect, y * _cellSize);

                    // 为矩形添加点击事件
                    int currentX = x; // 避免闭包问题
                    int currentY = y;
                    rect.MouseDown += (sender, e) => OnRectangleClicked(currentX, currentY);

                    // 将矩形添加到 Canvas
                    MapCanvas.Children.Add(rect);
                }
            }
        }

        #region 鼠标操作

        /// <summary>
        /// 鼠标拖动接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            MapCanvas_MouseMove1(sender, e);
            MapCanvas_MouseMove2(sender, e);
        }
        
        private void OnRectangleClicked(int x, int y)
        {
            // 检查索引是否越界
            if (x >= 0 && x < _mapBlock.Width && y >= 0 && y < _mapBlock.Height)
            {
                if (_MapSelected)
                {
                    _MapSelected = false;
                    if (_SelectMap.Contains(x * _mapBlock.Width + y))
                    {
                        foreach (var index in _SelectMap)
                        {
                            var mapBlockWidth = index/_mapBlock.Width;
                            var blockWidth = index%_mapBlock.Width;
                            _mapBlock.Map[mapBlockWidth, blockWidth] = !_mapBlock.Map[mapBlockWidth, blockWidth];
                        }
                    }

                    if (_SelectMap.Any())
                    {
                        RefreshAllMaoBlock();
                        _SelectMap.Clear();
                    }
                }
                else
                {
                    _mapBlock.Map[x, y] = !_mapBlock.Map[x, y];
                    RefreshMapBlock(x, y);
                }

                // 刷新显示
            }
            else
            {
                MessageBox.Show($"Invalid index: x={x}, y={y}");
            }
        }

        // 鼠标按下时开始拖动
        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _startPoint = e.GetPosition(MapCanvas);
                MapCanvas.CaptureMouse();
            }
        }

        // 鼠标移动时拖动 Canvas
        private void MapCanvas_MouseMove1(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = e.GetPosition(MapCanvas);
                double offsetX = currentPoint.X - _startPoint.X;
                double offsetY = currentPoint.Y - _startPoint.Y;

                // 移动 Canvas 中的所有子元素
                foreach (var child in MapCanvas.Children)
                {
                    if (child is UIElement element)
                    {
                        Canvas.SetLeft(element, Canvas.GetLeft(element) + offsetX);
                        Canvas.SetTop(element, Canvas.GetTop(element) + offsetY);
                    }
                }

                _startPoint = currentPoint;
            }
        }

        // 鼠标释放时停止拖动
        private void MapCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                MapCanvas.ReleaseMouseCapture();
            }
        }
        
        private void MapCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取当前的缩放比例
            double scale = e.Delta > 0 ? 1.1 : 0.9; // 滚轮向上放大，向下缩小

            // 获取鼠标相对于 Canvas 的位置
            Point mousePosition = e.GetPosition(MapCanvas);

            // 遍历 Canvas 中的所有子元素
            foreach (var child in MapCanvas.Children)
            {
                if (child is UIElement element)
                {
                    // 计算元素的当前位置
                    double left = Canvas.GetLeft(element);
                    double top = Canvas.GetTop(element);

                    // 计算缩放后的新位置
                    double newLeft = mousePosition.X + (left - mousePosition.X) * scale;
                    double newTop = mousePosition.Y + (top - mousePosition.Y) * scale;

                    // 设置元素的新位置
                    Canvas.SetLeft(element, newLeft);
                    Canvas.SetTop(element, newTop);

                    // 缩放元素的大小
                    if (element is FrameworkElement frameworkElement)
                    {
                        frameworkElement.Width *= scale;
                        frameworkElement.Height *= scale;
                    }
                }
            }
        }

        #region 批量处理

        private bool _isRightDragging = false;
        private Point _dragStartPoint;
        private Rectangle _selectionRect;
        private HashSet<int> _SelectMap = new HashSet<int>();
        private bool _MapSelected;
        
        // 右键按下时开始拖动
        private void MapCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isRightDragging = true;
            _dragStartPoint = e.GetPosition(MapCanvas);

            // 创建半透明框选矩形
            _selectionRect = new Rectangle
            {
                Stroke = Brushes.Orange,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.LightBlue) { Opacity = 0.3 }
            };

            // 设置矩形初始位置
            Canvas.SetLeft(_selectionRect, _dragStartPoint.X);
            Canvas.SetTop(_selectionRect, _dragStartPoint.Y);
            MapCanvas.Children.Add(_selectionRect);
        }

        // 拖动过程中更新框选矩形
        private void MapCanvas_MouseMove2(object sender, MouseEventArgs e)
        {
            if (!_isRightDragging) return;

            Point currentPoint = e.GetPosition(MapCanvas);

            // 计算矩形的位置和尺寸
            double x = Math.Min(_dragStartPoint.X, currentPoint.X);
            double y = Math.Min(_dragStartPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - _dragStartPoint.X);
            double height = Math.Abs(currentPoint.Y - _dragStartPoint.Y);

            // 更新框选矩形
            Canvas.SetLeft(_selectionRect, x);
            Canvas.SetTop(_selectionRect, y);
            _selectionRect.Width = width;
            _selectionRect.Height = height;
        }

        // 右键释放时结束拖动并处理选中的块
        private void MapCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isRightDragging) return;

            _isRightDragging = false;
            MapCanvas.Children.Remove(_selectionRect); // 移除框选矩形

            // 计算选中的区域
            Rect selectionArea = new Rect(
                Math.Min(_dragStartPoint.X, e.GetPosition(MapCanvas).X),
                Math.Min(_dragStartPoint.Y, e.GetPosition(MapCanvas).Y),
                Math.Abs(e.GetPosition(MapCanvas).X - _dragStartPoint.X),
                Math.Abs(e.GetPosition(MapCanvas).Y - _dragStartPoint.Y)
            );

            // 遍历所有地图块，判断是否被选中
            var index = 0;
            foreach (var child in MapCanvas.Children.OfType<Rectangle>())
            {
                // 跳过框选矩形本身
                if (child == _selectionRect) continue;

                // 获取地图块的位置和范围
                Rect tileRect = new Rect(
                    Canvas.GetLeft(child),
                    Canvas.GetTop(child),
                    child.Width,
                    child.Height
                );

                // 判断是否与框选区域相交
                if (selectionArea.IntersectsWith(tileRect))
                {
                    child.Fill = new SolidColorBrush(Colors.Green) { Opacity = 0.5 };

                    _SelectMap.Add(index);
                    _MapSelected = true;
                }

                index++;
            }

            
        }

        #endregion

        #region 刷新地图块

        private void RefreshMapBlock(int x, int y)
        {
            int index = x * _mapBlock.Height + y;
            if (index >= 0 && index < MapCanvas.Children.Count)
            {
                if (MapCanvas.Children[index] is Rectangle rect)
                {
                    rect.Fill = new SolidColorBrush(_mapBlock.Map[x, y] ? Colors.Gray : Colors.Black) { Opacity = 0.5 };
                }
            }
        }

        private void RefreshAllMaoBlock()
        {
            for (int i = 0; i < MapCanvas.Children.Count; i++)
            {
                if (MapCanvas.Children[i] is Rectangle rect)
                {
                    rect.Fill = new SolidColorBrush(_mapBlock.Map[i/_mapBlock.Height, i%_mapBlock.Height] ? Colors.Gray : Colors.Black) { Opacity = 0.5 };
                }
            }
        }

        #endregion
        
        #endregion

        #region 文件系统

        private string _directoryPath = @"E:\MaskMapAsset";
        
        private string _currentFilePath;

        #region 按钮点击

        /// <summary>
        /// 新文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            InputDialog inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                int width = inputDialog.WidthValue;
                int height = inputDialog.HeightValue;
                MessageBox.Show($"宽: {width}, 高: {height}", "输入结果");
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json", // 文件类型过滤器
                DefaultExt = "json", // 默认文件扩展名
                FileName = "map.json" // 默认文件名
            };
            
            // 显示对话框并检查用户是否点击了保存
            if (saveFileDialog.ShowDialog() == true)
            {
                // 将 MapBlock 序列化为 JSON
                string json = JsonConvert.SerializeObject(new MapBlock(inputDialog.WidthValue,inputDialog.HeightValue), Formatting.Indented);

                // 将 JSON 写入文件
                File.WriteAllText(saveFileDialog.FileName, json);

                MessageBox.Show("文件创建成功", "New File", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(_currentFilePath)) return;
            
            string json = JsonConvert.SerializeObject(_mapBlock, Formatting.Indented);

            File.WriteAllText(_currentFilePath, json);

            MessageBox.Show("文件保存成功\n" + _currentFilePath, "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        /// <summary>
        /// 刷新文件夹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // 更新路径
            _directoryPath = PathTextBox.Text;

            // 重新加载文件列表
            LoadFileList();
        }

        #endregion
        
        /// <summary>
        /// 载入文件列表
        /// </summary>
        private void LoadFileList()
        {
            try
            {
                // 检查路径是否存在
                if (Directory.Exists(_directoryPath))
                {
                    // 获取路径下的所有文件
                    string[] files = Directory.GetFiles(_directoryPath);

                    // 将文件列表添加到 ListBox
                    FileListBox.Items.Clear();
                    foreach (string file in files)
                    {
                        FileListBox.Items.Add(System.IO.Path.GetFileName(file));
                    }
                }
                else
                {
                    MessageBox.Show("指定路径不存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载文件列表时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// FileListBox_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedFile = FileListBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedFile))
            {
                LoadFile(System.IO.Path.Combine(_directoryPath, selectedFile));
            }
        }

        /// <summary>
        /// 载入资产文件
        /// </summary>
        /// <param name="fullPath"></param>
        private void LoadFile(string fullPath)
        {
            try
            {
                // 读取 JSON 文件内容
                string jsonContent = File.ReadAllText(fullPath);

                // 反序列化为 MapBlock 对象
                _mapBlock = JsonConvert.DeserializeObject<MapBlock>(jsonContent);
                _currentFilePath = fullPath;
                
                DrawMapBlock();

                // 打印反序列化结果（可选）
                Console.WriteLine($"文件加载成功：{fullPath}");
                Console.WriteLine($"宽度：{_mapBlock.Width}, 高度：{_mapBlock.Height}");
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"加载文件失败：{ex.Message}");
            }
        }

        #endregion
    }
}