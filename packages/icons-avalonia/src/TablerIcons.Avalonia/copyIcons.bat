@echo off

if not exist "icons" mkdir "icons"

set source="..\..\..\icons\categories"
set destination="icons"

::Not sure if this is needed
::It guarantees you have a canonical path (standard form)
for %%F in (%destination%) do set destination="%%~fF"

for /r %source% %%F in (.) do if "%%~fF" neq %destination% ROBOCOPY "%%F" %destination% /R:0  /NP /NS /NC /NFL /NDL

exit /b 0