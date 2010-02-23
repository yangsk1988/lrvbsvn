/****************************************************************************
 *
 * MODULE:             Queue
 *
 * COMPONENT:          $RCSfile: queue.h,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.3 $
 *
 * DATED:              $Date: 2008-07-07 16:46:56 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             MRW
 *
 * DESCRIPTION
 *
 * CHANGE HISTORY:
 *
 * $Log: not supported by cvs2svn $
 * Revision 1.2  2007/09/25 09:03:20  mwild
 * Updated comments and headers
 *
 * Revision 1.1  2007/08/16 09:40:58  mwild
 * Created
 * MRW
 *
 * LAST MODIFIED BY:   $Author: rsmit $
 *                     $Modtime: $
 *
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

#ifndef  QUEUE_H_INCLUDED
#define  QUEUE_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#define QUEUE_DECLARE_Q(n, s) uint8 qData_##n[s]; tsQueue n = { s, 0, 0, 0, (uint8 *)&qData_##n};
#define QUEUE_REF(a) &(a)

#define bQueue_Full(q)          bQueue__Full(QUEUE_REF(q))
#define bQueue_Empty(q)         bQueue__Empty(QUEUE_REF(q))
#define u8Queue_RemoveItem(q)   u8Queue__RemoveItem(QUEUE_REF(q))
#define vQueue_AddItem(q, i)    vQueue__AddItem(QUEUE_REF(q), (i))
#define vQueue_Flush(q)         vQueue__Flush(QUEUE_REF(q))
#define u16Queue_Count(q)       u16Queue__Count(QUEUE_REF(q))

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef struct
{
    uint16 u16Size;
    uint16 u16Head;
    uint16 u16Tail;
    uint16 u16Count;
    uint8  *u8Buff;
} tsQueue;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC bool_t bQueue__Full(tsQueue *psQueue);
PUBLIC bool_t bQueue__Empty(tsQueue *psQueue);
PUBLIC uint8 u8Queue__RemoveItem(tsQueue *psQueue);
PUBLIC void   vQueue__AddItem(tsQueue *psQueue, uint8 u8Item);
PUBLIC void   vQueue__Flush(tsQueue *psQueue);
PUBLIC uint16 u16Queue__Count(tsQueue *psQueue);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
