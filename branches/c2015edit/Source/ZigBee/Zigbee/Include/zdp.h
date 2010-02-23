/*****************************************************************************
 *
 * MODULE:              Jennic Zigbee: Application Framework
 *
 * COMPONENT:           $RCSfile: zdp.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.6 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              Korwin
 *
 * DESCRIPTION:
 * Application Framework
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
#ifndef __ZDP_H
#define __ZDP_H

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include "af.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define ZDP_EP			0x00
#define zdpcMaxPayloadSize		 afcMaxMsgPayloadSize
#define ZDP_PROFILEID	0x0000

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef enum
{
	ZDP_SINGLE_DEVICE_RESPONSE,
	ZDP_EXTENDED_RESPONSE
}ZDP_RequestType_e;

typedef enum
{
	ZDP_NwkAddrReq				= 0x00,
	ZDP_IeeeAddrReq,
	ZDP_NodeDescReq,
	ZDP_PowerDescReq,
	ZDP_SimpleDescReq,
	ZDP_ActiveEpReq,
	ZDP_MatchDescReq,
	ZDP_ComplexDescReq			= 0x10,
	ZDP_UserDescReq,
	ZDP_DiscoveryRegisterReq,
	ZDP_EndDeviceAnnce,
	ZDP_UserDescSet,
	ZDP_EndDeviceBindReq		= 0x20,
	ZDP_BindReq,
	ZDP_UnbindReq,
	ZDP_MgmtNwkDiscReq			= 0x30,
	ZDP_MgmtLqiReq,
	ZDP_MgmtRtgReq,
	ZDP_MgmtBindReq,
	ZDP_MgmtLeaveReq,
	ZDP_MgmtDirectJoinReq,
	ZDP_NwkAddrRsp				= 0x80,
	ZDP_IeeeAddrRsp,
	ZDP_NodeDescRsp,
	ZDP_PowerDescRsp,
	ZDP_SimpleDescRsp,
	ZDP_ActiveEpRsp,
	ZDP_MatchDescRsp,
	ZDP_ComplexDescRsp			= 0x90,
	ZDP_UserDescRsp,
	ZDP_DiscoveryRegisterRsp,
	ZDP_UserDescConf			= 0x94,
	ZDP_EndDeviceBindRsp		= 0xa0,
	ZDP_BindRsp,
	ZDP_UnbindRsp,
	ZDP_MgmtNwkDiscRsp			= 0xb0,
	ZDP_MgmtLqiRsp,
	ZDP_MgmtRtgRsp,
	ZDP_MgmtBindRsp,
	ZDP_MgmtLeaveRsp,
	ZDP_MgmtDirectJoinRsp,
	ZDP_Zdo64bitAddressing		= 0xff
}ZDP_ClusterID_e;


typedef enum
{
	ZDP_SUCCESS_VALID = 0X00,
	ZDP_INV_REQUESTTYPE = 0x80,
	ZDP_DEVICE_NOT_FOUND,
	ZDP_INV_EP,
	ZDP_NOT_ACTIVE,
	ZDP_NOT_SUPPORTED,
	ZDP_TIMEOUT,
	ZDP_NO_MATCH,
	ZDP_TABLE_FULL,
	ZDP_NO_ENTRY,
	ZDP_NO_DESCRIPTOR
}ZDP_Status_e;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC Stack_Status_e eZdpAfdeDataReq(APS_Addrmode_e		eAddrMode,
												uint16					u16DstAddr,
												ZDP_ClusterID_e		eClusterID,
												uint8						u8AfduLength,
												uint8						*pau8Afdu,
												APS_TxOptions_e		eTxOptions );

PUBLIC APS_TxOptions_e eZdpMakeTxOptions(APS_FrameControl_f	*bFrameCtrl);

PUBLIC Stack_Status_e zdpNwkAddrReq(MAC_ExtAddr_s sExtAddr,
												ZDP_RequestType_e eReqType,
												uint8 u8StartIndex,
												APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e  zdpNwkAddrRsp(APS_FrameControl_f		bFrameCtrl,
												uint16					u16AddrSrc,
												ZDP_Status_e			eZdpStatus,
												MAC_ExtAddr_s			sExtAddr,
												uint16					u16AddrRemote,
												uint8						u8AssocDevCount,
												uint8						u8StartIndex,
												uint16					*pu16AddrList);

PUBLIC Stack_Status_e  zdpIeeeAddrReq(uint16 			u16AddrInterest,
												ZDP_RequestType_e eReqType,
												uint8 				u8StartIndex,
												APS_TxOptions_e 	eTxOptions);

PUBLIC Stack_Status_e zdpIeeeAddrRsp(APS_FrameControl_f		bFrameCtrl,
												uint16				u16AddrSrc,
												ZDP_Status_e		eZdpStatus,
												MAC_ExtAddr_s		sExtAddr,
												uint16				u16AddrRemote,
												uint8					u8AssocDevCount,
												uint8					u8StartIndex,
												uint16				*pu16AddrList);

PUBLIC Stack_Status_e zdpNodeDescReq(uint16 u16AddrInterest, APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpNodeDescRsp(APS_FrameControl_f	bFrameCtrl,
												uint16					u16AddrSrc,
												ZDP_Status_e			eZdpStatus,
												uint16					u16AddrInterest,
												AF_NodeDescriptor_s	*pDesc);

PUBLIC Stack_Status_e zdpPowerDescReq(uint16 u16AddrInterest, APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpPowerDescRsp(APS_FrameControl_f			bFrameCtrl,
												uint16							u16AddrSrc,
												ZDP_Status_e					eZdpStatus,
												uint16							u16AddrInterest,
												AF_NodePowerDescriptor_s	*pDesc);

PUBLIC Stack_Status_e zdpSimpleDescReq(uint16 u16AddrInterest, uint8 nEndPoint, APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpSimpleDescRsp(APS_FrameControl_f	bFrameCtrl,
													uint16					u16AddrSrc,
													ZDP_Status_e			eZdpStatus,
													uint16					u16AddrInterest,
													uint8						nDescLength,
													uint8						*pDesc);

PUBLIC Stack_Status_e zdpActiveEpReq(uint16 u16AddrInterest, APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpActiveEpRsp(APS_FrameControl_f		bFrameCtrl,
												uint16				u16AddrSrc,
												ZDP_Status_e		eZdpStatus,
												uint16				u16AddrInterest,
												uint8					nActiveEP,
												uint8					*pActiveEPList);

PUBLIC Stack_Status_e zdpMatchDescReq(uint16		u16AddrInterest,
												uint16		u16ProfileID,
												uint8			u8InClusterCount,
												uint8			*pau8InClusterList,
												uint8			u8OutClusterCount,
												uint8			*pau8OutClusterList,
												APS_TxOptions_e		eTxOptions);

PUBLIC Stack_Status_e zdpMatchDescRsp(APS_FrameControl_f		bFrameCtrl,
												uint16					u16AddrSrc,
												ZDP_Status_e			eZdpStatus,
												uint16					u16AddrInterest,
												uint8						nMatchLength,
												uint8						*pMatchList);

PUBLIC Stack_Status_e zdpUserDescReq(uint16 u16AddrInterest, APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpUserDescRsp(APS_FrameControl_f	bFrameCtrl,
												uint16					u16AddrSrc,
												ZDP_Status_e			eZdpStatus,
												uint16					u16AddrInterest,
												AF_UserDescriptor_s	*psUserDesc);

PUBLIC Stack_Status_e zdpEndDeviceAnnce(uint16 u16DstAddr, APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpUserDescSet(uint16 u16AddrInterest,
												AF_UserDescriptor_s *pUserDesc,
												APS_TxOptions_e eTxOptions);

PUBLIC Stack_Status_e zdpUserDescConf(APS_FrameControl_f		bFrameCtrl,
												uint16					u16AddrSrc,
												ZDP_Status_e			eZdpStatus,
												uint16					u16AddrInterest);

PUBLIC Stack_Status_e zdpEndDeviceBindReq(uint16		u16AddrBindingTarget,
														uint8			u8EndPoint,
														uint16		u16ProfileID,
														uint8			u8InClusterCount,
														uint8			*pau8InClusterList,
														uint8			u8OutClusterCount,
														uint8			*pau8OutClusterList,
														APS_TxOptions_e		eTxOptions);

PUBLIC Stack_Status_e zdpEndDeviceBindRsp(APS_FrameControl_f bFrameCtrl, uint16 u16AddrSrc, ZDP_Status_e eZdpStatus);

PUBLIC Stack_Status_e zdpBindReq(MAC_ExtAddr_s				sExtAddrSrc,
											uint8							u8SrcEP,
											ZDP_ClusterID_e			eClusterID,
											MAC_ExtAddr_s				sExtAddrDst,
											uint8							u8DstEP,
											APS_TxOptions_e 			eTxOptions);

PUBLIC Stack_Status_e zdpBindRsp(APS_FrameControl_f bFrameCtrl, uint16 u16AddrSrc, ZDP_Status_e eZdpStatus);

PUBLIC Stack_Status_e zdpUnbindReq(MAC_ExtAddr_s		sExtAddrSrc,
											uint8						u8SrcEP,
											ZDP_ClusterID_e		eClusterID,
											MAC_ExtAddr_s			sExtAddrDst,
											uint8						u8DstEP,
											APS_TxOptions_e		eTxOptions);

PUBLIC Stack_Status_e zdpUnbindRsp(APS_FrameControl_f bFrameCtrl, uint16 u16AddrSrc, ZDP_Status_e eZdpStatus);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/
extern PUBLIC uint8	gu8ZdpEp;


#if defined __cplusplus
}
#endif

#endif	/* __ZDP_H */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
