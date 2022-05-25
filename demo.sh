#!/bin/bash
#set -x

# ==================== PROJECT SPECIFIC VARIABLES ====================
readonly PROJ_SRC='./src/'
readonly PROJ_CSP='./src/DemoCliApp/DemoCliApp.csproj'
readonly PROJ_DLL='./src/DemoCliApp/bin/Release/net6.0/DemoCliApp.dll'
# ====================================================================

# script variables
readonly CLI_DIR="./.cli"
readonly CLI_LSH="$CLI_DIR/lshash/lshash.dll"
readonly CLI_PROJ_NAM="$(basename $0 .sh)"
readonly CLI_PROJ_DIR="$CLI_DIR/$CLI_PROJ_NAM"
readonly CLI_PROJ_VER="$CLI_PROJ_DIR/version_old.txt"
readonly CLI_PROJ_LOG_DIR="$CLI_PROJ_DIR/logs"
readonly CLI_PROJ_LOG_TXT="$CLI_PROJ_LOG_DIR/$(date +%Y-%m-%d_%H-%M-%S_%N).txt"

# always run from working directory of script
cd $(dirname $0)

# HASH_DIR

# create cli project directories
if [[ ! -d $CLI_PROJ_DIR ]]
then
	mkdir $CLI_PROJ_DIR
fi
if [[ ! -d $CLI_PROJ_LOG_DIR ]]
then
	mkdir $CLI_PROJ_LOG_DIR
fi

# clean log dir (keep last 10), don't wait for result
(ls $CLI_PROJ_LOG -t | tail -n +10 | xargs -I{} rm "$CLI_PROJ_LOG/{}") &

# run slow prerequisite processes in parallel
readonly VERSION_NEW=$(dotnet $CLI_LSH $PROJ_SRC 'bin|obj' '*.cs')

# load old version file
if [[ -f $CLI_PROJ_VER ]]
then
	readonly VERSION_OLD=$(cat $CLI_PROJ_VER)
fi

# check if alias flag is set
if [[ $1 == "--add-alias" ]]
then

	# check if alias already exists
	if [[ $(cat ~/.bashrc | grep "alias $CLI_PROJ_NAM=" | wc -l) = "1" ]]
	then
		echo "ERROR: Alias '$CLI_PROJ_NAM' already exists"
		exit 1
	fi

	echo '' >> ~/.bashrc
	echo "alias $CLI_PROJ_NAM='./$CLI_PROJ_NAM.sh \$@'" >> ~/.bashrc
	echo "========================================================"
	echo "RESTART YOUR SHELL TO ACTIVATE THE '$CLI_PROJ_NAM' ALIAS"
	echo "========================================================"
	exit 0
fi

# check if build flag is set
if [[ $1 == "--build-cli" ]]
then
	readonly BUILD_FLAG=true
fi

if [[ $1 == "--print-log" ]]
then
	readonly LAST_LOG="$CLI_PROJ_LOG_DIR/$(ls -r -t $CLI_PROJ_LOG_DIR | tail -n 1)"
	echo $LAST_LOG
	cat $LAST_LOG
	exit 0
fi

# build if: 1) dll does not exist, 2) build flag is set, or 3) version has changed
if [[ ! -f $PROJ_DLL ]] || [[ $BUILD_FLAG == true ]] || [[ $VERSION_NEW != $VERSION_OLD ]]
then
	# build flag is specified, so clean to force a build
	if [[ $BUILD_FLAG == true ]]
	then
		dotnet clean -c Release $PROJ_CSP 2>&1 > $CLI_PROJ_DIR/dotnet_clean.txt
	fi

	# build cli project
	dotnet build -c Release --verbosity minimal $PROJ_CSP 2>&1 > $CLI_PROJ_DIR/dotnet_build.txt

	# print error if build fails
	readonly BUILD_EXIT_CODE=$?
	if [[ $BUILD_EXIT_CODE != 0 ]]
	then
		cat $CLI_PROJ_DIR/dotnet_build.txt
		exit $BUILD_EXIT_CODE
	fi
	
	# success, write the version and break
	echo $VERSION_NEW > $CLI_PROJ_VER
fi

# create log file
echo "$@
" > $CLI_PROJ_LOG_TXT

# execute command line
dotnet $PROJ_DLL "$@" 2>&1 | tee -a $CLI_PROJ_LOG_TXT
exit $?
