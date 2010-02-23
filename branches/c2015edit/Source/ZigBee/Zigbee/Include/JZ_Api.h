/*****************************************************************************
 *
 * MODULE:              Jennic Zigbee: Main API entry point
 *
 * COMPONENT:           $RCSfile: JZ_Api.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.12 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              CJG Jennic
 *
 * DESCRIPTION:
 * API for Jennic Zigbee stack. Calls into stack are tagged JZS (Jennic Zigbee
 * Stack) whilst calls into the application are tagged JZA (Jennic Zigbee
 * Application)
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
 * ACCURACY OR LACK OF NEGLIGENCE. JENNIC SHALL NOT, IN ANY CIRCUMSTANCES, BE
 * LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, SPECIAL,
 * INCIDENTAL OR CONSEQUENTIAL DAMAGES FOR ANY REASON WHATSOEVER.
 *
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ****************************************************************************/

#ifndef JZ_API_H
#define JZ_API_H

#ifdef __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include "jendefs.h"
#include "af.h"
#include "aps.h"
#include "bos.h"
#include "nwk.h"
#include "AppHardwareApi.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define	END_DEVICE_POLL_INTERVAL_1SEC			100
#define JZS_vEnableModifiedJoining				vEnableModifiedJoining
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef struct
{
    uint32 			u32Channel;
    uint16 			u16PanId;
    uint8			*pu8AppData;
    uint16			u16AppDataLength;
    uint16			u16AutoPollInterval;
    bool_t			bAutoPoll;
    bool_t			bRestrictedJoinAttempts;
    bool_t			bAutoJoin;
    bool_t 			bNwkAddrReUse;
    uint16			u16CommsLossPeriod;
    uint8			u8MaxChildren;
    uint8			u8MaxRouters;
    uint8			u8MaxDepth;
} tsJZS_Config;

enum tag_teJZS_EventIdentifier
{
    JZS_EVENT_APS_DATA_CONFIRM,
    JZS_EVENT_NWK_JOINED_AS_ROUTER,
    JZS_EVENT_NWK_JOINED_AS_ENDDEVICE,
    JZS_EVENT_NWK_STARTED,
    JZS_EVENT_CONTEXT_RESTORED,
    JZS_EVENT_FAILED_TO_START_NETWORK,
    JZS_EVENT_FAILED_TO_JOIN_NETWORK,
    JZS_EVENT_NEW_NODE_HAS_JOINED,
    JZS_EVENT_REMOVE_NODE,
    JZS_EVENT_LEAVE_INDICATION,
    JZS_EVENT_NETWORK_DISCOVERY_COMPLETE,
    JZS_EVENT_INACTIVE_ED_DELETED,
    JZS_EVENT_POLL_COMPLETE,
    JZS_EVENT_AUTHORIZE_DEVICE,
    JZS_EVENT_APP_POWER_DOWN
};
typedef enum tag_teJZS_EventIdentifier teJZS_EventIdentifier;

typedef union
{
    struct
    {
		uint16	u16DstAddr;
		uint8	u8DstEP;
		uint8	u8SrcEP;
        uint8 u8Status;
    } sApsDataConfirmEvent;

    struct
    {
        uint16 u16Addr;
    } sNwkJoinedEvent;

    struct
    {
		uint16 			u16ShortAddr;
		MAC_ExtAddr_s 	*psExtAddr;
		uint8 			u8CapabilityInfo;
	} sNewNodeEvent;

	struct
	{
		MAC_ExtAddr_s 	*psExtAddr;
		NWK_Enum_e		eStatus;
	} sRemoveNodeEvent;

	struct
	{
		MAC_ExtAddr_s 	*psExtAddr;
	} sLeaveIndicationEvent;

	struct
	{
		uint8 u8NetworkCount;
		NWK_PanDescriptor_s *pNetworkList;
		NWK_Enum_e eStatus;
	} sNetworkDiscoveryEvent;

    struct
    {
        uint16 u16Addr;
    } sInactiveEDDeletedEvent;

    struct
    {
		NWK_Enum_e		eStatus;
	} sPollConfirmEvent;

    struct
    {
		MAC_ExtAddr_s 	*psExtAddr;
		uint8 			u8CapabilityInfo;
		bool_t			bJoin;
	} sAuthorizeNodeEvent;

} tuJZS_StackEvent;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
extern PUBLIC uint32 JZS_u32InitSystem(bool_t bColdStart);
extern PUBLIC void JZS_vStartStack(void);
extern PUBLIC void JZS_vRejoinNetwork(void);
extern PUBLIC bool_t JZA_bAfKvpObject(APS_Addrmode_e		eAddrMode,
										uint16				u16AddrSrc,
										uint8				u8SrcEP,
										uint8				u8LQI,
										uint8				u8DstEP,
										uint8				u8ClusterId,
										uint8				*pu8ClusterIDRsp,
										AF_Transaction_s	*puTransactionInd,
										AF_Transaction_s	*puTransactionRsp);

extern PUBLIC void JZA_vAfKvpResponse(	APS_Addrmode_e		eAddrMode,
										uint16				u16AddrSrc,
										uint8				u8SrcEP,
										uint8				u8LQI,
										uint8				u8DstEP,
										uint8				u8ClusterID,
										AF_Transaction_s	*puTransactionInd);

extern PUBLIC bool_t JZA_bAfMsgObject(APS_Addrmode_e		eAddrMode,
								uint16				u16AddrSrc,
								uint8					u8SrcEP,
								uint8					u8LQI,
								uint8 				u8DstEP,
								uint8					u8ClusterID,
								uint8 				*pu8ClusterIDRsp,
								AF_Transaction_s	*puTransactionInd,
								AF_Transaction_s	*puTransactionRsp);

extern PUBLIC void JZA_vZdpResponse(uint8  u8Type,
                                    uint8  u8LQI,
                                    uint8 *pu8Payload,
                                    uint8  u8PayloadLen);

extern PUBLIC void JZA_vAppDefineTasks(void);

extern PUBLIC void JZA_vAppEventHandler(void);

extern PUBLIC bool_t JZA_boAppStart(void);

extern PUBLIC void JZA_vPeripheralEvent(uint32 u32Device,
                                        uint32 u32ItemBitmap);

extern PUBLIC void JZA_vStackEvent(teJZS_EventIdentifier eEventId,
                                   tuJZS_StackEvent *puStackEvent);

extern PUBLIC void JZS_vPollParent(void);
extern PUBLIC void JZS_vRemoveNode( MAC_ExtAddr_s *psExtAddr,
									bool_t bRemoveChildren);
extern PUBLIC void JZS_vStartNetwork(void);
extern PUBLIC void JZS_vJoinNetwork(void);
extern PUBLIC void JZS_vDiscoverNetworks(void);
extern PUBLIC void JZS_vEnableEDAddrReuse(uint16 u16CommsLossPeriod);
extern PUBLIC void JZS_vSwReset(void);
extern PUBLIC void JZS_vEnableBroadcastsToED(bool_t bEnable);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/
extern PUBLIC tsJZS_Config 	JZS_sConfig;

#if defined __cplusplus
}
#endif

#endif  /* JZ_API_H */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
