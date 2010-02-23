/*****************************************************************************
 *
 * MODULE:
 *
 * COMPONENT:           $RCSfile$
 *
 * VERSION:             $Name$
 *
 * REVISION:            $Revision: 11865 $
 *
 * DATED:               $Date: 2009-04-01 17:29:47 +0100 (Wed, 01 Apr 2009) $
 *
 * STATUS:              $State$
 *
 * AUTHOR:
 *
 * DESCRIPTION:
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

#ifndef  JENNET_API_H_INCLUDED
#define  JENNET_API_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include <jendefs.h>
#include "mac_sap.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define SCAN_LIST_SIZE              8
#define RESET_STACK     TRUE        // PR1125
#define CONTINUE_SCAN   FALSE
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum
{
    E_JENNET_SUCCESS,
    E_JENNET_DEFERRED,
    E_JENNET_ERROR
}teJenNetStatusCode;

typedef struct
{
    MAC_ExtAddr_s   sExtAddr;
    uint16          u16PanId;
    uint16          u16Depth;
    uint8           u8Channel;
    uint8           u8LinkQuality;
    uint8           u8NumChildren;
    uint16          u16UserDefined;
}tsScanElement;

typedef struct
{
    MAC_ExtAddr_s   sDestAddr;
    uint8           u8NextHop;
    uint8           u8RouteUsage;   // GNATS PR545
    bool_t          bEntryActive;
    bool_t          bRoutesToImport;
} tsRoutingTable;

typedef struct
{
    uint32 u32Protocol;
    char *pau8VersionString;
}tsVersionInfo;


typedef enum
{
    NETWORK_VERSION,
    MAC_VERSION,
    CHIP_VERSION
}teJenNetComponent;

typedef enum
{
    E_JENNET_CR,
    E_JENNET_ED
} teJenNetStackType;


typedef bool_t (*trSortScanCallback)(tsScanElement *pasScanResult,
                                   uint8 u8ScanListSize,
                                   uint8 *pau8ScanListOrder);

typedef bool_t (*trBeaconNotifyCallback)(tsScanElement *psBeaconInfo,

                                       uint32 u32NetworkID,

                                       uint16 u16ProtocolVersion);

typedef bool_t (*trAuthoriseCallback)(MAC_ExtAddr_s *psAddr);

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC teJenNetStatusCode eApi_SendDataToExtNwk(MAC_Addr_s *psDestAddr, uint8 *pu8Payload,
                    uint8 u8Length);

PUBLIC void     vApi_SetBcastTTL(uint8 u8MaxTTL);

PUBLIC void     vApi_SetUserBeaconBits(uint16 u16Bits);
PUBLIC uint16   u16Api_GetUserBeaconBits(void);

PUBLIC void     vApi_SetPurgeRoute(bool_t bPurge);
PUBLIC void     vApi_SetPurgeInterval(uint32 u32Interval);

PUBLIC void     vApi_RegScanSortCallback(trSortScanCallback prCallback);
PUBLIC void     vApi_RegBeaconNotifyCallback(trBeaconNotifyCallback prCallback);
PUBLIC void     vApi_RegLocalAuthoriseCallback(trAuthoriseCallback prCallback);
PUBLIC void     vApi_RegNwkAuthoriseCallback(trAuthoriseCallback prCallback);
PUBLIC void     vApi_SetScanSleep(uint32 u32ScanSleepDuration);
PUBLIC void 	vNwk_SetBeaconCalming(bool bState);
PUBLIC void 	vNwk_DeleteChild(MAC_ExtAddr_s *psNodeAddr);
PUBLIC uint16   u16Api_GetDepth(void);
PUBLIC uint32   u32Api_GetVersion(teJenNetComponent eComponent,tsVersionInfo* psVersionInfo);
PUBLIC uint8    u8Api_GetLastPktLqi(void);      //PR633 & 1067
PUBLIC uint8    u8Api_GetStackState(void);      // PR1274

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/
extern PUBLIC teJenNetStackType gStackType;
extern PUBLIC uint32    gNetworkID;
extern PUBLIC uint16    gPanID;
extern PUBLIC uint8     gChannel;
extern PUBLIC uint32    gScanChannels;
extern PUBLIC uint8     gMaxChildren;
extern PUBLIC uint8     gMaxSleepingChildren;
extern PUBLIC uint8     gMaxFailedPkts;
extern PUBLIC uint8     gMaxBcastTTL;
extern PUBLIC uint16    gRouterPingPeriod;
extern PUBLIC bool_t    gRouterPurgeInactiveED;
extern PUBLIC uint8     gEndDevicePingInterval;
extern PUBLIC uint32    gEndDeviceScanTimeout;
extern PUBLIC uint32    gEndDeviceScanSleep;
extern PUBLIC uint32    gEndDevicePollPeriod;

extern PUBLIC uint8     gMinBeaconLQI;
extern PUBLIC bool_t    bPermitExtNwkPkts;
extern PUBLIC uint32    gRoutingTableSize;
extern PUBLIC void     *gpvRoutingTableSpace;
extern PUBLIC uint8     gInternalTimer;
extern PUBLIC bool_t    gRouterEnableAutoPurge;
extern PUBLIC bool_t    gRouteImport;
extern PUBLIC uint32    gEDChildActivityTimeout;  // GNATS PR893


#if defined __cplusplus
}
#endif

#endif  /* JENNET_API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
