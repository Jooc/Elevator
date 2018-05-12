using System;
using System.Collections.Generic;
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

using System.Threading;
using System.Windows.Threading;

namespace OS_Project_1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer timer = new DispatcherTimer();               //计时器

        public int currentElevator;                                         //标记电梯

        public ElevatorController[] elevator = new ElevatorController[5];        

        public MainWindow()
        {
            InitializeComponent();

            elevator[0] = new ElevatorController();                         //给每个电梯添加实例对象
            elevator[1] = new ElevatorController();
            elevator[2] = new ElevatorController();
            elevator[3] = new ElevatorController();
            elevator[4] = new ElevatorController();

            Thread ele1 = new Thread(new ParameterizedThreadStart(elevator[0].Run));
            Thread ele2 = new Thread(new ParameterizedThreadStart(elevator[1].Run));
            Thread ele3 = new Thread(new ParameterizedThreadStart(elevator[2].Run));
            Thread ele4 = new Thread(new ParameterizedThreadStart(elevator[3].Run));
            Thread ele5 = new Thread(new ParameterizedThreadStart(elevator[4].Run));
            Thread.CurrentThread.IsBackground = true;

            ele1.Name = @"ele1";
            ele2.Name = @"ele2";
            ele3.Name = @"ele3";
            ele4.Name = @"ele4";
            ele5.Name = @"ele5";

            ele1.Start(elevator1);
            ele2.Start(elevator2);
            ele3.Start(elevator3);
            ele4.Start(elevator4);
            ele5.Start(elevator5);
            
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Start();
            timer.Tick += new EventHandler(UpdateInLight);
            
            timer.Tick += new EventHandler(UpdateOutLight);
        }

        public void UpdateInLight(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                string buttonName = @"button";
                buttonName += (i + 1).ToString();

                if (elevator[currentElevator].eControl.eFloorLight[i] == 1)
                {
                    TurnPressButton(FindName(buttonName) as Button);
                }
                if (elevator[currentElevator].eControl.eFloorLight[i] == 0)
                {
                    TurnUnPressButton(FindName(buttonName) as Button);
                }
            }
        }

        public void UpdateOutLight(object sender,EventArgs e)
        {
            for (int i = 0; i < 20;i++)
            {
                string buttonName = @"button";
                buttonName += (i + 21).ToString();
                if (elevator[currentElevator].eControl.allOutTarget[0][i] == 2)
                    TurnPressButton(FindName(buttonName) as Button);
                if (elevator[currentElevator].eControl.allOutTarget[0][i] == 0) 
                    TurnUnPressButton(FindName(buttonName) as Button);
            }
            for (int i = 20; i < 40; i++) 
            {
                string buttonName = @"button";
                buttonName += (i + 21).ToString();
                if (elevator[currentElevator].eControl.allOutTarget[1][i - 20] == 2) 
                    TurnPressButton(FindName(buttonName) as Button);
                if (elevator[currentElevator].eControl.allOutTarget[1][i - 20] == 0) 
                    TurnUnPressButton(FindName(buttonName) as Button);
            }
        }

 //按键响应//

        //选择当前操作电梯
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentElevator = whichElevator.SelectedIndex;
        }

        //电梯内部请求
        private void Button_Click_eFloor(object sender, RoutedEventArgs e)
        {
            //suppose every request is reasonable
            //亮灯函数 lightUP();

            //设立target函数
            int buttonID = (sender as Button).TabIndex;
            //elevator[currentElevator].eControl.allTarget;

            elevator[currentElevator].SetInTarget();


        }

        //电梯外部请求
        private void Button_Click_FloorUpDown(object sender, RoutedEventArgs e)
        {
            int buttonID = (sender as Button).TabIndex;

            if (1 <= buttonID && buttonID <= 20)
                elevator[currentElevator].eControl.allOutTarget[0][buttonID - 1] = 2;
            if (21 <= buttonID && buttonID <= 40)
                elevator[currentElevator].eControl.allOutTarget[1][buttonID - 21] = 2;

            elevator[currentElevator].SetTarget();
        }

       /* private void Button_Click_FloorUp(object sender, RoutedEventArgs e)
        {
            if (elevator[currentElevator].eControl.allTarget[0][(sender as Button).TabIndex - 1] == 0)
                elevator[currentElevator].eControl.allTarget[0][(sender as Button).TabIndex - 1] = 2;
            elevator[currentElevator].SetTarget();
        }
        private void Button_Click_FloorDown(object sender, RoutedEventArgs e)
        {
            if (elevator[currentElevator].eControl.allTarget[1][(sender as Button).TabIndex - 1] == 0)
                elevator[currentElevator].eControl.allTarget[1][(sender as Button).TabIndex - 1] = 2;
            elevator[currentElevator].SetTarget();
        }
        */

        public void TurnPressButton(Button obj)
        {
            obj.Background = Brushes.Red;
        }
        public void TurnUnPressButton(Button obj)
        {
            obj.Background = Brushes.White;
        }


    }
}
