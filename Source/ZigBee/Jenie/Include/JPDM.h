/*****************************************************************************
 *
 * MODULE:              JPDM.h
 *
 * COMPONENT:           $RCSfile$
 *
 * VERSION:             $Name$
 *
 * REVISION:            $Revision: 11847 $
 *
 * DATED:               $Date: 2009-04-01 16:51:03 +0100 (Wed, 01 Apr 2009) $
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
#ifndef JPDM_H_INCLUDED
#define JPDM_H_INCLUDED

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

#include "jenie.h"
#include "AppHardwareApi.h" /* for flash defs */

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

#define JPDM_NAME_SIZE 16

#define JPDM_DECLARE_BUFFER_DESCRIPTION(name, ptr, size) \
    {                                           \
        JPDM_RECOVERY_STATE_NONE,               \
        name,                                   \
        (ptr),                                  \
        (size),                                 \
        NULL,                                   \
        0                                       \
    }



/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum
{
    JPDM_RECOVERY_STATE_NONE,
    JPDM_RECOVERY_STATE_NEW,
    JPDM_RECOVERY_STATE_RECOVERED,
    JPDM_RECOVERY_STATE_SAVED
} teRecoveryState;

typedef struct sJPDM_BufferDescription
{
    teRecoveryState eState;

    char acName[JPDM_NAME_SIZE];
    void  *pvBuffer;
    uint32 u32BufferSize;

    struct sJPDM_BufferDescription *psNext;
    uint16 u16OffsetInFlash;

} tsJPDM_BufferDescription;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC teJenieStatusCode eJPDM_RestoreContext(tsJPDM_BufferDescription *psDescription);
PUBLIC void vJPDM_SaveContext(void);
PUBLIC void vJPDM_EraseAllContext(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#ifndef JN5121
extern PUBLIC uint8              gJpdmSector;
extern PUBLIC uint32             gJpdmSectorSize;
extern PUBLIC teFlashChipType    gJpdmFlashType;
extern PUBLIC tSPIflashFncTable *gJpdmFlashFuncTable;
#endif

#if defined __cplusplus
};
#endif

#endif /*JPDM_H_INCLUDED*/

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
