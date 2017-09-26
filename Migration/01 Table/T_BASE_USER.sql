create table  T_BASE_USER
(
       Id                bigint identity(1, 1) not null,
       BUSurname         nvarchar(20) default '' not null,
       BUGivenname       nvarchar(20) default '' not null,
       BUJobNumber       nvarchar(20) default '' not null,
       BUSex             tinyint default 0 not null,
       BUAvatars         nvarchar(200) default '' not null,
       BUPhoneNum        nvarchar(50) default '' not null,
       BUEmail           nvarchar(50) default '' not null,
       BUDepartId        bigint default 0 not null,
       BUTitle           nvarchar(100) default '' not null,
       BUCreateUserId    bigint default 0 not null,
       BUCreateUserName  nvarchar(50) default '' not null,
       BUCreateTime      datetime default getdate() not null,
       BUOperateUserId   bigint default 0 not null,
       BUOperateUserName nvarchar(50) default '' not null,
       BUOperateTime     datetime default getdate() not null,
       BUIsValid         tinyint default 1 not null
);
alter  table T_BASE_USER
       add constraint PK_T_BASE_USER_Id primary key (Id);
EXEC sp_addextendedproperty 'MS_Description', '人员信息表 User Info', 'user', dbo, 'table', T_BASE_USER, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '人员表Id', 'user', dbo, 'table', T_BASE_USER, 'column', Id;
EXEC sp_addextendedproperty 'MS_Description', 'Surname 姓', 'user', dbo, 'table', T_BASE_USER, 'column', BUSurname;
EXEC sp_addextendedproperty 'MS_Description', 'Givenname 名', 'user', dbo, 'table', T_BASE_USER, 'column', BUGivenname;
EXEC sp_addextendedproperty 'MS_Description', 'JobNumber 工号', 'user', dbo, 'table', T_BASE_USER, 'column', BUJobNumber;
EXEC sp_addextendedproperty 'MS_Description', 'Sex 性别(1-男，2-女，0-其他)', 'user', dbo, 'table', T_BASE_USER, 'column', BUSex;
EXEC sp_addextendedproperty 'MS_Description', 'Avatars 头像', 'user', dbo, 'table', T_BASE_USER, 'column', BUAvatars;
EXEC sp_addextendedproperty 'MS_Description', 'PhoneNum 电话', 'user', dbo, 'table', T_BASE_USER, 'column', BUPhoneNum;
EXEC sp_addextendedproperty 'MS_Description', 'Email 邮箱', 'user', dbo, 'table', T_BASE_USER, 'column', BUEmail;
EXEC sp_addextendedproperty 'MS_Description', 'DepartId 所属部门Id', 'user', dbo, 'table', T_BASE_USER, 'column', BUDepartId;
EXEC sp_addextendedproperty 'MS_Description', 'UserTitle 岗位头衔', 'user', dbo, 'table', T_BASE_USER, 'column', BUTitle;
EXEC sp_addextendedproperty 'MS_Description', 'CreateUserId 创建人Id', 'user', dbo, 'table', T_BASE_USER, 'column', BUCreateUserId;
EXEC sp_addextendedproperty 'MS_Description', 'CreateUserName 创建人姓名', 'user', dbo, 'table', T_BASE_USER, 'column', BUCreateUserName;
EXEC sp_addextendedproperty 'MS_Description', 'CreateTime 创建日期', 'user', dbo, 'table', T_BASE_USER, 'column', BUCreateTime;
EXEC sp_addextendedproperty 'MS_Description', 'OperateUserId 操作人Id', 'user', dbo, 'table', T_BASE_USER, 'column', BUOperateUserId;
EXEC sp_addextendedproperty 'MS_Description', 'OperateUserName 操作人姓名', 'user', dbo, 'table', T_BASE_USER, 'column', BUOperateUserName;
EXEC sp_addextendedproperty 'MS_Description', 'OperateTime 操作时间', 'user', dbo, 'table', T_BASE_USER, 'column', BUOperateTime;
EXEC sp_addextendedproperty 'MS_Description', 'IsValid 有效性（0.无效 1.有效）', 'user', dbo, 'table', T_BASE_USER, 'column', BUIsValid;
