create table Users(
Id int primary key identity,
Username varchar(128),
PasswordHash varbinary(max),
PasswordSalt varbinary(max)
)