select nickname,contraseņa,tipoUsuario from usuario

use
_login


select * from usuario

create proc _iniciosesion
@nickname nvarchar (30),
@contraseņa nvarchar (30)
as
select*from usuario where
nickname=@nickname and contraseņa=@contraseņa
go

