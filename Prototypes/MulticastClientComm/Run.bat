echo off 
cls

echo *************************************************
echo ** Verification Of MulticastClientComm         **
echo *************************************************
echo.
echo Two instances will be created.  One will talk, both will hear the 10 messages it sends.
echo 20 seconds later, a third instance will be created, which will talk.  All 3 will hear the 10 messages it sends.
echo.
echo Please enter user 'A' for the first sender.  Enter 'C' for the second sender.
pause
cls

cd Demo\bin\Debug
start Demo.exe
cd ..\..\..

cd DemoB\bin\Debug
start DemoB.exe
cd ..\..\..

sleep 20

cd Demo\bin\Debug
start Demo.exe
cd ..\..\..


