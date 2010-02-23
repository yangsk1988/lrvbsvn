#!/bin/bash

MAKEFILE="AllBoardLibs.mk"

function MakeApps {
	echo "************************************"
	echo "*Building ba for $1 on $2*"
	echo "************************************"
	export CFLAGS=""
	export JENNIC_CHIP=$1
	export JENNIC_PCB=$2

	make -f $MAKEFILE clean
	make -f $MAKEFILE
}

MAKEFILE_OR32="AllBoardLibs_or32.mk"

function MakeApps_or32 {
	echo "*************************************"
	echo "*Building or32 for $1 on $2*"
	echo "*************************************"
	export CFLAGS=""
	export JENNIC_CHIP=$1
	export JENNIC_PCB=$2

	make -f $MAKEFILE_OR32 clean
	make -f $MAKEFILE_OR32
}


MakeApps JN5121 DEVKIT1
MakeApps JN5121 HPDEVKIT
MakeApps JN5121 DEVKIT2
MakeApps JN5121 NTS

MakeApps JN5139R DEVKIT1
MakeApps JN5139R HPDEVKIT
MakeApps JN5139R DEVKIT2
MakeApps JN5139R NTS

MakeApps JN5139R1 DEVKIT1
MakeApps JN5139R1 HPDEVKIT
MakeApps JN5139R1 DEVKIT2
MakeApps JN5139R1 NTS

MakeApps JN5139 DEVKIT1
MakeApps JN5139 HPDEVKIT
MakeApps JN5139 DEVKIT2
MakeApps JN5139 NTS

MakeApps JN5139J01 HPDEVKIT
MakeApps JN5139J01 DEVKIT2
MakeApps JN5139J01 NTS

MakeApps JN5139T01 HPDEVKIT
MakeApps JN5139T01 DEVKIT2
MakeApps JN5139T01 NTS

MakeApps JN5147 HPDEVKIT
MakeApps JN5147 DEVKIT2
MakeApps JN5147 NTS

MakeApps JN5148 HPDEVKIT
MakeApps JN5148 DEVKIT2
MakeApps JN5148 NTS


