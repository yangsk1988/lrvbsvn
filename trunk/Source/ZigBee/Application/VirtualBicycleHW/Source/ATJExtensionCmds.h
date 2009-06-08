/*****************************************************************************
 *
 * MODULE:              ATJExtensioinCmds.h
 *
 * COMPONENT:           $RCSfile: ATJExtensioinCmds.h,v $
 *
 * VERSION:             $Name: not supported by cvs2svn $
 *
 * REVISION:            $Revision: 1.3 $
 *
 * DATED:               $Date: 2008-05-09 10:22:10 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              RDS
 *
 * DESCRIPTION:         Interrupt enabling / disabling
 *
 * $Log: not supported by cvs2svn $
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
 * Copyright Jennic Ltd 2009. All rights reserved
 *
 ****************************************************************************/

#ifndef ATJEXTENSIONCMDS_H_
#define ATJEXTENSIONCMDS_H_

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

PUBLIC void vATJ_ExtensionCommandsInit(void);
PUBLIC char *vATJ_GetExtCommandStrings(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif /*ATJEXTENSIONCMDS_H_*/


/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/


