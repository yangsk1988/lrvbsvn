
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

PRIVATE int32 angle;
PRIVATE int32 wheel;
PRIVATE float wheelSpeed;

PUBLIC void vSensor_CbInit(bool_t warmStart)
{
    vJPI_DioSetDirection(SpeedA,0);
    vJPI_DioSetDirection(SpeedB,0);

    vJPI_DioSetDirection(AngleA,0);
    vJPI_DioSetDirection(AngleB,0);
    vJPI_DioSetDirection(AngleZ,0);

    vJPI_DioWake(SpeedA,0,SpeedA,0);
    vJPI_DioWake(SpeedB,0,SpeedB,0);

    vJPI_DioWake(AngleA,0,AngleA,0);
    vJPI_DioWake(AngleB,0,AngleB,0);
    vJPI_DioWake(AngleZ,0,AngleZ,0);

}

PUBLIC void vSensor_CbHwEvent(uint32 u32DeviceId,uint32 u32ItemBitmap)
{
    switch (u32DeviceId)
    {
        case E_JPI_DEVICE_TICK_TIMER:
            break;
        case E_JPI_DEVICE_SYSCTRL:
            if (u32ItemBitmap & AngleZ)
            {
                angle = 0;
            }
            if (u32ItemBitmap & SpeedA)
            {

            }
            break;
    }
}
PUBLIC void vSensor_CbMain(void)
{

}
