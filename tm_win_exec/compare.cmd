@echo off


rem 
tm1.exe %1 -b L -v -r "L||#|||L" <compare.tm   
Pause
rem Program.cs.exe %1 -b L -v -r "L||||#||||L"  -f compare.tm    
set err=%errorlevel%
if   %err%. == 1. echo there was some error



exit
tm1.exe -v -b L -r "L||#||L" <compare.tm

exit

tm1.exe -v -b L <TuringCmp.txt

