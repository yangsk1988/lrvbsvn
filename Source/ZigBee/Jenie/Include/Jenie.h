/*****************************************************************************
 *
 * MODULE:              jenie.h
 *
 * COMPONENT:           $RCSfile$
 *
 * VERSION:             $Name$
 *
 * REVISION:            $Revision: 11846 $
 *
 * DATED:               $Date: 2009-04-01 16:50:14 +0100 (Wed, 01 Apr 2009) $
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

#ifndef  JENIE_H_INCLUDED
#define  JENIE_H_INCLUDED

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
/* tx options */
#define TXOPTION_ACKREQ     0x01
#define TXOPTION_SECURE     0x02
#define TXOPTION_BDCAST     0x04
#define TXOPTION_SILENT     0x08

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/
typedef struct
{
    uint16 u16LogicalEntryNo;       // Entry number (physical or logical)
    bool_t bEntryInUse;             // If in use
    uint8 u8Status;                 // Status??
    uint64 u64DestAddr;          // Jenie dest address
    uint64 u64NextHopAddr;        // Jenie next hop address
}tsJenie_RtgInfo;

typedef struct
{
    uint16 u16LogicalEntryNo;       // Entry number (physical or logical)
    uint64 u64DestAddr;          // Jenie dest address
    bool_t bRxOnIdle;
}tsJenie_NgbrInfo;

typedef struct
{
    uint16  u16EntryNum;        // Entry number
    uint16  u16TotalEntries;    // Total number of entries
    uint64  u64DestAddr;        // dest address
    uint64  u64NextHopAddr;     // next hop address
}tsJenie_RoutingEntry;

typedef struct
{
    uint8   u8EntryNum;         // Entry
    uint8   u8TotalEntries;     // Total number of entries
    uint64  u64Addr;            // Address of node
    bool_t  bSleepingED;        // If device is a sleeping node
    uint32  u32Services;        // Services provided by the node
    uint8   u8LinkQuality;      // Last received link quality info
    uint16  u16PktsLost;        // No of packets not acknowledged
    uint16  u16PktsSent;        // Acknowledged packets sent to node
    uint16  u16PktsRcvd;        // Pkts rx’d from node
}tsJenie_NeighbourEntry;

typedef struct
{
    uint32 u32register0;
    uint32 u32register1;
    uint32 u32register2;
    uint32 u32register3;
}tsJenieSecKey;

typedef enum
{
    E_JENIE_REG_SVC_RSP,
    E_JENIE_SVC_REQ_RSP,
    E_JENIE_POLL_CMPLT,
    E_JENIE_PACKET_SENT,
    E_JENIE_PACKET_FAILED,
    E_JENIE_NETWORK_UP,
    E_JENIE_CHILD_JOINED,
    E_JENIE_DATA,
    E_JENIE_DATA_TO_SERVICE,
    E_JENIE_DATA_ACK,
    E_JENIE_DATA_TO_SERVICE_ACK,
    E_JENIE_STACK_RESET,
    E_JENIE_CHILD_LEAVE,
    E_JENIE_CHILD_REJECTED
}teEventType;

typedef enum
{
    E_JENIE_COORDINATOR,
    E_JENIE_ROUTER,
    E_JENIE_END_DEVICE
}teJenieDeviceType;

typedef enum
{
    E_JENIE_SUCCESS,
    E_JENIE_DEFERRED,
    E_JENIE_ERR_UNKNOWN,
    E_JENIE_ERR_INVLD_PARAM,
    E_JENIE_ERR_STACK_RSRC,
    E_JENIE_ERR_STACK_BUSY
}teJenieStatusCode;

typedef enum
{
    E_JENIE_COMPONENT_JENIE,
    E_JENIE_COMPONENT_NETWORK,
    E_JENIE_COMPONENT_MAC,
    E_JENIE_COMPONENT_CHIP
}teJenieComponent;

typedef enum
{
    E_JENIE_POLL_NO_DATA,
    E_JENIE_POLL_DATA_READY,
    E_JENIE_POLL_TIMEOUT
}teJeniePollStatus;

typedef enum
{
    E_JENIE_RADIO_ON  = 20,
    E_JENIE_RADIO_OFF = 21
}teJenieRadioPower;

/* Sleep Modes */
typedef enum
{
    E_JENIE_SLEEP_OSCON_RAMON,     /*32Khz Osc on and Ram On*/
    E_JENIE_SLEEP_OSCON_RAMOFF,    /*32Khz Osc on and Ram off*/
    E_JENIE_SLEEP_OSCOFF_RAMON,    /*32Khz Osc off and Ram on*/
    E_JENIE_SLEEP_OSCOFF_RAMOFF,   /*32Khz Osc off and Ram off*/
    E_JENIE_SLEEP_DEEP,            /*Deep Sleep*/
} teJenieSleepMode;

typedef struct
{
    MAC_ExtAddr_s   sDestAddr;
    uint8           u8NextHop;
    uint8           u8RouteUsage;
    bool_t          bEntryActive;
    bool_t          bRoutesToImport;
} tsJenieRoutingTable;

/* Templates for messages passed up to application layer */
typedef struct
{
    /* place holder */
}tsRegSvcRsp;

typedef struct
{
    uint64      u64SrcAddress;
    uint32      u32Services;
}tsSvcReqRsp;

typedef struct
{
    teJeniePollStatus ePollStatus;
}tsPollCmplt;

typedef struct
{
    /* place holder */
}tsPacketSent;

typedef struct
{
    /* place holder */
}tsPacketFailed;

typedef struct
{
    /* place holder */
}tsStackReset;

typedef struct
{
    uint64 u64SrcAddress;
} tsChildJoined;

typedef struct
{
    uint64 u64SrcAddress;
} tsChildLeave;

typedef struct
{
    uint64 u64SrcAddress;
} tsChildRejected;

typedef struct
{
    uint64 u64ParentAddress;
    uint64 u64LocalAddress;
    uint16 u16Depth;
    uint16 u16PanID;
    uint8  u8Channel;
}tsNwkStartUp;

typedef struct
{
    uint64  u64SrcAddress;  /* Address of message source */
    uint8   u8MsgFlags;     /* Flags indicating security enabled etc */
    uint16  u16Length;      /* Length of data payload */
    uint8   *pau8Data;      /* Pointer to data payload */
}tsData;

typedef struct
{
    uint64  u64SrcAddress;   /* Address of message source */
    uint8   u8SrcService;    /* Service on sending node */
    uint8   u8DestService;   /* Service on receiving node */
    uint8   u8MsgFlags;      /* Flags indicating security enabled etc */
    uint16  u16Length;       /* Length of data payload */
    uint8   *pau8Data;       /* Pointer to data payload */
}tsDataToService;

typedef struct
{
    uint64  u64SrcAddress;
}tsDataAck;

typedef struct
{
    uint64  u64SrcAddress;
}tsDataToServiceAck;


/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/* Management Functions */
PUBLIC teJenieStatusCode    eJenie_Start(teJenieDeviceType eDevType);
PUBLIC teJenieStatusCode    eJenie_Leave(void);
PUBLIC teJenieStatusCode    eJenie_RegisterServices(uint32 u32Services);
PUBLIC teJenieStatusCode    eJenie_RequestServices(uint32 u32Services, bool_t bMatchAll);
PUBLIC teJenieStatusCode    eJenie_BindService(uint8 u8SrcService,
                                uint64 u64DestAddr, uint8 u8DestService);
PUBLIC teJenieStatusCode    eJenie_UnBindService(uint8 u8SrcService,
                                uint64 u64DestAddr, uint8 u8DestService);
PUBLIC teJenieStatusCode    eJenie_SetPermitJoin(bool_t bPermitJoin);
PUBLIC bool_t               bJenie_GetPermitJoin(void);

/* old stats functions for NTS */
PUBLIC teJenieStatusCode eJenie_GetRtgTblEntry(uint16 u16CurrentEntry,tsJenie_RtgInfo *psResult);
PUBLIC teJenieStatusCode eJenie_GetNgbrTblEntry(uint16 u16CurrentEntry,tsJenie_NgbrInfo *psResult);

/* new stats functions for customers */
PUBLIC uint16               u16Jenie_GetRoutingTableSize(void);
PUBLIC teJenieStatusCode    eJenie_GetRoutingTableEntry(uint16 u16EntryNum,
                                          tsJenie_RoutingEntry *psRoutingEntry);
PUBLIC uint8                u8Jenie_GetNeighbourTableSize(void);
PUBLIC teJenieStatusCode    eJenie_GetNeighbourTableEntry(uint8 u8EntryNum,
                                            tsJenie_NeighbourEntry *psNeighbourEntry);
PUBLIC teJenieStatusCode    eJenie_ResetNeighbourStats(uint8 u8EntryNum);

/* Data Functions */
PUBLIC teJenieStatusCode    eJenie_SendData(uint64 u64DestAddr, uint8 *pau8Payload,
                                uint16 u16Length, uint8 u8TxFlags);
PUBLIC teJenieStatusCode    eJenie_SendDataToBoundService(uint8 u8Service,
                                uint8 *pu8Payload, uint16 u16Length, uint8 u8TxFlags);
PUBLIC teJenieStatusCode    eJenie_SetSecurityKey(tsJenieSecKey *pKey, uint64 u64Addr);
PUBLIC teJenieStatusCode    eJenie_PollParent(void);

PUBLIC teJenieStatusCode    eJenie_SetSleepPeriod(uint32 u32SleepPeriodMs);
PUBLIC teJenieStatusCode    eJenie_Sleep(teJenieSleepMode eSleepMode);
PUBLIC teJenieStatusCode    eJenie_RadioPower(int iPowerLevel, bool_t bHighPower);
PUBLIC uint32               u32Jenie_GetVersion(teJenieComponent eComponent);

/* Callbacks */
PUBLIC void     vJenie_CbConfigureNetwork(void);
PUBLIC void     vJenie_CbInit(bool_t bWarmStart);
PUBLIC void     vJenie_CbMain(void);
PUBLIC void     vJenie_CbStackMgmtEvent(teEventType eEventType, void *pvEventPrim);
PUBLIC void     vJenie_CbStackDataEvent(teEventType eEventType, void *pvEventPrim);
PUBLIC void     vJenie_CbHwEvent(uint32 u32DeviceId, uint32 u32ItemBitmap);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

extern PUBLIC bool_t    gJenie_RoutingEnabled;
extern PUBLIC uint32    gJenie_NetworkApplicationID;
extern PUBLIC uint16    gJenie_PanID;
extern PUBLIC uint8     gJenie_Channel;
extern PUBLIC uint32    gJenie_ScanChannels;
extern PUBLIC uint8     gJenie_MaxChildren;
extern PUBLIC uint8     gJenie_MaxSleepingChildren;
extern PUBLIC uint8     gJenie_MaxFailedPkts;
extern PUBLIC uint8     gJenie_MaxBcastTTL;
extern PUBLIC uint16    gJenie_RouterPingPeriod;
extern PUBLIC uint8     gJenie_EndDevicePingInterval;
extern PUBLIC uint32    gJenie_EndDeviceScanSleep;
extern PUBLIC uint32    gJenie_EndDevicePollPeriod;
extern PUBLIC bool_t    gJenie_RecoverFromJpdm;
extern PUBLIC bool_t    gJenie_RecoverChildrenFromJpdm;

extern PUBLIC uint32	gJenie_EndDeviceChildActivityTimeout;   // GNATS PR893

extern PUBLIC uint32    gJenie_RoutingTableSize;
extern PUBLIC void     *gJenie_RoutingTableSpace;


#if defined __cplusplus
}
#endif

#endif  /* API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
