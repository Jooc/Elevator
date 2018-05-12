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
using System.Collections.Specialized;


//v2.0结构优化
//1.对已被分配但还没有完成处理的按钮进行分类：2->未分配; 1->已分配未完成; 0->无请求

namespace OS_Project_1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public int currentElevator;                                         //电梯ID

        public DispatcherTimer timer = new DispatcherTimer();               //计时器

        public ElevatorController[] elevator = new ElevatorController[5];   //电梯控制器实例

        public Queue<int> allRequest = new Queue<int>();                    //为分配器存放请求
        


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

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Start();
            timer.Tick += new EventHandler(UpdateInLight);
            timer.Tick += new EventHandler(UpdateOutLight);
            timer.Tick += new EventHandler(TargetAssign);

        }

        //可以合并 ①
        //用于控制电梯内部按钮灯
        //需要依靠Elevator 内部变量inTarget 来判断
        //v1.0
        public void UpdateInLight(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                string buttonName = @"button";
                buttonName += (i + 1).ToString();

                if (elevator[currentElevator].eControl.inTarget[i] == 2)
                {
                    TurnPressButton(FindName(buttonName) as Button);
                }
                if (elevator[currentElevator].eControl.inTarget[i] == 0)
                {
                    TurnUnPressButton(FindName(buttonName) as Button);
                }
            }
        }

        //v1.0
        //可以合并②
        //用于控制外部的上下请求灯
        //需要依靠Elevator 内部变量outTarget 来判断

        //v2.0 优化1
        public void UpdateOutLight(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                string buttonName = @"button";
                buttonName += (i + 21).ToString();
                for (int j = 0; j < 5; j++)
                {
                    //Console.WriteLine("HelloWorld");
                    if (elevator[j].eControl.outTarget[0][i] == 2)
                    {
                        TurnPressButton(FindName(buttonName) as Button);
                        break;
                    }
                    TurnUnPressButton(FindName(buttonName) as Button);
                }
            }
            for (int i = 20; i < 40; i++)
            {
                string buttonName = @"button";
                buttonName += (i + 21).ToString();
                for (int j = 0; j < 5; j++)
                {
                    if (elevator[j].eControl.outTarget[1][i - 20] == 2)
                    {
                        TurnPressButton(FindName(buttonName) as Button);
                        break;
                    }
                    TurnUnPressButton(FindName(buttonName) as Button);
                }
            }
        }

 //按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应//
 //按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应////按键响应//

        //选择当前操作电梯
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentElevator = whichElevator.SelectedIndex;
        }

        //电梯内部请求
        //判断请求是否合理
        //更新inTarget状态√
        private void Button_Click_eFloor(object sender, RoutedEventArgs e)
        {
            int buttonID = (sender as Button).TabIndex;

            elevator[currentElevator].eControl.inTarget[buttonID - 1] = 2;

            elevator[currentElevator].eControl.toDeal++;
        }

        //电梯外部请求
        //判断请求是否合理
        //更新outTarget状态     √
        //待处理请求+1          √
        private void Button_Click_FloorUpDown(object sender, RoutedEventArgs e)
        {
            int buttonID = (sender as Button).TabIndex;

            allRequest.Enqueue(buttonID + 20);

            /*
            if (1 <= buttonID && buttonID <= 20)
            {
                //elevator[currentElevator].eControl.typeOfOutTarget = 0;
                elevator[currentElevator].eControl.outTarget[0][buttonID - 1] = 2;
            }
            if (21 <= buttonID && buttonID <= 40)
            {
                //elevator[currentElevator].eControl.typeOfOutTarget = 1;
                elevator[currentElevator].eControl.outTarget[1][buttonID - 21] = 2;
            }
            
            elevator[currentElevator].eControl.toDeal++;
            */
        }

        //按钮处于按下状态
        public void TurnPressButton(Button obj)
        {
            obj.Background = Brushes.Red;
        }
        //按钮处于松开状态
        public void TurnUnPressButton(Button obj)
        {
            obj.Background = Brushes.White;
        }


        //五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度
        //五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度//五部电梯调度

        //基于队列allRequest的任务调度分配器
        //当allRequest 不空时，就对其队头的请求按钮所表示的请求进行分配
        //完成button_click -> queue ->elevator[].target的转换
        //具体步骤：
        //Ⅰ判断表头请求的楼层及请求方向
        //Ⅱ根据请求方向安排电梯
        //  ①先找同向电梯 ->找出距离最短者
        //  ②若没有同向电梯 ->在静止电梯中找到距离最近
        //Ⅲ分配Target

        public void TargetAssign(object sender, EventArgs e)
        {
            if (allRequest.Count == 0) return;
            
            int buttonID = allRequest.Dequeue();

            int[] distance1 = new int[5];                           //顺路电梯距离请求的位置
            int[] distance2 = new int[5];                           //静止电梯距离请求的距离

            //inTarget
            //内部请求本身具有电梯指向性
            //因此不需要计算直接在Button响应中分配
            //if (1 <= buttonID && buttonID <= 20) { }

            //outUpTarget
            if (21 <= buttonID && buttonID <= 40)
            {
                int floor = buttonID - 20;                              //请求楼层数

                for (int i = 0; i < 5; i++)
                {
                    distance1[i] = 999; distance2[i] = 999;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (elevator[i].eControl.eStatus == 1)
                    {
                        if (elevator[i].eControl.currentFloor <= floor)
                            distance1[i] = floor - elevator[i].eControl.currentFloor;
                    }
                    if (elevator[i].eControl.eStatus == 0)
                    {
                        distance2[i] = Math.Abs(floor - elevator[i].eControl.currentFloor);
                    }
                }

                int closestEleID = -1;                                  //最近的电梯ID
                int minDistance = 20;                                   //最近的距离

                for (int i = 0; i < 5; i++)
                    if (distance1[i] < minDistance)
                    {
                        minDistance = distance1[i];
                        closestEleID = i;
                    }

                if (minDistance < 20)
                {
                    elevator[closestEleID].eControl.outTarget[0][floor - 1] = 2;
                    elevator[closestEleID].eControl.toDeal++;
                    return;
                }

                minDistance = 20;
                for (int i = 0; i < 5; i++)
                    if (distance2[i] < minDistance)
                    {
                        minDistance = distance2[i];
                        closestEleID = i;
                    }

                if (minDistance < 20)
                {
                    elevator[closestEleID].eControl.outTarget[0][floor - 1] = 2;
                    elevator[closestEleID].eControl.toDeal++;
                    return;
                }
            }

            //putDownTarget
            if (41 <= buttonID && buttonID <= 60)
            {
                int floor = buttonID - 40;

                for (int i = 0; i < 5; i++)
                {
                    distance1[i] = 999; distance2[i] = 999;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (elevator[i].eControl.eStatus == -1)
                    {
                        if (elevator[i].eControl.currentFloor >= floor)
                            distance1[i] = elevator[i].eControl.currentFloor - floor;
                    }
                    if (elevator[i].eControl.eStatus == 0)
                    {
                        distance2[i] = Math.Abs(floor - elevator[i].eControl.currentFloor);
                    }
                }

                int closestEleID = -1;                                  //最近的电梯ID
                int minDistance = 20;                                   //最近的距离

                for (int i = 0; i < 5; i++)
                    if (distance1[i] < minDistance)
                    {
                        minDistance = distance1[i];
                        closestEleID = i;
                    }

                if (minDistance < 20)
                {
                    elevator[closestEleID].eControl.outTarget[1][floor - 1] = 2;
                    elevator[closestEleID].eControl.toDeal++;
                    return;
                }

                minDistance = 20;
                for (int i = 0; i < 5; i++)
                    if (distance2[i] < minDistance)  
                    {
                        minDistance = distance2[i];
                        closestEleID = i;
                    }
                if (minDistance < 20)
                {
                    elevator[closestEleID].eControl.outTarget[1][floor - 1] = 2;
                    elevator[closestEleID].eControl.toDeal++;
                    return;
                }
            }
        }
          
    }
}
