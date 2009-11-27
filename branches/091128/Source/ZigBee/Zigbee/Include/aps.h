/*****************************************************************************
 *
 * MODULE:              Jennic Zigbee: APS layer
 *
 * COMPONENT:           $RCSfile: aps.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.8 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              Korwin
 *
 * DESCRIPTION:
 * Defines the interface to the APS layer
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

#ifndef __APS_H
#define __APS_H

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include "nwk.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#ifndef PACK
#define PACK      __attribute__ ((packed))        /* align to byte boundary  */
#endif

#define	APS_SIZE_ADDRESSMAP					50
#define	APS_SIZE_BINDINGTABLE				5
#define	apscMaxFrameOverhead				6
#define	apscMaxPayloadSize					(nwkcMaxPayloadSize - apscMaxFrameOverhead)

#define afmeAddSimpleDesc					afmeSimpleDescAdd
#define afmeDeleteSimpleDesc				afmeSimpleDescDelete
#define afmeSearchEndpoint					afmeSimpleDescSearch
#define	afmeUserDescSetting 				afmeUserDescSet
#define JZA_vZdpResponse					JZA_ZdpResponse

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef enum
{
	APS_ADDRMODE_NOT_PRESENT = 0x00,
	APS_ADDRMODE_SHORT
}APS_Addrmode_e;

typedef enum
{
	APS_TXOPTION_NONE						= 0x00,
	SECURITY_ENABLE_TRANSMISSION			= 0x01,
	USE_NWK_KEY								= 0x02,
	ACKNOWLEDGED_TRANSMISSION				= 0x04
}APS_TxOptions_e;

typedef enum
{
	APS_ENUM_SUCCESS = 0x00,
	APS_ENUM_ILLEGAL_DEVICE,
	APS_ENUM_ILLEGAL_REQUEST,
	APS_ENUM_TABLE_FULL,
	APS_ENUM_NOT_SUPPORTED,
	APS_ENUM_INVALID_BINDING,
	APS_ENUM_UNSUPPORTED_ATTRIBUTE,
	APS_ENUM_INVALID_PARAMETER,
	APS_ENUM_NO_BOUND_DEVICE,
	APS_ENUM_SECURITY_FAIL,
	APS_ENUM_NO_ACK
}APS_Status_e;

typedef struct PACK
{
	uint8			o1Reserved					: 1;
	uint8			o1AckRequest				: 1;
	uint8			o1Security					: 1;
	uint8			o1IndirectAddrMode			: 1;
	uint8			o2DeliveryMode				: 2;
	uint8			o2FrameType					: 2;
}APS_FrameControl_f;

typedef struct
{
	MAC_ExtAddr_s		sExtAddr;
	uint16				u16ShortAddr;
}APS_AddressMap_s;

typedef struct
{
	MAC_ExtAddr_s		sExtAddrSrc;
	uint8				u8SrcEP;
	uint8				u8ClusterId;
	MAC_ExtAddr_s		sExtAddrDst;
	uint8				u8DstEP;
	bool_t				bInUse;
} APS_BindingTable_s;

typedef struct
{
	APS_BindingTable_s	*BindingTable;		//! APS Binding Table (size of Table is changeable)
	uint8				apsBindingTableSize;
	APS_AddressMap_s	*AddressMap;		//! APS Address Map (size of Table is changeable)
	uint8				apsAddressMapSize;
}APS_IB_s;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC void initAis(void);
PUBLIC Stack_Status_e apsmeAMAdd(MAC_ExtAddr_s *psExtAddr, uint16 shortAddr);
PUBLIC Stack_Status_e apsmeAMDelete(MAC_ExtAddr_s *psExtAddr, uint16 shortAddr);
PUBLIC uint8 apsmeAMGetCount(void);
PUBLIC APS_AddressMap_s* apsmeAMSearchShortAddr(uint16 u16Addr);
PUBLIC APS_AddressMap_s* apsmeAMSearchExtAddr(MAC_ExtAddr_s *psExtAddr);
PUBLIC APS_AddressMap_s* apsmeAMSearchIndex(uint8 index);

PUBLIC Stack_Status_e apsdeDataRequest(APS_Addrmode_e		eAddrMode,
										uint16					u16AddrDst,
										uint8						u8DstEP,
										uint16					u16ProfileID,
										uint8						u8ClusterID,
										uint8						u8SrcEP,
										uint8						u8AsduLength,
										uint8						*pau8Asdu,
										APS_TxOptions_e		eTxOptions,
										NWK_DiscoverRoute_e	eDiscoverRoute,
										uint8 					u8RadiusCounter);

PUBLIC void apsdeDataConfirm(APS_Addrmode_e		eAddrModeDst,
									uint16		u16AddrDst,
									uint8			u8DstEP,
									uint8			u8SrcEP,
									uint8			eStatus);

PUBLIC APS_Status_e apsmeBindRequest(MAC_ExtAddr_s 	*psSrcAddr,
												uint8 			u8SrcEP,
												uint8 			u8ClusterID,
												MAC_ExtAddr_s 	*psDstAddr,
												uint8 			u8DstEP);

void apsmeBindConfirm(APS_Status_e		eStatus,
							MAC_ExtAddr_s 		*psExtAddrSrc,
							uint8					u8SrcEP,
							uint8					u8ClusterID,
							MAC_ExtAddr_s 		*psExtAddrDst,
							uint8					u8DstEP);

PUBLIC APS_Status_e apsmeUnbindRequest(MAC_ExtAddr_s 	*psSrcAddr,
													uint8 		u8SrcEP,
													uint8 		u8ClusterID,
													MAC_ExtAddr_s 	*psDstAddr,
													uint8 		u8DstEP);

PUBLIC void apsmeUnbindConfirm(APS_Status_e	eStatus,
										MAC_ExtAddr_s 	*psExtAddrSrc,
										uint8				u8SrcEP,
										uint8				u8ClusterID,
										MAC_ExtAddr_s 	*psExtAddrDst,
										uint8				u8DstEP);

PUBLIC uint8 apsmeBTGetCount(void);

PUBLIC APS_BindingTable_s* apsmeBTSearchItem(MAC_ExtAddr_s 	*psSrcAddr,
															uint8 			u8SrcEP,
															uint8 			u8ClusterID,
															MAC_ExtAddr_s 	*psDstAddr,
															uint8 			u8DstEP);

PUBLIC APS_BindingTable_s* apsmeBTSearchIndex(uint8 u8Index);
PUBLIC void apsInit(void);

PUBLIC void apsdeAckfailIndication(uint16 u16DstAddr,
							uint8 	u8Radius,
							bool_t 	bSecurityEnable,
							uint8 	u8NsduHandle,
							uint8 	u8NsduLength,
							uint8	*pu8Nsdu);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/
extern PUBLIC MAC_ExtAddr_s		kzExtendedAddress;
extern PUBLIC APS_IB_s			gsApsIB;

#if defined __cplusplus
}
#endif

#endif	/* __APS_H */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
