/*****************************************************************************
 *
 * MODULE:              Jennic Zigbee: Network layer definitions
 *
 * COMPONENT:           $RCSfile: nwk.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.14 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              Korwin
 *
 * DESCRIPTION:
 * Network layer definitions
 *
 * CHANGE HISTORY:
 *
 *
 *
 * LAST MODIFIED BY:    $Author: gpfef $
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
 * ACCURACY OR LACK OF NEGLIGENCE. JENNIC SHALL NOT, IN ANY CIRCUMSTANCES, BE
 * LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, SPECIAL,
 * INCIDENTAL OR CONSEQUENTIAL DAMAGES FOR ANY REASON WHATSOEVER.
 *
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ****************************************************************************/

#ifndef __NWK_H
#define __NWK_H

#ifdef __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include "mac_sap.h"
#include "mac_pib.h"

#ifndef PACK
#define PACK      __attribute__ ((packed))        /* align to byte boundary  */
#endif

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define nwkcDefaultSecurityLevel			0x00
#define nwkcDiscoveryRetryLimit				0x03
#define nwkcMinHeaderOverhead				8
#define nwkcProtocolVersion					0x01
#define nwkcRouteDiscoveryTime				0x3E8
#define nwkcMaxBroadcastJitter				0x40
#define nwkcInitialRREQRetries				0x03
#define nwkcRREQRetries						0x02
#define nwkcRREQRetyInterval				0xfe
#define nwkcMinRREQJitter					0x01
#define nwkcMaxRREQJitter					0x40
#define nwkcRepairThreshold					3
#define nwkcMaxMacRequestCount				4
#define nwkcUseTreeAddrAlloc				TRUE
#define nwkcUseTreeRouting					TRUE
#define nwkcSuperframeOrder					15
#define nwkcBeaconOrder						15
#define nwkcMaxNeighborTransmitFailure		3
#define nwkcMaxChildrenLimit				20
#define nwkcMaxRoutersLimit					20
#define nwkcMaxDepth						8
#define nwkcPassiveAckTimeoutLimit			0x0A
#define maccMaxPayloadSize					(MAC_MAX_PHY_PKT_SIZE - MAC_MAX_DATA_FRM_OVERHEAD)
#define nwkcMaxPayloadSize   				(maccMaxPayloadSize - nwkcMinHeaderOverhead)
#define NWK_SIZE_ROUTINGTABLE				20
#define NWK_SIZE_BTT						20
#define COORD_SHORT_ADDR					0x0000
#define BROADCAST_SHORT_ADDR				0xFFFF
#define NULL_SHORT_ADDR						MAC_PIB_SHORT_ADDRESS_DEF
#define NULL_PANId 							0xFFFF
#define MAC_TX_OPTION_NONE       			0
#define MAX_CHANNELS_24GHZ      			0x07FFF800

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef enum
{
	STACK_SUCCESS = 0,
	STACK_ROUTING_TABLE_FULL = 0x10,
	STACK_ROUTE_DISCOVERY_TABLE_FULL,
	STACK_NWK_QUEUE_FULL,
	STACK_RETRY_QUEUE_FULL,
	STACK_BTT_TABLE_FULL,
	STACK_APS_ACK_QUEUE_FULL,
	STACK_APS_CONFIRM_QUEUE_FULL,
	STACK_AF_INVALID_PARAMETER,
	STACK_APS_ACK_TIMER_CREATE_FAILURE,
	STACK_BROADCAST_TIMER_CREATE_FAILURE,
	STACK_ROUTE_DISCOVERY_TIMER_CREATE_FAILURE,
	STACK_APS_ADDRESSMAP_FULL,
	STACK_APS_ADDRESSMAP_NOT_EXIST,
	STACK_APS_ADDRESSMAP_EXIST,
	STACK_MAC_TRANSACTION_OVERFLOW,
	STACK_NWK_NO_DATA,
	STACK_NWK_PASSIVE_ACKED,
	STACK_INVALID_PARAMETER,
	STACK_ROUTE_DISCOVERY_TABLE_NOT_EXIST,
	STACK_ROUTING_TABLE_NOT_EXIST,
	STACK_NWK_QUEUE_EXIST,
	STACK_NWK_TRANSACTION_EXPIRED,
	STACK_AF_CLUSTER_NOT_IN_SIMPLE_DESC,
	STACK_INVALID_FRAME_RECEIVED,
	STACK_BOS_DEVICE_NOT_IN_SLEEP,
}Stack_Status_e;

typedef enum
{
	NWK_ENUM_SUCCESS = 0x00,
	NWK_ENUM_INVALID_PARAMETER=0xc1,
	NWK_ENUM_INVALID_REQUEST,
	NWK_ENUM_NOT_PERMITTED,
	NWK_ENUM_STARTUP_FAILURE,
	NWK_ENUM_ALREADY_PRESENT,
	NWK_ENUM_SYNC_FAILURE,
	NWK_ENUM_TABLE_FULL,
	NWK_ENUM_UNKNOWN_DEVICE,
	NWK_ENUM_UNSUPPORTED_ATTRIBUTE,
	NWK_ENUM_NO_NETWORKS,
	NWK_ENUM_LEAVE_UNCONFIRMED,
	NWK_ENUM_MAX_FRM_CNTR,
	NWK_ENUM_NO_KEY,
	NWK_ENUM_BAD_CCM_OUTPUT,
	NWK_ENUM_RESERVED1,
	NWK_ENUM_ROUTE_DISCOVERY_FAILED,
	NWK_ENUM_TRANSACTION_EXPIRED,
	NWK_ENUM_FORCE_REMOVED,
} NWK_Enum_e;

typedef enum
{
	NWK_SECURITY_LEVEL = 0x70,
	NWK_SECURITY_MATERIAL_SET = 0x71,
	NWK_ACTIVE_KEY_SEQ_NUMBER = 0x73,
	NWK_ALL_FRESH = 0x74,
	NWK_SEQUENCE_NUMBER = 0x81,
	NWK_PASSIVE_ACK_TIMEOUT = 0x82,
	NWK_MAX_BROADCAST_RETRIES = 0x83,
	NWK_MAX_CHILDREN = 0x84,
	NWK_MAX_DEPTH = 0x85,
	NWK_MAX_ROUTERS = 0x86,
	NWK_NEIGHBOR_TABLE = 0x87,
	NWK_NETWORK_BROADCAST_DELIVERY_TIME = 0x88,
	NWK_REPORT_CONSTANT_COST = 0x89,
	NWK_ROUTE_DISCOVERY_RETRIES_PERMITTED = 0x8a,
	NWK_ROUTE_TABLE = 0x8b,
	NWK_SYM_LINK = 0x8e,
	NWK_CAPABILITY_INFORMATION = 0x8f,
	NWK_USE_TREE_ADDR_ALLOC = 0x90,
	NWK_USE_TREE_ROUTING = 0x91,
	NWK_NEXT_ADDRESS = 0x92,
	NWK_AVAILABLE_ADDRESSES = 0x93,
	NWK_ADDRESS_INCREMENT = 0x94,
	NWK_TRANSACTION_PERSISTENCE_TIME = 0x95,
	NWK_DISTRIBUTING_BROADCAST_ENDDEVICE = 0x96,
	NWK_ACKED_BROADCAST_ENDDEVICE = 0x97,
	NWK_TRANSMIT_FAILURE_THRESHOLD = 0x98,
	NWK_RESERVED_TRANSACTION_COUNT = 0x99,
} NWK_NIBAttrubute_e;

typedef struct
{
	uint16 	u16PanID;
	uint8 	u8Channel;
	uint8 	StackProfile;
	uint8 	ZigbeeVersion;
	uint8 	u8BeaconOrder;
	uint8 	u8SuperframeOrder;
	bool_t 	permitJoining;
}NWK_PanDescriptor_s;


typedef enum
{
	ZIGBEE_COORDINATOR = 0x00,
	ZIGBEE_ROUTER,
	ZIGBEE_ENDDEVICE
}NWK_DeviceType_e;

typedef enum
{
	SUPPRESS_ROUTE_DISCOVERY = 0x00,
	ENABLE_ROUTE_DISCOVERY,
	FORCE_ROUTE_DISCOVERY
}NWK_DiscoverRoute_e;

typedef enum
{
	NEIGHBOR_PARENT = 0x00,
	NEIGHBOR_CHILD,
	NEIGHBOR_SIBLING,
	NEIGHBOR_LEAVING,
	NONE_NEIGHBOR
}NWK_NeighborRelationship_e;

typedef enum
{
	ACTIVE = 0x0,
	DISCOVERY_UNDERWAY,
	DISCOVERY_FAILED,
	INACTIVE
}NWK_RoutingTableStatus_e;


typedef struct
{
	uint16 	u16AddrDst;
	NWK_RoutingTableStatus_e eStatus;
	uint16 	u16AddrNextHop;
	uint8	u8NoackCount;
}NWK_RoutingTable_s;

typedef struct
{
	uint16 	u16PanID;
	MAC_ExtAddr_s 		sExtAddr;
	uint16 	u16Addr;
	NWK_DeviceType_e 	eDevicetype;
	bool_t 	bRxOnWhenIdle;
	NWK_NeighborRelationship_e eRelationship;
	uint8 	u8Depth;
	uint8 	u8BeaconOrder;
	bool_t 	bPermitjoining;
	bool_t 	bPotentialparent;
	uint8 	u8Transmitfailure;
	uint8 	u8LQI;
	uint8 	u8Channel;
	uint8 	u8StackProfile;
	uint8 	u8ZigbeeVersion;
}NWK_NeighborTable_s;

//! NWK layer information fields
typedef struct
{
	uint8 	ProtocolID;
	uint8 	StackProfile;
	uint8 	nwkProtocolVersion;
	uint8 	u8DeviceDepth;
	bool_t 	bRouterCapacity;
	bool_t 	bEndDeviceCapacity;
}NWK_IF_s;

typedef enum
{
	ADDRESS_NONE	= 0x00,
	ADDRESS_SHORT	= 0x02,
	ADDRESS_EXT		= 0x03
}NWK_AddressMode_e;

typedef enum
{
	NWK_NO_ROUTE_AVAILABLE	= 0x00,
	NWK_TREE_LINK_FAILURE,
	NWK_NON_TREE_LINK_FAILURE,
	NWK_LOW_BATTERY_LEVEL,
	NWK_NO_ROUTING_CAPACITY
} NWK_RouteErrorStatus_e;

typedef struct PACK
{
	uint8		bAllocateAddress		: 1;
	uint8		bSecurityCapability		: 1;
	uint8		Reserved1 				: 2;
	uint8		bRxOnWhenIdle			: 1;
	uint8		bPowerSource			: 1;
	uint8		bDeviceType				: 1;
	uint8		bAlternatePanCoordinator: 1;
} NWK_CapabilityInformation_s;

typedef struct
{
	bool_t 	bAcked;
	uint16 	u16Addr;
}NWK_BtrNeighborAddr_s;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC uint8 u8NwkGetNsduHandle(bool_t bAutoIncrease);
PUBLIC void vNwkInitNIS(void);
PUBLIC bool_t bZdoMlmeBeaconNotifyInd(MAC_MlmeIndBeacon_s *pMlmeBcNotiInd);
PUBLIC void nlmePermitJoiningRequest ( uint8 u8PermitDuration );
PUBLIC void nlmeNetworkDiscoveryRequest( uint32 u32ScanChannels, uint8 u8ScanDuration );
PUBLIC NWK_Enum_e nlmeDirectJoinRequest (MAC_ExtAddr_s sExtAddr,
											NWK_CapabilityInformation_s sCapabilityInfo);
PUBLIC void nlmeSyncRequest ( bool_t Track );
PUBLIC void nlmeStartRouterRequest(uint8 u8BeaconOrder,
									uint8 u8SuperframeOrder,
									bool_t bBatteryLifeExtension);
PUBLIC void nlmeJoinRequest (	uint16	u16PanID,
								bool_t	bJoinAsRouter,
								bool_t	bRejoinNetwork,
								uint32	u32ScanChannels,
								uint8		u8ScanDuration,
								bool_t	bPowerSource,
								bool_t	bRxOnWhenIdle,
								bool_t	bMACSecurity);

PUBLIC NWK_Enum_e nlmeSetRequest(NWK_NIBAttrubute_e eAttributeID, void *pvValue);
PUBLIC NWK_Enum_e nlmeGetRequest (NWK_NIBAttrubute_e eAttributeID, void *pvValue);
PUBLIC void nlmeResetRequest (void);
PUBLIC void nlmeNetworkFormationRequest (uint32	u32ScanChannels,
											uint8		u8ScanDuration,
											uint8		u8BeaconOrder,
											uint8		u8SuperframeOrder,
											uint16	u16PanID,
											bool_t	bBatteryLifeExtension);

PUBLIC Stack_Status_e nlmeLeaveRequest( MAC_ExtAddr_s *psExtAddr,
								bool_t bRemoveChildren,
								bool_t bMacSecurityEnable );
PUBLIC Stack_Status_e nldeDataRequest (uint16	u16DestAddr,
										uint8		u8NsduLength,
										uint8		*pau8Nsdu,
					 					uint8		u8NsduHandle,
					 					uint8		u8RadiusCounter,
					 					NWK_DiscoverRoute_e		eDiscoverRoute,
					 					bool_t	bSecurityEnable);

PUBLIC void nldeDataIndication(uint16 u16AddrSrc,
								uint8 u8NsduLength,
								uint8 *pau8Nsdu,
								uint8 u8LQI);
PUBLIC void nldeDataConfirm(NWK_Enum_e eStatus, uint8 u8NsduHandle);
PUBLIC void nlmeJoinConfirm (uint16 u16PanID, NWK_Enum_e eStatus);
PUBLIC void nlmePermitJoiningConfirm(uint8 eStatus);
PUBLIC void nlmeLeaveConfirm(MAC_ExtAddr_s *psExtAddr, NWK_Enum_e eStatus);
PUBLIC void nlmeJoinIndication( uint16 u16ShortAddr, MAC_ExtAddr_s sExtAddr, uint8 u8CapabilityInfo );
PUBLIC void nlmeLeaveIndication (MAC_ExtAddr_s *psExtAddr);
PUBLIC void nlmeNetworkDiscoveryConfirm (uint8 u8NetworkCount,
											NWK_PanDescriptor_s *pNetworkList,
											NWK_Enum_e eStatus);

PUBLIC void nlmeNetworkFormationConfirm (uint8 eStatus);
PUBLIC void nlmeRouteErrorIndication(uint16 u16AddrSrc,
										uint16 u16AddrDst,
										NWK_RouteErrorStatus_e eErrorCode);
PUBLIC void nlmeSyncConfirm(MAC_Enum_e eStatus);
PUBLIC void nlmeStartRouterConfirm (NWK_Enum_e eStatus);
PUBLIC void nlmeSyncIndication(NWK_Enum_e eStatus);
PUBLIC void vNwkRestoreContexts(uint16 u16AddrLocal,
								uint16 u16PanID,
								uint8 u8Channel,
								NWK_DeviceType_e eDeviceType,
								NWK_IF_s *psNIF,
								NWK_CapabilityInformation_s *psCapabilityInfo);

PUBLIC void vNwkFrameReceived(uint16 u16SourceAddr, uint16 u16DestAddr);
PUBLIC bool_t bNwkRemoveDevice(uint16 u16NwkAddr);
PUBLIC bool_t bAuthorizeNode(MAC_ExtAddr_s *psExtAddr, uint8 u8Capability);
PUBLIC void vEnableModifiedJoining(bool_t bEnable);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/
extern PUBLIC void 				*gpvMac;
extern PUBLIC MAC_Pib_s			*gpsMib;
extern PUBLIC uint16 			gNwkMyAddr;
extern PUBLIC uint16			gNwkPanID;
extern PUBLIC uint8				gNwkNsduHandle;
extern PUBLIC uint8 			gNwkChannel;
extern PUBLIC NWK_DeviceType_e 	gNwkDeviceType;


#ifdef __cplusplus
}
#endif //__cplusplus


#endif	// __NWK_H
/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
