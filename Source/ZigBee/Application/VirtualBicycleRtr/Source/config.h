#ifndef  CONFIG_H_INCLUDED
#define  CONFIG_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/
#include <jendefs.h>
#include "services.h"

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/* Network parameters - these MUST be changed to suit the target application */
#define PAN_ID                  0xABCDU
#define CHANNEL                 12
#define SERVICE_PROFILE_ID      0x12345678
#define POLL_PERIOD             10 /* in 10ths of a second */

/* UTILS config */
#define UTILS_UART              E_AHI_UART_0
#define UTILS_UART_BAUD_RATE    E_AHI_UART_RATE_115200

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* CONFIG_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/
