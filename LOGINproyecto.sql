create proc validar
@usuario varchar (45),
@contrase�a varchar(45),
@tipo varchar (45)
as 
select*from usuario
where nickname=@usuario and contrase�a=@contrase�a and tipoUsuario=@tipo
go