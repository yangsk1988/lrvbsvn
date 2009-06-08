/****************************************************************************
 *
 * MODULE:             Utilities Code
 *
 * COMPONENT:          $RCSfile: utils.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.1 $
 *
 * DATED:              $Date: 2008-08-18 12:21:54 $
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


/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/
#include "jendefs.h"
#include "AppHardwareApi.h"
#include "utils.h"
#include "AppApi.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define STARTUP_DELAY                   25000       // arbitary counter delay units

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
 * NAME: vUtils_Init
 *
 * DESCRIPTION:
 * Initialises UART
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS:
 * void, never returns
 *
 ****************************************************************************/
PUBLIC void vUtils_Init(void)
{
    static bool_t bReset = FALSE;

    // Enable UART 0: 19200-8-N-1
    vAHI_UartEnable(E_AHI_UART_0);
    vAHI_UartSetClockDivisor(E_AHI_UART_0, E_AHI_UART_RATE_115200);

    // Only reset UART once BEFORE AT-Jenie is initialised
    // Any further calls after AT-Jenie starts up should DO NOTHING as AT-Jenie takes over the UART

    if (bReset == FALSE)
    {
        bReset = TRUE;
        vAHI_UartReset(E_AHI_UART_0, TRUE, TRUE);
        vAHI_UartReset(E_AHI_UART_0, FALSE, FALSE);
    }
}

/****************************************************************************
 *
 * NAME: vUtils_DisplayMsg
 *
 * DESCRIPTION:
 * Used to display text plus number
 *
 * PARAMETERS:      Name            RW  Usage
 *                  pcMessage       R   Message to display
 *                  u32Data         R   Data to display
 *
 * RETURNS:
 * void, never returns
 *
 ****************************************************************************/
PUBLIC void vUtils_DisplayMsg(char *pcMessage, uint32 u32Data)
{
    vUtils_Debug(pcMessage);
    vUtils_DisplayHex(u32Data, 8);
}

/****************************************************************************
 *
 * NAME: vUtils_Debug
 *
 * DESCRIPTION:
 * Sends a string to UART0 using the hardware API with CR
 *
 * PARAMETERS:      Name            RW  Usage
 *                  pcMessage       R   Null-terminated message to send
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_Debug(char *pcMessage)
{
    vUtils_String(pcMessage);

    vAHI_UartWriteData(0, '\r');
    while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);

    vAHI_UartWriteData(0, '\n');
    while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
}

/****************************************************************************
 *
 * NAME: vUtils_String
 *
 * DESCRIPTION:
 * Sends a string to UART0 using the hardware API without CR
 *
 * PARAMETERS:      Name            RW  Usage
 *                  pcMessage       R   Null-terminated message to send
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_String(char *pcMessage)
{
    while (*pcMessage)
    {
        while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
        vAHI_UartWriteData(0, *pcMessage);
        pcMessage++;
    }
}

/****************************************************************************
 *
 * NAME: vUtils_DisplayHex
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32Data         R   Value to send
 *                  iSize           R   Length of value
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_DisplayHex(uint32 u32Data, int iSize)
{
    char acValue[9];

    vUtils_ValToHex(acValue, u32Data, 8);
    vUtils_Debug(acValue);
}

/****************************************************************************
 *
 * NAME: vUtils_DisplayDec
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32Data         R   Value to send
 *                  iSize           R   Length of value
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_DisplayDec(uint8 u8Data)
{
    char acValue[3];

    vUtils_ValToDec(acValue, u8Data);
    vUtils_Debug(acValue);
}

/****************************************************************************
 *
 * NAME: vUtils_DisplayHex
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32Data         R   Value to send
 *                  iSize           R   Length of value
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_ValToHex(char *pcString, uint32 u32Data, int iSize)
{
    uint8 u8Nybble;
    int i, j;

    j = 0;
    for (i = (iSize << 2) - 4; i >= 0; i -= 4)
    {
        u8Nybble = (uint8)((u32Data >> i) & 0x0f);
        u8Nybble += 0x30;
        if (u8Nybble > 0x39)
            u8Nybble += 7;

        *pcString = u8Nybble;
        pcString++;
    }
    *pcString = '\0';
}

/****************************************************************************
 *
 * NAME: vUtils_ValToDec
 *
 * DESCRIPTION:
 * Converts an 8-bit value to a string of the textual decimal representation.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  pcOutString     R   Location for new string
 *                  u8Value         R   Value to convert
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_ValToDec(char *pcOutString, uint8 u8Value)
{
    static const uint8 au8Digits[3] = {100, 10, 1};
    uint8 u8Digit;
    uint8 u8DigitIndex;
    uint8 u8Count;
    bool_t boPreviousDigitPrinted = FALSE;

    for (u8DigitIndex = 0; u8DigitIndex < 3; u8DigitIndex++)
    {
        u8Count = 0;
        u8Digit = au8Digits[u8DigitIndex];
        while (u8Value >= u8Digit)
        {
            u8Value -= u8Digit;
            u8Count++;
        }

        if ((u8Count != 0) || (boPreviousDigitPrinted == TRUE)
            || (u8DigitIndex == 2))
        {
            *pcOutString = '0' + u8Count;
            boPreviousDigitPrinted = TRUE;
            pcOutString++;
        }
    }
    *pcOutString = '\0';
}

/****************************************************************************
 *
 * NAME: vUtils_Val16ToDec
 *
 * DESCRIPTION:
 * Converts an 16-bit value to a string of the textual decimal representation.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  pcOutString     R   Location for new string
 *                  u8Value         R   Value to convert
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_Val16ToDec(char *pcOutString, uint16 u16Value)
{
    static const uint16 au16Digits[5] = {10000, 1000, 100, 10, 1};
    uint16 u16Digit;
    uint8 u8DigitIndex;
    uint8 u8Count;
    bool_t boPreviousDigitPrinted = FALSE;

    for (u8DigitIndex = 0; u8DigitIndex < 5; u8DigitIndex++)
    {
        u8Count = 0;
        u16Digit = au16Digits[u8DigitIndex];
        while (u16Value >= u16Digit)
        {
            u16Value -= u16Digit;
            u8Count++;
        }

        if ((u8Count != 0) || (boPreviousDigitPrinted == TRUE)
            || (u8DigitIndex == 4))
        {
            *pcOutString = '0' + u8Count;
            boPreviousDigitPrinted = TRUE;
            pcOutString++;
        }
    }
    *pcOutString = '\0';
}

/****************************************************************************
 *
 * NAME: vUtils_DisplayBytes
 *
 * DESCRIPTION:
 *
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_DisplayBytes(uint8 *pcOutString, uint8 u8Num)
{
uint8 chr;
uint16 col=0;
    while(u8Num--)
    {
        chr=(*pcOutString >> 4)+0x30;
        if(chr > 0x39)
        {
            chr+=0x07;
        }
        vAHI_UartWriteData(0,chr);
        while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
        chr=(*pcOutString & 0x0f)+0x30;
        if(chr > 0x39)
        {
            chr+=0x07;
        }
        vAHI_UartWriteData(0,chr);
        while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
        col++;
        if(col % 16 == 0)
        {
            vAHI_UartWriteData(0, '\r');
            while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);

            vAHI_UartWriteData(0, '\n');
            while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
            col=0;
        }
        else
        {
            vAHI_UartWriteData(0,' ');
            while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
        }
        pcOutString++;
    }
    vAHI_UartWriteData(0, '\r');
    while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
    vAHI_UartWriteData(0, '\n');
    while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
}

/****************************************************************************
 *
 * NAME: vUtils_DisplayCharacter
 *
 * DESCRIPTION:
 * Sends a single character to UART0 using the hardware API without CR
 *
 * PARAMETERS:      Name            RW  Usage
 *                  pcMessage       R   Null-terminated message to send
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vUtils_DisplayChar(char cCharOut)
{
    vAHI_UartWriteData(0, cCharOut);
    while ((u8AHI_UartReadLineStatus(0) & 0x20) == 0);
}

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/


/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
