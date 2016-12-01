echo sshdeploy\sshdeploy.exe  monitor -s "%1" -t "%3" -h "%4" -u "%5" -w "%6" --pre "sudo pgrep -f '%7.exe' | xargs -r kill" --post "sudo mono %3/%7.exe" > %2
echo %DATE% %TIME% > %1\sshdeploy.ready