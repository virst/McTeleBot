-- Create table
create table T_BOT_BASES
(
  ide     NUMBER not null,
  db_conn NVARCHAR2(1000) not null,
  note    NVARCHAR2(100)
);
-- Add comments to the table 
comment on table T_BOT_BASES
  is 'Перечень подключений к БД, для отслуживания телеграм ботом';
-- Add comments to the columns 
comment on column T_BOT_BASES.ide
  is 'Первичный ключ';
comment on column T_BOT_BASES.db_conn
  is 'Строка подключения к БД';
comment on column T_BOT_BASES.note
  is 'Примечание';
-- Create/Recreate primary, unique and foreign key constraints 
alter table T_BOT_BASES
  add constraint T_BOT_BASES_PK primary key (IDE)
  using index ;
