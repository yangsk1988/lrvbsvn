/****************************************************************************
 *
 * MODULE:             N60_Utils
 *
 * COMPONENT:          $RCSfile: utils.h,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.1 $
 *
 * DATED:              $Date: 2008-08-18 12:22:01 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             GPfef
 *
 * DESCRIPTION:
 *
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

#ifndef UTILS_H
#define UTILS_H

#ifdef __cplusplus
extern "C" {
#endif  //__cplusplus

/*****************************************************************************************
**  INCLUDE FILES
*****************************************************************************************/

/*****************************************************************************************
**  MACROS
*****************************************************************************************/

/*****************************************************************************************
**  CONSTANTS
*****************************************************************************************/

/*****************************************************************************************
**  TYPEDEFS
*****************************************************************************************/

/*****************************************************************************************
**  EXTERNAL VARIABLES
*****************************************************************************************/

/*****************************************************************************************
**  GLOBAL VARIABLES
*****************************************************************************************/

/*****************************************************************************************
**  LOCAL VARIABLES
*****************************************************************************************/

/*****************************************************************************************
**  EXPORTED FUNCTIONS
*****************************************************************************************/
PUBLIC void vUtils_Init(void);
PUBLIC void vUtils_DisplayHex(uint32 u32Data, int iSize);
PUBLIC void vUtils_DisplayDec(uint8 u8Data);
PUBLIC void vUtils_Debug(char *pcMessage);
PUBLIC void vUtils_DisplayMsg(char *pcMessage, uint32 u32Data);
PUBLIC void vUtils_String(char *pcMessage);
PUBLIC void vUtils_ValToHex(char *pcOutString, uint32 u32Data, int iSize);
PUBLIC void vUtils_ValToDec(char *pcOutString, uint8 u8Value);
PUBLIC void vUtils_Val16ToDec(char *pcOutString, uint16 u16Value);
PUBLIC void vUtils_DisplayBytes(uint8 *pcOutString, uint8 u8Num);
PUBLIC void vUtils_DisplayChar(char cCharOut);
PUBLIC void RandomDelay(void);

/*****************************************************************************************
**  LOCAL FUNCTIONS DESC
*****************************************************************************************/

/*****************************************************************************************
**  LOCAL FUNCTIONS
*****************************************************************************************/

#ifdef __cplusplus
}
#endif  //__cplusplus

#endif  // UTILS_H

/*****************************************************************************************
**  END OF FILE
*****************************************************************************************/

