
#ifndef  UI_H_INCLUDED
#define  UI_H_INCLUDED

#include <jendefs.h>

PUBLIC void vUI_CbStackDataEvent(teEventType eEventType);
PUBLIC void vUI_CbInit(bool_t warmStart);
PUBLIC void vUI_CbHwEvent(uint32 u32DeviceId,uint32 u32ItemBitmap);

PUBLIC void vUI_CbMain(void);

#endif
