/****************************************************************************
 *
 * MODULE:             Driver for TSL2550 ambient light sensor
 *
 * COMPONENT:          $RCSfile: AlsDriver.c,v $
 *
 * VERSION:            $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:           $Revision: 1.4 $
 *
 * DATED:              $Date: 2008/10/22 12:19:33 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             CJG
 *
 * DESCRIPTION:
 * Provides API for driving TSL2550 ambient light sensor.
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
 * Copyright Jennic Ltd 2005. All rights reserved
 *
 ***************************************************************************/

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/
#include "jendefs.h"
#include "AlsDriver.h"
#include "AppHardwareApi.h"

#if ( defined PCB_DEVKIT1 ) || ( defined PCB_HPDEVKIT )

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define I2C_WRITE          (0x00)
#define I2C_READ           (0x01)

#define ALS_DEVICE_ADDRESS (0x72)

#define ALS_POWER_DOWN     (0x00)
#define ALS_POWER_UP       (0x03)
#define ALS_EXTENDED_RANGE (0x1d)
#define ALS_READ_CHANNEL_0 (0x43)
#define ALS_READ_CHANNEL_1 (0x83)

#define POS_ACK            (TRUE)
#define NEG_ACK            (FALSE)

#define ALS_TIMEOUT        (300000)

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/
PRIVATE void vSetClock(uint8 u8Data);
PRIVATE void vSetData(uint8 u8Data);
PRIVATE void vSetDataDirection(uint8 u8Data);
PRIVATE void vStartSequence(void);
PRIVATE void vStopSequence(void);
PRIVATE void vSendByte(uint8 u8Data);
PRIVATE uint8 u8ReceiveByte(void);
PRIVATE bool_t boReadAck(void);
PRIVATE void vWriteAck(bool_t boPosNotNeg);
PRIVATE void vWait(int iWait);

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
 * NAME: vALSreset
 *
 * DESCRIPTION:
 * Resets the light sensor and sets up the DIO pins used to control it
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vALSreset(void)
{
    /* Set up GPIO pins */
    vSetClock(1);
    vSetData(1);
    vSetDataDirection(1);
    vAHI_DioSetDirection(0, ALS_CLK_DIO_BIT_MASK);

    vStartSequence();

    vSendByte(ALS_DEVICE_ADDRESS | I2C_WRITE);
    (void)boReadAck();

    vSendByte(ALS_POWER_UP);
    (void)boReadAck();
    vStopSequence();

    vStartSequence();

    vSendByte(ALS_DEVICE_ADDRESS | I2C_WRITE);
    (void)boReadAck();

    vSendByte(ALS_EXTENDED_RANGE);
    (void)boReadAck();
    vStopSequence();
}

/****************************************************************************
 *
 * NAME: vALSpowerDown
 *
 * DESCRIPTION:
 * Resets the light sensor and sets up the DIO pins used to control it
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PUBLIC void vALSpowerDown(void)
{
    vStartSequence();

    vSendByte(ALS_DEVICE_ADDRESS | I2C_WRITE);
    (void)boReadAck();

    vSendByte(ALS_POWER_DOWN);
    (void)boReadAck();
    vStopSequence();

    /* Set GPIO pins as inputs (lower power) */
    vSetDataDirection(0);
    vAHI_DioSetDirection(ALS_CLK_DIO_BIT_MASK, 0);
}

/****************************************************************************
 *
 * NAME: vALSstartReadChannel
 *
 * DESCRIPTION:
 * Sends a command to the sensor to start reading a channel. There are two
 * channels, one for the full light range and one for infra-red only.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u8Channel       R   Channel to start read on
 *
 * RETURNS:
 * Void
 *
 ****************************************************************************/
PUBLIC void vALSstartReadChannel(uint8 u8Channel)
{
    vStartSequence();

    vSendByte(ALS_DEVICE_ADDRESS | I2C_WRITE);
    (void)boReadAck();

    if (u8Channel)
    {
        vSendByte(ALS_READ_CHANNEL_1);
    }
    else
    {
        vSendByte(ALS_READ_CHANNEL_0);
    }

    (void)boReadAck();
    vStopSequence();
}

/****************************************************************************
 *
 * NAME: u16ALSreadChannelResult
 *
 * DESCRIPTION:
 * Reads a light level from the sensor and converts it to a reasonably
 * linear value.
 *
 * RETURNS:
 * Value in range 0-4015, 0 is darkest
 *
 ****************************************************************************/
PUBLIC uint16 u16ALSreadChannelResult(void)
{
    static const uint16 au16ChordValue[8] = {0, 16, 49, 115, 247, 511, 1039, 2095};

    uint8  u8Result;
    uint8  u8ChordIndex;
    uint16 u16ChordValue;
    uint16 u16StepValue;
    uint16 u16StepNumber;
    uint16 u16Result;
    int    iTimeout = 0;

    do
    {
        vStartSequence();
        vSendByte(ALS_DEVICE_ADDRESS | I2C_READ);
        (void)boReadAck();
        u8Result = u8ReceiveByte();
        vWriteAck(POS_ACK);
        vStopSequence();
        iTimeout++;
    } while (((u8Result & 128) == 0) && (iTimeout < ALS_TIMEOUT));

    if (iTimeout == ALS_TIMEOUT)
    {
        return 65534;
    }

    /* Work out value, using formula in TSL2550 data sheet */
    u8ChordIndex = (u8Result & 0x70) >> 4;
    u16ChordValue = au16ChordValue[u8ChordIndex];
    u16StepValue = 1 << u8ChordIndex;
    u16StepNumber = u8Result & 0x0f;
    u16Result = u16ChordValue + u16StepValue * u16StepNumber;
    return u16Result;
}

/****************************************************************************/
/***        Local Functions                                               ***/
/****************************************************************************/
/****************************************************************************
 *
 * NAME: vSetClock
 *
 * DESCRIPTION:
 * Sets or clears the DIO pin used for the clock to the sensor.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u8Data          R   Value to use
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vSetClock(uint8 u8Data)
{
    if (u8Data)
    {
        vAHI_DioSetOutput(ALS_CLK_DIO_BIT_MASK, 0);
    }
    else
    {
        vAHI_DioSetOutput(0, ALS_CLK_DIO_BIT_MASK);
    }
}

/****************************************************************************
 *
 * NAME: vSetData
 *
 * DESCRIPTION:
 * Sets or clears the DIO pin used for the data to the sensor.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u8Data          R   Value to use
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vSetData(uint8 u8Data)
{
    if (u8Data)
    {
        vAHI_DioSetOutput(ALS_DATA_DIO_BIT_MASK, 0);
    }
    else
    {
        vAHI_DioSetOutput(0, ALS_DATA_DIO_BIT_MASK);
    }
}

/****************************************************************************
 *
 * NAME:
 *
 * DESCRIPTION:
 * Sets the direction of the DIO pin used for the data to the sensor. This
 * is either an input or an output.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u8Data          R   Direction to set
 *
 * RETURNS:
 * void
 *
 * NOTES:
 * There is not an equivalent function for the clock line as that is only set
 * once and code clarity is not enhanced by having a separate function for it.
 *
 ****************************************************************************/
PRIVATE void vSetDataDirection(uint8 u8Data)
{
    if (u8Data)
    {
        vAHI_DioSetDirection(0, ALS_DATA_DIO_BIT_MASK);
    }
    else
    {
        vAHI_DioSetDirection(ALS_DATA_DIO_BIT_MASK, 0);
    }
}

/****************************************************************************
 *
 * NAME: vStartSequence
 *
 * DESCRIPTION:
 * Implements the combination of clock and data levels used to set the light
 * sensor to the start of a data transfer sequence.
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vStartSequence(void)
{
    vSetData(1);
    vSetDataDirection(1);
    vWait(10);
    vSetData(0);
    vWait(10);
    vSetClock(0);
    vWait(10);
}

/****************************************************************************
 *
 * NAME: vStopSequence
 *
 * DESCRIPTION:
 * Implements the combination of clock and data levels used to inform the
 * light sensor that a data transfer has completed.
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vStopSequence(void)
{
    vSetClock(1);
    vWait(10);
    vSetData(1);
    vSetDataDirection(1);
}
/****************************************************************************
 *
 * NAME: vSendByte
 *
 * DESCRIPTION:
 * Implements the combination of clock and data levels used to pass a byte of
 * data to the sensor.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  u8Data          R   Byte to send
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vSendByte(uint8 u8Data)
{
    uint8 i;

    vSetDataDirection(1);

    /* Send byte, MSB first */
    for (i = 128; i > 0; i >>= 1)
    {
        if (u8Data & i)
        {
            vSetData(1);
        }
        else
        {
            vSetData(0);
        }
        vSetClock(1);
        vWait(10);
        vSetClock(0);
        vWait(10);
    }
}

/****************************************************************************
 *
 * NAME: u8ReceiveByte
 *
 * DESCRIPTION:
 * Implements the combination of clock and data levels used to pass a byte of
 * data from the sensor.
 *
 * RETURNS:
 * uint8: byte received
 *
 ****************************************************************************/
PRIVATE uint8 u8ReceiveByte(void)
{
    uint8 i;
    uint8 u8Result = 0;

    vSetDataDirection(0);

    /* Get byte */
    for (i = 128; i > 0; i >>= 1)
    {
        vSetClock(1);
        vWait(10);
        if (u32AHI_DioReadInput() & ALS_DATA_DIO_BIT_MASK)
        {
            u8Result |= i;
        }
        vSetClock(0);
        vWait(10);
    }

    return u8Result;
}

/****************************************************************************
 *
 * NAME: boReadAck
 *
 * DESCRIPTION:
 * Implements the combination of clock and data levels used to receive an
 * acknowledgement from the sensor.
 *
 * RETURNS:
 * NEG_ACK: negative acknowledgement
 * POS_ACK: positive acknowledgement
 *
 ****************************************************************************/
PRIVATE bool_t boReadAck(void)
{
    bool_t boResult;

    /* Get ack */
    vSetDataDirection(0);
    vSetClock(1);
    vWait(10);

    boResult = (u32AHI_DioReadInput() & ALS_DATA_DIO_BIT_MASK) ? NEG_ACK : POS_ACK;

    vSetClock(0);
    vWait(10);

    return boResult;
}

/****************************************************************************
 *
 * NAME: vWriteAck
 *
 * DESCRIPTION:
 * Implements the combination of clock and data levels used to send an
 * acknowledgement to the sensor.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  boPosNotNeg     R   POS_ACK if positive ack to be sent
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vWriteAck(bool_t boPosNotNeg)
{
    if (boPosNotNeg == POS_ACK)
    {
        vSetData(0);
    }
    else
    {
        vSetData(1);
    }

    vSetDataDirection(1);
    vSetClock(1);
    vWait(10);
    vSetClock(0);
    vWait(10);
}

/****************************************************************************
 *
 * NAME: vWait
 *
 * DESCRIPTION:
 * Waits by looping around for the specified number of iterations.
 *
 * PARAMETERS:      Name            RW  Usage
 *                  iWait           R   Number of iterations to wait for
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vWait(int iWait)
{
    volatile int i;
    for (i = 0; i < iWait; i++);
}

#endif
/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
