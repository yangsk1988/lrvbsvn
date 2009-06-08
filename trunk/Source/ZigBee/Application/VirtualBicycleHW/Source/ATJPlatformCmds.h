/*****************************************************************************
 *
 * MODULE:              JRTSCmds.h
 *
 * COMPONENT:           $RCSfile: ATJPlatformCmds.h,v $
 *
 * VERSION:             $Name: not supported by cvs2svn $
 *
 * REVISION:            $Revision: 1.3 $
 *
 * DATED:               $Date: 2008-05-09 10:22:10 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              MRW
 *
 * DESCRIPTION:         Interrupt enabling / disabling
 *
 * $Log: not supported by cvs2svn $
 * Revision 1.2  2007/10/18 13:57:08  mwild
 * Fixed source header
 *
 * Revision 1.1  2007/10/03 14:06:17  mwild
 * Added platform support
 *
 * Revision 1.2  2007/09/25 09:03:21  mwild
 * Updated comments and headers
 *
 *
 * LAST MODIFIED BY:    $Author: btayl $
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
 * Copyright Jennic Ltd 2007. All rights reserved
 *
 ****************************************************************************/

#ifndef ATJPLATFORMCMDS_H_
#define ATJPLATFORMCMDS_H_

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vATJ_PlatformCommandsInit(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif /*ATJPLATFORMCMDS_H_*/


/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/


