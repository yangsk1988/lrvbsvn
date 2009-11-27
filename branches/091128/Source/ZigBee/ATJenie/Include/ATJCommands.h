/*****************************************************************************
 *
 * MODULE:              ATJCommands.h
 *
 * COMPONENT:           ATJCommands.h,v
 *
 * VERSION:             Build_Release_030308_RC1
 *
 * REVISION:            1.4
 *
 * DATED:               2007/11/23 09:46:26
 *
 * STATUS:              Exp
 *
 * AUTHOR:              MRW
 *
 * DESCRIPTION:         AT Jenie Commands
 *
 * LAST MODIFIED BY:    mwild
 *                      $Modtime: $
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
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ****************************************************************************/

#ifndef  ATJCOMMAND_H_INCLUDED
#define  ATJCOMMAND_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum {
    E_JENIE_UNCONFIGURED,
    E_JENIE_INITIALISED,
    E_JENIE_STARTED
} teJenieState;

typedef union {
     tsRegSvcRsp     sRegSvcRsp;
     tsSvcReqRsp     sSvcReqRsp;
     tsPollCmplt     sPollCmplt;
     tsPacketSent    sPacketSent;
     tsPacketFailed  sPacketFailed;
     tsChildJoined   sChildJoined;
     tsChildLeave    sChildLeave;
     tsChildRejected sChildRejected;        /* PR 878 */
     tsNwkStartUp    sNwkStartUp;
     tsData          sData;
     tsDataToService sDataToService;
} tuEventData;

typedef struct {
    bool_t              bGoToSleep;
    teJenieSleepMode    eSleepMode;
} tsJenieSleepState;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vATJ_CommandsInit(void);
PUBLIC bool_t vATJ_CommandsJenieEventToStr(teEventType eType, tuEventData *uEventData , uint8 *EventStr);
PUBLIC bool_t bATJ_CommandsJenieHwEventToStr(uint32 u32DeviceId, uint32 u32ItemBitmap, uint8 *EventStr);
PUBLIC bool_t bRecoverAppContext(void);
/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

PUBLIC teJenieState ATJ_CommandsJenieState;
PUBLIC tsJenieSleepState ATJ_CommandsSleepState;


#if defined __cplusplus
}
#endif

#endif

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/


