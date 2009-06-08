/****************************************************************************
 *
 * MODULE:             MCP9803 Temperature sensor driver
 *
 * COMPONENT:          $RCSfile: MCP9803.c,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.3 $
 *
 * DATED:              $Date: 2008/02/29 18:02:05 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             Lee Mitchell
 *
 * DESCRIPTION:
 * A driver for the Microchip MCP9803 temperature sensor IC
 *
 * CHANGE HISTORY:
 *
 * $Log: MCP9803.c,v $
 * Revision 1.3  2008/02/29 18:02:05  dclar
 * dos2unix
 *
 * Revision 1.2  2007/01/22 10:01:55  lmitch
 * Added function descriptions
 * Corrected return value type
 *
 * Revision 1.1  2006/12/08 10:51:17  lmitch
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
#include <AppHardwareApi.h>
#include <SMBus.h>
#include <MCP9803.h>

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#define MCP9803_ADDRESS			0x48

#define MCP9803_TEMPERATURE_REG	0x00
#define MCP9803_CONFIG_REG		0x01
#define MCP9803_HYSTERISIS_REG	0x02
#define MCP9803_LIMITS_REG		0x03


#define MCP9803_CONFIG			0x60

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
 * NAME:       bMCP9803_Init
 *
 * DESCRIPTION:
 * Initialises the MCP9803 temperature sensor IC
 *
 * PARAMETERS: 	None
 *
 * RETURNS:
 * bool_t: 	TRUE if initialisation ok
 *			FALSE if initialisation failed
 *
 ****************************************************************************/
PUBLIC bool_t bMCP9803_Init(void)
{

	bool_t bOk = TRUE;

	uint8 u8Mode = MCP9803_CONFIG;

	/* run bus at 400KHz */
	vAHI_SiConfigure(TRUE, FALSE, 31);

	bOk &= bSMBusWrite(MCP9803_ADDRESS, MCP9803_CONFIG_REG, 1, &u8Mode);

	return(bOk);
}


/****************************************************************************
 *
 * NAME:       bMCP9803_Read
 *
 * DESCRIPTION:
 * Reads the temperature from the MCP9803 IC.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  psResult	    W   pointer to a tsMCP9803_Result struct
 *
 * RETURNS:
 * bool_t: 	TRUE if temperature read ok
 *			FALSE if temperature read failed
 *
 ****************************************************************************/
PUBLIC bool_t bMCP9803_Read(tsMCP9803_Result *psResult)
{

	bool_t bOk = TRUE;
	uint8 au8Data[2];
	int16 i16SignedResult = 0;


	bOk &= bSMBusWrite(MCP9803_ADDRESS, MCP9803_TEMPERATURE_REG, 0, NULL);

	bOk &= bSMBusSequentialRead(MCP9803_ADDRESS, 2, au8Data);

	if(!bOk) return(FALSE);

	i16SignedResult = au8Data[0];
	i16SignedResult <<= 8;
	i16SignedResult |= au8Data[1];

	/* lose the unwanted bits */
	i16SignedResult >>= 4;

	if(i16SignedResult & 0x800){
		i16SignedResult |= 0xf000;
		psResult->bPositive = FALSE;
		psResult->u8Temperature = ((~i16SignedResult) + 1) >> 4;
		psResult->u16Fraction = (((~i16SignedResult) + 1) & 0xf);
	} else {
		psResult->bPositive = TRUE;
		psResult->u8Temperature = i16SignedResult >> 4;
		psResult->u16Fraction = (i16SignedResult & 0xf);
	}


	psResult->u16Fraction = psResult->u16Fraction * 625;

	psResult->i8Temperature = i16SignedResult >> 4;

	psResult->i16Temperature = i16SignedResult;

	return(bOk);

}
/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
