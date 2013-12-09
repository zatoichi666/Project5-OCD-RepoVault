echo off 
cls

echo *************************************************
echo ** Verification Of MetadataRecursiveCrawl      **
echo *************************************************
echo.
echo  Press enter when ready to examine MetadataRecursiveCrawl.  
echo. 
echo AbstractCommunicator.cs is dependent on ICommLib2.cs and BlockingQueue.cs
echo BlockingQueue.cs is dependent on ICommLib.cs
echo.
echo To see the dependency tree (via the metadata contents for each Package)...
pause
cls



cd Demo\bin\Debug
type AbstractCommunicator.cs.metadata
echo.
echo.
type ICommLib2.cs.metadata
echo.
echo.
type BlockingQueue.cs.metadata
echo.
echo.
type ICommLib2.cs.metadata
echo.
echo. 
Demo.exe
pause




