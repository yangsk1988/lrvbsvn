/*****************************************************************************
 *
 * MODULE:              Application API header
 *
 * COMPONENT:           $RCSfile: AppQueueApi.h,v $
 *
 * VERSION:             $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:            $Revision: 1.3 $
 *
 * DATED:               $Date: 2008/10/22 12:09:43 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              CJG
 *
 * DESCRIPTION:
 * Access functions and structures used by the application to interact with
 * the Jennic 802.15.4 stack.
 *
 * LAST MODIFIED BY:    $Author: pjtw $
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
 * ACCURACY OR LACK OF NEGLIGENCE. JENNIC SHALL NOT, IN ANY CIRCUMSTANCES,
 * BE LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, SPECIAL,
 * INCIDENTAL OR CONSEQUENTIAL DAMAGES FOR ANY REASON WHATSOEVER.
 *
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ***************************************************************************/

#ifndef  APP_Q_API_H_INCLUDED
#define  APP_Q_API_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include <jendefs.h>
#include <mac_sap.h>
#include <AppApi.h>

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef void (*PR_QIND_CALLBACK)(void);
typedef void (*PR_HWQINT_CALLBACK)(void);

typedef struct
{
    uint32 u32DeviceId;
    uint32 u32ItemBitmap;
} AppQApiHwInd_s;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC uint32 u32AppQApiInit(PR_QIND_CALLBACK prMlmeCallback,
                             PR_QIND_CALLBACK prMcpsCallback,
                             PR_HWQINT_CALLBACK prHwCallback);
PUBLIC MAC_MlmeDcfmInd_s *psAppQApiReadMlmeInd(void);
PUBLIC MAC_McpsDcfmInd_s *psAppQApiReadMcpsInd(void);
PUBLIC AppQApiHwInd_s *psAppQApiReadHwInd(void);
PUBLIC void vAppQApiReturnMlmeIndBuffer(MAC_MlmeDcfmInd_s *psBuffer);
PUBLIC void vAppQApiReturnMcpsIndBuffer(MAC_McpsDcfmInd_s *psBuffer);
PUBLIC void vAppQApiReturnHwIndBuffer(AppQApiHwInd_s *psBuffer);
PUBLIC void vAppQApiPostHwInt(uint32 u32Device, uint32 u32ItemBitmap);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* APP_Q_API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/

