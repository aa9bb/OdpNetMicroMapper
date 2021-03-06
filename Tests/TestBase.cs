﻿using System;
using OdpNetMicroMapper;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestBase
    {
        static bool firstRun = true;
        protected DbMapper orm = new DbMapper();
        protected DbMapper sysOrm = new DbMapper();

        private void CreateUser()
        {
            if (!firstRun)
                return;

            firstRun = false;

            sysOrm.NonQueryIgnoreError("drop user onmm cascade");
            sysOrm.NonQuery("create user onmm identified by onmm");
            sysOrm.NonQuery("grant create session to onmm");
            sysOrm.NonQuery("alter user onmm quota unlimited on users");
            Console.WriteLine("user onmm created");

            var sql = "create table onmm.bigclobtest (text clob)";
            sysOrm.NonQuery(sql);

            sql = "create table onmm.rawtest (bytes raw(16))";
            sysOrm.NonQuery(sql);

            sql = "create table onmm.item_with_long (text long)";
            sysOrm.NonQuery(sql);

            sql = @"create table onmm.item_odd (id number(10) not null, yield_2date number)";
            sysOrm.NonQuery(sql);

            sql = @"create table onmm.item_composite_key (id  number(10) not null, type  number(10) not null, text varchar2(100))";
            sysOrm.NonQuery(sql);

            sql = @"create table onmm.item (id number(10) not null, name varchar2(100), decimal_value number, date_value timestamp default sysdate not null)";
            sysOrm.NonQuery(sql);

            sql = @"create sequence onmm.seq_items start with 3";
            sysOrm.NonQuery(sql);

            sql = @"create or replace function onmm.append1function (pString varchar2) return varchar2
                as
                begin
                    return pString ||  '1';
                end;";
            //todo fix elsewhere? //http://boncode.blogspot.com/2009/03/oracle-pls-00103-encountered-symbol.html
            sql = sql.Replace(Environment.NewLine, "\n");
            sysOrm.NonQuery(sql);

            sql = @"create or replace function onmm.plus1function (pNumber number) return integer
                    as
                    begin
                         return pNumber + 1;
                    end;";
            sql = sql.Replace(Environment.NewLine, "\n");
            sysOrm.NonQuery(sql);

            sql = @"create or replace procedure onmm.get_items_by_name(pName in varchar2, io_cursor out SYS_REFCURSOR) is
                    begin
                        open io_cursor for
                         select * from onmm.item t where t.name = pName;
                    end;";
            sql = sql.Replace(Environment.NewLine, "\n");
            sysOrm.NonQuery(sql);

            sql = @"create or replace procedure onmm.rename_item(pId number, pName in varchar2) is
                    begin
                        update onmm.item t set t.name = pName where t.id = pId;
                    end;";
            sql = sql.Replace(Environment.NewLine, "\n");
            sysOrm.NonQuery(sql);

            sql = @"create or replace procedure onmm.proc_with_many_out_parameters(pInput number, pEcho out number, pCount out number, io_cursor out SYS_REFCURSOR) is
                    begin
                        select pInput into pEcho from dual;
                        select count(1) into pCount from onmm.fund;
                        open io_cursor for
                            select * from onmm.fund;
                    end;";
            sql = sql.Replace(Environment.NewLine, "\n");
            sysOrm.NonQuery(sql);
        }
           
        [TearDown]
        public void DropSchema()
        {
        }

        [SetUp]
        public void CreateSchema()
        {
            throw new Exception("Please set 'data source' and 'password' in function CreateSchema()");
        		
            sysOrm.ConnectAsSys("tns", "password");
            orm.ConnectionString = "data source=tns;user id=onmm;password=onmm;";

            CreateUser();

            sysOrm.NonQuery("delete from onmm.item_composite_key");
            sysOrm.NonQuery("delete from onmm.item");
            sysOrm.NonQuery("delete from onmm.bigclobtest");
            sysOrm.NonQuery("delete from onmm.item_with_long");
            sysOrm.NonQuery("delete from onmm.item_odd");
            sysOrm.NonQuery("delete from onmm.rawtest");

            var sql = "insert into onmm.item_odd (id, yield_2date) values (1, 99)";
            sysOrm.NonQuery(sql);

            sql = "insert into onmm.item (id, name, decimal_value) values (1, 'First Item', 0.321)";
            sysOrm.NonQuery(sql);

            sql = "insert into onmm.item (id, name, decimal_value) values (2, 'Second Item', 0.123)";
            sysOrm.NonQuery(sql);

        }
    }
}
