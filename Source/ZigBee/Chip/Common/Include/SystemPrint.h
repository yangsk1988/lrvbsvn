/****************************************************************************
 *
 * MODULE:             Printf Header
 *
 * COMPONENT:          $RCSfile: SystemPrint.h,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.3 $
 *
 * DATED:              $Date: 2008/10/22 10:27:00 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             Wayne Ellis
 *
 * DESCRIPTION:
 * System Print Mechanism Header File
 *
 * LAST MODIFIED BY:   $Author: pjtw $
 *                     $Modtime: $
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

#ifndef SYSTEMPRINT_H_INCLUDED
#define SYSTEMPRINT_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/

#include <jendefs.h>
#include <Utilities.h>
#ifdef PRINT_LCD
#include <LcdDriver.h>
#endif

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/* API Platform dependent DISPLAY MACRO'S */

/* UART PRINT option */
#if(defined PRINT_UART)
#define _PRINT_INIT()                                   vUTIL_UartInit()
#define _CLEAR()
#define _WRITE_TEXT_TO_CLEAR_LINE(string, row, col)     vUTIL_UartTextToNewLine(string)
#define _WRITE_TEXT_TO_LINE(string, row, col)           vUTIL_UartText(string)
#define _UPDATE_TEXT()
#define _CONVERT_NUM_TO_STR(num, str)                   vUTIL_NumToString(num, str)

/* LCD PRINT option */
#elif(defined PRINT_LCD)
#define _PRINT_INIT()                                   vLcdResetDefault()
#define _CLEAR()                                        vLcdClear()
#define _WRITE_TEXT_TO_CLEAR_LINE(string, row, col)     vLcdWriteTextToClearLine(string, row, col)
#define _WRITE_TEXT_TO_LINE(string, row, col)           vLcdWriteText(string, row, col)
#define _UPDATE_TEXT()                                  vLcdRefreshAll()
#define _CONVERT_NUM_TO_STR(num, str)                   vUTIL_NumToString(num, str)

/* No PRINT option */
#else
#define _PRINT_INIT()
#define _CLEAR()
#define _WRITE_TEXT_TO_CLEAR_LINE(string, row, col)
#define _WRITE_TEXT_TO_LINE(string, row, col)
#define _UPDATE_TEXT()
#define _CONVERT_NUM_TO_STR(num, str)

#endif
/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif /* SYSTEMPRINT_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/


