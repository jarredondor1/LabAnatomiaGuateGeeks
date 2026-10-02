#!/bin/zsh
set -eu
PROJECT="${0:A:h:h}"
UNITY='/Applications/Unity/Hub/Editor/6000.6.1f1/Unity.app/Contents/MacOS/Unity'
mkdir -p "$PROJECT/TestResults"
case "${1:-}" in
 configure) exec "$UNITY" -batchmode -nographics -quit -projectPath "$PROJECT" -executeMethod AnatomyProjectBuilder.Configure -logFile "$PROJECT/configure.log" ;;
 edit-tests) exec "$UNITY" -batchmode -nographics -projectPath "$PROJECT" -runTests -testPlatform EditMode -testResults "$PROJECT/TestResults/editmode.xml" -logFile "$PROJECT/editmode.log" ;;
 play-tests) exec "$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform PlayMode -testResults "$PROJECT/TestResults/playmode.xml" -logFile "$PROJECT/playmode.log" ;;
 preview) exec "$UNITY" -batchmode -projectPath "$PROJECT" -executeMethod AnatomyPreview.Run -logFile "$PROJECT/preview.log" ;;
 audit) exec "$UNITY" -batchmode -nographics -quit -projectPath "$PROJECT" -executeMethod AnatomyDatasetImporter.Validate -logFile "$PROJECT/audit.log" ;;
 build) exec "$UNITY" -batchmode -quit -buildTarget Android -projectPath "$PROJECT" -executeMethod AnatomyProjectBuilder.BuildQuest -logFile "$PROJECT/build.log" ;;
 *) print 'Usage: Tools/run_unity.sh configure|edit-tests|play-tests|preview|audit|build'; exit 2 ;;
esac
