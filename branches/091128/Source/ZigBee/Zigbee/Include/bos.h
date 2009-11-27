/*****************************************************************************
 *
 * MODULE:              Jennic Zigbee: Bee Operating System
 *
 * COMPONENT:           $RCSfile: bos.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.5 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              Korwin
 *
 * DESCRIPTION:
 * BOS service interface
 *
 * CHANGE HISTORY:
 *
 * LAST MODIFIED BY:    $Author: gpfef $
 *                      $Modtime: $
 *
 ****************************************************************************
 *
 * This software is owned by Jennic and/or its supplier and is protected
 * under applicable copyright laws. All rights are reserved. We grant You,
 * and any third parties, a license to use this software solely and
 * exclusively on Jennic products. You, and any third parties must reproduce
 * the copyright and warranty notice and any other legend of ownership on
 * each copy or partial copy of the software.
 *
 * THIS SOFTWARE IS PROVIDED "AS IS". JENNIC MAKES NO WARRANTIES, WHETHER
 * EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE,
 * ACCURACY OR LACK OF NEGLIGENCE. JENNIC SHALL NOT, IN ANY CIRCUMSTANCES, BE
 * LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, SPECIAL,
 * INCIDENTAL OR CONSEQUENTIAL DAMAGES FOR ANY REASON WHATSOEVER.
 *
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ****************************************************************************/

#ifndef __BOS_H
#define __BOS_H

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include "jendefs.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define BOS_TASK_HANDLER_DEFAULTVALUE		0xff

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef enum
{
	E_SUCCESS,				//! 0x00 - Success
	E_INVALID_HANDLE,		//! 0x01 - Input task handle value is wrong
	E_NULL_FUNCTION,		//! 0x02 - Input function pointer is NULL
	E_NO_MSG,				//! 0x03 - Size of message is zero or pointer of message is NULL
	E_NO_MEMORY,			//! 0x04 - Insufficient heap to be allocated
	E_NO_TASK,				//! 0x05 - No available task
	E_NO_TIMER,				//! 0x06 - Timer doesn't exist
	E_SCHED_INIT_ERROR,		//! 0x07 - Scheduler initialization error
	E_SCHED_STARTED,		//! 0x08 - Scheduler ran already
	E_PROTECT_ERROR,		//! 0x09 - Stack protection error
	E_PARAM,				//! 0x0A - Input parameter is wrong
	E_ACCESS_VIOLATE,		//! 0x0B - Heap descriptor is being used already
	E_TASK_CREATE,			//! 0x0C - Appliation task shall be created in only JZA_vAppDefineTasks
	E_ISR_VIOLATE			//! 0x0D - pvBOSMemAlloc, bBOSMemFree, bBosPutMsg, bBosCreateTimer, bBosRemoveTimer These functions can not be called in ISR Only bBosSetEvent function can be called in ISR
} teBosError;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC teBosError eBosGetLastError(void);
PUBLIC bool_t bBosSetEvent(uint8 u8Handle, uint8 u8Event);
PUBLIC bool_t bBosPutMsg(uint8 u8Handle, void *pvMsg, uint8 u8MsgSize);
PUBLIC bool_t bBosSetTask(uint8	u8Handle, bool_t bIterMode);
PUBLIC bool_t bBosCreateTask(void (*pfvInit)(uint8), void (*pfvHandler)(void *, uint8, uint8));
PUBLIC bool_t bBosCreateTimer(void (*pfvHandler)(void *, uint8), void *pvMsg, uint8 u8MsgSize, uint32 u32Delay, uint8 *pu8TimerId);
PUBLIC bool_t bBosRemoveTimer(void (*pfvHandler)(void *, uint8), uint8 u8TimerId);
PUBLIC void *pvBosMemAlloc(uint16 u16Size);
PUBLIC bool_t bBosMemFree(void *pvMem);
PUBLIC bool_t bBosRun(bool_t bColdStart);
PUBLIC void vBosCpuDoze(bool_t bEnable);
PUBLIC void vBosSleep(bool_t bMemHold);
PUBLIC void vBosRequestSleep(bool_t bMemoryHold);
PUBLIC void vBOSReverseMemCpy( void *pau8DstBuf, const void *pau8SrcBuf, uint8 u8Len );
PUBLIC uint8 u8BosRand(uint8 u8Min, uint8 u8Max);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif	/* __BOS_H */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
