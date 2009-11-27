/****************************************************************************
 *
 * MODULE:             Interrupt
 *
 * COMPONENT:          $RCSfile: Interrupt.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.4 $
 *
 * DATED:              $Date: 2008-07-07 16:46:56 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             MRW
 *
 * DESCRIPTION         Manage interrupt enabling / disabling
 *
 * CHANGE HISTORY:
 *
 * $Log: not supported by cvs2svn $
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

#include "interrupt.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#define SR_IEE  (4)
#define SR_TEE  (2)

#define GET_SR(reg)  { asm volatile ("l.mfspr %0,r0,0x0011" :"=r"(reg) : ); }

#define CLEAR_SR_BITS(bits) { \
    register unsigned int reg; \
    asm volatile ("l.mfspr %0,r0,0x0011" :"=r"(reg) : ); \
    reg &= ~(bits); \
    asm volatile ("l.mtspr r0,%0,0x0011" : : "r"(reg) ); \
}

#define SET_SR_BITS(bits) { \
    register unsigned int reg; \
    asm volatile ("l.mfspr %0,r0,0x0011" :"=r"(reg) : ); \
    reg |= (bits); \
    asm volatile ("l.mtspr r0,%0,0x0011" : : "r"(reg) ); \
}

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

volatile uint32 gu32Counter;
uint32 gu32_SR;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vInterrupt_Suspend
 *
 * DESCRIPTION:
 * Disables interrupts if they were not already disabled
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

PUBLIC void vInterrupt_Suspend(void)
{
    if (0 == gu32Counter++) {
        register uint32 u32SR;
        GET_SR(u32SR);
        gu32_SR = u32SR;
        if (gu32_SR & (SR_IEE | SR_TEE)) {
            CLEAR_SR_BITS(SR_IEE | SR_TEE);
        }
    }
}

/****************************************************************************
 *
 * NAME: vInterrupt_Resume
 *
 * DESCRIPTION:
 * Enables interrupts if they were not already enabled
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

PUBLIC void vInterrupt_Resume(void)
{
    if (gu32Counter) {
        if (0 == --gu32Counter) {
            SET_SR_BITS(gu32_SR & (SR_IEE | SR_TEE));
        }
    } else {
        /* interrupt enable / disable  underflow */
        while (1);
    }
}

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/

