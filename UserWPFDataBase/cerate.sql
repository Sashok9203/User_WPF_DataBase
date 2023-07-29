create database [Users]
go
use [Users]
go


create table [Positions]
(
  [Id] int not null primary key identity(0,1),
  [Position] nvarchar(20) not null unique
);
go


create table [Users]
(
  [Id] int not null primary key identity,
  [Login] nvarchar(56) not null check([Login]<>'') unique,
  [Password] nvarchar(56) not null check([Password]<>''),
  [Phone] nvarchar(16) null check([Phone]<>''),
  [PositionId] int not null references [Positions](Id),
  [Addres] nvarchar(256) not null check([Addres]<>'')
);

go