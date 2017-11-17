CREATE DATABASE WarningTraffic
USE WarningTraffic
GO
CREATE TABLE [User](
	email nvarchar(50) PRIMARY KEY,
	[password] nvarchar(50),
	name nvarchar(50),
	phone nvarchar(50),
	[address] nvarchar(60)
)
CREATE TABLE Warning(
	idWarning int IDENTITY PRIMARY KEY,
	[address] nvarchar(100),
	contentWarning nvarchar(100),
	[time] nvarchar(50),
	police nvarchar(50),
	hotline nvarchar(50),
	email nvarchar(50),
	Constraint fk1 FOREIGN KEY  (email) REFERENCES  [User](email),
	lon float,
	lat float
)
CREATE TABLE Law(
	idLaw int IDENTITY PRIMARY KEY,
	nameLaw nvarchar(max),
	contentLaw nvarchar(max),
	condemn nvarchar(max),
	[description] nvarchar(max)
)

insert into [User] values('thanhbuikaka@gmail.com','1234',N'Bùi Thanh Tùng','0999999999',N'Hà Nam')
insert into [User] values('khachhang1@gmail.com','1234',N'Khách hàng 1','0123456789',N'Hà Nội')
Select * from [User]
select * from Warning

Insert into Law values(N'Chở theo 3 người trở lên trên xe',N'Tại Điểm l Khoản 3, Điểm b Khoản 4, Điều 6, Nghị định 171/2013/NĐ-CP quy định xử phạt người điều khiển, người ngồi trên xe mô tô, xe gắn máy (kể cả xe máy điện), các loại xe tương tự xe mô tô và các loại xe tương tự xe gắn máy vi phạm quy tắc giao thông đường bộ',N'Phạt từ 200.000 - 400.000 đồng',N'http://www.doisongphapluat.com/phap-luat/tinh-huong-phap-luat/cho-qua-so-nguoi-quy-dinh-bi-phat-bao-nhieu-tien-a89992.html');
Select * from Law