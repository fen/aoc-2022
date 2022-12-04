@echo off
if [%1] equ [] goto :error
if [%2] equ [] goto :error

dotnet run -c Release --project %1\%1.csproj -- %2
goto :done

:error
echo aoc ^<day^> ^<input^>
exit -1

:done
exit 0
