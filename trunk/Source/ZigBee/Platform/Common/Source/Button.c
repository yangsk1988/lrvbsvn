/****************************************************************************
 *
 * MODULE:             Demo board button controls
 *
 * COMPONENT:          $RCSfile: Button.c,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.4 $
 *
 * DATED:              $Date: 2009/01/15 16:17:54 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             Lee Mitchell
 *
 * DESCRIPTION:
 * Functions to make it easier to read buttons on demo boards
 *
 * CHANGE HISTORY:
 *
 * $Log: Button.c,v $
 * Revision 1.4  2009/01/15 16:17:54  dclar
 * error in source file. DEVKIT2 should be PCB_DEVKIT2
 *
 * Revision 1.3  2008/02/29 18:02:04  dclar
 * dos2unix
 *
 * Revision 1.2  2008/02/29 14:12:01  dclar
 * #ifdef is incorrect
 *
 * Revision 1.1  2006/12/08 10:51:16  lmitch
 * Added to repository
 *
 *
 *
 * LAST MODIFIED BY:   $Author: dclar $
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

#include <jendefs.h>
#include <Button.h>
#include <AppHardwareApi.h>

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

#ifdef PCB_DEVKIT2

PUBLIC uint8 u8ButtonReadRfd(void)
{

	uint32 u32DioPins;
	uint8 u8RetVal = 0;

	u32DioPins = (u32AHI_DioReadInput() & BUTTON_ALL_MASK_RFD_PIN) ^ BUTTON_ALL_MASK_RFD_PIN;

	if(u32DioPins & BUTTON_0_PIN) u8RetVal |= BUTTON_0_MASK;
	if(u32DioPins & BUTTON_1_PIN) u8RetVal |= BUTTON_1_MASK;

	return(u8RetVal);

}



PUBLIC uint8 u8ButtonReadFfd(void)
{

	uint32 u32DioPins;
	uint8 u8RetVal = 0;

	u32DioPins = (u32AHI_DioReadInput() & BUTTON_ALL_MASK_FFD_PIN) ^ BUTTON_ALL_MASK_FFD_PIN;

	if(u32DioPins & BUTTON_0_PIN) u8RetVal |= BUTTON_0_MASK;
	if(u32DioPins & BUTTON_1_PIN) u8RetVal |= BUTTON_1_MASK;
	if(u32DioPins & BUTTON_2_PIN) u8RetVal |= BUTTON_2_MASK;
	if(u32DioPins & BUTTON_3_PIN) u8RetVal |= BUTTON_3_MASK;

	return(u8RetVal);

}

#endif

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/


/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
