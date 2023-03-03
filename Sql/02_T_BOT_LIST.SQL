-- Create table
create table T_BOT_LIST
(
  ide     NUMBER not null,
  token   NVARCHAR2(100) not null,
  chat_id NUMBER(18)
);
-- Add comments to the columns 
comment on column T_BOT_LIST.ide
  is 'Первичный ключ';
comment on column T_BOT_LIST.token
  is 'Токен';
comment on column T_BOT_LIST.chat_id
  is 'Идентификатор чата';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_BOT_LIST
  add constraint T_BOT_LIST_PK primary key (IDE)
  using index ;
