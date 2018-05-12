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
        //private bool flagForEStatusSet = false;                                                //1表示已更改
        public Elevator eControl = new Elevator();
        public TextBox elevatorText = new TextBox();
        private delegate void boxDelegate();

        public void FloorLightControl(int floor) { eControl.eFloorLight[floor - 1] = 1; }       //点亮目标楼层灯

        //public void ELightControl(int floor) { eControl.lightStatu[floor - 1] = false; }      //灭灯

        //Based on the given TargetFloor, set the direction
        //But suppose every requeset will be finished by this only ele at first
        public void SetDirection()
        {
            if (eControl.currentTarget == eControl.currentFloor) eControl.eStatus = 0;

            eControl.eStatus = (eControl.currentTarget > eControl.currentFloor) ? 1 : -1;

            return;
            /*
            if (this.eControl.eStatus == 1)
                for (int i = eControl.currentFloor; i < 20; i++)
                    if (eControl.upTarget[i] == 1)
                        break;

            if (this.eControl.eStatus == -1) 
                for(int i=eControl.currentFloor;i>=0;i--)
                    if(eControl.downTarget[i]==1)
                        break;

            if (this.eControl.eStatus == 0) 
            {
                int distanceUp = 0, distanceDown = 0;
                for (int i = eControl.currentFloor; i < 20; i++)
                {
                    distanceUp++;
                    if (eControl.upTarget[i] == 1)
                        break;
                }
                for (int i = eControl.currentFloor; i >= 0; i--)
                {
                    distanceDown++;
                    if (eControl.downTarget[i] == 1)
                        break;
                }
                this.eControl.eStatus = (distanceUp > distanceDown) ? 1 : -1;
            }
            //flagForEStatusSet = true;
            */
        }

        /*    if (eControl.eStatus == 1)
            {
                for (int i = eControl.currentFloor; i < 20; i++)
                {
                    if (requestFloor[i] == 1)
                    {
                        eControl.eStatus = 1;
                        break;
                    }
                }
            }
            if (eControl.eStatus == -1)
            {
                for (int i = eControl.currentFloor; i >= 0; i++)
                {
                    if (requestFloor[i] == 1)
                    {
                        eControl.eStatus = -1;
                        break;
                    }
                }
            }
            else
            {
                int distanceUp = 0, distanceDown = 0;
                for (int i = eControl.currentFloor; i < 20; i++)
                {
                    distanceUp++;
                    if (requestFloor[i] == 1)
                        break;
                }
                for (int i = eControl.currentFloor; i >= 0; i--)
                {
                    distanceDown++;
                    if (requestFloor[i] == 1)
                        break;
                }
                eControl.eStatus = (distanceUp > distanceDown) ? 1 : -1;
            }
            eControl.eStatus = 0;*/

        public void SetTarget()
        {
            //没有考虑当层有请求的情况
            //没有考虑运行中接收到另一指令的情况----------------------------------------------------->使用待处理请求数来解决
            if (eControl.eStatus == 1)
            {
                for (int i = eControl.currentTarget; i >= eControl.currentFloor - 1; i--)
                {
                    if (eControl.allOutTarget[0][i] == 2)
                    {
                        eControl.currentTarget = i + 1;
                    }
                }
            }
            else if (eControl.eStatus == -1)
            {
                for (int i = eControl.currentTarget ; i <= eControl.currentFloor - 1; i++)
                {
                    if (eControl.allOutTarget[1][i] == 2)
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
                    if (eControl.allOutTarget[0][i] == 2 || eControl.allOutTarget[1][i] == 2) { flag = true; break; }
                }
                if (!flag) distanceUp = 999;
                flag = false;

                for (int i = eControl.currentFloor; i >= 0; i--)
                {
                    distanceDown++;
                    if (eControl.allOutTarget[0][i] == 2 || eControl.allOutTarget[1][i] == 2) { flag = true; break; }
                }
                if (!flag) distanceDown = 999;
                flag = false;

                //there maybe problem here
                if (distanceUp <= distanceDown) eControl.currentTarget = eControl.currentFloor + distanceUp;
                if (distanceUp > distanceDown) eControl.currentTarget = eControl.currentFloor - distanceDown;

            }
            SetDirection();
        }

        //main function to be invoked
        public void Run(object t)
        {
            elevatorText = (t as TextBox);

            while (true)
            {
                this.elevatorText.Dispatcher.Invoke(MoveFlash);
                this.elevatorText.Dispatcher.Invoke(Arrive);
                Thread.Sleep(100);
            }
        }

        //control the textbox[currentElevator] to move up or down
        public void MoveFlash()
        {
            if (eControl.eStatus == 0) return;

            double left = elevatorText.Margin.Left;
            double bottom = elevatorText.Margin.Bottom;

            if (eControl.eStatus == 1)
            {
                bottom += 30;
            }
            else if (eControl.eStatus == -1)
            {
                bottom -= 30;
            }
            elevatorText.Margin = new Thickness(left, 0, 0, bottom);
            eControl.currentFloor++;
        }

        //存在问题：无法判断电梯是冲着向上键还是向下键来的
        public void Arrive()
        {
            if (eControl.currentFloor == eControl.currentTarget)
            {
                if (eControl.eStatus == 1)
                    eControl.allOutTarget[0][eControl.currentTarget - 1] = 0;

                if (eControl.eStatus == -1)
                    eControl.allOutTarget[1][eControl.currentTarget - 1] = 0;

                eControl.eStatus = 0;
                eControl.currentTarget = -1;
                //if (eControl.upTarget[eControl.currentFloor] == 2)
            }
        }
    }
}

