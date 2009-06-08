/****************************************************************************
 *
 * MODULE:             UART
 *
 * COMPONENT:          $RCSfile: uart.h,v $
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
 * DESCRIPTION:        Hardware abstarction layer for UART peripheral
 *
 * CHANGE HISTORY:
 *
 * $Log: not supported by cvs2svn $
 * Revision 1.3  2007/10/08 08:15:15  mwild
 * Implemented data rate setting
 * Implemented configuration in flash
 *
 * Revision 1.2  2007/09/25 09:03:21  mwild
 * Updated comments and headers
 *
 * Revision 1.1  2007/08/16 09:40:58  mwild
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


#ifndef  UART_H_INCLUDED
#define  UART_H_INCLUDED

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

PUBLIC void vUART_Init(void (*pfRxFn)(uint8), void (*pfTxRdy)(void));
PUBLIC void vUART_TxChar(uint8 u8RxChar);
PUBLIC bool_t bUART_TxReady(void);
PUBLIC void vUART_RtsStartFlow(void);
PUBLIC void vUART_RtsStopFlow(void);
PUBLIC void vUART_SetTxInterrupt(bool_t bState);
PUBLIC void vUART_SetBaudRate(uint32 u32BaudRate);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* UART_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/


