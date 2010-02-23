#############################################################################
#
# MODULE:             AllBoardLibs.mk
# AUTHOR:             
# MODIFIED	      
#
# DESCRIPTION:        Builds the Board API for all targets
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
#  (c) Copyright 2008, Jennic Limited
#
#############################################################################

# Builds the Board API for all targets

BASE_DIR = ../../..

ifeq ($(JENNIC_PCB),DEVKIT1)
BOARDAPI_BASE = $(BASE_DIR)/Platform/DK1
else
BOARDAPI_BASE = $(BASE_DIR)/Platform/DK2
endif

export BASE_DIR
export JENNIC_PCB
export JENNIC_CHIP

include $(BASE_DIR)/Common/Build/config.mk

#########################################################################


#########################################################################

all:
	make -C $(BOARDAPI_BASE)/Build -f BoardLib.mk
	
#########################################################################

clean:
	make -C $(BOARDAPI_BASE)/Build -f BoardLib.mk clean

#########################################################################
