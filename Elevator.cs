using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Project_1
{
    public class Elevator                                                       //电梯数据块
    {
        public int eStatus;                                                  //电梯运行状况 0->stay; 1->up;-1->down  
        public int toDeal;                                                  //剩下的待处理请求

        public int currentFloor;                                            //电梯当前楼层
        public int currentTarget;                                          //运行状态下当前的目标楼层，-1表示无目标

        public int[] allInTarget = new int[20];                                 //所有内部请求楼层标识，2->有请求且未被分配；1->有请求且已被分配；0->无请求
        public int[][] allOutTarget = new int[2][];                                //标识楼层Button请求，2->有请求且未被分配；1->有请求且已被分配；0->无请求

        public int[] upTarget = new int[20];                                    //电梯上方的目的地状态 1有请求；0没请求
        public int[] downTarget = new int[20];                                  //电梯下方的目的地状态 1有请求；0没请求

        public int eLightStatus;                                                //电梯内部的运行状态灯 1->up; 0->stay; -1->down
        public int[] eFloorLight = new int[20];                                 //电梯内部楼层按钮状态 1有需求；0没需求

        public Elevator()
        {
            eStatus = 0;
            toDeal = 0;
            currentFloor = 1;
            currentTarget = -1;

            allOutTarget[0] = new int[20];
            allOutTarget[1] = new int[20];
        }
    }
}
