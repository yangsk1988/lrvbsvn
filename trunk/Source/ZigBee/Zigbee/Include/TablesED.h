/*****************************************************************************
 *
 * MODULE:              TablesED
 *
 * COMPONENT:           $RCSfile: TablesED.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.8 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              GPfef
 *
 * DESCRIPTION:
 * Additional definitions for End Device nieghbour table access
 *
 * CHANGE HISTORY:
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

#ifndef  TABLES_ED_INCLUDED
#define  TABLES_ED_INCLUDED

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define NWK_SIZE_NEIGHBORTABLE			8

#define MAX_CONTEXT_SIZE_ED 	(sizeof(App_WakeupMode_e) \
	 	 	 	 	 	 	 	+ sizeof(uint16) \
	 	 	 	 	 	 	 	+ sizeof(uint16) \
	 	 	 	 	 	 	 	+ sizeof(uint8) \
	 	 	 	 	 	 	 	+ sizeof(NWK_DeviceType_e) \
	 	 	 	 	 	 	 	+ sizeof(NWK_CapabilityInformation_s) \
	 	 	 	 	 	 	 	+ sizeof(uint8) \
	 	 	 	 	 	 	 	+ (sizeof(NWK_NeighborTable_s) * NWK_SIZE_NEIGHBORTABLE) \
	 	 	 	 	 	 	 	+ sizeof(tsJZS_Config))

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef struct
{
	uint8 	nwkSequenceNumber;
	uint8 	nwkPassiveAckTimeout;		//! Broadcast Passive ACK Timeout in second
	uint8	nwkMaxBroadcastRetries;		//! The maximum time duration in seconds allowed for the parent and all child devices to retransmit a broadcast message (passive acknowledgment timeout).
	uint8 	nwkMaxChildren;				// Range : 0 ~ nwkcMaxChildrenLimit
	uint8 	nwkMaxDepth;				// Range : 0 ~ nwkcMaxDepth
	uint8 	nwkMaxRouters;				// Range : 0 ~ nwkcMaxRoutersLimit
	NWK_NeighborTable_s 	*nwkNeighborTable;			// NWK_SIZE_NEIGHBORTABLE is changeable
	uint8	nwkNeighborTableSize;
	uint8	nwkNetworkBroadcastDeliveryTime;		//! Broadcast Delivery Timeout in second
	NWK_CapabilityInformation_s nwkCapabilityInformation;
	bool_t 	nwkSymLink;
	bool_t 	nwkUseTreeRouting;
	bool_t	nwkDistributingBroadcastEndDevice;		//! TRUE or FALSE, which turns on/off distributing broadcasts to End Devices. not declared in Spec.
	bool_t	nwkAckedBroadcastEndDevice;				//! TRUE or FALSE, which choose Acknowledged Transmission or not ACKed Transmisson when distributing broadcasts to End Devices. not declared in Spec.
	uint8	nwkRouteRepairThreshold;				//! ( Range : 0 ~ nwkcRepairThreshold ) Maximum number of allowed communication errors after which the route repair mechanism is initiated.
	bool_t	nwkReuseShortAddress;					//! TRUE or FALSE, which choose Re-Use Short-Address in Neighbor Table, delete or not Neighbor Table when receipt Leave Indication
	uint8	nwkReservedTransactionCount;			//! ( Range : 0 ~ nwkcMaxMacRequestCount-1 ) Number of Reserved Transaction for Unicast Data Frame
	uint8	nwkReservedRoutingTableCount;			//! ( Range : 0 ~ NWK_SIZE_ROUTINGTABLE ) Number of Reserve Routing Table
	uint8	nwkBTTSize;
	MAC_ExtAddr_s	nwkExtAddrLocal;
	bool_t	nwkDelNeighborTableWhenJoinFailed;		// If TRUE, the entry of neighbor table will be deleted when association is failed
}NWK_IB_s;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
extern PUBLIC NWK_IB_s			gsNIB;

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#endif  /* TABLES_ED_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
