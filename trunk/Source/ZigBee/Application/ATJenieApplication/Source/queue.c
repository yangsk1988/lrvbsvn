/****************************************************************************
 *
 * MODULE:             Queue
 *
 * COMPONENT:          $RCSfile: queue.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.4 $
 *
 * DATED:              $Date: 2008-07-07 16:46:56 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             MRW
 *
 * DESCRIPTION:         Generic FIFO queue
 *
 *
 * CHANGE HISTORY:
 *
 * $Log: not supported by cvs2svn $
 * Revision 1.3  2007/09/25 09:03:21  mwild
 * Updated comments and headers
 *
 * Revision 1.2  2007/08/21 15:13:53  mwild
 * Removed LCD debug
 *
 * Revision 1.1  2007/08/16 09:40:59  mwild
 * Created
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

#include "interrupt.h"
#include "queue.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vQueue_AddItem
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

PUBLIC void vQueue__AddItem(tsQueue *psQueue, uint8 u8Item)
{
    if (psQueue->u16Count < psQueue->u16Size)
    {
        /* Space available in buffer so add data */
        psQueue->u8Buff[psQueue->u16Head++] = u8Item;
        if (psQueue->u16Head == psQueue->u16Size) {
            psQueue->u16Head = 0;
        }

        /* read, modify write operation must be atomic */
        vInterrupt_Suspend();
        psQueue->u16Count++;
        vInterrupt_Resume();
    }
}

/****************************************************************************
 *
 * NAME: u8Queue_RemoveItem
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

PUBLIC uint8 u8Queue__RemoveItem(tsQueue *psQueue)
{
    uint8 u8Item = 0;

    if (psQueue->u16Count > 0)
    {
        /* Data available in buffer so remove data */
        u8Item = psQueue->u8Buff[psQueue->u16Tail++];

        if (psQueue->u16Tail == psQueue->u16Size) {
            psQueue->u16Tail = 0;
        }

        /* read, modify write operation must be atomic */
        vInterrupt_Suspend();
        psQueue->u16Count--;
        vInterrupt_Resume();
    }

    return u8Item;
}

/****************************************************************************
 *
 * NAME: bQueue_Empty
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

PUBLIC bool_t bQueue__Empty(tsQueue *psQueue)
{
    return psQueue->u16Count == 0;
}

/****************************************************************************
 *
 * NAME: bQueue_Full
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

PUBLIC bool_t bQueue__Full(tsQueue *psQueue)
{
    return psQueue->u16Count == psQueue->u16Size;
}

/****************************************************************************
 *
 * NAME: vQueue_Flush
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

PUBLIC void vQueue__Flush(tsQueue *psQueue)
{
    vInterrupt_Suspend();
    psQueue->u16Count = psQueue->u16Head = psQueue->u16Tail = 0;
    vInterrupt_Resume();
}

/****************************************************************************
 *
 * NAME: vQueue_Count
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * Count of number if items in queue
 *
 * NOTES:
 * None.
 ****************************************************************************/

PUBLIC uint16 u16Queue__Count(tsQueue *psQueue)
{
    return psQueue->u16Count;
}

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
