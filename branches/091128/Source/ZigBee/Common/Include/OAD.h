/*****************************************************************************
 *
 * MODULE:              Over Air Download API header
 *
 * COMPONENT:           $RCSfile: OAD.h,v $
 *
 * VERSION:             $Name: RD_RELEASE_6thMay09 $
 *
 * REVISION:            $Revision: 1.2 $
 *
 * DATED:               $Date: 2008/10/22 12:09:43 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              DClar
 *
 * DESCRIPTION:
 * Access functions and structures used by the application to initiate/receive an Over Air Download.
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
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ***************************************************************************/


#ifndef  OAD_API_H_INCLUDED
#define  OAD_API_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include <jendefs.h>
#include <AppHardwareApi.h>


/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/
#define OAD_MAX_NUM_DATA_BYTES 80 //Maximum Size of chunk is 80 bytes
#define OAD_HEADER_SIZE 10
#define OAD_MAX_NUM_CHUNKS 1229 //Based on Maximum Image size of 96K and Chunk size of 80
#define OAD_FLASHHEADER_SIZE 0x30
/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/



/**
* Over Air Download Packet types,
*/

typedef enum
{
    E_OAD_PACKET_RESET      = 4,
    E_OAD_PACKET_INVALIDATE = 7,
} teOAD_PayloadType;



/**
* Structures Provide details of OAD messages
*/
typedef struct
{
  uint8 u8Version[2];
  uint8 u8DeviceID[2];
  uint8 u8ServerID[2];
  uint8 u8ImageLength[4];
  uint8 u8NumberOfChunks[2];
  uint8 u8FlashHeader[OAD_FLASHHEADER_SIZE];
}tOAD_StartPayload_s;


/*
* Forces Device To Reset
*/
typedef struct
{
  uint32 u8Blank;
}tOAD_ResetPayload_s;


typedef struct
{
  uint8 u8Version[2];
  uint8 u8DeviceID[2];
  uint8 u8ServerID[2];
  uint8 u8NumberOfChunks[2];
  uint8 u8ChunkNumber[2];
  uint8 u8AddrOffset[4];
  uint8 u8DataLength;
  uint8 u8EndOfSequence;
  uint8 u8Data[OAD_MAX_NUM_DATA_BYTES];
}tOAD_ChunkPayload_s;


typedef struct
{
  uint8 u8Version[2];
  uint8 u8DeviceID[2];
  uint8 u8ServerID[2];
  uint8 u8ChunkNumber[2];
}tOAD_ReqChunkPayload_s;


typedef struct
{
  uint8 u8Blank;
}tOAD_RevertPayload_s;



typedef struct
{
  uint8  u8Version[2];
  uint8  u8DeviceID[2];
}tOAD_CompletePayload_s;


typedef struct
{
  uint8  u8CurrentVersion[2];
  uint8  u8DeviceID[2];
  uint8  u8ServerID[2];
}tOAD_ReqOADPayload_s;


/*
* Invalidate Payload Structure
*/
typedef struct
{
  uint8  u8ImageNumber; /* Image to Invalidate*/
  uint8  u8ForceReset;  /* Reset after Invalidate*/
}tOAD_InvalidatePayload_s;

 /**
 * @brief OAD transmission Parameter union
 *
 * Union of all the possible OAD payloads
 */
typedef union
{
    tOAD_StartPayload_s      sStartPayload;
    tOAD_CompletePayload_s   sCompletePayload;
    tOAD_ChunkPayload_s      sChunkPayload;
    tOAD_ReqChunkPayload_s   sReqChunkPayload;
    tOAD_ResetPayload_s      sResetPayload;
    tOAD_RevertPayload_s     sRevertPayload;
    tOAD_ReqOADPayload_s     sReqOADPayload;
    tOAD_InvalidatePayload_s sInvalidatePayload;
} tOAD_Payload_u;


/**
*  The Payload Structure
*  Format
*  HDR  0x00,0x0A,0x0D,0x0,0x0A,0x0D,0x0,0x0A,0x0D,0x0
*  TYPE     E_OAD_PACKET_START      = 0,
*           E_OAD_PACKET_COMPLETE   = 1,
*           E_OAD_PACKET_CHUNK      = 2,
*           E_OAD_PACKET_REQCHUNK   = 3,
*           E_OAD_PACKET_RESET      = 4,
*           E_OAD_PACKET_REVERT     = 5,
*           E_OAD_PACKET_REQOAD     = 6,
*           E_OAD_PACKET_INVALIDATE = 7,
*/

typedef struct
{
    uint8                OADPacketHeader[OAD_HEADER_SIZE];
    uint8                uint8PayloadType;
    tOAD_Payload_u       uPayload;
}tOAD_Payload_s;




/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/


/****************************************************************************
 *
 * NAME:       vOADInit
 */
/**
 * Enable Over Air Downloads
 *
 * @ingroup
 *          bool bInitOAD:       TRUE enable Over Air Download, False disable
 *
 * @return void
 *
 * @note
 *
 ****************************************************************************/
PUBLIC void vOADInitClient(bool_t bInitOAD);




/****************************************************************************
 *
 * NAME:        vOADInvalidateImage
 */
/**
 * Invalidate the software image. Can be used to force the node into download mode
 * in the bootloader
 *
 * @ingroup
 *        uint8 u8ImageNumber
 *
 * @return
 *        void
 * @note
 *
 ****************************************************************************/
PUBLIC void vOADInvalidateImage(uint8 u8ImageNumber);
//Alias for initial version only have one sw image
#define vOADInvalidateSWImage() vOADInvalidateImage(0);




/****************************************************************************
 *
 * NAME:        bProcessOADReceivedDataPacket
 */
/**
 *
 *
 * @ingroup
 *          uint8 *pu8Data: Ponter to data payload
 *          uint16 u16SrcAddr: Source Short Address of Data
 *
 * @return bool, True if packet is identified is an OAD packet
 *
 * @note
 *
 ****************************************************************************/
PUBLIC bool_t bOADProcessReceivedDataPacket(uint16 u16SrcAddr, uint8 *pu8Data);




/****************************************************************************
 *
 * NAME:       vOADSendReset
 */
/**
 *  Sends reset to device so that it can load new image and join network
 *
 * @ingroup   uint16 u16DeviceAddress: Short address of device to reset
 *            uint16 u16PanId: Network Pan ID of current network
 *            uint16 u16SrcAddr: Short Address of Source of Reset
 *
 * @return void
 *
 * @note
 *
 ****************************************************************************/
PUBLIC void vOADSendReset(uint16 u16DeviceAddress, uint16 u16PanId, uint16 u16SrcAddr);

PUBLIC void vOADSendInvalidate(uint16 u16DeviceAddress,  uint16 u16PanId, uint16 u16SrcAddr, uint8  u8Image, bool_t bForceReset);
/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* OAD_API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/

