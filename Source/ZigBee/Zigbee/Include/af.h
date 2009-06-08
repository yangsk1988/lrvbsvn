/*****************************************************************************
 *
 * MODULE:              Jennic Zigbee: Application Framework
 *
 * COMPONENT:           $RCSfile: af.h,v $
 *
 * VERSION:             $Name: ZB_RELEASE_1v11 $
 *
 * REVISION:            $Revision: 1.7 $
 *
 * DATED:               $Date: 2008/02/27 14:53:30 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              Korwin
 *
 * DESCRIPTION:
 * Defines interface to application framework layer
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

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include "aps.h"

#ifndef __AF_H
#define __AF_H

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define AF_SIZE_SIMPLEDESC				5
#define	afcMaxEP						0xF0
#define	afcBroadcastEP					0xFF
#define	afcMaxKvpTransactionCount		9
#define afcMaxClusterCount				0x0F
#define NULL_ENDPOINT					0xFF
#define	MENUFACTURER_CODE				0x100D
#define afmeAddSimpleDesc				afmeSimpleDescAdd
#define afmeDeleteSimpleDesc			afmeSimpleDescDelete
#define afmeSearchEndpoint				afmeSimpleDescSearch
#define	afmeUserDescSetting 			afmeUserDescSet
#define JZA_vZdpResponse				JZA_ZdpResponse
#define AF_COMMAND_TYPE_ID				AF_KvpCommandTypeID_e
#define AF_ATTRIBUTE_DATA_TYPE 			AF_KvpAttributeDataType_e
#define	AF_ERROR_CODE					AF_ErrorCode_e
#define AF_SIMPLE_DESCRIPTOR			AF_SimpleDescriptor_s
#define afcMaxUserDescriptorSize		16
#define afcMaxFrameOverhead				1
#define afcMaxKvpOverhead				5
#define	afcMaxMsgOverhead				2
#define	afcMaxTransactionFieldSize		( (apscMaxPayloadSize - afcMaxFrameOverhead) )
#define	afcMaxKvpPayloadSize			( afcMaxTransactionFieldSize - (afcMaxKvpOverhead) )
#define	afcMaxMsgPayloadSize			( afcMaxTransactionFieldSize - (afcMaxMsgOverhead) )
#define afcMaxKvpStringAttrPayloadSize  (afcMaxKvpPayloadSize-1)

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef enum
{
	AF_KVP		= 0x1,
	AF_MSG
}AF_Frametype_e;

typedef enum
{
	SET									= 0x1,
	KVP_SET								= 0x1,

	EVENT								= 2,
	KVP_EVENT							= 2,

    GET_ACKNOWLEDGEMENT   				= 4,
    GET_ACKNOWLEDGMENT    				= 4,
    KVP_GET_ACKNOWLEDGMENT    			= 4,

    SET_ACKNOWLEDGEMENT   				= 5,
    SET_ACKNOWLEDGMENT    				= 5,
    KVP_SET_ACKNOWLEDGMENT    			= 5,

	EVENT_ACKNOWLEDGEMENT				= 6,
	EVENT_ACKNOWLEDGMENT				= 6,
	KVP_EVENT_ACKNOWLEDGMENT			= 6,

	GET_RESPONSE						= 0x8,
	KVP_GET_RESPONSE					= 0x8,

	SET_RESPONSE						= 9,
	KVP_SET_RESPONSE					= 9,

	EVENT_RESPONSE						= 0xa,
	KVP_EVENT_RESPONSE					= 0xa,

}AF_KvpCommandTypeID_e;


typedef enum
{
	KVP_NODATA							= 0x0,
	KVP_UNSIGNED_8BIT_INTEGER,
	KVP_SIGNED_8BIT_INTEGER,
	KVP_UNSIGNED_16BIT_INTEGER,
	KVP_SIGNED_16BIT_INTEGER,
	KVP_SEMI_PRECISION					= 0xb,
	KVP_ABSOLUTE_TIME,
	KVP_RELATIVE_TIME,
	KVP_CHARACTER_STRING,
	KVP_OCTET_STRING
}AF_KvpAttributeDataType_e;

typedef enum
{
	KVP_SUCCESS							= 0x00,
	KVP_INVALID_ENDPOINT,
	KVP_UNSUPPORTED_ATTRIBUTE			= 0x03,
	KVP_INVALID_COMMAND_TYPE,
	KVP_INVALID_ATTRIBUTE_DATA_LEN,
	KVP_INVALID_ATTRIBUTE_DATA
}AF_ErrorCode_e;

typedef struct PACK
{
	uint8					o5Reserved			: 5;
	uint8					o3LogicalType		: 3;
	uint8					o5FrequencyBand		: 5;
	uint8					o3ApsFlags			: 3;

	uint8					u8MacCapabilityFlags;
	uint16					u16ManufacturerCode;
	uint8					u8MaximumBufferSize;
	uint16					u16MaximumTransferSize;
}AF_NodeDescriptor_s;

typedef struct PACK
{
	uint8					o4AvailablePowerSource		: 4;
	uint8					o4CurrentPowerMode			: 4;
	uint8					o4CurrentPowerSourceLevel	: 4;
	uint8					o4CurrentPowerSource		: 4;
}AF_NodePowerDescriptor_s;

typedef struct PACK
{
	uint8					u8EndPoint;
	uint16					u16ProfileId;
	uint16					u16DeviceId;
	uint8					o4u8Flags		: 4;
	uint8					o4DeviceVer		: 4;
	uint8					u8InClusterCount;
	uint8					au8InClusterList[afcMaxClusterCount];
	uint8					u8OutClusterCount;
	uint8					au8OutClusterList[afcMaxClusterCount];
}AF_SimpleDescriptor_s;

typedef struct
{
	uint8					u8UserDescLen;
	uint8					u8UserDesc[afcMaxUserDescriptorSize];
}AF_UserDescriptor_s;

typedef struct
{
	uint8 u8CharacterCount;
	uint8 au8CharacterData[afcMaxKvpStringAttrPayloadSize];
}AF_Kvp_AttributeData_String_s;
typedef struct
{
	AF_KvpAttributeDataType_e	eAttributeDataType;
	AF_KvpCommandTypeID_e		eCommandTypeID;
	uint16						u16AttributeID;
	AF_ErrorCode_e 				eErrorCode;
	union
	{
		uint8 	UnsignedInt8;
		int8 	SignedInt8;
		uint16 	UnsignedInt16;
		int16 	SignedInt16;
		uint16 	SemiPrecision;
		uint32 	AbsoluteTime;
		uint32 	RelativeTime;
		AF_Kvp_AttributeData_String_s CharacterString;
		AF_Kvp_AttributeData_String_s OctetString;
	}uAttributeData;
} AF_Kvp_Transaction_s;

typedef struct
{
	uint8					u8TransactionDataLen;
	uint8 					au8TransactionData[afcMaxMsgPayloadSize];
} AF_Msg_Transaction_s;

typedef struct
{
	uint8			u8SequenceNum;
	union
	{
		AF_Msg_Transaction_s sMsg;
		AF_Kvp_Transaction_s sKvp;
	}uFrame;
}AF_Transaction_s;

typedef struct
{
	AF_SimpleDescriptor_s		*afSimpleDesc;
	uint8						afSimpleDescSize;
	AF_NodeDescriptor_s			afNodeDesc;
	AF_NodePowerDescriptor_s	afNodePowerDesc;
	AF_UserDescriptor_s			afUserDesc;
}AF_IB_s;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/
PUBLIC Stack_Status_e afdeDataRequest(APS_Addrmode_e				eAddrMode,
												uint16				u16AddrDst,
												uint8				u8DstEP,
												uint8 				u8SrcEP,
												uint16 				u16ProfileID,
												uint8 				u8ClusterID,
												AF_Frametype_e 		eFrameType,
												uint8 				u8TransCount,
												AF_Transaction_s	*pauTransactions,
												APS_TxOptions_e		eTxOptions,
												NWK_DiscoverRoute_e	eDiscoverRoute,
												uint8 				u8RadiusCounter);

PUBLIC bool_t afmeSimpleDescAdd(uint8	u8EndPoint,
								uint16	u16ProfileID,
								uint16	u16DeviceID,
								uint8	u8DeviceVersion,
								uint8	u8Flags,
								uint8	u8InClusterCount,
								uint8	*pau8InClusterList,
								uint8	u8OutClusterCount,
								uint8	*pau8OutClusterList);

PUBLIC bool_t afmeSimpleDescDelete(uint8 u8EndPoint);
PUBLIC AF_SimpleDescriptor_s* afmeSimpleDescSearch(uint8 u8EndPoint);
PUBLIC uint8 u8AfSimpleDescCount(void);
PUBLIC uint8 u8AfMakeActiveEPList(uint8 *pau8EPList);
PUBLIC void vAfMakeMatchEPList(	uint16	u16ProfileId,
								uint8		u8InClusterCount,
								uint8		*pau8InClusterList,
								uint8		u8OutClusterCount,
								uint8		*pau8OutClusterList,
								uint8		*pu8MatchEPCount,
								uint8		*pau8MatchEPList);
void afmeDescInit(void);
PUBLIC AF_NodeDescriptor_s* afmeNodeDescGet(void);
PUBLIC void afmeNodeDescUpdate(NWK_DeviceType_e eDeviceType);
PUBLIC AF_NodePowerDescriptor_s* afmeNodePowerDescGet(void);
PUBLIC bool_t afmeNodePowerDescSet(uint8 u8CurrentPowerMode,
									uint8 u8AvailablePowerSrc,
									uint8 u8CurrentPowerSrc,
									uint8 u8CurrentPowerSrcLevel);

PUBLIC AF_UserDescriptor_s* afmeUserDescGet(void);
PUBLIC bool_t afmeUserDescSet(uint8 u8UserDescLen, uint8 *pUserDesc);
PUBLIC bool_t JZA_bAfKvpObject(APS_Addrmode_e		eAddrMode,
						uint16				u16AddrSrc,
						uint8				u8SrcEP,
						uint8				u8LQI,
						uint8				u8DstEP,
						uint8				u8ClusterId,
						uint8				*pu8ClusterIDRsp,
						AF_Transaction_s	*puTransactionInd,
						AF_Transaction_s	*puTransactionRsp);

PUBLIC void JZA_vAfKvpResponse(APS_Addrmode_e		eAddrMode,
						uint16				u16AddrSrc,
						uint8				u8SrcEP,
						uint8				u8LQI,
						uint8				u8DstEP,
						uint8				u8ClusterID,
						AF_Transaction_s	*puTransactionInd);

PUBLIC bool_t JZA_bAfMsgObject(APS_Addrmode_e		eAddrMode,
						uint16				u16AddrSrc,
						uint8				u8SrcEP,
						uint8				u8LQI,
						uint8 				u8DstEP,
						uint8				u8ClusterID,
						uint8 				*pu8ClusterIDRsp,
						AF_Transaction_s	*puTransactionInd,
						AF_Transaction_s	*puTransactionRsp);

PUBLIC uint8 u8AfGetTransactionSequence(bool_t bAutoIncrease);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/
extern PUBLIC uint16		gZdoEndDevPollReqTime;
extern PUBLIC AF_IB_s		gsAFIB;

#if defined __cplusplus
}
#endif

#endif	/* __AF_H */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
