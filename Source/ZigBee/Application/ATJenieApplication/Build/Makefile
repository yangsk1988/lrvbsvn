#############################################################################
#
# MODULE:       ATJenie Application      
# AUTHOR:       
#
# DESCRIPTION: ATJenie Application
# 
#
#############################################################################
#
#  This software is owned by Jennic and/or its supplier and is protected
#  under applicable copyright laws. All rights are reserved. We grant You,
#  and any third parties, a license to use this software solely and
#  exclusively on Jennic products. You, and any third parties must reproduce
#  the copyright and warranty notice and any other legend of ownership on each
#  copy or partial copy of the software.
#
#  THIS SOFTWARE IS PROVIDED "AS IS". JENNIC MAKES NO WARRANTIES, WHETHER
#  EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, IMPLIED
#  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE,
#  ACCURACY OR LACK OF NEGLIGENCE. JENNIC SHALL NOT, IN ANY CIRCUMSTANCES,
#  BE LIABLE FOR ANY DAMAGES, INCLUDING, BUT NOT LIMITED TO, SPECIAL,
#  INCIDENTAL OR CONSEQUENTIAL DAMAGES FOR ANY REASON WHATSOEVER.
#
#  (c) Copyright 2007, Jennic Limited
#
#############################################################################

TARGET = ATJenie_App


#JENNIC_CHIP   = JN5121
JENNIC_CHIP   = JN513x
T_APP_BUILD = JN5139

ifeq ($(JENNIC_CHIP),JN5121)
JENNIC_PCB    = DEVKIT1
else
  ifdef PLATFORM
  JENNIC_PCB    = $(PLATFORM)
  else
  JENNIC_PCB    = DEVKIT2
  endif
endif

BASE_DIR      = ../../..

export JENNIC_CHIP
export JENNIC_PCB
export BASE_DIR

BUILD_CFG     = $(BASE_DIR)/Common/Build

include $(BUILD_CFG)/config.mk

CHIP_BASE     = $(BASE_DIR)/Chip/$(JENNIC_CHIP_FAMILY)

ATJENIE_PUB   = $(BASE_DIR)/ATJenie/Include
ATJENIE_LIB   = $(BASE_DIR)/ATJenie/Library
JENIE_PUB     = $(BASE_DIR)/Jenie/Include
JENIE_LIB     = $(BASE_DIR)/Jenie/Library
GENERAL_PUB   = $(BASE_DIR)/Common/Include
UTILITIES_PUB = $(BASE_DIR)/Chip/Common/Include
CHIP_PUB      = $(CHIP_BASE)/Include

#########################################################################

APPOBJS = queue.o
APPOBJS += serial.o
APPOBJS += uart.o
APPOBJS += Interrupt.o
APPOBJS += ATJPlatformCmds.o
APPOBJS += ATJExtensionCmds.o
ifeq ($(DEBUG_LCD), 1)
APPOBJS += Lcd.o
endif

#########################################################################

BLDDEPS           = $(APPOBJS:.o=.d) ATJenie_App_Coord.d \
					ATJenie_App_Router.d ATJenie_App_EndD.d

#########################################################################

ifeq ($(DEBUG), 1)
CFLAGS       := $(subst -Os,,$(CFLAGS))
CFLAGS		 += -g -O0 -DGDB
$(info Building debug version ...)
endif

ifeq ($(DEBUG_LCD), 1)
CFLAGS		 += -DDEBUG_LCD
endif

#########################################################################

# ATJ Tunnelling support
CFLAGS		 += -DATJ_TUNNELING

#########################################################################

# ATJ set serial comms to use UART0 
CFLAGS		 += -DUART=0

#########################################################################

# ATJ set input output character buffer sizes
CFLAGS		 += -DATJ_MAX_INPUT_STRING_SIZE=384
CFLAGS		 += -DATJ_MAX_OUTPUT_STRING_SIZE=384

#########################################################################

INCFLAGS  = -I$(ATJENIE_PUB)
INCFLAGS += -I$(JENIE_PUB)
INCFLAGS += -I$(BOARDAPI_COMMON_PUB)
INCFLAGS += -I$(BOARDAPI_PLATFORM_PUB)
INCFLAGS += -I$(GENERAL_PUB)
INCFLAGS += -I$(UTILITIES_PUB)
INCFLAGS += -I$(CHIP_PUB)

#########################################################################

-include $(BLDDEPS)

%.d: # if .d is missing then force rebuilding of .o
	@rm -f $*.o

#########################################################################

vpath %.c ../Source

all: $(TARGET)_Coord.bin $(TARGET)_Router.bin $(TARGET)_EndD.bin

ATJenie_App_Coord.o: ATJenie_App.c 
	$(CC) -c -o $@ -DATJ_COORDINATOR $(CFLAGS) $(INCFLAGS) $< -MD -MF $*.d -MP
	
ATJenie_App_Router.o: ATJenie_App.c 
	$(CC) -c -o $@ -DATJ_ROUTER $(CFLAGS) $(INCFLAGS) $< -MD -MF $*.d -MP
	
ATJenie_App_EndD.o: ATJenie_App.c 
	$(CC) -c -o $@ -DATJ_ENDDEVICE $(CFLAGS) $(INCFLAGS) $< -MD -MF $*.d -MP

%.o: %.c
	$(CC) -c -o $*.o $(CFLAGS) $(INCFLAGS) $< -MD -MF $*.d -MP

%.o: %.S
	$(CC) -c -o $*.o $(ASFLAGS) $<

$(TARGET)_Coord.elf $(TARGET)_Router.elf: ATJenie_App_Router.o ATJenie_App_Coord.o $(APPOBJS)
	$(CC) -nostartfiles -L$(CHIP_BASE)/Build -TAppBuild_$(T_APP_BUILD).ld -o$@ $(APPOBJS) $(basename $@).o $(ATJENIE_LIB)/ATJenie.a  $(JENIE_LIB)/Jenie_TreeCRLib.a $(CHIP_BASE)/Library/ChipLib.a $(CHIP_BASE)/Library/OADLib_$(JENNIC_CHIP).a $(BOARDAPI_BASE)/Library/BoardLib_$(JENNIC_CHIP_FAMILY).a 

$(TARGET)_EndD.elf: ATJenie_App_EndD.o $(APPOBJS)
	$(CC) -nostartfiles -L$(CHIP_BASE)/Build -TAppBuild_$(T_APP_BUILD).ld -o$@ $(APPOBJS) $(basename $@).o $(ATJENIE_LIB)/ATJenie.a $(JENIE_LIB)/Jenie_TreeEDLib.a $(CHIP_BASE)/Library/ChipLib.a $(CHIP_BASE)/Library/OADLib_$(JENNIC_CHIP).a $(BOARDAPI_BASE)/Library/BoardLib_$(JENNIC_CHIP_FAMILY).a
 
$(TARGET)_Coord.bin: $(TARGET)_Coord.elf
	ba-elf-objcopy -S -O binary $< $@
    
$(TARGET)_Router.bin: $(TARGET)_Router.elf
	ba-elf-objcopy -S -O binary $< $@
    
$(TARGET)_EndD.bin: $(TARGET)_EndD.elf
	ba-elf-objcopy -S -O binary $< $@
	
#########################################################################

.PHONY: clean
clean:
	rm -f *.o *.elf *.bin
	
#########################################################################
