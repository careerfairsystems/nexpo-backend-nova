@echo off

IF "%1"=="-start" (
    REM Check if the nexpo_database container exists
    docker inspect nexpo_database >nul 2>&1
    IF %errorlevel% EQU 0 (
        REM Reset the database before running the tests
        docker rm -f nexpo_database
    )
    docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
) ELSE IF "%1"=="-run" (
    REM Check if the nexpo_database container is running
    docker inspect -f {{.State.Running}} nexpo_database >nul 2>&1
    IF %errorlevel% EQU 0 (
        IF "%2"=="" (
            REM Run all tests
            dotnet test Nexpo.Tests/
        ) ELSE (
            REM Check if the test class ends with "ControllerTest"
            SET "test_class=%2"
            IF "%test_class:~-13%"=="controllertest" (
                REM Run the specific test class
                dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.%test_class%"
            ) ELSE IF "%test_class:~-10%"=="controller" (
                REM Append "test" to the test class and run
                SET "test_class=%test_class%test"
                dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.%test_class%"
            ) ELSE (
                REM Append "controllerTest" to the test class and run
                SET "test_class=%test_class%controllerTest"
                dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.%test_class%"
            )
        )
    ) ELSE (
        REM Ask user if they want to continue without starting the database
        set /p "choice=Warning: nexpo_database container is not running. It is recommended to run ./test.sh -start first. Do you want to continue without starting it? (y/n): "
        IF /i "%choice%"=="y" (
            IF "%2"=="" (
                REM Run all tests
                dotnet test Nexpo.Tests/
            ) ELSE (
                REM Check if the test class ends with "ControllerTest"
                SET "test_class=%2"
                IF "%test_class:~-13%"=="controllertest" (
                    REM Run the specific test class
                    dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.%test_class%"
                ) ELSE IF "%test_class:~-10%"=="controller" (
                    REM Append "test" to the test class and run
                    SET "test_class=%test_class%test"
                    dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.%test_class%"
                ) ELSE (
                    REM Append "controllerTest" to the test class and run
                    SET "test_class=%test_class%controllerTest"
                    dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.%test_class%"
                )
            )
        ) ELSE IF /i "%choice%"=="n" (
            echo Aborted.
        ) ELSE (
            echo Invalid choice. Aborted.
        )
    )
) ELSE IF "%1"=="-help" (
    REM Print help message
    echo Usage: ./test.sh [OPTIONS]
    echo Note: Mandatory to use options
    echo Options:
    echo -start    Reset the database before running tests
    echo           This needs to be done when the database seeding has changed
    echo           The first time this is done, the tests will fail
    echo -run      Run the tests
    echo -help     Show this help message
) ELSE (
    REM Print error message
    echo Error: no option provided.
    echo Usage: ./test.sh [OPTIONS]
    echo Options:
    echo -start    Reset the database before running tests
    echo           This needs to be done when the database seeding has changed
    echo           The first time this is done, the tests will fail
    echo -run      Run the tests
    echo -help     Get help
)