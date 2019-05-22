select nickname,contraseña,tipoUsuario from usuario

use
_login


select * from usuario

create proc _iniciosesion
@nickname nvarchar (30),
@contraseña nvarchar (30)
as
select*from usuario where
nickname=@nickname and contraseña=@contraseña
go

