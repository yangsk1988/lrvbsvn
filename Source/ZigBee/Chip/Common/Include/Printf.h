/****************************************************************************
 *
 * MODULE:             Printf Header
 *
 * COMPONENT:          $RCSfile: Printf.h,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.3 $
 *
 * DATED:              $Date: 2008/10/22 10:27:00 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             LJM
 *
 * DESCRIPTION:
 * Printf header file
 *
 * CHANGE HISTORY:
 *
 * $Log: Printf.h,v $
 * Revision 1.3  2008/10/22 10:27:00  pjtw
 * Added disclaimer
 *
 * Revision 1.2  2008/02/29 18:19:35  dclar
 * dos2unix
 *
 * Revision 1.1  2006/12/07 15:56:25  imorr
 * Initial version
 *
 * Revision 1.4  2006/02/23 16:12:06  we1
 * added __cplusplus check and vUART_printInit() function
 *
 * Revision 1.3  2006/02/23 12:25:19  we1
 * added bWaitForKey on UART init
 *
 * Revision 1.2  2006/02/03 14:34:53  we1
 * added UART functions required for Printf operation
 *
 * Revision 1.1  2006/01/30 13:48:14  we1
 * first check in
 *
 * Revision 1.1  2006/01/13 09:54:48  lmitch
 * Initial revision
 *
 *
 *
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
 * Copyright Jennic Ltd 2004, 2005, 2006. All rights reserved
 *
 ***************************************************************************/

#ifndef PRINTF_H_INCLUDED
#define PRINTF_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/

#include <jendefs.h>

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vUART_printInit(void);

PUBLIC void vInitPrintf(void (*fp)(char c));
PUBLIC void vPrintf(const char *fmt, ...);

PUBLIC void vPutC(unsigned char c);
PUBLIC void vUART_Init(bool bWaitForKey);

#if defined __cplusplus
}
#endif

#endif /* PRINTF_H_INCLUDED */


