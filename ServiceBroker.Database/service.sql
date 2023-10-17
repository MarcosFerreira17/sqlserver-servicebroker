CREATE SERVICE ConsoleService
    ON QUEUE ConsoleQueue
    (
        ConsoleContract
    );
