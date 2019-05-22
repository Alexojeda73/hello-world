create proc validar
@usuario varchar (45),
@contraseña varchar(45),
@tipo varchar (45)
as 
select*from usuario
where nickname=@usuario and contraseña=@contraseña and tipoUsuario=@tipo
go