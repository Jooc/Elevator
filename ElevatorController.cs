using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace OS_Project_1
{
    //给每个电梯都设立独立的ElevatorController 类
    //用来判断电梯的当前状态，并设置接下来的状态

    public class ElevatorController
    {
        public Elevator eControl = new Elevator();
        public TextBox elevatorText = new TextBox();
        

        //Based on the given TargetFloor, set the direction √
        public void SetDirection()
        {
            if (eControl.currentTarget == eControl.currentFloor) eControl.eStatus = 0;

            eControl.eStatus = (eControl.currentTarget > eControl.currentFloor) ? 1 : -1;
        }

        //main function to be invoked
        public void Run(object t)
        {
            elevatorText = (t as TextBox);

            while (true)
            {
                while (true)
                {
                    SetTarget();
                    if (eControl.eStatus != 0) break;
                }

                if (WhetherStop())
                {
                    Thread.Sleep(1500);
                    this.ElvatorStop();
                }
                else
                    this.elevatorText.Dispatcher.Invoke(Move);

                Thread.Sleep(500);
            }

        }

        public void SetTarget()
        {
            if (eControl.toDeal == 0) return;

            //向上状态分析
            //检测内外部请求中有没有必当前目标更近的楼层
            //如有则切换当前目标
            //没有则do nothing
            if (eControl.eStatus == 1)
            {
                for (int i = eControl.currentTarget - 1; i > eControl.currentFloor; i--)
                {
                    if (eControl.inTarget[i] == 2)
                    {
                        eControl.currentTarget = i + 1;
                        eControl.typeOfOutTarget = -1;
                    }
                    if (eControl.outTarget[0][i] == 2)
                    {
                        eControl.currentTarget = i + 1;
                        eControl.typeOfOutTarget = 0;   
                    }
                }
            }

            //向下 ->需求同上
            if (eControl.eStatus == -1)
            {
                for (int i = eControl.currentTarget + 1; i < eControl.currentFloor; i++)
                {
                    if (eControl.inTarget[i] == 2)
                    {
                        eControl.currentTarget = i + 1;
                        eControl.typeOfOutTarget = -1;
                    }
                    if(eControl.outTarget[1][i] == 2)
                    {
                        eControl.currentTarget = i + 1;
                        eControl.typeOfOutTarget = 1;
                    }
                }
            }

            //静止状态分析
            //检测最近的请求并将其设为当前目标楼层
            if (eControl.eStatus == 0) 
            {
                int distanceUp = 999, distanceDown = 999;

                for (int i = eControl.currentFloor - 1; i < 20; i++)
                {
                    if (eControl.inTarget[i] == 2 || eControl.outTarget[0][i] == 2 || eControl.outTarget[1][i] == 2)
                        distanceUp = i + 1 - eControl.currentFloor;
                }
                for(int i=eControl.currentFloor-1;i>=0;i--)
                {
                    if (eControl.inTarget[i] == 2 || eControl.outTarget[0][i] == 2 || eControl.outTarget[1][i] == 2)
                        distanceDown = eControl.currentFloor - (i + 1);
                }
                if (distanceUp <= distanceDown) { eControl.currentTarget = eControl.currentFloor + distanceUp; }
                if (distanceUp > distanceDown) { eControl.currentTarget = eControl.currentFloor - distanceDown; }

                if (eControl.inTarget[eControl.currentTarget - 1] == 2)
                    eControl.typeOfOutTarget = -1;
                if (eControl.outTarget[0][eControl.currentTarget - 1] == 2)
                    eControl.typeOfOutTarget = 0;
                if (eControl.outTarget[1][eControl.currentTarget - 1] == 2)
                    eControl.typeOfOutTarget = 1;
            }
            SetDirection();
        }

        //只要根据当前的运动状态做出TextBox的行为回应即可
        //v1.0
        public void Move()
        {
            if (eControl.eStatus == 0) return;

            double left = elevatorText.Margin.Left;
            double bottom = elevatorText.Margin.Bottom;

            if (eControl.eStatus == 1)
            {
                bottom += 30;
                eControl.currentFloor++;
            }
            else if (eControl.eStatus == -1)
            {
                bottom -= 30;
                eControl.currentFloor--;
            }
            elevatorText.Margin = new Thickness(left, 0, 0, bottom);
        }


        //仅仅用于判断是否需要停靠      √
        private bool WhetherStop() { return eControl.currentFloor == eControl.currentTarget; }

        //根据当前楼层和当前目标楼层判断是否需要停靠
        //只要相等就停靠
        //待处理请求-1
        public void ElvatorStop()
        {

            int floor = eControl.currentFloor;

            if (eControl.inTarget[floor - 1] == 2) { eControl.inTarget[floor - 1] = 0; }

            if (eControl.typeOfOutTarget == 0 || eControl.typeOfOutTarget == 1)
                if (eControl.outTarget[eControl.typeOfOutTarget][floor - 1] == 2)
                    eControl.outTarget[eControl.typeOfOutTarget][floor - 1] = 0;

            eControl.toDeal--;
            eControl.currentTarget = -1;
            eControl.eStatus = 0;

            return;

            /* if (eControl.eStatus == 1)
             {
                 //总有为了↓按钮而上升的情况

                 if (eControl.inTarget[floor - 1] == 2) { eControl.inTarget[floor - 1] = 0; }
                 if (eControl.outTarget[0][floor - 1] == 2) { eControl.outTarget[0][floor - 1] = 0; }
             }
             if (eControl.eStatus == -1)
             {
                 //同样
                 //总有为了↑按钮而下降的情况

                 if (eControl.inTarget[floor - 1] == 2) { eControl.inTarget[floor - 1] = 0; }
                 if (eControl.outTarget[1][floor - 1] == 2) { eControl.outTarget[1][floor - 1] = 0; }
             }*/
            //该类请求应该会在Button 触发函数被拒绝掉
            //if (eControl.eStatus == 0) { }
        }

    }
}

/*
public void SetTarget()
{
    //没有考虑当层有请求的情况
    //没有考虑运行中接收到另一指令的情况----------------------------------------------------->使用待处理请求数来解决
    if (eControl.eStatus == 1)
    {
        for (int i = eControl.currentTarget; i >= eControl.currentFloor - 1; i--)
        {
            if (eControl.outTarget[0][i] == 2)
            {
                eControl.currentTarget = i + 1;
            }
        }
    }
    else if (eControl.eStatus == -1)
    {
        for (int i = eControl.currentTarget; i <= eControl.currentFloor - 1; i++)
        {
            if (eControl.outTarget[1][i] == 2)
            {
                eControl.currentTarget = i + 1;
            }
        }
    }
    else if (eControl.eStatus == 0)
    {
        bool flag = false;                                          //标记检索途中是否真的有target
        int distanceUp = 0, distanceDown = 0;

        for (int i = eControl.currentFloor; i < 20; i++)
        {
            distanceUp++;
            if (eControl.outTarget[0][i] == 2 || eControl.outTarget[1][i] == 2) { flag = true; break; }
        }
        if (!flag) distanceUp = 999;
        flag = false;

        for (int i = eControl.currentFloor; i >= 0; i--)
        {
            distanceDown++;
            if (eControl.outTarget[0][i] == 2 || eControl.outTarget[1][i] == 2) { flag = true; break; }
        }
        if (!flag) distanceDown = 999;
        flag = false;

        //there maybe problem here
        if (distanceUp <= distanceDown) eControl.currentTarget = eControl.currentFloor + distanceUp;
        if (distanceUp > distanceDown) eControl.currentTarget = eControl.currentFloor - distanceDown;

    }
    SetDirection();
}*/


//存在问题：无法判断电梯是冲着向上键还是向下键来的
/*
public void WhetherArrive()
{
    if (eControl.currentFloor == eControl.currentTarget)
    {
        if (eControl.eStatus == 1)
            eControl.outTarget[0][eControl.currentTarget - 1] = 0;

        if (eControl.eStatus == -1)
            eControl.outTarget[1][eControl.currentTarget - 1] = 0;

        eControl.eStatus = 0;
        eControl.currentTarget = -1;
        //if (eControl.upTarget[eControl.currentFloor] == 2)
    }
}
*/
