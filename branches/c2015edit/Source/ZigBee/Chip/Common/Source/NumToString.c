/****************************************************************************
 *
 * MODULE:             Driver for UC1601 and UC1606 LCD Driver-Controller
 *
 * COMPONENT:          $RCSfile: NumToString.c,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.3 $
 *
 * DATED:              $Date: 2008/10/22 10:27:00 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             CJG
 *
 * DESCRIPTION:
 * Provides API for driving LCD panels using UC1601 or UC1606 LCD
 * Driver-Controller.
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

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/
#include "jendefs.h"
#include "Utilities.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
/****************************************************************************
 *
 * NAME: vNumToString
 *
 * DESCRIPTION:
 * Converts a 32 bit value to a hexadecimal string.
 *
 * PARAMETERS:  Name      RW  Usage
 *              u32Data   R   Value to convert
 *              pcString  W   Store for string. Must be at least 9 characters
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUTIL_NumToString(uint32 u32Data, char *pcString)
{
    int    i;
    uint8  u8Nybble;

    for (i = 28; i >= 0; i -= 4)
    {
        u8Nybble = (uint8)((u32Data >> i) & 0x0f);
        u8Nybble += 0x30;
        if (u8Nybble > 0x39)
            u8Nybble += 7;

        *pcString = u8Nybble;
        pcString++;
    }
    *pcString = 0;
}

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
