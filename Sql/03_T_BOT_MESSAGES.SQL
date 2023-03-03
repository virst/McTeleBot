-- Create table
create table T_BOT_MESSAGES
(
  ide         NUMBER not null,
  bot_ide     NUMBER not null,
  chat_id     NUMBER(18),
  message_txt NVARCHAR2(1000) not null,
  parse_mode  NUMBER default 0 not null,
  status      NUMBER default 0 not null
);
-- Add comments to the columns 
comment on column T_BOT_MESSAGES.ide
  is 'первичный ключ';
comment on column T_BOT_MESSAGES.bot_ide
  is 'код бота';
comment on column T_BOT_MESSAGES.chat_id
  is 'особый номер чата (если требуется)';
comment on column T_BOT_MESSAGES.message_txt
  is 'текст сообщения';
comment on column T_BOT_MESSAGES.parse_mode
  is 'способ представления сообщения (0-simple; 1- Markdown; 2 - Html; 3 - MarkdownV2)';
comment on column T_BOT_MESSAGES.status
  is 'статус обработки сообщения';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_BOT_MESSAGES
  add constraint T_BOT_MESSAGES_PK primary key (IDE)
  using index ;
alter table T_BOT_MESSAGES
  add constraint T_BOT_MESSAGES_FK_BOT foreign key (BOT_IDE)
  references T_BOT_LIST (IDE);
