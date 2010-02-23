/*****************************************************************************
 *
 * MODULE:              JPI.h
 *
 * COMPONENT:           $RCSfile$
 *
 * VERSION:             $Name$
 *
 * REVISION:            $Revision: 11848 $
 *
 * DATED:               $Date: 2009-04-01 16:51:27 +0100 (Wed, 01 Apr 2009) $
 *
 * STATUS:              $State$
 *
 * AUTHOR:              MRW
 *
 * DESCRIPTION:         Jenie peripheral API
 *
 * LAST MODIFIED BY:    $Author: mmark $
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
 * Copyright Jennic Ltd 2008. All rights reserved
 *
 ****************************************************************************/
#ifndef  JPI_H_INCLUDED
#define  JPI_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

#include "AppHardwareApi.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#ifdef JPI_FUNCTION_POINTERS
#define u32JPI_Init                           u32AHI_Init
#define u8JPI_PowerStatus                     u8AHI_PowerStatus
#define vJPI_SysCtrlRegisterCallback          vAHI_SysCtrlRegisterCallback

#define vJPI_AnalogueConfigure                vAHI_ApConfigure
#define vJPI_AnalogueStartSample              vAHI_AdcStartSample
#define u16JPI_AnalogueAdcRead                u16AHI_AdcRead
#define vJPI_AnalogueDacOutput                vAHI_DacOutput
#define bJPI_APRegulatorEnabled               bAHI_APRegulatorEnabled
#define vJPI_APRegisterCallback               vAHI_APRegisterCallback
#define bJPI_AdcPoll                          bAHI_AdcPoll
#define bJPI_DacPoll                          bAHI_DacPoll

#define vJPI_ComparatorEnable                 vAHI_ComparatorEnable
#define vJPI_ComparatorDisable                vAHI_ComparatorDisable
#define vJPI_ComparatorIntEnable              vAHI_ComparatorIntEnable

#define vJPI_WakeTimerEnable                  vAHI_WakeTimerEnable
#define vJPI_WakeTimerStart                   vAHI_WakeTimerStart
#define vJPI_WakeTimerStop                    vAHI_WakeTimerStop
#define u8JPI_WakeTimerStatus                 u8AHI_WakeTimerStatus
#define u32JPI_WakeTimerCalibrate             u32AHI_WakeTimerCalibrate
#define u8JPI_WakeTimerFiredStatus            u8AHI_WakeTimerFiredStatus

#define vJPI_DioSetDirection                  vAHI_DioSetDirection
#define vJPI_DioSetOutput                     vAHI_DioSetOutput
#define vJPI_DioSetPullup                     vAHI_DioSetPullup
#define u32JPI_DioReadInput                   u32AHI_DioReadInput
#define u32JPI_DioWakeStatus                  u32AHI_DioWakeStatus

#define vJPI_TimerDisable                     vAHI_TimerDisable
#define vJPI_TimerStartCapture                vAHI_TimerStartCapture
#define vJPI_TimerStop                        vAHI_TimerStop
#define u8JPI_TimerFired                      u8AHI_TimerFired
#define vJPI_Timer0RegisterCallback           vAHI_Timer0RegisterCallback
#define vJPI_Timer1RegisterCallback           vAHI_Timer1RegisterCallback

#define vJPI_UartEnable                       vAHI_UartEnable
#define vJPI_UartDisable                      vAHI_UartDisable
#define vJPI_UartSetBaudDivisor               vAHI_UartSetBaudDivisor
#define vJPI_UartSetClockDivisor              vAHI_UartSetClockDivisor
#define vJPI_UartSetControl                   vAHI_UartSetControl
#define vJPI_UartSetInterrupt                 vAHI_UartSetInterrupt
#define vJPI_UartReset                        vAHI_UartReset
#define u8JPI_UartReadLineStatus              u8AHI_UartReadLineStatus
#define u8JPI_UartReadModemStatus             u8AHI_UartReadModemStatus
#define u8JPI_UartReadInterruptStatus         u8AHI_UartReadInterruptStatus
#define vJPI_UartWriteData                    vAHI_UartWriteData
#define u8JPI_UartReadData                    u8AHI_UartReadData
#define vJPI_UartSetRTSCTS                    vAHI_UartSetRTSCTS
#define vJPI_Uart0RegisterCallback            vAHI_Uart0RegisterCallback
#define vJPI_Uart1RegisterCallback            vAHI_Uart1RegisterCallback

/* SPI master */
#define vJPI_SpiConfigure                     vAHI_SpiConfigure
#define vJPI_SpiReadConfiguration             vAHI_SpiReadConfiguration
#define vJPI_SpiRestoreConfiguration          vAHI_SpiRestoreConfiguration
#define vJPI_SpiSelect                        vAHI_SpiSelect
#define vJPI_SpiStop                          vAHI_SpiStop
#define vJPI_SpiStartTransfer32               vAHI_SpiStartTransfer32
#define u32JPI_SpiReadTransfer32              u32AHI_SpiReadTransfer32
#define vJPI_SpiStartTransfer16               vAHI_SpiStartTransfer16
#define u16JPI_SpiReadTransfer16              u16AHI_SpiReadTransfer16
#define vJPI_SpiStartTransfer8                vAHI_SpiStartTransfer8
#define u8JPI_SpiReadTransfer8                u8AHI_SpiReadTransfer8
#define bJPI_SpiPollBusy                      bAHI_SpiPollBusy
#define vJPI_SpiWaitBusy                      vAHI_SpiWaitBusy
#define vJPI_SpiRegisterCallback              vAHI_SpiRegisterCallback

/* Serial 2-wire interface */
#define vJPI_SiConfigure                      vAHI_SiConfigure
#define vJPI_SiSetCmdReg                      vAHI_SiSetCmdReg
#define vJPI_SiWriteData8                     vAHI_SiWriteData8
#define vJPI_SiWriteSlaveAddr                 vAHI_SiWriteSlaveAddr
#define u8JPI_SiReadData8                     u8AHI_SiReadData8
#define bJPI_SiPollBusy                       bAHI_SiPollBusy
#define bJPI_SiPollTransferInProgress         bAHI_SiPollTransferInProgress
#define bJPI_SiPollRxNack                     bAHI_SiPollRxNack
#define bJPI_SiPollArbitrationLost            bAHI_SiPollArbitrationLost
#define vJPI_SiRegisterCallback               vAHI_SiRegisterCallback

/* Intelligent Peripheral */
#define vJPI_IpEnable                         vAHI_IpEnable
#define bJPI_IpSendData                       bAHI_IpSendData
#define bJPI_IpReadData                       bAHI_IpReadData
#define bJPI_IpTxDone                         bAHI_IpTxDone
#define bJPI_IpRxDataAvailable                bAHI_IpRxDataAvailable
#define vJPI_IpRegisterCallback               vAHI_IpRegisterCallback

#define vJPI_SwReset                          vAHI_SwReset
#define vJPI_DriveResetOut                    vAHI_DriveResetOut

/* Radio */
#define vJPI_SetBoostMode                     vAppApiSetBoostMode
#define vJPI_HighPowerModuleEnable            vAHI_HighPowerModuleEnable
#else

#define u32JPI_Init()                         u32AHI_Init()
#define u8JPI_PowerStatus()                   u8AHI_PowerStatus()
#define vJPI_SysCtrlRegisterCallback(_a)      vAHI_SysCtrlRegisterCallback(_a)

#define vJPI_AnalogueConfigure(_a, _b, _c, _d, _e) vAHI_ApConfigure( _a, _b, _c, _d, _e)
#define vJPI_AnalogueStartSample()            vAHI_AdcStartSample()
#define u16JPI_AnalogueAdcRead()              u16AHI_AdcRead()
#define vJPI_AnalogueDacOutput(_a, _b)        vAHI_DacOutput(_a, _b)
#define bJPI_APRegulatorEnabled()             bAHI_APRegulatorEnabled()
#define vJPI_APRegisterCallback(_a)           vAHI_APRegisterCallback(_a)
#define bJPI_AdcPoll()                        bAHI_AdcPoll()
#define bJPI_DacPoll()                        bAHI_DacPoll()

#define vJPI_ComparatorEnable(_a, _b, _c)     vAHI_ComparatorEnable(_a, _b, _c)
#define vJPI_ComparatorDisable(_a)            vAHI_ComparatorDisable(_a)
#define vJPI_ComparatorIntEnable(_a, _b, _c)  vAHI_ComparatorIntEnable(_a, _b, _c)

#define vJPI_WakeTimerEnable(_a, _b)          vAHI_WakeTimerEnable(_a, _b)
#define vJPI_WakeTimerStart(_a, _b)           vAHI_WakeTimerStart(_a, _b)
#define vJPI_WakeTimerStop(_a)                vAHI_WakeTimerStop(_a)
#define u8JPI_WakeTimerStatus()               u8AHI_WakeTimerStatus()
#define u32JPI_WakeTimerCalibrate()           u32AHI_WakeTimerCalibrate()
#define u8JPI_WakeTimerFiredStatus()          u8AHI_WakeTimerFiredStatus()
#define u32JPI_WakeTimerRead(_a)              u32AHI_WakeTimerRead(_a)

#define vJPI_DioSetDirection(_a, _b)          vAHI_DioSetDirection(_a, _b)
#define vJPI_DioSetOutput(_a, _b)             vAHI_DioSetOutput(_a, _b)
#define vJPI_DioSetPullup(_a, _b)             vAHI_DioSetPullup(_a, _b)
#define u32JPI_DioReadInput()                 u32AHI_DioReadInput()
#define u32JPI_DioWakeStatus()                u32AHI_DioWakeStatus()

#define vJPI_TimerDisable(_a)                 vAHI_TimerDisable(_a)
#define vJPI_TimerStartCapture(_a)            vAHI_TimerStartCapture(_a)
#define vJPI_TimerStop(_a)                    vAHI_TimerStop(_a)
#define u8JPI_TimerFired(_a)                  u8AHI_TimerFired(_a)
#define vJPI_Timer0RegisterCallback(_a)       vAHI_Timer0RegisterCallback(_a)
#define vJPI_Timer1RegisterCallback(_a)       vAHI_Timer1RegisterCallback(_a)

#define vJPI_UartEnable(_a)                   vAHI_UartEnable(_a)
#define vJPI_UartDisable(_a)                  vAHI_UartDisable(_a)
#define vJPI_UartSetBaudDivisor(_a, _b)       vAHI_UartSetBaudDivisor(_a, _b)
#define vJPI_UartSetClockDivisor(_a, _b)      vAHI_UartSetClockDivisor(_a, _b)
#define vJPI_UartSetControl(_a, _b, _c, _d, _e, _f) vAHI_UartSetControl(_a, _b, _c, _d, _e, _f)
#define vJPI_UartSetInterrupt(_a, _b, _c, _d, _e, _f) vAHI_UartSetInterrupt(_a, _b, _c, _d, _e, _f)
#define vJPI_UartReset(_a, _b, _c)            vAHI_UartReset(_a, _b, _c)
#define u8JPI_UartReadLineStatus(_a)          u8AHI_UartReadLineStatus(_a)
#define u8JPI_UartReadModemStatus(_a)         u8AHI_UartReadModemStatus(_a)
#define u8JPI_UartReadInterruptStatus(_a)     u8AHI_UartReadInterruptStatus(_a)
#define vJPI_UartWriteData(_a, _b)            vAHI_UartWriteData(_a, _b)
#define u8JPI_UartReadData(_a)                u8AHI_UartReadData(_a)
#define vJPI_UartSetRTSCTS(_a, _b)            vAHI_UartSetRTSCTS(_a, _b)
#define vJPI_Uart0RegisterCallback(_a)        vAHI_Uart0RegisterCallback(_a)
#define vJPI_Uart1RegisterCallback(_a)        vAHI_Uart1RegisterCallback(_a)

/* SPI master */
#define vJPI_SpiConfigure(_a, _b, _c, _d, _e, _f, _g) vAHI_SpiConfigure(_a, _b, _c, _d, _e, _f, _g)
#define vJPI_SpiReadConfiguration(_a)         vAHI_SpiReadConfiguration(_a)
#define vJPI_SpiRestoreConfiguration(_a)      vAHI_SpiRestoreConfiguration(_a)
#define vJPI_SpiSelect(_a)                    vAHI_SpiSelect(_a)
#define vJPI_SpiStop()                        vAHI_SpiStop()
#define vJPI_SpiStartTransfer32(_a)           vAHI_SpiStartTransfer32(_a)
#define u32JPI_SpiReadTransfer32()            u32AHI_SpiReadTransfer32()
#define vJPI_SpiStartTransfer16(_a)           vAHI_SpiStartTransfer16(_a)
#define u16JPI_SpiReadTransfer16()            u16AHI_SpiReadTransfer16()
#define vJPI_SpiStartTransfer8(_a)            vAHI_SpiStartTransfer8(_a)
#define u8JPI_SpiReadTransfer8()              u8AHI_SpiReadTransfer8()
#define bJPI_SpiPollBusy()                    bAHI_SpiPollBusy()
#define vJPI_SpiWaitBusy()                    vAHI_SpiWaitBusy()
#define vJPI_SpiRegisterCallback(_a)          vAHI_SpiRegisterCallback(_a)

/* Serial 2-wire interface */
#define vJPI_SiConfigure(_a, _b, _c)          vAHI_SiConfigure(_a, _b, _c)
#define vJPI_SiSetCmdReg(_a, _b, _c, _d, _e, _f) vAHI_SiSetCmdReg(_a, _b, _c, _d, _e, _f)
#define vJPI_SiWriteData8(_a)                 vAHI_SiWriteData8(_a)
#define vJPI_SiWriteSlaveAddr(_a, _b)         vAHI_SiWriteSlaveAddr(_a, _b)
#define u8JPI_SiReadData8()                   u8AHI_SiReadData8()
#define bJPI_SiPollBusy()                     bAHI_SiPollBusy()
#define bJPI_SiPollTransferInProgress()       bAHI_SiPollTransferInProgress()
#define bJPI_SiPollRxNack()                   bAHI_SiPollRxNack()
#define bJPI_SiPollArbitrationLost()          bAHI_SiPollArbitrationLost()
#define vJPI_SiRegisterCallback(_a)           vAHI_SiRegisterCallback(_a)

/* Intelligent Peripheral */
#define vJPI_IpEnable(_a, _b, _c)             vAHI_IpEnable(_a, _b, _c)
#define bJPI_IpSendData(_a, _b)               bAHI_IpSendData(_a, _b)
#define bJPI_IpReadData(_a, _b)               bAHI_IpReadData(_a, _b)
#define bJPI_IpTxDone()                       bAHI_IpTxDone()
#define bJPI_IpRxDataAvailable()              bAHI_IpRxDataAvailable()
#define vJPI_IpRegisterCallback(_a)           vAHI_IpRegisterCallback(_a)

#define vJPI_SwReset()                        vAHI_SwReset()
#define vJPI_DriveResetOut(_a)                vAHI_DriveResetOut(_a)

/* Radio */
#define vJPI_SetBoostMode(_a)                 vAppApiSetBoostMode(_a)
#define vJPI_HighPowerModuleEnable(_a, _b)    vAHI_HighPowerModuleEnable(_a, _b)

#endif

#define E_JPI_TIMER_INTERRUPT_RISING          (1U)
#define E_JPI_TIMER_INTERRUPT_COMPLETE        (2U)

#define E_JPI_WAKE_TIMER_0         0
#define E_JPI_WAKE_TIMER_1         1
#define E_JPI_UART_0               0
#define E_JPI_UART_1               1

/* Value enumerations: wake timer */
#define E_JPI_WAKE_TIMER_MASK_0    1
#define E_JPI_WAKE_TIMER_MASK_1    2

/* Value enumerations: Analogue Peripherals */
#define E_JPI_ADC_SRC_ADC_1        0
#define E_JPI_ADC_SRC_ADC_2        1
#define E_JPI_ADC_SRC_ADC_3        2
#define E_JPI_ADC_SRC_ADC_4        3
#define E_JPI_ADC_SRC_TEMP         4
#define E_JPI_ADC_SRC_VOLT         5
#define E_JPI_AP_REGULATOR_ENABLE  TRUE
#define E_JPI_AP_REGULATOR_DISABLE FALSE
#define E_JPI_AP_SAMPLE_2          0
#define E_JPI_AP_SAMPLE_4          1
#define E_JPI_AP_SAMPLE_6          2
#define E_JPI_AP_SAMPLE_8          3
#define E_JPI_AP_CLOCKDIV_2MHZ     0
#define E_JPI_AP_CLOCKDIV_1MHZ     1
#define E_JPI_AP_CLOCKDIV_500KHZ   2
#define E_JPI_AP_CLOCKDIV_250KHZ   3
#define E_JPI_AP_INPUT_RANGE_2     TRUE
#define E_JPI_AP_INPUT_RANGE_1     FALSE
#define E_JPI_AP_GAIN_2            TRUE
#define E_JPI_AP_GAIN_1            FALSE
#define E_JPI_AP_EXTREF            TRUE
#define E_JPI_AP_INTREF            FALSE
#define E_JPI_ADC_CONVERT_ENABLE   TRUE
#define E_JPI_ADC_CONVERT_DISABLE  FALSE
#define E_JPI_ADC_CONTINUOUS       TRUE
#define E_JPI_ADC_SINGLE_SHOT      FALSE
#define E_JPI_AP_INT_ENABLE        TRUE
#define E_JPI_AP_INT_DISABLE       FALSE
#define E_JPI_DAC_RETAIN_ENABLE    TRUE
#define E_JPI_DAC_RETAIN_DISABLE   FALSE

/* Value enumerations: Comparator */
#define E_JPI_COMP_HYSTERESIS_0MV  0
#define E_JPI_COMP_HYSTERESIS_5MV  1
#define E_JPI_COMP_HYSTERESIS_10MV 2
#define E_JPI_COMP_HYSTERESIS_20MV 3
#define E_JPI_AP_COMPARATOR_MASK_1 1
#define E_JPI_AP_COMPARATOR_MASK_2 2
#define E_JPI_COMP_SEL_EXT         0x00
#define E_JPI_COMP_SEL_DAC         0x01
#define E_JPI_COMP_SEL_BANDGAP     0x03

/* Value enumerations: UART */
#define E_JPI_UART_RATE_4800       0
#define E_JPI_UART_RATE_9600       1
#define E_JPI_UART_RATE_19200      2
#define E_JPI_UART_RATE_38400      3
#define E_JPI_UART_RATE_76800      4
#define E_JPI_UART_RATE_115200     5
#define E_JPI_UART_WORD_LEN_5      0
#define E_JPI_UART_WORD_LEN_6      1
#define E_JPI_UART_WORD_LEN_7      2
#define E_JPI_UART_WORD_LEN_8      3
#define E_JPI_UART_FIFO_LEVEL_1    0
#define E_JPI_UART_FIFO_LEVEL_4    1
#define E_JPI_UART_FIFO_LEVEL_8    2
#define E_JPI_UART_FIFO_LEVEL_14   3
#define E_JPI_UART_LS_ERROR        0x80
#define E_JPI_UART_LS_TEMT         0x40
#define E_JPI_UART_LS_THRE         0x20
#define E_JPI_UART_LS_BI           0x10
#define E_JPI_UART_LS_FE           0x08
#define E_JPI_UART_LS_PE           0x04
#define E_JPI_UART_LS_OE           0x02
#define E_JPI_UART_LS_DR           0x01
#define E_JPI_UART_MS_DCTS         0x01
#define E_JPI_UART_INT_MODEM       0
#define E_JPI_UART_INT_TX          1
#define E_JPI_UART_INT_RXDATA      2
#define E_JPI_UART_INT_RXLINE      3
#define E_JPI_UART_INT_TIMEOUT     6
#define E_JPI_UART_TX_RESET        TRUE
#define E_JPI_UART_RX_RESET        TRUE
#define E_JPI_UART_TX_ENABLE       FALSE
#define E_JPI_UART_RX_ENABLE       FALSE
#define E_JPI_UART_EVEN_PARITY     TRUE
#define E_JPI_UART_ODD_PARITY      FALSE
#define E_JPI_UART_PARITY_ENABLE   TRUE
#define E_JPI_UART_PARITY_DISABLE  FALSE
#define E_JPI_UART_1_STOP_BIT      TRUE
#define E_JPI_UART_2_STOP_BITS     FALSE
#define E_JPI_UART_RTS_HIGH        TRUE
#define E_JPI_UART_RTS_LOW         FALSE

/* Value enumerations: SPI */
#define E_JPI_SPIM_MSB_FIRST       FALSE
#define E_JPI_SPIM_LSB_FIRST       TRUE
#define E_JPI_SPIM_INT_ENABLE      TRUE
#define E_JPI_SPIM_INT_DISABLE     FALSE
#define E_JPI_SPIM_AUTOSLAVE_ENBL  TRUE
#define E_JPI_SPIM_AUTOSLAVE_DSABL FALSE
#define E_JPI_SPIM_SLAVE_ENBLE_0   0x1
#define E_JPI_SPIM_SLAVE_ENBLE_1   0x2
#define E_JPI_SPIM_SLAVE_ENBLE_2   0x4
#define E_JPI_SPIM_SLAVE_ENBLE_3   0x8

/* Value enumerations: Serial Interface */
#define E_JPI_SI_INT_AL            0x20
#define E_JPI_SI_SLAVE_RW_SET      FALSE
#define E_JPI_SI_START_BIT         TRUE
#define E_JPI_SI_NO_START_BIT      FALSE
#define E_JPI_SI_STOP_BIT          TRUE
#define E_JPI_SI_NO_STOP_BIT       FALSE
#define E_JPI_SI_SLAVE_READ        TRUE
#define E_JPI_SI_NO_SLAVE_READ     FALSE
#define E_JPI_SI_SLAVE_WRITE       TRUE
#define E_JPI_SI_NO_SLAVE_WRITE    FALSE
#define E_JPI_SI_SEND_ACK          FALSE
#define E_JPI_SI_SEND_NACK         TRUE
#define E_JPI_SI_IRQ_ACK           TRUE
#define E_JPI_SI_NO_IRQ_ACK        FALSE

/* Value enumerations: Intelligent Peripheral */
#define E_JPI_IP_MAX_MSG_SIZE      0x3F
#define E_JPI_IP_TXPOS_EDGE        FALSE
#define E_JPI_IP_TXNEG_EDGE        TRUE
#define E_JPI_IP_RXPOS_EDGE        FALSE
#define E_JPI_IP_RXNEG_EDGE        TRUE
#define E_JPI_IP_BIG_ENDIAN        TRUE
#define E_JPI_IP_LITTLE_ENDIAN     FALSE

/* Value enumerations: Timer */
#define E_JPI_TIMER_INT_PERIOD     1
#define E_JPI_TIMER_INT_RISE       2

/* Value enumerations: DIO */
#define E_JPI_DIO0_INT             0x00000001
#define E_JPI_DIO1_INT             0x00000002
#define E_JPI_DIO2_INT             0x00000004
#define E_JPI_DIO3_INT             0x00000008
#define E_JPI_DIO4_INT             0x00000010
#define E_JPI_DIO5_INT             0x00000020
#define E_JPI_DIO6_INT             0x00000040
#define E_JPI_DIO7_INT             0x00000080
#define E_JPI_DIO8_INT             0x00000100
#define E_JPI_DIO9_INT             0x00000200
#define E_JPI_DIO10_INT            0x00000400
#define E_JPI_DIO11_INT            0x00000800
#define E_JPI_DIO12_INT            0x00001000
#define E_JPI_DIO13_INT            0x00002000
#define E_JPI_DIO14_INT            0x00004000
#define E_JPI_DIO15_INT            0x00008000
#define E_JPI_DIO16_INT            0x00010000
#define E_JPI_DIO17_INT            0x00020000
#define E_JPI_DIO18_INT            0x00040000
#define E_JPI_DIO19_INT            0x00080000
#define E_JPI_DIO20_INT            0x00100000

/* Interrupt Item Bitmap Masks */
#define E_JPI_SYSCTRL_WK0_MASK     (1 << E_JPI_SYSCTRL_WK0)
#define E_JPI_SYSCTRL_WK1_MASK     (1 << E_JPI_SYSCTRL_WK1)
#define E_JPI_SYSCTRL_COMP0_MASK   (1 << E_JPI_SYSCTRL_COMP0)
#define E_JPI_SYSCTRL_COMP1_MASK   (1 << E_JPI_SYSCTRL_COMP1)

#define E_JPI_UART_TIMEOUT_MASK    (1 << E_JPI_UART_INT_TIMEOUT)
#define E_JPI_UART_RXLINE_MASK     (1 << E_JPI_UART_INT_RXLINE)
#define E_JPI_UART_RXDATA_MASK     (1 << E_JPI_UART_INT_RXDATA)
#define E_JPI_UART_TX_MASK         (1 << E_JPI_UART_INT_TX)
#define E_JPI_UART_MODEM_MASK      (1 << E_JPI_UART_INT_MODEM)

#define E_JPI_TIMER_RISE_MASK      E_JPI_TIMER_INT_RISE
#define E_JPI_TIMER_PERIOD_MASK    E_JPI_TIMER_INT_PERIOD

#define E_JPI_SI_RXACK_MASK        (1 << 7)
#define E_JPI_SI_BUSY_MASK         (1 << 6)
#define E_JPI_SI_AL_MASK           (1 << 5)
#define E_JPI_SI_ACK_CTRL_MASK     (1 << 2)
#define E_JPI_SI_TIP_MASK          (1 << 1)
#define E_JPI_SI_INT_STATUS_MASK   (1 << 0)

#define E_JPI_SPIM_TX_MASK         (1 << 0)

#define E_JPI_IP_INT_STATUS_MASK   (1 << 6)
#define E_JPI_IP_TXGO_MASK         (1 << 1)
#define E_JPI_IP_RXGO_MASK         (1 << 0)

#define E_JPI_AP_INT_STATUS_MASK   (1 << 0)

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/* Device types, used to identify interrupt source */
typedef enum
{
    E_JPI_DEVICE_TICK_TIMER = 0, /* Tick timer */
    E_JPI_DEVICE_SYSCTRL = 2,    /* System controller */
    E_JPI_DEVICE_BBC,            /* Baseband controller */
    E_JPI_DEVICE_AES,            /* Encryption engine */
    E_JPI_DEVICE_PHYCTRL,        /* Phy controller */
    E_JPI_DEVICE_UART0,          /* UART 0 */
    E_JPI_DEVICE_UART1,          /* UART 1 */
    E_JPI_DEVICE_TIMER0,         /* Timer 0 */
    E_JPI_DEVICE_TIMER1,         /* Timer 1 */
    E_JPI_DEVICE_SI,             /* Serial Interface (2 wire) */
    E_JPI_DEVICE_SPIM,           /* SPI master */
    E_JPI_DEVICE_INTPER,         /* Intelligent peripheral */
    E_JPI_DEVICE_ANALOGUE        /* Analogue peripherals */
} teJPI_Device;

/* Individual interrupts */
typedef enum
{
    E_JPI_SYSCTRL_WK0   = 26,    /* Wake timer 0 */
    E_JPI_SYSCTRL_WK1   = 27,    /* Wake timer 1 */
    E_JPI_SYSCTRL_COMP0 = 28,    /* Comparator 0 */
    E_JPI_SYSCTRL_COMP1 = 29,    /* Comparator 1 */
} teJPI_Item;

typedef enum {
    E_JPI_ANALOGUE_DAC_0,
    E_JPI_ANALOGUE_DAC_1,
    E_JPI_ANALOGUE_ADC
} teJPI_AnalogueChannel;

typedef enum {
    E_JPI_COMPARATOR_0,
    E_JPI_COMPARATOR_1
} teJPI_Comparator;

typedef enum {
    E_JPI_TIMER_0,
    E_JPI_TIMER_1
} teJPI_Timer;

typedef enum {
    E_JPI_TIMER_MODE_SINGLESHOT,
    E_JPI_TIMER_MODE_REPEATING,
    E_JPI_TIMER_MODE_DELTASIGMA,
    E_JPI_TIMER_MODE_DELTASIGMARTZ
} teJPI_TimerMode;

typedef enum {
    E_JPI_TIMER_CLOCK_INTERNAL_NORMAL,
    E_JPI_TIMER_CLOCK_INTERNAL_INVERTED,
    E_JPI_TIMER_CLOCK_EXTERNAL_NORMAL,
    E_JPI_TIMER_CLOCK_EXTERNAL_INVERTED
} teJPI_TimerClockType;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vJPI_AnalogueEnable(teJPI_AnalogueChannel eChan, bool_t bInputRange, bool_t bContinuous, uint8 u8Source, bool_t bOutputHold, uint16 u16InitValue);
PUBLIC void vJPI_AnalogueDisable(teJPI_AnalogueChannel eChan);
PUBLIC bool_t bJPI_ComparatorStatus(teJPI_Comparator eComparator);
PUBLIC bool_t bJPI_ComparatorWakeStatus(teJPI_Comparator eComparator);
PUBLIC void vJPI_DioWake(uint32 u32Enable, uint32 u32Disable, uint32 u32Rising, uint32 u32Falling);
PUBLIC void vJPI_TimerEnable(teJPI_Timer eTimer, uint8 u8Prescale, uint8 mIntMask, bool_t bOutputEn, bool_t bTimerIOEn, teJPI_TimerClockType eClockType);
PUBLIC void vJPI_TimerStart(teJPI_Timer eTimer, teJPI_TimerMode eTimerMode, uint16 u16HighPeriod, uint16 u16LowPeriod);
PUBLIC uint32 u32JPI_TimerReadCapture(teJPI_Timer eTimer);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/


#if defined __cplusplus
}
#endif

#endif

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/



