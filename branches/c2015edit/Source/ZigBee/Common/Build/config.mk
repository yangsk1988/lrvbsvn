#########################################################################
#
# Specify the path to the tool chain
#
#########################################################################
ifndef CROSS_COMPILE
CROSS_COMPILE = ba-elf-
endif

#########################################################################

#
# Include the make variables (CC, etc...)
#
AS	= $(CROSS_COMPILE)as
LD	= $(CROSS_COMPILE)ld
CC	= $(CROSS_COMPILE)gcc
AR	= $(CROSS_COMPILE)ar
NM	= $(CROSS_COMPILE)nm
STRIP	= $(CROSS_COMPILE)strip
OBJCOPY = $(CROSS_COMPILE)objcopy
OBJDUMP = $(CROSS_COMPILE)objdump
RANLIB	= $(CROSS_COMPILE)ranlib

CFLAGS += -I.
CFLAGS += -DOR1K
#CFLAGS += -Wstrict-prototypes
#CFLAGS += -Werror-implicit-function-declaration
#CFLAGS += -fno-strength-reduce
#CFLAGS += -pipe -fno-builtin -nostdlib

CFLAGS += -Os -Wall -fomit-frame-pointer
CFLAGS += -msibcall -mno-entri -mno-multi -mno-setcc
CFLAGS += -mno-cmov -mno-carry -mno-subb -mno-sext -mno-ror -mno-ff1
CFLAGS += -mno-hard-div -mhard-mul -mbranch-cost=3 -msimple-mul
CFLAGS += -mabi=1 -march=ba1 -mredzone-size=4

CFLAGS += -DPCB_$(JENNIC_PCB)
CFLAGS += -DEMBEDDED -DLEAN_N_MEAN -DSINGLE_CONTEXT

ifeq ($(JENNIC_CHIP),JN5121)
JENNIC_CHIP_FAMILY = JN5121
else
ifeq ($(JENNIC_CHIP),JN5139R)
JENNIC_CHIP_FAMILY = JN513xR
else
ifeq ($(JENNIC_CHIP),JN5139R1)
JENNIC_CHIP_FAMILY = JN513xR1
else
JENNIC_CHIP_FAMILY = JN513x
endif
endif
endif
LINKER_FILE = AppBuild_$(JENNIC_CHIP).ld

CHIP_BASE = $(BASE_DIR)/Chip/$(JENNIC_CHIP_FAMILY)

ifeq ($(JENNIC_PCB), DEVKIT1)
BOARDAPI_BASE = $(BASE_DIR)/Platform/DK1
endif
ifeq ($(JENNIC_PCB), DEVKIT2)
BOARDAPI_BASE = $(BASE_DIR)/Platform/DK2
endif
ifeq ($(JENNIC_PCB), HPDEVKIT)
BOARDAPI_BASE = $(BASE_DIR)/Platform/HPDevKit
endif
ifeq ($(JENNIC_PCB), NTS)
BOARDAPI_BASE = $(BASE_DIR)/Platform/NTS
endif

BOARDAPI_COMMON_PUB 	= $(BASE_DIR)/Platform/Common/Include
BOARDAPI_PLATFORM_PUB 	= $(BOARDAPI_BASE)/Include

include $(BASE_DIR)/Chip/$(JENNIC_CHIP_FAMILY)/Build/ChipConfig.mk

#########################################################################

export	AS LD CC AR NM STRIP OBJCOPY OBJDUMP \
	MAKE CFLAGS ASFLAGS CROSS_COMPILE JENNIC_CHIP_FAMILY \
	BOARDAPI_BASE BOARDAPI_COMMON_PUB BOARDAPI_PLATFORM_PUB \
	CHIP_BASE LINKER_FILE
	
	
#########################################################################

#########################################################################
