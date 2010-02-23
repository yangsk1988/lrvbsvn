/*****************************************************************************
 *
 * MODULE:              Interrupt.h
 *
 * COMPONENT:           $RCSfile: Interrupt.h,v $
 *
 * VERSION:             $Name: not supported by cvs2svn $
 *
 * REVISION:            $Revision: 1.3 $
 *
 * DATED:               $Date: 2008-07-07 16:46:56 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              MRW
 *
 * DESCRIPTION:         Interrupt enabling / disabling
 *
 * $Log: not supported by cvs2svn $
 * Revision 1.2  2007/09/25 09:03:21  mwild
 * Updated comments and headers
 *
 * 
 * LAST MODIFIED BY:    $Author: rsmit $
 *                      $Modtime: $
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

#ifndef  INTERRUPT_H_INCLUDED
#define  INTERRUPT_H_INCLUDED

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

PUBLIC void vInterrupt_Suspend(void);
PUBLIC void vInterrupt_Resume(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/


