/****************************************************************************
 *
 * MODULE:             Application Hardware API
 *
 * COMPONENT:          $RCSfile: HyperterminalPrint.h,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.3 $
 *
 * DATED:              $Date: 2008/10/22 10:27:00 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             Mike Scott
 *
 * DESCRIPTION:
 * Basic Hyperterminal Print Functions
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

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vHTWriteText(char *pcString, uint8 u8Row, uint8 u8Column);

PUBLIC void vHTWriteTextToClearLine(char *pcString, uint8 u8Row, uint8 u8Column);

PUBLIC void vHTRefreshAll(void);

PUBLIC void vHTReset(uint8 u8Bias, uint8 u8Gain);

PUBLIC void vHTClear(void);

PUBLIC void vHTClearLine(uint8 u8Row);

/****************************************************************************/
/*          End Of File                                                     */
/*****************************************************************************/
