/****************************************************************************
 *
 * MODULE:             UART
 *
 * COMPONENT:          $RCSfile: uart.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.7 $
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
 * Revision 1.6  2008/06/03 07:14:51  rsmit
 * 'SetBaudRate'  now uses API calls rather than Direct Memory Access
 *
 * Revision 1.5  2007/10/09 15:14:52  mwild
 * Added call toRTCCTS control enable function
 *
 * Revision 1.4  2007/10/08 08:15:15  mwild
 * Implemented data rate setting
 * Implemented configuration in flash
 *
 * Revision 1.3  2007/09/25 09:03:21  mwild
 * Updated comments and headers
 *
 * Revision 1.2  2007/08/21 15:13:53  mwild
 * Removed LCD debug
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

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/

#include <jendefs.h>
#include "AppHardwareApi.h"

#include "uart.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/* default to uart 0 */
#ifndef UART
#define UART E_AHI_UART_0
#endif

#if (UART != E_AHI_UART_0 && UART != E_AHI_UART_1)
#error UART must be either 0 or 1
#endif

/* default BAUD rate 9600 */
#ifndef UART_BAUD_RATE
#define UART_BAUD_RATE        9600
#endif

#define UART_LCR_OFFSET     0x0C
#define UART_DLM_OFFSET     0x04

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/

PRIVATE void vUART_HandleUartInterrupt(uint32 u32Device, uint32 u32ItemBitmap);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/

/* callbacks */
void (*gpfRxFn)(uint8) = NULL;
void (*gpfTxRdy)(void) = NULL;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vUART_Init
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/
PUBLIC void vUART_Init(void (*pfRxFn)(uint8), void (*pfTxRdy)(void))
{
    gpfRxFn = pfRxFn;
    gpfTxRdy = pfTxRdy;

    /* Enable UART 0 */
    vAHI_UartEnable(UART);

    vAHI_UartReset(UART, TRUE, TRUE);
    vAHI_UartReset(UART, FALSE, FALSE);

#if (UART == E_AHI_UART_0)
    vAHI_Uart0RegisterCallback(vUART_HandleUartInterrupt);
#elif (UART == E_AHI_UART_1)
    vAHI_Uart1RegisterCallback(vUART_HandleUartInterrupt);
#endif

    /* Set the clock divisor register to give required buad, this has to be done
       directly as the normal routines (in ROM) do not support all baud rates */
    vUART_SetBaudRate(UART_BAUD_RATE);

    vAHI_UartSetRTSCTS(UART, TRUE);
    vAHI_UartSetControl(UART, FALSE, FALSE, E_AHI_UART_WORD_LEN_8, TRUE, FALSE); /* [I SP001222_P1 279] */
    vAHI_UartSetInterrupt(UART, FALSE, FALSE, FALSE, TRUE, E_AHI_UART_FIFO_LEVEL_1);    // No TX ints!
}

/****************************************************************************
 *
 * NAME: vUART_SetBuadRate
 *
 * DESCRIPTION:
 *
 * PARAMETERS: Name        RW  Usage
 *
 * RETURNS:
 *
 ****************************************************************************/

PUBLIC void vUART_SetBaudRate(uint32 u32BaudRate)
{
    uint16 u16Divisor;
    uint32 u32Remainder;

    /* Divisor  = 16MHz / (16 x baud rate) */
    u16Divisor = (uint16)(16000000UL / (16UL * u32BaudRate));

    /* Correct for rounding errors */
    u32Remainder = (uint32)(16000000UL % (16UL * u32BaudRate));

    if (u32Remainder >= ((16UL * u32BaudRate) / 2))
    {
        u16Divisor += 1;
    }

    vAHI_UartSetBaudDivisor(UART, u16Divisor);
}

/****************************************************************************
 *
 * NAME: vUART_HandleUart0Interrupt
 *
 * DESCRIPTION:
 *
 * PARAMETERS:      Name            RW  Usage
 * None.
 *
 * RETURNS:
 * None.
 *
 * NOTES:
 * None.
 ****************************************************************************/

PRIVATE void vUART_HandleUartInterrupt(uint32 u32Device, uint32 u32ItemBitmap)
{
#if (UART == E_AHI_UART_0)
    if (u32Device == E_AHI_DEVICE_UART0)
    {
        if ((u32ItemBitmap & 0x000000FF) == E_AHI_UART_INT_RXDATA)
        {
            if (NULL != gpfRxFn)
            {
                gpfRxFn(u8AHI_UartReadData(E_AHI_UART_0));
            }
        }
        else if (u32ItemBitmap == E_AHI_UART_INT_TX)
        {
            if (NULL != gpfTxRdy)
            {
                gpfTxRdy();
            }
        }
    }
#elif (UART == E_AHI_UART_1)
    if (u32Device == E_AHI_DEVICE_UART1)
    {
        if ((u32ItemBitmap & 0x000000FF) == E_AHI_UART_INT_RXDATA)
        {
            if (NULL != gpfRxFn)
            {
                gpfRxFn(u8AHI_UartReadData(E_AHI_UART_1));
            }
        }
        else if (u32ItemBitmap == E_AHI_UART_INT_TX)
        {
            if (NULL != gpfTxRdy)
            {
                gpfTxRdy();
            }
        }
    }
#endif
}

/****************************************************************************
 *
 * NAME: vUART_RtsStopFlow
 *
 * DESCRIPTION:
 * Set UART RS-232 RTS line high to stop any further data coming in
 *
 ****************************************************************************/

/* [I SP001222_P1 283] begin */
PUBLIC void vUART_RtsStopFlow(void)
{
    vAHI_UartSetControl(UART, FALSE, FALSE, E_AHI_UART_WORD_LEN_8, TRUE, E_AHI_UART_RTS_HIGH);
}
/* [I SP001222_P1 283] end */

/****************************************************************************
 *
 * NAME: vUART_RtsStartFlow
 *
 * DESCRIPTION:
 * Set UART RS-232 RTS line low to allow further data
 *
 ****************************************************************************/

/* [I SP001222_P1 283] begin */
PUBLIC void vUART_RtsStartFlow(void)
{
    vAHI_UartSetControl(UART, FALSE, FALSE, E_AHI_UART_WORD_LEN_8, TRUE, E_AHI_UART_RTS_LOW);
}
/* [I SP001222_P1 283] end */

/****************************************************************************
 *
 * NAME: vUART_TxChar
 *
 * DESCRIPTION:
 * Set UART RS-232 RTS line low to allow further data
 *
 ****************************************************************************/
PUBLIC void vUART_TxChar(uint8 u8Char)
{
    vAHI_UartWriteData(UART, u8Char);
}

/****************************************************************************
 *
 * NAME: vUART_TxReady
 *
 * DESCRIPTION:
 * Set UART RS-232 RTS line low to allow further data
 *
 ****************************************************************************/
PUBLIC bool_t bUART_TxReady()
{
    return u8AHI_UartReadLineStatus(UART) & E_AHI_UART_LS_THRE;
}

/****************************************************************************
 *
 * NAME: vUART_SetTxInterrupt
 *
 * DESCRIPTION:
 * Enable / disable the tx interrupt
 *
 ****************************************************************************/
PUBLIC void vUART_SetTxInterrupt(bool_t bState)
{
    vAHI_UartSetInterrupt(UART, FALSE, FALSE, bState, TRUE, E_AHI_UART_FIFO_LEVEL_1);
}

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
