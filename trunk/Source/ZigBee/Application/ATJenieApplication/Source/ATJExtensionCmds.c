/****************************************************************************
 *
 * MODULE:             ATJExtensionCmds.c
 *
 * COMPONENT:          $RCSfile: ATJPlatformCmds.c,v $
 *
 * VERSION:            $Name: not supported by cvs2svn $
 *
 * REVISION:           $Revision: 1.8 $
 *
 * DATED:              $Date: 2008-08-07 10:49:51 $
 *
 * STATUS:             $State: Exp $
 *
 * AUTHOR:             RDS
 *
 * DESCRIPTION:        AT-Jenie User-defined Extension Commands
 *
 * LAST MODIFIED BY:   $Author: rsmit $
 *                     $Modtime: $
 *
 *
 * HISTORY:
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

/****************************************************************************/
/***        Include files                                                 ***/
/****************************************************************************/

#include <string.h>

#include "jendefs.h"
#include "atjparser.h"
#include "ledcontrol.h"
#include "Button.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Function Prototypes                                     ***/
/****************************************************************************/
PRIVATE void vNodeHighlight(bool_t bState);
PRIVATE bool_t bGetSwitch1State(void);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Local Variables                                               ***/
/****************************************************************************/
PRIVATE char acExtCmdString[64];

/****************************************************************************/
/***    Declare details of parameter ranges or validation functions here  ***/
/****************************************************************************/
ATJ_DECLARE_VALIDATOR(on_off)=ATJ_VALIDATOR_RANGE(0,1);


/****************************************************************************/
/***        Declare any parameters to your command here                   ***/
/****************************************************************************/
ATJ_DECLARE_CMD_PARAMS(NHL) = {
    ATJ_CMD_PARAM(E_ATJ_INPUT,  E_ATJ_PARAM, 4, ATJ_PARAM_VAL_RANGE(on_off))
};

/****************************************************************************/
/***        Declare your command set here                                 ***/
/****************************************************************************/
PRIVATE tsATJCommandSet asATJExtensionCommandSet = {
    ATJ_BEGIN_COMMAND_SET
    ATJ_COMMAND_NP(NBG, bGetSwitch1State, E_ATJ_OKP, NULL),      /*'_NP' means no parameters and 'P' on 'E_ATJ_OKP' means expect return value*/
    ATJ_COMMAND   (NHL, vNodeHighlight,   E_ATJ_OK,  NULL),
    ATJ_END_COMMAND_SET
};

/****************************************************************************/
/***    Macros                                                            ***/
/****************************************************************************/


/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************
 *
 * NAME: vATJ_PlatformCommandsInit
 *
 * DESCRIPTION: Initialise AT Jenie Extension Commands
 *
 * HISTORY:
 *
 ****************************************************************************/
PUBLIC void vATJ_ExtensionCommandsInit(void)
{
    vATJ_ParserAddCommands(&asATJExtensionCommandSet);
}

/****************************************************************************
 *
 * NAME: vATJ_GetExtCommandStrings
 *
 * DESCRIPTION:
 * Extracts the 3-character mnemonic codes from the command set and returns them
 * as a comma-separated list.
 *
 * *************  DO NOT EDIT THIS FILE *****************
 *
 * PARAMETERS:
 *
 * RETURNS:
 *
 *
 ****************************************************************************/
PUBLIC char *vATJ_GetExtCommandStrings(void)
{
    tsATJCommand *psCurCmd = NULL;
    tsATJCommandSet *psCurrentSet = &asATJExtensionCommandSet;
    bool_t bFirstString = TRUE;

    memset(acExtCmdString,0,sizeof(acExtCmdString));

    for(psCurCmd = psCurrentSet->asCommands;
        psCurCmd->CommandStr[0] != '\0';        /* command with an empty name terminates list */
        psCurCmd++)
    {
        if (!bFirstString)
        {
            strcat((char *)acExtCmdString, ",");
        }
        bFirstString = FALSE;
        strcat((char *)acExtCmdString, (char *)psCurCmd->CommandStr);
    }
    /* terminate string */
    strcat((char *)acExtCmdString, "\0");

    return acExtCmdString;
}

/****************************************************************************
 *                              Local Functions
 *
 * Define the functionality of each command here
 *
 ****************************************************************************/

/****************************************************************************
 *
 * NAME: vNodeHighlight
 *
 * DESCRIPTION:
 * New local command to turn LED 1 on/off..
 *
 * PARAMETERS:      Name                    RW  Usage
 *
 *
 * RETURNS:
 * void
 *
 ****************************************************************************/
PRIVATE void vNodeHighlight(bool_t bState)
{
    vLedControl(0, bState);             /* turn LED 1 on/off */
}

/****************************************************************************
 *
 * NAME: bGetSwitch1State
 *
 * DESCRIPTION:
 * Get switch 1 state -> 1=pressed, 0=un-pressed
 *
 ****************************************************************************/
PRIVATE bool_t bGetSwitch1State(void)
{
    bool_t  bSwitch1Pressed;
    uint8   u8ButtonState;

    u8ButtonState = u8ButtonReadRfd();                  /* get value from API */
    bSwitch1Pressed = u8ButtonState & BUTTON_0_MASK;

    return bSwitch1Pressed;
}
/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
