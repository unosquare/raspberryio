echo Generating Deployment Command
echo sshdeploy\sshdeploy.exe  monitor -s "%1" -t "%3" -h "%4" -u "%5" -w "%6" --pre "sudo pgrep -f '%7.exe' | xargs -r kill" --post "sudo mono %3/%7.exe" > %2
tasklist /nh /fi "imagename eq SshDeploy.exe" | find /i "SshDeploy.exe" > nul

IF ERRORLEVEL 1 GOTO :START
goto :EXIT

:START
echo SSH Deploy Not started. Run %2
cd ..
cd ..
REM CALL "cmd %2"
REM START CMD /C CALL "%2"
REM START "" CMD /C "%2"
REM CMD /C START "" "%2"
REM START /B "DEPLOYMENT" sshdeploy\sshdeploy.exe  monitor -s "%1" -t "%3" -h "%4" -u "%5" -w "%6" --pre "sudo pgrep -f '%7.exe' | xargs -r kill" --post "sudo mono %3/%7.exe" > %2
REM sshdeploy\winfork.exe %2
REM START /B "DEPLOYMENT" sshdeploy\winfork.exe %2
REM START "" CMD /C sshdeploy\winfork.exe %2

:EXIT
echo Writing deployment file
echo %DATE% %TIME% > %1\sshdeploy.ready
