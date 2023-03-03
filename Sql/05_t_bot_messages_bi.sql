CREATE OR REPLACE TRIGGER t_bot_messages_bi
  BEFORE INSERT ON t_bot_messages
  FOR EACH ROW
DECLARE
  i NUMBER;
BEGIN
  IF :new.ide IS NULL THEN
    SELECT t_bot_messages_seq.nextval INTO i FROM dual;
    :new.ide := i;
  END IF;
END t_bot_messages_bi;
