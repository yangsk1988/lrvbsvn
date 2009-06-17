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
#define Button1Id E_JPI_DIO0_INT
#define Button2Id E_JPI_DIO1_INT
#define Button3Id E_JPI_DIO2_INT
#define Button4Id E_JPI_DIO3_INT

#define Led1 E_JPI_DIO16_INT
#define Led2 E_JPI_DIO17_INT


#define SpeedA E_JPI_DIO14_INT
#define SpeedB E_JPI_DIO15_INT

#define AngleA E_JPI_DIO18_INT
#define AngleB E_JPI_DIO19_INT
#define AngleZ E_JPI_DIO20_INT

#define tkPKST (uint8)'P'
#define tkButton (uint8)'S'
#define tkReset (uint8)'R'
#define tkAll (uint8)'D'
#define tkWheel (uint8)'W'
#define tkHandlebar (uint8)'H'
#define tkForce (uint8)'F'


#define DEBUG
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum
{
    APP_STATE_WAITING_FOR_NETWORK,
    APP_STATE_NETWORK_UP,
    APP_STATE_SERVICE_REGISTERED,
    APP_STATE_WAITING_FOR_REQUEST_SERVICE,
    APP_STATE_SERVICE_REQUEST_RETURNED,
    APP_STATE_RUNNING
} teAppState;

typedef struct
{
    teAppState eAppState;
    uint64     u64ServiceAddress;
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

PRIVATE bool isNetworkAvailable;

PRIVATE uint32 numSccessfulPacket;
PRIVATE uint32 numFailedPacket;


PRIVATE int32 angleDir;
PRIVATE int32 angle;
PRIVATE uint32 wheel;
PRIVATE uint32 lastWheel;

PRIVATE uint32 wheelSpeed;

PRIVATE uint32 buttonState;

PRIVATE inline void vSendData(uint8* dataPtr, uint16 length)
{
    if (sAppData.eAppState == APP_STATE_RUNNING &&
        isNetworkAvailable)
    {
        eJenie_SendData(0, dataPtr, length, TXOPTION_ACKREQ);
        isNetworkAvailable = FALSE;
    }
}

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
    gJenie_Channel          = CHANNEL;
    gJenie_NetworkApplicationID = SERVICE_PROFILE_ID;
    gJenie_PanID            = PAN_ID;

    /* Configure stack with routing table data */
    gJenie_RoutingEnabled    = TRUE;
    gJenie_RoutingTableSize  = 25;
    gJenie_RoutingTableSpace = (void *) asRoutingTable;

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
    #ifdef DEBUG
    vUtils_Debug("Device Initializing");
    #endif

    isNetworkAvailable = TRUE;
    numSccessfulPacket = 0;
    numFailedPacket = 0;
    buttonState = 0;

    wheel = 0;
    lastWheel = 0;
    angle = 0;
    angleDir = 1;


    eJenie_RadioPower(0, FALSE);

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
    vJPI_DioSetOutput(0, Led1);
    vJPI_DioSetOutput(Led2, 0);


    if(eJenie_Start(E_JENIE_ROUTER) != E_JENIE_SUCCESS)
    {
        vUtils_Debug("!!Failed to initiailze!!");
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
    static int i = 0;

    switch(sAppData.eAppState)
    {
    case APP_STATE_WAITING_FOR_NETWORK:
        break;

    case APP_STATE_NETWORK_UP:
        /* as we are a router, allow nodes to associate with us */
        #ifdef DEBUG
        vUtils_Debug("enabling association");
        #endif
        eJenie_SetPermitJoin(TRUE);

        /* we provide SECOND_SERVICE */
        #ifdef DEBUG
        vUtils_Debug("registering service");
        #endif
        eJenie_RegisterServices(SECOND_SERVICE_MASK);

        /* go to the running state */
        sAppData.eAppState = APP_STATE_RUNNING;

        /* Or request services from coordinator */
        //sAppData.eAppState = APP_STATE_SERVICE_REGISTERED;
        break;

    case APP_STATE_SERVICE_REGISTERED:
        /* we use service FIRST_SERVICE on a remote node */
        #ifdef DEBUG
        vUtils_Debug("requesting service");
        #endif

        eJenie_RequestServices(FIRST_SERVICE_MASK, TRUE);

        sAppData.eAppState = APP_STATE_WAITING_FOR_REQUEST_SERVICE;
        i = 40000;
        break;

    case APP_STATE_WAITING_FOR_REQUEST_SERVICE:
        if(i == 0)
        {
            sAppData.eAppState = APP_STATE_SERVICE_REGISTERED;
        }
        i--;
        break;

    case APP_STATE_SERVICE_REQUEST_RETURNED:
        /* bind local SECOND_SERVICE to remote FIRST_SERVICE */
        #ifdef DEBUG
        vUtils_Debug("binding service");
        #endif

        if (eJenie_BindService(SECOND_SERVICE, sAppData.u64ServiceAddress, FIRST_SERVICE) == E_JENIE_SUCCESS)
        {
        	uint8 au8Data[] = "SECOND->FIRST";
        	eJenie_SendDataToBoundService(SECOND_SERVICE, au8Data, 14, TXOPTION_ACKREQ);
       	}

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


PRIVATE void vFlashNetPacketLed(void)
{
    static bool netLedStat = FALSE;

    if (netLedStat)
    {
        vJPI_DioSetOutput(Led2, 0);
        netLedStat = FALSE;
    }
    else
    {
        vJPI_DioSetOutput(0, Led2);
        netLedStat = TRUE;
    }
}
/****************************************************************************
 *
 * NAME: vJenie_CbStackMgmtEvent
 *
 * DESCRIPTION:
 * Used to receive stack management events
 *
 * PARAMETERS:      Name                    RW  Usage
 *                  *psStackMgmtEvent       R   Pointer to event structure
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vJenie_CbStackMgmtEvent(teEventType eEventType, void *pvEventPrim)
{
    switch(eEventType)
    {
    case E_JENIE_NETWORK_UP:
        /* Indicates stack is up and running */
        #ifdef DEBUG
        vUtils_Debug("network up");
        #endif
        if(sAppData.eAppState == APP_STATE_WAITING_FOR_NETWORK)
        {
            sAppData.eAppState = APP_STATE_NETWORK_UP;
        }

        break;

    case E_JENIE_SVC_REQ_RSP:
        #ifdef DEBUG
        vUtils_Debug("Service req response");
        #endif
        if(sAppData.eAppState == APP_STATE_WAITING_FOR_REQUEST_SERVICE)
        {
            if(((tsSvcReqRsp *)pvEventPrim)->u32Services & FIRST_SERVICE_MASK)
            {
                sAppData.u64ServiceAddress = ((tsSvcReqRsp *)pvEventPrim)->u64SrcAddress;
                sAppData.eAppState = APP_STATE_SERVICE_REQUEST_RETURNED;
            }
            else
            {
                vUtils_Debug("wrong service");
            }
        }
        break;

    case E_JENIE_PACKET_SENT:
        vFlashNetPacketLed();
        numSccessfulPacket++;

        #ifdef DEBUG
        vUtils_Debug("PKS");
        #endif
        break;

    case E_JENIE_PACKET_FAILED:
        vFlashNetPacketLed();
        numFailedPacket++;
        isNetworkAvailable = TRUE;
        #ifdef DEBUG
        vUtils_Debug("PKF");
        #endif
        break;

    case E_JENIE_CHILD_JOINED:
        break;
    case E_JENIE_CHILD_LEAVE:
        break;
    case E_JENIE_CHILD_REJECTED:
        break;

    case E_JENIE_STACK_RESET:
        vJPI_DioSetOutput(0, Led1);
        sAppData.eAppState = APP_STATE_WAITING_FOR_NETWORK;

        break;

    default:
        /* Unknown data event type */
        vUtils_DisplayMsg("!!Unknown Mgmt Event!!", eEventType);
        break;
   }
}


PRIVATE inline uint32 u32ConvertAngle(int a)
{
    return (uint32)(a + 0xFFFF);
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
    static uint32 sendBuferAll[4] = {
        ('S' << 24) | ('V' << 16) | ('E' << 8) | 'L' , 0,
        ('S' << 24) | ('A' << 16) | ('N' << 8) | 'G' , 0};
    static uint32 sendBuferWheel[2] = { ('S' << 24) | ('V' << 16) | ('E' << 8) | 'L' , 0};
    static uint32 sendBuferHandlebar[2] = { ('S' << 24) | ('A' << 16) | ('N' << 8) | 'G' , 0};
    static uint32 sendBufferPKST[3] = { ('P' << 24) | ('K' << 16) | ('T' << 8) | 'S', 0, 0 };
    static uint32 sendBuferBtn[2] = { ('U' << 24) | ('B' << 16) | ('T' << 8) | 'N' , 0};


    tsData* data;

    switch(eEventType)
    {
    case E_JENIE_DATA:
        data = (tsData*)pvEventPrim;

        uint8* str = data->pau8Data;

        switch(str[0])
        {
        case tkAll:
            sendBuferAll[1] = wheel;
            sendBuferAll[3] = u32ConvertAngle(angle);
            vSendData((uint8*)&sendBuferAll, sizeof(sendBuferAll));
            break;
        case tkForce:

            break;
        case tkReset:
            wheel = 0;
            lastWheel = 0;
            wheelSpeed = 0;
            angle = 0;

            break;
        case tkHandlebar:
            sendBuferHandlebar[1] = u32ConvertAngle(angle);
            vSendData((uint8*)&sendBuferHandlebar, sizeof(sendBuferHandlebar));
            break;
        case tkWheel:
            sendBuferWheel[1] = wheel;
            vSendData((uint8*)&sendBuferWheel, sizeof(sendBuferWheel));
            wheel = 0;
            lastWheel = 0;
            break;
        case tkButton:
            sendBuferBtn[1] = buttonState;
            vSendData((uint8*)&sendBuferBtn, sizeof(sendBuferBtn));
            break;
        case tkPKST:
            sendBufferPKST[1] = numSccessfulPacket;
            sendBufferPKST[2] = numFailedPacket;
            vSendData((uint8*)&sendBufferPKST, sizeof(sendBufferPKST));
            break;
        }

        break;

    case E_JENIE_DATA_ACK:
        #ifdef DEBUG
        vUtils_Debug("ACK");
        #endif
        isNetworkAvailable = TRUE;

        break;

    case E_JENIE_DATA_TO_SERVICE:
        //vUtils_Debug("Data to service event");
        break;
    case E_JENIE_DATA_TO_SERVICE_ACK:
        //vUtils_Debug("Data to service ack");
        break;

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
PUBLIC void vJenie_CbHwEvent(uint32 u32DeviceId,uint32 u32ItemBitmap)
{
    static int i = 0;
    static uint32 sendBufer[2] = { ('U' << 24) | ('B' << 16) | ('T' << 8) | 'N' , 0};

    switch (u32DeviceId)
    {
    case E_JPI_DEVICE_TICK_TIMER:
        if (sAppData.eAppState == APP_STATE_RUNNING)
        {
            i++;

            if (i == 40)
            {
                vJPI_DioSetOutput(Led1, 0);
            }
            else if (i == 80)
            {
                vJPI_DioSetOutput(0, Led1);
                i = 0;
            }
        }


        if (wheel < lastWheel)
        {
            lastWheel = 0xFFFFFFFFU - lastWheel;
            wheel += lastWheel;
        }

        wheelSpeed = (wheel - lastWheel);
        lastWheel = wheel;
        break;
    case E_JPI_DEVICE_SYSCTRL:
        buttonState = u32ItemBitmap & (Button1Id | Button2Id | Button3Id | Button4Id);
        if (buttonState)
        {
            // 向网关发送数据: buttonStates & newState
            sendBufer[1] = buttonState;

            vSendData((uint8*)&sendBufer[0], sizeof(sendBufer));
            #ifdef DEBUG
            vUtils_Debug("Button Pressed");
            #endif
        }



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
