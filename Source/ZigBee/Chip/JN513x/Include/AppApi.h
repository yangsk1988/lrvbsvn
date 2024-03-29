/*****************************************************************************
 *
 * MODULE:              Application API header
 *
 * COMPONENT:           $RCSfile: AppApi.h,v $
 *
 * VERSION:             $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:            $Revision: 1.5 $
 *
 * DATED:               $Date: 2008/10/22 10:34:20 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              CJG
 *
 * DESCRIPTION:
 * Access functions and structures used by the application to interact with
 * the Jennic 802.15.4 stack.
 *
 * LAST MODIFIED BY:    $Author: pjtw $
 *                      $Modtime: $
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
 * Copyright Jennic Ltd 2006. All rights reserved
 *
 ***************************************************************************/

/**
 * @defgroup g_app_sap Application MAC Service Access Point (SAP)
 */

#ifndef  APP_API_H_INCLUDED
#define  APP_API_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include <jendefs.h>
#include <mac_sap.h>

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

#ifndef AHI_DEVICE_ENUM
#define AHI_DEVICE_ENUM

/* Device types, used to identify interrupt source */
typedef enum
{
    E_AHI_DEVICE_SYSCTRL = 2, /* System controller */
    E_AHI_DEVICE_BBC,         /* Baseband controller */
    E_AHI_DEVICE_AES,         /* Encryption engine */
    E_AHI_DEVICE_PHYCTRL,     /* Phy controller */
    E_AHI_DEVICE_UART0,       /* UART 0 */
    E_AHI_DEVICE_UART1,       /* UART 1 */
    E_AHI_DEVICE_TIMER0,      /* Timer 0 */
    E_AHI_DEVICE_TIMER1,      /* Timer 1 */
    E_AHI_DEVICE_SI,          /* Serial Interface (2 wire) */
    E_AHI_DEVICE_SPIM,        /* SPI master */
    E_AHI_DEVICE_INTPER,      /* Intelligent peripheral */
    E_AHI_DEVICE_ANALOGUE     /* Analogue peripherals */
} teAHI_Device;

/* Individual interrupts */
typedef enum
{
    E_AHI_SYSCTRL_WK0 = 26,   /* Wake timer 0 */
    E_AHI_SYSCTRL_WK1 = 27    /* Wake timer 1 */
} teAHI_Item;

#endif /* !AHI_DEVICE_ENUM */

typedef enum
{
    E_PHY_ENTERED_TX,
    E_PHY_ENTERED_RX,
    E_PHY_ENTERED_IDLE
} tePhyEnteredState;

/**
 * @ingroup g_app_sap
 * @brief Get Buffer routine type
 *
 * Type of Get Buffer callback routine
 */
typedef MAC_DcfmIndHdr_s * (*PR_GET_BUFFER)(void *);

/**
 * @ingroup g_app_sap
 * @brief Post routine type
 *
 * Type of Post callback routine
 */
typedef void (*PR_POST_CALLBACK)(void *, MAC_DcfmIndHdr_s *);

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
/* Functions that are defined in library file */

PUBLIC void * pvAppApiGetMacHandle(void);
PUBLIC void vAppApiMcpsRequest(MAC_McpsReqRsp_s *psMcpsReqRsp,
                                      MAC_McpsSyncCfm_s *psMcpsSyncCfm);
PUBLIC PHY_Enum_e eAppApiPlmeGet(PHY_PibAttr_e ePhyPibAttribute,
                                        uint32 *pu32PhyPibValue);


PUBLIC uint32
u32AppApiInit(PR_GET_BUFFER prMlmeGetBuffer,
              PR_POST_CALLBACK prMlmeCallback,
              void *pvMlmeParam,
              PR_GET_BUFFER prMcpsGetBuffer,
              PR_POST_CALLBACK prMcpsCallback,
              void *pvMcpsParam);
PUBLIC void
vAppApiMlmeRequest(MAC_MlmeReqRsp_s *psMlmeReqRsp,
                   MAC_MlmeSyncCfm_s *psMlmeSyncCfm);
PUBLIC PHY_Enum_e eAppApiPlmeSet(PHY_PibAttr_e ePhyPibAttribute,
                                 uint32 u32PhyPibValue);
PUBLIC void
vAppApiSaveMacSettings(void);
PUBLIC void
vAppApiRestoreMacSettings(void);
PUBLIC void *
pvAppApiGetMacAddrLocation(void);
PUBLIC void
vAppApiSetBoostMode(bool_t bOnNotOff);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* APP_API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/

