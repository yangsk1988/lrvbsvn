#############################################################################
#
# MODULE:             Jenie Application Configuration
# AUTHOR:             Mike Royce
#
# DESCRIPTION:        Provides the common parts required for a Jenie Makefile
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

ifeq ($(JENNIC_CHIP),JN5121)
JENNIC_PCB    = DEVKIT1
else
JENNIC_PCB    = DEVKIT2
endif

export JENNIC_CHIP
export JENNIC_PCB
export BASE_DIR

BUILD_CFG     ?= $(BASE_DIR)/Common/Build

include $(BUILD_CFG)/config.mk

STACK_BASE    ?= $(BASE_DIR)/Chip/$(JENNIC_CHIP_FAMILY)
JENIE_BASE    ?= $(BASE_DIR)/Jenie
JENIE_PUB     ?= $(JENIE_BASE)/Include
JENIE_LIB     ?= $(JENIE_BASE)/Library
BOARDAPI_PUB  ?= $(BOARDAPI_BASE)/Include
BOARDAPI_BLD  ?= $(BOARDAPI_BASE)/Build
BOARDAPI_LIB  ?= $(BOARDAPI_BASE)/Library
STACK_LIB     ?= $(STACK_BASE)/Library
STACK_PUB     ?= $(STACK_BASE)/Include
STACK_BLD     ?= $(STACK_BASE)/Build
GENERAL_PUB   ?= $(BASE_DIR)/Common/Include
GENERAL_SRC   ?= $(BASE_DIR)/Common/Source
GENERAL_LIB   ?= $(BASE_DIR)/Common/Library
UTILITIES_PUB ?= $(BASE_DIR)/Chip/Common/Include
UTILITIES_SRC ?= $(BASE_DIR)/Chip/Common/Source
ZIGBEE_LIB    ?= $(BASE_DIR)/Zigbee/Library
ZIGBEE_PUB    ?= $(BASE_DIR)/ZigBee/Include

BOARD_LIB     ?= BoardLib_$(JENNIC_CHIP_FAMILY)

#########################################################################

JENIE_LIB_COMMON     = $(BOARDAPI_LIB)/$(BOARD_LIB).a
JENIE_LIB_COMMON    += $(GENERAL_LIB)/libgcc.a
JENIE_LIB_COMMON    += $(GENERAL_LIB)/libc.a
JENIE_LIB_COMMON    += $(STACK_LIB)/ChipLib.a

ifeq ($(JENIE_IMPL), ZB)
# ZigBee 04 implementation

JENIE_LIB_COORDINATOR  = $(JENIE_LIB)/JenieZB04_CoordLib.a
JENIE_LIB_COORDINATOR += $(ZIGBEE_LIB)/JZ_CoordLib_Slim.a
JENIE_LIB_COORDINATOR += $(JENIE_LIB_COMMON)

JENIE_LIB_ROUTER       = $(JENIE_LIB)/JenieZB04_RouterLib.a
JENIE_LIB_ROUTER      += $(ZIGBEE_LIB)/JZ_RouterLib_Slim.a
JENIE_LIB_ROUTER      += $(JENIE_LIB_COMMON)

JENIE_LIB_ENDDEVICE    = $(JENIE_LIB)/JenieZB04_EndDLib.a
JENIE_LIB_ENDDEVICE   += $(ZIGBEE_LIB)/JZ_EndDeviceLib_Slim.a
JENIE_LIB_ENDDEVICE   += $(JENIE_LIB_COMMON)

else

ifeq ($(JENIE_IMPL), JN)
# JenNet implementation

JENIE_LIB_COORDINATOR  = $(JENIE_LIB)/Jenie_TreeCRLib.a
JENIE_LIB_COORDINATOR += $(JENIE_LIB_COMMON)

JENIE_LIB_ROUTER       = $(JENIE_LIB)/Jenie_TreeCRLib.a
JENIE_LIB_ROUTER      += $(JENIE_LIB_COMMON)

JENIE_LIB_ENDDEVICE    = $(JENIE_LIB)/Jenie_TreeEDLib.a
JENIE_LIB_ENDDEVICE   += $(JENIE_LIB_COMMON)

else
$(error JENIE_IMPL=$(JENIE_IMPL) not recognised)

endif
endif

#########################################################################

INCFLAGS  = -I$(BOARDAPI_PUB)
INCFLAGS += -I$(APP_SRC)
INCFLAGS += -I$(APP_PUB)
INCFLAGS += -I$(JENIE_PUB)
INCFLAGS += -I$(GENERAL_PUB)
INCFLAGS += -I$(STACK_PUB)
INCFLAGS += -I$(BOARDAPI_COMMON_PUB)
INCFLAGS += -I$(UTILITIES_PUB)

#########################################################################

ifeq ($(DEBUG),YES)
# remove the -Os flag and add the -g flag
CFLAGS := $(subst -Os,,$(CFLAGS)) -g
endif

#############################################################################
# default target
#############################################################################

all:

#############################################################################
# diagnostic target
#############################################################################

# application name, just for diagnostic purposes
CYGPATH = $(shell which cygpath 2> /dev/null)

ifneq ($(CYGPATH),)
CYGPATH += -a -m
APP_NAME = $(notdir $(shell $(CYGPATH) "$(APP_BASE)"))
else
CYGPATH = echo
endif

_E:=$(shell \
echo -n "****************************************" 1>&2; \
echo "*******************************************" 1>&2; \
echo " $(APP_NAME)"                                 1>&2; \
echo -n "****************************************" 1>&2; \
echo "*******************************************" 1>&2; \
echo "Options:"                                    1>&2; \
echo " Network Implementation: $(JENIE_IMPL)"      1>&2; \
echo " Debugger Enabled:       $(DEBUG)"           1>&2; \
echo " Target Chip:            $(JENNIC_CHIP)"     1>&2; \
echo -n "****************************************" 1>&2; \
echo "*******************************************" 1>&2; \
)

diagnostics:
	@echo "Paths:"

	@echo " SDK Root:";   echo -n "  "; $(CYGPATH) "$(BASE_DIR)"
	@echo " App Root:";   echo -n "  "; $(CYGPATH) "$(APP_BASE)"
	@echo " Jenie Root:"; echo -n "  "; $(CYGPATH) "$(JENIE_BASE)"

	@echo -n "****************************************"
	@echo "*******************************************"
.PHONEY: diagnostics

#############################################################################
