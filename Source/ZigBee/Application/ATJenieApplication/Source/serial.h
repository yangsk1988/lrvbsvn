/****************************************************************************
 *
 * MODULE:             Serial
 *
 * COMPONENT:          $RCSfile: serial.h,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.3 $
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
 * Revision 1.2  2007/09/25 09:03:21  mwild
 * Updated comments and headers
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


#ifndef  SERIAL_H_INCLUDED
#define  SERIAL_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#define CR_CHAR   0x0DU
#define LF_CHAR   0x0AU

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vSerial_Init(void);
PUBLIC bool_t bSerial_TxChar(uint8 u8Chr);
PUBLIC int16 i16Serial_RxChar(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* SERIAL_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
