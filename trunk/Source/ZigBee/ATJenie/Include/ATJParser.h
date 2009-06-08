/*****************************************************************************
 *
 * MODULE:              ATJParser.h
 *
 * COMPONENT:           $RCSfile: ATJParser.h,v $
 *
 * VERSION:             $Name:  $
 *
 * REVISION:            $Revision: 1.10 $
 *
 * DATED:               $Date: 2008/07/07 08:14:08 $
 *
 * STATUS:              $State: Exp $
 *
 * AUTHOR:              MRW
 *
 * DESCRIPTION:         AT Jenie parser
 *
 * LAST MODIFIED BY:    $Author: rsmit $
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
 * Copyright Jennic Ltd 2005, 2006. All rights reserved
 *
 ****************************************************************************/

#ifndef  ATJPARSER_H_INCLUDED
#define  ATJPARSER_H_INCLUDED

#if defined __cplusplus
extern "C" {
#endif

/****************************************************************************/
/***        Include Files                                                 ***/
/****************************************************************************/

/****************************************************************************/
/***        Macro Definitions                                             ***/
/****************************************************************************/

/* maximum command size for character based channel */
/*
#ifndef ATJ_MAX_INPUT_STRING_SIZE
#error "You need to define ATJ_MAX_INPUT_STRING_SIZE"
#endif

#ifndef ATJ_MAX_OUTPUT_STRING_SIZE
#error "You need to define ATJ_MAX_OUTPUT_STRING_SIZE"
#endif
*/

#define E_ATJ_OPERTION_RESTOREDEFAULTS              0
#define E_ATJ_OPERTION_RESTOREFACTORYDEFAULTS       1
#define E_ATJ_OPERTION_STORETOFLASH                 2
#define E_ATJ_OPERTION_STORETOEFUSE                 3

#define E_ATJ_PARAM             0
#define E_ATJ_STRUCT(n)         (n)

#define ATJ_BEGIN_COMMAND_SET       NULL, { /* [I SP001222_P1 247] */
#define ATJ_END_COMMAND_SET         { "", 0, 0, NULL, NULL, 0, 0 } } /* [I SP001222_P1 248] */

#define ATJ_COMMAND(c, api, r, s) /* [I SP001222_P1 249] */ \
   { #c, 0, sizeof(asParams_##c) / sizeof(asParams_##c[0]), asParams_##c, api, r, s }
#define ATJ_COMMAND_NP(c, api, r, s) /* [I SP001222_P1 250] */ \
   { #c, 0, 0, NULL, api, r, s }

#define ATJ_COMMAND_LAST \
   { "", 0, 0, NULL, NULL, 0, 0 }

#define ATJ_DECLARE_CMD_PARAMS(c)       PRIVATE tsParamDescriptor asParams_##c[] /* [I SP001222_P1 252] */
#define ATJ_CMD_PARAM(d, st, s, v)      { d, st, s, v } /* [I SP001222_P1 253] */
#define ATJ_PARAM_VAL_NONE()            E_ATJ_VALIDATOR_NONE, NULL /* [I SP001222_P1 254] */
#define ATJ_PARAM_VAL_RANGE(v)          E_ATJ_VALIDATOR_RANGE, &uValidator_##v /* [I SP001222_P1 255] */
#define ATJ_PARAM_VAL_FUNC(v)           E_ATJ_VALIDATOR_FUNC, &uValidator_##v /* [I SP001222_P1 256] */

#define ATJ_DECLARE_VALIDATOR(v)        PRIVATE tuValidator uValidator_##v /* [I SP001222_P1 257] */
#define ATJ_VALIDATOR_RANGE(a,b)        { .Range={ a, b } } /* [I SP001222_P1 258] */
#define ATJ_VALIDATOR_FUNC(f)           { .Func={f} } /* [I SP001222_P1 259] */

/* [I SP001222_P1 326] */
#define ATJ_DECLARE_CHAR_CHANNEL(_n, _g, _p, _s, _i, _o, imax, omax) \
    PRIVATE tsATJCharStream _n##_State = { _g, _p, E_IDLE_STATE, 0, 0, 0, _i, _o, imax, omax}; \
    PRIVATE tsATJChannelDescriptor _n = { NULL, FALSE, &_n##_State, _s, FALSE, 0 }

/* [I SP001222_P1 327] */
#define ATJ_DECLARE_BLOCK_CHANNEL(_n, _p, _s ) \
    PRIVATE tsATJChannelDescriptor _n = { NULL, TRUE, _p, _s, FALSE, 0 }

/****************************************************************************/
/***        Type Definitions                                              ***/
/****************************************************************************/

typedef enum
{
    E_ATJ_VALIDATOR_NONE,
    E_ATJ_VALIDATOR_RANGE,
    E_ATJ_VALIDATOR_FUNC
} teATJValidatorType;

typedef enum
{
    E_ATJ_INPUT,
    E_ATJ_OUTPUT
} teATJParameterDirection;

typedef enum
{
    E_ATJ_OK,
    E_ATJ_OKA,
    E_ATJ_OKP,
    E_ATJ_OKV,
    E_ATJ_OKO
} teResponseType;

/* [I SP001222_P1 265, 266] begin */
typedef union {
    struct {
        uint64 u64Min;
        uint64 u64Max;
    } Range;
    struct {
        bool_t (*pfValidator)(uint64 );
    } Func;
} tuValidator;
/* [I SP001222_P1 265, 266] end */

/* [I SP001222_P1 264] begin */
typedef struct {
    uint8 eDir;         /* one of teATJParameterDirection */
    uint8 u8Struct;
    uint8 u8Size;
    uint8 eValidator;   /* one of teATJValidatorType */
    tuValidator *puValidator;
} tsParamDescriptor;
/* [I SP001222_P1 264] end */

/* [I SP001222_P1 263] begin */
typedef struct {
    char CommandStr[4];
    uint16 u16Enabled;
    uint16 u16NumParams;
    tsParamDescriptor *psParamDesc;
    void *pfAPIFunction;
    teResponseType eResponseType;
    bool_t (*pfSuccess)(uint64 rv, uint8 *au8ParamBuffer); /* [I SP001222_P1 346] */
} tsATJCommand; /* [I SP001222_P1 246] */
/* [I SP001222_P1 263] end */

typedef struct _tsATJCommandSet
{
    struct _tsATJCommandSet *psNext;
    tsATJCommand asCommands[]; /* unsized array trick in GCC / C99 */
} tsATJCommandSet;

typedef struct  {   /* [I SP001222_P1 295] */
    int16 (*pfi16GetStream)(void); /* [I SP001222_P1 296] */
    bool_t (*pfu8PutStream)(uint8 ); /* [I SP001222_P1 297] */
    enum {
        E_IDLE_STATE,
        E_READING_COMMAND_STATE,
        E_COMMAND_RESPONSE_STATE
    } eState;
    uint32 u32In;
    uint32 u32Out;
    uint32 u32Num;
    uint8 *InCommandStr;        /* AT-jenie 1.4 - array space allocated in Application */
    uint8 *OutCommandStr;       /* AT-jenie 1.4 - array space allocated in Application */
    uint16 InCommandSize;
    uint16 OutCommandSize;
} tsATJCharStream;

typedef struct tsATJChannelDescriptor { /* [I SP001222_P1 289] */
    struct tsATJChannelDescriptor *psNext;
    bool_t bBlock;
    void *pvStream; /* [I SP001222_P1 311] */
    void (*pfvSetBitRate)(uint32 );
    uint8 bOpen;
    uint8 u8ChanNum;
} tsATJChannelDescriptor;

/****************************************************************************/
/***        Exported Functions                                            ***/
/****************************************************************************/

PUBLIC void vATJ_ParserInit(bool_t bWarmStart); /* [I SP001222_P1 287] */
PUBLIC void vATJ_ParserAddCommands(tsATJCommandSet *psCommandSet); /* [I SP001222_P1 251] */
PUBLIC void vATJ_ParserAddChannel(tsATJChannelDescriptor *psChannel); /* [I SP001222_P1 288,290] */
PUBLIC void vATJ_ParserOpenChannel(tsATJChannelDescriptor *psChannel); /* [I SP001222_P1 288,291] */
PUBLIC void vATJ_ParserCloseChannel(tsATJChannelDescriptor *psChannel); /* [I SP001222_P1 288,292] */
PUBLIC bool_t vATJ_ParserProcessCharChannel(tsATJChannelDescriptor *psChannel); /* [I SP001222_P1 295,298] */
PUBLIC void vATJ_ParserProcessBlockChannel(tsATJChannelDescriptor *psChannel, uint8 *InStr); /* [I SP001222_P1 299,310] */
PUBLIC void vATJ_ParserSetCommandEnable(tsATJChannelDescriptor *psChannel, char *Commands, bool_t bEnabled); /* [I SP001222_P1 312] */
PUBLIC bool_t bATJ_ParserSetConfiguration(uint32 u32BitRate, bool_t bHexMode, bool_t bCRCEnable, bool_t bRespEnable, bool_t bBinEnable);
PUBLIC bool_t bATJ_ParserManageConfiguration(uint32 eOperation);
PUBLIC void vATJ_ParserProcessEvent(uint8 *EventStr); /* [I SP001222_P1 294] */

PUBLIC void vATJ_ParserValueToStr32(uint8 *Str, uint32 u32Val, bool_t bLeadingZero);
PUBLIC void vATJ_ParserValueToStr64(uint8 *Str, uint64 u64Val, bool_t bLeadingZero);
PUBLIC void vATJ_ParserBytesToHexStr(uint8 *Str, uint8 *au8Values, uint32 u32Len);

/****************************************************************************/
/***        Exported Variables                                            ***/
/****************************************************************************/

#if defined __cplusplus
}
#endif

#endif  /* API_H_INCLUDED */

/****************************************************************************/
/***        END OF FILE                                                   ***/
/****************************************************************************/

