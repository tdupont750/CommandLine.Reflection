#!/bin/bash
# set -x

# project specific variables
readonly PROJ_CPS='./src/DemoCliApp/DemoCliApp.csproj'
readonly PROJ_DLL='./src/DemoCliApp/bin/Release/net6.0/DemoCliApp.dll'
readonly DEMO_DIR='./.demo'

# always run from working directory of script
cd $(dirname $0)

# create log directory
if [[ ! -d $DEMO_DIR/logs ]]
then
	mkdir $DEMO_DIR/logs
fi

# clean log dir (keep last 10), don't wait for result
(ls $DEMO_DIR/logs -t | tail -n +10 | xargs -I{} rm "$DEMO_DIR/logs/{}") &

# run slow prerequisite processes in parallel
readonly VERSION_NEW=$(dotnet $DEMO_DIR/lshash/lshash.dll $(dirname $PROJ_CPS) 'bin|obj' '*.cs')

# load old version file
if [[ -f $DEMO_DIR/version_old.txt ]]
then
	readonly VERSION_OLD=$(cat $DEMO_DIR/version_old.txt)
fi

# check if alias flag is set
for i in "$@"
do
	readonly ALIAS_NAME="$(basename $0 .sh)"

	if [[ $i == "--add-alias" ]]
	then
		# check if alias already exists
		if [[ $(cat ~/.bashrc | grep "alias $ALIAS_NAME=" | wc -l) = "1" ]]
		then
			echo "ERROR: Alias '$ALIAS_NAME' already exists"
			exit 1
		fi
	
		echo '' >> ~/.bashrc
		echo "alias $ALIAS_NAME='./$ALIAS_NAME.sh \$@'" >> ~/.bashrc
		echo "RESTART YOUR SHELL TO ACTIVATE THE ALIAS"
		exit 0
	fi
done

# check if build flag is set
for i in "$@"
do
	if [[ $i == "--build-cli" ]]
	then
		readonly BUILD_FLAG=true
	fi
done

# build if: 1) dll does not exist, 2) build flag is set, or 3) version has changed
if [[ ! -f $PROJ_DLL ]] || [[ $BUILD_FLAG == true ]] || [[ $VERSION_NEW != $VERSION_OLD ]]
then
	# build flag is specified, so clean to force a build
	if [[ $BUILD_FLAG == true ]]
	then
		dotnet clean -c Release $PROJ_CPS 2>&1 > $DEMO_DIR/dotnet_clean.txt
	fi

	# build cli project
	dotnet build -c Release --verbosity minimal $PROJ_CPS 2>&1 > $DEMO_DIR/dotnet_build.txt

	# print error if build fails
	readonly BUILD_EXIT_CODE=$?
	if [[ $BUILD_EXIT_CODE != 0 ]]
	then
		cat $DEMO_DIR/dotnet_build.txt
		exit $BUILD_EXIT_CODE
	fi
	
	# success, write the version and break
	echo $VERSION_NEW > $DEMO_DIR/version_old.txt
fi

# create log file
readonly LOG_FILE="$DEMO_DIR/logs/$(date +%Y-%m-%d_%H-%M-%S_%N).txt"
echo "$@" > $LOG_FILE

# execute command line
dotnet $PROJ_DLL "$@" 2>&1 | tee -a $LOG_FILE
exit $?
