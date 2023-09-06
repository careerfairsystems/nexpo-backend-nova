@echo off

setlocal

call :processargs %*
call :checkInstalled docker
call :checkInstalled dotnet

docker rm -f nexpo_database >nul 2>nul
docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14 >nul 2>nul
dotnet run --project Nexpo

:checkInstalled
where.exe %1 >nul 2>nul
if %errorlevel% equ 0 (
    echo %1 is installed
) else (
    echo Error: %1 is not installed. >&2
    exit 1
)
goto :eof

:processargs
set arg=%1
if defined ARG (
    if "%arg%"=="-h" (
        call :show_help
        exit 0
    ) else if "%arg%"=="-q" (
        git config core.filemode false
        git config --local alias.run "./runBackend"
        exit 0
    ) else if "%arg%"=="-c" (
        docker-compose up -d
        exit 1
    ) else (
        echo Invalid option
        echo Try 'runBackend.bat --help' for more information
        exit 1
    )
    shift
    goto processargs
)
goto :eof

:show_help
echo Usage: ./runBackend [OPTIONS]
echo Script for running the backend
echo.
echo Options:
echo -h     Display this help message and exit
echo -c     run docker-compose
echo -q     Create an alias for this file. Is then run with "git run"
echo test by running "dotnet test Nexpo.Tests/"
echo.
goto :eof

endlocal
