select nickname,contrase�a,tipoUsuario from usuario

use
_login


select * from usuario

create proc _iniciosesion
@nickname nvarchar (30),
@contrase�a nvarchar (30)
as
select*from usuario where
nickname=@nickname and contrase�a=@contrase�a
go

