/*****************************************************************************
 *
 * MODULE:              ATJTunnel.h
 *
 * COMPONENT:           $RCSfile: ATJTunnel.h,v $
 *
 * VERSION:             $Name:  $
 *
 * REVISION:            $Revision: 1.3 $
 *
 * DATED:               $Date: 2007/10/18 13:53:10 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              MRW
 *
 * DESCRIPTION:         AT Jenie command tunnelling
 *
 * LAST MODIFIED BY:    $Author: mwild $
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

#ifndef  ATJTUNNEL_H_INCLUDED
#define  ATJTUNNEL_H_INCLUDED

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

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vATJ_TunnelInit(tsATJChannelDescriptor *psUartChannel, uint8 u8LocalServerSvc);
PUBLIC bool_t bATJ_TunnelCommand(teEventType eEventType, void *pvEventPrim);
PUBLIC void vATJ_TunnelSetCommandEnable(char *CmdsStr, bool_t bEnabled);

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


