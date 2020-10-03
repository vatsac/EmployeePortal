create table Users(
Id int primary key identity,
Username varchar(128),
PasswordHash varbinary(max),
PasswordSalt varbinary(max)
)

ALTER TABLE Users
    ADD 
        Gender VARCHAR(128) ,
        DateOfBirth DateTime ,
		KnownAs VARCHAR(128),
		Created DateTime ,
		LastActive DateTime ,
		Introduction VARCHAR(max),
		LookingFor VARCHAR(128),
		Interests VARCHAR(128),
		City VARCHAR(128),
		Country VARCHAR(128);

		 

		create table Photo(
Id int primary key identity,
Url varchar(max),
Description varchar(max),
DateAdded Datetime ,
IsMain bit,
UserId int Not Null
  Constraint FK_UserId Foreign Key(UserId)
  references Users(Id),
)

UPDATE Users SET Gender = 'male',DateOfBirth = '1996-09-07',KnownAs = 'Bhide',Created = '2017-07-07',LastActive = '2017-07-07',Introduction = 'Dolor voluptate enim laborum duis proident commodo sit. Sint cupidatat Lorem ullamco dolor qui proident. Consectetur fugiat ut ipsum ullamco ut magna minim consectetur consectetur. Quis labore nostrud minim dolor.',LookingFor = 'Female',Interests = 'Reprehenderit fugiat Lorem consectetur fugiat minim nulla magna commodo anim esse eu consectetur.',City = 'Inkerman',Country = 'Canada' WHERE Username= 'bhide';
UPDATE Users  SET Gender = 'male',DateOfBirth = '1957-03-07',KnownAs = 'Verma',Created = '2017-08-22',LastActive = '2017-08-22',Introduction = 'Nostrud reprehenderit mollit magna incididunt nulla mollit ad do ullamco ipsum reprehenderit consequat minim. Nisi id est et consequat reprehenderit tempor fugiat aliquip mollit aute consectetur culpa. Ut amet aliqua minim magna. Velit dolore consectetur culpa ea magna nisi eu sit velit.',LookingFor = 'Female',Interests = 'Velit aute elit elit pariatur amet adipisicing sint eiusmod sunt eu ex voluptate ad.',City = 'Loma',Country = 'Paraguay' WHERE Username= 'pratik';
UPDATE Users SET Gender = 'male',DateOfBirth = '1969-08-15',KnownAs = 'Bhusan',Created = '2017-02-17',LastActive = '2017-02-17',Introduction = 'Mollit cillum ex eiusmod magna et dolor sit duis velit pariatur enim anim occaecat. Excepteur excepteur ut dolor nisi consequat mollit quis. Nisi irure quis et id amet aliqua.',LookingFor = 'Male',Interests = 'In non sint ea do veniam sunt duis quis mollit.',City = 'Bynum',Country = 'Pakistan' WHERE Username= 'divya';
UPDATE Users SET Gender = 'male',DateOfBirth = '1984-02-19',KnownAs = 'Kumar',Created = '2017-06-27',LastActive = '2017-06-27',Introduction = 'Aliqua nisi labore tempor adipisicing. Lorem in ut enim quis dolor magna commodo nisi ad voluptate et excepteur aliqua. Ea nostrud velit laborum sit nisi cillum proident nisi.',LookingFor = 'Male',Interests = 'Sit ea id excepteur adipisicing ea excepteur sint ex et amet commodo do voluptate esse.',City = 'Cavalero',Country = 'Viet Nam' WHERE Username= 'shivam';

INSERT INTO Photo(Url,IsMain,Description,UserId) VALUES
('https://randomuser.me/api/portraits/men/90.jpg','true','Irure cupidatat magna aute et dolore aliquip reprehenderit dolor irure.',1)
,('https://randomuser.me/api/portraits/men/96.jpg','true','Magna duis consectetur sit ut commodo non eiusmod.',2)
,('https://randomuser.me/api/portraits/men/4.jpg','true','Cupidatat veniam ea magna cillum velit minim non minim tempor ipsum excepteur.',3)
,('https://randomuser.me/api/portraits/men/76.jpg','true','Amet officia enim pariatur mollit tempor in ut.',4)