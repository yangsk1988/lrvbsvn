
/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/
#include <jendefs.h>
#include <gdb.h>
#include <string.h>

#include <Jenie.h>
#include <JPI.h>
#include "Utils.h"
#include "config.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define UARTBufferSize 4
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum
{
    APP_STATE_WAITING_FOR_NETWORK,
    APP_STATE_NETWORK_UP,
    APP_STATE_REGISTERING_SERVICE,
    APP_STATE_RUNNING
} teAppState;

typedef struct
{
    teAppState eAppState;
} tsAppData;

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/

PRIVATE tsAppData sAppData;

/* Routing table storage */
PRIVATE tsJenieRoutingTable asRoutingTable[25];

PRIVATE uint64 deviceMAC;
PRIVATE uint8 uartBuffer[UARTBufferSize];

PRIVATE const uint32 HardwareId = (('H' << 24) | ('W' << 16) | ('I' << 8) | 'D');
/****************************************************************************
 *
 * NAME: gJenie_CbNetworkApplicationID
 *
 * DESCRIPTION:
 * Entry point for application from boot loader.
 * CALLED AT COLDSTART ONLY
 * Allows application to initialises network paramters before stack starts.
 *
 * RETURNS:
 * Nothing
 *
 ****************************************************************************/
PUBLIC void vJenie_CbConfigureNetwork(void)
{
    /* Change default network config */
    gJenie_Channel              = CHANNEL;
    gJenie_NetworkApplicationID = SERVICE_PROFILE_ID;
    gJenie_PanID                = PAN_ID;

    /* Configure stack with routing table data */
    gJenie_RoutingEnabled    = TRUE;
    gJenie_RoutingTableSize  = 25;
    gJenie_RoutingTableSpace = (void *)asRoutingTable;

}

/****************************************************************************
 *
 * NAME: vJenie_CbInit
 *
 * DESCRIPTION:
 * Entry point for application after stack init.
 * Called after stack has initialised. Whether warm or cold start
 *
 * RETURNS:
 * Nothing
 *
 ****************************************************************************/

PUBLIC void vJenie_CbInit(bool_t bWarmStart)
{
    /* Initialise utilities */
    vUtils_Init();

    memset(&sAppData, 0, sizeof(sAppData));
    vUtils_Debug("VirtualBicycle Initializing");

    eJenie_RadioPower(0, TRUE);
    vJPI_UartSetInterrupt(E_JPI_UART_0, FALSE, FALSE, FALSE, TRUE, E_JPI_UART_FIFO_LEVEL_4);

    deviceMAC = 0;

    if(eJenie_Start(E_JENIE_COORDINATOR) != E_JENIE_SUCCESS)
    {
        vUtils_Debug("!!Failed to initialize!!");
        while(1);
    }
}

/****************************************************************************
 *
 * NAME: gJenie_CbMain
 *
 * DESCRIPTION:
 * Main user routine. This is called by the stack at regular intervals.
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vJenie_CbMain(void)
{
    switch(sAppData.eAppState)
    {
    case APP_STATE_WAITING_FOR_NETWORK:
        /* nothing to do till network is up and running */
        break;

    case APP_STATE_NETWORK_UP:
        /* as we are a coorinator, allow nodes to associate with us */
        vUtils_Debug("enabling association");
        eJenie_SetPermitJoin(TRUE);

        /* register services */
        sAppData.eAppState = APP_STATE_REGISTERING_SERVICE;
        break;

    case APP_STATE_REGISTERING_SERVICE:
        /* we provide FIRST_SERVICE */
        vUtils_Debug("registering service");
        eJenie_RegisterServices(FIRST_SERVICE_MASK);

        /* go to the running state */
        sAppData.eAppState = APP_STATE_RUNNING;
        break;

    case APP_STATE_RUNNING:
        /* do all necessary processing here */
        break;

    default:
        vUtils_DisplayMsg("!!Unknown state!!", sAppData.eAppState);
        while(1);
    }
}
/****************************************************************************
 *
 * NAME: vJenie_CbStackMgmtEvent
 *
 * DESCRIPTION:
 * Used to receive stack management events
 *
 * PARAMETERS:      Name            RW  Usage
 *                  eEventType      R   Enumeration to indicate event type
 *                  *pvEventPrim    R   Pointer to data structure containing
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vJenie_CbStackMgmtEvent(teEventType eEventType, void *pvEventPrim)
{
    tsChildJoined* childInfo;

    switch(eEventType)
    {
    case E_JENIE_NETWORK_UP:
        /* Indicates stack is up and running */
        vUtils_Debug("network up");
        if(sAppData.eAppState == APP_STATE_WAITING_FOR_NETWORK)
        {
            sAppData.eAppState = APP_STATE_NETWORK_UP;
        }
        break;

    case E_JENIE_REG_SVC_RSP:
        vUtils_Debug("Reg service response");
        break;

    case E_JENIE_SVC_REQ_RSP:
        vUtils_Debug("Service req response");
        break;

    case E_JENIE_PACKET_SENT:
        vUtils_Debug("PKS");
        break;

    case E_JENIE_PACKET_FAILED:
        vUtils_Debug("PKF");
        break;

    case E_JENIE_CHILD_JOINED:
        childInfo = (tsChildJoined*)pvEventPrim;

        deviceMAC = childInfo->u64SrcAddress;
        vUtils_Debug("Child Jointed");
        vUtils_DisplayHex((uint32)((deviceMAC >> 32) & 0xFFFFFFFFU), 0);
        vUtils_DisplayHex((uint32)(deviceMAC & 0xFFFFFFFFU), 0);

        break;

    case E_JENIE_CHILD_LEAVE:
        vUtils_Debug("Child Leave");
        break;

    case E_JENIE_CHILD_REJECTED:
        vUtils_Debug("Child Rejected");
        break;

    case E_JENIE_STACK_RESET:
        vUtils_Debug("Stack Reset");
        sAppData.eAppState = APP_STATE_WAITING_FOR_NETWORK;
        break;

    default:
        /* Unknown data event type */
        vUtils_DisplayMsg("!!Unknown Mgmt Event!!", eEventType);
        break;
   }
}

/****************************************************************************
 *
 * NAME: vJenie_CbStackDataEvent
 *
 * DESCRIPTION:
 * Used to receive stack data events
 *
 * PARAMETERS:      Name                    RW  Usage
 *                  *psStackDataEvent       R   Pointer to data structure
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vJenie_CbStackDataEvent(teEventType eEventType, void *pvEventPrim)
{
    int i;
    tsData* data;

    switch(eEventType)
    {
    case E_JENIE_DATA:
        data = (tsData*)pvEventPrim;

        for (i = 0; i < data->u16Length; i++)
        {
            while ((u8AHI_UartReadLineStatus(E_JPI_UART_0) & E_JPI_UART_LS_THRE) == 0);
            vAHI_UartWriteData(E_JPI_UART_0, data->pau8Data[i]);
        }

        break;
/*
    case E_JENIE_DATA_TO_SERVICE:
        vUtils_Debug("Data to service event");
        break;

    case E_JENIE_DATA_ACK:
        vUtils_Debug("Data ack");
        break;

    case E_JENIE_DATA_TO_SERVICE_ACK:
        vUtils_Debug("Data to service ack");
        break;
*/
    default:
        // Unknown data event type
        vUtils_DisplayMsg("!!Unknown Data Event!!", eEventType);
        break;
    }
}

/****************************************************************************
 *
 * NAME: vJenie_CbHwEvent
 *
 * DESCRIPTION:
 * Adds events to the hardware event queue.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32Device       R   Peripheral responsible for interrupt e.g DIO
 *                  u32ItemBitmap   R   Source of interrupt e.g. DIO bit map
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vJenie_CbHwEvent(uint32 u32DeviceId, uint32 u32ItemBitmap)
{
    if (u32DeviceId == E_JPI_DEVICE_UART0)
    {
        if (u32ItemBitmap & E_JPI_UART_RXDATA_MASK)
        {
            // 处理PC数据
            if (deviceMAC)
            {
                int bufferSize = 0;
                while ((u8AHI_UartReadLineStatus(E_JPI_UART_0) & E_JPI_UART_LS_THRE))
                {
                    if (bufferSize < UARTBufferSize)
                    {
                        uartBuffer[bufferSize++] = u8AHI_UartReadData(E_JPI_UART_0);
                    }
                }

                if (*((uint32*)&uartBuffer[0]) == HardwareId)
                {
                    vUtils_Debug("VirtualBicycle");
                }
                else
                {
                    eJenie_SendData(deviceMAC, (uint8*)&uartBuffer, (uint16)bufferSize, TXOPTION_ACKREQ);
                }
            }
        }
    }

}
