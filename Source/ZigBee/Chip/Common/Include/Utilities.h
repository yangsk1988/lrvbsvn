/*****************************************************************************
 *
 * MODULE:              Utility functions for string handling
 *
 * COMPONENT:           $RCSfile: Utilities.h,v $
 *
 * VERSION:             $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:            $Revision: 1.4 $
 *
 * DATED:               $Date: 2008/10/22 10:27:00 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              CJG
 *
 * DESCRIPTION:
 * Utility functions for string handling.
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

#ifndef  NUM_TO_STRING_INCLUDED
#define  NUM_TO_STRING_INCLUDED

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

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
#ifdef CHIP_RELEASE_5121
typedef void *(*tprmemcpy)(void *s1, const void *s2, long unsigned int n);
typedef int (*tprmemcmp)(const void * cs, const void * ct, long unsigned int count);
typedef void *(*tprmemset)(void *s, int c, long unsigned int n);
#endif

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
/* Number conversion */
PUBLIC void vUTIL_NumToString(uint32 u32Data, char *pcString);

/* UART printing */
PUBLIC void vUTIL_UartInit(void);
PUBLIC void vUTIL_UartText(char *pcString);
PUBLIC void vUTIL_UartTextToNewLine(char *pcString);

/* Standard memcpy, memset, memcmp but defined to be linked from ROM for
   JN5121 */
#ifdef CHIP_RELEASE_5121
#define memcpy ((tprmemcpy) 0xf9ec)
#define memcmp ((tprmemcmp) 0xfb04)
#define memset ((tprmemset) 0xfb5c)
#else
PUBLIC void *memcpy(void *s1, const void *s2, long unsigned int n);
PUBLIC int   memcmp(const void * cs, const void * ct, long unsigned int count);
PUBLIC void *memset(void *s, int c, long unsigned int n);
#endif

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* NUM_TO_STRING_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/

