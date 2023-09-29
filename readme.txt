ALTER DATABASE pocDatabase SET ENABLE_BROKER;

CREATE QUEUE pocQueue	
	WITH STATUS = ON,
	RETENTION = ON,
	ACTIVATION (
		PROCEDURE_NAME = procTriggerQueue,
		MAX_QUEUE_READERS = 10,
		EXECUTE AS SELF
	)
	ON [DEFAULT];
	
CREATE CONTRACT pocContract
(
    messageType SENT BY INITIATOR
);

CREATE SERVICE pocService
    ON QUEUE pocQueue
    (
        pocContract
    );

CREATE MESSAGE TYPE messageType
VALIDATION = WELL_FORMED_XML;


select * from pocQueue

SELECT *
FROM sys.service_queues;


CREATE PROCEDURE procTriggerQueue
AS
BEGIN
    DECLARE @MessageBody NVARCHAR(MAX);
    DECLARE @MessageTypeName NVARCHAR(256);
    DECLARE @ConversationHandle UNIQUEIDENTIFIER;

    WHILE (1 = 1) -- Loop infinito para processar mensagens continuamente
    BEGIN
        BEGIN TRY
            -- Inicia a transação para processar a mensagem
            BEGIN TRANSACTION;

            -- Recebe a próxima mensagem da fila
            RECEIVE TOP(1)
                @MessageBody = message_body,
                @MessageTypeName = message_type_name,
                @ConversationHandle = conversation_handle
            FROM ExpenseQueue;

            -- Verifica se há mensagens
            IF @@ROWCOUNT = 0
            BEGIN
                -- Não há mais mensagens, encerra a transação e sai do loop
                COMMIT;
                BREAK;
            END

            -- Verifique o tipo de mensagem e faça o processamento apropriado
            IF @MessageTypeName = 'ExpenseMessageTypeName'
            BEGIN
                -- Processa a mensagem de despesa (substitua 'ExpenseMessageTypeName' pelo nome correto)
                -- Pode ser necessário realizar operações de processamento aqui

                -- Commit a transação após o processamento bem-sucedido
                COMMIT;
            END
            ELSE
            BEGIN
                -- Tipo de mensagem desconhecido, encerra a transação sem processamento
                ROLLBACK;
            END
        END TRY
        BEGIN CATCH
            -- Ocorreu uma exceção durante o processamento, reverta a transação
            IF @@TRANCOUNT > 0
                ROLLBACK;

            -- Lidar com exceções como apropriado (por exemplo, registrar erros)
        END CATCH;
    END
END;
