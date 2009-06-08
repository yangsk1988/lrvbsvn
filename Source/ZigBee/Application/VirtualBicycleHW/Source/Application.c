/****************************************************************************
 *
 * MODULE:             ATJenieApp.c
 *
 * COMPONENT:          ATJenie_App.c,v
 *
 * VERSION:            Build_Release_030308_RC1
 *
 * REVISION:           1.21
 *
 * DATED:              2007/11/23 12:07:22
 *
 * STATUS:             Exp
 *
 * AUTHOR:             MRW
 *
 * DESCRIPTION:        AT Jenie Example Application
 *
 * LAST MODIFIED BY:   mwild
 *                     $Modtime: $
 *
 * HISTORY:
 *
 ****************************************************************************
 *
 * This software is owned by Jennic and/or its supplier and is protected
 * under applicable copyright laws. All rights are reserved. We grant You,
 * and any third parties, a license to use this software solely and
 * exclusively on Jennic products. You, and any third parties must reproduce
 * the copyright and warranty notice and any other legend of ownership on each
 * copy or partial copy of the software.
 *
 * THIS SOFTWARE IS PROVIDED "AS IS". JENNIC MAKES NO WARRANTIES, WHETHER
 * EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE,
 * ACCURACY OR LACK OF NEGLIGENCE. JENNIC SHALL NOT, IN ANY CIRCUMSTANCES,
 * BE LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, SPECIAL,
 * INCIDENTAL OR CONSEQUENTIAL DAMAGES FOR ANY REASON WHATSOEVER.
 *
 * Copyright Jennic Ltd 2007. All rights reserved
 *
 ****************************************************************************/

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/

#include <string.h>
#include <jendefs.h>

#include "gdb.h"

#include "Jenie.h"
#include "ATJParser.h"
#include "ATJCommands.h"
#include "ATJPlatformCmds.h"
#ifdef ATJ_TUNNELING
#include "ATJTunnel.h"
#endif
#include "JPI.h"
#include "ledcontrol.h"
#include "Serial.h"
#include "uart.h"
#include "AppHardwareApi.h"
#include "JPDM.h"
#include "ATJExtensionCmds.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#ifdef ATJ_TUNNELING
/* service AT Jenie command tunnel is connected to */
#define ATJ_TUNNEL_SERVICE              (32UL)
#endif

#define ED_MAIN_LOOP_INTERVAL       10000
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum
{
    APP_STATE_WAITING_FOR_NETWORK,
    APP_STATE_NETWORK_UP,
    APP_STATE_RUNNING
} teAppState;

typedef struct
{
    teAppState    eAppState;            /* not used yet - available for future use but need to use one of the timers */
    /* declare any application data to be saved to flash here !! */
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
#ifndef ATJ_MAX_INPUT_STRING_SIZE
#error "You need to define ATJ_MAX_INPUT_STRING_SIZE"
#endif

#ifndef ATJ_MAX_OUTPUT_STRING_SIZE
#error "You need to define ATJ_MAX_OUTPUT_STRING_SIZE"
#endif

PRIVATE uint8 InCommandStr[ATJ_MAX_INPUT_STRING_SIZE];      /* PR 1173 - allocate buffer area for Parser input  characters here then declare channel below */
PRIVATE uint8 OutCommandStr[ATJ_MAX_OUTPUT_STRING_SIZE];    /* PR 1173 - allocate buffer area for Parser output characters here then declare channel below */

ATJ_DECLARE_CHAR_CHANNEL(UartChannel, i16Serial_RxChar, bSerial_TxChar, vUART_SetBaudRate, InCommandStr, OutCommandStr, ATJ_MAX_INPUT_STRING_SIZE, ATJ_MAX_OUTPUT_STRING_SIZE); /* [I SP001222_P1 1] */

PRIVATE bool_t bRestoreContext = FALSE;         /* PR 759 */
PRIVATE tsAppData sAppData;                     /* PR 759 */
PRIVATE tsJPDM_BufferDescription sAppFlashData = JPDM_DECLARE_BUFFER_DESCRIPTION("APPCONTEXT",&sAppData, sizeof(sAppData));    /* PR 759 */

/* PR 879 - We now allocate the routing table for Coordinator and Routers - although routing MUST be set/reset using the INI command */
#ifndef ATJ_ENDDEVICE
#define ROUTING_TABLE_SIZE  100
PRIVATE tsJenieRoutingTable asRoutingTable[ROUTING_TABLE_SIZE];
#endif

#ifdef ATJ_ENDDEVICE
PRIVATE bool_t bAsleep = FALSE;
#endif

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vJenie_ConfigureNetwork
 *
 * DESCRIPTION:
 * Entry point for application from boot loader. Initialises system.
 *
 * RETURNS:
 * Nothing
 *
 ****************************************************************************/

PUBLIC void vJenie_CbConfigureNetwork(void)
{
    /* Debug hooks: include these regardless of whether debugging or not */
    HAL_GDB_INIT();
#ifdef GDB
    /* Increase the UART speed from the default of 9600 */
    vAHI_UartSetClockDivisor(0, E_AHI_UART_RATE_38400);
#endif
    HAL_BREAKPOINT();

    #ifdef ATJ_JN5148
    vAHI_WatchdogStop();
    #endif

    bRestoreContext = bRecoverAppContext();

    gJenie_RecoverFromJpdm = bRestoreContext;
    gJenie_RecoverChildrenFromJpdm = TRUE;

    #if defined (ATJ_COORDINATOR) || defined (ATJ_ROUTER)
    /* NOTE: 'gJenie_RoutingEnabled' MUST be set/reset using the INI command */
    gJenie_RoutingTableSize = ROUTING_TABLE_SIZE;       /* this space is always allocated even if routing disabled */
    gJenie_RoutingTableSpace = (void *)asRoutingTable;
    #endif

    vSerial_Init(); /* [I SP001222_P1 1] */

    vATJ_ParserInit(FALSE);
    vATJ_CommandsInit();
    vATJ_PlatformCommandsInit();
    vATJ_ExtensionCommandsInit();          /* added for AT-Jenie extension commands */

    vATJ_ParserAddChannel(&UartChannel);
    vATJ_ParserOpenChannel(&UartChannel);

    /* only allow applicable commands when configuring */
    vATJ_ParserSetCommandEnable(&UartChannel, "INI,CFG,CFP,GTV,CCF,CCS,RST,OAD", TRUE); /* [I SP001222_P1 272] */

    /* enable all peripheral commands */
    vATJ_ParserSetCommandEnable(&UartChannel, "PAC,PAE,PAD,PAS,PAR,PAO,PCE,PCD,PCI,PCS,PCW,PWE,PWG,PWX,PWS,PWC,PWF,PDD,PDO,PDP,PDR,PDE,PDS,PTE,PTD,PTG,PTC,PTR,PTX,PTF", TRUE); /* [I SP001222_P1 272] */

    eJPDM_RestoreContext(&sAppFlashData);                           /* <-- restore application context here */
    if (sAppFlashData.eState == JPDM_RECOVERY_STATE_RECOVERED)
    {
        sAppData.eAppState = APP_STATE_WAITING_FOR_NETWORK;         /* <-- application context is valid */
    }

    while ((E_JENIE_INITIALISED != ATJ_CommandsJenieState) && (!bRestoreContext))       /* PR 759  - add 'bRestoreContext' */
    { /* [I SP001222_P1 273] */
        vATJ_ParserProcessCharChannel(&UartChannel); /* [I SP001222_P1 271] */
    }

    ATJ_CommandsJenieState = E_JENIE_INITIALISED;

    vATJ_ParserSetCommandEnable(&UartChannel, "INI", FALSE);
}

/****************************************************************************
 *
 * NAME: vJenie_Init
 *
 * DESCRIPTION:
 * Called by Jenie after stack started to perforn hardware init
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/

PUBLIC void vJenie_CbInit(bool_t bWarmStart)
{
    if (bWarmStart)
    {
        HAL_GDB_INIT();
    #ifdef GDB
        /* Increase the UART speed from the default of 9600 */
        vAHI_UartSetClockDivisor(0, E_AHI_UART_RATE_38400);
    #endif

        #ifdef ATJ_JN5148
        vAHI_WatchdogStop();
        #endif

        vSerial_Init();
        vATJ_ParserInit(TRUE);

#if defined(ATJ_COORDINATOR)
        eJenie_Start(E_JENIE_COORDINATOR);
#elif defined(ATJ_ROUTER)
        eJenie_Start(E_JENIE_ROUTER);
#elif defined(ATJ_ENDDEVICE)
        eJenie_Start(E_JENIE_END_DEVICE);

        bAsleep = FALSE;                            /* PR 1216 */

#else
#error One of ATJ_COORDINATOR, ATJ_ROUTER or ATJ_ENDDEVICE must be defined
#endif

    }
    else
    {
        /* only allow applicable commands when starting */
        vATJ_ParserSetCommandEnable(&UartChannel, "STR", TRUE); /* [I SP001222_P1 275] */

        while ((E_JENIE_STARTED != ATJ_CommandsJenieState) && (!bRestoreContext))      /* [I SP001222_P1 276] */
        {
            vATJ_ParserProcessCharChannel(&UartChannel); /* [I SP001222_P1 274] */
        }

        if (bRestoreContext)                    /* PR 759 */
        {
            #if defined(ATJ_COORDINATOR)
            eJenie_Start(E_JENIE_COORDINATOR);
            #elif defined(ATJ_ROUTER)
            eJenie_Start(E_JENIE_ROUTER);
            #elif defined(ATJ_ENDDEVICE)
            eJenie_Start(E_JENIE_END_DEVICE);
            #endif
            ATJ_CommandsJenieState = E_JENIE_STARTED;
        }

        vATJ_ParserSetCommandEnable(&UartChannel, "STR,CFG,CFP", FALSE); /* [I SP001222_P1 278] */
    }

}

/****************************************************************************/
/***               Functions called by the stack                          ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vJenie_Main
 *
 * DESCRIPTION:
 * Main user routine.
 * at regular intervals.
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/

PUBLIC void vJenie_CbMain(void)
{
    static bool_t bInit = TRUE;
#if defined(ATJ_ENDDEVICE)
    static uint16 u16EDMainLoopInterval = 0;
#endif

    /* enable commands once the wireless net is up */
    if (bInit)
    {
        bInit = FALSE;
        /* register all services here */
        eJenie_RegisterServices(1UL << (ATJ_TUNNEL_SERVICE - 1));
        vATJ_TunnelInit(&UartChannel, ATJ_TUNNEL_SERVICE);
        vATJ_TunnelSetCommandEnable((char *) vATJ_GetExtCommandStrings(), TRUE);                     /* required to use AT-Jenie extension commands */
        /* enable commands once the wireless network is up */
        vATJ_ParserSetCommandEnable(&UartChannel, "REG,RQS,BND,UBN,SAS,GAS,KEY,SND,SDS,SCN,RCN,SSP,SLP,RDP,GTV,POL,LVE,GDP", TRUE); // [I SP001222_P1 278]
        vATJ_ParserSetCommandEnable(&UartChannel, "BGV,BGT,BGL,BGH,BGR,BGF,BLO,BLF,BTX", TRUE);
        /* enable all extension commands */
        vATJ_ParserSetCommandEnable(&UartChannel, (char *) vATJ_GetExtCommandStrings(), TRUE);       /* required to use AT-Jenie extension commands */
    }


#if defined(ATJ_ENDDEVICE)
    if (!bAsleep)                                                   /* PR 1216 - don't attempt write to UART whilst we are asleep */
    {
        if (vATJ_ParserProcessCharChannel(&UartChannel))            /* check if UART busy */
        {
            u16EDMainLoopInterval = 0;                              /* PR 1216 - UART busy - don't do any further processing until next loop interval */
        }
    }
#else
    vATJ_ParserProcessCharChannel(&UartChannel);                    /* UART input/output */
#endif


    /* PR808 - Perform any processing on a regular basis
       Required specifically for the Sleep command as it gives an inherent post-Parser delay necessary
       for the AT-Parser to return to an idle state before sleeping - otherwise Parser cannot process any further commands */
#if defined(ATJ_ENDDEVICE)
    if (u16EDMainLoopInterval++ > ED_MAIN_LOOP_INTERVAL)                /* PR 1181 - End Device only enters main function at intervals based on a simple counter since H/W tick not reliable */
    {
        if (ATJ_CommandsSleepState.bGoToSleep)                          /* PR808 - Parser controlled sleep */
        {
            ATJ_CommandsSleepState.bGoToSleep = FALSE;
            bAsleep = TRUE;
            /* ensure UART is powered down to save power */
            while((u8JPI_UartReadLineStatus(0) & E_JPI_UART_LS_TEMT) == 0);
            vJPI_UartDisable(E_JPI_UART_0);                             /* PR 1124 */
            eJenie_Sleep(ATJ_CommandsSleepState.eSleepMode);            /* PR808 - go to sleep */
        }
        u16EDMainLoopInterval = 0;
    }
#endif

}

/****************************************************************************
 *
 * NAME: vJenie_HwEvent
 *
 * DESCRIPTION:
 * Adds events to the hardware event queue.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32DeviceId     R   Peripheral responsible for interrupt e.g DsGateway.u8NodeDetailsIO
 *                  u32ItemBitmap   R   Source of interrupt e.g. DIO bit map
 *
 * RETURNS:
 * void
 *
 * HISTORY: Ver0.7 - 07/08/07 - new AT_Jenie Command set - End-of-Command character changed to "Line Feed"
 *                              instead of any non-CSV character as previously
 ****************************************************************************/

PUBLIC void vJenie_CbHwEvent(uint32 u32DeviceId,uint32 u32ItemBitmap)
{
    uint8 EventStr[128];

    if (bATJ_CommandsJenieHwEventToStr(u32DeviceId, u32ItemBitmap, EventStr)) {
        vATJ_ParserProcessEvent(EventStr);
    }
}

/****************************************************************************
 *
 * NAME: vJenie_StackDataEvent
 *
 * DESCRIPTION:
 * Used to receive stack data events
 *
 * PARAMETERS:      Name                    RW  Usage
 *                  *psStackDataEvent       R   Pointer to data structure
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/

PUBLIC void vJenie_CbStackDataEvent(teEventType eEventType, void *pvEventPrim)
{
#ifdef ATJ_TUNNELING
    if (!bATJ_TunnelCommand(eEventType, pvEventPrim)) {
#endif
        uint8 EventStr[128];
        if (vATJ_CommandsJenieEventToStr(eEventType, pvEventPrim, EventStr)) {
            vATJ_ParserProcessEvent(EventStr);
        }
#ifdef ATJ_TUNNELING
    }
#endif
}


/****************************************************************************
 *
 * NAME: vJenie_StackMgmtEvent
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
    uint8 EventStr[128];
    bool_t  bHideEvent = FALSE;

    /* following modifications added for PR 1207 */
    if (eEventType == E_JENIE_CHILD_LEAVE)
    {
        if (pvEventPrim == NULL)           /* NULL means this is the CHILD not the PARENT */
        {
            vATJ_ParserSetCommandEnable(&UartChannel, "STR", TRUE);     /* ... re-enable 'STR' command in case we want to re-join */
            bHideEvent = TRUE;                                          /* don't display the event */
        }
    }
    else if (eEventType == E_JENIE_NETWORK_UP)      /* disable any previously re-enabled 'STR' command */
    {
        vATJ_ParserSetCommandEnable(&UartChannel, "STR", FALSE);
    }
    /* end of PR 1207 mods */

    if (vATJ_CommandsJenieEventToStr(eEventType, pvEventPrim, EventStr)) {
        if (!bHideEvent) {
            vATJ_ParserProcessEvent(EventStr);
        }
    }
}

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************/
/***          END OF FILE                                                 ***/
/****************************************************************************/
