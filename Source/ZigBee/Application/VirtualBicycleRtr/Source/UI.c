#include <gdb.h>
#include <string.h>

#include <Jenie.h>
#include <JPI.h>
#include "Utils.h"
#include "config.h"

#include "UI.h"

#define Button1Id E_JPI_DIO9_INT
#define Button2Id E_JPI_DIO10_INT
#define Button3Id E_JPI_DIO2_INT
#define Button4Id E_JPI_DIO3_INT

#define Led1 E_JPI_DIO16_INT
#define Led2 E_JPI_DIO17_INT

PRIVATE uint32 buttonStates;

PUBLIC void vUI_CbStackDataEvent(teEventType eEventType)
{
    switch (eEventType)
    {
        case E_JENIE_DATA:
            vJPI_DioSetOutput(Led2, 0);
            break;
        default:
            vJPI_DioSetOutput(0, Led2);
            break;
    }
}

PUBLIC void vUI_CbHwEvent(uint32 u32DeviceId,uint32 u32ItemBitmap)
{
    static int i = 0;
    uint32 newState;

    switch (u32DeviceId)
    {
        case E_JPI_DEVICE_TICK_TIMER:
            i++;

            if (i == 5)
            {
                vJPI_DioSetOutput(Led1, 0);
            }
            else if (i == 120)
            {
                vJPI_DioSetOutput(0, Led1);
                i = 0;
            }
            break;
        case E_JPI_DEVICE_SYSCTRL:
            newState = u32ItemBitmap & (Button1Id | Button2Id | Button3Id | Button4Id);
            if (newState != buttonStates)
            {
                // 向网关发送数据: buttonStates & newState
                eJenie_SendData(0, (uint8*)&buttonStates, sizeof(buttonStates), TXOPTION_ACKREQ);
                vUtils_Debug("Button Pressed");

                buttonStates = newState;
            }
            break;
    }
}

PUBLIC void vUI_CbInit(bool_t warmStart)
{
    vJPI_DioSetDirection(Button1Id,0);
    vJPI_DioSetDirection(Button2Id,0);
    vJPI_DioSetDirection(Button3Id,0);
    vJPI_DioSetDirection(Button4Id,0);

    vJPI_DioWake(Button1Id,0,Button1Id,0);
    vJPI_DioWake(Button2Id,0,Button2Id,0);
    vJPI_DioWake(Button3Id,0,Button3Id,0);
    vJPI_DioWake(Button4Id,0,Button4Id,0);

    vJPI_DioSetDirection(0, Led1);
    vJPI_DioSetDirection(0, Led2);

    vJPI_DioSetPullup(!Led1, Led1);
    vJPI_DioSetPullup(!Led2, Led2);
    vJPI_DioSetOutput(0, Led2);

    buttonStates = 0;
}

PUBLIC void vUI_CbMain(void)
{

}
