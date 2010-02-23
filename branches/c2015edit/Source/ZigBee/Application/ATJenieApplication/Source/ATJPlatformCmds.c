/****************************************************************************
 *
 * MODULE:             ATJPlatformCmds.c
 *
 * COMPONENT:          $RCSfile: ATJPlatformCmds.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.8 $
 *
 * DATED:              $Date: 2008-08-07 10:49:51 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             MRW
 *
 * DESCRIPTION:        AT-Jenie Platform Specific Commands
 *
 * LAST MODIFIED BY:   $Author: rsmit $
 *                     $Modtime: $
 *
 *
 * HISTORY:
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

#include <string.h>

#include "jendefs.h"
#include "atjparser.h"
#include "ledcontrol.h"
#include "alsdriver.h"
#include "button.h"
#ifdef  PCB_NTS
    #include "MCP9803.h"        /* temp sensor chip */
#else
    #include "htsdriver.h"      /* temp/humidity chip */
    #include "lcddriver.h"
#endif

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define ADC_TIMEOUT     3000
#define TEMP_HUMID_TIMEOUT    3000000
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/

PRIVATE uint16  u16ALSreadChannelResult_Wrapper(void);
PRIVATE void    vLEDOn_Wrapper(uint32 u32LED);
PRIVATE void    vLEDOff_Wrapper(uint32 u32LED);
PRIVATE int32   i32GetBoardVoltage(void);
PRIVATE bool_t  bGetBoardValue_Validator(uint64 u64Value, uint8 *au8ParamBuffer);
PRIVATE uint8   u8ButtonReadRfd_Wrapper(void);
PRIVATE uint8   u8ButtonReadFfd_Wrapper(void);
PRIVATE int32   i32GetTemperature(void);
PRIVATE int32   i32GetHumidity(void);
PRIVATE void    vInitADC(uint8 u8Channel);
#ifndef PCB_NTS
PRIVATE void    vLCDWriteText_Wrapper(char *TextStr, uint32 u32Row, uint32 u32Col);
#endif

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/

/* [I SP001222_P1 313] begin */
ATJ_DECLARE_VALIDATOR(LEDs)                   =   ATJ_VALIDATOR_RANGE(0, 3);
#ifndef PCB_NTS
ATJ_DECLARE_VALIDATOR(LCD_Columns)          =   ATJ_VALIDATOR_RANGE(0, 128);
ATJ_DECLARE_VALIDATOR(LCD_Rows)             =   ATJ_VALIDATOR_RANGE(0, 8);
#endif


ATJ_DECLARE_CMD_PARAMS(BLO) = {
    ATJ_CMD_PARAM(E_ATJ_INPUT,  E_ATJ_PARAM, 4, ATJ_PARAM_VAL_RANGE(LEDs)) /* [I SP001222_P1 319] */
};

ATJ_DECLARE_CMD_PARAMS(BLF) = {
    ATJ_CMD_PARAM(E_ATJ_INPUT,  E_ATJ_PARAM, 4, ATJ_PARAM_VAL_RANGE(LEDs)) /* [I SP001222_P1 321] */
};

#ifndef PCB_NTS
ATJ_DECLARE_CMD_PARAMS(BTX) = {
    ATJ_CMD_PARAM(E_ATJ_INPUT,  E_ATJ_PARAM, 40, ATJ_PARAM_VAL_NONE()), /* [I SP001222_P1 323] */
    ATJ_CMD_PARAM(E_ATJ_INPUT,  E_ATJ_PARAM, 4, ATJ_PARAM_VAL_RANGE(LCD_Rows)), /* [I SP001222_P1 331] */
    ATJ_CMD_PARAM(E_ATJ_INPUT,  E_ATJ_PARAM, 4, ATJ_PARAM_VAL_RANGE(LCD_Columns)) /* [I SP001222_P1 330] */
};
#endif


PRIVATE tsATJCommandSet asATJPlatformCommandSet = {
    ATJ_BEGIN_COMMAND_SET

    ATJ_COMMAND_NP(BGV, i32GetBoardVoltage,              E_ATJ_OKP, bGetBoardValue_Validator), /* [I SP001222_P1 314] */
    ATJ_COMMAND_NP(BGT, i32GetTemperature,               E_ATJ_OKP, bGetBoardValue_Validator), /* [I SP001222_P1 315] */
    ATJ_COMMAND_NP(BGL, u16ALSreadChannelResult_Wrapper, E_ATJ_OKP, NULL), /* [I SP001222_P1 316] */
    ATJ_COMMAND_NP(BGH, i32GetHumidity,                  E_ATJ_OKP, bGetBoardValue_Validator), /* [I SP001222_P1 317] */
    ATJ_COMMAND_NP(BGR, u8ButtonReadRfd_Wrapper,         E_ATJ_OKP, NULL), /* [I SP001222_P1 328] */
    ATJ_COMMAND_NP(BGF, u8ButtonReadFfd_Wrapper,         E_ATJ_OKP, NULL), /* [I SP001222_P1 329] */
    ATJ_COMMAND(   BLO, vLEDOn_Wrapper,                  E_ATJ_OK,  NULL), /* [I SP001222_P1 318] */
    ATJ_COMMAND(   BLF, vLEDOff_Wrapper,                 E_ATJ_OK,  NULL), /* [I SP001222_P1 320] */
    #ifndef PCB_NTS
    ATJ_COMMAND(   BTX, vLCDWriteText_Wrapper,           E_ATJ_OK,  NULL), /* [I SP001222_P1 322] */
    #endif

    ATJ_END_COMMAND_SET
};

/* [I SP001222_P1 313] end */

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vATJ_PlatformCommandsInit
 *
 * DESCRIPTION: Initialise AT Jenie Platform Specific Commands
 *
 * HISTORY:
 *
 ****************************************************************************/

PUBLIC void vATJ_PlatformCommandsInit(void)
{
    vALSreset();
    #ifdef PCB_NTS
    bMCP9803_Init();        /* temp sensor */
    vLedInitNts();
    vButtonInitNts();
    #else
    vHTSreset();            /* temp/humidity sensor */
    vLedInitRfd();
    vButtonInitRfd();
    vLcdResetDefault();
    #endif

    vInitADC(E_AHI_ADC_SRC_VOLT);   /* initialize ADC channel for voltage */
    vAHI_AdcStartSample();
    vALSstartReadChannel(0);
    vATJ_ParserAddCommands(&asATJPlatformCommandSet);
}

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: bGetBoardValue_Validator
 *
 * DESCRIPTION:
 * Validates the returned board value by rejecting negative values caused
 * by a time-out during the ADC/sensor read process.
 *
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u64Value        R   Voltage
 *                  *au8ParamBuffer W   Parameter Buffer
 *
 * RETURNS: Valid for postive voltages
 *
 ****************************************************************************/
PRIVATE bool_t bGetBoardValue_Validator(uint64 u64Value, uint8 *au8ParamBuffer)
{
    bool_t bValid = TRUE;
    int32 i32BoardReading = (int32)u64Value;

    if (0 > i32BoardReading) {
        bValid = FALSE;
    }

    return bValid;
}

/****************************************************************************
 *
 * NAME: vInitADC
 *
 * DESCRIPTION:
 * Initialises the ADC channel dedicated for battery voltage
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u8Channel       R   Channel number
 *
 * RETURNS: None
 *
 ****************************************************************************/
PRIVATE void vInitADC(uint8 u8Channel)
{
    vAHI_ApConfigure(E_AHI_AP_REGULATOR_ENABLE,
                     E_AHI_AP_INT_DISABLE,
                     E_AHI_AP_SAMPLE_2,
                     E_AHI_AP_CLOCKDIV_2MHZ,
                     E_AHI_AP_INTREF);

    /* Wait until the analogue peripheral regulator has come up before setting
       the ADC. */
    while(!bAHI_APRegulatorEnabled());

    vAHI_AdcEnable(E_AHI_ADC_CONVERT_DISABLE,
                   E_AHI_AP_INPUT_RANGE_2,
                   u8Channel);
}

/****************************************************************************
 *
 * NAME: u8ButtonReadRfd_Wrapper
 *
 * DESCRIPTION:
 * Calls the appropriate button read function depending on platform
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS: Button state
 *
 ****************************************************************************/
PRIVATE uint8 u8ButtonReadRfd_Wrapper(void)
{
    uint8   u8ButtonState;
    #ifdef PCB_NTS
    u8ButtonState = u8ButtonReadNts();
    #else
    u8ButtonState = u8ButtonReadRfd();
    #endif
    return u8ButtonState;
}

/****************************************************************************
 *
 * NAME: u8ButtonReadFfd_Wrapper
 *
 * DESCRIPTION:
 * Calls the appropriate button read function depending on platform
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS: Button state
 *
 ****************************************************************************/
PRIVATE uint8 u8ButtonReadFfd_Wrapper(void)
{
    uint8   u8ButtonState;
    #ifdef PCB_NTS
    u8ButtonState = u8ButtonReadNts();
    #else
    u8ButtonState = u8ButtonReadFfd();
    #endif
    return u8ButtonState;
}

/****************************************************************************
 *
 * NAME: u16ALSreadChannelResult_Wrapper
 *
 * DESCRIPTION:
 * Returns the light level from the TSL2550 Light level sensor
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS: Light Level
 *
 ****************************************************************************/
PRIVATE uint16 u16ALSreadChannelResult_Wrapper(void)
{
    return u16ALSreadChannelResult();
}

/****************************************************************************
 *
 * NAME: vLEDOn_Wrapper
 *
 * DESCRIPTION:
 * Calls the LED control function to turn on the LED
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32LED          R   LED bit in bitmap
 *
 * RETURNS: None
 *
 ****************************************************************************/
PRIVATE void vLEDOn_Wrapper(uint32 u32LED)
{
    vLedControl(u32LED, 1);
}

/****************************************************************************
 *
 * NAME: vLEDOn_Wrapper
 *
 * DESCRIPTION:
 * Calls the LED control function to turn off the LED
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u32LED          R   LED bit in bitmap
 *
 * RETURNS: None
 *
 ****************************************************************************/
PRIVATE void vLEDOff_Wrapper(uint32 u32LED)
{
    vLedControl(u32LED, 0);
}

/****************************************************************************
 *
 * NAME: vLCDWriteText_Wrapper
 *
 * DESCRIPTION:
 * Write text to the LCD
 *
 * PARAMETERS:      Name            RW  Usage
 *                  *TextStr        R   Text to write
 *                  u32Row          R   Row
 *                  u32Col          R   Column
 *
 * RETURNS: None
 *
 ****************************************************************************/
#ifndef PCB_NTS
PRIVATE void vLCDWriteText_Wrapper(char *TextStr, uint32 u32Row, uint32 u32Col)
{
    vLcdWriteText(TextStr, u32Row, u32Col);
    vLcdRefreshAll();
}
#endif


/****************************************************************************
 *
 * NAME: i32GetBoardVoltage
 *
 * DESCRIPTION:
 * Returns the board (battery) voltage by reading the dedicated ADC channel
 * and scales it in mV (e.g 3V = 3000).
 * If the ADC times out a non-valid negative value is returned
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS: Scaled voltage. Returns -1 if ADC timed out.
 *
 ****************************************************************************/
PRIVATE int32 i32GetBoardVoltage(void)
{
    uint16  u16AdcReading;
    int32   i32VoltsReading = 0;
    volatile uint32 u32Timeout = 0;

    vInitADC(E_AHI_ADC_SRC_VOLT);   /* Fix for PR1024 - initialize ADC channel for voltage */

    vAHI_AdcStartSample();      /* start sample */

    while (bAHI_AdcPoll()) {
        if (ADC_TIMEOUT == u32Timeout++) {
            i32VoltsReading = -1;
            break;
        }
    }

    if (-1 != i32VoltsReading)
    {
        u16AdcReading = u16AHI_AdcRead();
        i32VoltsReading = ((uint32)((uint32)(u16AdcReading * 586) +
                            ((uint32)(u16AdcReading * 586) >> 1)))  /
                                    1000;
    }

    return i32VoltsReading;
}

/****************************************************************************
 *
 * NAME: i32GetTemperature
 *
 * DESCRIPTION:
 * Returns the temperature by reading the appropriate temp sensor IC
 * i.e MCP9803 for NTS board or SHT1X (combined temp/humidity sensor) for other platforms
 * If the sensor reading times out a non-valid negative value is returned
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS: Temperature in degrees C
 *
 ****************************************************************************/
PRIVATE int32 i32GetTemperature(void)
{
    int32   i32TempReading = 0;
    volatile uint32 u32Timeout = 0;

    #ifdef PCB_NTS
    tsMCP9803_Result    sMCP9803;
    while (!bMCP9803_Read(&sMCP9803)) {
        if (TEMP_HUMID_TIMEOUT == u32Timeout++) {
            i32TempReading = -1;
            break;
        }
    }
    if (-1 != i32TempReading)
    {
        i32TempReading = (int32) sMCP9803.i8Temperature;
    }
    #else
    vHTSstartReadTemp();
    while ((u32AHI_DioReadInput() & HTS_DATA_DIO_BIT_MASK) != 0) {
        if (TEMP_HUMID_TIMEOUT == u32Timeout++) {
            i32TempReading = -1;
            break;
        }
    }
    if (-1 != i32TempReading)
    {
        i32TempReading = (int32) u16HTSreadTempResult();
    }
    #endif

    return i32TempReading;
}

/****************************************************************************
 *
 * NAME: i32GetHumidity
 *
 * DESCRIPTION:
 * Returns the humidity by reading the SHT1X (combined temp/humidity sensor).
 * If the sensor reading times out a non-valid negative value is returned
 * Note the NTS board does not populate this chip and therefore returns zero.
 *
 * PARAMETERS:      Name            RW  Usage
 *
 * RETURNS: Humidity in percentage
 *
 ****************************************************************************/
PRIVATE int32 i32GetHumidity(void)
{
    int32   i32HumidReading = 0;

    #ifndef PCB_NTS              /* note NTS board not supported */
    volatile uint32 u32Timeout = 0;

    vHTSstartReadHumidity();
    while ((u32AHI_DioReadInput() & HTS_DATA_DIO_BIT_MASK) != 0) {
        if (TEMP_HUMID_TIMEOUT == u32Timeout++) {
            i32HumidReading = -1;
            break;
        }
    }
    if (-1 != i32HumidReading)
    {
        i32HumidReading = (int32) u16HTSreadHumidityResult();
    }
    #endif

    return i32HumidReading;
}

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
