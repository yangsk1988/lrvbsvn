/****************************************************************************
 *
 * MODULE:             Serial
 *
 * COMPONENT:          $RCSfile: serial.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.4 $
 *
 * DATED:              $Date: 2008-07-07 16:46:57 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             MRW
 *
 * DESCRIPTION:        Generic buffered UART serial communications with RX
 *                     flow control on RTS
 *
 * CHANGE HISTORY:
 *
 * $Log: not supported by cvs2svn $
 * Revision 1.3  2007/09/25 09:03:20  mwild
 * Updated comments and headers
 *
 * Revision 1.2  2007/08/21 15:12:17  mwild
 * Fixed bug in transmit not correctly testing that the queue is empty
 *
 * Revision 1.1  2007/08/16 09:40:59  mwild
 * Created
 * MRW
 *
 * LAST MODIFIED BY:   $Author: rsmit $
 *                     $Modtime: $
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

#include <jendefs.h>
#include <AppHardwareApi.h>

#include "interrupt.h"
#include "uart.h"
#include "queue.h"
#include "serial.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#define SERIAL_TX_QUEUE_SIZE                32
#define SERIAL_RX_QUEUE_SIZE                32
#define SERIAL_RX_QUEUE_HIGH_WATER_MARK     28
#define SERIAL_RX_QUEUE_LOW_WATER_MARK      4

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/

PRIVATE void vRxChar(uint8 u8Char);
PRIVATE void vTxReady(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/

QUEUE_DECLARE_Q(sRxQueue, SERIAL_RX_QUEUE_SIZE); /* [I SP001222_P1 282]*/
QUEUE_DECLARE_Q(sTxQueue, SERIAL_TX_QUEUE_SIZE); /* [I SP001222_P1 282]*/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vSerial_Init
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/

PUBLIC void vSerial_Init(void)
{
    vUART_Init(vRxChar, vTxReady);
    vUART_RtsStartFlow(); /* [I SP001222_P1 284]*/
}

/****************************************************************************
 *
 * NAME: vSerial_TxChar
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/

PUBLIC bool_t bSerial_TxChar(uint8 u8Chr)
{
    bool_t bStatus = TRUE;

    /*
     * prevent race condition where transmitter is busy, but becomes ready
     * between reading its status and queuing the character to transmit
     */
    vInterrupt_Suspend();

    if (bQueue_Empty(sTxQueue) && bUART_TxReady()) {
        vUART_SetTxInterrupt(TRUE);
        vUART_TxChar(u8Chr);
    } else {
        if (!bQueue_Full(sTxQueue)) {
            vQueue_AddItem(sTxQueue, u8Chr);
        } else {
            bStatus = FALSE;
        }
    }

    vInterrupt_Resume();

    return bStatus;
}

/****************************************************************************
 *
 * NAME: i16Serial_RxChar
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/

PUBLIC int16 i16Serial_RxChar(void)
{
    int16 i16Result = -1;

    if(!bQueue_Empty(sRxQueue)) {
        i16Result = (int16)u8Queue_RemoveItem(sRxQueue);

        if (u16Queue_Count(sRxQueue) == SERIAL_RX_QUEUE_LOW_WATER_MARK) {
            vUART_RtsStartFlow(); /* [I SP001222_P1 286]*/
        }
    }

    return i16Result;
}

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vTxReady
 *
 * DESCRIPTION:
 * Transmitter ready callback
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/

PRIVATE void vTxReady(void)
{
    if (!bQueue_Empty(sTxQueue)) {
        vUART_TxChar(u8Queue_RemoveItem(sTxQueue));
    } else {
        vUART_SetTxInterrupt(FALSE);
    }
}

/****************************************************************************
 *
 * NAME: vRxChar
 *
 * DESCRIPTION:
 * Receive character callback
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/

PRIVATE void vRxChar(uint8 u8Char)
{
    vQueue_AddItem(sRxQueue, u8Char);

    if (u16Queue_Count(sRxQueue) == SERIAL_RX_QUEUE_HIGH_WATER_MARK) {
        vUART_RtsStopFlow(); /* [I SP001222_P1 285]*/
    }
}

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
