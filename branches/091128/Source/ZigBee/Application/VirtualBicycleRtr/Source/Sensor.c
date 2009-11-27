#include <gdb.h>
#include <string.h>

#include <Jenie.h>
#include <JPI.h>
#include "Utils.h"
#include "config.h"

#include "Sensor.h"

#define SpeedA E_JPI_DIO14_INT
#define SpeedB E_JPI_DIO15_INT

#define AngleA E_JPI_DIO18_INT
#define AngleB E_JPI_DIO19_INT
#define AngleZ E_JPI_DIO20_INT

PRIVATE int32 angleDir;
PRIVATE int32 angle;
PRIVATE uint32 wheel;
PRIVATE uint32 lastWheel;

PRIVATE uint32 wheelSpeed;

PUBLIC void vSensor_CbInit(bool_t warmStart)
{
    vJPI_DioSetDirection(SpeedA,0);
    //vJPI_DioSetDirection(SpeedB,0);

    vJPI_DioSetDirection(AngleA,0);
    vJPI_DioSetDirection(AngleB,0);
    vJPI_DioSetDirection(AngleZ,0);

    vJPI_DioWake(SpeedA,0,SpeedA,0);
    vJPI_DioWake(SpeedB,0,SpeedB,0);

    vJPI_DioWake(AngleA,0,AngleA,0);
    vJPI_DioWake(AngleB,0,AngleB,0);
    vJPI_DioWake(AngleZ,0,AngleZ,0);
    angleDir = 1;
}

PUBLIC void vSensor_CbHwEvent(uint32 u32DeviceId,uint32 u32ItemBitmap)
{
    switch (u32DeviceId)
    {
        case E_JPI_DEVICE_TICK_TIMER:
            if (wheel < lastWheel)
            {
                lastWheel = 0xFFFFFFFFU - lastWheel;
                wheel += lastWheel;
            }

            wheelSpeed = (wheel - lastWheel);
            lastWheel = wheel;
            break;
        case E_JPI_DEVICE_SYSCTRL:
            if (u32ItemBitmap & AngleB)
            {
                angleDir = 1;
            }
            else
            {
                angleDir = -1;
            }

            if (u32ItemBitmap & AngleA)
            {
                angle += angleDir;
            }

            if (u32ItemBitmap & AngleZ)
            {
                angle = 0;
            }

            if (u32ItemBitmap & SpeedA)
            {
                wheel++;
            }
            break;
    }
}

PUBLIC void vSensor_CbMain(void)
{

}

PRIVATE inline uint32 u32ConvertAngle(int a)
{
    return (uint32)(a + 0xFFFF);
}

PUBLIC void vSensor_CbStackDataEvent(teEventType eEventType, void *pvEventPrim)
{
    static uint32 sendBuferAll[4] = {
        ('S' << 24) | ('W' << 16) | ('H' << 8) | 'L' , 0,
        ('S' << 24) | ('A' << 16) | ('N' << 8) | 'G' , 0};
    static uint32 sendBuferWheel[2] = { ('S' << 24) | ('W' << 16) | ('H' << 8) | 'L' , 0};
    static uint32 sendBuferHandlebar[2] = { ('S' << 24) | ('A' << 16) | ('N' << 8) | 'G' , 0};

    static char* strAll = "SALL";
    static char* strWheel = "SWHL";
    static char* strHandlebar = "SAND";

    if (eEventType == E_JENIE_DATA && pvEventPrim)
    {
        tsData* data = (tsData*)pvEventPrim;

        uint8* str = data->pau8Data;

        if (memcmp(str, strAll, 4))
        {
            sendBuferAll[1] = wheel;
            sendBuferAll[3] = u32ConvertAngle(angle);
            eJenie_SendData(0, (uint8*)&sendBuferAll, sizeof(sendBuferAll), TXOPTION_ACKREQ);
        }
        else if (memcmp(str, strWheel, 4))
        {
            sendBuferWheel[1] = wheel;
            eJenie_SendData(0, (uint8*)&sendBuferWheel, sizeof(sendBuferWheel), TXOPTION_ACKREQ);
        }
        else if (memcmp(str, strHandlebar, 4))
        {
            sendBuferHandlebar[1] = u32ConvertAngle(angle);
            eJenie_SendData(0, (uint8*)&sendBuferHandlebar, sizeof(sendBuferHandlebar), TXOPTION_ACKREQ);
        }
    }
}
